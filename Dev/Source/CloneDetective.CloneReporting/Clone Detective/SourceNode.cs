using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// Represents a node in a <see cref="SourceTree"/>. A source node is either a
	/// file or a directory.
	/// </summary>
	public sealed class SourceNode
	{
		private string _name;
		private string _fullPath;
		private SourceFile _sourceFile;
		private SourceNode _parent;
		private List<SourceNode> _children = new List<SourceNode>();
		private HashSet<CloneClass> _cloneClasses = new HashSet<CloneClass>();
		private int _linesOfCode;
		private int _numberOfClones;
		private int _numberOfClonedLines;

		/// <summary>
		/// Gets or sets the name of this node.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the fully qualified path of this node.
		/// </summary>
		public string FullPath
		{
			get { return _fullPath; }
			set { _fullPath = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CloneReporting.SourceFile"/> of this node.
		/// </summary>
		/// <remarks>
		/// If this node does not represent a file but a directory the value is
		/// <see langword="null"/>.
		/// </remarks>
		public SourceFile SourceFile
		{
			get { return _sourceFile; }
			set { _sourceFile = value; }
		}

		/// <summary>
		/// Gets or sets the parent of this node.
		/// </summary>
		public SourceNode Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		/// <summary>
		/// Gets a list representing the children of this node. If this node represents a file
		/// the list is always empty.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<SourceNode> Children
		{
			get { return _children; }
		}

		/// <summary>
		/// Gets the set of all clone classes contained in this node.
		/// </summary>
		/// <remarks>
		/// If this node represents a file this is the set of all clone classes contained in it.
		/// If this node represents a directory this is the union of all of its children.
		/// </remarks>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public HashSet<CloneClass> CloneClasses
		{
			get { return _cloneClasses; }
			set { _cloneClasses = value; }
		}

		/// <summary>
		/// Gets or sets the number of lines of code contained in this node.
		/// </summary>
		/// <remarks>
		/// If this node represents a file this is the number of lines of code of this file.
		/// If this node represents a directory this is the sum of all of its children.
		/// </remarks>
		public int LinesOfCode
		{
			get { return _linesOfCode; }
			set { _linesOfCode = value; }
		}

		/// <summary>
		/// Gets or sets the number of clones contained in this node.
		/// </summary>
		/// <remarks>
		/// If this node represents a file this is the number of clones contained in it.
		/// If this node represents a directory this is the sum of all of its children.
		/// </remarks>
		public int NumberOfClones
		{
			get { return _numberOfClones; }
			set { _numberOfClones = value; }
		}

		/// <summary>
		/// Gets the number of elements contained in <see cref="CloneClasses"/>.
		/// </summary>
		public int NumberOfCloneClasses
		{
			get { return _cloneClasses.Count; }
		}

		/// <summary>
		/// Gets or sets the number of cloned lines.
		/// </summary>
		/// <remarks>
		/// If this node represents a file this is the number of cloned lines contained in it.
		/// If this node represents a directory this is the sum of all of its children.
		/// </remarks>
		public int NumberOfClonedLines
		{
			get { return _numberOfClonedLines; }
			set { _numberOfClonedLines = value; }
		}

		/// <summary>
		/// Gets the percentage of cloned lines, i.e. <see cref="NumberOfClonedLines"/> divided by
		/// <see cref="LinesOfCode"/>.
		/// </summary>
		public double ClonePercentage
		{
			get
			{
				if (_linesOfCode == 0)
					return 0.0;

				return (double)_numberOfClonedLines / _linesOfCode;
			}
		}
	}
}