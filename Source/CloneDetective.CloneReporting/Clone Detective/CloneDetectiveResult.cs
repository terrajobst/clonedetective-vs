using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class acts as container that groups all the artifacts returned
	/// by <see cref="CloneDetective"/>.
	/// </summary>
	public sealed class CloneDetectiveResult
	{
		private CloneReport _cloneReport;
		private SourceTree _sourceTree;
		private CloneDetectiveResultStatus _status;
		private TimeSpan _usedTime;
		private long _usedMemory;
		private Dictionary<string, CloneClass> _cloneClassFingerprintDictionary;

		private CloneDetectiveResult()
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether the clone detection was
		/// successful, failed, or stopped.
		/// </summary>
		public CloneDetectiveResultStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		/// <summary>
		/// Gets or sets the associated <see cref="CloneReporting.CloneReport"/>.
		/// </summary>
		public CloneReport CloneReport
		{
			get { return _cloneReport; }
			set { _cloneReport = value; }
		}

		/// <summary>
		/// Gets or sets the associated <see cref="CloneReporting.SourceTree"/>.
		/// </summary>
		public SourceTree SourceTree
		{
			get { return _sourceTree; }
			set { _sourceTree = value; }
		}

		/// <summary>
		/// Gets or sets the time clone detective needed to produce the result.
		/// </summary>
		public TimeSpan UsedTime
		{
			get { return _usedTime; }
			set { _usedTime = value; }
		}

		/// <summary>
		/// Gets or sets the amount of memory clone detective needed.
		/// </summary>
		public long UsedMemory
		{
			get { return _usedMemory; }
			set { _usedMemory = value; }
		}

		/// <summary>
		/// Loads a <see cref="CloneDetectiveResult"/> from a Visual Studio solution given by
		/// <paramref name="solutionPath"/>.
		/// </summary>
		/// <param name="solutionPath">The path to the Visual Studio solution (including the filename).</param>
		/// <returns>
		/// If the solution does not contain a clone detective result the return value is
		/// <see langword="null"/>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static CloneDetectiveResult FromSolutionPath(string solutionPath)
		{
			// Get path to log file.
			string logPath = PathHelper.GetLogPath(solutionPath);
			string cloneReportPath = PathHelper.GetCloneReportPath(solutionPath);

			// If the log file does not exist no clone detective result can be
			// constructed (if only the clone report is missing it can).
			if (!File.Exists(logPath))
				return null;

			// Construct the clone detective result.
			CloneDetectiveResult result = new CloneDetectiveResult();

			if (File.Exists(cloneReportPath))
			{
				try
				{
					// We have a clone report. Parse it and construct the source tree from it.
					result.CloneReport = CloneReport.FromFile(cloneReportPath);
					result.SourceTree = SourceTree.FromCloneReport(result.CloneReport, solutionPath);
				}
				catch (Exception ex)
				{
					// If we could not parse the clone report we write an error to the
					// log file (which we will parse below).
					result.CloneReport = null;
					result.SourceTree = null;
					LogHelper.WriteError(logPath, ex);
					LogHelper.WriteStatusInfo(logPath, CloneDetectiveResultStatus.Failed, 0, TimeSpan.Zero);
				}
			}

			// Parse the summary information out of the log file.
			CloneDetectiveResultStatus status;
			long usedMemory;
			TimeSpan usedTime;
			LogHelper.ParseStatusInfo(logPath, out status, out usedMemory, out usedTime);

			result.Status = status;
			result.UsedMemory = usedMemory;
			result.UsedTime = usedTime;

			return result;
		}

		/// <summary>
		/// Finds a clone class by a given <paramref name="fingerprint"/>.
		/// </summary>
		/// <param name="fingerprint">The fingerprint of the clone class to search.</param>
		/// <returns>
		/// If no clone class matches the given <paramref name="fingerprint"/> the return value
		/// is <see langword="null"/>. If more than one clone class matches <paramref name="fingerprint"/>
		/// the return value is arbitrary.
		/// </returns>
		public CloneClass FindCloneClass(string fingerprint)
		{
			// Check if we already setup a dictionary for mapping clone class fingerprints.
			if (_cloneClassFingerprintDictionary == null)
			{
				// No, we have not.
				_cloneClassFingerprintDictionary = new Dictionary<string, CloneClass>();
				foreach (CloneClass cloneClass in _cloneReport.CloneClasses)
				{
					// NOTE: Since fingerprints only represent a hash they are by-design
					//       not unique. Therefore we cannot use Dictionary.Add() here.
					_cloneClassFingerprintDictionary[cloneClass.Fingerprint] = cloneClass;
				}
			}

			// Get the clone class by the given fingerprint.
			CloneClass result;
			_cloneClassFingerprintDictionary.TryGetValue(fingerprint, out result);
			return result;
		}
	}
}
