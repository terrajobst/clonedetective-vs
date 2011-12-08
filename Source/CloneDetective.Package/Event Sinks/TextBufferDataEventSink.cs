using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CloneDetective.Package
{
	public sealed class TextBufferDataEventSink : IVsTextBufferDataEvents
	{
		private IVsTextLines _textLines;
		private IConnectionPoint _connectionPoint;
		private uint _cookie;

		public IVsTextLines TextLines
		{
			get { return _textLines; }
			set { _textLines = value; }
		}

		public IConnectionPoint ConnectionPoint
		{
			get { return _connectionPoint; }
			set { _connectionPoint = value; }
		}

		public uint Cookie
		{
			get { return _cookie; }
			set { _cookie = value; }
		}

		#region IVsTextBufferDataEvents Members

		public void OnFileChanged(uint grfChange, uint dwFileAttrs)
		{
		}

		public int OnLoadCompleted(int fReload)
		{
			// The load procedure completed. Now we can safely notify the
			// CloneDetectiveManager about it and so we don't need to listen to these
			// events any more.
			ConnectionPoint.Unadvise(Cookie);
			CloneDetectiveManager.OnDocumentOpened(TextLines);

			return VSConstants.S_OK;
		}

		#endregion
	}
}