using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexGame
{
	public partial class Form1 : Form
	{
		Board board;
		public Form1()
		{
			
			InitializeComponent();
			board = new HexGame.Board(5);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			PaintBoard(e.Graphics, 80, 80, 25, board);
		}

		Point[,][] hexagons;

		void PaintBoard(Graphics g, int x, int y, int radius, Board board)
		{
			double hr = Math.Sin(Math.PI / 3) * radius;
			double hr2 = Math.Cos(Math.PI / 3) * radius;

			int circleradius = 10;

			Pen borderpen = Pens.Black;
			Brush color1 = Brushes.White;

			hexagons = new Point[board.Size, board.Size][];

			float x1 = x - (float)hr;
			float y1 = y - radius - 30;

			float x2 = x1 + 2 * board.Size * (float)hr;
			float y2 = y1;
			

			g.FillEllipse(color1, x1 - 10, y1, 20, 20);
			g.FillEllipse(color1, x2 - 10, y1, 20, 20);
			g.DrawEllipse(borderpen, x1 - 10, y1, 20, 20);
			g.DrawEllipse(borderpen, x2 - 10 , y1, 20, 20);
			g.FillRectangle(Brushes.White, x1, y1, x2 - x1, 20);
			g.DrawLine(borderpen, x1, y1, x2, y1 );
			g.DrawLine(borderpen, x1, y1+ 20, x2, y1+ 20);


			x1 = (board.Size+1) *(float)hr +(float)hr;
			y1 = y  + board.Size * (float)(radius + hr2); 

			x2 = x1 + 2 * board.Size * (float)hr;
			y2 = y1;
			
			g.FillEllipse(color1, x1 - 10, y1, 20, 20);
			g.FillEllipse(color1, x2 - 10, y1, 20, 20);
			g.DrawEllipse(borderpen, x1 - 10, y1, 20, 20);
			g.DrawEllipse(borderpen, x2 - 10, y1, 20, 20);
			g.FillRectangle(Brushes.White, x1, y1, x2 - x1, 20);
			g.DrawLine(borderpen, x1, y1, x2, y1);
			g.DrawLine(borderpen, x1, y1 + 20, x2, y1 + 20);


			x1 = x - (float)hr - 40;
			y1 = y - radius + (float) hr2;

			y2 = y1 + (board.Size) * (float)(radius + hr2) -(float) hr2;
			x2 = x1 +  (y2 - y1) *(float)((2*hr)/(radius+hr2)/2);


			g.FillEllipse(color1, x1 - 10, y1 - 10, 20, 20);
			g.FillEllipse(color1, x2 - 10, y2 - 10 , 20, 20);
			g.DrawEllipse(borderpen, x1 - 10, y1 -10, 20, 20);
			g.DrawEllipse(borderpen, x2 - 10, y2 - 10, 20, 20);
			//g.FillRectangle(Brushes.White, x1, y1, x2 - x1, 20);
			g.DrawLine(borderpen, x1, y1, x2, y2);
			//g.DrawLine(borderpen, x1, y1 + 20, x2, y2 + 20);

			for (int ix = 0; ix < board.Size; ix++)
			{
				for (int iy = 0; iy < board.Size; iy++)
				{
					int px = x + (int)Math.Round((2 * ix + iy) * hr);
					int py = y + (int)Math.Round(iy * (radius + hr2));
					hexagons[ix, iy] = PaintHexagon(g, px, py, radius);

					if (board.BoardState[ix, iy] == Player.White)
					{
						g.FillEllipse(Brushes.White, new Rectangle(px - circleradius, py - circleradius, 2 * circleradius, 2 * circleradius));
						g.DrawEllipse(Pens.Black, new Rectangle(px - circleradius, py - circleradius, 2 * circleradius, 2 * circleradius));
					}

					if (board.BoardState[ix, iy] == Player.Black)
						g.FillEllipse(Brushes.Black, new Rectangle(px - circleradius, py - circleradius, 2 * circleradius, 2 * circleradius));
				}
			}

		}

		Point[] PaintHexagon(Graphics g, int x, int y, double radius, double rotation = Math.PI / 6)
		{
			Point[] ps = new Point[7];

			for (int i = 0; i < 6; i++)
			{
				Point p = new Point();
				p.X = x + (int)Math.Round(Math.Cos(i  * 2 * Math.PI/ 6 + rotation) * radius);

				p.Y = y + (int)Math.Round(Math.Sin(i  * 2 * Math.PI/ 6 + rotation) * radius);
				ps[i] = p;
			}
			ps[6] = ps[0];

			var pen = new Pen(Color.Black, 2);
			g.FillPolygon(Brushes.Yellow, ps);
			g.DrawPolygon(pen, ps);


			return ps;
		}
		
		private bool PointInPolygon(Point[] polygon, PointF testPoint)
		{
			bool result = false;
			int j = polygon.Count() - 1;
			for (int i = 0; i < polygon.Count(); i++)
			{
				if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
				{
					if (polygon[i].X + ((float)testPoint.Y - polygon[i].Y) / ((float)polygon[j].Y - polygon[i].Y) * ((float)polygon[j].X - polygon[i].X) < testPoint.X)
					{
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}

		private void panel1_MouseClick(object sender, MouseEventArgs e)
		{
			this.Text = e.Location.ToString();
			if (hexagons == null)
				return;

			for (int x = 0; x < board.Size; x++)
			{
				for (int y = 0; y < board.Size; y++)
				{
					if (PointInPolygon(hexagons[x, y], e.Location))
					{
						if(board.Play(x, y))
							this.Refresh();
						return;
					}
				}
			}
		}
	}
}
