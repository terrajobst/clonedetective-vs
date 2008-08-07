using System;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class wraps a <see cref="CloneDetectiveResult"/> in the .NET standard
	/// <see cref="EventArgs"/> fashion needed by event handlers. This event argument
	/// is passed to <see cref="CloneDetective.Completed"/>
	/// </summary>
	public sealed class CloneDetectiveCompletedEventArgs : EventArgs
	{
		private CloneDetectiveResult _result;
		private Exception _exception;

		/// <summary>
		/// Creates a new instance of <see cref="CloneDetectiveCompletedEventArgs"/> by the given
		/// <see cref="CloneDetectiveResult"/> and <see cref="Exception"/>.
		/// </summary>
		/// <param name="result">Contains the result returned by Clone Detective.</param>
		/// <param name="exception">If an error occured running Clone Detective
		/// <paramref name="exception"/> refers to the <see cref="System.Exception"/>.</param>
		public CloneDetectiveCompletedEventArgs(CloneDetectiveResult result, Exception exception)
		{
			_result = result;
			_exception = exception;
		}

		/// <summary>
		/// Gets the <see cref="CloneDetectiveResult"/>.
		/// </summary>
		public CloneDetectiveResult Result
		{
			get { return _result; }
		}

		/// <summary>
		/// Gets the exception (if any) that was raised while running the clone detective. If
		/// no error occurred the return value is <see langword="null"/>.
		/// </summary>
		public Exception Exception
		{
			get { return _exception; }
		}
	}
}