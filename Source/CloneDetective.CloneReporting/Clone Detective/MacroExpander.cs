using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class expands macros in the solution-specific settings.
	/// </summary>
	public sealed class MacroExpander
	{
		private Dictionary<string, string> _macros = new Dictionary<string, string>();

		/// <summary>
		/// Creates a new instance of <see cref="MacroExpander"/> by the given
		/// Visual Studio solution file.
		/// </summary>
		/// <param name="solutionFileName">The fully qualified path to the Visual Studio solution file.</param>
		public MacroExpander(string solutionFileName)
		{
			_macros.Add("$(InstallDir)", GlobalSettings.GetInstallDir());
			_macros.Add("$(ConQATDir)", GlobalSettings.GetConqatDir());
			_macros.Add("$(DevEnvDir)", GlobalSettings.GetDevEnvDir());
			_macros.Add("$(SolutionPath)", solutionFileName);
			_macros.Add("$(SolutionDir)", PathHelper.EnsureTrailingBackslash(Path.GetDirectoryName(solutionFileName)));
			_macros.Add("$(SolutionFileName)", Path.GetFileName(solutionFileName));
			_macros.Add("$(SolutionName)", Path.GetFileNameWithoutExtension(solutionFileName));
			_macros.Add("$(SolutionExt)", Path.GetExtension(solutionFileName));
		}

		/// <summary>
		/// Returns a new string in which all macros in given <paramref name="text"/> are expanded.
		/// </summary>
		/// <param name="text">The text in which all macros should be expanded.</param>
		public string Expand(string text)
		{
			StringBuilder sb = new StringBuilder(text);
			foreach (KeyValuePair<string, string> macro in _macros)
				sb.Replace(macro.Key, macro.Value);

			return sb.ToString();
		}

		/// <summary>
		/// Gets a dictionary with all macros.
		/// </summary>
		/// <remarks>
		/// The dictionary already contains all macros. You are not required to register the macros.
		/// <note>
		/// The keys in the dictionary already include the <c>$(</c> prefix as well as the <c>)</c> suffix.
		/// </note>
		/// </remarks>
		public Dictionary<string, string> Macros
		{
			get { return _macros; }
		}
	}
}