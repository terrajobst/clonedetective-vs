using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class encapsulates an object model represention of a ConQAT
	/// clone report.
	/// </summary>
	public sealed class CloneReport
	{
		private List<SourceFile> _sourceFiles = new List<SourceFile>();
		private List<CloneClass> _cloneClasses = new List<CloneClass>();

		/// <summary>
		/// Gets a list of all source files.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<SourceFile> SourceFiles
		{
			get { return _sourceFiles; }
		}

		/// <summary>
		/// Gets a list of all clone classes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<CloneClass> CloneClasses
		{
			get { return _cloneClasses; }
		}

		/// <summary>
		/// Reads a clone report from the given ConQAT clone report file.
		/// </summary>
		/// <param name="fileName">The fully qualified path of the clone report file.</param>
		public static CloneReport FromFile(string fileName)
		{
			return CloneReportReader.Read(fileName);
		}

		/// <summary>
		/// Writes the clone report to the given ConQAT clone report file.
		/// </summary>
		/// <param name="fileName">The fully qualified path of the clone report file.</param>
		/// <param name="cloneReport">The clone report to be written.</param>
		public static void ToFile(string fileName, CloneReport cloneReport)
		{
			CloneReportWriter.Write(fileName, cloneReport);
		}
	}
}