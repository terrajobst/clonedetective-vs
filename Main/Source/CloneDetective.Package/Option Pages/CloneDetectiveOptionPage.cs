using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell;

namespace CloneDetective.Package
{
	[Guid("1670d890-b3e2-465d-9ec8-80ef3eac7ba1")]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public sealed class CloneDetectiveOptionPage : DialogPage
	{
		private string _conqatFileName;
		private string _javaHome;
		private int _minimumCloneLength = 10;
		private CloneDetectiveOptionPageControl _window;

		public string ConqatFileName
		{
			get { return _conqatFileName; }
			set { _conqatFileName = value; }
		}

		public string JavaHome
		{
			get { return _javaHome; }
			set { _javaHome = value; }
		}

		public int MinimumCloneLength
		{
			get { return _minimumCloneLength; }
			set { _minimumCloneLength = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected override IWin32Window Window
		{
			get
			{
				if (_window == null)
					_window = new CloneDetectiveOptionPageControl(this);
				return _window;
			}
		}

		//Tools Options First Time:
		//	LoadSettings
		//	Activate
		//
		//Tools Options Second Time:
		//	Activate
		//
		//Cancel:
		//	LoadSettings
		//
		//Ok:
		//	Deactivate
		//	OnApply
		//	LoadSettings

		protected override void OnActivate(CancelEventArgs e)
		{
			base.OnActivate(e);
			_window.LoadSettings();
		}

		protected override void OnDeactivate(CancelEventArgs e)
		{
			base.OnDeactivate(e);
			_window.SaveSettings();
		}
	}
}
