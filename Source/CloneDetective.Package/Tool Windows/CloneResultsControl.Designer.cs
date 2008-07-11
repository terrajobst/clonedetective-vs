using TD.SandDock;

namespace CloneDetective.Package
{
	partial class CloneResultsControl
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
			this.documentContainer = new TD.SandDock.DocumentContainer();
			this.noResultsBorderPanel = new BorderPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.noResultsBorderPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// documentContainer
			// 
			this.documentContainer.Guid = new System.Guid("db5d5a37-b18a-4f9a-83a8-27ec7773916e");
			this.documentContainer.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
			this.documentContainer.Location = new System.Drawing.Point(0, 0);
			this.documentContainer.Manager = null;
			this.documentContainer.Name = "documentContainer";
			this.documentContainer.Size = new System.Drawing.Size(503, 150);
			this.documentContainer.TabIndex = 1;
			this.documentContainer.Text = "button1";
			this.documentContainer.ActiveDocumentChanged += new TD.SandDock.ActiveDocumentEventHandler(this.documentContainer_ActiveDocumentChanged);
			this.documentContainer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.documentContainer_MouseClick);
			// 
			// noResultsBorderPanel
			// 
			this.noResultsBorderPanel.BorderSides = ((System.Windows.Forms.Border3DSide)(((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top)
						| System.Windows.Forms.Border3DSide.Right)
						| System.Windows.Forms.Border3DSide.Bottom)
						| System.Windows.Forms.Border3DSide.Middle)));
			this.noResultsBorderPanel.Controls.Add(this.label1);
			this.noResultsBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.noResultsBorderPanel.Location = new System.Drawing.Point(0, 0);
			this.noResultsBorderPanel.Name = "noResultsBorderPanel";
			this.noResultsBorderPanel.Padding = new System.Windows.Forms.Padding(1);
			this.noResultsBorderPanel.Size = new System.Drawing.Size(503, 150);
			this.noResultsBorderPanel.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(1, 1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(501, 148);
			this.label1.TabIndex = 0;
			this.label1.Text = "No clone results available.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CloneResultsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.noResultsBorderPanel);
			this.Controls.Add(this.documentContainer);
			this.Name = "CloneResultsControl";
			this.Size = new System.Drawing.Size(503, 150);
			this.noResultsBorderPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DocumentContainer documentContainer;
		private BorderPanel noResultsBorderPanel;
		private System.Windows.Forms.Label label1;
	}
}