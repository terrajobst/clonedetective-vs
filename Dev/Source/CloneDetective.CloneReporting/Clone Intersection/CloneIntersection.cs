using System;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// Represents an intersecting clone used by <see cref="CloneIntersectionResult"/>.
	/// </summary>
	public sealed class CloneIntersection
	{
		private Clone _clone;
		private int _cloneClassId;

		/// <summary>
		/// Gets or sets the clone.
		/// </summary>
		public Clone Clone
		{
			get { return _clone; }
			set { _clone = value; }
		}

		/// <summary>
		/// Gets or sets the clone class id.
		/// </summary>
		/// <remarks>
		/// This id is not the same as <see cref="CloneClass.Id"/> obtained via <see cref="Clone">Clone's</see>
		/// <see cref="CloneReporting.Clone.CloneClass"/>. Instead this id is only unique within the intersection
		/// but is guaranteed to be zero-based and having no gaps. This is used to assign the same color to all
		/// clone intersections having the same <see cref="CloneClassId"/>.
		/// </remarks>
		public int CloneClassId
		{
			get { return _cloneClassId; }
			set { _cloneClassId = value; }
		}
	}
}