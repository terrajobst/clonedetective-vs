using System;

namespace CloneDetective.Package
{
	/// <summary>
	/// This class provides the names of the resources that must be used in a
	/// context where the type-safe access of the designer generated Res class 
	/// is not possible, e.g. attributes.
	/// </summary>
	internal static class ResNames
	{
		public const string CategoryCloneInformation = "CategoryCloneInformation";

		public const string DisplayNameName = "DisplayNameName";
		public const string DisplayNameFullPath = "DisplayNameFullPath";
		public const string DisplayNameNumberOfClones = "DisplayNameNumberOfClones";
		public const string DisplayNumberOfCloneClasses = "DisplayNumberOfCloneClasses";
		public const string DisplayNameNumberOfClonedLines = "DisplayNameNumberOfClonedLines";
		public const string DisplayNameClonePercentage = "DisplayNameClonePercentage";

		public const string DisplayNameCloneIntersections = "DisplayNameCloneIntersections";
	}
}