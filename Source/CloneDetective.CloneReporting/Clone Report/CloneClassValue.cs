using System;
using System.Diagnostics.CodeAnalysis;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents an abitrary key-value pair that can be associated
	/// with a <see cref="CloneClass" />.
	/// </summary>
	public sealed class CloneClassValue
	{
		private string _key;
		private string _value;
		private string _type;

		/// <summary>
		/// Gets or sets the key of this key-value pair.
		/// </summary>
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		/// <summary>
		/// Gets or sets the value of this key-value pair.
		/// </summary>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary>
		/// Gets or sets the type of this key-value pair.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}
	}
}