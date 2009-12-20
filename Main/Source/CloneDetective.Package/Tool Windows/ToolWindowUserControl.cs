using System;
using System.Security.Permissions;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CloneDetective.Package
{
	public partial class ToolWindowUserControl : UserControl, IServiceProviderHost
	{
		private SelectionContainer _selectionContainer = new SelectionContainer(true, true);
		private IServiceProvider _serviceProvider;

		public ToolWindowUserControl()
		{
			InitializeComponent();
		}

		void IServiceProviderHost.Initialize(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			Initialize();
		}

		protected virtual void Initialize()
		{
		}

		protected SelectionContainer SelectionContainer
		{
			get { return _selectionContainer; }
		}

		public new object GetService(Type serviceType)
		{
			return _serviceProvider.GetService(serviceType);
		}

		/// <summary> 
		/// Let this control process the mnemonics.
		/// </summary>
		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogChar(char charCode)
		{
			// If we're the top-level form or control, we need to do the mnemonic handling
			if (charCode != ' ' && ProcessMnemonic(charCode))
				return true;

			return base.ProcessDialogChar(charCode);
		}

		/// <summary>
		/// Enable the IME status handling for this control.
		/// </summary>
		protected override bool CanEnableIme
		{
			get { return true; }
		}

		protected void UpdatePropertyGrid(object selection)
		{
			object[] selectedObjects;
			if (selection == null)
				selectedObjects = null;
			else
				selectedObjects = new object[] {selection};

			_selectionContainer.SelectableObjects = selectedObjects;
			_selectionContainer.SelectedObjects = selectedObjects;

			ITrackSelection trackSelection = (ITrackSelection)_serviceProvider.GetService(typeof(SVsTrackSelectionEx));
			ErrorHandler.ThrowOnFailure(trackSelection.OnSelectChange(_selectionContainer));
		}

		protected static void SetToolStripRenderer(ToolStrip toolStrip)
		{
			ToolStripRenderer visualStudioToolWindowRenderer = VSPackage.Instance.GetVisualStudioToolWindowRenderer();

			if (visualStudioToolWindowRenderer == null)
				toolStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
			else
				toolStrip.Renderer = visualStudioToolWindowRenderer;
		}
	}
}