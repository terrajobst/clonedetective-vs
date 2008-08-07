using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class encapsulates the asynchronous execution of Clone Detective.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class CloneDetectiveRunner
	{
		private SolutionSettings _solutionSettings = new SolutionSettings();
		private string _conqatLogFileName;
		private string _cloneReportFileName;
		private bool _running;
		private Process _process;
		private TimeSpan _usedTime;
		private long _usedMemory;
		private bool _aborted;

		/// <summary>
		/// Creates a new instance of <see cref="CloneDetectiveRunner"/> by the given
		/// solution file name.
		/// </summary>
		/// <param name="solutionFileName">The fully qualified path to the Visual Studio solution.</param>
		/// <remarks>
		/// <para>The path to the solution settings as well as the log file are derived from the
		/// solution name.</para>
		/// </remarks>
		public CloneDetectiveRunner(string solutionFileName)
		{
			// First we load settings in the expanded form, i.e. all macros and
			// flags are expanded in order to create the effective settings.
			_solutionSettings.LoadAndExpand(solutionFileName);

			// Derive solution specific log and clone report file names.
			_conqatLogFileName = PathHelper.GetLogPath(solutionFileName);
			_cloneReportFileName = PathHelper.GetCloneReportPath(solutionFileName);
		}

		/// <summary>
		/// Starts Clone Detective by spawning a new process that runs ConQAT.
		/// </summary>
		private void RunConQAT(Action<string> outputHandler)
		{
			// NOTE: We use a field instead of a local variable because we need a reference
			//       to the spawned process in order to be able to terminate it in Abort().

			using (_process = new Process())
			{
				// Start new stop watch to measure the time Clone Detective needed.
				Stopwatch stopwatch = Stopwatch.StartNew();

				// We will use the directory of the analysis file as the working directory.
				// Normally, ConQAT also creates additional files in working directory (such as
				// the log4j log file). This makes sure the file is in a location the user expects.
				// If the analysis is the default analysis file (which resides under %ProgramFiles%) 
				// ConQAT will not have write permission in the working folder but log4j will just
				// ignore this.
				string workingDirectory = Path.GetDirectoryName(_solutionSettings.AnalysisFileName);

				// Build the command line arguments string.
				string arguments = BuildArgumentsString(_solutionSettings);

				// Read global settings
				string conqatBatFileName = GlobalSettings.GetConqatBatFileName();
				string javaHome = GlobalSettings.GetJavaHome();

				// Setup the process. Make sure the process starts in a hidden window.
				_process.StartInfo.FileName = conqatBatFileName;
				_process.StartInfo.WorkingDirectory = workingDirectory;
				_process.StartInfo.Arguments = arguments;
				_process.StartInfo.UseShellExecute = false;
				_process.StartInfo.CreateNoWindow = true;
				_process.StartInfo.EnvironmentVariables["PATH"] = PrependPathWithJavaBinFolder(_process.StartInfo.EnvironmentVariables["PATH"], javaHome);
				_process.StartInfo.EnvironmentVariables["JAVA_HOME"] = javaHome;

				// Notify start handler that we are going to start clone detection.
				EventHandler<CloneDetectiveStartedEventArgs> started = Started;
				if (started != null)
					started(this, new CloneDetectiveStartedEventArgs(_process.StartInfo.FileName, _process.StartInfo.Arguments));

				// NOTE: We will not use a commmand line redirect to create the log file.
				//       Instead we manually redirect the error and standard ouput streams
				//       in order to be able to progressively report progress to our host.
				//       Since we also want it to be written to a log file we write the
				//       received output to our log file as well.
				using (StreamWriter sw = new StreamWriter(_conqatLogFileName, false))
				{
					DataReceivedEventHandler dataReceivedHandler = (sender, e) =>
					                                               	{
																		// At the end of the stream we will get a null value.
																		// We will just ignore that.
																		if (e.Data == null)
																			return;

					                                               		sw.WriteLine(e.Data);
					                                               		outputHandler(e.Data);
					                                               	};

					_process.StartInfo.RedirectStandardError = true;
					_process.StartInfo.RedirectStandardOutput = true;
					_process.ErrorDataReceived += dataReceivedHandler;
					_process.OutputDataReceived += dataReceivedHandler;

					// Run ConQAT.
					_process.Start();

					// Read output and wait for completion.
					_process.BeginErrorReadLine();
					_process.BeginOutputReadLine();
					_process.WaitForExit();
				}

				// Get run time.
				_usedTime = stopwatch.Elapsed;

				// Get memory consumption.
				_usedMemory = GetMemory(_conqatLogFileName);
			}
		}

		/// <summary>
		/// Prepends the given path by the Java bin folder of the given Java home directory.
		/// </summary>
		/// <param name="path">The path to be prefixed.</param>
		/// <param name="javaHome">The fully qualified path of the Jave home directory.</param>
		/// <returns></returns>
		private static string PrependPathWithJavaBinFolder(string path, string javaHome)
		{
			string javaBinFolder = Path.Combine(javaHome, "bin");
			return javaBinFolder + Path.PathSeparator + path;
		}

		/// <summary>
		/// Builds a string representing the command line arguments for ConQAT.
		/// </summary>
		private static string BuildArgumentsString(SolutionSettings solutionSettings)
		{
			StringBuilder sb = new StringBuilder();

			// Set the name of the config file.
			sb.Append("-f \"");
			sb.Append(solutionSettings.AnalysisFileName);
			sb.Append("\"");

			// TODO: Make path to properties file configurable.
			string propertiesFilePath = PathHelper.GetPropertiesFilePath(solutionSettings.AnalysisFileName);
			if (File.Exists(propertiesFilePath))
			{
				sb.Append("-s \"");
				sb.Append(propertiesFilePath);
				sb.Append("\"");
			}

			// Add all property overrides by -p flag.
			foreach (KeyValuePair<string, string> propertyOverride in solutionSettings.PropertyOverrides)
			{
				sb.Append(" -p \"");
				sb.Append(propertyOverride.Key);
				sb.Append("=");
				sb.Append(propertyOverride.Value);

				if (!String.IsNullOrEmpty(propertyOverride.Value))
				{
					// If the last character is a backslash we need to add another one. Otherwise the trailing
					// backslash together with the quote would be interpreted as an escaped quote.
					bool lastCharacterIsBackslash = (propertyOverride.Value[propertyOverride.Value.Length - 1] == '\\');
					if (lastCharacterIsBackslash)
						sb.Append("\\");
				}

				sb.Append("\"");
			}


			return sb.ToString();
		}

		/// <summary>
		/// Reads the memory usage from the log file by using a simple regular expression.
		/// </summary>
		/// <param name="tempConqatLogFileName">The path and file name of the ConQAT log file.</param>
		/// <returns>
		/// If the "Max memory: XXXX" output cannot be found in the log file the return value is zero.
		/// Otherwise the return value is the maximum memory consumption in bytes.
		/// </returns>
		private static long GetMemory(string tempConqatLogFileName)
		{
			Regex regex = new Regex(@"Max memory\: (?<Number>[0-9,.]+)kB");
			long result = 0;

			foreach (string message in File.ReadAllLines(tempConqatLogFileName))
			{
				if (message != null)
				{
					Match match = regex.Match(message);
					if (match.Success)
					{
						string number = match.Groups["Number"].Value;
						// NOTE: ConQAT writes the number using the system's current locale. Since they include
						//       the culture dependent thousand marker we have to explicitly specify the
						//       NumberStyles.AllowThousands and use the current culture (NOT CurrentUICulture).
						result = Int64.Parse(number, NumberStyles.AllowThousands, CultureInfo.CurrentCulture)*1024;

						// Don't break here. This will ensure we will use the last entry in the log file that
						// matches the pattern.
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Runs Clone Detective asynchronously.
		/// </summary>
		/// <remarks>
		/// To get the Clone Detection results you must register a handler for the <see cref="Completed"/>
		/// event.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public void RunAsync()
		{
			if (_running)
				throw ExceptionBuilder.CloneDetectiveAlreadyRunning();

			// Make sure our internal state is updated first.
			_running = true;
			_aborted = false;
			
			// Time for multi-threading.
			//
			// For synchronization purposes we use the .NET 2.0 new SynchronizationContext.
			// This way we can write code that runs on the correct thread context without
			// binding to a concrete UI technology (such as Windows Forms, ASP.NET etc.)

			// Get context for current thread. This will typically be the main UI thread.
			SynchronizationContext ctx = SynchronizationContext.Current;

			// Now we will queue a delegate that will be run on a worker thread.
			ThreadPool.QueueUserWorkItem(
				delegate
					{
						// !! WORKER THREAD CONTEXT !!

						Exception exception = null;
						try
						{
							// Make sure the directory exists
							string cloneReportDirectory = Path.GetDirectoryName(_solutionSettings.CloneReportFileName);
							if (!Directory.Exists(cloneReportDirectory))
								Directory.CreateDirectory(cloneReportDirectory);

							// First we delete the old clone report
							File.Delete(_solutionSettings.CloneReportFileName);

							// Create output lambda that is used by RunConQAT.
							Action<string> outputHandler;
							EventHandler<CloneDetectiveMessageEventArgs> messageHandler = Message;
							if (messageHandler == null)
								outputHandler = s => { };
							else
								outputHandler = (s => ctx.Send(delegate
								                               	{
								                               		CloneDetectiveMessageEventArgs args = new CloneDetectiveMessageEventArgs(s);
								                               		messageHandler(this, args);
								                               	}, null));

							// Run ConQAT and wait for completion.
							RunConQAT(outputHandler);

							// Check if ConQAT exited normally or we killed it using Abort().
							if (!_aborted)
							{
								// OK, ConQAT has run to completion.
								//
								// Check if ConQAT produced a clone report.
								//
								// If this is not the case then ConQAT failed for some reason.
								if (!File.Exists(_solutionSettings.CloneReportFileName) || (new FileInfo(_solutionSettings.CloneReportFileName)).Length == 0)
									throw ExceptionBuilder.ConqatDidNotProduceCloneReport();

								// Copy the clone report file to our solution clone report file (overwriting any existing one).
								try
								{
									File.Copy(_solutionSettings.CloneReportFileName, _cloneReportFileName, true);
								}
								catch (IOException)
								{
									// If the _solutionSettings.CloneReportFileName points to the same file
									// as _cloneReportFileName we obviously cannot copy the file. However,
									// in .NET there is no simple way to check whether to given paths point
									// to the same file (remember that there are many ways to express file paths:
									// mapped drives, relative paths, special characters, etc.)
									//
									// The safest way is to just try to copy and in case of a failure just ignore
									// the error.
								}
							}
						}
						catch (Exception ex)
						{
							// Just record the exception, we will handle it below.
							exception = ex;
						}

						// OK, here ConQAT has either run successully, failed or was killed by the Abort() method.
						// Depending on what our internal state is we will update the solution log file.

						if (exception != null)
						{
							// ConQAT failed to produce a clone report.
							LogHelper.WriteError(_conqatLogFileName, exception);
							LogHelper.WriteStatusInfo(_conqatLogFileName, CloneDetectiveResultStatus.Failed, _usedMemory, _usedTime);
						}
						else if (_aborted)
						{
							// ConQAT was aborted.
							LogHelper.WriteStatusInfo(_conqatLogFileName, CloneDetectiveResultStatus.Stopped, _usedMemory, _usedTime);
						}
						else
						{
							// ConQAT actually succeeded.
							LogHelper.WriteStatusInfo(_conqatLogFileName, CloneDetectiveResultStatus.Succeeded, _usedMemory, _usedTime);
						}

						// Here we are still in the worker thread context. The completion event must be executed in
						// the same thread context as caller of RunAsync(). To change the thread context we use the
						// stored synchronization context.
						ctx.Send(delegate
						{
							// !! MAIN THREAD CONTEXT !!

							// Back to main thread. From here we can safely update our internal state and invoke the
							// completion event.

							// Reset process and running flag.
							_process = null;
							_running = false;

							// Load clone detective result and notify event subscribers (if any).
							CloneDetectiveResult cloneDetectiveResult = CloneDetectiveResult.FromSolutionPath(_solutionSettings.SolutionFileName);
							EventHandler<CloneDetectiveCompletedEventArgs> handler = Completed;
							if (handler != null)
							{
								CloneDetectiveCompletedEventArgs eventArgs = new CloneDetectiveCompletedEventArgs(cloneDetectiveResult, exception);
								handler(this, eventArgs);
							}
						}, null);
					}
				);
		}

		/// <summary>
		/// Aborts Clone Detetive. If Clone Detective is not running this method does nothing.
		/// </summary>
		public void Abort()
		{
			if (!_running)
				return;

			_aborted = true;

			// Since ConQAT is started via a batch file we cannot simply
			// call _process.Kill() since this would only stop the batch
			// file -- not any task started from the batch file.
			//
			// Therefore we use a windows command line utility to terminate
			// the whole process tree.
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = "taskkill.exe";
					process.StartInfo.Arguments = String.Format(CultureInfo.InvariantCulture, "/PID {0} /T /F", _process.Id);
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.Start();
					process.WaitForExit();
				}
			}
			catch (InvalidOperationException)
			{
				// This exception is thrown by the Process.Id property if
				// the process finished normally in the meantime. We cannot
				// simply check this before without introducing a race condition.
			}
		}

		/// <summary>
		/// This method disables all <see cref="Started"/>, <see cref="Message"/>, and <see cref="Completed"/>
		/// events by clearing their event handler lists.
		/// </summary>
		/// <remarks>
		/// This is useful if the host application is going to be shut down
		/// and Clone Detective is still running. In this case you should first
		/// call <see cref="DisableEvents"/> and then <see cref="Abort"/>.
		/// </remarks>
		public void DisableEvents()
		{
			Started = null;
			Message = null;
			Completed = null;
		}

		/// <summary>
		/// Gets a value indicating whether Clone Detective is still running.
		/// </summary>
		public bool IsRunning
		{
			get { return _running; }
		}

		/// <summary>
		/// Adds or removes a handler for the started notification.
		/// </summary>
		/// <note>The handler will be executed in the context of the thread that called
		/// <see cref="RunAsync"/> (typically the main UI thread) as opposed to the thread
		/// that is running Clone Detective. That means no synchronization is required by
		/// handler itself.</note>
		public event EventHandler<CloneDetectiveStartedEventArgs> Started;

		/// <summary>
		/// Adds or removes a handler for a message notification.
		/// </summary>
		/// <remarks>
		/// <para>This event can be raised mutliple times by clone detection.
		/// Each message represents an output line of its own.</para>
		/// <note>The handler will be executed in the context of the thread that called
		/// <see cref="RunAsync"/> (typically the main UI thread) as opposed to the thread
		/// that is running Clone Detective. That means no synchronization is required by
		/// handler itself.</note>
		/// </remarks>
		public event EventHandler<CloneDetectiveMessageEventArgs> Message;

		/// <summary>
		/// Adds or removes a handler for the completion notification.
		/// </summary>
		/// <remarks>
		/// <para>The <see cref="Completed"/> handler will be raised regardless whether Clone
		/// Detective succeeded, failed or was aborted.</para>
		/// <note>The handler will be executed in the context of the thread that called
		/// <see cref="RunAsync"/> (typically the main UI thread) as opposed to the thread
		/// that is running Clone Detective. That means no synchronization is required by
		/// handler itself.</note>
		/// </remarks>
		public event EventHandler<CloneDetectiveCompletedEventArgs> Completed;
	}
}