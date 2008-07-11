using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class encapsulates the reading and parsing of additional information to the ConQAT log file.
	/// This allows managing information such as a success/error status and statistics such as time taken
	/// and memory usage.
	/// </summary>
	public static class LogHelper
	{
		/// <summary>
		/// Writes the given exception to the log file. Adds a status marker that marks the Clone Detective
		/// result as failed.
		/// </summary>
		/// <param name="logFilePath">The fully qualified path to the log file to write to.</param>
		/// <param name="exception">The exception to be written.</param>
		public static void WriteError(string logFilePath, Exception exception)
		{
			using (StreamWriter sw = new StreamWriter(logFilePath, true))
				WriteError(sw, exception);
		}

		/// <summary>
		/// Writes the given status and stats to the log file.
		/// </summary>
		/// <param name="logFilePath">The fully qualified path to the log file to write to.</param>
		/// <param name="status">The status to be written.</param>
		/// <param name="usedMemory">The memory usage to be written.</param>
		/// <param name="usedTime">The time to be written.</param>
		public static void WriteStatusInfo(string logFilePath, CloneDetectiveResultStatus status, long usedMemory, TimeSpan usedTime)
		{
			using (StreamWriter sw = new StreamWriter(logFilePath, true))
				WriteStatusInfo(sw, status, usedMemory, usedTime);
		}

		/// <summary>
		/// Writes the given exception to the log file. Adds a status marker that marks the Clone Detective
		/// result as failed.
		/// </summary>
		/// <param name="logWriter">The log file writer to write to.</param>
		/// <param name="exception">The exception to be written.</param>
		public static void WriteError(TextWriter logWriter, Exception exception)
		{
			logWriter.WriteLine();
			logWriter.WriteLine("**** Error ****");
			logWriter.WriteLine(exception);
		}

		/// <summary>
		/// Writes the given status and stats to the log file.
		/// </summary>
		/// <param name="logWriter">The log file writer to write to.</param>
		/// <param name="status">The status to be written.</param>
		/// <param name="usedMemory">The memory usage to be written.</param>
		/// <param name="usedTime">The time to be written.</param>
		public static void WriteStatusInfo(TextWriter logWriter, CloneDetectiveResultStatus status, long usedMemory, TimeSpan usedTime)
		{
			logWriter.WriteLine();
			logWriter.WriteLine("**** Result ****");
			logWriter.WriteLine(String.Format(CultureInfo.InvariantCulture, "CD_STATUS: {0}", status));
			logWriter.WriteLine(String.Format(CultureInfo.InvariantCulture, "CD_MEMORY: {0}", usedMemory));
			logWriter.WriteLine(String.Format(CultureInfo.InvariantCulture, "CD_TIME: {0}", usedTime));
		}

		/// <summary>
		/// Parses the status and stats from the given log file.
		/// </summary>
		/// <param name="logFilePath">The log file to parse.</param>
		/// <param name="status">Contains the status.</param>
		/// <param name="usedMemory">Contains the memory usage.</param>
		/// <param name="usedTime">Contains the time.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#")]
		public static void ParseStatusInfo(string logFilePath, out CloneDetectiveResultStatus status, out long usedMemory, out TimeSpan usedTime)
		{
			string logFileContent = File.ReadAllText(logFilePath);

			status = CloneDetectiveResultStatus.Succeeded;
			usedMemory = 0;
			usedTime = TimeSpan.Zero;

			Match statusMatch = Regex.Match(logFileContent, @"CD_STATUS\: (?<Status>[a-zA-Z]+)");
			Match memoryMatch = Regex.Match(logFileContent, @"CD_MEMORY\: (?<Memory>[0-9]+)");
			Match timeMatch = Regex.Match(logFileContent, @"CD_TIME\: (?<Time>[0-9.:]+)");

			SkipToLastMatch(ref statusMatch);
			SkipToLastMatch(ref memoryMatch);
			SkipToLastMatch(ref timeMatch);

			if (statusMatch.Success)
				status = (CloneDetectiveResultStatus) Enum.Parse(typeof (CloneDetectiveResultStatus), statusMatch.Groups["Status"].Value);

			if (memoryMatch.Success)
				usedMemory = Int64.Parse(memoryMatch.Groups["Memory"].Value, CultureInfo.InvariantCulture);

			if (timeMatch.Success)
				usedTime = TimeSpan.Parse(timeMatch.Groups["Time"].Value);
		}

		/// <summary>
		/// Makes sure the given match is the last possible match.
		/// </summary>
		private static void SkipToLastMatch(ref Match match)
		{
			Match nextMatch = match;
			
			while (nextMatch != null && nextMatch.Success)
			{
				match = nextMatch;
				nextMatch = match.NextMatch();
			}
		}
	}
}