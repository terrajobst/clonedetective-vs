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
			this.listViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.borderPanel1 = new CloneDetective.Package.BorderPanel();
			this.fileNameLabel = new System.Windows.Forms.Label();
			this.borderPanel3 = new CloneDetective.Package.BorderPanel();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewImageColumn();
			this.panel1 = new System.Windows.Forms.Panel();
			this.referenceContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.referencePictureBox = new System.Windows.Forms.PictureBox();
			this.borderPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.referencePictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// listViewContextMenuStrip
			// 
			this.listViewContextMenuStrip.Name = "listViewContextMenuStrip";
			this.listViewContextMenuStrip.Size = new System.Drawing.Size(61, 4);
			this.listViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.listViewContextMenuStrip_Opening);
			// 
			// borderPanel1
			// 
			this.borderPanel1.BorderSides = ((System.Windows.Forms.Border3DSide)((System.Windows.Forms.Border3DSide.Top | System.Windows.Forms.Border3DSide.Bottom)));
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
			this.borderPanel3.BorderSides = ((System.Windows.Forms.Border3DSide)(((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)
						| System.Windows.Forms.Border3DSide.Bottom)
						| System.Windows.Forms.Border3DSide.Middle)));
			this.borderPanel3.Controls.Add(this.dataGridView);
			this.borderPanel3.Controls.Add(this.borderPanel1);
			this.borderPanel3.Controls.Add(this.panel1);
			this.borderPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.borderPanel3.Location = new System.Drawing.Point(0, 1);
			this.borderPanel3.Name = "borderPanel3";
			this.borderPanel3.Padding = new System.Windows.Forms.Padding(1);
			this.borderPanel3.Size = new System.Drawing.Size(792, 228);
			this.borderPanel3.TabIndex = 6;
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
			this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
			this.dataGridView.ContextMenuStrip = this.listViewContextMenuStrip;
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(1, 27);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.RowTemplate.Height = 19;
			this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.Size = new System.Drawing.Size(790, 200);
			this.dataGridView.TabIndex = 4;
			this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
			this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
			this.dataGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView_ColumnWidthChanged);
			this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
			// 
			// Column1
			// 
			this.Column1.HeaderText = "File Name";
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			this.Column1.Width = 200;
			// 
			// Column2
			// 
			this.Column2.HeaderText = "Clone Intersections";
			this.Column2.Name = "Column2";
			this.Column2.ReadOnly = true;
			this.Column2.Width = 150;
			// 
			// Column3
			// 
			this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Column3.HeaderText = "Clone Visualization";
			this.Column3.Name = "Column3";
			this.Column3.ReadOnly = true;
			this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
			this.referenceContextMenuStrip.Name = "referenceContextMenuStrip";
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
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.referencePictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BorderPanel borderPanel1;
		private System.Windows.Forms.Label fileNameLabel;
		private BorderPanel borderPanel3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox referencePictureBox;
		private System.Windows.Forms.ContextMenuStrip referenceContextMenuStrip;
		private System.Windows.Forms.ContextMenuStrip listViewContextMenuStrip;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewImageColumn Column3;
	}
}
