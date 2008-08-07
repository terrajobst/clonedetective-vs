using System;
using System.IO;
using System.Text;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class centralizes all file name and path handling used by clone detective.
	/// </summary>
	public static class PathHelper
	{
		/// <summary>
		/// Gets the extension used for the solution-specific settings file of Clone Detective.
		/// </summary>
		public static string SettingsExtension
		{
			get { return ".CloneDetective.user"; }
		}

		/// <summary>
		/// Gets the extension used for the solution-specific log file of Clone Detective.
		/// </summary>
		public static string LogExtension
		{
			get { return ".CloneDetective.Log.user"; }
		}

		/// <summary>
		/// Gets the extension used for the solution-specific clone report file of Clone Detective.
		/// </summary>
		public static string CloneReportExtension
		{
			get { return ".CloneDetective.Clones.user"; }
		}

		/// <summary>
		/// Gets the file name of the default clone detective analysis file.
		/// </summary>
		/// <remarks>
		/// <see cref="DefaultAnalysisFileName"/> only includes the file name parte (not the fully qualified path).
		/// </remarks>
		public static string DefaultAnalysisFileName
		{
			get { return "DefaultCloneDetection.cqa"; }
		}

		/// <summary>
		/// Returns the fully qualified path of the Clone Detective settings file.
		/// </summary>
		/// <param name="solutionPath">The path and file name of the Visual Studio solution file.</param>
		public static string GetSettingsPath(string solutionPath)
		{
			return Path.ChangeExtension(solutionPath, SettingsExtension);
		}

		/// <summary>
		/// Returns the fully qualified path of the Clone Detective log file.
		/// </summary>
		/// <param name="solutionPath">The path and file name of the Visual Studio solution file.</param>
		public static string GetLogPath(string solutionPath)
		{
			return Path.ChangeExtension(solutionPath, LogExtension);
		}

		/// <summary>
		/// Returns the fully qualified path of the Clone Detective report file.
		/// </summary>
		/// <param name="solutionPath">The path and file name of the Visual Studio solution file.</param>
		public static string GetCloneReportPath(string solutionPath)
		{
			return Path.ChangeExtension(solutionPath, CloneReportExtension);
		}

		/// <summary>
		/// Ensures the given path is suffixed by a single backslash.
		/// </summary>
		/// <param name="path">The path for which this method makes sure that it is suffixed by backslash.</param>
		public static string EnsureTrailingBackslash(string path)
		{
			if (!String.IsNullOrEmpty(path) && path[path.Length - 1] != Path.DirectorySeparatorChar)
				path += Path.DirectorySeparatorChar;

			return path;
		}

		/// <summary>
		/// Returns a new path that expresses <paramref name="path"/> relatively to <paramref name="relativeToPath"/>.
		/// </summary>
		/// <param name="relativeToPath">The path <paramref name="path"/> should be expressed relatively to.</param>
		/// <param name="path">The path to be made relative.</param>
		/// <returns>
		/// <para>This method first tries to find a common root directory between the two given paths. If there is no 
		/// common root director (e.g. because both paths are on different drives) this method returns <paramref name="path"/>.
		/// </para>
		/// <para>As a heuristic, this method also simply returns <paramref name="path"/> if both paths only have
		/// the drive in common.
		/// </para>
		/// </returns>
		public static string GetRelativePath(string relativeToPath, string path)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(relativeToPath);

			string commonPrefix = relativeToPath;
			while (!path.StartsWith(commonPrefix, StringComparison.OrdinalIgnoreCase))
			{
				commonPrefix = Path.GetDirectoryName(commonPrefix);
				if (commonPrefix == null)
					return path;

				sb.Append(Path.DirectorySeparatorChar);
				sb.Append("..");
			}

			// If the drive was the only part both directories have
			// in common we return an absolute path.
			if (Path.GetDirectoryName(commonPrefix) == null)
				return path;

			string pathDir = Path.GetDirectoryName(path);
			while (!String.Equals(commonPrefix, pathDir, StringComparison.OrdinalIgnoreCase))
			{
				string newPathDir = Path.GetDirectoryName(pathDir);
				string delta = pathDir.Substring(newPathDir.Length + 1);
				sb.Append(Path.DirectorySeparatorChar);
				sb.Append(delta);
				pathDir = newPathDir;
			}

			sb.Append(Path.DirectorySeparatorChar);
			sb.Append(Path.GetFileName(path));

			return sb.ToString();
		}

		/// <summary>
		/// Returns the conventional fully qualified path of the Java properties file that
		/// is used by the given ConQAT analysis file.
		/// </summary>
		/// <param name="analysisFileName">The ConQAT analysis file for which the Java properties
		/// file should be returned for.</param>
		public static string GetPropertiesFilePath(string analysisFileName)
		{
			// NOTE: We don't use Path.ChangeExtension() because Java property files also include
			//       the extension. For example the Java properties file for D:\ConQAT\Test.cqa is
			//       simply D:\ConQAT\Test.cqa.properties.
			return analysisFileName + ".properties";
		}
	}
}
