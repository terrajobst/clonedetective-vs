using System;
using System.ComponentModel;

namespace CloneDetective.Package
{
	internal sealed class ResourcedCategoryAttribute : CategoryAttribute
	{
		public ResourcedCategoryAttribute(string resourceName)
			: base(resourceName)
		{
		}

		protected override string GetLocalizedString(string value)
		{
			return Res.ResourceManager.GetString(value);
		}
	}
}
