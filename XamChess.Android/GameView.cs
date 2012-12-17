/*
 * GameView.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SharpChess.Model;

using XamChess.Common;

namespace XamChess.Android
{
	public class GameView : View
	{
		global::Android.Graphics.Color? white;
		global::Android.Graphics.Color? black;
		GameActivity activity;

		public GameView (GameActivity activity, Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			this.activity = activity;
			Initialize ();
		}

		private void Initialize ()
		{
			XamGame.Invalidate += () => 
			{
				Invalidate ();
			};

			Game.New ();
		}
		
		void LoadResources ()
		{
			if (white != null)
				return;

			white = global::Android.Graphics.Color.LightGray;
			black = global::Android.Graphics.Color.DarkGray;
		
			int s = (int) (Math.Min (this.Right, this.Bottom) / 8);
			XamGame.SquareSize = new Size (s, s);
			XamGame.BoardUpperLeftCorner = new global::System.Drawing.PointF (0, (this.Bottom - XamGame.SquareSize.Height * 8) / 2);
			XamGame.LoadResources ();
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Down:
				XamGame.TouchDown (new System.Drawing.PointF (e.GetX (), e.GetY ()));
				return true;
			case MotionEventActions.Move:
				XamGame.TouchMove (new System.Drawing.PointF (e.GetX (), e.GetY ()));
				return true;
			case MotionEventActions.Up:
				XamGame.TouchUp ();
				if (e.GetY () > XamGame.BoardUpperLeftCorner.Y + XamGame.SquareSize.Height * 8 + 10) {
					activity.NewGame ();
				}
				return true;
			}

			return false;
		}

		public override void Draw (Canvas canvas)
		{
			LoadResources ();

			var blackPaint = new Paint () { Color = black.Value };
			var whitePaint = new Paint () { Color = white.Value };

			XamGame.RenderBoard ((RectangleF rect, Square.ColourNames color) =>
			{
				var paint = color == Square.ColourNames.White ? whitePaint : blackPaint;
				canvas.DrawRect (rect.X, rect.Y, rect.Right, rect.Bottom, paint);

			}, (RectangleF rect, object image) =>
			{
				if (image != null)
					canvas.DrawBitmap ((Bitmap) image, rect.Left, rect.Top, null);
			});

			// New Game button
			whitePaint.Color = white.Value;
			whitePaint.SetStyle (Paint.Style.Fill);
			whitePaint.TextSize = 30;
			whitePaint.AntiAlias = true;
			Rect bounds = new Rect ();
			whitePaint.GetTextBounds ("New Game", 0, 8, bounds);
			canvas.DrawText ("New Game", (this.Width - bounds.Width ()) / 2, this.Bottom - (XamGame.BoardUpperLeftCorner.Y - bounds.Height ()) / 2, whitePaint);

			whitePaint.Dispose ();
			blackPaint.Dispose ();

			base.Draw (canvas);
		}
	}
}
