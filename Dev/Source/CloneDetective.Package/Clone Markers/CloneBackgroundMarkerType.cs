using System;
using System.Drawing;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CloneDetective.Package
{
	[Guid(Guids.GuidCloneBackgroundMarkerString)]
	internal sealed class CloneBackgroundMarkerType : IVsPackageDefinedTextMarkerType, IVsMergeableUIItem, IVsHiColorItem
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
			// This marker should colorize the text iff the indicator margin is invisible.
			// In that case we want the whole line to be colored. Otherwise it should be
			// visualized in the margin using a glyph that might span multiple lines.
			pdwVisualFlags = (uint)(
				  MARKERVISUAL.MV_COLOR_LINE_IF_NO_MARGIN
				| MARKERVISUAL.MV_GLYPH
				| MARKERVISUAL.MV_MULTILINE_GLYPH);

			return VSConstants.S_OK;
		}

		public int GetDefaultColors(COLORINDEX[] piForeground, COLORINDEX[] piBackground)
		{
			// This method won't be called as we implement the IVsHiColorItem interface.
			return VSConstants.E_NOTIMPL;
		}

		public int GetDefaultLineStyle(COLORINDEX[] piLineColor, LINESTYLE[] piLineIndex)
		{
			// These values are requested by the IDE although they are not used. They
			// would be used if we would want a border around the text.
			piLineColor[0] = COLORINDEX.CI_BLACK;
			piLineIndex[0] = LINESTYLE.LI_SOLID;

			return VSConstants.S_OK;
		}

		public int GetDefaultFontFlags(out uint pdwFontFlags)
		{
			// Don't modify the text at all.
			pdwFontFlags = (uint) FONTFLAGS.FF_DEFAULT;

			return VSConstants.S_OK;
		}

		public int GetBehaviorFlags(out uint pdwFlags)
		{
			// This marker might span multiple lines. It should be extended and shrinked
			// by Visual Studio on modification on the edges. And of course we want a
			// smooth feedback to the user (as we only want this marker to affect the
			// text appearance iff the indicator margin is invisible). So we inherit the
			// foreground coloring by the tokenizer and the other markers.
			pdwFlags = (uint)(
				  MARKERBEHAVIORFLAGS.MB_MULTILINESPAN
				| MARKERBEHAVIORFLAGS.MB_LEFTEDGE_LEFTTRACK
				| MARKERBEHAVIORFLAGS.MB_RIGHTEDGE_RIGHTTRACK) | (uint)
				  MARKERBEHAVIORFLAGS2.MB_INHERIT_FOREGROUND;

			return VSConstants.S_OK;
		}

		public int GetPriorityIndex(out int piPriorityIndex)
		{
			// This marker is for informational purposes only. So read-only should be
			// the best priority.
			piPriorityIndex = (int) MARKERTYPE.MARKER_READONLY;

			return VSConstants.S_OK;
		}

		public int DrawGlyphWithColors(IntPtr hdc, RECT[] pRect, int iMarkerType,
			IVsTextMarkerColorSet pMarkerColors, uint dwGlyphDrawFlags, int iLineHeight)
		{
			RECT rect = pRect[0];
			int rectWidth = rect.right - rect.left;
			int lineWidth = 2 + rectWidth / 20;

			uint clrFore;
			uint clrBack;
			ErrorHandler.ThrowOnFailure(pMarkerColors.GetMarkerColors(CloneMarginMarkerType.Id, out clrFore, out clrBack));

			int drawColorref = (int) clrFore;
			Color color = Color.FromArgb(
					drawColorref & 0x0000FF,
					(drawColorref & 0x00FF00) >> 8,
					(drawColorref & 0xFF0000) >> 16
				);

			using (Graphics graphics = Graphics.FromHdc(hdc))
			using (SolidBrush brush = new SolidBrush(color))
			{
				graphics.FillRectangle(brush, rect.left + (rectWidth - lineWidth) / 2, rect.top, lineWidth, rect.bottom - rect.top);
			}

			return VSConstants.S_OK;
		}

		#endregion

		#region IVsMergeableUIItem Members

		public int GetCanonicalName(out string pbstrNonLocalizeName)
		{
			pbstrNonLocalizeName = "Clone (Background)";

			return VSConstants.S_OK;
		}

		public int GetDisplayName(out string pbstrDisplayName)
		{
			// This string is displayed in the "Fonts and Colors" section
			// of the Visual Studio Options dialog.
			// TODO: Shouldn't we externalize this to a resource file?
			pbstrDisplayName = "Clone (Background)";

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
			pbstrDesc = "Clone (Background)";

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
					// Black.
					pcrColor = 0x00000000;
					break;

				case __tagVSCOLORDATA.CD_BACKGROUND:
					// Very light purple.
					pcrColor = 0x00F0E0F0;
					break;

				default:
					throw ExceptionBuilder.UnhandledCaseLabel(colorData);
			}

			return VSConstants.S_OK;
		}

		#endregion
	}
}
