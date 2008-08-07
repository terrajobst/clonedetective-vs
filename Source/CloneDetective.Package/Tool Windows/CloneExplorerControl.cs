using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using CloneDetective.CloneReporting;

using Microsoft.Win32;

namespace CloneDetective.Package
{
	public partial class CloneExplorerControl : ToolWindowUserControl
	{
		#region RollupType

		private enum RollupType
		{
			None,
			NumberOfClones,
			NumberOfCloneClasses,
			NumberOfClonedLines,
			NumberOfLines,
			ClonePercentage
		}

		#endregion

		public CloneExplorerControl()
		{
			InitializeComponent();
		}

		protected override void Initialize()
		{
			SetToolStripRenderer(toolStrip);

			imageList.Images.Add(Res.FolderClosed);
			imageList.Images.Add(Res.FolderOpened);
			imageList.Images.Add(Res.CSharpFile);
			imageList.Images.Add(Res.Solution);
			LoadSettings();

			UpdateUI();

			EventHandler<EventArgs> updateUIHandler = delegate { UpdateUI(); };
			CloneDetectiveManager.CloneDetectiveResultChanged += updateUIHandler;
			VSPackage.Instance.SolutionOpened += updateUIHandler;
			VSPackage.Instance.SolutionClosed += updateUIHandler;

			UpdatePropertyGrid();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void LoadSettings()
		{
			RollupType rollupType = RollupType.ClonePercentage;

			try
			{
				using (RegistryKey rootKey = VSPackage.Instance.UserRegistryRoot)
				using (RegistryKey ourKey = rootKey.OpenSubKey(VSPackage.ProductName))
				{
					if (ourKey != null)
					{
						object value = ourKey.GetValue("CloneExplorerRollup");
						if (value != null)
						{
							string rollupTypeAsString = Convert.ToString(value, CultureInfo.InvariantCulture);
							RollupType parsedRollupType = (RollupType) Enum.Parse(typeof(RollupType), rollupTypeAsString);
							if (Enum.IsDefined(typeof(RollupType), parsedRollupType))
								rollupType = parsedRollupType;
						}
					}
				}
			}
			catch
			{
				// Ignore any errors since it's not a big deal if we can't read
				// this setting. Not catching it would cause ugly error messages
				// in Visual Studio.
			}

			SelectedRollupType = rollupType;
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void SaveSettings()
		{
			try
			{
				using (RegistryKey rootKey = VSPackage.Instance.UserRegistryRoot)
				using (RegistryKey ourKey = rootKey.CreateSubKey(VSPackage.ProductName))
				{
					ourKey.SetValue("CloneExplorerRollup", SelectedRollupType, RegistryValueKind.String);
				}
			}
			catch
			{
				// Ignore any errors. As in the Load case it's not a big deal if
				// we can't persist this setting.
			}
		}

		private enum UIState
		{
			NoSolution,
			Solution,
			SolutionAndRunning,
		}

		private static UIState CurrentUIState
		{
			get
			{
				if (VSPackage.Instance.GetSolutionPath() == null)
					return UIState.NoSolution;

				if (CloneDetectiveManager.IsCloneDetectiveRunning)
					return UIState.SolutionAndRunning;

				return UIState.Solution;
			}
		}

		private void UpdateUI()
		{
			CloneDetectiveResult result = CloneDetectiveManager.CloneDetectiveResult;

			switch (CurrentUIState)
			{
				case UIState.NoSolution:
					runToolStripButton.Enabled = false;
					stopToolStripButton.Enabled = false;
					settingsToolStripButton.Enabled = false;
					importToolStripButton.Enabled = false;
					exportToolStripButton.Enabled = false;
					closeToolStripButton.Enabled = false;
					rollupTypeToolStripComboBox.Enabled = false;
					resultTableLayoutPanel.Visible = false;
					pictureBox.Image = null;
					runningLabel.Visible = false;
					progressBar.Visible = false;
					break;
				case UIState.SolutionAndRunning:
					runToolStripButton.Enabled = false;
					stopToolStripButton.Enabled = true;
					settingsToolStripButton.Enabled = false;
					importToolStripButton.Enabled = false;
					exportToolStripButton.Enabled = false;
					closeToolStripButton.Enabled = false;
					rollupTypeToolStripComboBox.Enabled = false;
					resultTableLayoutPanel.Visible = false;
					pictureBox.Image = Res.Running;
					runningLabel.Visible = true;
					progressBar.Visible = true;
					break;
				case UIState.Solution:
					runToolStripButton.Enabled = true;
					stopToolStripButton.Enabled = false;
					settingsToolStripButton.Enabled = true;
					importToolStripButton.Enabled = true;
					rollupTypeToolStripComboBox.Enabled = true;
					runningLabel.Visible = false;
					progressBar.Visible = false;
					if (result == null)
					{
						exportToolStripButton.Enabled = false;
						closeToolStripButton.Enabled = false;
						rollupTypeToolStripComboBox.Enabled = false;
						resultTableLayoutPanel.Visible = false;
						pictureBox.Image = null;
					}
					else
					{
						closeToolStripButton.Enabled = true;
						resultTableLayoutPanel.Visible = true;
						switch (result.Status)
						{
							case CloneDetectiveResultStatus.Succeeded:
								exportToolStripButton.Enabled = true;
								rollupTypeToolStripComboBox.Enabled = true;
								pictureBox.Image = Res.Succeeded;
								resultLinkLabel.Text = Res.CloneExplorerSucceeded;
								timeLabel.Text = String.Format(CultureInfo.CurrentCulture, Res.CloneExplorerTimeStatistic, FormattingHelper.FormatTime(result.UsedTime));
								memoryLabel.Text = String.Format(CultureInfo.CurrentCulture, Res.CloneExplorerMemoryStatistic, FormattingHelper.FormatMemory(result.UsedMemory));
								timeLabel.Visible = true;
								memoryLabel.Visible = true;
								break;
							case CloneDetectiveResultStatus.Failed:
								exportToolStripButton.Enabled = false;
								rollupTypeToolStripComboBox.Enabled = false;
								pictureBox.Image = Res.Error;
								resultLinkLabel.Text = Res.CloneExplorerFailed;
								timeLabel.Visible = false;
								memoryLabel.Visible = false;
								break;
							case CloneDetectiveResultStatus.Stopped:
								exportToolStripButton.Enabled = false;
								rollupTypeToolStripComboBox.Enabled = false;
								pictureBox.Image = Res.Stopped;
								resultLinkLabel.Text = Res.CloneExplorerStopped;
								timeLabel.Visible = false;
								memoryLabel.Visible = false;
								break;
							default:
								throw ExceptionBuilder.UnhandledCaseLabel(result.Status);
						}
					}
					break;
				default:
					throw ExceptionBuilder.UnhandledCaseLabel(CurrentUIState);
			}

			UpdateTreeView();
		}

		private RollupType SelectedRollupType
		{
			get { return (RollupType)rollupTypeToolStripComboBox.SelectedIndex; }
			set
			{
				rollupTypeToolStripComboBox.SelectedIndex = (int)value;
				treeView.Invalidate();
			}
		}

		private SourceNode SelectedSourceNode
		{
			get
			{
				if (treeView.SelectedNode == null)
					return null;

				return (SourceNode)treeView.SelectedNode.Tag;
			}
		}

		private void RunCloneDetective()
		{
			bool started = CloneDetectiveManager.RunCloneDetective(
				delegate(object sender, CloneDetectiveCompletedEventArgs e)
					{
						if (e.Exception != null)
							VSPackage.Instance.ShowError(e.Exception.Message);
						UpdateUI();
					});

			if (started)
				UpdateUI();
		}

		private static void AbortCloneDetective()
		{
			CloneDetectiveManager.AbortCloneDetective();
		}

		private void ExportCloneDetectiveResults()
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
				CloneDetectiveManager.ExportCloneDetectiveResults(saveFileDialog.FileName);
		}

		private void ImportCloneDetectiveResults()
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
				CloneDetectiveManager.ImportCloneDetectiveResults(openFileDialog.FileName);
		}

		private static void CloseCloneDetectiveResults()
		{
			CloneDetectiveManager.CloseCloneDetectiveResults();
		}

		private void UpdateTreeView()
		{
			CloneDetectiveResult result = CloneDetectiveManager.CloneDetectiveResult;
			UpdateTreeView(treeView, result);
		}

		private static void UpdateTreeView(TreeView treeView, CloneDetectiveResult result)
		{
			treeView.BeginUpdate();
			try
			{
				bool wasEmpty = (treeView.Nodes.Count == 0);

				if (result == null || result.Status != CloneDetectiveResultStatus.Succeeded)
					treeView.Nodes.Clear();
				else
				{
					List<SourceNode> nodesToAdd = new List<SourceNode>();
					nodesToAdd.Add(result.SourceTree.Root);

					HashSet<string> fileSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					while (nodesToAdd.Count > 0)
					{
						SourceNode nodeToAdd = nodesToAdd[0];
						nodesToAdd.RemoveAt(0);
						fileSet.Add(nodeToAdd.FullPath);
						nodesToAdd.AddRange(nodeToAdd.Children);
					}

					UpdateTreeView(treeView.Nodes, new SourceNode[] { result.SourceTree.Root }, fileSet);
				}

				if (wasEmpty && treeView.Nodes.Count > 0)
					treeView.Nodes[0].Expand();
			}
			finally
			{
				treeView.EndUpdate();
			}
		}

		private static void UpdateTreeView(TreeNodeCollection treeNodes, IList<SourceNode> sourceNodes, HashSet<string> fileSet)
		{
			int index = 0;

			while (true)
			{
				if (index == treeNodes.Count)
				{
					// Add remaining source nodes to end of treeNodes.
					for (; index < sourceNodes.Count; index++)
					{
						AddTreeNode(treeNodes, index, sourceNodes[index]);
						UpdateTreeView(treeNodes[index].Nodes, sourceNodes[index].Children, fileSet);
					}

					return;
				}

				if (index == sourceNodes.Count)
				{
					// Remove remaining tree nodes.
					while (index < treeNodes.Count)
						treeNodes.RemoveAt(index);

					return;
				}

				SourceNode oldNode = (SourceNode) treeNodes[index].Tag;
				SourceNode newNode = sourceNodes[index];

				if (oldNode.FullPath == newNode.FullPath)
				{
					// The nodes refer to the same file. So just replace the source
					// node, everything will be fine afterwards.
					treeNodes[index].Tag = newNode;
					UpdateTreeView(treeNodes[index].Nodes, newNode.Children, fileSet);
					index++;
				}
				else
				{
					// The nodes do not match. Now there are two possibilities: either
					// oldNode does not exist any more or newNode has been added.
					if (fileSet.Contains(oldNode.FullPath))
					{
						// Add tree node at index position.
						AddTreeNode(treeNodes, index, newNode);
						UpdateTreeView(treeNodes[index].Nodes, newNode.Children, fileSet);
						index++;
					}
					else
					{
						treeNodes.RemoveAt(index);
					}
				}
			}
		}

		private static void AddTreeNode(TreeNodeCollection nodes, int index, SourceNode node)
		{
			int imageIndex;

			if (node.SourceFile != null)
				imageIndex = 2;
			else if (node.Parent == null)
				imageIndex = 3;
			else
				imageIndex = 0;

			TreeNode treeNode = new TreeNode();
			treeNode.Text = node.Name;
			treeNode.ImageIndex = treeNode.SelectedImageIndex = imageIndex;
			treeNode.Tag = node;
			nodes.Insert(index, treeNode);
		}

		private string GetRollupLabel(TreeNode treeNode)
		{
			SourceNode node = (SourceNode)treeNode.Tag;
			string rollupValue = FormatRollupValue(node, SelectedRollupType);
			if (String.IsNullOrEmpty(rollupValue))
				return null;

			return String.Format(CultureInfo.CurrentCulture, "({0})", rollupValue);
		}

		private static string FormatRollupValue(SourceNode node, RollupType rollupType)
		{
			switch (rollupType)
			{
				case RollupType.None:
					return String.Empty;
				case RollupType.NumberOfClones:
					if (node.NumberOfClones == 0)
						return String.Empty;
					return FormattingHelper.FormatInteger(node.NumberOfClones);
				case RollupType.NumberOfCloneClasses:
					if (node.NumberOfCloneClasses == 0)
						return String.Empty;
					return FormattingHelper.FormatInteger(node.NumberOfCloneClasses);
				case RollupType.NumberOfClonedLines:
					if (node.NumberOfClonedLines == 0)
						return String.Empty;
					return FormattingHelper.FormatInteger(node.NumberOfClonedLines);
				case RollupType.NumberOfLines:
					if (node.LinesOfCode == 0)
						return String.Empty;
					return FormattingHelper.FormatInteger(node.LinesOfCode);
				case RollupType.ClonePercentage:
					if (Math.Abs(node.ClonePercentage - Double.Epsilon) <= Double.Epsilon)
						return String.Empty;
					return FormattingHelper.FormatPercentage(node.ClonePercentage);
				default:
					throw ExceptionBuilder.UnhandledCaseLabel(rollupType);
			}
		}

		private void OpenSelectedSourceNode()
		{
			SourceNode sourceNode = SelectedSourceNode;
			if (sourceNode.SourceFile != null)
				VSPackage.Instance.OpenDocument(sourceNode.SourceFile.Path);
		}

		private void UpdatePropertyGrid()
		{
			object selectedObject;

			if (SelectedSourceNode == null)
				selectedObject = null;
			else
				selectedObject = new CloneFileSummary(SelectedSourceNode);

			UpdatePropertyGrid(selectedObject);
		}

		private void runToolStripButton_Click(object sender, EventArgs e)
		{
			RunCloneDetective();
		}

		private void stopToolStripButton_Click(object sender, EventArgs e)
		{
			AbortCloneDetective();
		}

		private void exportToolStripButton_Click(object sender, EventArgs e)
		{
			ExportCloneDetectiveResults();
		}

		private void importToolStripButton_Click(object sender, EventArgs e)
		{
			ImportCloneDetectiveResults();
		}

		private void closeToolStripButton_Click(object sender, EventArgs e)
		{
			CloseCloneDetectiveResults();
		}

		private void settingsToolStripButton_Click(object sender, EventArgs e)
		{
			using (SolutionSettingsForm dlg = new SolutionSettingsForm(VSPackage.Instance.GetSolutionPath()))
				dlg.ShowDialog();
		}

		private void rollupTypeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			treeView.Invalidate();
		}

		private void resultLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (File.Exists(CloneDetectiveManager.ConqatLogFileName))
				VSPackage.Instance.OpenDocumentReadOnly(CloneDetectiveManager.ConqatLogFileName);
		}

		private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Cancel = e.Node.Parent == null;
		}

		private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Parent != null)
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
		}

		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Parent != null)
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdatePropertyGrid();
		}

		private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (!e.Node.IsVisible)
				return;

			Font font = e.Node.TreeView.Font;
			bool selected = e.Node.IsSelected;
			bool focused = (e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused;
			Brush background;
			Color foreColor;
			Color backColor;
			if (selected && treeView.Focused)
			{
				background = SystemBrushes.Highlight;
				backColor = SystemColors.Highlight;
				foreColor = SystemColors.HighlightText;
			}
			else if (selected)
			{
				background = SystemBrushes.Control;
				backColor = SystemColors.Control;
				foreColor = SystemColors.ControlText;
			}
			else
			{
				background = SystemBrushes.Window;
				backColor = SystemColors.Window;
				foreColor = SystemColors.WindowText;
			}

			string text = e.Node.Text;
			string rollupLabel = GetRollupLabel(e.Node);
			Size textSize = TextRenderer.MeasureText(e.Graphics, text, font);
			Point textPosition = new Point(e.Bounds.Left, e.Bounds.Top + (e.Bounds.Height - textSize.Height) / 2);
			Point labelPosition = new Point(e.Bounds.Left + textSize.Width, textPosition.Y);

			Rectangle backgroundRect = e.Bounds;
			backgroundRect.Width++;
			e.Graphics.FillRectangle(background, backgroundRect);
			TextRenderer.DrawText(e.Graphics, text, font, textPosition, foreColor);

			if (focused)
				ControlPaint.DrawFocusRectangle(e.Graphics, backgroundRect, foreColor, backColor);

			Rectangle remainingRect = e.Bounds;
			remainingRect.X = e.Bounds.Right + 1;
			remainingRect.Width = treeView.Width - e.Bounds.Right;
			e.Graphics.FillRectangle(SystemBrushes.Window, remainingRect);

			if (!String.IsNullOrEmpty(rollupLabel))
				TextRenderer.DrawText(e.Graphics, rollupLabel, font, labelPosition, Color.Blue);
		}

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			OpenSelectedSourceNode();
		}

		private void treeView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
				OpenSelectedSourceNode();
		}
	}
}
