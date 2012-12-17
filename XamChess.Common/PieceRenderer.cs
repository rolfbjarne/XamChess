/*
 * PieceRenderer.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using SharpChess.Model;

#if ANDROID
using RenderTarget=Android.Graphics.Canvas;
using Color=Android.Graphics.Color;
#elif IOS
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using RenderTarget=MonoTouch.CoreGraphics.CGContext;
using Color=MonoTouch.CoreGraphics.CGColor;
#else
#error Neither IOS nor ANDROID defined
#endif

namespace XamChess
{
	public static partial class PieceRenderer
	{
		static RenderTarget target;

		public static SizeF Size {
			get { return new SizeF (45, 45); }
		}

		public static void Render (RenderTarget c, Player.PlayerColourNames player, Piece.PieceNames name)
		{
			target = c;

			var color = player == Player.PlayerColourNames.White ? White : Black;
			var main = color;
			var alternate = player != Player.PlayerColourNames.White ? White : Black;


			var white = White;
			var black = Black;

			switch (name) {
			case Piece.PieceNames.Bishop:
				RenderBishop (c, main, alternate, white, black);
				break;
			case Piece.PieceNames.King:
				if (player == Player.PlayerColourNames.White)
					RenderWhiteKing (c, main, alternate, white, black);
				else
					RenderBlackKing (c, main, alternate, white, black);
				break;
			case Piece.PieceNames.Knight:
				RenderKnight (c, player, main, alternate, white, black);
				break;
			case Piece.PieceNames.Pawn:
				RenderPawn (c, main, alternate, white, black);
				break;
			case Piece.PieceNames.Queen:
				if (player == Player.PlayerColourNames.White)
					RenderWhiteQueen (c, white, black);
				else
					RenderBlackQueen (c);
				break;
			case Piece.PieceNames.Rook:
				if (player == Player.PlayerColourNames.White)
					RenderWhiteRook (c, white, black);
				else
					RenderBlackRook (c);
				break;
			}
		}

		public static void RenderBishop (RenderTarget c, Color main, Color alternate, Color white, Color black)
		{
			SetFillColor (main);
			SetStrokeColor (black);
			MoveTo (9f, 36f);
			AddCurveToPoint (12.39f, 35.03f, 19.11f, 36.43f, 22.5f, 34f);
			AddCurveToPoint (25.89f, 36.43f, 32.61f, 35.03f, 36f, 36f);
			AddCurveToPoint (36f, 36f, 37.65f, 36.54f, 39f, 38f);
			AddCurveToPoint (38.32f, 38.97f, 37.35f, 38.99f, 36f, 38.5f);
			AddCurveToPoint (32.61f, 37.53f, 25.89f, 38.96f, 22.5f, 37.5f);
			AddCurveToPoint (19.11f, 38.96f, 12.39f, 37.53f, 9f, 38.5f);
			AddCurveToPoint (7.646f, 38.99f, 6.677f, 38.97f, 6f, 38f);
			AddCurveToPoint (7.354f, 36.06f, 9f, 36f, 9f, 36f);
			ClosePath ();
			MoveTo (9f, 36f);
			FillAndStrokePath ();
			
			SetFillColor (main);
			SetStrokeColor (black);
			MoveTo (15f, 32f);
			AddCurveToPoint (17.5f, 34.5f, 27.5f, 34.5f, 30f, 32f);
			AddCurveToPoint (30.5f, 30.5f, 30f, 30f, 30f, 30f);
			AddCurveToPoint (30f, 27.5f, 27.5f, 26f, 27.5f, 26f);
			AddCurveToPoint (33f, 24.5f, 33.5f, 14.5f, 22.5f, 10.5f);
			AddCurveToPoint (11.5f, 14.5f, 12f, 24.5f, 17.5f, 26f);
			AddCurveToPoint (17.5f, 26f, 15f, 27.5f, 15f, 30f);
			AddCurveToPoint (15f, 30f, 14.5f, 30.5f, 15f, 32f);
			ClosePath ();
			MoveTo (15f, 32f);
			FillAndStrokePath ();
			
			SetFillColor (main);
			SetStrokeColor (black);
			MoveTo (25f, 8f);
			AddCurveToPoint (25f, 9.380712f, 23.88071f, 10.5f, 22.5f, 10.5f);
			AddCurveToPoint (21.11929f, 10.5f, 20f, 9.380712f, 20f, 8f);
			AddCurveToPoint (20f, 6.619288f, 21.11929f, 5.5f, 22.5f, 5.5f);
			AddCurveToPoint (23.88071f, 5.5f, 25f, 6.619288f, 25f, 8f);
			ClosePath ();
			MoveTo (25f, 8f);
			FillAndStrokePath ();
			
			SetStrokeColor (alternate);
			MoveTo (17.5f, 26f);
			AddLineToPoint (27.5f, 26f);
			MoveTo (15f, 30f);
			AddLineToPoint (30f, 30f);
			MoveTo (22.5f, 15.5f);
			AddLineToPoint (22.5f, 20.5f);
			MoveTo (20f, 18f);
			AddLineToPoint (25f, 18f);
			StrokePath ();
		}

		public static void RenderKnight (RenderTarget c, Player.PlayerColourNames player, Color main, Color alternate, Color white, Color black)
		{	
			SetFillColor (main);
			SetStrokeColor (black);
			MoveTo (22f, 10f);
			AddCurveToPoint (32.5f, 11f, 38.5f, 18f, 38f, 39f);
			AddLineToPoint (15f, 39f);
			AddCurveToPoint (15f, 30f, 25f, 32.5f, 23f, 18f);
			FillAndStrokePath ();

			SetFillColor (main);
			SetStrokeColor (black);
			MoveTo (24f, 18f);
			AddCurveToPoint (24.38f, 20.91f, 18.45f, 25.37f, 16f, 27f);
			AddCurveToPoint (13f, 29f, 13.18f, 31.34f, 11f, 31f);
			AddCurveToPoint (9.958f, 30.06f, 12.41f, 27.96f, 11f, 28f);
			AddCurveToPoint (10f, 28f, 11.19f, 29.23f, 10f, 30f);
			AddCurveToPoint (9f, 30f, 5.997f, 31f, 6f, 26f);
			AddCurveToPoint (6f, 24f, 12f, 14f, 12f, 14f);
			AddCurveToPoint (12f, 14f, 13.89f, 12.1f, 14f, 10.5f);
			AddCurveToPoint (13.27f, 9.506f, 13.5f, 8.5f, 13.5f, 7.5f);
			AddCurveToPoint (14.5f, 6.5f, 16.5f, 10f, 16.5f, 10f);
			AddLineToPoint (18.5f, 10f);
			AddCurveToPoint (18.5f, 10f, 19.28f, 8.008f, 21f, 7f);
			AddCurveToPoint (22f, 7f, 22f, 10f, 22f, 10f);
			FillAndStrokePath ();

			SetFillColor (main);
			SetStrokeColor (alternate);
			MoveTo (9.5f, 25.5f);
			AddCurveToPoint (9.5f, 25.77614f, 9.276142f, 26f, 9f, 26f);
			AddCurveToPoint (8.723858f, 26f, 8.5f, 25.77614f, 8.5f, 25.5f);
			AddCurveToPoint (8.5f, 25.22386f, 8.723858f, 25f, 9f, 25f);
			AddCurveToPoint (9.276142f, 25f, 9.5f, 25.22386f, 9.5f, 25.5f);
			ClosePath ();
			MoveTo (9.5f, 25.5f);
			FillAndStrokePath ();

			SetMatrix (0.866f, 0.5f, -0.5f, 0.866f, 9.693f, -5.173f);
			SetFillColor (black);
			SetStrokeColor (alternate);
			MoveTo (15f, 15.5f);
			AddCurveToPoint (15f, 16.32843f, 14.77614f, 17f, 14.5f, 17f);
			AddCurveToPoint (14.22386f, 17f, 14f, 16.32843f, 14f, 15.5f);
			AddCurveToPoint (14f, 14.67157f, 14.22386f, 14f, 14.5f, 14f);
			AddCurveToPoint (14.77614f, 14f, 15f, 14.67157f, 15f, 15.5f);
			ClosePath ();
			MoveTo (15f, 15.5f);
			FillAndStrokePath ();

			if (player == Player.PlayerColourNames.Black) {
				SetFillColor (white);
				MoveTo (24.55f, 10.4f);
				AddLineToPoint (24.1f, 11.85f);
				AddLineToPoint (24.6f, 12f);
				AddCurveToPoint (27.75f, 13f, 30.25f, 14.49f, 32.5f, 18.75f);
				AddCurveToPoint (34.75f, 23.01f, 35.75f, 29.06f, 35.25f, 39f);
				AddLineToPoint (35.2f, 39.5f);
				AddLineToPoint (37.45f, 39.5f);
				AddLineToPoint (37.5f, 39f);
				AddCurveToPoint (38f, 28.94f, 36.62f, 22.15f, 34.25f, 17.66f);
				AddCurveToPoint (31.88f, 13.17f, 28.46f, 11.02f, 25.06f, 10.5f);
				AddLineToPoint (24.55f, 10.4f);
				ClosePath ();
				MoveTo (24.55f, 10.4f);
				FillPath ();
			}
		}

		public static void RenderWhiteRook (RenderTarget c, Color white, Color black)
		{
			SetFillColor (white);
			SetStrokeColor (black);

			MoveTo (9f, 39f);
			AddLineToPoint (36f, 39f);
			AddLineToPoint (36f, 36f);
			AddLineToPoint (9f, 36f);
			AddLineToPoint (9f, 39f);
			ClosePath ();
			MoveTo (9f, 39f);
			FillAndStrokePath ();

			MoveTo (12f, 36f);
			AddLineToPoint (12f, 32f);
			AddLineToPoint (33f, 32f);
			AddLineToPoint (33f, 36f);
			AddLineToPoint (12f, 36f);
			ClosePath ();
			MoveTo (12f, 36f);
			FillAndStrokePath ();

			MoveTo (11f, 14f);
			AddLineToPoint (11f, 9f);
			AddLineToPoint (15f, 9f);
			AddLineToPoint (15f, 11f);
			AddLineToPoint (20f, 11f);
			AddLineToPoint (20f, 9f);
			AddLineToPoint (25f, 9f);
			AddLineToPoint (25f, 11f);
			AddLineToPoint (30f, 11f);
			AddLineToPoint (30f, 9f);
			AddLineToPoint (34f, 9f);
			AddLineToPoint (34f, 14f);
			FillAndStrokePath ();

			MoveTo (34f, 14f);
			AddLineToPoint (31f, 17f);
			AddLineToPoint (14f, 17f);
			AddLineToPoint (11f, 14f);
			FillAndStrokePath ();

			MoveTo (31f, 17f);
			AddLineToPoint (31f, 29.5f);
			AddLineToPoint (14f, 29.5f);
			AddLineToPoint (14f, 17f);
			FillAndStrokePath ();

			MoveTo (31f, 29.5f);
			AddLineToPoint (32.5f, 32f);
			AddLineToPoint (12.5f, 32f);
			AddLineToPoint (14f, 29.5f);
			FillAndStrokePath ();

			MoveTo (11f, 14f);
			AddLineToPoint (34f, 14f);
			StrokePath ();
		}

		public static void RenderBlackRook (RenderTarget c)
		{
			SetFillColor (Black);
			SetStrokeColor (White);

			MoveTo (9f, 39f);
			AddLineToPoint (36f, 39f);
			AddLineToPoint (36f, 36f);
			AddLineToPoint (9f, 36f);
			AddLineToPoint (9f, 39f);
			ClosePath ();
			MoveTo (9f, 39f);
			FillPath ();

			MoveTo (12.5f, 32f);
			AddLineToPoint (14f, 29.5f);
			AddLineToPoint (31f, 29.5f);
			AddLineToPoint (32.5f, 32f);
			AddLineToPoint (12.5f, 32f);
			ClosePath ();
			MoveTo (12.5f, 32f);
			FillPath ();

			MoveTo (12f, 36f);
			AddLineToPoint (12f, 32f);
			AddLineToPoint (33f, 32f);
			AddLineToPoint (33f, 36f);
			AddLineToPoint (12f, 36f);
			ClosePath ();
			MoveTo (12f, 36f);
			FillPath ();

			MoveTo (14f, 29.5f);
			AddLineToPoint (14f, 16.5f);
			AddLineToPoint (31f, 16.5f);
			AddLineToPoint (31f, 29.5f);
			AddLineToPoint (14f, 29.5f);
			ClosePath ();
			MoveTo (14f, 29.5f);
			FillPath ();

			MoveTo (14f, 16.5f);
			AddLineToPoint (11f, 14f);
			AddLineToPoint (34f, 14f);
			AddLineToPoint (31f, 16.5f);
			AddLineToPoint (14f, 16.5f);
			ClosePath ();
			MoveTo (14f, 16.5f);
			FillPath ();

			MoveTo (11f, 14f);
			AddLineToPoint (11f, 9f);
			AddLineToPoint (15f, 9f);
			AddLineToPoint (15f, 11f);
			AddLineToPoint (20f, 11f);
			AddLineToPoint (20f, 9f);
			AddLineToPoint (25f, 9f);
			AddLineToPoint (25f, 11f);
			AddLineToPoint (30f, 11f);
			AddLineToPoint (30f, 9f);
			AddLineToPoint (34f, 9f);
			AddLineToPoint (34f, 14f);
			AddLineToPoint (11f, 14f);
			ClosePath ();
			MoveTo (11f, 14f);
			FillPath ();

			MoveTo (12f, 35.5f);
			AddLineToPoint (33f, 35.5f);
			AddLineToPoint (33f, 35.5f);
			StrokePath ();

			MoveTo (13f, 31.5f);
			AddLineToPoint (32f, 31.5f);
			StrokePath ();

			MoveTo (14f, 29.5f);
			AddLineToPoint (31f, 29.5f);
			StrokePath ();

			MoveTo (14f, 16.5f);
			AddLineToPoint (31f, 16.5f);
			StrokePath ();

			MoveTo (11f, 14f);
			AddLineToPoint (34f, 14f);
			StrokePath ();
		}

		public static void RenderPawn (RenderTarget c, Color main, Color alternate, Color white, Color black)
		{
			SetFillColor (main);
			SetStrokeColor (black);

			MoveTo (22f, 9f);
			AddCurveToPoint (19.79f, 9f, 18f, 10.79f, 18f, 13f);
			AddCurveToPoint (18f, 13.89f, 18.29f, 14.71f, 18.78f, 15.38f);
			AddCurveToPoint (16.83f, 16.5f, 15.5f, 18.59f, 15.5f, 21f);
			AddCurveToPoint (15.5f, 23.03f, 16.44f, 24.84f, 17.91f, 26.03f);
			AddCurveToPoint (14.91f, 27.09f, 10.5f, 31.58f, 10.5f, 39.5f);
			AddLineToPoint (33.5f, 39.5f);
			AddCurveToPoint (33.5f, 31.58f, 29.09f, 27.09f, 26.09f, 26.03f);
			AddCurveToPoint (27.56f, 24.84f, 28.5f, 23.03f, 28.5f, 21f);
			AddCurveToPoint (28.5f, 18.59f, 27.17f, 16.5f, 25.22f, 15.38f);
			AddCurveToPoint (25.71f, 14.71f, 26f, 13.89f, 26f, 13f);
			AddCurveToPoint (26f, 10.79f, 24.21f, 9f, 22f, 9f);

			ClosePath ();
			MoveTo (22f, 9f);
			FillAndStrokePath ();
		}

		public static void RenderWhiteQueen (RenderTarget c, Color white, Color black)
		{
			SetFillColor (white);
			SetStrokeColor (black);

			MoveTo (9f, 26f);
			AddCurveToPoint (17.5f, 24.5f, 30f, 24.5f, 36f, 26f);
			AddLineToPoint (38f, 14f);
			AddLineToPoint (31f, 25f);
			AddLineToPoint (31f, 11f);
			AddLineToPoint (25.5f, 24.5f);
			AddLineToPoint (22.5f, 9.5f);
			AddLineToPoint (19.5f, 24.5f);
			AddLineToPoint (14f, 10.5f);
			AddLineToPoint (14f, 25f);
			AddLineToPoint (7f, 14f);
			AddLineToPoint (9f, 26f);
			ClosePath ();
			MoveTo (9f, 26f);
			FillAndStrokePath ();

			MoveTo (9f, 26f);
			AddCurveToPoint (9f, 28f, 10.5f, 28f, 11.5f, 30f);
			AddCurveToPoint (12.5f, 31.5f, 12.5f, 31f, 12f, 33.5f);
			AddCurveToPoint (10.5f, 34.5f, 10.5f, 36f, 10.5f, 36f);
			AddCurveToPoint (9f, 37.5f, 11f, 38.5f, 11f, 38.5f);
			AddCurveToPoint (17.5f, 39.5f, 27.5f, 39.5f, 34f, 38.5f);
			AddCurveToPoint (34f, 38.5f, 35.5f, 37.5f, 34f, 36f);
			AddCurveToPoint (34f, 36f, 34.5f, 34.5f, 33f, 33.5f);
			AddCurveToPoint (32.5f, 31f, 32.5f, 31.5f, 33.5f, 30f);
			AddCurveToPoint (34.5f, 28f, 36f, 28f, 36f, 26f);
			AddCurveToPoint (27.5f, 24.5f, 17.5f, 24.5f, 9f, 26f);
			ClosePath ();
			MoveTo (9f, 26f);
			FillAndStrokePath ();

			MoveTo (11.5f, 30f);
			AddCurveToPoint (15f, 29f, 30f, 29f, 33.5f, 30f);
			StrokePath ();

			MoveTo (12f, 33.5f);
			AddCurveToPoint (18f, 32.5f, 27f, 32.5f, 33f, 33.5f);
			StrokePath ();


			SetFillColor (White);
			SetStrokeColor (Black);

			AddCircle (6f, 12f, 2.0f);
			FillAndStrokePath ();

			AddCircle (14f, 9, 2.0f);
			FillAndStrokePath ();

			AddCircle (22.5f, 8, 2.0f);
			FillAndStrokePath ();

			AddCircle (31, 9, 2.0f);
			FillAndStrokePath ();

			AddCircle (39, 12, 2.0f);
			FillAndStrokePath ();
		}

		public static void RenderBlackQueen (RenderTarget c)
		{
			SetFillColor (Black);
			SetStrokeColor (White);

			AddCircle (6f, 12f, 2.75f);
			FillPath ();

			AddCircle (14f, 9, 2.75f);
			FillPath ();

			AddCircle (22.5f, 8, 2.75f);
			FillPath ();

			AddCircle (31, 9, 2.75f);
			FillPath ();

			AddCircle (39, 12, 2.75f);
			FillPath ();

			MoveTo (9f, 26f);
			AddCurveToPoint (17.5f, 24.5f, 30f, 24.5f, 36f, 26f);
			AddLineToPoint (38.5f, 13.5f);
			AddLineToPoint (31f, 25f);
			AddLineToPoint (30.7f, 10.9f);
			AddLineToPoint (25.5f, 24.5f);
			AddLineToPoint (22.5f, 10f);
			AddLineToPoint (19.5f, 24.5f);
			AddLineToPoint (14.3f, 10.9f);
			AddLineToPoint (14f, 25f);
			AddLineToPoint (6.5f, 13.5f);
			AddLineToPoint (9f, 26f);
			ClosePath ();
			MoveTo (9f, 26f);
			FillPath ();

			MoveTo (9f, 26f);
			AddCurveToPoint (9f, 28f, 10.5f, 28f, 11.5f, 30f);
			AddCurveToPoint (12.5f, 31.5f, 12.5f, 31f, 12f, 33.5f);
			AddCurveToPoint (10.5f, 34.5f, 10.5f, 36f, 10.5f, 36f);
			AddCurveToPoint (9f, 37.5f, 11f, 38.5f, 11f, 38.5f);
			AddCurveToPoint (17.5f, 39.5f, 27.5f, 39.5f, 34f, 38.5f);
			AddCurveToPoint (34f, 38.5f, 35.5f, 37.5f, 34f, 36f);
			AddCurveToPoint (34f, 36f, 34.5f, 34.5f, 33f, 33.5f);
			AddCurveToPoint (32.5f, 31f, 32.5f, 31.5f, 33.5f, 30f);
			AddCurveToPoint (34.5f, 28f, 36f, 28f, 36f, 26f);
			AddCurveToPoint (27.5f, 24.5f, 17.5f, 24.5f, 9f, 26f);
			ClosePath ();
			MoveTo (9f, 26f);
			FillPath ();

			MoveTo (11f, 38.5f);
			AddCurveToPoint (18.44776f, 41.09097f, 26.55224f, 41.09097f, 34f, 38.5f);
			FillAndStrokePath ();

			MoveTo (11f, 29f);
			AddCurveToPoint (18.44776f, 26.40903f, 26.55224f, 26.40903f, 34f, 29f);
			FillAndStrokePath ();

			MoveTo (12.5f, 31.5f);
			AddLineToPoint (32.5f, 31.5f);
			FillAndStrokePath ();

			MoveTo (11.5f, 34.5f);
			AddCurveToPoint (18.64271f, 36.86467f, 26.35729f, 36.86467f, 33.5f, 34.5f);
			FillAndStrokePath ();

			MoveTo (10.5f, 37.5f);
			AddCurveToPoint (18.24997f, 40.32858f, 26.75003f, 40.32858f, 34.5f, 37.5f);
			FillAndStrokePath ();
		}

		public static void RenderWhiteKing (RenderTarget c, Color main, Color alternate, Color white, Color black)
		{
			SetStrokeColor (black);
			MoveTo (22.5f, 11.63f);
			AddLineToPoint (22.5f, 6f);
			StrokePath ();

			MoveTo (20f, 8f);
			AddLineToPoint (25f, 8f);
			StrokePath ();

			SetFillColor (white);
			MoveTo (22.5f, 25f);
			AddCurveToPoint (22.5f, 25f, 27f, 17.5f, 25.5f, 14.5f);
			AddCurveToPoint (25.5f, 14.5f, 24.5f, 12f, 22.5f, 12f);
			AddCurveToPoint (20.5f, 12f, 19.5f, 14.5f, 19.5f, 14.5f);
			AddCurveToPoint (18f, 17.5f, 22.5f, 25f, 22.5f, 25f);
			FillAndStrokePath ();

			MoveTo (11.5f, 37f);
			AddCurveToPoint (17f, 40.5f, 27f, 40.5f, 32.5f, 37f);
			AddLineToPoint (32.5f, 30f);
			AddCurveToPoint (32.5f, 30f, 41.5f, 25.5f, 38.5f, 19.5f);
			AddCurveToPoint (34.5f, 13f, 25f, 16f, 22.5f, 23.5f);
			AddLineToPoint (22.5f, 27f);
			AddLineToPoint (22.5f, 23.5f);
			AddCurveToPoint (19f, 16f, 9.5f, 13f, 6.5f, 19.5f);
			AddCurveToPoint (3.5f, 25.5f, 11.5f, 29.5f, 11.5f, 29.5f);
			AddLineToPoint (11.5f, 37f);
			ClosePath ();
			MoveTo (11.5f, 37f);
			FillAndStrokePath ();

			MoveTo (11.5f, 30f);
			AddCurveToPoint (17f, 27f, 27f, 27f, 32.5f, 30f);
			StrokePath ();

			MoveTo (11.5f, 33.5f);
			AddCurveToPoint (17f, 30.5f, 27f, 30.5f, 32.5f, 33.5f);
			StrokePath ();

			MoveTo (11.5f, 37f);
			AddCurveToPoint (17f, 34f, 27f, 34f, 32.5f, 37f);
			StrokePath ();
		}

		public static void RenderBlackKing (RenderTarget c, Color main, Color alternate, Color white, Color black)
		{
			SetStrokeColor (black);
			MoveTo (22.5f, 11.63f);
			AddLineToPoint (22.5f, 6f);
			StrokePath ();
			
			SetFillColor (main);
			SetStrokeColor (alternate);
			MoveTo (22.5f, 25f);
			AddCurveToPoint (22.5f, 25f, 27f, 17.5f, 25.5f, 14.5f);
			AddCurveToPoint (25.5f, 14.5f, 24.5f, 12f, 22.5f, 12f);
			AddCurveToPoint (20.5f, 12f, 19.5f, 14.5f, 19.5f, 14.5f);
			AddCurveToPoint (18f, 17.5f, 22.5f, 25f, 22.5f, 25f);
			FillPath ();

			MoveTo (11.5f, 37f);
			AddCurveToPoint (17f, 40.5f, 27f, 40.5f, 32.5f, 37f);
			AddLineToPoint (32.5f, 30f);
			AddCurveToPoint (32.5f, 30f, 41.5f, 25.5f, 38.5f, 19.5f);
			AddCurveToPoint (34.5f, 13f, 25f, 16f, 22.5f, 23.5f);
			AddLineToPoint (22.5f, 27f);
			AddLineToPoint (22.5f, 23.5f);
			AddCurveToPoint (19f, 16f, 9.5f, 13f, 6.5f, 19.5f);
			AddCurveToPoint (3.5f, 25.5f, 11.5f, 29.5f, 11.5f, 29.5f);
			AddLineToPoint (11.5f, 37f);
			ClosePath ();
			MoveTo (11.5f, 37f);
			FillPath ();

			SetStrokeColor (black);
			MoveTo (20f, 8f);
			AddLineToPoint (25f, 8f);
			StrokePath ();

			SetStrokeColor (alternate);
			MoveTo (32f, 29.5f);
			AddCurveToPoint (32f, 29.5f, 40.5f, 25.5f, 38.03f, 19.85f);
			AddCurveToPoint (34.15f, 14f, 25f, 18f, 22.5f, 24.5f);
			AddLineToPoint (22.51f, 26.6f);
			AddLineToPoint (22.5f, 24.5f);
			AddCurveToPoint (20f, 18f, 9.906f, 14f, 6.997f, 19.85f);
			AddCurveToPoint (4.5f, 25.5f, 11.85f, 28.85f, 11.85f, 28.85f);
			StrokePath ();

			MoveTo (11.5f, 30f);
			AddCurveToPoint (17f, 27f, 27f, 27f, 32.5f, 30f);
			MoveTo (11.5f, 33.5f);
			AddCurveToPoint (17f, 30.5f, 27f, 30.5f, 32.5f, 33.5f);
			MoveTo (11.5f, 37f);
			AddCurveToPoint (17f, 34f, 27f, 34f, 32.5f, 37f);
			StrokePath ();
		}
	}
}

