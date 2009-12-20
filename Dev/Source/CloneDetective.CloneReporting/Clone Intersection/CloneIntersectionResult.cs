using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class describes a clone intersection result.
	/// </summary>
	/// <remarks>
	/// A clone intersection result is defined as a set of clones that are associated 
	/// with a clone class contained in a given source file.
	/// </remarks>
	public sealed class CloneIntersectionResult
	{
		private CloneIntersectedItem _target;
		private List<CloneIntersectedItem> _references = new List<CloneIntersectedItem>();

		/// <summary>
		/// Gets or sets the <see cref="CloneIntersectedItem"/> representing the source file
		/// for which the intersection result was constructed.
		/// </summary>
		public CloneIntersectedItem Target
		{
			get { return _target; }
			set { _target = value; }
		}

		/// <summary>
		/// Gets a list of <see cref="CloneIntersectedItem"/> that share their <see cref="CloneClass"/>
		/// with <see cref="Target"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<CloneIntersectedItem> References
		{
			get { return _references; }
		}
	}
}