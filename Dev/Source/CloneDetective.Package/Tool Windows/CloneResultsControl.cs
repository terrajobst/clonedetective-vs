using System;
using System.Windows.Forms;

using CloneDetective.CloneReporting;

using TD.SandDock;

namespace CloneDetective.Package
{
	public partial class CloneResultsControl : ToolWindowUserControl
	{
		public CloneResultsControl()
		{
			InitializeComponent();
		}

		protected override void Initialize()
		{
			VSPackage.Instance.SolutionClosed += delegate { CloseResults(); };
			CloneDetectiveManager.CloneDetectiveResultChanged += delegate { UpdateResults(); };
		}

		private void CloseResults()
		{
			for (int i = documentContainer.Documents.Length - 1; i >= 0; i--)
			{
				DockControl dockControl = documentContainer.Documents[i];
				documentContainer.RemoveDocument(dockControl);
			}
		}

		private void UpdateResults()
		{
			if (!CloneDetectiveManager.IsCloneReportAvailable)
				CloseResults();
			else
			{
				for (int i = documentContainer.Documents.Length - 1; i >= 0; i--)
				{
					DockControl dockControl = documentContainer.Documents[i];
					CloneResultPageControl pageControl = (CloneResultPageControl) dockControl.Controls[0];

					CloneClass newCloneClass = CloneDetectiveManager.CloneDetectiveResult.FindCloneClass(pageControl.CloneClass.Fingerprint);
					if (newCloneClass == null)
						documentContainer.RemoveDocument(dockControl);
					else
						pageControl.CloneClass = newCloneClass;
				}
			}
		}

		public void Add(CloneClass cloneClass)
		{
			CloneResultPageControl pageControl = new CloneResultPageControl();
			pageControl.CloneClass = cloneClass;
			string resultName = FormattingHelper.FormatCloneClassName(cloneClass);
			DockControl dockControl = new DockControl(pageControl, resultName);
			documentContainer.AddDocument(dockControl);
			documentContainer.ActiveDocument = dockControl;
		}

		private void documentContainer_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				documentContainer.ActiveDocument.Close();
		}

		private void documentContainer_ActiveDocumentChanged(object sender, ActiveDocumentEventArgs e)
		{
			if (documentContainer.Documents.Length == 0)
			{
				documentContainer.Visible = false;
				noResultsBorderPanel.Visible = true;
			}
			else
			{
				documentContainer.Visible = true;
				noResultsBorderPanel.Visible = false;
			}
		}
	}
}
