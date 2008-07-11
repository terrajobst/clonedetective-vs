using System;
using System.ComponentModel;

namespace CloneDetective.Package
{
	internal sealed class ResourcedDisplayNameAttribute : DisplayNameAttribute
	{
		public ResourcedDisplayNameAttribute(string resourceName)
			: base(resourceName)
		{
		}

		public override string DisplayName
		{
			get { return Res.ResourceManager.GetString(base.DisplayName); }
		}
	}
}