using System;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class wraps the startup arguments of ConQAT in the .NET standard 
	/// <see cref="EventArgs"/> fashion needed by event handlers. This event
	/// argument is passed to <see cref="CloneDetectiveRunner.Started"/>.
	/// </summary>
	public sealed class CloneDetectiveStartedEventArgs : EventArgs
	{
		private string _program;
		private string _arguments;

		/// <summary>
		/// Creates a new instance of <see cref="CloneDetectiveStartedEventArgs"/> 
		/// by the given startup arguments.
		/// </summary>
		/// <param name="program">The fully qualified path to <c>conqat.bat</c>.</param>
		/// <param name="arguments">The command line arguments passed to <c>conqat.bat</c>.</param>
		public CloneDetectiveStartedEventArgs(string program, string arguments)
		{
			_program = program;
			_arguments = arguments;
		}

		/// <summary>
		/// Gets the fully qualified path to <c>conqat.bat</c>.
		/// </summary>
		public string Program
		{
			get { return _program; }
		}

		/// <summary>
		/// Gets the command line arguments passed to <c>conqat.bat</c>.
		/// </summary>
		public string Arguments
		{
			get { return _arguments; }
		}
	}
}