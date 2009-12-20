using System;
using System.Globalization;

namespace CloneDetective.Package
{
	/// <summary>
	/// This class centralizes exception creation to ensure that
	/// <list type="bullet">
	/// <item>Exception types are consistent</item>
	/// <item>the same errors are constructed the same way</item>
	/// </list>
	/// </summary>
	internal static class ExceptionBuilder
	{
		/// <summary>
		/// This exception is thrown when a switch statement has no useful behavior
		/// for the default case and raising an error is an appropriate action.
		/// </summary>
		/// <param name="value">The value used in the switch statement.</param>
		public static NotImplementedException UnhandledCaseLabel(object value)
		{
			string message = String.Format(CultureInfo.CurrentCulture, Res.UnhandledCaseLabel, value);
			return new NotImplementedException(message);
		}

		/// <summary>
		/// This exception is thrown when a tool window could not be created.
		/// </summary>
		public static NotSupportedException CannotCreateWindow()
		{
			return new NotSupportedException(Res.CannotCreateWindow);
		}
	}
}
