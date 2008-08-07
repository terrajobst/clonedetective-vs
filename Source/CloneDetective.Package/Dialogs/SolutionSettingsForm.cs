using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
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
		private List<string> _declaredProperties;
		private HashSet<string> _usedProperies = new HashSet<string>();

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
				propertyOverridesDataGridView.Rows.Add(propertyOverride.Key, propertyOverride.Value);
				_usedProperies.Add(propertyOverride.Key);
			}

			propertyOverridesDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

			UpdateEnabledState();
		}

		private void UpdateProperties()
		{
			if (analysisFileNameTextBox.Text != _lastAnalysisFileName)
			{
				_declaredProperties = GetDeclaredProperties();
				_lastAnalysisFileName = analysisFileNameTextBox.Text;
			}
		}

		private IEnumerable<string> GetUnusedProperties()
		{
			foreach (string property in _declaredProperties)
			{
				if (!_usedProperies.Contains(property))
					yield return property;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private List<string> GetDeclaredProperties()
		{
			List<string> propertyNames = new List<string>();

			XmlDocument document = new XmlDocument();
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
			namespaceManager.AddNamespace("cqa", "http://conqat.cs.tum.edu/ns/config");
			try
			{
				string expandedAnalysisFileName = GetExpandedAnalysisFileName();
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
					string key = (string) row.Cells[0].Value;
					string value = (string) row.Cells[1].Value;
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

		private void analysisFileNameTextBox_Leave(object sender, EventArgs e)
		{
			UpdateProperties();
		}

		private void browseAnalysisFileNameButton_Click(object sender, EventArgs e)
		{
			openFileDialog.FileName = GetExpandedAnalysisFileName();
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				analysisFileNameTextBox.Text = TryToMakePathRelativeToSolution(openFileDialog.FileName);
		}

		private void browseCloneReportFileNameButton_Click(object sender, EventArgs e)
		{
			saveFileDialog.FileName = GetExpandedCloneReportFileName();
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				cloneReportFileNameTextBox.Text = TryToMakePathRelativeToSolution(saveFileDialog.FileName);
		}

		private void addToolStripButton_Click(object sender, EventArgs e)
		{
			if (_usedProperies.Count == _declaredProperties.Count)
			{
				string expandedAnalysisFileName = GetExpandedAnalysisFileName();
				string message = String.Format(CultureInfo.CurrentCulture, Res.AllPropertiesAlreadyOverridden, expandedAnalysisFileName);
				VSPackage.Instance.ShowError(message);
				return;
			}

			using (PropertyForm dlg = new PropertyForm(GetUnusedProperties()))
			{
				dlg.PropertyNameReadOnly = false;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					propertyOverridesDataGridView.Rows.Add(dlg.PropertyName, dlg.PropertyValue);
					_usedProperies.Add(dlg.PropertyName);
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
				dlg.PropertyNameReadOnly = true;
				dlg.PropertyName = (string)selectedRow.Cells[0].Value;
				dlg.PropertyValue = (string)selectedRow.Cells[1].Value;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					selectedRow.Cells[0].Value = dlg.PropertyName;
					selectedRow.Cells[1].Value = dlg.PropertyValue;
				}
			}
		}

		private void deleteToolStripButton_Click(object sender, EventArgs e)
		{
			DataGridViewRow[] selectedRows = new DataGridViewRow[propertyOverridesDataGridView.SelectedRows.Count];
			propertyOverridesDataGridView.SelectedRows.CopyTo(selectedRows, 0);

			foreach (DataGridViewRow selectedRow in selectedRows)
			{
				string property = (string)selectedRow.Cells[0].Value;
				_usedProperies.Remove(property);
				propertyOverridesDataGridView.Rows.Remove(selectedRow);
			}
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
			editToolStripButton.PerformClick();
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
	}
}