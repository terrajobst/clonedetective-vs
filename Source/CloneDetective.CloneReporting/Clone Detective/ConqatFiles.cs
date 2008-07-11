using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class centralizes all file names that are used by clone detective.
	/// </summary>
	public static class ConqatFiles
	{
		/// <summary>
		/// Returns the fully qualified path of the default clone detection analysis file.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetDefaultCloneDetectionPath()
		{
			string installationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return Path.GetFullPath(Path.Combine(installationPath, "DefaultCloneDetection.cqa"));
		}

		/// <summary>
		/// Returns the fully qualified path of the solution specific clone detection analysis file.
		/// </summary>
		/// <param name="solutionPath">The path and file name of the Visual Studio solution file.</param>
		public static string GetSolutionCloneDetectionPath(string solutionPath)
		{
			return Path.Combine(Path.GetDirectoryName(solutionPath), "CloneDetection.cqa");
		}

		/// <summary>
		/// Returns the fully qualified path of the Clone Detective report file.
		/// </summary>
		/// <param name="solutionPath">The path and file name of the Visual Studio solution file.</param>
		public static string GetCloneReportPath(string solutionPath)
		{
			return Path.ChangeExtension(solutionPath, ".Clones.user");
		}

		/// <summary>
		/// Returns the fully qualified path of the Clone Detective log file.
		/// </summary>
		/// <param name="solutionPath">The path and file name of the Visual Studio solution file.</param>
		public static string GetLogPath(string solutionPath)
		{
			return Path.ChangeExtension(solutionPath, ".Clones.Log.user");
		}
	}
}
