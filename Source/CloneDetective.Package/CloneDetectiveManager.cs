using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using CloneDetective.CloneReporting;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CloneDetective.Package
{
	public static class CloneDetectiveManager
	{
		public static event EventHandler<EventArgs> CloneDetectiveResultChanged;

		private static CloneReporting.CloneDetective _cloneDetective;
		private static CloneDetectiveResult _cloneDetectiveResult;
		private static Dictionary<IVsTextLines, DocumentInfo> _textLinesToDocInfos = new Dictionary<IVsTextLines, DocumentInfo>();

		public static CloneDetectiveResult CloneDetectiveResult
		{
			get { return _cloneDetectiveResult; }
			private set
			{
				// Because of our internal cache and the existing text marker instances
				// this is not a trivial setter. We have to remove the existing markers,
				// change the result and re-add markers within the open text views.
				// Afterwards we shouldn't forget to notify our listeners about the
				// modification.
				RemoveAllMarkers();
				_cloneDetectiveResult = value;
				AddMarkersToOpenDocuments();
				OnCloneDetectiveResultChanged();
			}
		}

		private static void OnCloneDetectiveResultChanged()
		{
			EventHandler<EventArgs> handler = CloneDetectiveResultChanged;
			if (handler != null)
				handler(null, EventArgs.Empty);
		}

		public static CloneClass GetCloneClass(IVsTextLineMarker marker)
		{
			IVsTextLines textLines;
			ErrorHandler.ThrowOnFailure(marker.GetLineBuffer(out textLines));

			DocumentInfo documentInfo;
			if (_textLinesToDocInfos.TryGetValue(textLines, out documentInfo))
			{
				CloneClass cloneClass;
				if (documentInfo.MarkersToCloneClasses.TryGetValue(marker, out cloneClass))
					return cloneClass;
			}

			return null;
		}

		#region UI Management

		public static void FindClones(CloneClass cloneClass)
		{
			CloneResultsControl cloneResultsControl = VSPackage.Instance.GetToolWindowUserControl<CloneResultsToolWindow, CloneResultsControl>();
			cloneResultsControl.Add(cloneClass);
			VSPackage.Instance.ShowToolWindow<CloneResultsToolWindow>();
		}

		public static void ShowCloneIntersections()
		{
			VSPackage.Instance.ShowToolWindow<CloneIntersectionsToolWindow>();
		}

		#endregion

		#region Event notifications

		internal static void OnSolutionOpened()
		{
			// Try to open an existing clone report.
			string solutionPath = VSPackage.Instance.GetSolutionPath();

			CloneDetectiveResult = CloneDetectiveResult.FromSolutionPath(solutionPath);

			// We do NOT have to do anything else as Visual Studio closes all document
			// windows when a solution is closed. Similarly all document windows are
			// restored when a solution is opened. However this will happen after this
			// event is fired, so our TextManagerEventSink will get all the notifications
			// for the restored windows and insert all text markers as usual.
		}

		internal static void OnSolutionClosed()
		{
			if (_cloneDetective != null)
			{
				// Terminate clone detective if it is still running. We need to disable
				// the events upfront to prevent any unwanted UI changes after we close
				// the result.
				_cloneDetective.DisableEvents();
				_cloneDetective.Abort();
			}

			// Since the clone report is solution specific now it's the right time to
			// close it.
			CloneDetectiveResult = null;
		}

		internal static void OnDocumentOpened(IVsTextLines textLines)
		{
			// A document has been opened. Let's insert clone markers for each clone in
			// the opened file. If there is no clone detective result loaded yet we do
			// not have to insert any markers which means we're done.
			if (!IsCloneReportAvailable)
				return;

			AddMarkersToDocument(textLines);
		}

		internal static void OnDocumentSaved(uint docCookie)
		{
			// The document identified by docCookie has been saved. Now we have to update
			// our internal data structures and persist them to file. This will ensure we
			// will display the same data again when the user closes and reopens the document.

			// If there is currently no clone report available there is nothing to do.
			if (!IsCloneReportAvailable)
				return;

			CloneDetectiveResult cloneDetectiveResult = CloneDetectiveResult;

			// Get the text buffer. We're done if this fails because that means it was not
			// a text document.
			IVsTextLines textLines = GetDocumentTextLines(docCookie);
			if (textLines == null)
				return;

			// Get the SourceFile of the document in our data structure. We're done if this
			// fails because that means the document was not included in clone detection.
			string path = GetDocumentPath(textLines);
			SourceNode sourceNode = cloneDetectiveResult.SourceTree.FindNode(path);
			if (sourceNode == null)
				return;
			SourceFile sourceFile = sourceNode.SourceFile;

			// And we need to be able to map existing clone markers to their corresponding
			// clone classes. If that fails we cannot update any clone information.
			DocumentInfo documentInfo;
			if (!_textLinesToDocInfos.TryGetValue(textLines, out documentInfo))
				return;

			// If the hash of the file didn't match when we opened it, we don't want to save
			// any changes.
			if (!documentInfo.HashMatched)
				return;

			// Store the new line count of the file. Be aware of the fact that the last line
			// is not taken into account if it is empty. Replace the fingerprint of the file
			// with the new one such that we get no problems when opening up the file the
			// next time.
			int lastLineIndex;
			int lastLineLength;
			if (ErrorHandler.Failed(textLines.GetLastLineIndex(out lastLineIndex, out lastLineLength)))
				return;
			sourceFile.Length = lastLineIndex + ((lastLineLength > 0) ? 1 : 0);
			sourceFile.Fingerprint = GetHashFromFile(path);

			// We need to track which source file clone information was modified to be able
			// to update the rollups afterwards.
			HashSet<SourceFile> modifiedSourceFiles = new HashSet<SourceFile>();

			// Clear old clone information of the saved document as we're going to rebuild
			// it from scratch.
			foreach (Clone clone in sourceFile.Clones)
				clone.CloneClass.Clones.Remove(clone);
			sourceFile.Clones.Clear();
			modifiedSourceFiles.Add(sourceFile);

			// Store information about current position of each marker.
			foreach (KeyValuePair<IVsTextLineMarker, CloneClass> markerWithCloneClass in documentInfo.MarkersToCloneClasses)
			{
				// Retrieve the current text span of the marker we just enumerated.
				TextSpan[] span = new TextSpan[1];
				ErrorHandler.ThrowOnFailure(markerWithCloneClass.Key.GetCurrentSpan(span));

				// Create a new clone object and initialize it appropriately.
				Clone clone = new Clone();
				clone.CloneClass = markerWithCloneClass.Value;
				clone.SourceFile = sourceFile;
				clone.StartLine = span[0].iStartLine;
				clone.LineCount = span[0].iEndLine - span[0].iStartLine + 1;

				// Add the clone to the clone class as well as the source file.
				clone.CloneClass.Clones.Add(clone);
				clone.SourceFile.Clones.Add(clone);
			}

			// Remove clone classes with less than two clones.
			List<CloneClass> cloneClasses = cloneDetectiveResult.CloneReport.CloneClasses;
			for (int i = cloneClasses.Count - 1; i >= 0; i--)
			{
				List<Clone> clones = cloneClasses[i].Clones;
				if (clones.Count < 2)
				{
					foreach (Clone clone in clones)
					{
						clone.SourceFile.Clones.Remove(clone);
						modifiedSourceFiles.Add(clone.SourceFile);
					}

					cloneClasses.RemoveAt(i);
				}
			}

			// Save the new clone report to disk.
			string solutionPath = VSPackage.Instance.GetSolutionPath();
			string cloneReportPath = PathHelper.GetCloneReportPath(solutionPath);
			CloneReport.ToFile(cloneReportPath, cloneDetectiveResult.CloneReport);

			// Rollup changes within the SourceTree.
			foreach (SourceFile modifiedSourceFile in modifiedSourceFiles)
			{
				SourceNode modifiedSourceNode = cloneDetectiveResult.SourceTree.FindNode(modifiedSourceFile.Path);
				SourceTree.RecalculateRollups(modifiedSourceNode);
			}

			// Send notification about changed result to listeners.
			OnCloneDetectiveResultChanged();
		}

		internal static void OnDocumentClosed(IVsTextLines textLines)
		{
			// We use the IVsTextLines reference as key in the text marker bookkeeping
			// dictionary. After a document has been closed we have to remove the
			// entries from this dictionary to prevent memory leaks.
			if (_textLinesToDocInfos.ContainsKey(textLines))
				_textLinesToDocInfos.Remove(textLines);
		}

		internal static void OnMarkerInvalidated(IVsTextLineMarker marker)
		{
			// Remove the invalidated marker reference from our dictionary of
			// markers. Since this dictionary is held in another one we have
			// to get the text buffer of the marker first.
			IVsTextLines textLines;
			ErrorHandler.ThrowOnFailure(marker.GetLineBuffer(out textLines));

			DocumentInfo documentInfo;
			if (_textLinesToDocInfos.TryGetValue(textLines, out documentInfo))
			{
				if (documentInfo.MarkersToCloneClasses.ContainsKey(marker))
					documentInfo.MarkersToCloneClasses.Remove(marker);
			}
		}

		#endregion

		#region Clone Detective interaction

		public static bool RunCloneDetective(EventHandler<CloneDetectiveCompletedEventArgs> completedHandler)
		{
			string solutionPath = VSPackage.Instance.GetSolutionPath();

			// Save all modified documents before we start the build. Pay attention
			// to the fact that the user can cancel this operation.
			IVsSolutionBuildManager2 solutionBuildManager = (IVsSolutionBuildManager2) VSPackage.Instance.GetService(typeof(SVsSolutionBuildManager));
			int hr = solutionBuildManager.SaveDocumentsBeforeBuild(null, 0, 0);
			if (hr == VSConstants.E_ABORT)
				return false;

			VSPackage.Instance.ClearOutput();

			_cloneDetective = new CloneReporting.CloneDetective(solutionPath);
			_cloneDetective.Started += (sender, e) => WriteStartedMessage(e);
			_cloneDetective.Message += (sender, e) => WriteOutputMessage(e);
			_cloneDetective.Completed += (sender, e) =>
			                                   	{
													WriteCompletedMessage(e);

			                                   		_cloneDetective = null;

			                                   		if (e.Result != null)
			                                   			CloneDetectiveResult = e.Result;

			                                   		completedHandler(sender, e);
			                                   	};

			// Get configuration options.
			CloneDetectiveOptionPage optionPage = VSPackage.Instance.GetOptionPage();

			// Validate path to ConQAT.bat
			if (!File.Exists(optionPage.ConqatFileName))
			{
				VSPackage.Instance.ShowError(Res.InvalidConqatBatPath);
				return false;
			}

			// Validate Java Home.
			if (!File.Exists(Path.Combine(Path.Combine(optionPage.JavaHome, "bin"), "Java.exe")))
			{
				VSPackage.Instance.ShowError(Res.InvalidJavaExePath);
				return false;
			}

			// Now run the clone detective.
			_cloneDetective.RunAsync();

			return true;
		}

		private static void WriteStartedMessage(CloneDetectiveStartedEventArgs e)
		{
			VSPackage.Instance.WriteOutputLine(Res.OutputHeaderStarted);
			VSPackage.Instance.WriteOutputLine(e.Program + " " + e.Arguments);
			VSPackage.Instance.WriteOutputLine();
		}

		private static void WriteOutputMessage(CloneDetectiveMessageEventArgs e)
		{
			VSPackage.Instance.WriteOutputLine(e.Message);
		}

		private static void WriteCompletedMessage(CloneDetectiveCompletedEventArgs e)
		{
			string message;

			switch (e.Result.Status)
			{
				case CloneDetectiveResultStatus.Succeeded:
					message = Res.OutputFooterSucceeded;
					break;
				case CloneDetectiveResultStatus.Failed:
					message = Res.OutputFooterFailed;
					break;
				case CloneDetectiveResultStatus.Stopped:
					message = Res.OutputFooterStopped;
					break;
				default:
					throw ExceptionBuilder.UnhandledCaseLabel(e.Result.Status);
			}

			VSPackage.Instance.WriteOutputLine(message);
		}

		public static void AbortCloneDetective()
		{
			if (_cloneDetective != null)
				_cloneDetective.Abort();
		}

		public static bool IsCloneDetectiveRunning
		{
			get { return _cloneDetective != null; }
		}

		public static bool IsCloneReportAvailable
		{
			get
			{
				return CloneDetectiveResult != null &&
					   CloneDetectiveResult.Status == CloneDetectiveResultStatus.Succeeded;
			}
		}

		public static string ConqatLogFileName
		{
			get
			{
				string solutionPath = VSPackage.Instance.GetSolutionPath();
				if (solutionPath == null)
					return null;

				return PathHelper.GetLogPath(solutionPath);
			}
		}

		public static void ExportCloneDetectiveResults(string fileName)
		{
			string solutionPath = VSPackage.Instance.GetSolutionPath();
			if (solutionPath == null)
				return;

			string cloneReportPath = PathHelper.GetCloneReportPath(solutionPath);
			File.Copy(cloneReportPath, fileName, true);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static void ImportCloneDetectiveResults(string fileName)
		{
			string solutionPath = VSPackage.Instance.GetSolutionPath();
			if (solutionPath == null)
				return;

			string logPath = PathHelper.GetLogPath(solutionPath);
			string cloneReportPath = PathHelper.GetCloneReportPath(solutionPath);

			using (StreamWriter logWriter = new StreamWriter(logPath))
			{
				try
				{
					logWriter.WriteLine("Opening clone report from '{0}'...", fileName);
					CloneReport cloneReport = CloneReport.FromFile(fileName);

					string oldRootPath = GetCommonRoot(cloneReport);
					string newRootPath = AskUserForNewRootPath(cloneReport, oldRootPath, Path.GetDirectoryName(solutionPath));
					if (newRootPath == null)
					{
						logWriter.WriteLine("User aborted import.");
						return;
					}

					logWriter.WriteLine("Remapping paths from '{0}' to '{1}'...", oldRootPath, newRootPath);
					RemapPaths(cloneReport, oldRootPath, newRootPath);

					logWriter.WriteLine("Saving clone report to '{0}'...", cloneReportPath);
					CloneReport.ToFile(cloneReportPath, cloneReport);

					logWriter.WriteLine("Import of '{0}' succeeded.", fileName);
					LogHelper.WriteStatusInfo(logWriter, CloneDetectiveResultStatus.Succeeded, 0, TimeSpan.Zero);
				}
				catch (Exception ex)
				{
					LogHelper.WriteError(logWriter, ex);
					LogHelper.WriteStatusInfo(logWriter, CloneDetectiveResultStatus.Failed, 0, TimeSpan.Zero);
				}
			}

			CloneDetectiveResult = CloneDetectiveResult.FromSolutionPath(solutionPath);
		}

		public static void CloseCloneDetectiveResults()
		{
			string solutionPath = VSPackage.Instance.GetSolutionPath();
			if (solutionPath == null)
				return;

			string logPath = PathHelper.GetLogPath(solutionPath);
			string cloneReportPath = PathHelper.GetCloneReportPath(solutionPath);
			File.Delete(logPath);
			File.Delete(cloneReportPath);

			CloneDetectiveResult = CloneDetectiveResult.FromSolutionPath(solutionPath);
		}

		private static string AskUserForNewRootPath(CloneReport cloneReport, string oldRootPath, string newRootPath)
		{
			string firstNonMappedFile;
			do
			{
				using (FolderBrowserDialog dlg = new FolderBrowserDialog())
				{
					dlg.Description = String.Format(CultureInfo.CurrentCulture, Res.ImportCloneReportSelectFolder, oldRootPath);
					dlg.SelectedPath = newRootPath;
					dlg.ShowNewFolderButton = false;
					if (dlg.ShowDialog() != DialogResult.OK)
						return null;

					newRootPath = dlg.SelectedPath;
				}
				firstNonMappedFile = FindFirstNonMappedFileName(cloneReport, oldRootPath, newRootPath);

				if (firstNonMappedFile != null)
				{
					string message = String.Format(CultureInfo.CurrentCulture, Res.ImportCloneReportIncorrectMapping, firstNonMappedFile, RemapPath(oldRootPath, newRootPath, firstNonMappedFile));
					VSPackage.Instance.ShowError(message);
				}
			} while (firstNonMappedFile != null);

			return newRootPath;
		}

		private static string GetCommonRoot(CloneReport cloneReport)
		{
			string rootPath = null;
			foreach (SourceFile sourceFile in cloneReport.SourceFiles)
			{
				string currentFolder = Path.GetDirectoryName(sourceFile.Path);
				if (rootPath == null)
					rootPath = currentFolder;
				else
				{
					while (!currentFolder.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
						rootPath = Path.GetDirectoryName(rootPath);
				}
			}
			return rootPath;
		}

		private static string RemapPath(string oldRootPath, string newRootPath, string fileName)
		{
			return newRootPath + fileName.Substring(oldRootPath.Length);
		}

		private static void RemapPaths(CloneReport cloneReport, string oldRootPath, string newRootPath)
		{
			foreach (SourceFile sourceFile in cloneReport.SourceFiles)
				sourceFile.Path = RemapPath(oldRootPath, newRootPath, sourceFile.Path);
		}

		private static string FindFirstNonMappedFileName(CloneReport cloneReport, string oldRootPath, string newRootPath)
		{
			foreach (SourceFile sourceFile in cloneReport.SourceFiles)
			{
				string newFilenName = RemapPath(oldRootPath, newRootPath, sourceFile.Path);
				if (!File.Exists(newFilenName))
					return sourceFile.Path;
			}

			return null;
		}

		#endregion

		#region Helper methods

		private static IVsTextLines GetDocumentTextLines(uint docCookie)
		{
			// Get an instance of the running document table service (RDT).
			IVsRunningDocumentTable rdt = (IVsRunningDocumentTable) VSPackage.Instance.GetService(typeof(SVsRunningDocumentTable));

			IntPtr unkDocData = IntPtr.Zero;
			try
			{
				// Get the information about document docCookie from the RDT.
				uint flags;
				uint readLocks;
				uint editLocks;
				string mkDocument;
				IVsHierarchy hierarchy;
				uint itemId;
				ErrorHandler.ThrowOnFailure(rdt.GetDocumentInfo(docCookie,
				                                                out flags, out readLocks, out editLocks, out mkDocument,
				                                                out hierarchy, out itemId, out unkDocData));

				// unkDocData points to additional information about the document.
				// In the case of a text file this is an object that implements IVsTextLines.
				IVsTextLines lines = Marshal.GetObjectForIUnknown(unkDocData) as IVsTextLines;
				return lines;
			}
			finally
			{
				// The GetDocumentInfo method often returns information in the
				// unkDocData parameter. Since the type is IntPtr the .NET runtime
				// only knows it's some kind of pointer. But we know that it is a
				// pointer to a COM IUnknown interface instance which has to be
				// freed separately by calling the Release method.
				if (unkDocData != IntPtr.Zero)
					Marshal.Release(unkDocData);
			}
		}

		private static string GetDocumentPath(IVsTextLines textLines)
		{
			// The path to the document of the IVsTextLines text buffer is used
			// as the moniker of the corresponding IVsUserData interface implementation.
			object fileName = null;

			IVsUserData userData = textLines as IVsUserData;
			if (userData != null)
			{
				// Read the moniker (which is the file name) from userData.
				Guid monikerGuid = typeof(IVsUserData).GUID;
				ErrorHandler.Ignore(userData.GetData(ref monikerGuid, out fileName));
			}

			return (string) fileName;
		}

		private static void RemoveAllMarkers()
		{
			// Invalidate all markers in all text views. Ignore any errors.
			foreach (KeyValuePair<IVsTextLines, DocumentInfo> linesToDocInfo in _textLinesToDocInfos)
			{
				IVsTextLineMarker[] markers = new IVsTextLineMarker[linesToDocInfo.Value.MarkersToCloneClasses.Count];
				linesToDocInfo.Value.MarkersToCloneClasses.Keys.CopyTo(markers, 0);

				foreach (IVsTextLineMarker marker in markers)
					ErrorHandler.Ignore(marker.Invalidate());
			}
		}

		private static void AddMarkersToOpenDocuments()
		{
			// If there is no clone detective result loaded we're already done.
			if (!IsCloneReportAvailable)
				return;

			// Loop through all open documents and insert a text marker for each
			// clone in an open document.

			// Get an enumerator of all open documents.
			IEnumRunningDocuments runningDocumentsEnum;
			IVsRunningDocumentTable rdt = (IVsRunningDocumentTable) VSPackage.Instance.GetService(typeof(SVsRunningDocumentTable));
			ErrorHandler.ThrowOnFailure(rdt.GetRunningDocumentsEnum(out runningDocumentsEnum));

			while (true)
			{
				// Move enumerator to next document if there is one.
				uint[] docCookies = new uint[1];
				uint fetched;
				ErrorHandler.ThrowOnFailure(runningDocumentsEnum.Next(1, docCookies, out fetched));
				if (fetched < 1)
					break;

				// Look up the text buffer of the document.
				IVsTextLines textLines = GetDocumentTextLines(docCookies[0]);
				if (textLines != null)
					AddMarkersToDocument(textLines);
			}
		}

		private static void AddMarkersToDocument(IVsTextLines textLines)
		{
			// Find the node of the opened document in our clone detective source tree.
			// There might be no such node if the file has been added or moved in the
			// meantime.
			string path = GetDocumentPath(textLines);
			SourceNode sourceNode = CloneDetectiveResult.SourceTree.FindNode(path);
			if (sourceNode == null)
				return;

			// Create a dictionary for bookkeeping of the text markers we're going to
			// insert.
			DocumentInfo documentInfo;
			if (!_textLinesToDocInfos.TryGetValue(textLines, out documentInfo))
			{
				documentInfo = new DocumentInfo();
				_textLinesToDocInfos.Add(textLines, documentInfo);
			}

			// Validate MD5 hash. We're done if it doesn't match.
			documentInfo.HashMatched = ValidateHash(textLines, sourceNode.SourceFile.Fingerprint);
			if (!documentInfo.HashMatched)
				return;

			// Create line markers for each clone.
			foreach (Clone clone in sourceNode.SourceFile.Clones)
				AddMarker(documentInfo, textLines, clone);
		}

		private static void AddMarker(DocumentInfo documentInfo, IVsTextLines textLines, Clone clone)
		{
			// The line marker should span the text range from the first character
			// in the first line to the last character in the last line. So we need
			// to retrieve the length of the last line. If that is not possible
			// we simply ignore the clone.
			int firstLine = clone.StartLine;
			int lastLine = clone.StartLine + clone.LineCount - 1;
			int lastLineLength;

			if (ErrorHandler.Failed(textLines.GetLengthOfLine(lastLine, out lastLineLength)))
				return;

			// Finally create the clone marker and store it in our dictionary.
			// Ignore any errors.
			IVsTextLineMarker[] markers = new IVsTextLineMarker[1];
			TextMarkerClientEventSink clientEventSink = new TextMarkerClientEventSink();
			int hr = textLines.CreateLineMarker(CloneBackgroundMarkerType.Id,
			                                    firstLine, 0,
			                                    lastLine, lastLineLength - 1,
			                                    clientEventSink, markers);
			if (ErrorHandler.Succeeded(hr))
			{
				clientEventSink.Marker = markers[0];
				documentInfo.MarkersToCloneClasses.Add(markers[0], clone.CloneClass);
			}
		}

		#endregion

		#region MD5 hash helper methods

		private static bool ValidateHash(IVsTextLines textLines, string expectedHash)
		{
			// We can't get the content of the text buffer from an IVsTextLines instance.
			// We need an IVsTextStream instance instead.
			IVsTextStream textStream = textLines as IVsTextStream;
			if (textStream == null)
				return false;

			// Before retrieving the content we need to allocate a buffer that is big
			// enough to hold all the text. So query the text buffer for the content
			// length first.
			int length;
			int hr = textStream.GetSize(out length);
			if (ErrorHandler.Failed(hr))
				return false;

			// Allocate a native OLE buffer for the text and the trailing termination
			// character. Don't forget to always free it!
			string text;
			int neededNativeBufferSize = (length + 1) * Marshal.SystemDefaultCharSize;
			IntPtr nativeBuffer = Marshal.AllocCoTaskMem(neededNativeBufferSize);
			try
			{
				// Get the content of the text buffer.
				hr = textStream.GetStream(0, length, nativeBuffer);
				if (ErrorHandler.Failed(hr))
					return false;

				// Marshal the text from the unmanaged OLE memory region to a .NET
				// string object.
				text = Marshal.PtrToStringAuto(nativeBuffer);
			}
			finally
			{
				if (nativeBuffer != IntPtr.Zero)
					Marshal.FreeCoTaskMem(nativeBuffer);
			}

			// Now compute the hash of the text in the buffer. If it equals the expected
			// hash we're successful.
			byte[] content = Encoding.Default.GetBytes(text);
			string hash1 = GetHash(content);
			if (String.Compare(expectedHash, hash1, StringComparison.OrdinalIgnoreCase) == 0)
				return true;

			// The Unicode Byte-Order Mark is interpreted correctly by the text editor.
			// However it does not exist in the text buffer (and of course it shouldn't).
			// To be able to open UTF-8 files we add the corresponding BOM to the content.
			byte[] contentWithBom = new byte[content.Length + 3];
			contentWithBom[0] = 0xEF;
			contentWithBom[1] = 0xBB;
			contentWithBom[2] = 0xBF;
			Array.Copy(content, 0, contentWithBom, 3, content.Length);

			// Let's see whether the hash matches a file with UTF-8 BOM.
			string hash2 = GetHash(contentWithBom);
			return String.Compare(expectedHash, hash2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private static string GetHashFromFile(string path)
		{
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				byte[] hash = MD5.Create().ComputeHash(stream);
				return GetHashAsString(hash);
			}
		}

		private static string GetHash(byte[] content)
		{
			byte[] hash = MD5.Create().ComputeHash(content);
			return GetHashAsString(hash);
		}

		private static string GetHashAsString(byte[] hash)
		{
			StringBuilder sb = new StringBuilder(32);
			foreach (byte b in hash)
				sb.AppendFormat("{0:x2}", b);

			return sb.ToString();
		}

		#endregion

		private sealed class DocumentInfo
		{
			public bool HashMatched;
			public Dictionary<IVsTextLineMarker, CloneClass> MarkersToCloneClasses = new Dictionary<IVsTextLineMarker, CloneClass>();
		}
	}
}
