using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CloneDetective.Package
{
	public sealed partial class GroupLabel : Label
	{
		public GroupLabel()
		{
			InitializeComponent();
		}

		[DefaultValue(false)]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		protected override Padding DefaultMargin
		{
			get { return new Padding(3, 10, 3, 10); }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			const int lineSpacing = 4;

			Pen pen = SystemPens.ControlDarkDark;
			int middleY = (int)((this.ClientRectangle.Height - this.Padding.Size.Height) / 2 - pen.Width / 2) + 1 + Padding.Top;
			e.Graphics.DrawLine(pen, PreferredWidth + lineSpacing, middleY, ClientRectangle.Width, middleY);
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)
				height = PreferredHeight;

			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
}