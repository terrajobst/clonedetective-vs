using System;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This enumeration is used to describe the possible outcomes of
	/// running clone detective.
	/// </summary>
	public enum CloneDetectiveResultStatus
	{
		/// <summary>
		/// Indicates that Clone Detective succeeded normally.
		/// </summary>
		Succeeded,

		/// <summary>
		/// Indicates that Clone Detective failed.
		/// </summary>
		Failed,

		/// <summary>
		/// Indicates that Clone Detective has been stopped by the user.
		/// </summary>
		Stopped
	}
}