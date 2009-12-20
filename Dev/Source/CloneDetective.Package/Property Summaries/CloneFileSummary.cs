using System;

using CloneDetective.CloneReporting;

namespace CloneDetective.Package
{
	/// <summary>
	/// This class is used to clone information of a <see cref="SourceNode"/> for
	/// the .NET property grid.
	/// </summary>
	public sealed class CloneFileSummary : PropertyGridSummary
	{
		private SourceNode _sourceNode;

		public CloneFileSummary(SourceNode sourceNode)
		{
			_sourceNode = sourceNode;
		}

		protected override string InternalComponentName
		{
			get { return _sourceNode.Name; }
		}

		protected override string InternalClassName
		{
			get { return Res.CloneFileSummary; }
		}

		[ResourcedDisplayName(ResNames.DisplayNameName)]
		public string Name
		{
			get { return _sourceNode.Name; }
		}

		[ResourcedDisplayName(ResNames.DisplayNameFullPath)]
		public string FullPath
		{
			get { return _sourceNode.FullPath; }
		}

		[ResourcedCategory(ResNames.CategoryCloneInformation)]
		[ResourcedDisplayName(ResNames.DisplayNameNumberOfClones)]
		public int NumberOfClones
		{
			get { return _sourceNode.NumberOfClones; }
		}

		[ResourcedCategory(ResNames.CategoryCloneInformation)]
		[ResourcedDisplayName(ResNames.DisplayNumberOfCloneClasses)]
		public int NumberOfCloneClasses
		{
			get { return _sourceNode.NumberOfCloneClasses; }
		}

		[ResourcedCategory(ResNames.CategoryCloneInformation)]
		[ResourcedDisplayName(ResNames.DisplayNameNumberOfClonedLines)]
		public int NumberOfClonedLines
		{
			get { return _sourceNode.NumberOfClonedLines; }
		}

		[ResourcedCategory(ResNames.CategoryCloneInformation)]
		[ResourcedDisplayName(ResNames.DisplayNameClonePercentage)]
		public string ClonePercentage
		{
			get { return FormattingHelper.FormatPercentage(_sourceNode.ClonePercentage); }
		}
	}
}