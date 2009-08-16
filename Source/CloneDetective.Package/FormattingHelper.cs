using System;
using System.Globalization;

using CloneDetective.CloneReporting;

namespace CloneDetective.Package
{
	/// <summary>
	/// Helper class used to create formatted strings optimized for human readability.
	/// </summary>
	internal static class FormattingHelper
	{
		public static string FormatMemory(long bytes)
		{
			double kiloBytes = bytes / 1024;
			double megaBytes = bytes / (1024 * 1024);
			double gigaBytes = bytes / (1024 * 1024 * 1024);

			if (bytes == 0)
				return Res.MemoryUnknown;
			if (kiloBytes < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.MemoryBytes, bytes);
			if (megaBytes < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.MemoryKiloBytes, kiloBytes);
			if (gigaBytes < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.MemoryMegaBytes, megaBytes);

			return String.Format(CultureInfo.CurrentCulture, Res.MemoryGigaBytes, gigaBytes);
		}

		public static string FormatTime(TimeSpan time)
		{
			if (time.TotalSeconds < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.TimeMilliseconds, time.TotalMilliseconds);
			if (time.TotalMinutes < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.TimeSeconds, time.TotalSeconds);
			if (time.TotalHours < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.TimeMinutes, time.TotalMinutes);
			if (time.TotalDays < 1)
				return String.Format(CultureInfo.CurrentCulture, Res.TimeHours, time.TotalHours);

			return String.Format(CultureInfo.CurrentCulture, Res.TimeDays, time.TotalDays);
		}

		public static string FormatPercentage(double value)
		{
			return String.Format(CultureInfo.CurrentCulture, "{0:N2} %", value * 100);
		}

		public static string FormatInteger(int value)
		{
			return String.Format(CultureInfo.CurrentCulture, "{0:N0}", value);
		}

		public static string FormatCloneClassName(CloneClass cloneClass)
		{
			return String.Format(CultureInfo.CurrentCulture, Res.CloneClassName, cloneClass.Id);
		}

		public static string FormatCloneForMenu(Clone clone)
		{
			int cloneClassId = clone.CloneClass.Id;
			int startLine = clone.StartLine;
			int finishLine = clone.StartLine + clone.LineCount;
			return String.Format(CultureInfo.CurrentCulture, Res.MenuClone, cloneClassId, startLine, finishLine);
		}
	}
}