/*
 * PieceRenderer.Android.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using RenderTarget=Android.Graphics.Canvas;

using SharpChess.Model;

namespace XamChess
{
	public static partial class PieceRenderer
	{
		static Path path;
		static Color stroke_color;
		static Color fill_color;
		static Matrix matrix;
		static Matrix original_matrix;
		
		public static Bitmap RenderPiece (System.Drawing.SizeF SquareSize, Player.PlayerColourNames player, Piece.PieceNames name)
		{
			var size = SquareSize;
			var scale_w = size.Width / (float) PieceRenderer.Size.Width;
			var scale_h = size.Height / (float) PieceRenderer.Size.Height;

			var bitmap = Bitmap.CreateBitmap ((int) size.Width, (int) size.Height, Bitmap.Config.Argb8888);
			using (var graphics = new Canvas (bitmap)) {
				graphics.Scale (scale_w, scale_h);
				PieceRenderer.Render (graphics, player, name);
				return bitmap;
			}
		}

		static Color White {
			get { return Color.White; }
		}

		static Color Black {
			get { return Color.Black; }
		}

		static void Init ()
		{
			if (path == null)
				path = new Path ();
		}

		static void Clear ()
		{
			path.Dispose ();
			path = null;

			if (matrix != null) {
				target.Matrix = original_matrix;
				matrix.Dispose ();
				matrix = null;
			}
		}

		static void SetMatrix (float v11, float v12, float v21, float v22, float v31, float v32)
		{
			matrix = new Matrix ();
			matrix.SetValues (new float[] {v11, v21, v31, v12, v22, v32, 0, 0, 1 });
			matrix.PostTranslate (5, 5); // Magic numbers. Not idea why the math doesn't work like on iOS
			original_matrix = target.Matrix;
			target.Matrix = matrix;
		}

		static void Translate (float dx, float dy)
		{
			matrix = new Matrix ();
			matrix.SetTranslate (dx, dy);
			original_matrix = target.Matrix;
			target.Matrix = matrix;
		}

		static void SetFillColor (Color color)
		{
			fill_color = color;
		}

		static void SetStrokeColor (Color color)
		{
			stroke_color = color;
		}

		static void MoveTo (float x, float y)
		{
			Init ();
			path.MoveTo (x, y);
		}

		static void AddLineToPoint (float x, float y)
		{
			Init ();
			path.LineTo (x, y);
		}

		static void AddCurveToPoint (float x1, float y1, float x2, float y2, float x3, float y3)
		{
			Init ();
			path.CubicTo (x1, y1, x2, y2, x3, y3);
		}

		static void AddCircle (float x, float y, float radius)
		{
			Init ();
			path.AddCircle (x, y, radius, Path.Direction.Cw);
		}

		static void ClosePath ()
		{
			path.Close ();
		}

		static void FillPath ()
		{
			using (var paint = new Paint ()) {
				paint.Color = fill_color;
				paint.AntiAlias = true;
				target.DrawPath (path, paint);
				Clear ();
			}
		}

		static void StrokePath ()
		{
			using (var paint = new Paint ()) {
				paint.SetStyle (Paint.Style.Stroke);
				paint.Color = stroke_color;
				paint.StrokeWidth = 1;
				paint.AntiAlias = true;
				target.DrawPath (path, paint);
				Clear ();
			}
		}

		static void FillAndStrokePath ()
		{
			using (var paint = new Paint ()) {
				paint.Color = fill_color;
				paint.AntiAlias = true;
				target.DrawPath (path, paint);
			}
			using (var paint = new Paint ()) {
				paint.SetStyle (Paint.Style.Stroke);
				paint.Color = stroke_color;
				paint.StrokeWidth = 1;
				paint.AntiAlias = true;
				target.DrawPath (path, paint);
			}
			Clear ();
		}
	}
}
