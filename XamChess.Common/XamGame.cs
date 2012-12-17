/*
 * XamGame.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;
using System.Drawing;
using System.IO;

using SharpChess.Model;

namespace XamChess.Common
{
	public static class XamGame
	{
		static Peer player_white;
		static Peer player_black;

		public static Piece[,] Board;
		public static object [] PieceImages;

		public static Action<Action> InvokeOnMainThread;
		public static SizeF SquareSize;
		public static PointF BoardUpperLeftCorner;
		public static PointF Touched;
		public static PointF Touching;

		public static event Action BoardPositionChanged; // This will always be raised on the main thread.
		public static event Action Invalidate;
		public delegate void EndedDelegate (bool you_won);
		public static event EndedDelegate Ended;

		public static Player PeerPlayerToPlay {
			get {
				return Game.PlayerToPlay.Colour == Player.PlayerColourNames.White
						? (player_white == null ? null : Game.PlayerToPlay) 
						: (player_black == null ? null : Game.PlayerToPlay);
			}
		}

		public static Peer PlayerWhite {
			get {
				return player_white;
			}
			set {
				if (player_white != null)
					player_white.MoveCompleted -= OnMoveCompleted;
				player_white = value;
				if (player_white != null)
					player_white.MoveCompleted += OnMoveCompleted;
			}
		}
		
		public static Peer PlayerBlack {
			get {
				return player_black;
			}
			set {
				if (player_black != null)
					player_black.MoveCompleted += OnMoveCompleted;
				player_black = value;
				if (player_black != null)
					player_black.MoveCompleted += OnMoveCompleted;
			}
		}

		public static void Initialize ()
		{
			if (Board != null)
				return;

			Board = new Piece[8, 8];

			Game.BackupGamePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "Backup.png");
			Game.PlayerWhite.Intelligence = Player.PlayerIntelligenceNames.Human;
			Game.PlayerBlack.Intelligence = Player.PlayerIntelligenceNames.Computer;

			Game.PlayerWhite.Brain.MoveConsideredEvent += () => Console.WriteLine ("White MoveConsideredEvent");
			Game.PlayerBlack.Brain.MoveConsideredEvent += () => 
			{
				// Console.WriteLine ("Black MoveConsideredEvent");
			};
			Game.PlayerWhite.Brain.ThinkingBeginningEvent += () => Console.WriteLine ("White ThinkingBeginningEvent");
			Game.PlayerBlack.Brain.ThinkingBeginningEvent += () => Console.WriteLine ("Black ThinkingBeginningEvent");

			Game.BoardPositionChanged += () => 
			{
				InvokeOnMainThread (() =>
				{
					SaveBoard ();
					if (Invalidate != null)
						Invalidate ();
					if (BoardPositionChanged != null)
						BoardPositionChanged ();
			
					if (PlayerWhite != null)
						PlayerWhite.SendMoveCompleted ();
					if (PlayerBlack != null)
						PlayerBlack.SendMoveCompleted ();

					if (!Game.PlayerToPlay.CanMove && Ended != null) {
						bool won;
						if (XamGame.PeerPlayerToPlay != null)
							won = true;
						else if (Game.PlayerToPlay.Intelligence == Player.PlayerIntelligenceNames.Computer && (XamGame.PlayerWhite != null || XamGame.PlayerBlack != null)) {
							// Computer against other player
							won = false;
						} else if (Game.PlayerToPlay.Intelligence == Player.PlayerIntelligenceNames.Computer) {
							won = true;
						} else {
							won = false;
						}
						Ended (won);
					}
				});
			};

			Game.GamePaused += () => Console.WriteLine ("Paused");
			Game.GameResumed += () => Console.WriteLine ("Resumed");
			Game.GameSaved += () => Console.WriteLine ("Saved");
			Game.SettingsUpdated += () => Console.WriteLine ("SettingsUpdated");

			Game.ClockFixedTimePerMove = TimeSpan.FromSeconds (3);
			Game.MaximumSearchDepth = 20;
			Game.New ();
		}

		public static void LoadResources ()
		{
			PieceImages = new object [12];
			PieceImages [0] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.Black, Piece.PieceNames.Bishop);
			PieceImages [1] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.White, Piece.PieceNames.Bishop);
			PieceImages [2] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.Black, Piece.PieceNames.King);
			PieceImages [3] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.White, Piece.PieceNames.King);
			PieceImages [4] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.Black, Piece.PieceNames.Knight);
			PieceImages [5] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.White, Piece.PieceNames.Knight);
			PieceImages [6] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.Black, Piece.PieceNames.Pawn);
			PieceImages [7] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.White, Piece.PieceNames.Pawn);
			PieceImages [8] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.Black, Piece.PieceNames.Queen);
			PieceImages [9] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.White, Piece.PieceNames.Queen);
			PieceImages [10] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.Black, Piece.PieceNames.Rook);
			PieceImages [11] = PieceRenderer.RenderPiece (SquareSize, Player.PlayerColourNames.White, Piece.PieceNames.Rook);
		}

		public static void SaveBoard ()
		{
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					Board [i, j] = SharpChess.Model.Board.GetPiece (i, j);
				}
			}
		}

		public static void CreateGame (bool from_event = false)
		{
			if (!from_event) {
				if (XamGame.PlayerWhite != null)
					XamGame.PlayerWhite.SendStartGame (true);
				if (XamGame.PlayerBlack != null)
					XamGame.PlayerBlack.SendStartGame (false);
			}

			Game.New ();
			Game.ResumePlay ();
		}

		static void OnMoveCompleted (Peer sender, Move.MoveNames name, Piece from_piece, Square to_square)
		{
			if (from_piece == null)
				return;

			InvokeOnMainThread (() =>
			{
				Game.MakeAMove (name, from_piece, to_square);
			});
		}
		
		public static System.Drawing.RectangleF GetRectangle (int file, int rank)
		{
			var size = SquareSize;
			var point = new System.Drawing.PointF (BoardUpperLeftCorner.X + file * size.Width, BoardUpperLeftCorner.Y + (7 - rank) * size.Height);

			return new System.Drawing.RectangleF (point.X, point.Y, size.Width, size.Height);
		}
		
		public static Square GetSquare (System.Drawing.PointF location)
		{
			var size = SquareSize;

			var file = (int) ((location.X - BoardUpperLeftCorner.X) / size.Width);
			var rank = (int) ((location.Y - BoardUpperLeftCorner.Y) / size.Height);

			if (file < 0 || file > 7 || rank < 0 || rank > 7)
				return null;

			return SharpChess.Model.Board.GetSquare (file, 7 - rank);
		}

		public static void ExecuteMove ()
		{
			var from_square = GetSquare (Touched);
			if (from_square == null || from_square.Piece == null)
				return;

			var from_piece = from_square.Piece;	
			if (from_piece.Player != Game.PlayerToPlay)
				return;

			var to_square = GetSquare (Touching);
			if (to_square == null || (to_square.Piece != null && to_square.Piece.Player.Colour == from_square.Piece.Player.Colour))
				return;

			var legal_moves = new Moves ();

			from_square.Piece.GenerateLegalMoves (legal_moves);

			foreach (var move in legal_moves) {
				if (move.To != to_square)
					continue;

				Game.MakeAMove (move.Name, from_piece, to_square);
				break;
			}
		}

		public static object GetImage (Piece piece) {
			int index = 0;

			if (piece == null)
				return null;

			switch (piece.Name) {
			case Piece.PieceNames.Bishop:
				index = 0;
				break;
			case Piece.PieceNames.King:
				index = 2;
				break;
			case Piece.PieceNames.Knight:
				index = 4;
				break;
			case Piece.PieceNames.Pawn:
				index = 6;
				break;
			case Piece.PieceNames.Queen:
				index = 8;
				break;
			case Piece.PieceNames.Rook:
				index = 10;
				break;
			}

			if (piece.Player.Colour == Player.PlayerColourNames.White)
				index += 1;

			return PieceImages [index];
		}

		public static bool IsHumanTurn {
			get {
				if (Game.PlayerToPlay.Intelligence != Player.PlayerIntelligenceNames.Human)
					return false;

				if (PlayerBlack != null && Game.PlayerToPlay.Colour == Player.PlayerColourNames.Black)
					return false;

				if (PlayerWhite != null && Game.PlayerToPlay.Colour == Player.PlayerColourNames.White)
					return false;

				return true;
			}
		}

		public static void RenderBoard (Action<RectangleF, Square.ColourNames> DrawRect, Action<RectangleF, object> DrawImage)
		{
			object from_piece = null;
			int from_square = -1;
			RectangleF from_rect = RectangleF.Empty;

			// Render the board.
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					var square = SharpChess.Model.Board.GetSquare (i, j);
					var srect = XamGame.GetRectangle (i, j);

					DrawRect (srect, square.Colour);

					if (!XamGame.Touched.IsEmpty && srect.Contains (XamGame.Touched.X, XamGame.Touched.Y) && XamGame.Board [i, j] != null && XamGame.Board [i, j].Player == Game.PlayerToPlay) {
						from_piece = XamGame.GetImage (XamGame.Board [i, j]);
						from_square = i * 8 + j;
						from_rect = srect;
					}
				}
			}

			// Render the pieces
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					var to_square = i * 8 + j;
					var srect = XamGame.GetRectangle (i, j);

					if (!XamGame.Touching.IsEmpty && srect.Contains (XamGame.Touching.X, XamGame.Touching.Y)) {
						var prect = new RectangleF (XamGame.Touching, srect.Size);
						prect.Offset (from_rect.Location.X - XamGame.Touched.X, from_rect.Location.Y - XamGame.Touched.Y);
						DrawImage (prect, from_piece);
					}

					if (from_square != to_square) {
						DrawImage (srect, XamGame.GetImage (XamGame.Board [i, j]));
					}
				}
			}
		}

		public static void TouchDown (PointF pnt)
		{
			if (!XamGame.IsHumanTurn)
				return;

			Touched = pnt;
			Touching = pnt;
			Invalidate ();
		}

		public static void TouchMove (PointF pnt)
		{
			if (!XamGame.IsHumanTurn)
				return;

			Touching = pnt;
			Invalidate ();
		}

		public static void TouchUp ()
		{
			if (!XamGame.IsHumanTurn)
				return;

			ExecuteMove ();
			Invalidate ();
			Touched = PointF.Empty;
			Touching = PointF.Empty;
		}
	}
}
