using System;
using System.Globalization;

using Microsoft.VisualStudio.Shell;

namespace CloneDetective.Package
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class ProvideTextMarkerAttribute : RegistrationAttribute
	{
		private Type _markerType;
		private Type _providerType;
		private string _name;

		public ProvideTextMarkerAttribute(Type markerType, Type providerType, string name)
		{
			_markerType = markerType;
			_providerType = providerType;
			_name = name;
		}

		private string GetKeyPath()
		{
			return string.Format(CultureInfo.InvariantCulture, @"Text Editor\External Markers\{0:B}", _markerType.GUID);
		}

		public override void Register(RegistrationContext context)
		{
			using (Key key = context.CreateKey(GetKeyPath()))
			{
				key.SetValue(null, _name);
				key.SetValue("DisplayName", _name);
				key.SetValue("Package", context.ComponentType.GUID.ToString("B"));
				key.SetValue("Service", _providerType.GUID.ToString("B"));
			}
		}

		public override void Unregister(RegistrationContext context)
		{
			context.RemoveKey(GetKeyPath());
		}
	}
}