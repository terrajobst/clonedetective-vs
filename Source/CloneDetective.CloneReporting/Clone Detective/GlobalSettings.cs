using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

using Microsoft.Win32;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class provides read-only access to system-wide, i.e non solution-specific settings
	/// of Clone Detective. For solution-specific settings see <see cref="SolutionSettings"/>.
	/// </summary>
	public static class GlobalSettings
	{
		/// <summary>
		/// Reads a system-wide, user-specific setting from the registry.
		/// </summary>
		/// <param name="keyName">The name of the setting to be read.</param>
		private static string GetUserSetting(string keyName)
		{
			const string rootKey = @"Software\Microsoft\VisualStudio\9.0\DialogPage\CloneDetective.Package.CloneDetectiveOptionPage";
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(rootKey))
			{
				if (registryKey == null)
					return null;

				return Convert.ToString(registryKey.GetValue(keyName), CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Reads a system-wide, non user-specific setting from the registry.
		/// </summary>
		/// <param name="keyName">The name of the setting to be read.</param>
		private static string GetMachineSetting(string keyName)
		{
			const string rootKey = @"SOFTWARE\Microsoft\VisualStudio\9.0\Clone Detective for Visual Studio";
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(rootKey))
			{
				if (registryKey == null)
					return null;

				return Convert.ToString(registryKey.GetValue(keyName), CultureInfo.InvariantCulture);
			}
		}

		/// <summary> 
		/// Returns the setting for fully qualified path of conqat.bat.
		/// </summary>
		/// <returns>
		/// If the user has not yet configured this setting this method will
		/// try to derive this setting by using the installation path of ConQAT.
		/// This only works if ConQAT has been installed together with Clone Detective.
		/// If this is not the case the method will return <see langword="null"/>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetConqatBatFileName()
		{
			// First we try to read the user-specific setting.
			string conqatBatFileName = GetUserSetting("ConqatFileName");

			if (String.IsNullOrEmpty(conqatBatFileName))
			{
				// The user has not yet configured it. Try the machine
				// setting.
				conqatBatFileName = GetMachineSetting("ConqatFileName");
			}

			return conqatBatFileName;
		}

		/// <summary>
		/// Returns the setting for the Java home that should be used by Clone Detective
		/// when running ConQAT.
		/// </summary>
		/// <returns>
		/// If the user has not yet configured this setting this method will try to derive this setting
		/// by using the Java home of the JVM that is marked as the current one in the Windows registry.
		/// This setting is stored under <c>SOFTWARE\JavaSoft\Java Runtime Environment</c>. If no JVM
		/// is marked as the current one this method returns <see langword="null"/>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetJavaHome()
		{
			// First we try to read the user-specific setting.
			string javaHome = GetUserSetting("JavaHome");

			if (String.IsNullOrEmpty(javaHome))
			{
				// The user has not yet configured it. Look in the registry to see if a JVM
				// is marked as the current one.
				const string rootKey = @"SOFTWARE\JavaSoft\Java Runtime Environment";
				using (RegistryKey javaRuntimeRoot = Registry.LocalMachine.OpenSubKey(rootKey))
				{
					if (javaRuntimeRoot != null)
					{
						string currentVersion = Convert.ToString(javaRuntimeRoot.GetValue("CurrentVersion"), CultureInfo.InvariantCulture);
						if (currentVersion != null)
						{
							using (RegistryKey currentVersionRoot = Registry.LocalMachine.OpenSubKey(rootKey + "\\" + currentVersion))
							{
								if (currentVersionRoot != null)
									javaHome = Convert.ToString(currentVersionRoot.GetValue("JavaHome"), CultureInfo.InvariantCulture);
							}
						}
					}
				}
			}

			return javaHome;
		}

		/// <summary>
		/// Returns the installation directory of Clone Detective.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetInstallDir()
		{
			return PathHelper.EnsureTrailingBackslash(GetMachineSetting("InstallDir"));
		}

		/// <summary>
		/// Returns the directory in which the Visual Studio 2008 IDE is installed, e.g.
		/// <c>C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE</c>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Env")]
		public static string GetDevEnvDir()
		{
			const string rootKey = @"SOFTWARE\Microsoft\VisualStudio\9.0";
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(rootKey))
			{
				if (registryKey == null)
					return null;

				string installDirSetting = Convert.ToString(registryKey.GetValue("InstallDir"), CultureInfo.InvariantCulture);
				return PathHelper.EnsureTrailingBackslash(installDirSetting);
			}
		}

		/// <summary>
		/// Returns the installation directory of ConQAT.
		/// </summary>
		/// <returns>
		/// If ConQAT has not been installed together with Clone Detective the return
		/// value is <see langword="null"/>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetConqatDir()
		{
			string conqatBatFile = GetMachineSetting("ConqatFileName");
			if (String.IsNullOrEmpty(conqatBatFile))
				return null;

			string conqatBinDir = Path.GetDirectoryName(conqatBatFile);
			string conqatDir = Path.GetDirectoryName(conqatBinDir);
			return PathHelper.EnsureTrailingBackslash(conqatDir);
		}
	}
}