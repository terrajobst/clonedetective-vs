using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CloneDetective.Package
{
	internal sealed class TextMarkerClientEventSink : IVsTextMarkerClient
	{
		private IVsTextLineMarker _marker;

		public IVsTextLineMarker Marker
		{
			set { _marker = value; }
		}

		#region IVsTextMarkerClient Members

		public void MarkerInvalidated()
		{
			CloneDetectiveManager.OnMarkerInvalidated(_marker);
		}

		public int GetTipText(IVsTextMarker pMarker, string[] pbstrText)
		{
			return VSConstants.S_OK;
		}

		public void OnBufferSave(string pszFileName)
		{
		}

		public void OnBeforeBufferClose()
		{
		}

		public int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf)
		{
			// For each command we add we have to specify that we support it.
			// Furthermore it should always be enabled.
			const uint menuItemFlags = (uint) (
				  OLECMDF.OLECMDF_SUPPORTED
				| OLECMDF.OLECMDF_ENABLED);

			if (pbstrText == null || pcmdf == null)
				return VSConstants.S_OK;

			switch (iItem)
			{
				case 0:
					pbstrText[0] = Res.CommandFindClones;
					pcmdf[0] = menuItemFlags;
					return VSConstants.S_OK;

				case 1:
					pbstrText[0] = Res.CommandShowCloneIntersections;
					pcmdf[0] = menuItemFlags;
					return VSConstants.S_OK;

				default:
					return VSConstants.S_FALSE;
			}
		}

		public int ExecMarkerCommand(IVsTextMarker pMarker, int iItem)
		{
			switch(iItem)
			{
				case 0:
					CloneDetectiveManager.FindClones(CloneDetectiveManager.GetCloneClass(_marker));
					return VSConstants.S_OK;

				case 1:
					CloneDetectiveManager.ShowCloneIntersections();
					return VSConstants.S_OK;

				default:
					return VSConstants.S_FALSE;
			}
		}

		public void OnAfterSpanReload()
		{
		}

		public int OnAfterMarkerChange(IVsTextMarker pMarker)
		{
			return VSConstants.S_OK;
		}

		#endregion
	}
}