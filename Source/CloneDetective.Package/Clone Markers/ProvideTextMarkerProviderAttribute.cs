using System;
using System.Globalization;

using Microsoft.VisualStudio.Shell;

namespace CloneDetective.Package
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class ProvideTextMarkerProviderAttribute : RegistrationAttribute
	{
		private Type _type;
		private string _name;

		public ProvideTextMarkerProviderAttribute(Type type, string name)
		{
			_type = type;
			_name = name;
		}

		private string GetKeyPath()
		{
			return string.Format(CultureInfo.InvariantCulture, @"Services\{0:B}", _type.GUID);
		}

		public override void Register(RegistrationContext context)
		{
			using (Key key = context.CreateKey(GetKeyPath()))
			{
				key.SetValue(null, context.ComponentType.GUID.ToString("B"));
				key.SetValue("Name", _name);
			}
		}

		public override void Unregister(RegistrationContext context)
		{
			context.RemoveKey(GetKeyPath());
		}
	}
}