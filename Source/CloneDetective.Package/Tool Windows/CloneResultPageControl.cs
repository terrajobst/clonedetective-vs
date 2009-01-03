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

			dataGridView.Sort(dataGridView.Columns[1], ListSortDirection.Descending);
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

			dataGridView.Rows.Clear();

			foreach (CloneGroup cloneGroup in cloneGroups)
			{
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(dataGridView);
				row.Cells[0].Value = Path.GetFileName(cloneGroup.SourceFile.Path);
				row.Cells[1].Value = cloneGroup.Clones.Count;
				row.Cells[2].Value = GetCloneOverviewBitmap(cloneGroup);
				row.Tag = cloneGroup;
				dataGridView.Rows.Add(row);
			}

			dataGridView.Sort(dataGridView.SortedColumn, GetSortDirectionFromSortOrder(dataGridView.SortOrder));
		}

		private static ListSortDirection GetSortDirectionFromSortOrder(SortOrder sortOrder)
		{
			return sortOrder == SortOrder.Ascending
					? ListSortDirection.Ascending
					: ListSortDirection.Descending;
		}

		private CloneGroup SelectedCloneGroup
		{
			get
			{
				if (dataGridView.SelectedRows.Count == 0)
					return null;

				return (CloneGroup)dataGridView.SelectedRows[0].Tag;
			}
		}

		private void OpenSelectedClone()
		{
			if (SelectedCloneGroup != null)
				VSPackage.Instance.SelectCloneInEditor(SelectedCloneGroup.Clones[0]);
		}

		private static int GetLinesOfCode(SourceFile sourceFile)
		{
			if (!CloneDetectiveManager.IsCloneReportAvailable)
				return 1; // Since an empty text file contains at least one line.

			SourceNode sourceNode = CloneDetectiveManager.CloneDetectiveResult.SourceTree.FindNode(sourceFile.Path);
			return sourceNode.LinesOfCode;
		}

		private void UpdateCloneVisualizations()
		{
			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				CloneGroup cloneGroup = (CloneGroup) row.Tag;
				Bitmap oldBitmap = (Bitmap)row.Cells[2].Value;
				if (oldBitmap != null)
					oldBitmap.Dispose();

				row.Cells[2].Value = GetCloneOverviewBitmap(cloneGroup);
			}
		}

		private object GetCloneOverviewBitmap(CloneGroup group)
		{
			Bitmap bitmap = new Bitmap(dataGridView.Columns[2].Width, 10);
			Rectangle bounds = new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				int linesOfCode = GetLinesOfCode(group.SourceFile);
				int totalWidth = (int) Math.Floor((double) (bounds.Width - 1)/_maximumLoc*linesOfCode);

				Rectangle backgroundRect = new Rectangle();
				backgroundRect.X = bounds.X;
				backgroundRect.Y = bounds.Top;
				backgroundRect.Width = totalWidth - 1;
				backgroundRect.Height = bounds.Height - 1;
				graphics.FillRectangle(Brushes.LightGray, backgroundRect);

				// TODO: Use same color as marker
				Brush brush = Brushes.Purple;
				foreach (Clone clone in group.Clones)
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
			return bitmap;
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

		private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			OpenSelectedClone();
		}

		private void dataGridView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
				OpenSelectedClone();
		}

		private void dataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{
			if (e.Column.Index == 2)
				UpdateCloneVisualizations();
		}

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			UpdateContextMenu();
			e.Cancel = (contextMenuStrip.Items.Count == 0);
		}
	}
}