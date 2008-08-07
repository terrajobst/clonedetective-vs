using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CloneDetective.Package
{
	public partial class PropertyForm : Form
	{
		public PropertyForm(IEnumerable<string> properties)
		{
			InitializeComponent();

			foreach (string property in properties)
				propertyNameComboBox.Items.Add(property);
		}

		public bool PropertyNameReadOnly
		{
			get { return !propertyNameComboBox.Enabled; }
			set { propertyNameComboBox.Enabled = !value; }
		}

		public string PropertyName
		{
			get { return propertyNameComboBox.Text; }
			set { propertyNameComboBox.Text = value; }
		}

		public string PropertyValue
		{
			get { return propertyValueTextBox.Text; }
			set { propertyValueTextBox.Text = value; }
		}

		private void PropertyForm_Shown(object sender, EventArgs e)
		{
			if (propertyNameComboBox.Enabled)
				propertyNameComboBox.Focus();
			else
				propertyValueTextBox.Focus();
		}
	}
}