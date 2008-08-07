using System;
using System.Windows.Forms;

namespace CloneDetective.Package
{
	public partial class CloneDetectiveOptionPageControl : UserControl
	{
		private CloneDetectiveOptionPage _page;

		public CloneDetectiveOptionPageControl(CloneDetectiveOptionPage page)
		{
			_page = page;
			InitializeComponent();
		}

		private void browseConqatFileButton_Click(object sender, EventArgs e)
		{
			openFileDialog.FileName = conqatFileNameTextBox.Text;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
				conqatFileNameTextBox.Text = openFileDialog.FileName;
		}

		private void browseJavaHomeButton_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = javaHomeTextBox.Text;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				javaHomeTextBox.Text = folderBrowserDialog.SelectedPath;
		}

		public void LoadSettings()
		{
			conqatFileNameTextBox.Text = _page.ConqatFileName;
			javaHomeTextBox.Text = _page.JavaHome;
		}

		public void SaveSettings()
		{
			_page.ConqatFileName = conqatFileNameTextBox.Text;
			_page.JavaHome = javaHomeTextBox.Text;
		}
	}
}