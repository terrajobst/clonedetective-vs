// Guids.cs
// MUST match guids.h
using System;

namespace CloneDetective.Package
{
	internal static class Guids
	{
		public const string GuidPackageString = "2188d17a-79a4-4f07-87b0-c66c31823ae7";
		public const string GuidCommandSetString = "1096b961-b193-4160-9d5e-060d46511610";
		public const string GuidCloneMarkerServiceString = "03c6bf5f-4814-4927-92ad-41ad9faee8af";

		public static readonly Guid GuidCommandSet = new Guid(GuidCommandSetString);

		public static readonly Guid GuidCloneBackgroundMarker = new Guid("877b4dbe-df33-4764-9e8e-9e19fa772e21");
		public static readonly Guid GuidCloneMarginMarker = new Guid("c6d11640-5b3a-45ec-8118-53ee3380ddab");
		public static readonly Guid GuidFontsAndColorsTextEditor = new Guid("a27b4e24-a735-4d1d-b8e7-9716e1e3d8e0");
	}
}