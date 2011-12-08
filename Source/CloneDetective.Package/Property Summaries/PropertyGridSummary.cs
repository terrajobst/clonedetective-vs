using System;
using System.ComponentModel;

namespace CloneDetective.Package
{
	/// <summary>
	/// This class allows an object to expose a human readable representation of
	/// an object that can be displayed by the property grid.
	/// </summary>
	public abstract class PropertyGridSummary : CustomTypeDescriptor
	{
		public sealed override string GetClassName()
		{
			return InternalClassName;
		}

		public sealed override string GetComponentName()
		{
			return InternalComponentName;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties(this, true);
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(this, attributes, true);
		}

		public override object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		protected abstract string InternalComponentName { get; }
		protected abstract string InternalClassName { get; }
	}
}