using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{
	public class History
	{
		public Player StartPlayer { get; set; }
		public bool ColorSwaped { get; set; }
		public int TurnCount => Turns.Count();
		public List<Move> Turns { get; set; }
		public Player Winner { get; set; }

		public History(Player startplayer)
		{
			StartPlayer = startplayer;
			Turns = new List<Move>();
			Winner = Player.None;
		}
	}

	public class Move : IEquatable<Move>
	{
		public int X;
		public int Y;
		public Player Player;

		public Move(int x, int y, Player player)
		{
			X = x;
			Y = y;
			Player = player;
		}
		public int Id
		{
			get { return (X * 100 + Y) * 100 + (int) Player; }
		}
		public bool Equals(Move other)
		{
			return null != other && Id == other.Id;
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as Move);
		}
		public override int GetHashCode()
		{
			return Id;
		}
	}

	public class ColorSwapMove : Move
	{
		public ColorSwapMove( Player player) : base(-1, -1, player)
		{
		}
	}
}
