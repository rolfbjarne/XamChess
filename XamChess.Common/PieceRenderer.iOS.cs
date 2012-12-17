/*
 * PieceRenderer.iOS.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

using SharpChess.Model;

using RenderTarget=MonoTouch.CoreGraphics.CGContext;
using Color=MonoTouch.CoreGraphics.CGColor;

namespace XamChess
{
	public static partial class PieceRenderer
	{
		static CGAffineTransform? transform;
		
		public static object RenderPiece (SizeF SquareSize, Player.PlayerColourNames player, Piece.PieceNames name)
		{
			var size = SquareSize;
			var scale_w = size.Width / (float) PieceRenderer.Size.Width;
			var scale_h = size.Height / (float) PieceRenderer.Size.Height;
			UIGraphics.BeginImageContextWithOptions (size, false, 0);
			try {
				using (var graphics = UIGraphics.GetCurrentContext ()) {
					graphics.ScaleCTM (scale_w, scale_h);
					PieceRenderer.Render (graphics, player, name);
					return UIGraphics.GetImageFromCurrentImageContext ();
				}
			} finally {
				UIGraphics.EndImageContext ();
			}
		}

		static Color White {
			get { return UIColor.White.CGColor; }
		}

		static Color Black {
			get { return UIColor.Black.CGColor; }
		}

		static void Clean ()
		{
			if (transform != null) {
				target.ConcatCTM (transform.Value.Invert ());
				transform = null;
			}
		}

		static void SetMatrix (float v11, float v12, float v21, float v22, float v31, float v32)
		{
			transform = new CGAffineTransform (v11, v12, v21, v22, v31, v32);
			target.ConcatCTM (transform.Value);
		}
		
		static void Translate (float dx, float dy)
		{
			target.TranslateCTM (dx, dy);
			transform = target.GetCTM ();
		}

		static void SetFillColor (Color color)
		{
			target.SetFillColor (color);
		}

		static void SetStrokeColor (Color color)
		{
			target.SetStrokeColor (color);
		}

		static void MoveTo (float x, float y)
		{
			target.MoveTo (x, y);
		}

		static void AddLineToPoint (float x, float y)
		{
			target.AddLineToPoint (x, y);
		}

		static void AddCurveToPoint (float x1, float y1, float x2, float y2, float x3, float y3)
		{
			target.AddCurveToPoint (x1, y1, x2, y2, x3, y3);
		}

		static void AddCircle (float x, float y, float radius)
		{
			target.AddArc (x, y, radius, 0, (float)(Math.PI * 2), true);
		}

		static void ClosePath ()
		{
			target.ClosePath ();
		}

		static void FillPath ()
		{
			target.FillPath ();
			Clean ();
		}

		static void StrokePath ()
		{
			target.StrokePath ();
			Clean ();
		}

		static void FillAndStrokePath ()
		{
			target.DrawPath (CGPathDrawingMode.FillStroke);
			Clean ();
		}
	}
}

