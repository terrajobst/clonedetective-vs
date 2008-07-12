using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents a clone class in the <see cref="CloneReport"/>
	/// returned by Clone Detective.
	/// </summary>
	public sealed class CloneClass
	{
		private int _id;
		private int _normalizedLength;
		private string _fingerprint;
		private List<CloneClassValue> _values = new List<CloneClassValue>();
		private List<Clone> _clones = new List<Clone>();

		/// <summary>
		/// Gets or sets the numeric ID of this clone class.
		/// </summary>
		/// <remarks>
		/// The clone class ID is session specific. That means if you modify the source code
		/// and re-run the clone detective it is possible that the same clone class gets a
		/// different ID. If you want to identify clone classes by the content use
		/// <see cref="Fingerprint"/>.
		/// </remarks>
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Gets or sets the normalized length of this clone class.
		/// </summary>
		public int NormalizedLength
		{
			get { return _normalizedLength; }
			set { _normalizedLength = value; }
		}

		/// <summary>
		/// Gets or sets the fingerprint (i.e. hash) of this clone class.
		/// </summary>
		/// <remarks>
		/// Can be used to identify clone classes independently from the
		/// session specific numerical <see cref="Id"/>.
		/// </remarks>
		public string Fingerprint
		{
			get { return _fingerprint; }
			set { _fingerprint = value; }
		}

		/// <summary>
		/// Gets a list of all <see cref="CloneClassValue">values</see> associated
		/// with this clone class.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<CloneClassValue> Values
		{
			get { return _values; }
		}

		/// <summary>
		/// Gets a list of all <see cref="Clone">Clones</see> associated with
		/// this clone class.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<Clone> Clones
		{
			get { return _clones; }
		}
	}
}