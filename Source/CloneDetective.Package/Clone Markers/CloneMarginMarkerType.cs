using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CloneDetective.Package
{
	// This marker type is not used throughout the whole system. However we
	// need it to have a separate entry in the Options Fonts and Colors
	// configuration dialog.
	[Guid(Guids.GuidCloneMarginMarkerString)]
	internal sealed class CloneMarginMarkerType : IVsPackageDefinedTextMarkerType, IVsMergeableUIItem, IVsHiColorItem
	{
		private static int _id;

		public static int Id
		{
			get { return _id; }
			internal set { _id = value; }
		}

		#region IVsPackageDefinedTextMarkerType Members

		public int GetVisualStyle(out uint pdwVisualFlags)
		{
			pdwVisualFlags = (uint) MARKERVISUAL.MV_COLOR_ALWAYS;

			return VSConstants.S_OK;
		}

		public int GetDefaultColors(COLORINDEX[] piForeground, COLORINDEX[] piBackground)
		{
			// This method won't be called as we implement the IVsHiColorItem interfacce.
			return VSConstants.E_NOTIMPL;
		}

		public int GetDefaultLineStyle(COLORINDEX[] piLineColor, LINESTYLE[] piLineIndex)
		{
			piLineColor[0] = COLORINDEX.CI_PURPLE;
			piLineIndex[0] = LINESTYLE.LI_SOLID;

			return VSConstants.S_OK;
		}

		public int GetDefaultFontFlags(out uint pdwFontFlags)
		{
			pdwFontFlags = (uint) FONTFLAGS.FF_DEFAULT;

			return VSConstants.S_OK;
		}

		public int GetBehaviorFlags(out uint pdwFlags)
		{
			pdwFlags = (uint) MARKERBEHAVIORFLAGS2.MB_INHERIT_BACKGROUND;

			return VSConstants.S_OK;
		}

		public int GetPriorityIndex(out int piPriorityIndex)
		{
			piPriorityIndex = (int) MARKERTYPE.MARKER_READONLY;

			return VSConstants.S_OK;
		}

		public int DrawGlyphWithColors(IntPtr hdc, RECT[] pRect, int iMarkerType,
			IVsTextMarkerColorSet pMarkerColors, uint dwGlyphDrawFlags, int iLineHeight)
		{
			return VSConstants.S_OK;
		}

		#endregion

		#region IVsMergeableUIItem Members

		public int GetCanonicalName(out string pbstrNonLocalizeName)
		{
			pbstrNonLocalizeName = "Clone (Margin)";

			return VSConstants.S_OK;
		}

		public int GetDisplayName(out string pbstrDisplayName)
		{
			// This string is displayed in the "Fonts and Colors" section
			// of the Visual Studio Options dialog.
			// TODO: Shouldn't we externalize this to a resource file?
			pbstrDisplayName = "Clone (Margin)";

			return VSConstants.S_OK;
		}

		public int GetMergingPriority(out int piMergingPriority)
		{
			piMergingPriority = 0;

			return VSConstants.S_OK;
		}

		public int GetDescription(out string pbstrDesc)
		{
			// TODO: Shouldn't we externalize this to a resource file?
			pbstrDesc = "Clone (Margin)";

			return VSConstants.S_OK;
		}

		#endregion

		#region IVsHiColorItem Members

		public int GetColorData(int cdElement, out uint pcrColor)
		{
			__tagVSCOLORDATA colorData = (__tagVSCOLORDATA) cdElement;

			switch (colorData)
			{
				case __tagVSCOLORDATA.CD_FOREGROUND:
				case __tagVSCOLORDATA.CD_LINECOLOR:
					// Purple.
					pcrColor = 0x00800080;
					break;

				case __tagVSCOLORDATA.CD_BACKGROUND:
					// White.
					pcrColor = 0x00FFFFFF;
					break;

				default:
					throw ExceptionBuilder.UnhandledCaseLabel(colorData);
			}

			return VSConstants.S_OK;
		}

		#endregion
	}
}