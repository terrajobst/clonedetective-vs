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

		protected abstract string HelpKeyword { get; }

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
			IServiceProviderHost serviceProviderHost = (IServiceProviderHost)Window;
			serviceProviderHost.Initialize(serviceProvider);

			SetupF1Help();
		}

		private void SetupF1Help()
		{
			// Get the window frame's user context
			object userContextObject;
			ErrorHandler.ThrowOnFailure(((IVsWindowFrame)Frame).GetProperty((int)__VSFPROPID.VSFPROPID_UserContext, out userContextObject));

			// Add an F1 keyword identifying the help topic for this toolwindow
			var userContext = (IVsUserContext)userContextObject;
			ErrorHandler.ThrowOnFailure(userContext.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "Keyword", HelpKeyword));
		}
	}
}