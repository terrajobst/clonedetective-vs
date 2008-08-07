using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using CloneDetective.CloneReporting;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.VSHelp80;
using Microsoft.Win32;

using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace CloneDetective.Package
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	///
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the 
	/// IVsPackage interface and uses the registration attributes defined in the framework to 
	/// register itself and its components with the shell.
	/// </summary>
	// This attribute tells the registration utility (regpkg.exe) that this class needs
	// to be registered as package.
	[PackageRegistration(UseManagedResourcesOnly = true)]
	// A Visual Studio component can be registered under different regitry roots; for instance
	// when you debug your package you want to register it in the experimental hive. This
	// attribute specifies the registry root to use if no one is provided to regpkg.exe with
	// the /root switch.
	[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
	// This attribute is used to register the informations needed to show the this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration(true, null, null, null)]
	// In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
	// package needs to have a valid load key (it can be requested at 
	// http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
	// package has a load key embedded in its resources.
	[ProvideLoadKey(VSPackage.MinimumVisualStudioEdition, VSPackage.ProductVersion, VSPackage.ProductName, VSPackage.Company, 666)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource(1000, 1)]
	// These attributes register the tool windows exposed by this package.
	[ProvideToolWindow(typeof(CloneExplorerToolWindow))]
	[ProvideToolWindow(typeof(CloneIntersectionsToolWindow))]
	[ProvideToolWindow(typeof(CloneResultsToolWindow))]
	[ProvideOptionPage(typeof(CloneDetectiveOptionPage), "Clone Detective", "Settings", 0, 0, true)]
	// We'd like to automatically load this package as soon as a solution exists.
	// The GUID is the same as VSConstants.UICONTEXT_SolutionExists.
	[ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
	// Don't forget to specify the GUID of the package.
	[Guid(Guids.GuidPackageString)]
	public sealed class VSPackage : Microsoft.VisualStudio.Shell.Package, IVsInstalledProduct
	{
		public const string MinimumVisualStudioEdition = "Standard";
		public const string ProductName = "Clone Detective for Visual Studio";
		public const string ProductVersion = "1.0";
		public const string Description = "This package detects and visualizes clones in C# source code files.";
		public const string Company = ProductName;

		private static VSPackage _instance;
		private IVsOutputWindowPane _outputWindowPane;

		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public VSPackage()
		{
			_instance = this;
		}

		public static VSPackage Instance
		{
			get { return _instance; }
		}

		public new object GetService(Type serviceType)
		{
			return base.GetService(serviceType);
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public ToolStripRenderer GetVisualStudioToolWindowRenderer()
		{
			ServiceProvider serviceProvider = new ServiceProvider(GetServiceProvider());
			IUIService service = serviceProvider.GetService(typeof(IUIService)) as IUIService;
			if (service != null)
			{
				ToolStripRenderer renderer = service.Styles["VsToolWindowRenderer"] as ToolStripRenderer;
				if (renderer != null)
					return renderer;
			}

			return null;
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public IServiceProvider GetServiceProvider()
		{
			return (IServiceProvider) GetService(typeof(IServiceProvider));
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public CloneDetectiveOptionPage GetOptionPage()
		{
			return (CloneDetectiveOptionPage)GetDialogPage(typeof(CloneDetectiveOptionPage));
		}

		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TUserControl GetToolWindowUserControl<TToolWindow, TUserControl>()
			where TToolWindow : ToolWindowPane
			where TUserControl : UserControl
		{
			ToolWindowPane window = FindToolWindow(typeof(TToolWindow), 0, true);
			if (window == null)
				return null;

			return (TUserControl)window.Window;
		}

		private void ShowCloneExplorer(object sender, EventArgs e)
		{
			ShowToolWindow<CloneExplorerToolWindow>();
		}

		private void ShowCloneIntersections(object sender, EventArgs e)
		{
			ShowToolWindow<CloneIntersectionsToolWindow>();
		}

		private void ShowCloneResults(object sender, EventArgs e)
		{
			ShowToolWindow<CloneResultsToolWindow>();
		}

		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public void ShowToolWindow<T>()
			where T: ToolWindowPane
		{
			// Get the instance number 0 of this tool window. This window is single instance so this instance
			// is actually the only one.
			// The last flag is set to true so that if the tool window does not exists it will be created.
			ToolWindowPane window = FindToolWindow(typeof(T), 0, true);
			if (window == null || window.Frame == null)
				throw ExceptionBuilder.CannotCreateWindow();

			IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
			ErrorHandler.ThrowOnFailure(windowFrame.Show());
		}

		#region Package Members

		private uint _solutionEventCookie;
		private IConnectionPoint _tmConnectionPoint;
		private uint _tmConnectionCookie;
		private uint _rdtEventCookie;

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initilaization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			ToolStripManager.Renderer = new VisualStudioLikeToolStripRenderer();

			// Add the Text Marker Service to the service container. The container will care
			// about all the rest (proffer the service, revoke it, and so on).
			CloneMarkerTypeProvider markerTypeProvider = new CloneMarkerTypeProvider();
			((IServiceContainer) this).AddService(markerTypeProvider.GetType(), markerTypeProvider, true);

			base.Initialize();

			// Now it's time to initialize our copies of the marker IDs. We need them to be
			// able to create marker instances.
			CloneMarkerTypeProvider.InitializeMarkerIds(this);

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				// Create the commands for the tool windows.
				CommandID cloneExplorerCommandID = new CommandID(Guids.GuidCommandSet, (int) PkgCmdid.CmdidCloneExplorer);
				MenuCommand cloneExplorerMenuItem = new MenuCommand(ShowCloneExplorer, cloneExplorerCommandID);
				mcs.AddCommand(cloneExplorerMenuItem);

				CommandID cloneIntersectionsCommandID = new CommandID(Guids.GuidCommandSet, (int) PkgCmdid.CmdidCloneIntersections);
				MenuCommand cloneIntersectionsMenuItem = new MenuCommand(ShowCloneIntersections, cloneIntersectionsCommandID);
				mcs.AddCommand(cloneIntersectionsMenuItem);

				CommandID cloneResultsCommandID = new CommandID(Guids.GuidCommandSet, (int)PkgCmdid.CmdidCloneResults);
				MenuCommand cloneResultsMenuItem = new MenuCommand(ShowCloneResults, cloneResultsCommandID);
				mcs.AddCommand(cloneResultsMenuItem);
			}

			// Advise event sinks. We need to know when a solution is opened and closed
			// (SolutionEventSink), when a document is opened and closed (TextManagerEventSink),
			// and when a document is saved (RunningDocTableEventSink).
			SolutionEventSink solutionEventSink = new SolutionEventSink();
			TextManagerEventSink textManagerEventSink = new TextManagerEventSink();
			RunningDocTableEventSink runningDocTableEventSink = new RunningDocTableEventSink();

			IVsSolution solution = (IVsSolution) GetService(typeof(SVsSolution));
			ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(solutionEventSink, out _solutionEventCookie));

			IConnectionPointContainer textManager = (IConnectionPointContainer) GetService(typeof(SVsTextManager));
			Guid interfaceGuid = typeof(IVsTextManagerEvents).GUID;
			textManager.FindConnectionPoint(ref interfaceGuid, out _tmConnectionPoint);
			_tmConnectionPoint.Advise(textManagerEventSink, out _tmConnectionCookie);

			IVsRunningDocumentTable rdt = (IVsRunningDocumentTable) GetService(typeof(SVsRunningDocumentTable));
			ErrorHandler.ThrowOnFailure(rdt.AdviseRunningDocTableEvents(runningDocTableEventSink, out _rdtEventCookie));

			// Since we register custom text markers we have to ensure the font and color
			// cache is up-to-date.
			ValidateFontAndColorCacheManagerIsUpToDate();

			// Ensure settings are initialized correctly.
			InitializeUserSettings();
		}

		protected override void Dispose(bool disposing)
		{
			// Remove solution event notifications.
			IVsSolution solution = (IVsSolution) GetService(typeof(SVsSolution));
			ErrorHandler.Ignore(solution.UnadviseSolutionEvents(_solutionEventCookie));

			// Remove text manager event notifications.
			if (_tmConnectionPoint != null)
			{
				_tmConnectionPoint.Unadvise(_tmConnectionCookie);
				_tmConnectionPoint = null;
			}

			// Remove running document table (RDT) event notifications.
			// Ignore any errors that might occur since we're shutting down.
			IVsRunningDocumentTable rdt = (IVsRunningDocumentTable) GetService(typeof(SVsRunningDocumentTable));
			ErrorHandler.Ignore(rdt.UnadviseRunningDocTableEvents(_rdtEventCookie));

			// Forward call to the base class.
			base.Dispose(disposing);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void ValidateFontAndColorCacheManagerIsUpToDate()
		{
			// The Fonts and Colors cache has to be refreshed after each marker change
			// (during development) and of course after install/uninstall.

			// To identify when we have to refresh the cache we store a custom value
			// in the Visual Studio registry hive. Downsides of this approach:
			//
			//     1. The cache is not refreshed until the package is loaded for the
			//        first time. That means that our text markers don't show up in
			//        the Fonts and Colors settings after install. They will only
			//        show up after the first solution has been opened.
			//
			//     2. Since this code is part of the package we can't execute it
			//        after uninstall. That means that the user will see our markers
			//        in the Fonts and Colors settings dialog even after he has
			//        uninstalled the package. That is very ugly!

			IVsFontAndColorCacheManager cacheManager = (IVsFontAndColorCacheManager) GetService(typeof(SVsFontAndColorCacheManager));
			if (cacheManager == null)
				return;

			// We need to know whether we already refreshed the fonts and colors
			// cache to reflect the text markers we registered. We have to do this
			// on a per-user basis.
			bool alreadyInitialized = false;

			try
			{
				string registryValueName = "InstalledVersion";
				string expectedVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

				using (RegistryKey rootKey = UserRegistryRoot)
				using (RegistryKey ourKey = rootKey.CreateSubKey(ProductName))
				{
					object registryValue = ourKey.GetValue(registryValueName);
					string initializedVersion = Convert.ToString(registryValue, CultureInfo.InvariantCulture);

					alreadyInitialized = (initializedVersion == expectedVersion);

					ourKey.SetValue(registryValueName, expectedVersion, RegistryValueKind.String);
				}
			}
			catch
			{
				// Ignore any errors since it's not a big deal if we can't read
				// this setting. We just always refresh the cache in that case.
			}

			// Actually refresh the Fonts and Colors cache now if we detected we have
			// to do so.
			if (!alreadyInitialized)
			{
				ErrorHandler.ThrowOnFailure(cacheManager.ClearAllCaches());

				Guid categoryGuid = Guid.Empty;
				ErrorHandler.ThrowOnFailure(cacheManager.RefreshCache(ref categoryGuid));
				categoryGuid = Guids.GuidFontsAndColorsTextEditor;
				ErrorHandler.ThrowOnFailure(cacheManager.RefreshCache(ref categoryGuid));
			}
		}

		private void InitializeUserSettings()
		{
			CloneDetectiveOptionPage optionPage = GetOptionPage();

			bool conqatFileNameInitialized = !String.IsNullOrEmpty(optionPage.ConqatFileName);
			if (!conqatFileNameInitialized)
				optionPage.ConqatFileName = GlobalSettings.GetConqatBatFileName();

			bool javaHomeInitialized = !String.IsNullOrEmpty(optionPage.JavaHome);
			if (!javaHomeInitialized)
				optionPage.JavaHome = GlobalSettings.GetJavaHome();

			bool needSave = !conqatFileNameInitialized ||
			                !javaHomeInitialized;
			if (needSave)
				optionPage.SaveSettingsToStorage();
		}

		#endregion

		#region IVsInstalledProduct Members

		int IVsInstalledProduct.IdBmpSplash(out uint pIdBmp)
		{
			pIdBmp = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsInstalledProduct.OfficialName(out string pbstrName)
		{
			pbstrName = ProductName;
			return VSConstants.S_OK;
		}

		int IVsInstalledProduct.ProductID(out string pbstrPID)
		{
			pbstrPID = ProductVersion;
			return VSConstants.S_OK;
		}

		int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
		{
			pbstrProductDetails = Description;
			return VSConstants.S_OK;
		}

		int IVsInstalledProduct.IdIcoLogoForAboutbox(out uint pIdIco)
		{
			pIdIco = 400;
			return VSConstants.S_OK;
		}

		#endregion

		public event EventHandler<EventArgs> SolutionOpened;
		public event EventHandler<EventArgs> SolutionClosed;

		internal void OnSolutionOpened()
		{
			EventHandler<EventArgs> handler = SolutionOpened;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		internal void OnSolutionClosed()
		{
			EventHandler<EventArgs> handler = SolutionClosed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public void OpenDocument(string fileName)
		{
			IServiceProvider oleServiceProvider = GetServiceProvider();
			ServiceProvider serviceProvider = new ServiceProvider(oleServiceProvider);
			VsShellUtilities.OpenDocument(serviceProvider, fileName);
		}

		public void OpenDocumentReadOnly(string fileName)
		{
			OpenDocument(fileName);

			IVsTextLines textLines;
			IVsTextView textView = GetTextViewAndEnsureVisible(fileName);
			ErrorHandler.ThrowOnFailure(textView.GetBuffer(out textLines));
			ErrorHandler.ThrowOnFailure(textLines.SetStateFlags((uint) BUFFERSTATEFLAGS.BSF_USER_READONLY));
		}

		public IVsTextView GetTextViewAndEnsureVisible(string fileName)
		{
			ServiceProvider serviceProvider = new ServiceProvider(GetServiceProvider());
			IVsUIHierarchy hierarchy;
			uint itemID;
			IVsWindowFrame windowFrame;
			if (VsShellUtilities.IsDocumentOpen(serviceProvider, fileName, Guid.Empty, out hierarchy, out itemID, out windowFrame))
			{
				ErrorHandler.ThrowOnFailure(windowFrame.Show());
				return VsShellUtilities.GetTextView(windowFrame);
			}
			
			OpenDocument(fileName);
			if (!VsShellUtilities.IsDocumentOpen(serviceProvider, fileName, Guid.Empty, out hierarchy, out itemID, out windowFrame))
				return null;

			return VsShellUtilities.GetTextView(windowFrame);
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public string GetSolutionPath()
		{
			string solutionDirectory;
			string solutionFileName;
			string userOptions;

			IVsSolution solution = (IVsSolution) GetService(typeof(SVsSolution));
			ErrorHandler.ThrowOnFailure(solution.GetSolutionInfo(out solutionDirectory, out solutionFileName, out userOptions));

			return solutionFileName;
		}

		public void SelectCloneInEditor(Clone clone)
		{
			string filePath = clone.SourceFile.Path;
			int startLine = clone.StartLine;
			int endLine = clone.StartLine + clone.LineCount;

			IVsTextView view = GetTextViewAndEnsureVisible(filePath);
			if (view != null)
				ErrorHandler.ThrowOnFailure(view.SetSelection(startLine, 0, endLine, 0));
		}

		public void ShowError(string message)
		{
			ServiceProvider serviceProvider = new ServiceProvider(GetServiceProvider());
			VsShellUtilities.ShowMessageBox(serviceProvider, message, ProductName, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "f")]
		public void ShowHelp(string f1Keyword)
		{
			// Obtain the Help2 object from SVsHelp service.
			Help2 help2 = (Help2)GetService(typeof(Microsoft.VisualStudio.VSHelp.SVsHelp));
			help2.DisplayTopicFromF1Keyword(f1Keyword);
		}

		#region Output Window

		public void ClearOutput()
		{
			if (_outputWindowPane != null)
				ErrorHandler.ThrowOnFailure(_outputWindowPane.Clear());
		}

		public void WriteOutput(string message)
		{
			if (_outputWindowPane == null)
			{
				IVsOutputWindow output = (IVsOutputWindow)GetService(typeof(SVsOutputWindow));

				Guid paneGuid = Guid.Empty;
				const bool visible = true;
				const bool clearWithSolution = true;

				// Create a new pane.
				ErrorHandler.ThrowOnFailure(output.CreatePane(ref paneGuid, Res.CloneDetective, Convert.ToInt32(visible),
				                                              Convert.ToInt32(clearWithSolution)));

				// Retrieve the new pane.
				ErrorHandler.ThrowOnFailure(output.GetPane(ref paneGuid, out _outputWindowPane));
			}

			ErrorHandler.ThrowOnFailure(_outputWindowPane.OutputString(message));
		}

		public void WriteOutputLine()
		{
			WriteOutput(Environment.NewLine);
		}

		public void WriteOutputLine(string message)
		{
			WriteOutput(message);
			WriteOutputLine();
		}

		#endregion

	}
}
