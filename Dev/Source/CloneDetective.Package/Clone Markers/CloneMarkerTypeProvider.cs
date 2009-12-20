using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CloneDetective.Package
{
	[Guid(Guids.GuidCloneMarkerServiceString)]
	public sealed class CloneMarkerTypeProvider : IVsTextMarkerTypeProvider
	{
		private CloneBackgroundMarkerType _backgroundMarkerType = new CloneBackgroundMarkerType();
		private CloneMarginMarkerType _marginMarkerType = new CloneMarginMarkerType();

		public int GetTextMarkerType(ref Guid pguidMarker, out IVsPackageDefinedTextMarkerType ppMarkerType)
		{
			// This method is called by Visual Studio when it needs the marker
			// type information in order to retrieve the implementing objects.
			if (pguidMarker == Guids.GuidCloneBackgroundMarker)
			{
				ppMarkerType = _backgroundMarkerType;
				return VSConstants.S_OK;
			}
			else if (pguidMarker == Guids.GuidCloneMarginMarker)
			{
				ppMarkerType = _marginMarkerType;
				return VSConstants.S_OK;
			}

			ppMarkerType = null;
			return VSConstants.E_FAIL;
		}

		internal static void InitializeMarkerIds(VSPackage package)
		{
			// Retrieve the Text Marker IDs. We need them to be able to create instances.
			IVsTextManager textManager = (IVsTextManager) package.GetService(typeof(SVsTextManager));

			int markerId;
			Guid markerGuid = Guids.GuidCloneBackgroundMarker;
			ErrorHandler.ThrowOnFailure(textManager.GetRegisteredMarkerTypeID(ref markerGuid, out markerId));
			CloneBackgroundMarkerType.Id = markerId;

			markerGuid = Guids.GuidCloneMarginMarker;
			ErrorHandler.ThrowOnFailure(textManager.GetRegisteredMarkerTypeID(ref markerGuid, out markerId));
			CloneMarginMarkerType.Id = markerId;
		}
	}
}
