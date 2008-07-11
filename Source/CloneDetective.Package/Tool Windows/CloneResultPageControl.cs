using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using CloneDetective.CloneReporting;

namespace CloneDetective.Package
{
	public partial class CloneResultPageControl : UserControl
	{
		private CloneClass _cloneClass;
		private int _maximumLoc;
		private static Bitmap _cloneBitmap = CreateCloneBitmap();

		public CloneResultPageControl()
		{
			InitializeComponent();
		}

		private static Bitmap CreateCloneBitmap()
		{
			// TODO: Use same color as marker
			Brush brush = Brushes.Purple;

			Bitmap bitmap = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				int height = 9;
				int width = bitmap.Width - 1;
				int top = (int) Math.Floor(bitmap.Height/2.0 - height/2.0);
				Rectangle rectangle = new Rectangle(0, top, width, height);
				g.FillRectangle(brush, rectangle);
				g.DrawRectangle(Pens.Black, rectangle);
			}

			return bitmap;
		}

		public CloneClass CloneClass
		{
			get { return _cloneClass; }
			set
			{
				_cloneClass = value;
				LoadClones();
			}
		}

		private sealed class CloneGroup
		{
			public SourceFile SourceFile;
			public List<Clone> Clones = new List<Clone>();
		}

		private void LoadClones()
		{
			_maximumLoc = Int32.MinValue;

			List<CloneGroup> cloneGroups = new List<CloneGroup>();
			Dictionary<SourceFile, CloneGroup> cloneGroupDictionary = new Dictionary<SourceFile, CloneGroup>();
			foreach (Clone clone in _cloneClass.Clones)
			{
				CloneGroup cloneGroup;
				if (!cloneGroupDictionary.TryGetValue(clone.SourceFile, out cloneGroup))
				{
					_maximumLoc = Math.Max(_maximumLoc, GetLinesOfCode(clone.SourceFile));
					cloneGroup = new CloneGroup();
					cloneGroup.SourceFile = clone.SourceFile;
					cloneGroupDictionary.Add(cloneGroup.SourceFile, cloneGroup);
					cloneGroups.Add(cloneGroup);
				}

				cloneGroup.Clones.Add(clone);
			}

			listView.BeginUpdate();
			try
			{
				listView.Items.Clear();

				foreach (CloneGroup cloneGroup in cloneGroups)
				{
					ListViewItem item = new ListViewItem();
					item.Text = Path.GetFileName(cloneGroup.SourceFile.Path);
					item.SubItems.Add(FormattingHelper.FormatInteger(cloneGroup.Clones.Count));
					item.SubItems.Add(String.Empty);
					item.Tag = cloneGroup;
					listView.Items.Add(item);
				}
			}
			finally
			{
				listView.EndUpdate();				
			}
		}

		private CloneGroup SelectedCloneGroup
		{
			get
			{
				if (listView.FocusedItem == null)
					return null;

				return (CloneGroup)listView.FocusedItem.Tag;
			}
		}

		private void OpenSelectedClone()
		{
			if (SelectedCloneGroup != null)
				VSPackage.Instance.SelectCloneInEditor(SelectedCloneGroup.Clones[0]);
		}

		private static int GetLinesOfCode(SourceFile sourceFile)
		{
			SourceNode sourceNode = CloneDetectiveManager.CloneDetectiveResult.SourceTree.FindNode(sourceFile.Path);
			return sourceNode.LinesOfCode;
		}

		private void DrawClones(Graphics graphics, Rectangle bounds, CloneGroup cloneGroup)
		{
			int linesOfCode = GetLinesOfCode(cloneGroup.SourceFile);
			int totalWidth = (int) Math.Floor((double) (bounds.Width - 1)/_maximumLoc*linesOfCode);

			Rectangle backgroundRect = new Rectangle();
			backgroundRect.X = bounds.X;
			backgroundRect.Y = bounds.Top;
			backgroundRect.Width = totalWidth - 1;
			backgroundRect.Height = bounds.Height - 1;
			graphics.FillRectangle(Brushes.LightGray, backgroundRect);

			// TODO: Use same color as marker
			Brush brush = Brushes.Purple;
			foreach (Clone clone in cloneGroup.Clones)
			{
				Rectangle cloneRect = new Rectangle();
				cloneRect.X = (int) Math.Floor(bounds.X + (double) clone.StartLine/linesOfCode*totalWidth);
				cloneRect.Width = (int) Math.Floor((double) clone.LineCount/linesOfCode*totalWidth);
				cloneRect.Y = bounds.Top;
				cloneRect.Height = bounds.Height;

				if (cloneRect.Right > bounds.X + totalWidth)
					cloneRect.Width = bounds.X + totalWidth - cloneRect.X;

				graphics.FillRectangle(brush, cloneRect);
			}


			graphics.DrawRectangle(Pens.Black, backgroundRect);
		}

		private void UpdateContextMenu()
		{
			contextMenuStrip.Items.Clear();

			if (SelectedCloneGroup != null)
			{
				foreach (Clone clone in SelectedCloneGroup.Clones)
				{
					ToolStripMenuItem item = new ToolStripMenuItem();
					item.Image = _cloneBitmap;
					item.Text = FormattingHelper.FormatCloneForMenu(clone);
					item.Tag = clone;
					item.Click += openCloneMenuItem_OnClick;
					contextMenuStrip.Items.Add(item);
				}
			}
		}

		private static void openCloneMenuItem_OnClick(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem) sender;
			Clone clone = (Clone) menuItem.Tag;
			VSPackage.Instance.SelectCloneInEditor(clone);
		}

		private void listView_DoubleClick(object sender, EventArgs e)
		{
			OpenSelectedClone();
		}

		private void listView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
				OpenSelectedClone();
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
			const int boxHeight = 10;

			if (e.ColumnIndex < 2)
			{
				e.DrawDefault = true;
			}
			else
			{
				e.DrawDefault = false;
				CloneGroup cloneGroup = (CloneGroup)e.Item.Tag;

				if (e.Item.Selected && listView.Focused)
					e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
				else
					e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

				Rectangle cloneRect = e.Bounds;
				if (boxHeight < e.Bounds.Height)
				{
					cloneRect.Y = e.Bounds.Top + (int)Math.Ceiling(e.Bounds.Height / 2.0 - boxHeight / 2.0);
					cloneRect.Height = boxHeight;
				}

				DrawClones(e.Graphics, cloneRect, cloneGroup);
			}
		}

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			UpdateContextMenu();
			e.Cancel = (contextMenuStrip.Items.Count == 0);
		}
	}
}