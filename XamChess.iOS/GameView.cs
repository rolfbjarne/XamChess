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

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

using SharpChess.Model;
using XamChess.Common;

namespace XamChess.iOS
{
	public class GameView : UIView
	{
		CGColor white;
		CGColor black;
		UIButton newGame;
		GameViewController controller;
		
		public GameView (GameViewController controller)
		{
			this.controller = controller;

			XamGame.InvokeOnMainThread = (Action action) =>
			{
				InvokeOnMainThread (() => action ());
			};
			XamGame.Initialize ();

			XamGame.Invalidate += () => 
			{
				SetNeedsDisplay ();
			};

			BackgroundColor = UIColor.White;
		}

		void LoadResources ()
		{
			if (white != null)
				return;
			
			int s = (int) (Math.Min (Frame.Width, Frame.Height) / 8);
			XamGame.SquareSize = new SizeF (s, s);
			XamGame.BoardUpperLeftCorner = new PointF (0, (Frame.Height - XamGame.SquareSize.Height * 8) / 2);
			XamGame.LoadResources ();

			newGame = new UIButton (new RectangleF (new PointF (0, XamGame.BoardUpperLeftCorner.Y + XamGame.SquareSize.Height * 8 + 3), new SizeF (Frame.Width, XamGame.BoardUpperLeftCorner.Y - 6)));
			newGame.SetTitle ("New Game", UIControlState.Normal);
			newGame.SetTitleColor (UIColor.Black, UIControlState.Normal);
			newGame.TouchUpInside += (object sender, EventArgs e) => 
			{
				controller.NewGame ();
			};
			AddSubview (newGame);


			white = UIColor.LightGray.CGColor;
			black = UIColor.DarkGray.CGColor;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			XamGame.TouchDown (touches.ToArray<UITouch> () [0].LocationInView (this));
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			XamGame.TouchMove (touches.ToArray<UITouch> () [0].LocationInView (this));
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			XamGame.TouchUp ();
			base.TouchesEnded (touches, evt);
		}

		public override void Draw (System.Drawing.RectangleF rect)
		{
			LoadResources ();

			using (var graphics = UIGraphics.GetCurrentContext ()) {
				XamGame.RenderBoard ((RectangleF r, Square.ColourNames color) =>
				{
					graphics.SetFillColor (color == Square.ColourNames.White ? white : black);
					graphics.FillRect (r);
				}, (RectangleF r, object image) =>
				{
					if (image != null)
						((UIImage) image).Draw (r);
				});
			}

			base.Draw (rect);
		}
	}
}

