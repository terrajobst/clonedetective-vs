using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Schema;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class centralizes exception creation to ensure that
	/// <list type="bullet">
	/// <item>exception types are consistent and</item>
	/// <item>the same errors are constructed the same way.</item>
	/// </list>
	/// </summary>
	internal static class ExceptionBuilder
	{
		/// <summary>
		/// This exception is thrown when ConQAT failed to produce a clone report.
		/// </summary>
		public static InvalidOperationException ConqatDidNotProduceCloneReport()
		{
			return new InvalidOperationException(Resources.ConqatDidNotProduceCloneReport);
		}

		/// <summary>
		/// This exception is thrown when the Clone Detective is started while it is still running.
		/// </summary>
		public static InvalidOperationException CloneDetectiveAlreadyRunning()
		{
			return new InvalidOperationException(Resources.CloneDetectiveAlreadyRunning);
		}

		/// <summary>
		/// This exception is thrown when a clone report cannot be loaded for the given reason.
		/// </summary>
		/// <param name="fileName">The file name of the clone report to load.</param>
		/// <param name="ex">Specifies the reason why the clone report failed to load.</param>
		public static InvalidOperationException CannotLoadCloneReport(string fileName, Exception ex)
		{
			string messages = String.Format(CultureInfo.CurrentCulture, Resources.CannotLoadCloneReport, fileName, ex.Message);
			return new InvalidOperationException(messages, ex);
		}

		/// <summary>
		/// This exception is thrown when a clone report could be loaded but did not successfully validate
		/// agains the clone report schema.
		/// </summary>
		/// <param name="fileName">The file name of the clone report to load.</param>
		/// <param name="errors">The list of validation errors.</param>
		/// <returns></returns>
		public static InvalidOperationException InvalidCloneReport(string fileName, List<XmlSchemaException> errors)
		{
			StringBuilder sb = new StringBuilder();

			foreach (XmlSchemaException exception in errors)
				sb.AppendLine(exception.Message);

			string messages = String.Format(CultureInfo.CurrentCulture, Resources.InvalidCloneReport, fileName, sb);
			return new InvalidOperationException(messages);
		}
	}
}
