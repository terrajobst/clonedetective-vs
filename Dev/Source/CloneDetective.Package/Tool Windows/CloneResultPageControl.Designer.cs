namespace CloneDetective.Package
{
	partial class CloneResultPageControl
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
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.borderPanel1 = new CloneDetective.Package.BorderPanel();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewImageColumn();
			this.borderPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
			this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
			// 
			// borderPanel1
			// 
			this.borderPanel1.BorderSides = ((System.Windows.Forms.Border3DSide)(((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)
						| System.Windows.Forms.Border3DSide.Bottom)
						| System.Windows.Forms.Border3DSide.Middle)));
			this.borderPanel1.Controls.Add(this.dataGridView);
			this.borderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.borderPanel1.Location = new System.Drawing.Point(0, 0);
			this.borderPanel1.Name = "borderPanel1";
			this.borderPanel1.Padding = new System.Windows.Forms.Padding(1);
			this.borderPanel1.Size = new System.Drawing.Size(768, 153);
			this.borderPanel1.TabIndex = 2;
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
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(1, 1);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.RowTemplate.Height = 19;
			this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.Size = new System.Drawing.Size(766, 151);
			this.dataGridView.TabIndex = 2;
			this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
			this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
			this.dataGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView_ColumnWidthChanged);
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
			this.Column2.HeaderText = "Clone Count";
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
			// CloneResultPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.borderPanel1);
			this.Name = "CloneResultPageControl";
			this.Size = new System.Drawing.Size(768, 153);
			this.borderPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BorderPanel borderPanel1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewImageColumn Column3;
	}
}