namespace CloneDetective.Package
{
	partial class CloneDetectiveOptionPageControl
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
			this.browseConqatFileButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.conqatFileNameTextBox = new System.Windows.Forms.TextBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.browseJavaHomeButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.javaHomeTextBox = new System.Windows.Forms.TextBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// browseConqatFileButton
			// 
			this.browseConqatFileButton.Location = new System.Drawing.Point(317, 20);
			this.browseConqatFileButton.Name = "browseConqatFileButton";
			this.browseConqatFileButton.Size = new System.Drawing.Size(75, 23);
			this.browseConqatFileButton.TabIndex = 2;
			this.browseConqatFileButton.Text = "&Browse...";
			this.browseConqatFileButton.UseVisualStyleBackColor = true;
			this.browseConqatFileButton.Click += new System.EventHandler(this.browseConqatFileButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "ConQAT &File Name:";
			// 
			// conqatFileNameTextBox
			// 
			this.conqatFileNameTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.conqatFileNameTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.conqatFileNameTextBox.Location = new System.Drawing.Point(6, 22);
			this.conqatFileNameTextBox.Name = "conqatFileNameTextBox";
			this.conqatFileNameTextBox.Size = new System.Drawing.Size(305, 20);
			this.conqatFileNameTextBox.TabIndex = 1;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Batch Files (*.bat)|*.bat|All Files (*.*)|*.*";
			// 
			// browseJavaHomeButton
			// 
			this.browseJavaHomeButton.Location = new System.Drawing.Point(317, 59);
			this.browseJavaHomeButton.Name = "browseJavaHomeButton";
			this.browseJavaHomeButton.Size = new System.Drawing.Size(75, 23);
			this.browseJavaHomeButton.TabIndex = 5;
			this.browseJavaHomeButton.Text = "Bro&wse...";
			this.browseJavaHomeButton.UseVisualStyleBackColor = true;
			this.browseJavaHomeButton.Click += new System.EventHandler(this.browseJavaHomeButton_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 45);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "&Java Home:";
			// 
			// javaHomeTextBox
			// 
			this.javaHomeTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.javaHomeTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.javaHomeTextBox.Location = new System.Drawing.Point(6, 61);
			this.javaHomeTextBox.Name = "javaHomeTextBox";
			this.javaHomeTextBox.Size = new System.Drawing.Size(305, 20);
			this.javaHomeTextBox.TabIndex = 4;
			// 
			// CloneDetectiveOptionPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.javaHomeTextBox);
			this.Controls.Add(this.conqatFileNameTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.browseJavaHomeButton);
			this.Controls.Add(this.browseConqatFileButton);
			this.Name = "CloneDetectiveOptionPageControl";
			this.Size = new System.Drawing.Size(395, 289);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button browseConqatFileButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox conqatFileNameTextBox;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button browseJavaHomeButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox javaHomeTextBox;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}