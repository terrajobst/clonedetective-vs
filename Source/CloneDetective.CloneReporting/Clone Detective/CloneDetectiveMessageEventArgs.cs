using System;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class wraps an output message written during clone detection
	/// in the .NET standard <see cref="EventArgs"/> fashion needed by event
	/// handlers. This event argument is passed to <see cref="CloneDetective.Message"/>.
	/// </summary>
	public sealed class CloneDetectiveMessageEventArgs : EventArgs
	{
		private string _message;

		/// <summary>
		/// Creates a new instance of <see cref="CloneDetectiveMessageEventArgs"/> by the
		/// given message.
		/// </summary>
		/// <param name="message">The output message written during clone detection.</param>
		public CloneDetectiveMessageEventArgs(string message)
		{
			_message = message;
		}

		/// <summary>
		/// Gets the output message written during clone detection.
		/// </summary>
		public string Message
		{
			get { return _message; }
		}
	}
}