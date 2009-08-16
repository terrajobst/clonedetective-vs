using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using CloneDetective.CloneReporting;

namespace CloneDetective.Package
{
	public partial class SolutionSettingsForm : Form
	{
		private SolutionSettings _solutionSettings = new SolutionSettings();
		private MacroExpander _macroExpander;
		private string _lastAnalysisFileName;
		private HashSet<string> _declaredProperties;

		private bool _analysisFileValid;
		private bool _cloneReportFileValid;
		private bool _propertiesFileValid;
		private bool _propertiesValid;

		public SolutionSettingsForm(string solutionPath)
		{
			InitializeComponent();

			_macroExpander = new MacroExpander(solutionPath);

			LoadSettings(solutionPath);
			UpdateEnabledState();
			UpdateDeclaredProperties();
			ValidateAll();
		}

		#region Load/save settings

		private void LoadSettings(string solutionPath)
		{
			_solutionSettings.Load(solutionPath);

			useCustomCloneDetectionCheckBox.Checked = _solutionSettings.UseCustomAnalysis;
			analysisFileNameTextBox.Text = _solutionSettings.AnalysisFileName;
			cloneReportFileNameTextBox.Text = _solutionSettings.CloneReportFileName;

			usePropertiesFileCheckBox.Checked = _solutionSettings.UsePropertiesFile;
			propertiesFileNameTextBox.Text = _solutionSettings.PropertiesFileName;

			foreach (KeyValuePair<string, string> propertyOverride in _solutionSettings.PropertyOverrides)
			{
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(propertyOverridesDataGridView);
				row.Cells[0].ToolTipText = propertyOverride.Key;
				row.Cells[0].Value = Res.Parameter;
				row.Cells[1].Value = propertyOverride.Key;
				row.Cells[2].Value = propertyOverride.Value;

				propertyOverridesDataGridView.Rows.Add(row);
			}

			propertyOverridesDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
		}

		private void SaveSettings()
		{
			_solutionSettings.UseCustomAnalysis = useCustomCloneDetectionCheckBox.Checked;
			_solutionSettings.AnalysisFileName = analysisFileNameTextBox.Text;
			_solutionSettings.CloneReportFileName = cloneReportFileNameTextBox.Text;

			_solutionSettings.UsePropertiesFile = usePropertiesFileCheckBox.Checked;
			_solutionSettings.PropertiesFileName = propertiesFileNameTextBox.Text;

			_solutionSettings.PropertyOverrides.Clear();
			foreach (DataGridViewRow row in propertyOverridesDataGridView.Rows)
			{
				string key = (string)row.Cells[1].Value;
				string value = (string)row.Cells[2].Value;
				if (key != null)
					_solutionSettings.PropertyOverrides[key] = value;
			}

			_solutionSettings.Save();
		}

		#endregion

		#region Validation Helpers

		private void ValidateAll()
		{
			ValidateAnalysisFile();
			ValidateCloneReportFile();
			ValidatePropertiesFile();
			ValidateProperties();
		}

		private void ValidateAnalysisFile()
		{
			if (useCustomCloneDetectionCheckBox.Checked)
				_analysisFileValid = ValidateTextBox(analysisFileNameTextBox, ValidateFileNameNotEmpty, ValidateMacros, ValidateFileExists);
			else
			{
				_analysisFileValid = true;
				ClearError(analysisFileNameTextBox);
			}
		}

		private void ValidateCloneReportFile()
		{
			if (useCustomCloneDetectionCheckBox.Checked)
				_cloneReportFileValid = ValidateTextBox(cloneReportFileNameTextBox, ValidateFileNameNotEmpty, ValidateMacros);
			else
			{
				_cloneReportFileValid = true;
				ClearError(cloneReportFileNameTextBox);
			}
		}

		private void ValidatePropertiesFile()
		{
			if (useCustomCloneDetectionCheckBox.Checked && usePropertiesFileCheckBox.Checked)
				_propertiesFileValid = ValidateTextBox(propertiesFileNameTextBox, ValidateFileNameNotEmpty, ValidateMacros, ValidateFileExists);
			else
			{
				_propertiesFileValid = true;
				ClearError(propertiesFileNameTextBox);
			}
		}

		private void ClearError(TextBox textBox)
		{
			errorProvider.SetError(textBox, null);
			toolTip.SetToolTip(textBox, _macroExpander.Expand(textBox.Text));
		}

		private bool ValidateTextBox(TextBox textBox, params Func<TextBox, bool>[] rules)
		{
			foreach (Func<TextBox, bool> rule in rules)
			{
				if (!rule(textBox))
					return false;
			}

			ClearError(textBox);
			return true;
		}

		private bool ValidateFileNameNotEmpty(TextBox textBox)
		{
			if (textBox.Text.Trim().Length == 0)
			{
				errorProvider.SetError(textBox, Res.FileNameCannotBeBlank);
				toolTip.SetToolTip(textBox, null);
				return false;
			}
			return true;
		}

		private bool ValidateMacros(TextBox textBox)
		{
			string macroErrorMessage = GetMacroErrorMessage(_macroExpander, textBox.Text);
			if (macroErrorMessage != null)
			{
				errorProvider.SetError(textBox, macroErrorMessage);
				toolTip.SetToolTip(textBox, null);
				return false;
			}
			return true;
		}

		private bool ValidateFileExists(TextBox textBox)
		{
			if (textBox.Text.Length == 0)
				return true;

			string expandedFileName = _macroExpander.Expand(textBox.Text);
			if (!File.Exists(expandedFileName))
			{
				string errorMessage = String.Format(CultureInfo.CurrentCulture, Res.FileDoesNotExist, expandedFileName);
				errorProvider.SetError(textBox, errorMessage);
				toolTip.SetToolTip(textBox, null);
				return false;
			}
			return true;
		}

		private void ValidateProperties()
		{
			bool atLeastOnePropertyInvalid = false;

			HashSet<string> overriddenProperties = new HashSet<string>();
			StringBuilder sb = new StringBuilder();

			foreach (DataGridViewRow row in propertyOverridesDataGridView.Rows)
			{
				sb.Length = 0;

				if (useCustomCloneDetectionCheckBox.Checked)
				{
					string propertyName = (string) row.Cells[1].Value;
					string propertyValue = (string) row.Cells[2].Value;

					if (!_declaredProperties.Contains(propertyName))
					{
						sb.AppendFormat(CultureInfo.CurrentCulture, Res.PropertyCannotBeOverridden, propertyName);
						sb.AppendLine();
					}
					else if (!overriddenProperties.Add(propertyName))
					{
						sb.AppendFormat(CultureInfo.CurrentCulture, Res.PropertyAlreadyOverridden, propertyName);
						sb.AppendLine();
					}

					string macroErrorMessage = GetMacroErrorMessage(_macroExpander, propertyValue);
					if (macroErrorMessage != null)
						sb.AppendLine(macroErrorMessage);
				}

				if (sb.Length == 0)
				{
					row.Cells[0].ToolTipText = String.Empty;
					row.Cells[0].Value = Res.Parameter;
				}
				else
				{
					row.Cells[0].ToolTipText = sb.ToString();
					row.Cells[0].Value = errorProvider.Icon;
					atLeastOnePropertyInvalid = true;
				}
			}

			_propertiesValid = !atLeastOnePropertyInvalid;
		}
		
		#endregion

		#region Helpers for extracting properties of .cqb files

		private void UpdateDeclaredProperties()
		{
			if (analysisFileNameTextBox.Text != _lastAnalysisFileName)
			{
				string expandedAnalysisFileName = _macroExpander.Expand(analysisFileNameTextBox.Text);
				_declaredProperties = GetDeclaredProperties(expandedAnalysisFileName);
				_lastAnalysisFileName = analysisFileNameTextBox.Text;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static HashSet<string> GetDeclaredProperties(string analysisFileName)
		{
			HashSet<string> propertyNames = new HashSet<string>();

			XmlDocument document = new XmlDocument();
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
			namespaceManager.AddNamespace("cqb", "http://conqat.cs.tum.edu/ns/config");
			try
			{
				document.Load(analysisFileName);
			}
			catch (Exception)
			{
				// In case there is any error parsing the ConQAT analysis file we just
				// return an empty list.
				return propertyNames;
			}

			foreach (XmlNode propertyNodeOuter in document.SelectNodes("/cqb:conqat/cqb:block-spec/cqb:param", namespaceManager))
			{
				string parameterName = propertyNodeOuter.Attributes["name"].Value;
				foreach (XmlNode propertyNodeInner in propertyNodeOuter.SelectNodes("cqb:attr", namespaceManager))
				{
					string attributeName = propertyNodeInner.Attributes["name"].Value;
					propertyNames.Add(parameterName + "." + attributeName);
				}
			}

			return propertyNames;
		}

		#endregion

		private static string GetMacroErrorMessage(MacroExpander macroExpander, string text)
		{
			Regex regex = new Regex(@"\$\([^)]*\)?");

			StringBuilder sb = new StringBuilder();

			foreach (Match match in regex.Matches(text))
			{
				string macro = match.Value;
				if (!macroExpander.Macros.ContainsKey(macro))
				{
					sb.AppendFormat(CultureInfo.CurrentCulture, Res.InvalidMacro, macro);
					sb.AppendLine();
				}
			}

			string errorMessage;
			if (sb.Length == 0)
				errorMessage = null;
			else
				errorMessage = sb.ToString();
			return errorMessage;
		}

		private void UpdateEnabledState()
		{
			pathsGroupLabel.Enabled = useCustomCloneDetectionCheckBox.Checked;

			analysisFileNameLabel.Enabled = useCustomCloneDetectionCheckBox.Checked;
			analysisFileNameTextBox.Enabled = useCustomCloneDetectionCheckBox.Checked;
			browseAnalysisFileNameButton.Enabled = useCustomCloneDetectionCheckBox.Checked;

			cloneReportFileNameLabel.Enabled = useCustomCloneDetectionCheckBox.Checked;
			cloneReportFileNameTextBox.Enabled = useCustomCloneDetectionCheckBox.Checked;
			browseCloneReportFileNameButton.Enabled = useCustomCloneDetectionCheckBox.Checked;

			propertiesFileGroupLabel.Enabled = useCustomCloneDetectionCheckBox.Checked;
			usePropertiesFileCheckBox.Enabled = useCustomCloneDetectionCheckBox.Checked;
			propertiesFileNameLabel.Enabled = useCustomCloneDetectionCheckBox.Checked && usePropertiesFileCheckBox.Checked;
			propertiesFileNameTextBox.Enabled = useCustomCloneDetectionCheckBox.Checked && usePropertiesFileCheckBox.Checked;
			browsePropertiesFileName.Enabled = useCustomCloneDetectionCheckBox.Checked && usePropertiesFileCheckBox.Checked;

			propertyOverridesGroupLabel.Enabled =  useCustomCloneDetectionCheckBox.Checked;
			propertyOverridesNoteLabel.Enabled = useCustomCloneDetectionCheckBox.Checked;
			propertyOverridesToolStrip.Enabled = useCustomCloneDetectionCheckBox.Checked;
			propertyOverridesDataGridView.Enabled = useCustomCloneDetectionCheckBox.Checked;

			if (useCustomCloneDetectionCheckBox.Checked)
			{
				propertyOverridesDataGridView.BackgroundColor = SystemColors.Window;
				propertyOverridesDataGridView.ForeColor = SystemColors.WindowText;
				propertyOverridesDataGridView.DefaultCellStyle.BackColor = propertyOverridesDataGridView.BackgroundColor;
			}
			else
			{
				propertyOverridesDataGridView.BackgroundColor = SystemColors.Control;
				propertyOverridesDataGridView.ForeColor = SystemColors.GrayText;
				propertyOverridesDataGridView.DefaultCellStyle.BackColor = propertyOverridesDataGridView.BackgroundColor;
			}
		}

		private void MoveRows(int offset)
		{
			DataGridViewRow[] selectedRows = new DataGridViewRow[propertyOverridesDataGridView.SelectedRows.Count];
			propertyOverridesDataGridView.SelectedRows.CopyTo(selectedRows, 0);
			Array.Sort(selectedRows, (x, y) => y.Index.CompareTo(x.Index) * offset);

			foreach (DataGridViewRow row in selectedRows)
			{
				int index = row.Index;
				int newIndex = index + offset;
				if (newIndex < 0 || newIndex >= propertyOverridesDataGridView.Rows.Count)
					break;
				
				propertyOverridesDataGridView.Rows.RemoveAt(index);
				propertyOverridesDataGridView.Rows.Insert(newIndex, row);
			}

			foreach (DataGridViewRow row in propertyOverridesDataGridView.Rows)
				row.Selected = false;
			
			foreach (DataGridViewRow row in selectedRows)
				row.Selected = true;

			ValidateProperties();
		}

		private string TryToMakePathRelativeToSolution(string path)
		{
			string solutionDir = Path.GetDirectoryName(_solutionSettings.SolutionFileName);
			string pathRelativeToSolution = PathHelper.GetRelativePath(solutionDir, path);
			return pathRelativeToSolution.Replace(PathHelper.EnsureTrailingBackslash(solutionDir), "$(SolutionDir)");
		}

		private static void ShowHelp()
		{
			VSPackage.Instance.ShowHelp("CloneDetective.UI.SolutionSettings");
		}

		private void SolutionSettingsForm_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			ShowHelp();
			e.Cancel = true;
		}

		private void SolutionSettingsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			ShowHelp();
		}

		private void SolutionSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				e.Cancel = !_analysisFileValid ||
				           !_cloneReportFileValid ||
				           !_propertiesFileValid ||
				           !_propertiesValid;
				if (e.Cancel)
					VSPackage.Instance.ShowError(Res.YouMustFixAllErrors);
			}
		}

		private void SolutionSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
				SaveSettings();
		}

		private void useCustomCloneDetectionCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			ValidateAll();
		}

		private void analysisFileNameTextBox_Validating(object sender, CancelEventArgs e)
		{
			ValidateAnalysisFile();
		}

		private void analysisFileNameTextBox_Validated(object sender, EventArgs e)
		{
			UpdateDeclaredProperties();
		}

		private void browseAnalysisFileNameButton_Click(object sender, EventArgs e)
		{
			openFileDialog.FileName = _macroExpander.Expand(analysisFileNameTextBox.Text);
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				analysisFileNameTextBox.Text = TryToMakePathRelativeToSolution(openFileDialog.FileName);
		}

		private void cloneReportFileNameTextBox_Validating(object sender, CancelEventArgs e)
		{
			ValidateCloneReportFile();
		}

		private void browseCloneReportFileNameButton_Click(object sender, EventArgs e)
		{
			saveFileDialog.FileName = _macroExpander.Expand(cloneReportFileNameTextBox.Text);
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				cloneReportFileNameTextBox.Text = TryToMakePathRelativeToSolution(saveFileDialog.FileName);
		}

		private void usePropertiesFileCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (usePropertiesFileCheckBox.Checked && propertiesFileNameTextBox.Text.Length == 0)
				propertiesFileNameTextBox.Text = PathHelper.GetPropertiesFilePath(analysisFileNameTextBox.Text);

			UpdateEnabledState();
			ValidateAll();
		}

		private void propertiesFileNameTextBox_Validating(object sender, CancelEventArgs e)
		{
			ValidatePropertiesFile();
		}

		private void addToolStripButton_Click(object sender, EventArgs e)
		{
			using (PropertyForm dlg = new PropertyForm(_declaredProperties))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					propertyOverridesDataGridView.Rows.Add(Res.Parameter, dlg.PropertyName, dlg.PropertyValue);
					ValidateProperties();
				}
			}
		}

		private void editToolStripButton_Click(object sender, EventArgs e)
		{
			if (propertyOverridesDataGridView.SelectedRows.Count == 0)
				return;

			DataGridViewRow selectedRow = propertyOverridesDataGridView.SelectedRows[0];

			using (PropertyForm dlg = new PropertyForm(_declaredProperties))
			{
				dlg.PropertyName = (string)selectedRow.Cells[1].Value;
				dlg.PropertyValue = (string)selectedRow.Cells[2].Value;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					selectedRow.Cells[1].Value = dlg.PropertyName;
					selectedRow.Cells[2].Value = dlg.PropertyValue;
					ValidateProperties();
				}
			}
		}

		private void deleteToolStripButton_Click(object sender, EventArgs e)
		{
			DataGridViewRow[] selectedRows = new DataGridViewRow[propertyOverridesDataGridView.SelectedRows.Count];
			propertyOverridesDataGridView.SelectedRows.CopyTo(selectedRows, 0);

			foreach (DataGridViewRow selectedRow in selectedRows)
				propertyOverridesDataGridView.Rows.Remove(selectedRow);

			ValidateProperties();
		}

		private void upToolStripButton_Click(object sender, EventArgs e)
		{
			MoveRows(-1);
		}

		private void downToolStripButton_Click(object sender, EventArgs e)
		{
			MoveRows(1);
		}

		private void propertyOverridesDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
				editToolStripButton.PerformClick();
		}
	}
}