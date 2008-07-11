using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using CloneDetective.CloneReporting;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace CloneDetective.Package
{
	public partial class CloneIntersectionsControl : ToolWindowUserControl
	{
		private SelectionTracker _selectionTracker;
		private SourceNode _sourceNode;
		private int _maximumLoc;

		private static Brush[] _cloneBrushes = GetCloneBrushes();
		private CloneIntersectionResult _cloneIntersectionResult;

		public CloneIntersectionsControl()
		{
			InitializeComponent();
		}

		protected override void Initialize()
		{
			_selectionTracker = new SelectionTracker(this);
			uint cookie;
			IVsMonitorSelection monitorSelection = (IVsMonitorSelection)VSPackage.Instance.GetService(typeof(SVsShellMonitorSelection));
			ErrorHandler.ThrowOnFailure(monitorSelection.AdviseSelectionEvents(_selectionTracker, out cookie));

			fileNameLabel.Text = Res.NoCloneReportOrNoFileSelected;
			CloneDetectiveManager.CloneDetectiveResultChanged += delegate
			{
				string filePath = null;
				if (_sourceNode != null)
					filePath = _sourceNode.FullPath;

				UpdateSelection(filePath);
				UpdateReferenceCloneVisualizationOffset();
			};

			UpdateReferenceCloneVisualizationOffset();
			UpdatePropertyGrid();
		}

		#region Helper Classes

		private sealed class SelectionTracker : IVsSelectionEvents
		{
			private CloneIntersectionsControl _cloneIntersectionsControl;

			public SelectionTracker(CloneIntersectionsControl cloneIntersectionsControl)
			{
				_cloneIntersectionsControl = cloneIntersectionsControl;
			}

			public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld,
			                              ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew,
			                              IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
			{
				if (pSCNew == _cloneIntersectionsControl.SelectionContainer)
					return VSConstants.S_OK;
				
				CloneFileSummary cloneFileSummary = GetCloneDetectiveSummary(pSCNew);
				if (cloneFileSummary != null)
				{
					_cloneIntersectionsControl.UpdateSelection(cloneFileSummary.FullPath);
					return VSConstants.S_OK;
				}

				string filePath = GetFilePath(pHierNew, itemidNew);
				if (filePath != null)
				{
					_cloneIntersectionsControl.UpdateSelection(filePath);
					return VSConstants.S_OK;
				}


				return VSConstants.S_OK;
			}

			private static CloneFileSummary GetCloneDetectiveSummary(ISelectionContainer pSCNew)
			{
				if (pSCNew == null)
					return null;

				uint count;
				ErrorHandler.ThrowOnFailure(pSCNew.CountObjects((uint) Constants.GETOBJS_SELECTED, out count));

				if (count <= 0)
					return null;
				
				object[] selectedObjects = new object[1];
				ErrorHandler.ThrowOnFailure(pSCNew.GetObjects((uint) Constants.GETOBJS_SELECTED, 1, selectedObjects));
				return selectedObjects[0] as CloneFileSummary;
			}

			private static string GetFilePath(IVsHierarchy pHierNew, uint itemidNew)
			{
				if (pHierNew == null)
					return null;
				
				// Will return E_UNEXPECTED for multi-selects
				// GetCanonicalName returns the full path to a file (selected in Solution Explorer,
				// Class View, or active editor window). Source Control Explorer does not work.
				// However, there is no other way to get the full path of the selected file, i.e.
				// there is no property we could call GetProperty for to retrieve it. So this is
				// the only way to do it.
				string strName;
				int hr = pHierNew.GetCanonicalName(itemidNew, out strName);
				if (hr != VSConstants.S_OK)
					return null;

				return strName;
			}

			public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
			{
				return VSConstants.S_OK;
			}

			public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
			{
				return VSConstants.S_OK;
			}
		}

		private sealed class ReferenceHitTestInfo
		{
			private RectangleF _rectangle;
			private CloneIntersection _cloneIntersection;

			public RectangleF Rectangle
			{
				get { return _rectangle; }
				set { _rectangle = value; }
			}

			public CloneIntersection CloneIntersection
			{
				get { return _cloneIntersection; }
				set { _cloneIntersection = value; }
			}
		}

		#endregion

		private List<ReferenceHitTestInfo> _referenceHitTestInfos = new List<ReferenceHitTestInfo>();

		private CloneIntersectedItem SelectedCloneIntersectedItem
		{
			get
			{
				if (listView.FocusedItem == null)
					return null;

				return (CloneIntersectedItem)listView.FocusedItem.Tag;
			}
		}

		private static Brush[] GetCloneBrushes()
		{
			// TODO: Add more colors
			List<Brush> result = new List<Brush>();
			result.Add(Brushes.Red);
			result.Add(Brushes.Green);
			result.Add(Brushes.Yellow);
			result.Add(Brushes.Purple);
			result.Add(Brushes.Blue);
			result.Add(Brushes.Violet);
			result.Add(Brushes.Orange);
			result.Add(Brushes.Aqua);
			result.Add(Brushes.Chocolate);
			result.Add(Brushes.Gold);
			return result.ToArray();
		}

		private static SourceNode FindSourceNode(string name)
		{
			if (name == null)
				return null;

			if (!CloneDetectiveManager.IsCloneReportAvailable)
				return null;

			return CloneDetectiveManager.CloneDetectiveResult.SourceTree.FindNode(name);
		}

		private void UpdateSelection(string filePath)
		{
			SourceNode newSourceNode = FindSourceNode(filePath);
			UpdateIntersection(newSourceNode);
		}

		private void UpdateIntersection(SourceNode newSourceNode)
		{
			if (ReferenceEquals(_sourceNode, newSourceNode))
				return;

			listView.BeginUpdate();
			try
			{
				listView.Items.Clear();

				if (newSourceNode == null || newSourceNode.SourceFile == null)
				{
					fileNameLabel.Text = Res.NoCloneReportOrNoFileSelected;
					referencePictureBox.Visible = false;
				}
				else
				{

					fileNameLabel.Text = newSourceNode.Name;
					_cloneIntersectionResult = CloneDetectiveManager.CloneDetectiveResult.SourceTree.GetCloneIntersections(newSourceNode);

					_maximumLoc = _cloneIntersectionResult.Target.SourceNode.LinesOfCode;
					foreach (CloneIntersectedItem intersection in _cloneIntersectionResult.References)
						_maximumLoc = Math.Max(_maximumLoc, intersection.SourceNode.LinesOfCode);

					UpdateReferenceCloneVisualization();
					referencePictureBox.Visible = true;

					foreach (CloneIntersectedItem cloneIntersectedItem in _cloneIntersectionResult.References)
					{
						SourceNode sourceNode = cloneIntersectedItem.SourceNode;
						ListViewItem item = new ListViewItem();
						item.Text = sourceNode.Name;
						item.SubItems.Add(FormattingHelper.FormatInteger(cloneIntersectedItem.Clones.Count));
						item.SubItems.Add(String.Empty);
						item.Tag = cloneIntersectedItem;
						listView.Items.Add(item);
					}
				}
			}
			finally
			{
				listView.EndUpdate();
			}

			listView.Invalidate();
			_sourceNode = newSourceNode;
		}

		private void UpdateReferenceCloneVisualization()
		{
			if (_cloneIntersectionResult == null)
				return;

			Bitmap referenceCloneOverviewBitmap = new Bitmap(listView.Columns[2].Width, GetBoxHeight());
			Rectangle bounds = new Rectangle(0, 0, referenceCloneOverviewBitmap.Width - 1, GetBoxHeight());
			using (Graphics graphics = Graphics.FromImage(referenceCloneOverviewBitmap))
			{
				graphics.FillRectangle(SystemBrushes.Info, bounds);
				DrawCloneVisualization(graphics, bounds, _cloneIntersectionResult.Target);
			}

			referencePictureBox.Image = referenceCloneOverviewBitmap;
			referencePictureBox.Width = referenceCloneOverviewBitmap.Width;

			UpdateReferenceHitTestInfos(bounds);
		}

		private void UpdateReferenceHitTestInfos(Rectangle bounds)
		{
			_referenceHitTestInfos.Clear();

			int linesOfCode = _cloneIntersectionResult.Target.SourceNode.LinesOfCode;
			int totalWidth = (int)Math.Floor((double)(bounds.Width - 1) / _maximumLoc * linesOfCode);

			foreach (CloneIntersection cloneIntersection in _cloneIntersectionResult.Target.Clones)
			{
				Rectangle cloneRect = new Rectangle();
				cloneRect.X = (int) Math.Floor(bounds.X + (double)cloneIntersection.Clone.StartLine / linesOfCode * totalWidth);
				cloneRect.Width = (int) Math.Floor((double)cloneIntersection.Clone.LineCount / linesOfCode * totalWidth);
				cloneRect.Y = bounds.Top + 2;
				cloneRect.Height = bounds.Height - 5;

				ReferenceHitTestInfo referenceHitTestInfo = new ReferenceHitTestInfo();
				referenceHitTestInfo.Rectangle = cloneRect;
				referenceHitTestInfo.CloneIntersection = cloneIntersection;
				_referenceHitTestInfos.Add(referenceHitTestInfo);
			}

			_referenceHitTestInfos.Reverse();
		}

		private void UpdateReferenceCloneVisualizationOffset()
		{
			int offset = 0;
			for (int i = 0; i < 2; i++)
			{
				ColumnHeader columnHeader = listView.Columns[i];
				offset += columnHeader.Width;
			}
			referencePictureBox.Left = offset;
		}

		private void SelectCloneInEditor(Point point)
		{
			CloneIntersection cloneIntersection = FindRerefenceCloneIntersection(point);
			if (cloneIntersection != null)
			{
				Clone clone = cloneIntersection.Clone;
				VSPackage.Instance.SelectCloneInEditor(clone);
			}
		}

		private void OpenSelectedIntersection()
		{
			if (SelectedCloneIntersectedItem != null)
			{
				if (SelectedCloneIntersectedItem.SourceNode.SourceFile != null)
					VSPackage.Instance.OpenDocument(SelectedCloneIntersectedItem.SourceNode.SourceFile.Path);
			}
		}

		private void DrawCloneVisualization(Graphics graphics, Rectangle bounds, CloneIntersectedItem cloneIntersectedItem)
		{
			int linesOfCode = cloneIntersectedItem.SourceNode.LinesOfCode;
			int totalWidth = (int) Math.Floor((double)(bounds.Width - 1) / _maximumLoc * linesOfCode);

			Rectangle backgroundRect = new Rectangle();
			backgroundRect.X = bounds.X;
			backgroundRect.Y = bounds.Top;
			backgroundRect.Width = totalWidth - 1;
			backgroundRect.Height = bounds.Height - 1;
			graphics.FillRectangle(Brushes.LightGray, backgroundRect);

			foreach (CloneIntersection cloneIntersection in cloneIntersectedItem.Clones)
			{
				Rectangle cloneRect = new Rectangle();
				cloneRect.X = (int) Math.Floor(bounds.X + (double)cloneIntersection.Clone.StartLine / linesOfCode * totalWidth);
				cloneRect.Width = (int) Math.Floor((double)cloneIntersection.Clone.LineCount / linesOfCode * totalWidth);
				cloneRect.Y = bounds.Top;
				cloneRect.Height = bounds.Height;

				if (cloneRect.Right > bounds.X + totalWidth)
					cloneRect.Width = bounds.X + totalWidth - cloneRect.X;

				Brush brush = GetCloneIntersectonBrush(cloneIntersection);
				graphics.FillRectangle(brush, cloneRect);
			}

			graphics.DrawRectangle(Pens.Black, backgroundRect);
		}

		private CloneIntersection FindRerefenceCloneIntersection(Point point)
		{
			foreach (ReferenceHitTestInfo info in _referenceHitTestInfos)
			{
				if (info.Rectangle.Contains(point))
					return info.CloneIntersection;
			}

			return null;
		}

		private static Brush GetCloneIntersectonBrush(CloneIntersection cloneIntersection)
		{
			return _cloneBrushes[cloneIntersection.CloneClassId % _cloneBrushes.Length];
		}

		private int GetBoxHeight()
		{
			return referencePictureBox.Height - 2;
		}

		private void UpdateContextMenu(ContextMenuStrip contextMenu, CloneIntersectedItem cloneIntersectedItem)
		{
			contextMenu.Items.Clear();

			if (cloneIntersectedItem == null)
				return;

			List<CloneIntersection> clones = new List<CloneIntersection>(cloneIntersectedItem.Clones);
			clones.Sort(delegate(CloneIntersection x, CloneIntersection y)
			{
				int result = x.Clone.StartLine.CompareTo(y.Clone.StartLine);
				if (result == 0)
					result = x.Clone.LineCount.CompareTo(y.Clone.LineCount);
				return result;
			});

			Dictionary<CloneClass, Bitmap> cloneClassBitmaps = new Dictionary<CloneClass, Bitmap>();

			foreach (CloneIntersection cloneIntersection in clones)
			{
				CloneClass cloneClass = cloneIntersection.Clone.CloneClass;

				Bitmap cloneClassBitmap;
				if (!cloneClassBitmaps.TryGetValue(cloneClass, out cloneClassBitmap))
				{
					cloneClassBitmap = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
					using (Graphics g = Graphics.FromImage(cloneClassBitmap))
					{
						int height = GetBoxHeight() - 1;
						int width = cloneClassBitmap.Width - 1;
						int top = (int) Math.Floor(cloneClassBitmap.Height / 2.0 - height / 2.0);
						Rectangle rectangle = new Rectangle(0, top, width, height);

						Brush brush = GetCloneIntersectonBrush(cloneIntersection);
						g.FillRectangle(brush, rectangle);
						g.DrawRectangle(Pens.Black, rectangle);
					}
					cloneClassBitmaps.Add(cloneClass, cloneClassBitmap);
				}

				ToolStripMenuItem cloneItem = new ToolStripMenuItem();
				cloneItem.Text = FormattingHelper.FormatCloneForMenu(cloneIntersection.Clone);
				cloneItem.Image = cloneClassBitmap;
				contextMenu.Items.Add(cloneItem);

				ToolStripMenuItem openInEditorItem = new ToolStripMenuItem();
				openInEditorItem.Text = Res.MenuOpenInEditor;
				openInEditorItem.Tag = cloneIntersection.Clone;
				openInEditorItem.Click += openInEditorToolStripItem_Click;
				cloneItem.DropDownItems.Add(openInEditorItem);

				ToolStripMenuItem findAllOccurencesItem = new ToolStripMenuItem();
				findAllOccurencesItem.Text = Res.MenuFindAllOccurrences;
				findAllOccurencesItem.Tag = cloneIntersection.Clone;
				findAllOccurencesItem.Click += findAllOccurencesToolStripItem_Click;
				cloneItem.DropDownItems.Add(findAllOccurencesItem);
			}
		}

		private void UpdatePropertyGrid()
		{
			object selectedObject;

			if (SelectedCloneIntersectedItem == null)
				selectedObject = null;
			else
				selectedObject = new CloneIntersectionSummary(SelectedCloneIntersectedItem);

			UpdatePropertyGrid(selectedObject);
		}

		private static void openInEditorToolStripItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem) sender;
			Clone clone = (Clone) item.Tag;
			VSPackage.Instance.SelectCloneInEditor(clone);
		}

		private static void findAllOccurencesToolStripItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			Clone clone = (Clone)item.Tag;
			CloneDetectiveManager.FindClones(clone.CloneClass);
		}

		private void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			UpdateReferenceCloneVisualization();
			UpdateReferenceCloneVisualizationOffset();
		}

		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdatePropertyGrid();
		}

		private void listView_DoubleClick(object sender, EventArgs e)
		{
			OpenSelectedIntersection();
		}

		private void listView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
				OpenSelectedIntersection();
		}

		private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawDefault = false;
		}

		private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			if (e.ColumnIndex < 2)
			{
				e.DrawDefault = true;
			}
			else
			{
				e.DrawDefault = false;
				CloneIntersectedItem cloneIntersectedItem = (CloneIntersectedItem)e.Item.Tag;

				if (e.Item.Selected && listView.Focused)
					e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
				else
					e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

				Rectangle cloneRect = e.Bounds;
				int height = GetBoxHeight();
				if (height < e.Bounds.Height)
				{
					cloneRect.Y = (int)Math.Floor(e.Bounds.Top + e.Bounds.Height / 2.0 - height / 2.0);
					cloneRect.Height = height;
				}

				DrawCloneVisualization(e.Graphics, cloneRect, cloneIntersectedItem);
			}
		}

		private void referencePictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			SelectCloneInEditor(new Point(e.X, e.Y));
		}

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			UpdateContextMenu(referenceContextMenuStrip, _cloneIntersectionResult.Target);
			e.Cancel = (referenceContextMenuStrip.Items.Count == 0);
		}

		private void listViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			UpdateContextMenu(listViewContextMenuStrip, SelectedCloneIntersectedItem);
			e.Cancel = (listViewContextMenuStrip.Items.Count == 0);
		}
	}
}
