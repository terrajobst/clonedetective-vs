using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class encapsulates the logic of reading a ConQAT clone report from XML.
	/// </summary>
	internal static class CloneReportReader
	{
		/// <summary>
		/// Reads a clone report from the given XML file.
		/// </summary>
		/// <param name="fileName">The fully qualified path to the ConQAT clone report file.</param>
		public static CloneReport Read(string fileName)
		{
			XmlDocument doc = new XmlDocument();

			// First we try to load the document.
			try
			{
				doc.Load(fileName);
			}
			catch (Exception ex)
			{
				// NOTE: Here we intentionally catch all exceptions. XmlDocment.Load() lists ten possible exceptions.
				throw ExceptionBuilder.CannotLoadCloneReport(fileName, ex);
			}

			// Add schema so that we can validate the document is validated.
			using (StringReader stringReader = new StringReader(Resources.CloneReportSchema))
			using (XmlTextReader xmlTextReader = new XmlTextReader(stringReader))
				doc.Schemas.Add(null, xmlTextReader);

			// Now we can validate document against the loaded schema.
			List<XmlSchemaException> validationErrors = new List<XmlSchemaException>();
			doc.Validate(delegate(object sender, ValidationEventArgs e)
			{
				validationErrors.Add(e.Exception);
			});

			if (validationErrors.Count > 0)
				throw ExceptionBuilder.InvalidCloneReport(fileName, validationErrors);

			CloneReport cloneReport = Read(doc);

			// Make sure the source files are sorted by their path.
			// This property is required by some algorithms later on.
			cloneReport.SourceFiles.Sort(delegate(SourceFile x, SourceFile y)
			{
				return String.Compare(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
			});

			return cloneReport;
		}

		/// <summary>
		/// Reads a clone report from the given XML document.
		/// </summary>
		/// <param name="doc">The XML document to read the clone report from.</param>
		private static CloneReport Read(XmlDocument doc)
		{
			XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
			manager.AddNamespace("cr", Resources.CloneReportSchemaNamespace);

			CloneReport cloneReport = new CloneReport();
			cloneReport.SystemDate = GetOptionalAttributeString(doc, "/cr:cloneReport/@systemdate", manager);

			// In order to associate source files and clones we need to map
			// source file ids to source file objects.
			Dictionary<int, SourceFile> sourceFileDictionary = new Dictionary<int, SourceFile>();

			// Read all source files.
			XmlNodeList sourceFiles = doc.SelectNodes("/cr:cloneReport/cr:sourceFile", manager);
			foreach (XmlNode sourceFileNode in sourceFiles)
			{
				// Create source file object and parse attributes.
				SourceFile sourceFile = new SourceFile();
				sourceFile.Id = XmlConvert.ToInt32(sourceFileNode.Attributes["id"].Value);
				sourceFile.Path = sourceFileNode.Attributes["path"].Value;
				sourceFile.Length = XmlConvert.ToInt32(sourceFileNode.Attributes["length"].Value);
				sourceFile.Fingerprint = sourceFileNode.Attributes["fingerprint"].Value;

				// Add source file object to list of all source files.
				cloneReport.SourceFiles.Add(sourceFile);

				// Add mapping sourceFile.Id -> sourceFile for later use.
				sourceFileDictionary.Add(sourceFile.Id, sourceFile);
			}

			// Read all clone classes.
			XmlNodeList cloneClasses = doc.SelectNodes("/cr:cloneReport/cr:cloneClass", manager);
			foreach (XmlNode cloneClassNode in cloneClasses)
			{
				// Create clone class and parse attributes.
				CloneClass cloneClass = new CloneClass();
				cloneClass.Id = XmlConvert.ToInt32(cloneClassNode.Attributes["id"].Value);
				cloneClass.UniqueId = GetOptionalAttributeString(cloneClassNode, "@uniqueId", manager);
				cloneClass.NormalizedLength = XmlConvert.ToInt32(cloneClassNode.Attributes["normalizedLength"].Value);
				cloneClass.Fingerprint = cloneClassNode.Attributes["fingerprint"].Value;

				// Add clone class to list of all clone classes.
				cloneReport.CloneClasses.Add(cloneClass);

				// Read all key-value pairs associated with this clone class.
				XmlNodeList cloneClassValues = cloneClassNode.SelectNodes("cr:values/cr:value", manager);
				ReadValues(cloneClassValues, cloneClass.Values);

				// Read all clones.
				XmlNodeList clones = cloneClassNode.SelectNodes("cr:clone", manager);
				foreach (XmlNode cloneNode in clones)
				{
					// Parse source file id and get associated source file object.
					int sourceFileId = XmlConvert.ToInt32(cloneNode.Attributes["sourceFileId"].Value);
					SourceFile sourceFile = sourceFileDictionary[sourceFileId];

					// Create clone and parse attributes.
					Clone clone = new Clone();
					clone.Id = GetOptionalAttributeInt32(cloneNode, "@id", manager);
					clone.UniqueId = GetOptionalAttributeString(cloneNode, "@uniqueId", manager);
					clone.SourceFile = sourceFile;
					clone.StartLine = XmlConvert.ToInt32(cloneNode.Attributes["startLine"].Value);
					clone.LineCount = XmlConvert.ToInt32(cloneNode.Attributes["lineCount"].Value);
					clone.StartUnitIndexInFile = XmlConvert.ToInt32(cloneNode.Attributes["startUnitIndexInFile"].Value);
					clone.LengthInUnits = XmlConvert.ToInt32(cloneNode.Attributes["lengthInUnits"].Value);
					clone.DeltaInUnits = XmlConvert.ToInt32(cloneNode.Attributes["deltaInUnits"].Value);
					clone.Gaps = cloneNode.Attributes["gaps"].Value;
					clone.Fingerprint = cloneNode.Attributes["fingerprint"].Value;

					// Read all key-value pairs associated with this clone.
					XmlNodeList cloneValues = cloneNode.SelectNodes("cr:values/cr:value", manager);
					ReadValues(cloneValues, clone.Values);

					clone.CloneClass = cloneClass;

					// Add clone to the clone class' list of clones.
					cloneClass.Clones.Add(clone);

					// Add clone to the source file's list of clones.
					clone.SourceFile.Clones.Add(clone);
				}
			}

			return cloneReport;
		}

		private static void ReadValues(XmlNodeList valueNodes, List<CustomValue> target)
		{
			foreach (XmlNode valueNode in valueNodes)
			{
				// Create key-value pair and parse attributes.
				CustomValue value = new CustomValue();
				value.Key = valueNode.Attributes["key"].Value;
				value.Value = valueNode.Attributes["value"].Value;
				value.Type = valueNode.Attributes["type"].Value;

				// Add value to the clone class' list of key-value pairs.
				target.Add(value);
			}
		}

		private static string GetOptionalAttributeString(XmlNode doc, string xpath, XmlNamespaceManager manager)
		{
			XmlAttribute systemdateAttribute = (XmlAttribute)doc.SelectSingleNode(xpath, manager);
			return systemdateAttribute == null
			       	? null
			       	: systemdateAttribute.Value;
		}

		private static int? GetOptionalAttributeInt32(XmlNode doc, string xpath, XmlNamespaceManager manager)
		{
			string stringValue = GetOptionalAttributeString(doc, xpath, manager);
			return stringValue == null
					? (int?) null
					: XmlConvert.ToInt32(stringValue);
		}
	}
}
