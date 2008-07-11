namespace CloneDetective.Package
{
	partial class CloneIntersectionsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.listViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.borderPanel1 = new BorderPanel();
			this.fileNameLabel = new System.Windows.Forms.Label();
			this.borderPanel3 = new BorderPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.referenceContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.referencePictureBox = new System.Windows.Forms.PictureBox();
			this.borderPanel3.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) (this.referencePictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listView.ContextMenuStrip = this.listViewContextMenuStrip;
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.FullRowSelect = true;
			this.listView.Location = new System.Drawing.Point(1, 27);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.OwnerDraw = true;
			this.listView.Size = new System.Drawing.Size(790, 200);
			this.listView.TabIndex = 2;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_DrawColumnHeader);
			this.listView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_DrawItem);
			this.listView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listView_ColumnWidthChanged);
			this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
			this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
			this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
			this.listView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_DrawSubItem);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "File Name";
			this.columnHeader1.Width = 200;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Clone Intersections";
			this.columnHeader2.Width = 130;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Clone Visualization";
			this.columnHeader3.Width = 400;
			// 
			// listViewContextMenuStrip
			// 
			this.listViewContextMenuStrip.Name = "listViewContextMenuStrip";
			this.listViewContextMenuStrip.Size = new System.Drawing.Size(153, 26);
			this.listViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.listViewContextMenuStrip_Opening);
			// 
			// borderPanel1
			// 
			this.borderPanel1.BorderSides = ((System.Windows.Forms.Border3DSide) ((System.Windows.Forms.Border3DSide.Top | System.Windows.Forms.Border3DSide.Bottom)));
			this.borderPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.borderPanel1.Location = new System.Drawing.Point(1, 24);
			this.borderPanel1.Name = "borderPanel1";
			this.borderPanel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this.borderPanel1.Size = new System.Drawing.Size(790, 3);
			this.borderPanel1.TabIndex = 3;
			// 
			// fileNameLabel
			// 
			this.fileNameLabel.BackColor = System.Drawing.SystemColors.Info;
			this.fileNameLabel.ForeColor = System.Drawing.SystemColors.InfoText;
			this.fileNameLabel.Location = new System.Drawing.Point(2, 4);
			this.fileNameLabel.Name = "fileNameLabel";
			this.fileNameLabel.Size = new System.Drawing.Size(288, 13);
			this.fileNameLabel.TabIndex = 0;
			this.fileNameLabel.Text = "[File Name]";
			// 
			// borderPanel3
			// 
			this.borderPanel3.BorderSides = ((System.Windows.Forms.Border3DSide) (((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)
						| System.Windows.Forms.Border3DSide.Bottom)
						| System.Windows.Forms.Border3DSide.Middle)));
			this.borderPanel3.Controls.Add(this.listView);
			this.borderPanel3.Controls.Add(this.borderPanel1);
			this.borderPanel3.Controls.Add(this.panel1);
			this.borderPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.borderPanel3.Location = new System.Drawing.Point(0, 1);
			this.borderPanel3.Name = "borderPanel3";
			this.borderPanel3.Padding = new System.Windows.Forms.Padding(1);
			this.borderPanel3.Size = new System.Drawing.Size(792, 228);
			this.borderPanel3.TabIndex = 6;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Info;
			this.panel1.ContextMenuStrip = this.referenceContextMenuStrip;
			this.panel1.Controls.Add(this.referencePictureBox);
			this.panel1.Controls.Add(this.fileNameLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(1, 1);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(790, 23);
			this.panel1.TabIndex = 0;
			// 
			// referenceContextMenuStrip
			// 
			this.referenceContextMenuStrip.Name = "contextMenuStrip";
			this.referenceContextMenuStrip.Size = new System.Drawing.Size(61, 4);
			this.referenceContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
			// 
			// referencePictureBox
			// 
			this.referencePictureBox.InitialImage = null;
			this.referencePictureBox.Location = new System.Drawing.Point(66, 6);
			this.referencePictureBox.Name = "referencePictureBox";
			this.referencePictureBox.Size = new System.Drawing.Size(100, 10);
			this.referencePictureBox.TabIndex = 1;
			this.referencePictureBox.TabStop = false;
			this.referencePictureBox.Visible = false;
			this.referencePictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.referencePictureBox_MouseDoubleClick);
			// 
			// CloneIntersectionsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.borderPanel3);
			this.Name = "CloneIntersectionsControl";
			this.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.Size = new System.Drawing.Size(792, 229);
			this.borderPanel3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) (this.referencePictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private BorderPanel borderPanel1;
		private System.Windows.Forms.Label fileNameLabel;
		private BorderPanel borderPanel3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.PictureBox referencePictureBox;
		private System.Windows.Forms.ContextMenuStrip referenceContextMenuStrip;
		private System.Windows.Forms.ContextMenuStrip listViewContextMenuStrip;
	}
}
