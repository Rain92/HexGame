using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexGame
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.SetStyle(
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.UserPaint |
			              ControlStyles.DoubleBuffer,
			              true);
			typeof (Panel).InvokeMember("DoubleBuffered",
			                            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
			                            null,
			                            panel1,
			                            new object[] {true});
			StartGame();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			PaintBoard(e.Graphics, 100, 100, 25, board);
		}

		Board board;
		Point[,][] hexagons;

		void PaintBoard(Graphics g, int x, int y, int radius, Board board)
		{
			double hr = Math.Sin(Math.PI / 3) * radius;
			double hr2 = Math.Cos(Math.PI / 3) * radius;

			int circleradius = 10;

			Pen borderpen = Pens.Black;
			Brush color1 = Brushes.White;
			Brush color2 = Brushes.Black;
			float barwidth = 20;

			hexagons = new Point[board.Size, board.Size][];

			float x1 = x;
			float y1 = y - radius - 20;

			float x2 = x1 + 2 * (board.Size - 1) * (float)hr;
			float y2 = y1;


			PainBar(g, x1, y1, x2, y2, barwidth, borderpen, color1);

			x1 = (float)Math.Round(x + (board.Size - 1) * (float)hr );
			y1 = (float)Math.Round(y + (board.Size - 1) * (float)(radius + hr2) + radius+ 20) ; 

			x2 = x1 + 2 * (board.Size-1) * (float)hr;
			y2 = y1;

			PainBar(g, x1, y1, x2, y2, barwidth, borderpen, color1);

			var angle = 2 * Math.PI * 5 / 12;

			x1 = x + (float)(Math.Cos(angle) * (radius + 20));
			y1 = y + (float)(Math.Sin(angle) * (radius + 20));

			x2 = x + (float)((( (board.Size - 1)) * hr) + Math.Cos(angle) * (radius + 20));
			y2 = y + (float)((board.Size - 1) * (radius + hr2) + Math.Sin(angle) * (radius + 20));

			PainBar(g, x1, y1, x2, y2, barwidth, borderpen, color2);


			x1 = x + (float)((2 * (board.Size - 1)  *hr) - Math.Cos(angle) * (radius + 20));
			y1 = y - (float)(Math.Sin(angle) * (radius + 20));

			x2 = x + (float)(((3*(board.Size - 1)) * hr) - Math.Cos(angle) * (radius + 20));
			y2 = y + (float)((board.Size - 1) * (radius + hr2) - Math.Sin(angle) * (radius + 20));

			PainBar(g, x1, y1, x2, y2, barwidth, borderpen, color2);

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

		void PainBar(Graphics g, float x1, float y1, float x2, float y2, float width, Pen border, Brush color)
		{

			var dx = x2 - x1;
			var dy = y2 - y1;
			var angle = Math.Sin(dx / dy);
			if (dy == 0)
				angle = Math.Sign(dx) * 2 * Math.PI / 4;
			var da = 2 * Math.PI / 4;
			var offx = Math.Sin(angle + da) * width / 2;
			var offy = Math.Cos(angle + da) * width / 2;

			PointF[] poly = new PointF[5];
			poly[0] = new PointF(x1 - (float)offx, y1 - (float)offy);
			poly[1] = new PointF(x2 - (float)offx, y2 - (float)offy);
			poly[2] = new PointF(x2 + (float)offx, y2 + (float)offy);
			poly[3] = new PointF(x1 + (float)offx, y1 + (float)offy);

			poly[4] = poly[0];

			g.FillEllipse(color, x1 - width / 2, y1 - width / 2, width, width);
			g.FillEllipse(color, x2 - width / 2, y2 - width / 2, width, width);
			g.DrawEllipse(border, x1 - width / 2, y1 - width / 2, width, width);
			g.DrawEllipse(border, x2 - width / 2, y2 - width / 2, width, width);
			g.FillPolygon(color, poly);
			g.DrawLine(border, poly[0], poly[1]);
			g.DrawLine(border, poly[2], poly[3]);
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
			if (hexagons == null)
				return;

			for (int x = 0; x < board.Size; x++)
			{
				for (int y = 0; y < board.Size; y++)
				{
					if (PointInPolygon(hexagons[x, y], e.Location))
					{
						if (board.Play(x, y))
							UpdateBoard();

						return;
					}
				}
			}
		}

		void UpdateBoard()
		{
			label2.Text = "It's " + board.CurrentPlayer.ToString() + "'s Turn.";
			this.Refresh();
			button3.Enabled = board.GameHistory.TurnCount != 0;

			button2.Enabled = board.GameHistory.TurnCount == 1;
			label3.Enabled = board.GameHistory.TurnCount == 1;

			if (board.GameHistory.Winner != Player.None)
				label4.Text = board.GameHistory.Winner + " Wins!";
			else
				label4.Text = "";
		}

		void StartGame()
		{ 
			board = new HexGame.Board((int)numericUpDown1.Value);
			UpdateBoard();
		}


		private void button1_Click(object sender, EventArgs e)
		{
			StartGame();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			board.RevertLast();
			UpdateBoard();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			board.SwapColors();
			UpdateBoard();
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			panel1.Size = new Size(this.Size.Width - 16, this.Size.Height - 89);
		}
	}
}
