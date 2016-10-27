using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{
	public enum Player
	{
		None,
		White,
		Black
	}
	class Board
	{
		public int Size { get; private set; }

		public Player[,] BoardState { get; private set; }

		public Player CurrentPlayer { get; private set; }

		public History GameHistory { get; private set; }


		public Board(int size = 12, Player startplayer = Player.White)
		{
			Size = size;
			BoardState = new Player[Size, Size];

			CurrentPlayer = startplayer;

			GameHistory = new HexGame.History(startplayer);
		}

		public void SwapColors()
		{
			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
					BoardState[x, y] = OtherPlayer(BoardState[x, y]);

			GameHistory.Turns.Add(new ColorSwapMove(CurrentPlayer));
			CurrentPlayer = OtherPlayer(CurrentPlayer);
			GameHistory.ColorSwaped = true;
		}

		public bool Play(int x, int y)
		{
			if (!InRange(0, Size, x) || !InRange(0, Size, y))
				return false;
			if (BoardState[x, y] != Player.None)
				return false;

			BoardState[x, y] = CurrentPlayer;
			GameHistory.Turns.Add(new Move(x, y, CurrentPlayer));


			if (CheckWinner(CurrentPlayer))
				this.GameHistory.Winner = CurrentPlayer;
			
			CurrentPlayer = OtherPlayer(CurrentPlayer);

			return true;
		}

		public bool RevertLast()
		{
			if (GameHistory.TurnCount == 0)
				return false;


			var m = GameHistory.Turns.Last();
			GameHistory.Turns.Remove(m);

			if (m is ColorSwapMove)
			{
				SwapColors();
				GameHistory.Turns.Remove(GameHistory.Turns.Last());
				GameHistory.ColorSwaped = false;
				return true;
			}

			BoardState[m.X, m.Y] = Player.None;
			CurrentPlayer = OtherPlayer(CurrentPlayer);

			return true;
		}

		bool CheckWinner(Player p)
		{
			List<Move> reached = new List<Move>();
			List<Move> newreached = new List<Move>();

			List<Move> target = new List<Move>();

			if (p == Player.White)
			{
				for (int i = 0; i < Size; i++)
				{
					if (BoardState[i, 0] == p)
						newreached.Add(new Move(i, 0, p));
					if (BoardState[i, Size-1] == p)
						target.Add(new Move(i, Size - 1, p));
				}
			}
			if (p == Player.Black)
			{
				for (int i = 0; i < Size; i++)
				{
					if (BoardState[0, i] == p)
						newreached.Add(new Move(0, i, p));
					if (BoardState[Size - 1, i] == p)
						target.Add(new Move(Size - 1, i, p));
				}
			}

			while (newreached.Count != 0)
			{
				if (newreached.Any(m => target.Contains(m)))
					return true;

				var newnewreached = new List<Move>();

				foreach (var m in newreached)
					foreach (var cm in GetConnected(m))
						if (!newnewreached.Contains(cm) && !newreached.Contains(cm) && !reached.Contains(cm))
							newnewreached.Add(cm);

				reached.AddRange(newreached);
				newreached = newnewreached;
			} 

			return false;
		}

		IEnumerable<Move> GetConnected(Move m)
		{
			var range = Enumerable.Range(0, Size);
			var result = range.SelectMany(x => range.Select(y => new Move(x, y, BoardState[x, y])).Where(mo => !m.Equals(mo) && 
																										 mo.Player == m.Player && 
																										 InRange(m.X - 1, 3, mo.X) && 
																										 InRange(m.Y - 1, 3, mo.Y) && 
																										 InRange(m.Y + m.X - 1, 3, mo.Y + mo.X)));

			return result;
		}

		bool InRange(int frominclusive, int size, int x)
		{
			return x >= frominclusive && x < frominclusive + size;
		}

		private Player OtherPlayer(Player p)
		{
			if (p == Player.White)
				return Player.Black;
			if (p == Player.Black)
				return Player.White;
			return Player.None;
		}
	}

}
