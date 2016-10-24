using System;
using System.Collections.Generic;
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
			if (!(x >= 0 && x < Size && y >= 0 && y < Size))
				return false;
			if (BoardState[x, y] != Player.None)
				return false;

			BoardState[x, y] = CurrentPlayer;
			GameHistory.Turns.Add(new Move(x, y, CurrentPlayer));

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
				GameHistory.ColorSwaped = false;
				return true;
			}

			BoardState[m.X, m.Y] = Player.None;
			CurrentPlayer = OtherPlayer(CurrentPlayer);

			return true;
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
