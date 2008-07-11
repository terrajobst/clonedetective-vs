using System;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class wraps a <see cref="CloneDetectiveResult"/> in the .NET standard
	/// <see cref="EventArgs"/> fashion needed by event handlers.
	/// </summary>
	public sealed class CloneDetectiveResultEventArgs : EventArgs
	{
		private CloneDetectiveResult _result;
		private Exception _exception;

		public CloneDetectiveResultEventArgs(CloneDetectiveResult result, Exception exception)
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