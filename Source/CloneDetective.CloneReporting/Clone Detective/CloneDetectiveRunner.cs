using System;
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
	/// This class encapsulates the asynchronous execution of running Clone Detective.
	/// </summary>
	public sealed class CloneDetectiveRunner
	{
		private string _conqatFileName;
		private string _javaHome;
		private int _minimumCloneLength;
		private string _solutionFileName;
		private string _conqatLogFileName;
		private string _cloneReportFileName;

		private bool _running;
		private Process _process;
		private TimeSpan _usedTime;
		private long _usedMemory;
		private bool _aborted;

		/// <summary>
		/// Starts Clone Detective by spawning a new process that runs ConQAT.
		/// </summary>
		/// <param name="conqatConfigFileName">The path and file name of the ConQAT config file.</param>
		/// <param name="tempConqatLogFileName">The path and file name of the temporary ConQAT log file.</param>
		/// <param name="tempCloneReportFileName">The path and file name of the temporary clone report file.</param>
		private void RunConQAT(string conqatConfigFileName, string tempConqatLogFileName, string tempCloneReportFileName)
		{
			// NOTE: We use a field instead of a local variable because we need a reference
			//       to the spawned process in order to be able to terminate it in Abort().

			using (_process = new Process())
			{
				// Start new stop watch to measure the time Clone Detective needed.
				Stopwatch stopwatch = Stopwatch.StartNew();

				// We will use the location of the temporary log file as our working folder.
				// Since the log file will reside in the user's temp folder our working folder
				// will also be in the temp folder. This ensures ConQAT has write permissions
				// there.
				string workingDirectory = Path.GetDirectoryName(tempConqatLogFileName);

				// Build the command line arguments string.
				string arguments = BuildArgumentsString(
					conqatConfigFileName,
					tempConqatLogFileName,
					tempCloneReportFileName,
					_solutionFileName,
					_minimumCloneLength);

				// Setup the process. Make sure the process starts in a hidden window.
				// NOTE: We will not use redirection features of System.Process() because we use output
				//       redirection using the command line. See BuildArgumentsString() for details.
				_process.StartInfo.FileName = _conqatFileName;
				_process.StartInfo.WorkingDirectory = workingDirectory;
				_process.StartInfo.Arguments = arguments;
				_process.StartInfo.UseShellExecute = false;
				_process.StartInfo.CreateNoWindow = true;
				_process.StartInfo.EnvironmentVariables["PATH"] = PrependPathWithJavaBinFolder(_process.StartInfo.EnvironmentVariables["PATH"], _javaHome);
				_process.StartInfo.EnvironmentVariables["JAVA_HOME"] = _javaHome;

				// Run ConQAT and wait for completion.
				_process.Start();
				_process.WaitForExit();

				// Get run time.
				_usedTime = stopwatch.Elapsed;

				// Get memory consumption.
				// NOTE: Since we have to scan the log file to get the memory consumption we cannot
				//       calculate the memory consumption if Clone Detective has been aborted. The
				//       reason is that the file might be still locked by ConQAT. Since ConQAT is not
				//       a single process (it is a batch file that starts Java) there is a race
				//       condition in Abort() that might result in Java still being running. This is
				//       by design. To be on the safe side we will not access any file that is touched
				//       by ConQAT if we abort Clone Detective.
				if (_aborted)
					_usedMemory = 0;
				else
					_usedMemory = GetMemory(tempConqatLogFileName);
			}
		}

		private static string PrependPathWithJavaBinFolder(string path, string javaHome)
		{
			string javaBinFolder = Path.Combine(javaHome, "bin");
			return javaBinFolder + Path.PathSeparator + path;
		}

		/// <summary>
		/// Builds a string representing the command line arguments for ConQAT.
		/// </summary>
		/// <param name="conqatConfigFileName">The path and file name of the ConQAT config file.</param>
		/// <param name="tempConqatLogFileName">The path and file name of the temporary ConQAT log file.</param>
		/// <param name="tempCloneReportFileName">The path and file name of the temporary clone report file.</param>
		/// <param name="solutionFileName">The path and file name of the Visual Studio solution file.</param>
		/// <param name="minimumCloneLength">The minimum clone length.</param>
		/// <returns></returns>
		private static string BuildArgumentsString(string conqatConfigFileName, string tempConqatLogFileName, string tempCloneReportFileName, string solutionFileName, int minimumCloneLength)
		{
			StringBuilder sb = new StringBuilder();

			// Set the name of the config file.
			sb.Append("-f \"");
			sb.Append(conqatConfigFileName);
			sb.Append("\"");

			// Set parameter solution.dir
			sb.Append(" -p \"solution.dir=");
			sb.Append(Path.GetDirectoryName(solutionFileName));
			// Ensure trailing backslash.
			// NOTE: Please note that we need two backslashes at the end. Otherwise the trailing backslash
			//       together with the quote would be interpreted as an escaped quote.
			sb.Append("\\\\\"");

			// Set parameter output.dir
			sb.Append(" -p \"output.dir=");
			sb.Append(Path.GetDirectoryName(tempCloneReportFileName));
			// NOTE: Please note that we need two backslashes at the end. Otherwise the trailing backslash
			//       together with the quote would be interpreted as an escaped quote.
			sb.Append("\\\\\"");

			// Set parameter output.file
			sb.Append(" -p \"output.file=");
			sb.Append(Path.GetFileName(tempCloneReportFileName));
			sb.Append("\"");

			// Set parameter clone.minlength
			sb.Append(" -p \"clone.minlength=");
			sb.Append(minimumCloneLength);
			sb.Append("\"");

			// Add redirection of standard output and standard error to temporary log file.
			sb.Append(" > \"");
			sb.Append(tempConqatLogFileName);
			sb.Append("\" 2>&1");

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
		/// <exception cref="InvalidOperationException">
		/// <para>Thrown when <see cref="ConqatFileName"/> has not been initialized.</para>
		/// <para>- or -</para>
		/// <para>Thrown when <see cref="SolutionFileName"/> has not been initialized.</para>
		/// <para>- or -</para>
		/// <para>Thrown when Clone Detective is already running.</para>
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public void RunAsync()
		{
			if (String.IsNullOrEmpty(_conqatFileName))
				throw ExceptionBuilder.PropertyMustBeInitializedFirst("ConqatFileName");

			if (String.IsNullOrEmpty(_solutionFileName))
				throw ExceptionBuilder.PropertyMustBeInitializedFirst("SolutionFileName");

			if (_running)
				throw ExceptionBuilder.CloneDetectiveAlreadyRunning();

			// Make sure our internal state is updated first.
			_running = true;
			_aborted = false;

			// Now we decide which clone detection analysis file we will use. If the solution folder
			// contains a clone detection we will use that. Otherwise we will fallback on the default
			// analysis file stored in the installation folder.
			string solutionConqatConfigFileName = ConqatFiles.GetSolutionCloneDetectionPath(_solutionFileName);
			string defaultConqatConfigFileName = ConqatFiles.GetDefaultCloneDetectionPath();
			string conqatConfigFileName;
			if (File.Exists(solutionConqatConfigFileName))
			{
				// Ok, the solution folder contains a ConQAT clone detection analysis file.
				// We will use that.
				conqatConfigFileName = solutionConqatConfigFileName;
			}
			else
			{
				// I am sorry, no custom clone detection file present. Use the default one.
				conqatConfigFileName = defaultConqatConfigFileName;
			}

			// For the log file and clone report file we first use temporary log files.
			string tempConqatLogFileName = Path.GetTempFileName();
			string tempCloneReportFileName = Path.GetTempFileName();
			
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
							try
							{
								// Run ConQAT and wait for completion.
								RunConQAT(conqatConfigFileName, tempConqatLogFileName, tempCloneReportFileName);

								// Check if ConQAT exited normally or we killed it using Abort().
								if (_aborted)
								{
									// ConQAT has been aborted.
									//
									// As mentioned above we cannot access any file that has been touched by
									// ConQAT. Write the note to our log file.
									File.WriteAllText(_conqatLogFileName, "ConQAT aborted -- log is unknown.");
								}
								else
								{
									// OK, ConQAT has run to completion.
									//
									// Copy the temp log to our solution log file (overwriting any existing one).
									File.Copy(tempConqatLogFileName, _conqatLogFileName, true);

									// Check if ConQAT produced a clone report.
									//
									// If this is not the case then ConQAT failed for some reason.
									if (!File.Exists(tempCloneReportFileName) || (new FileInfo(tempCloneReportFileName)).Length == 0)
										throw ExceptionBuilder.ConqatDidNotProduceCloneReport();

									// Great! ConQAT produced a clone report so we are just fine. Copy the temp report
									// to our solution folder (overwriting any exisiting one).
									File.Copy(tempCloneReportFileName, _cloneReportFileName, true);
								}
							}
							finally
							{
								// Check if ConQAT has been aborted.
								// In this case we cannot access any file that has been touched by
								// ConQAT.
								if (!_aborted)
								{
									// No, ConQAT either ran successfully or failed -- but we did not kill the
									// process. So we can safely delete our temp files.
									File.Delete(tempConqatLogFileName);
									File.Delete(tempCloneReportFileName);
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
							CloneDetectiveResult cloneDetectiveResult = CloneDetectiveResult.FromSolutionPath(_solutionFileName);
							EventHandler<CloneDetectiveResultEventArgs> handler = Completed;
							if (handler != null)
							{
								CloneDetectiveResultEventArgs eventArgs = new CloneDetectiveResultEventArgs(cloneDetectiveResult, exception);
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
		/// This method disables all completed events by clearing the event handler
		/// list.
		/// </summary>
		/// <remarks>
		/// This is useful if the host application is going to be shut down
		/// and Clone Detective is still running. In this case you should first
		/// call <see cref="DisableEvents"/> and then <see cref="Abort"/>.
		/// </remarks>
		public void DisableEvents()
		{
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
		/// Gets or sets the path and file name of the ConQAT executable.
		/// </summary>
		public string ConqatFileName
		{
			get { return _conqatFileName; }
			set { _conqatFileName = value; }
		}

		/// <summary>
		/// Gets or sets the path to the Java Home directory.
		/// </summary>
		public string JavaHome
		{
			get { return _javaHome; }
			set { _javaHome = value; }
		}

		/// <summary>
		/// Gets or sets the minimum clone length.
		/// </summary>
		public int MinimumCloneLength
		{
			get { return _minimumCloneLength; }
			set { _minimumCloneLength = value; }
		}

		/// <summary>
		/// Gets or sets the path and file name of the Visual Studio solution file.
		/// </summary>
		public string SolutionFileName
		{
			get { return _solutionFileName; }
			set
			{
				_solutionFileName = value;
				UpdateOtherFileNames();
			}
		}

		/// <summary>
		/// This method updates the file names that are dependent on the path of
		/// the <see cref="SolutionFileName"/>.
		/// </summary>
		private void UpdateOtherFileNames()
		{
			if (_solutionFileName != null)
			{
				_conqatLogFileName = ConqatFiles.GetLogPath(_solutionFileName);
				_cloneReportFileName = ConqatFiles.GetCloneReportPath(_solutionFileName);
			}
		}

		/// <summary>
		/// Adds or removes a handler completion notification.
		/// </summary>
		/// <remarks>
		/// The <see cref="Completed"/> handler will be raised regardless whether Clone
		/// Detective succeeded, failed or was aborted. The handler will be executed in
		/// the context of the main thread (opposed to the thread that is running Clone
		/// Detective). No synchronization is required.
		/// </remarks>
		public event EventHandler<CloneDetectiveResultEventArgs> Completed;
	}
}