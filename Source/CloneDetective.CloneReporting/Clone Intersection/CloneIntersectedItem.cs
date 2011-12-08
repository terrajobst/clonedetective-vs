using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents a clone intersection item used by <see cref="CloneIntersectionResult"/>.
	/// </summary>
	/// <remarks>
	/// A clone intersection item is a <see cref="SourceNode"/> and a list of intersecting clones, represented by
	/// <see cref="CloneIntersection"/>.
	/// </remarks>
	public sealed class CloneIntersectedItem
	{
		private SourceNode _sourceNode;
		private List<CloneIntersection> _clones = new List<CloneIntersection>();

		/// <summary>
		/// Gets or sets the <see cref="CloneReporting.SourceNode"/> the clones are contained in. Please
		/// note that the associated <see cref="CloneReporting.SourceNode"/> can only be a leaf node, i.e.
		/// <see cref="CloneReporting.SourceNode.SourceFile"/> cannot be <see langword="null"/>.
		/// </summary>
		public SourceNode SourceNode
		{
			get { return _sourceNode; }
			set { _sourceNode = value; }
		}

		/// <summary>
		/// Gets the list of intersecting clones.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<CloneIntersection> Clones
		{
			get { return _clones; }
		}
	}
}