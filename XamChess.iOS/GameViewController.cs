/*
* GameViewController.cs
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
	public class GameViewController : UIViewController
	{
		NewGameViewController newGameViewController;
		bool initial_game;

		public GameViewController ()
		{
			XamGame.Ended += (bool you_won) => 
			{
				var alert = new UIAlertView ();
				if (you_won) {
					switch (Game.PlayerToPlay.Status) {
					case Player.PlayerStatusNames.InCheckMate:
						alert.Message = "You won!";
						break;
					case Player.PlayerStatusNames.InStalemate:
						alert.Message = "Stalemate!";
						break;
					}
				} else {
					switch (Game.PlayerToPlay.Status) {
					case Player.PlayerStatusNames.InCheckMate:
						alert.Message = "You lost!";
						break;
					case Player.PlayerStatusNames.InStalemate:
						alert.Message = "Stalemate!";
						break;
					}
				}
				alert.AddButton ("New Game");
				alert.AddButton ("Review Game");
				alert.Show ();
				alert.Dismissed += (object sender, UIButtonEventArgs e) => 
				{
					switch (e.ButtonIndex) {
					case 0:
						NewGame ();
						break;
					case 1:
						break;
					}
				};
			};
		}

		new GameView View {
			get { return (GameView)base.View; }
			set { base.View = value; }
		}

		protected override void Dispose (bool disposing)
		{
			if (newGameViewController != null) {
				newGameViewController.Dispose ();
				newGameViewController = null;
			}
			base.Dispose (disposing);
		}

		public override void LoadView ()
		{
			initial_game = true;
			View = new GameView (this);
		}

		public override void ViewDidAppear (bool animated)
		{
			if (initial_game) {
				initial_game = false;
				NewGame ();
			}
			base.ViewDidAppear (animated);
		}

		public void NewGame ()
		{
			if (newGameViewController == null)
				newGameViewController = new NewGameViewController (this);

			PresentViewController (newGameViewController, true, null);
		}
	}
}

