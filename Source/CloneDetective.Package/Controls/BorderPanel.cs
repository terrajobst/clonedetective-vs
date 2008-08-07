using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CloneDetective.Package
{
	public sealed class BorderPanel : Panel
	{
		private Border3DSide _borderSides = Border3DSide.All;

		public BorderPanel()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Opaque, true);
			UpdatePadding();
		}

		private void UpdatePadding()
		{
			int left = 0;
			int right = 0;
			int top = 0;
			int bottom = 0;

			if ((_borderSides & Border3DSide.Top) == Border3DSide.Top)
				top = 1;

			if ((_borderSides & Border3DSide.Left) == Border3DSide.Left)
				left = 1;

			if ((_borderSides & Border3DSide.Right) == Border3DSide.Right)
				right = 1;

			if ((_borderSides & Border3DSide.Bottom) == Border3DSide.Bottom)
				bottom = 1;

			Padding = new Padding(left, top, right, bottom);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle rect = ClientRectangle;
			rect.Width--;
			rect.Height--;

			using (SolidBrush brush = new SolidBrush(BackColor))
				e.Graphics.FillRectangle(brush, ClientRectangle);
			
			if (_borderSides == Border3DSide.All)
				e.Graphics.DrawRectangle(SystemPens.ControlDark, rect);
			else
			{
				if ((_borderSides & Border3DSide.Top) == Border3DSide.Top)
					e.Graphics.DrawLine(SystemPens.ControlDark, rect.Left, rect.Top, rect.Right, rect.Top);

				if ((_borderSides & Border3DSide.Left) == Border3DSide.Left)
					e.Graphics.DrawLine(SystemPens.ControlDark, rect.Left, rect.Top, rect.Left, rect.Bottom);

				if ((_borderSides & Border3DSide.Right) == Border3DSide.Right)
					e.Graphics.DrawLine(SystemPens.ControlDark, rect.Right, rect.Top, rect.Right, rect.Bottom);

				if ((_borderSides & Border3DSide.Bottom) == Border3DSide.Bottom)
					e.Graphics.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			}
		}

		[Editor(typeof(BorderSidesEditor), typeof(UITypeEditor))]
		public Border3DSide BorderSides
		{
			get { return _borderSides; }
			set
			{
				_borderSides = value;
				UpdatePadding();
				Invalidate();
			}
		}
	}
}