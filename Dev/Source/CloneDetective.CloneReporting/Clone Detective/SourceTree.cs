using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class represents the source tree contained in a <see cref="CloneReport"/>. This allows
	/// tree based operations such as rolling up statistics such as lines of code or clone percentage.
	/// </summary>
	public sealed class SourceTree
	{
		private Dictionary<string, SourceNode> _pathDictionary = new Dictionary<string, SourceNode>(StringComparer.OrdinalIgnoreCase);
		private SourceNode _root;
		private string _solutionPath;

		/// <summary>
		/// Gets or sets the root of this tree. The root represents the solution itself.
		/// </summary>
		public SourceNode Root
		{
			get { return _root; }
			set { _root = value; }
		}

		/// <summary>
		/// Gets or sets the fully qualified path of the solution file.
		/// </summary>
		public string SolutionPath
		{
			get { return _solutionPath; }
			set { _solutionPath = value; }
		}

		/// <summary>
		/// Creates a <see cref="SourceTree"/> from the given <paramref name="cloneReport"/> and solution path.
		/// </summary>
		/// <param name="cloneReport">The clone report to create a source tree from.</param>
		/// <param name="solutionPath">The path to the solution of the clone report.</param>
		/// <returns>A hierarchical representation of the clone report.</returns>
		public static SourceTree FromCloneReport(CloneReport cloneReport, string solutionPath)
		{
			// Create the source and initialize path to solution.
			SourceTree sourceTree = new SourceTree();
			sourceTree.SolutionPath = solutionPath;

			// Create the tree structure.
			sourceTree.FillSourceElements(cloneReport);

			// Traverse tree and calculate statistics.
			PropagateDetails(sourceTree.Root);

			// Sort tree elements so that directories come before files and both
			// are sorted by name.
			SortSourceElements(sourceTree.Root);

			return sourceTree;
		}

		/// <summary>
		/// This method does post-order traversal calculating statistical information such
		/// as clone percentage and lines of code.
		/// </summary>
		/// <param name="node">The node to start traversal at.</param>
		private static void PropagateDetails(SourceNode node)
		{
			// Create stats for children first.
			foreach (SourceNode child in node.Children)
				PropagateDetails(child);

			// Now recalculate the stats for this node.
			RecalculateNode(node);
		}

		/// <summary>
		/// This method updates the statistical information for a single node
		/// all the way up to the root.
		/// </summary>
		/// <param name="node">The node to update to the root.</param>
		public static void RecalculateRollups(SourceNode node)
		{
			// Recalculate all the way up.
			while (node != null)
			{
				RecalculateNode(node);

				// Next parent please.
				node = node.Parent;
			}
		}

		/// <summary>
		/// This method calculates the statistical information for the given node assuming
		/// the children have already been processed.
		/// </summary>
		/// <param name="node">The node to recalculate statistics for.</param>
		private static void RecalculateNode(SourceNode node)
		{
			// Here we will calculate the following pieces of information:
			HashSet<CloneClass> cloneClasses;
			int linesOfCode;
			int numberOfClones;
			int numberOfClonedLines;

			if (node.SourceFile != null)
			{
				// For source files this is easy. Just get the values.
				cloneClasses = GetCloneClasses(node.SourceFile);
				linesOfCode = GetLinesOfCode(node.SourceFile);
				numberOfClones = GetNumberOfClones(node.SourceFile);
				numberOfClonedLines = GetNumberOfClonedLines(node.SourceFile);
			}
			else
			{
				// For directories things are more interesting.
				cloneClasses = new HashSet<CloneClass>();
				linesOfCode = 0;
				numberOfClones = 0;
				numberOfClonedLines = 0;
				foreach (SourceNode child in node.Children)
				{
					// Clone classes will be calculated by constructing
					// the union of all its children.
					cloneClasses.UnionWith(child.CloneClasses);

					// All other metrics are just the sum of the diretory's children.
					linesOfCode += child.LinesOfCode;
					numberOfClones += child.NumberOfClones;
					numberOfClonedLines += child.NumberOfClonedLines;
				}
			}

			// Just assign the results to the node.
			node.CloneClasses = cloneClasses;
			node.LinesOfCode = linesOfCode;
			node.NumberOfClones = numberOfClones;
			node.NumberOfClonedLines = numberOfClonedLines;
		}

		/// <summary>
		/// Returns the lines of code contained in the given source file.
		/// </summary>
		/// <param name="sourceFile">The source file to get the lines of code for.</param>
		private static int GetLinesOfCode(SourceFile sourceFile)
		{
			return sourceFile.Length;
		}

		/// <summary>
		/// Returns the number of clones contained in the given source file.
		/// </summary>
		/// <param name="sourceFile">The source file to get the number of clones for.</param>
		private static int GetNumberOfClones(SourceFile sourceFile)
		{
			return sourceFile.Clones.Count;
		}

		/// <summary>
		/// Returns the set of clone classes contained in the given source file.
		/// </summary>
		/// <param name="sourceFile">The source file to get the set of clones classes for.</param>
		private static HashSet<CloneClass> GetCloneClasses(SourceFile sourceFile)
		{
			HashSet<CloneClass> cloneClasses = new HashSet<CloneClass>();
			foreach (Clone clone in sourceFile.Clones)
				cloneClasses.Add(clone.CloneClass);

			return cloneClasses;
		}

		/// <summary>
		/// Returns the number of cloned lines contained in the given source file.
		/// </summary>
		/// <param name="sourceFile">The source file to get the number of cloned lines for.</param>
		private static int GetNumberOfClonedLines(SourceFile sourceFile)
		{
			// Conceptually, we will create an array of booleans for every line in the file.
			// Then we will iterate over all clones and set all the lines contained by the clone
			// to true.
			//
			// After all we will count the elements that are true.
			//
			// To optimize things a bit we first seek the first and last lines that are contained
			// in a clone. This ensures that the array we will allocate is as small as possible.
			// In addition we will use a BitArray instead of an array of Booleans.
			//
			// Memory is O((lastClonedLine - firstClonedLine + 1) / 8)
			// Time is O(3 * sourceFile.Clones)

			// Find first and last line that is contained in a clone.
			int firstClonedLine = Int32.MaxValue;
			int lastClonedLine = Int32.MinValue;
			foreach (Clone clone in sourceFile.Clones)
			{
				firstClonedLine = Math.Min(firstClonedLine, clone.StartLine);
				lastClonedLine = Math.Max(lastClonedLine, clone.StartLine + clone.LineCount);
			}

			// Now we can calculate the maximum length we need.
			int arraySize = lastClonedLine - firstClonedLine + 1;
			BitArray isClonedLine = new BitArray(arraySize);

			// Mark all cloned lines.
			foreach (Clone clone in sourceFile.Clones)
			{
				for (int i = clone.StartLine; i < clone.StartLine + clone.LineCount; i++)
					isClonedLine[i - firstClonedLine] = true;
			}

			// Count all marked lines.
			int numberOfClonedLines = 0;
			foreach (bool b in isClonedLine)
			{
				if (b)
					numberOfClonedLines++;
			}

			return numberOfClonedLines;
		}

		/// <summary>
		/// Creates the tree structure for the given clone report.
		/// </summary>
		/// <param name="report">The clone report to create the tree structure for.</param>
		private void FillSourceElements(CloneReport report)
		{
			// First we construct the root.
			_root = new SourceNode();
			_root.Name = Path.GetFileNameWithoutExtension(_solutionPath);
			_root.FullPath = Path.GetDirectoryName(_solutionPath);

			// Enter the root. This is important in order to make sure that our
			// tree will only contain elements relative to the directory the solution
			// is contained in.
			_pathDictionary.Add(_root.FullPath, _root);

			// Add all files to the tree.
			foreach (SourceFile sourceFile in report.SourceFiles)
			{
				// Create the file node.
				SourceNode fileNode = new SourceNode();
				fileNode.Name = Path.GetFileName(sourceFile.Path);
				fileNode.FullPath = sourceFile.Path;
				fileNode.SourceFile = sourceFile;

				// Enter the file node to our dictionary. This is not required by
				// GetSourceElementParent() but for FindNode().
				_pathDictionary.Add(fileNode.FullPath, fileNode);

				// Get the node for the directory the file is contained in.
				string directory = Path.GetDirectoryName(sourceFile.Path);
				fileNode.Parent = GetSourceElementParent(_pathDictionary, directory);

				// Enter the file node to the children of the directory.
				fileNode.Parent.Children.Add(fileNode);
			}
		}

		/// <summary>
		/// Returns the <see cref="SourceNode"/> for the fully qualified directory path.
		/// </summary>
		/// <param name="elements">A dictionary containing the mapping from fully qualified directory names to source nodes.</param>
		/// <param name="directoryPath">The fully qualified directory path to get the source node for.</param>
		/// <remarks>
		/// This method recursively calls itself to make sure that for all intermediate paths a source node exists as well.
		/// </remarks>
		private static SourceNode GetSourceElementParent(Dictionary<string, SourceNode> elements, string directoryPath)
		{
			SourceNode directoryNode;

			// First we check if we already have a mapping for the directory.
			if (elements.TryGetValue(directoryPath, out directoryNode))
			{
				// Wow, this was easy. We already know this directory so we
				// can return it directly.
				return directoryNode;
			}

			// No, we have no source node for this directory yet. Create one.
			directoryNode = new SourceNode();
			directoryNode.Name = Path.GetFileName(directoryPath);
			directoryNode.FullPath = directoryPath;

			// Enter the directory to the mapping.
			elements.Add(directoryNode.FullPath, directoryNode);

			// Now we need the parent for this directory.
			//
			// NOTE: No termination check here. The directory of the solution has
			//       already been added by our caller. So the termination check
			//       is implicitly performed by the check above.
			string parentDirectory = Path.GetDirectoryName(directoryPath);
			directoryNode.Parent = GetSourceElementParent(elements, parentDirectory);

			// Add our directory to our parent's list of children.
			directoryNode.Parent.Children.Add(directoryNode);

			// Return newly created directory.
			return directoryNode;
		}

		/// <summary>
		/// Sorts all nodes in the tree. The comparator function is <see cref="CompareSourceElements"/>.
		/// </summary>
		/// <param name="root">The root node to start sorting.</param>
		private static void SortSourceElements(SourceNode root)
		{
			// It is not important whether we traverse the tree in post-order
			// or pre-order but we will sort our children first (pre-order).
			root.Children.Sort(CompareSourceElements);

			foreach (SourceNode child in root.Children)
				SortSourceElements(child);
		}

		/// <summary>
		/// Compares two source nodes.
		/// </summary>
		/// <param name="x">The left hand side argument.</param>
		/// <param name="y">The right hand side argument.</param>
		/// <returns>
		/// This method compares two source nodes in such a way that directories
		/// come first and files later but both groups are sorted by name.
		/// </returns>
		private static int CompareSourceElements(SourceNode x, SourceNode y)
		{
			bool xHasNoChildren = x.Children.Count == 0;
			bool yHasNoChildren = y.Children.Count == 0;
			int result = xHasNoChildren.CompareTo(yHasNoChildren);
			if (result != 0)
				return result;

			// The sorting is used to for display purposes. That is why we use
			// CurrentCulture instead of Ordinal or OrdinalIgnoreCase.
			return String.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Searches a <see cref="SourceNode"/> by the given fully qualified path.
		/// </summary>
		/// <param name="path">The fully qualified path to get a <see cref="SourceNode"/> for.</param>
		/// <returns>
		/// If no <see cref="SourceNode"/> for the given path exists the return value is <see langword="null"/>.
		/// </returns>
		public SourceNode FindNode(string path)
		{
			SourceNode result;
			_pathDictionary.TryGetValue(path, out result);
			return result;
		}

		/// <summary>
		/// Calculates the clone intersections for the given <see cref="SourceNode"/>.
		/// </summary>
		/// <param name="sourceNode">The source node to calculate the clone intersections for.</param>
		/// <returns>
		/// If <paramref name="sourceNode"/> does not refer to a file the return value
		/// is <see langword="null"/>.
		/// </returns>
		public CloneIntersectionResult GetCloneIntersections(SourceNode sourceNode)
		{
			// Check if the source node represents a file.
			if (sourceNode.SourceFile == null)
				return null;

			// Create the result object.
			CloneIntersectionResult result = new CloneIntersectionResult();

			// Initialize the target we are performing the intersection. The target is
			// our source node.
			result.Target = new CloneIntersectedItem();
			result.Target.SourceNode = sourceNode;

			// First we will traverse over all clones contained in our source node.
			// While going, we create a list of all clone classes and setup a mapping
			// from a simple ascending ID to a clone class. This ID is used later on
			// to derive the color from.
			//
			// In addition we create a list of all clones contained in our source node
			// and add them to the target's list of clones.
			//
			// Actually that means that the CloneIntersectedItem representing the target
			// does not contain more information than the raw source node object itself.
			// However, this way the representation of the target is identical to the
			// representation of its references (i.e. both are CloneIntersectedItems).
			Dictionary<CloneClass, int> cloneClassIdDictionary = new Dictionary<CloneClass, int>();
			List<CloneClass> cloneClasses = new List<CloneClass>();
			int lastCloneClassId = 0;
			foreach (Clone clone in sourceNode.SourceFile.Clones)
			{
				// Check whether we have seen this clone class before (i.e. we have a 
				// clone class ID for it).
				int cloneClassId;
				if (!cloneClassIdDictionary.TryGetValue(clone.CloneClass, out cloneClassId))
				{
					// No, we have not. Generate new ID and enter clone class to both 
					// the list and the mapping.
					cloneClassId = lastCloneClassId++;
					cloneClassIdDictionary.Add(clone.CloneClass, cloneClassId);
					cloneClasses.Add(clone.CloneClass);
				}

				// Create clone intersection for the clone and add it to the
				// target's list of clones.
				CloneIntersection cloneIntersection = new CloneIntersection();
				cloneIntersection.Clone = clone;
				cloneIntersection.CloneClassId = cloneClassId;
				result.Target.Clones.Add(cloneIntersection);
			}

			// OK, now we are doing the real work by searching all other clones
			// that share a clone class with our source node (= target).

			// Since we don't use the SourceFiles object but CloneIntersectedItem
			// to represent files we need a mapping from a source file to
			// the CloneIntersectedItem.
			Dictionary<SourceFile, CloneIntersectedItem> referencesDictionary = new Dictionary<SourceFile, CloneIntersectedItem>();

			foreach (CloneClass cloneClass in cloneClasses)
			{
				foreach (Clone clone in cloneClass.Clones)
				{
					// If the clone's source file is identical to our source node
					// we will just ignore it.
					//
					// This ensures that a clone intersection result will never
					// have the target in the list of references.
					if (clone.SourceFile == sourceNode.SourceFile)
						continue;

					// Try to get a reference to the CloneIntersectedItem represented by the
					// source file.
					CloneIntersectedItem reference;
					if (!referencesDictionary.TryGetValue(clone.SourceFile, out reference))
					{
						// We have no CloneIntersectedItem for the source file. Create one.
						reference = new CloneIntersectedItem();
						reference.SourceNode = FindNode(clone.SourceFile.Path);
						result.References.Add(reference);
						referencesDictionary.Add(clone.SourceFile, reference);
					}

					// Since we know this clone shares a clone class with our source node
					// we add a clone intersection to the CloneIntersectedItem.
					CloneIntersection cloneIntersection = new CloneIntersection();
					cloneIntersection.CloneClassId = cloneClassIdDictionary[clone.CloneClass];
					cloneIntersection.Clone = clone;
					reference.Clones.Add(cloneIntersection);
				}
			}

			return result;
		}
	}
}
