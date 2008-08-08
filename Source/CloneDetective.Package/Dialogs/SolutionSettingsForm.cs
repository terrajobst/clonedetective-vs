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
		private bool _propertiesValid;

		public SolutionSettingsForm(string solutionPath)
		{
			InitializeComponent();

			_solutionSettings.Load(solutionPath);
			_macroExpander = new MacroExpander(solutionPath);

			useCustomCloneDetectionCheckBox.Checked = _solutionSettings.UseCustomAnalysis;
			analysisFileNameTextBox.Text = _solutionSettings.AnalysisFileName;
			cloneReportFileNameTextBox.Text = _solutionSettings.CloneReportFileName;

			UpdateProperties();
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

			UpdateEnabledState();
			ValidateAll();
		}

		private void ValidateAll()
		{
			ValidateFileNames();
			ValidateProperties();
		}

		private void ValidateFileNames()
		{
			_analysisFileValid = ValidateFileName(analysisFileNameTextBox, false);
			_cloneReportFileValid = ValidateFileName(cloneReportFileNameTextBox, true);
		}

		private bool ValidateFileName(TextBox textBox, bool suppressExistenceCheck)
		{
			string macroErrorMessage = GetMacroErrorMessage(_macroExpander, textBox.Text);
			if (macroErrorMessage != null)
			{
				errorProvider.SetError(textBox, macroErrorMessage);
				toolTip.SetToolTip(textBox, null);
				return false;
			}

			if (!suppressExistenceCheck)
			{
				string expandedFileName = _macroExpander.Expand(textBox.Text);
				if (!File.Exists(expandedFileName))
				{
					string errorMessage = String.Format(CultureInfo.CurrentCulture, Res.FileDoesNotExist, expandedFileName);
					errorProvider.SetError(textBox, errorMessage);
					toolTip.SetToolTip(textBox, null);
					return false;
				}
			}

			errorProvider.SetError(textBox, null);
			toolTip.SetToolTip(textBox, _macroExpander.Expand(textBox.Text));
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

				string propertyName = (string)row.Cells[1].Value;
				string propertyValue = (string)row.Cells[2].Value;

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

		private void UpdateProperties()
		{
			if (analysisFileNameTextBox.Text != _lastAnalysisFileName)
			{
				string expandedAnalysisFileName = GetExpandedAnalysisFileName();
				HashSet<string> declaredProperties = GetDeclaredProperties(expandedAnalysisFileName);
				if (declaredProperties.Count > 0)
				{
					_declaredProperties = declaredProperties;
					_lastAnalysisFileName = analysisFileNameTextBox.Text;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static HashSet<string> GetDeclaredProperties(string analysisFileName)
		{
			HashSet<string> propertyNames = new HashSet<string>();

			XmlDocument document = new XmlDocument();
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
			namespaceManager.AddNamespace("cqa", "http://conqat.cs.tum.edu/ns/config");
			try
			{
				string expandedAnalysisFileName = analysisFileName;
				document.Load(expandedAnalysisFileName);
			}
			catch (Exception)
			{
				// In case there is any error parsing the ConQAT analysis file we just
				// return an empty list.
				return propertyNames;
			}

			foreach (XmlNode propertyNode in document.SelectNodes("/cqa:conqat/cqa:property", namespaceManager))
			{
				string propertyName = propertyNode.Attributes["name"].Value;
				propertyNames.Add(propertyName);
			}

			return propertyNames;
		}

		private string GetExpandedAnalysisFileName()
		{
			return _macroExpander.Expand(analysisFileNameTextBox.Text);
		}

		private string GetExpandedCloneReportFileName()
		{
			return _macroExpander.Expand(cloneReportFileNameTextBox.Text);
		}

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

			propertyOverridesGroupLabel.Enabled =  useCustomCloneDetectionCheckBox.Checked;
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

		private void SolutionSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				_solutionSettings.UseCustomAnalysis = useCustomCloneDetectionCheckBox.Checked;
				_solutionSettings.AnalysisFileName = analysisFileNameTextBox.Text;
				_solutionSettings.CloneReportFileName = cloneReportFileNameTextBox.Text;

				_solutionSettings.PropertyOverrides.Clear();
				foreach (DataGridViewRow row in propertyOverridesDataGridView.Rows)
				{
					string key = (string) row.Cells[1].Value;
					string value = (string) row.Cells[2].Value;
					if (key != null)
						_solutionSettings.PropertyOverrides[key] = value;
				}

				_solutionSettings.Save();
			}
		}

		private void useCustomCloneDetectionCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void analysisFileNameTextBox_Validating(object sender, CancelEventArgs e)
		{
			_analysisFileValid = ValidateFileName(analysisFileNameTextBox, false);
		}

		private void analysisFileNameTextBox_Validated(object sender, EventArgs e)
		{
			UpdateProperties();
		}

		private void browseAnalysisFileNameButton_Click(object sender, EventArgs e)
		{
			openFileDialog.FileName = GetExpandedAnalysisFileName();
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				analysisFileNameTextBox.Text = TryToMakePathRelativeToSolution(openFileDialog.FileName);
		}

		private void cloneReportFileNameTextBox_Validating(object sender, CancelEventArgs e)
		{
			_cloneReportFileValid = ValidateFileName(cloneReportFileNameTextBox, true);
		}

		private void browseCloneReportFileNameButton_Click(object sender, EventArgs e)
		{
			saveFileDialog.FileName = GetExpandedCloneReportFileName();
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				cloneReportFileNameTextBox.Text = TryToMakePathRelativeToSolution(saveFileDialog.FileName);
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

		private void SolutionSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				e.Cancel = !_analysisFileValid || !_cloneReportFileValid || !_propertiesValid;
				if (e.Cancel)
					VSPackage.Instance.ShowError("Before you can save the changes you have to fix all errors first.");
			}
		}
	}
}