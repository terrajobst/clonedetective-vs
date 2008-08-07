using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents the solution-specific settings of Clone Detective.
	/// For For global settings see <see cref="GlobalSettings"/>.
	/// </summary>
	public sealed class SolutionSettings
	{
		private string _solutionFileName;
		private string _settingsFileName;
		private bool _useCustomAnalysis;
		private string _analysisFileName;
		private string _cloneReportFileName;
		private Dictionary<string, string> _propertyOverrides = new Dictionary<string, string>();

		/// <summary>
		/// Loads the solution-specific settings of the given Visual Studio solution.
		/// </summary>
		/// <param name="solutionFileName">The fully qualified path of the Visual Studio for which the settings
		/// should be loaded.</param>
		/// <remarks>
		/// <para>The settings are loaded as entered by the user. In particular this means that all macros are
		/// still inlcuded in the file names. If you want to have all macros being replaced please use
		/// <see cref="LoadAndExpand"/> instead.</para>
		/// <para>For more information about macros please see 
		/// <a href="/UI/SolutionSettings.html">Solution Settings</a>.</para>
		/// </remarks>
		public void Load(string solutionFileName)
		{
			// Remember solution file name.
			_solutionFileName = solutionFileName;

			// Derive file name of the solution-specific settings file.
			_settingsFileName = PathHelper.GetSettingsPath(solutionFileName);

			if (!File.Exists(_settingsFileName))
			{
				// If the settings file does not yet exist we initialize
				// this settings object to default values.
				ApplyDefaults();
			}
			else
			{
				// The file settings file exists. Now we will load its settings.
				XmlDocument document = new XmlDocument();
				document.Load(_settingsFileName);

				_useCustomAnalysis = XmlConvert.ToBoolean(document.SelectSingleNode("/solutionSettings/useCustomAnalysis").InnerText);
				_analysisFileName = document.SelectSingleNode("/solutionSettings/analysisFileName").InnerText;
				_cloneReportFileName = document.SelectSingleNode("/solutionSettings/cloneReportFileName").InnerText;

				foreach (XmlNode propertyNode in document.SelectNodes("/solutionSettings/propertyOverrides/property"))
				{
					string key = propertyNode.Attributes["name"].Value;
					string value = propertyNode.Attributes["value"].Value;
					_propertyOverrides.Add(key, value);
				}
			}
		}

		/// <summary>
		/// Loads and expands the solution-specific settings of the given Visual Studio solution.
		/// </summary>
		/// <param name="solutionFileName">The fully qualified path of the Visual Studio for which the settings
		/// should be loaded and expanded.</param>
		/// <remarks>
		/// <para>The settings are loaded and all macros are replaced. In addition, if the setting <see cref="UseCustomAnalysis"/>
		/// is <see langword="false"/> all settings are initialized to the correct default values. This means
		/// the settings are not represented as entered by the user but instead represent the effective settings to be used
		/// by Clone Detective.</para>
		/// <para>If you want the settings to represent what the the user has actually entered please use <see cref="Load"/> instead.</para>
		/// <para>For more information about macros please see 
		/// <a href="/UI/SolutionSettings.html">Solution Settings</a>.</para>
		/// </remarks>
		public void LoadAndExpand(string solutionFileName)
		{
			Load(solutionFileName);

			if (!_useCustomAnalysis)
				ApplyDefaults();

			MacroExpander macroExpander = new MacroExpander(_solutionFileName);
			_analysisFileName = macroExpander.Expand(_analysisFileName);
			_cloneReportFileName = macroExpander.Expand(_cloneReportFileName);

			List<string> copiedKeys = new List<string>(_propertyOverrides.Keys);
			foreach (string property in copiedKeys)
				_propertyOverrides[property] = macroExpander.Expand(_propertyOverrides[property]);
		}

		/// <summary>
		/// Initializes all settings to default values.
		/// </summary>
		private void ApplyDefaults()
		{
			_useCustomAnalysis = false;
			_analysisFileName = "$(InstallDir)" + PathHelper.DefaultAnalysisFileName;
			_cloneReportFileName = "$(SolutionDir)$(SolutionName)" + PathHelper.CloneReportExtension;
			_propertyOverrides.Clear();
			_propertyOverrides.Add("solution.dir", "$(SolutionDir)");
			_propertyOverrides.Add("output.dir", "$(SolutionDir)");
			_propertyOverrides.Add("output.file", "$(SolutionName)" + PathHelper.CloneReportExtension);
			_propertyOverrides.Add("clone.minlength", "10");
		}

		/// <summary>
		/// Saves this settings object back to disk.
		/// </summary>
		public void Save()
		{
			XmlDocument document = new XmlDocument();

			XmlNode rootNode = document.CreateElement("solutionSettings");
			document.AppendChild(rootNode);

			XmlNode useCustomAnalysisNode = document.CreateElement("useCustomAnalysis");
			useCustomAnalysisNode.InnerText = XmlConvert.ToString(_useCustomAnalysis);
			rootNode.AppendChild(useCustomAnalysisNode);

			XmlNode analysisFileNameNode = document.CreateElement("analysisFileName");
			analysisFileNameNode.InnerText = _analysisFileName;
			rootNode.AppendChild(analysisFileNameNode);

			XmlNode cloneReportFileNameNode = document.CreateElement("cloneReportFileName");
			cloneReportFileNameNode.InnerText = _cloneReportFileName;
			rootNode.AppendChild(cloneReportFileNameNode);

			XmlNode propertyOverridesNode = document.CreateElement("propertyOverrides");
			rootNode.AppendChild(propertyOverridesNode);

			foreach (KeyValuePair<string, string> propertyOverride in _propertyOverrides)
			{
				XmlNode propertyNode = document.CreateElement("property");
				propertyOverridesNode.AppendChild(propertyNode);

				XmlAttribute nameAttribute = document.CreateAttribute("name");
				nameAttribute.Value = propertyOverride.Key;
				propertyNode.Attributes.Append(nameAttribute);

				XmlAttribute valueAttribute = document.CreateAttribute("value");
				valueAttribute.Value = propertyOverride.Value;
				propertyNode.Attributes.Append(valueAttribute);
			}

			document.Save(_settingsFileName);
		}

		/// <summary>
		/// Gets the fully qualified file name of the Visual Studio solution these settings
		/// apply to.
		/// </summary>
		public string SolutionFileName
		{
			get { return _solutionFileName; }
		}

		/// <summary>
		/// Gets the fully qualified path of the settings file.
		/// </summary>
		public string SettingsFileName
		{
			get { return _settingsFileName; }
		}

		/// <summary>
		/// Gets or sets a value that indicates whether Clone Detective should use the settings
		/// as specified or use the default settings instead.
		/// </summary>
		public bool UseCustomAnalysis
		{
			get { return _useCustomAnalysis; }
			set { _useCustomAnalysis = value; }
		}

		/// <summary>
		/// Gets or sets the fully qualified file name of the ConQAT analysis file to be used by
		/// Clone Detective.
		/// </summary>
		/// <remarks>
		/// The path returned may contain macros.
		/// </remarks>
		public string AnalysisFileName
		{
			get { return _analysisFileName; }
			set { _analysisFileName = value; }
		}

		/// <summary>
		/// Gets or sets the fully qualified file name of the clone report that is created
		/// by the ConQAT analysis that is specified by <see cref="AnalysisFileName"/>.
		/// </summary>
		public string CloneReportFileName
		{
			get { return _cloneReportFileName; }
			set { _cloneReportFileName = value; }
		}

		/// <summary>
		/// Gets a dictionary with all properties that should be passed to ConQAT when processing
		/// the analysis file that is specified by <see cref="AnalysisFileName"/>.
		/// </summary>
		/// <remarks>
		/// <para>For more information about macros please see 
		/// <a href="/UI/SolutionSettings.html">Solution Settings</a>.</para>
		/// </remarks>
		public Dictionary<string, string> PropertyOverrides
		{
			get { return _propertyOverrides; }
		}
	}
}