using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CloneDetective.Package
{
	[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WindowPane")]
	public abstract class ServicedToolWindowPane : ToolWindowPane
	{
		protected ServicedToolWindowPane(System.IServiceProvider provider)
			: base(provider)
		{
		}

		private IServiceProvider GetServiceProvider()
		{
			object serviceProvider;
			IVsWindowFrame frame = (IVsWindowFrame)Frame;
			ErrorHandler.Ignore(frame.GetProperty((int)__VSFPROPID.VSFPROPID_SPFrame, out serviceProvider));
			return (IServiceProvider)serviceProvider;
		}

		public override void OnToolWindowCreated()
		{
			base.OnToolWindowCreated();

			IServiceProvider oleServiceProvider = GetServiceProvider();
			System.IServiceProvider serviceProvider = new ServiceProvider(oleServiceProvider);
			IServiceProviderHost serviceProviderHost = (IServiceProviderHost) Window;
			serviceProviderHost.Initialize(serviceProvider);
		}
	}
}