namespace CloneDetective.Package
{
	partial class CloneExplorerControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			SaveSettings();

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.borderPanel1 = new CloneDetective.Package.BorderPanel();
			this.treeView = new System.Windows.Forms.TreeView();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showCloneIntersectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.borderPanel2 = new CloneDetective.Package.BorderPanel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.runToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.settingsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exportToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.importToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.closeToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.rollupTypeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.borderPanel3 = new CloneDetective.Package.BorderPanel();
			this.resultTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.memoryLabel = new System.Windows.Forms.Label();
			this.timeLabel = new System.Windows.Forms.Label();
			this.resultLinkLabel = new System.Windows.Forms.LinkLabel();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.runningLabel = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.borderPanel1.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.borderPanel2.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.borderPanel3.SuspendLayout();
			this.resultTableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// borderPanel1
			// 
			this.borderPanel1.BorderSides = ((System.Windows.Forms.Border3DSide)(((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)
						| System.Windows.Forms.Border3DSide.Bottom)
						| System.Windows.Forms.Border3DSide.Middle)));
			this.borderPanel1.Controls.Add(this.treeView);
			this.borderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.borderPanel1.Location = new System.Drawing.Point(0, 49);
			this.borderPanel1.Name = "borderPanel1";
			this.borderPanel1.Padding = new System.Windows.Forms.Padding(1);
			this.borderPanel1.Size = new System.Drawing.Size(343, 166);
			this.borderPanel1.TabIndex = 3;
			// 
			// treeView
			// 
			this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView.ContextMenuStrip = this.contextMenuStrip;
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.treeView.HideSelection = false;
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageList;
			this.treeView.Location = new System.Drawing.Point(1, 1);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = 0;
			this.treeView.ShowRootLines = false;
			this.treeView.Size = new System.Drawing.Size(341, 164);
			this.treeView.TabIndex = 1;
			this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
			this.treeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCollapse);
			this.treeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseClick);
			this.treeView.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView_DrawNode);
			this.treeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCollapse);
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
			this.treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterExpand);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.showCloneIntersectionsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.propertiesToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(208, 98);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = global::CloneDetective.Package.Res.Open;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// showCloneIntersectionsToolStripMenuItem
			// 
			this.showCloneIntersectionsToolStripMenuItem.Image = global::CloneDetective.Package.Res.CloneIntersections;
			this.showCloneIntersectionsToolStripMenuItem.Name = "showCloneIntersectionsToolStripMenuItem";
			this.showCloneIntersectionsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.showCloneIntersectionsToolStripMenuItem.Text = "Show Clone Intersections";
			this.showCloneIntersectionsToolStripMenuItem.Click += new System.EventHandler(this.showCloneIntersectionsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(204, 6);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.Image = global::CloneDetective.Package.Res.Properties;
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.propertiesToolStripMenuItem.Text = "Properties";
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(256, 5);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(83, 13);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 2;
			// 
			// borderPanel2
			// 
			this.borderPanel2.AutoSize = true;
			this.borderPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.borderPanel2.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)));
			this.borderPanel2.Controls.Add(this.toolStrip);
			this.borderPanel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.borderPanel2.Location = new System.Drawing.Point(0, 0);
			this.borderPanel2.Name = "borderPanel2";
			this.borderPanel2.Padding = new System.Windows.Forms.Padding(1, 1, 1, 0);
			this.borderPanel2.Size = new System.Drawing.Size(343, 26);
			this.borderPanel2.TabIndex = 4;
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripButton,
            this.stopToolStripButton,
            this.settingsToolStripButton,
            this.toolStripSeparator2,
            this.exportToolStripButton,
            this.importToolStripButton,
            this.closeToolStripButton,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.rollupTypeToolStripComboBox});
			this.toolStrip.Location = new System.Drawing.Point(1, 1);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(341, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			// 
			// runToolStripButton
			// 
			this.runToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.runToolStripButton.Image = global::CloneDetective.Package.Res.Run;
			this.runToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.runToolStripButton.Name = "runToolStripButton";
			this.runToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.runToolStripButton.Text = "Run Clone Detective";
			this.runToolStripButton.Click += new System.EventHandler(this.runToolStripButton_Click);
			// 
			// stopToolStripButton
			// 
			this.stopToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stopToolStripButton.Image = global::CloneDetective.Package.Res.Stop;
			this.stopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stopToolStripButton.Name = "stopToolStripButton";
			this.stopToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.stopToolStripButton.Text = "Stop Clone Detective";
			this.stopToolStripButton.Click += new System.EventHandler(this.stopToolStripButton_Click);
			// 
			// settingsToolStripButton
			// 
			this.settingsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.settingsToolStripButton.Image = global::CloneDetective.Package.Res.Settings;
			this.settingsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.settingsToolStripButton.Name = "settingsToolStripButton";
			this.settingsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.settingsToolStripButton.Text = "Edit Solution Settings";
			this.settingsToolStripButton.Click += new System.EventHandler(this.settingsToolStripButton_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// exportToolStripButton
			// 
			this.exportToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.exportToolStripButton.Image = global::CloneDetective.Package.Res.Export;
			this.exportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.exportToolStripButton.Name = "exportToolStripButton";
			this.exportToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.exportToolStripButton.Text = "Export Clone Detective Results";
			this.exportToolStripButton.Click += new System.EventHandler(this.exportToolStripButton_Click);
			// 
			// importToolStripButton
			// 
			this.importToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.importToolStripButton.Image = global::CloneDetective.Package.Res.Import;
			this.importToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.importToolStripButton.Name = "importToolStripButton";
			this.importToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.importToolStripButton.Text = "Import Clone Detective Results";
			this.importToolStripButton.Click += new System.EventHandler(this.importToolStripButton_Click);
			// 
			// closeToolStripButton
			// 
			this.closeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.closeToolStripButton.Image = global::CloneDetective.Package.Res.CloseResults;
			this.closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.closeToolStripButton.Name = "closeToolStripButton";
			this.closeToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.closeToolStripButton.Text = "Close Clone Detective Results";
			this.closeToolStripButton.Click += new System.EventHandler(this.closeToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(44, 22);
			this.toolStripLabel1.Text = "Rollup:";
			// 
			// rollupTypeToolStripComboBox
			// 
			this.rollupTypeToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.rollupTypeToolStripComboBox.Items.AddRange(new object[] {
            "None",
            "Number of Clones",
            "Number of Clone Classes",
            "Number of Cloned Lines",
            "Number of Lines",
            "Clone Percentage"});
			this.rollupTypeToolStripComboBox.Name = "rollupTypeToolStripComboBox";
			this.rollupTypeToolStripComboBox.Size = new System.Drawing.Size(140, 25);
			this.rollupTypeToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.rollupTypeToolStripComboBox_SelectedIndexChanged);
			// 
			// borderPanel3
			// 
			this.borderPanel3.BackColor = System.Drawing.SystemColors.Info;
			this.borderPanel3.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)));
			this.borderPanel3.Controls.Add(this.resultTableLayoutPanel);
			this.borderPanel3.Controls.Add(this.pictureBox);
			this.borderPanel3.Controls.Add(this.runningLabel);
			this.borderPanel3.Controls.Add(this.progressBar);
			this.borderPanel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.borderPanel3.Location = new System.Drawing.Point(0, 26);
			this.borderPanel3.Name = "borderPanel3";
			this.borderPanel3.Padding = new System.Windows.Forms.Padding(1, 1, 1, 0);
			this.borderPanel3.Size = new System.Drawing.Size(343, 23);
			this.borderPanel3.TabIndex = 3;
			// 
			// resultTableLayoutPanel
			// 
			this.resultTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.resultTableLayoutPanel.ColumnCount = 5;
			this.resultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.resultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.resultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.resultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.resultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.resultTableLayoutPanel.Controls.Add(this.memoryLabel, 4, 0);
			this.resultTableLayoutPanel.Controls.Add(this.timeLabel, 2, 0);
			this.resultTableLayoutPanel.Controls.Add(this.resultLinkLabel, 0, 0);
			this.resultTableLayoutPanel.Location = new System.Drawing.Point(22, 5);
			this.resultTableLayoutPanel.Name = "resultTableLayoutPanel";
			this.resultTableLayoutPanel.RowCount = 1;
			this.resultTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.resultTableLayoutPanel.Size = new System.Drawing.Size(317, 13);
			this.resultTableLayoutPanel.TabIndex = 2;
			// 
			// memoryLabel
			// 
			this.memoryLabel.AutoSize = true;
			this.memoryLabel.Location = new System.Drawing.Point(138, 0);
			this.memoryLabel.Margin = new System.Windows.Forms.Padding(0);
			this.memoryLabel.Name = "memoryLabel";
			this.memoryLabel.Size = new System.Drawing.Size(50, 13);
			this.memoryLabel.TabIndex = 6;
			this.memoryLabel.Text = "[Memory]";
			// 
			// timeLabel
			// 
			this.timeLabel.AutoSize = true;
			this.timeLabel.Location = new System.Drawing.Point(82, 0);
			this.timeLabel.Margin = new System.Windows.Forms.Padding(0);
			this.timeLabel.Name = "timeLabel";
			this.timeLabel.Size = new System.Drawing.Size(36, 13);
			this.timeLabel.TabIndex = 6;
			this.timeLabel.Text = "[Time]";
			// 
			// resultLinkLabel
			// 
			this.resultLinkLabel.AutoSize = true;
			this.resultLinkLabel.Location = new System.Drawing.Point(0, 0);
			this.resultLinkLabel.Margin = new System.Windows.Forms.Padding(0);
			this.resultLinkLabel.Name = "resultLinkLabel";
			this.resultLinkLabel.Size = new System.Drawing.Size(62, 13);
			this.resultLinkLabel.TabIndex = 4;
			this.resultLinkLabel.TabStop = true;
			this.resultLinkLabel.Text = "Succeeded";
			this.resultLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.resultLinkLabel_LinkClicked);
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(4, 4);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(16, 16);
			this.pictureBox.TabIndex = 5;
			this.pictureBox.TabStop = false;
			// 
			// runningLabel
			// 
			this.runningLabel.AutoSize = true;
			this.runningLabel.ForeColor = System.Drawing.SystemColors.InfoText;
			this.runningLabel.Location = new System.Drawing.Point(22, 5);
			this.runningLabel.Name = "runningLabel";
			this.runningLabel.Size = new System.Drawing.Size(56, 13);
			this.runningLabel.TabIndex = 3;
			this.runningLabel.Text = "Running...";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Clone Detective Reports (*.xml)|*.xml|All Files (*.*)|*.*";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Clone Detective Reports (*.xml)|*.xml|All Files (*.*)|*.*";
			// 
			// CloneExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.borderPanel1);
			this.Controls.Add(this.borderPanel3);
			this.Controls.Add(this.borderPanel2);
			this.Name = "CloneExplorerControl";
			this.Size = new System.Drawing.Size(343, 215);
			this.borderPanel1.ResumeLayout(false);
			this.contextMenuStrip.ResumeLayout(false);
			this.borderPanel2.ResumeLayout(false);
			this.borderPanel2.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.borderPanel3.ResumeLayout(false);
			this.borderPanel3.PerformLayout();
			this.resultTableLayoutPanel.ResumeLayout(false);
			this.resultTableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.ImageList imageList;
		private BorderPanel borderPanel1;
		private System.Windows.Forms.TreeView treeView;
		private BorderPanel borderPanel2;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton runToolStripButton;
		private System.Windows.Forms.ToolStripButton stopToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox rollupTypeToolStripComboBox;
		private BorderPanel borderPanel3;
		private System.Windows.Forms.Label runningLabel;
		private System.Windows.Forms.LinkLabel resultLinkLabel;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Label timeLabel;
		private System.Windows.Forms.Label memoryLabel;
		private System.Windows.Forms.TableLayoutPanel resultTableLayoutPanel;
		private System.Windows.Forms.ToolStripButton importToolStripButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripButton settingsToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton exportToolStripButton;
		private System.Windows.Forms.ToolStripButton closeToolStripButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem showCloneIntersectionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;

	}
}
