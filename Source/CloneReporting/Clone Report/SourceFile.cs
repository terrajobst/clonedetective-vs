using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents a source file in the <see cref="CloneReport"/>
	/// returned by Clone Detective.
	/// </summary>
	public sealed class SourceFile
	{
		private int _id;
		private string _path;
		private int _length;
		private string _fingerprint;
		private List<Clone> _clones = new List<Clone>();

		/// <summary>
		/// Gets or sets the numeric ID of this source file.
		/// </summary>
		/// <remarks>
		/// The source file ID is session specific. That means if you add or remove files
		/// an re-run the clone detective it is possible that the same source file gets a
		/// different ID. If you want to identify source files by name use <see cref="Path"/>.
		/// If you want to identify source files by the content use <see cref="Fingerprint"/>.
		/// </remarks>
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Gets or sets the path of this source file.
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>
		/// Gets or sets the count of lines of this source file.
		/// </summary>
		public int Length
		{
			get { return _length; }
			set { _length = value; }
		}

		/// <summary>
		/// Gets or sets the fingerprint (i.e hash) of this source file.
		/// </summary>
		/// <remarks>
		/// Can be used to identify source files independently from the
		/// session specific numerical <see cref="Id"/> and independently
		/// from the <see cref="Path"/>.
		/// </remarks>
		public string Fingerprint
		{
			get { return _fingerprint; }
			set { _fingerprint = value; }
		}

		/// <summary>
		/// Gets a list of clones contained in this source file.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<Clone> Clones
		{
			get { return _clones; }
		}
	}
}