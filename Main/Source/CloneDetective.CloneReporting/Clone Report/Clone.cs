using System;
using System.Collections.Generic;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents a clone in the <see cref="CloneReport"/>
	/// returned by Clone Detective.
	/// </summary>
	public sealed class Clone
	{
		private CloneClass _cloneClass;
		private SourceFile _sourceFile;
		private int? _id;
		private string _uniqueId;
		private int _startLine;
		private int _lineCount;
		private int _startUnitIndexInFile;
		private int _lengthInUnits;
		private int _deltaInUnits;
		private string _gaps;
		private string _fingerprint;
		private List<CustomValue> _values = new List<CustomValue>();

		/// <summary>
		/// Gets or sets the associated <see cref="CloneClass"/> of this clone.
		/// </summary>
		public CloneClass CloneClass
		{
			get { return _cloneClass; }
			set { _cloneClass = value; }
		}

		/// <summary>
		/// The <see cref="SourceFile"/> this clone is contained in.
		/// </summary>
		public SourceFile SourceFile
		{
			get { return _sourceFile; }
			set { _sourceFile = value; }
		}

		/// <summary>
		/// Gets or sets the id of this clone.
		/// </summary>
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Gets or sets the string-based id of this clone.
		/// </summary>
		public string UniqueId
		{
			get { return _uniqueId; }
			set { _uniqueId = value; }
		}

		/// <summary>
		/// Gets or sets the line this clone starts on.
		/// </summary>
		public int StartLine
		{
			get { return _startLine; }
			set { _startLine = value; }
		}

		/// <summary>
		/// Gets or sets the clone length in lines.
		/// </summary>
		public int LineCount
		{
			get { return _lineCount; }
			set { _lineCount = value; }
		}

		/// <summary>
		/// Gets or sets the index of the normalization unit this clone starts.
		/// </summary>
		public int StartUnitIndexInFile
		{
			get { return _startUnitIndexInFile; }
			set { _startUnitIndexInFile = value; }
		}

		/// <summary>
		/// Gets or sets the length in normalization units of this clone.
		/// </summary>
		public int LengthInUnits
		{
			get { return _lengthInUnits; }
			set { _lengthInUnits = value; }
		}

		/// <summary>
		/// Gets or sets the delta in normalization units of this clone.
		/// </summary>
		public int DeltaInUnits
		{
			get { return _deltaInUnits; }
			set { _deltaInUnits = value; }
		}

		/// <summary>
		/// Gets or sets a string that represents the delta in modifications between
		/// this clone and the associated <see cref="CloneClass"/>.
		/// </summary>
		public string Gaps
		{
			get { return _gaps; }
			set { _gaps = value; }
		}

		/// <summary>
		/// Gets or sets the fingerprint of this clone.
		/// </summary>
		/// <remarks>
		/// Might be different from the <see cref="CloneClass"/>. See
		/// <see cref="Gaps"/> for details why.
		/// </remarks>
		public string Fingerprint
		{
			get { return _fingerprint; }
			set { _fingerprint = value; }
		}

		/// <summary>
		/// Gets a list of all <see cref="CustomValue">values</see> associated
		/// with this clone.
		/// </summary>
		public List<CustomValue> Values
		{
			get { return _values; }
		}
	}
}