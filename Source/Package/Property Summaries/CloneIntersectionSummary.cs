using System;

using CloneDetective.CloneReporting;

namespace CloneDetective.Package
{
	public sealed class CloneIntersectionSummary : PropertyGridSummary
	{
		private CloneIntersectedItem _cloneIntersectedItem;

		public CloneIntersectionSummary(CloneIntersectedItem cloneIntersectedItem)
		{
			_cloneIntersectedItem = cloneIntersectedItem;
		}

		protected override string InternalComponentName
		{
			get { return _cloneIntersectedItem.SourceNode.Name; }
		}

		protected override string InternalClassName
		{
			get { return Res.CloneIntersectionSummary; }
		}

		[ResourcedCategory(ResNames.CategoryCloneInformation)]
		[ResourcedDisplayName(ResNames.DisplayNameCloneIntersections)]
		public int CloneIntersections
		{
			get { return _cloneIntersectedItem.Clones.Count; }
		}

		[ResourcedDisplayName(ResNames.DisplayNameFullPath)]
		public string FullPath
		{
			get { return _cloneIntersectedItem.SourceNode.FullPath; }
		}

		[ResourcedDisplayName(ResNames.DisplayNameName)]
		public string Name
		{
			get { return _cloneIntersectedItem.SourceNode.Name; }
		}
	}
}