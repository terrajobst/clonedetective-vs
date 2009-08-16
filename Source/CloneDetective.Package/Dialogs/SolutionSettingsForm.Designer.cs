namespace CloneDetective.Package
{
	partial class SolutionSettingsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.analysisFileNameLabel = new System.Windows.Forms.Label();
			this.cloneReportFileNameLabel = new System.Windows.Forms.Label();
			this.analysisFileNameTextBox = new System.Windows.Forms.TextBox();
			this.cloneReportFileNameTextBox = new System.Windows.Forms.TextBox();
			this.useCustomCloneDetectionCheckBox = new System.Windows.Forms.CheckBox();
			this.browseAnalysisFileNameButton = new System.Windows.Forms.Button();
			this.browseCloneReportFileNameButton = new System.Windows.Forms.Button();
			this.propertyOverridesDataGridView = new System.Windows.Forms.DataGridView();
			this.iconColumn = new System.Windows.Forms.DataGridViewImageColumn();
			this.propertyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.propertyOverridesToolStrip = new System.Windows.Forms.ToolStrip();
			this.addToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.editToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.deleteToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.upToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.downToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.propertiesFileNameTextBox = new System.Windows.Forms.TextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.browsePropertiesFileName = new System.Windows.Forms.Button();
			this.propertiesFileNameLabel = new System.Windows.Forms.Label();
			this.usePropertiesFileCheckBox = new System.Windows.Forms.CheckBox();
			this.propertyOverridesNoteLabel = new System.Windows.Forms.Label();
			this.pathsGroupLabel = new CloneDetective.Package.GroupLabel();
			this.propertiesFileGroupLabel = new CloneDetective.Package.GroupLabel();
			this.propertyOverridesGroupLabel = new CloneDetective.Package.GroupLabel();
			((System.ComponentModel.ISupportInitialize)(this.propertyOverridesDataGridView)).BeginInit();
			this.propertyOverridesToolStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(456, 409);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 17;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(537, 409);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 18;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// analysisFileNameLabel
			// 
			this.analysisFileNameLabel.AutoSize = true;
			this.analysisFileNameLabel.Location = new System.Drawing.Point(29, 71);
			this.analysisFileNameLabel.Name = "analysisFileNameLabel";
			this.analysisFileNameLabel.Size = new System.Drawing.Size(93, 13);
			this.analysisFileNameLabel.TabIndex = 2;
			this.analysisFileNameLabel.Text = "&Analysis file name:";
			// 
			// cloneReportFileNameLabel
			// 
			this.cloneReportFileNameLabel.AutoSize = true;
			this.cloneReportFileNameLabel.Location = new System.Drawing.Point(29, 97);
			this.cloneReportFileNameLabel.Name = "cloneReportFileNameLabel";
			this.cloneReportFileNameLabel.Size = new System.Drawing.Size(112, 13);
			this.cloneReportFileNameLabel.TabIndex = 5;
			this.cloneReportFileNameLabel.Text = "&Clone report file name:";
			// 
			// analysisFileNameTextBox
			// 
			this.analysisFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.errorProvider.SetIconAlignment(this.analysisFileNameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
			this.errorProvider.SetIconPadding(this.analysisFileNameTextBox, 3);
			this.analysisFileNameTextBox.Location = new System.Drawing.Point(164, 68);
			this.analysisFileNameTextBox.Name = "analysisFileNameTextBox";
			this.analysisFileNameTextBox.Size = new System.Drawing.Size(367, 20);
			this.analysisFileNameTextBox.TabIndex = 3;
			this.analysisFileNameTextBox.Validated += new System.EventHandler(this.analysisFileNameTextBox_Validated);
			this.analysisFileNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.analysisFileNameTextBox_Validating);
			// 
			// cloneReportFileNameTextBox
			// 
			this.cloneReportFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.errorProvider.SetIconAlignment(this.cloneReportFileNameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
			this.errorProvider.SetIconPadding(this.cloneReportFileNameTextBox, 3);
			this.cloneReportFileNameTextBox.Location = new System.Drawing.Point(164, 94);
			this.cloneReportFileNameTextBox.Name = "cloneReportFileNameTextBox";
			this.cloneReportFileNameTextBox.Size = new System.Drawing.Size(367, 20);
			this.cloneReportFileNameTextBox.TabIndex = 6;
			this.cloneReportFileNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.cloneReportFileNameTextBox_Validating);
			// 
			// useCustomCloneDetectionCheckBox
			// 
			this.useCustomCloneDetectionCheckBox.AutoSize = true;
			this.useCustomCloneDetectionCheckBox.Location = new System.Drawing.Point(12, 12);
			this.useCustomCloneDetectionCheckBox.Name = "useCustomCloneDetectionCheckBox";
			this.useCustomCloneDetectionCheckBox.Size = new System.Drawing.Size(158, 17);
			this.useCustomCloneDetectionCheckBox.TabIndex = 0;
			this.useCustomCloneDetectionCheckBox.Text = "&Use custom clone detection";
			this.useCustomCloneDetectionCheckBox.UseVisualStyleBackColor = true;
			this.useCustomCloneDetectionCheckBox.CheckedChanged += new System.EventHandler(this.useCustomCloneDetectionCheckBox_CheckedChanged);
			// 
			// browseAnalysisFileNameButton
			// 
			this.browseAnalysisFileNameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseAnalysisFileNameButton.Location = new System.Drawing.Point(537, 66);
			this.browseAnalysisFileNameButton.Name = "browseAnalysisFileNameButton";
			this.browseAnalysisFileNameButton.Size = new System.Drawing.Size(75, 23);
			this.browseAnalysisFileNameButton.TabIndex = 4;
			this.browseAnalysisFileNameButton.Text = "Browse...";
			this.browseAnalysisFileNameButton.UseVisualStyleBackColor = true;
			this.browseAnalysisFileNameButton.Click += new System.EventHandler(this.browseAnalysisFileNameButton_Click);
			// 
			// browseCloneReportFileNameButton
			// 
			this.browseCloneReportFileNameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseCloneReportFileNameButton.Location = new System.Drawing.Point(537, 92);
			this.browseCloneReportFileNameButton.Name = "browseCloneReportFileNameButton";
			this.browseCloneReportFileNameButton.Size = new System.Drawing.Size(75, 23);
			this.browseCloneReportFileNameButton.TabIndex = 7;
			this.browseCloneReportFileNameButton.Text = "Browse...";
			this.browseCloneReportFileNameButton.UseVisualStyleBackColor = true;
			this.browseCloneReportFileNameButton.Click += new System.EventHandler(this.browseCloneReportFileNameButton_Click);
			// 
			// propertyOverridesDataGridView
			// 
			this.propertyOverridesDataGridView.AllowUserToAddRows = false;
			this.propertyOverridesDataGridView.AllowUserToDeleteRows = false;
			this.propertyOverridesDataGridView.AllowUserToResizeRows = false;
			this.propertyOverridesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyOverridesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.propertyOverridesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.propertyOverridesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.propertyOverridesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.propertyOverridesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.propertyOverridesDataGridView.ColumnHeadersHeight = 21;
			this.propertyOverridesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.propertyOverridesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconColumn,
            this.propertyColumn,
            this.valueColumn});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.propertyOverridesDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			this.propertyOverridesDataGridView.Location = new System.Drawing.Point(32, 279);
			this.propertyOverridesDataGridView.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.propertyOverridesDataGridView.Name = "propertyOverridesDataGridView";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.propertyOverridesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.propertyOverridesDataGridView.RowHeadersVisible = false;
			this.propertyOverridesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.propertyOverridesDataGridView.Size = new System.Drawing.Size(580, 124);
			this.propertyOverridesDataGridView.TabIndex = 16;
			this.propertyOverridesDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.propertyOverridesDataGridView_CellDoubleClick);
			// 
			// iconColumn
			// 
			this.iconColumn.HeaderText = "";
			this.iconColumn.MinimumWidth = 20;
			this.iconColumn.Name = "iconColumn";
			this.iconColumn.ReadOnly = true;
			this.iconColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.iconColumn.Width = 20;
			// 
			// propertyColumn
			// 
			this.propertyColumn.HeaderText = "Property";
			this.propertyColumn.Name = "propertyColumn";
			this.propertyColumn.ReadOnly = true;
			this.propertyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.propertyColumn.Width = 120;
			// 
			// valueColumn
			// 
			this.valueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.valueColumn.HeaderText = "Value";
			this.valueColumn.Name = "valueColumn";
			this.valueColumn.ReadOnly = true;
			this.valueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// propertyOverridesToolStrip
			// 
			this.propertyOverridesToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyOverridesToolStrip.AutoSize = false;
			this.propertyOverridesToolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.propertyOverridesToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.propertyOverridesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripButton,
            this.editToolStripButton,
            this.deleteToolStripButton,
            this.toolStripSeparator1,
            this.upToolStripButton,
            this.downToolStripButton});
			this.propertyOverridesToolStrip.Location = new System.Drawing.Point(32, 254);
			this.propertyOverridesToolStrip.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.propertyOverridesToolStrip.Name = "propertyOverridesToolStrip";
			this.propertyOverridesToolStrip.Size = new System.Drawing.Size(580, 25);
			this.propertyOverridesToolStrip.TabIndex = 15;
			this.propertyOverridesToolStrip.Text = "toolStrip1";
			// 
			// addToolStripButton
			// 
			this.addToolStripButton.Image = global::CloneDetective.Package.Res.NewParameter;
			this.addToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.addToolStripButton.Name = "addToolStripButton";
			this.addToolStripButton.Size = new System.Drawing.Size(97, 22);
			this.addToolStripButton.Text = "Add property";
			this.addToolStripButton.Click += new System.EventHandler(this.addToolStripButton_Click);
			// 
			// editToolStripButton
			// 
			this.editToolStripButton.Image = global::CloneDetective.Package.Res.Edit;
			this.editToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.editToolStripButton.Name = "editToolStripButton";
			this.editToolStripButton.Size = new System.Drawing.Size(95, 22);
			this.editToolStripButton.Text = "Edit property";
			this.editToolStripButton.Click += new System.EventHandler(this.editToolStripButton_Click);
			// 
			// deleteToolStripButton
			// 
			this.deleteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.deleteToolStripButton.Image = global::CloneDetective.Package.Res.Delete;
			this.deleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.deleteToolStripButton.Name = "deleteToolStripButton";
			this.deleteToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.deleteToolStripButton.Text = "Delete property";
			this.deleteToolStripButton.Click += new System.EventHandler(this.deleteToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// upToolStripButton
			// 
			this.upToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.upToolStripButton.Image = global::CloneDetective.Package.Res.Up;
			this.upToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.upToolStripButton.Name = "upToolStripButton";
			this.upToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.upToolStripButton.Text = "Move up";
			this.upToolStripButton.Click += new System.EventHandler(this.upToolStripButton_Click);
			// 
			// downToolStripButton
			// 
			this.downToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.downToolStripButton.Image = global::CloneDetective.Package.Res.Down;
			this.downToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.downToolStripButton.Name = "downToolStripButton";
			this.downToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.downToolStripButton.Text = "Move down";
			this.downToolStripButton.Click += new System.EventHandler(this.downToolStripButton_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "ConQAT block files (*.cqb)|*.cqb|All files (*.*)|*.*";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
			// 
			// errorProvider
			// 
			this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider.ContainerControl = this;
			// 
			// propertiesFileNameTextBox
			// 
			this.propertiesFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.errorProvider.SetIconAlignment(this.propertiesFileNameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
			this.errorProvider.SetIconPadding(this.propertiesFileNameTextBox, 3);
			this.propertiesFileNameTextBox.Location = new System.Drawing.Point(164, 176);
			this.propertiesFileNameTextBox.Name = "propertiesFileNameTextBox";
			this.propertiesFileNameTextBox.Size = new System.Drawing.Size(367, 20);
			this.propertiesFileNameTextBox.TabIndex = 11;
			this.propertiesFileNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.propertiesFileNameTextBox_Validating);
			// 
			// toolTip
			// 
			this.toolTip.ToolTipTitle = "Expanded Path";
			// 
			// browsePropertiesFileName
			// 
			this.browsePropertiesFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browsePropertiesFileName.Location = new System.Drawing.Point(537, 174);
			this.browsePropertiesFileName.Name = "browsePropertiesFileName";
			this.browsePropertiesFileName.Size = new System.Drawing.Size(75, 23);
			this.browsePropertiesFileName.TabIndex = 12;
			this.browsePropertiesFileName.Text = "Browse...";
			this.browsePropertiesFileName.UseVisualStyleBackColor = true;
			// 
			// propertiesFileNameLabel
			// 
			this.propertiesFileNameLabel.AutoSize = true;
			this.propertiesFileNameLabel.Location = new System.Drawing.Point(29, 179);
			this.propertiesFileNameLabel.Name = "propertiesFileNameLabel";
			this.propertiesFileNameLabel.Size = new System.Drawing.Size(102, 13);
			this.propertiesFileNameLabel.TabIndex = 10;
			this.propertiesFileNameLabel.Text = "P&roperties file name:";
			// 
			// usePropertiesFileCheckBox
			// 
			this.usePropertiesFileCheckBox.AutoSize = true;
			this.usePropertiesFileCheckBox.Location = new System.Drawing.Point(32, 153);
			this.usePropertiesFileCheckBox.Name = "usePropertiesFileCheckBox";
			this.usePropertiesFileCheckBox.Size = new System.Drawing.Size(368, 17);
			this.usePropertiesFileCheckBox.TabIndex = 9;
			this.usePropertiesFileCheckBox.Text = "Use explicit .properties file to override values inside ConQAT analysis file.";
			this.usePropertiesFileCheckBox.UseVisualStyleBackColor = true;
			this.usePropertiesFileCheckBox.CheckedChanged += new System.EventHandler(this.usePropertiesFileCheckBox_CheckedChanged);
			// 
			// propertyOverridesNoteLabel
			// 
			this.propertyOverridesNoteLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyOverridesNoteLabel.Location = new System.Drawing.Point(29, 235);
			this.propertyOverridesNoteLabel.Margin = new System.Windows.Forms.Padding(3);
			this.propertyOverridesNoteLabel.Name = "propertyOverridesNoteLabel";
			this.propertyOverridesNoteLabel.Size = new System.Drawing.Size(583, 13);
			this.propertyOverridesNoteLabel.TabIndex = 14;
			this.propertyOverridesNoteLabel.Text = "Here you can define properties that override the values defined in the .propertie" +
				"s file and inside ConQAT analysis file.";
			// 
			// pathsGroupLabel
			// 
			this.pathsGroupLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pathsGroupLabel.Location = new System.Drawing.Point(9, 42);
			this.pathsGroupLabel.Name = "pathsGroupLabel";
			this.pathsGroupLabel.Size = new System.Drawing.Size(603, 13);
			this.pathsGroupLabel.TabIndex = 1;
			this.pathsGroupLabel.Text = "Analysis settings";
			// 
			// propertiesFileGroupLabel
			// 
			this.propertiesFileGroupLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertiesFileGroupLabel.Location = new System.Drawing.Point(9, 127);
			this.propertiesFileGroupLabel.Name = "propertiesFileGroupLabel";
			this.propertiesFileGroupLabel.Size = new System.Drawing.Size(603, 13);
			this.propertiesFileGroupLabel.TabIndex = 8;
			this.propertiesFileGroupLabel.Text = "Property file";
			// 
			// propertyOverridesGroupLabel
			// 
			this.propertyOverridesGroupLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyOverridesGroupLabel.Location = new System.Drawing.Point(9, 209);
			this.propertyOverridesGroupLabel.Name = "propertyOverridesGroupLabel";
			this.propertyOverridesGroupLabel.Size = new System.Drawing.Size(603, 13);
			this.propertyOverridesGroupLabel.TabIndex = 13;
			this.propertyOverridesGroupLabel.Text = "Property &overrides";
			// 
			// SolutionSettingsForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(624, 444);
			this.Controls.Add(this.propertyOverridesNoteLabel);
			this.Controls.Add(this.usePropertiesFileCheckBox);
			this.Controls.Add(this.browsePropertiesFileName);
			this.Controls.Add(this.propertiesFileNameTextBox);
			this.Controls.Add(this.propertiesFileNameLabel);
			this.Controls.Add(this.pathsGroupLabel);
			this.Controls.Add(this.propertiesFileGroupLabel);
			this.Controls.Add(this.propertyOverridesGroupLabel);
			this.Controls.Add(this.propertyOverridesToolStrip);
			this.Controls.Add(this.propertyOverridesDataGridView);
			this.Controls.Add(this.browseCloneReportFileNameButton);
			this.Controls.Add(this.browseAnalysisFileNameButton);
			this.Controls.Add(this.useCustomCloneDetectionCheckBox);
			this.Controls.Add(this.cloneReportFileNameTextBox);
			this.Controls.Add(this.analysisFileNameTextBox);
			this.Controls.Add(this.cloneReportFileNameLabel);
			this.Controls.Add(this.analysisFileNameLabel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "SolutionSettingsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Clone Detective Solution Settings";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SolutionSettingsForm_HelpButtonClicked);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SolutionSettingsForm_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SolutionSettingsForm_FormClosing);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SolutionSettingsForm_HelpRequested);
			((System.ComponentModel.ISupportInitialize)(this.propertyOverridesDataGridView)).EndInit();
			this.propertyOverridesToolStrip.ResumeLayout(false);
			this.propertyOverridesToolStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label analysisFileNameLabel;
		private System.Windows.Forms.Label cloneReportFileNameLabel;
		private System.Windows.Forms.TextBox analysisFileNameTextBox;
		private System.Windows.Forms.TextBox cloneReportFileNameTextBox;
		private System.Windows.Forms.CheckBox useCustomCloneDetectionCheckBox;
		private System.Windows.Forms.Button browseAnalysisFileNameButton;
		private System.Windows.Forms.Button browseCloneReportFileNameButton;
		private System.Windows.Forms.DataGridView propertyOverridesDataGridView;
		private System.Windows.Forms.ToolStrip propertyOverridesToolStrip;
		private System.Windows.Forms.ToolStripButton addToolStripButton;
		private System.Windows.Forms.ToolStripButton editToolStripButton;
		private System.Windows.Forms.ToolStripButton deleteToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton upToolStripButton;
		private System.Windows.Forms.ToolStripButton downToolStripButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private GroupLabel propertyOverridesGroupLabel;
		private GroupLabel pathsGroupLabel;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button browsePropertiesFileName;
		private System.Windows.Forms.TextBox propertiesFileNameTextBox;
		private System.Windows.Forms.Label propertiesFileNameLabel;
		private GroupLabel propertiesFileGroupLabel;
		private System.Windows.Forms.CheckBox usePropertiesFileCheckBox;
		private System.Windows.Forms.Label propertyOverridesNoteLabel;
		private System.Windows.Forms.DataGridViewImageColumn iconColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn propertyColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn valueColumn;
	}
}