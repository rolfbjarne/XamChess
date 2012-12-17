/*
* NewGameViewController.cs
* 
* Authors:
*     Rolf Bjarne Kvinge <rolf@xamarin.com>
* 
* Copyright (C) 2012 Xamarin Inc. All rights reserved.
*/

using System;
using System.Collections.Generic;

using SharpChess.Model;

using MonoTouch.UIKit;
using MonoTouch.Dialog;

using XamChess.Common;

namespace XamChess.iOS
{
	public class NewGameViewController : DialogViewController
	{
		Section white;
		Section black;

		StyledStringElement white_you;
		StyledStringElement white_computer;

		StyledStringElement black_you;
		StyledStringElement black_computer;

		StyledStringElement selected_white;
		StyledStringElement selected_black;

		public NewGameViewController (GameViewController gameViewController)
			: base (null)
		{
			Root = CreateRoot ();
			selected_white = white_you;
			selected_black = black_computer;
			Update ();

			foreach (var peer in Network.Peers)
				AddPeer (peer);

			Network.PeerAdded += (Peer obj) => 
			{
				BeginInvokeOnMainThread (() => 
				{
					AddPeer (obj);
				});
			};
			Network.PeerRemoved += (Peer obj) => 
			{
				BeginInvokeOnMainThread (() =>
				{
					foreach (StyledStringElement el in white.Elements) {
						if (el.Caption == obj.DisplayName) {
							white.Remove (el);
							break;
						}
					}

					foreach (StyledStringElement el in black.Elements) {
						if (el.Caption == obj.DisplayName) {
							black.Remove (el);
							break;
						}
					}
					ReloadData ();
				});
			};
		}

		void AddPeer (Peer obj)
		{
			var wel = new StyledStringElement (obj.DisplayName);
			wel.Tapped += () => { SelectWhite (wel, obj); };
			white.Add (wel);
			var bel = new StyledStringElement (obj.DisplayName);
			bel.Tapped += () => { SelectBlack (bel, obj); };
			black.Add (bel);
			ReloadData ();

			Peer.ColoursSelectedDelegate selectColours = (Peer sender, bool is_white) =>
			{
				if (is_white) {
					SelectWhite (white_you, null, true);
					SelectBlack (bel, sender, true);
				} else {
					SelectWhite (wel, sender, true);
					SelectBlack (black_you, null, true);
				}
			};
			obj.ColoursSelected += (Peer sender, bool is_white) => 
			{
				BeginInvokeOnMainThread (() =>
				                         {
					selectColours (sender, is_white);
				});
			};

			obj.StartGame += (Peer sender, bool is_white) => 
			{
				BeginInvokeOnMainThread (() =>
				{
					CreateGame (true);
				});
			};
		}

		RootElement CreateRoot ()
		{
			return new RootElement ("New Game")
			{
				(white = new Section ("White")
				{
					(white_you = new StyledStringElement ("Human", () => { SelectWhite (white_you); } )),
					(white_computer = new StyledStringElement ("Engine", () => { SelectWhite (white_computer); })),
				}),
				(black = new Section ("Black")
				{
					(black_you = new StyledStringElement ("Human", () => { SelectBlack (black_you); } )),
					(black_computer = new StyledStringElement ("Engine", () => { SelectBlack (black_computer); } )),
				}),
				new Section ()
				{
					new ImageStringElement ("Mate Me!", () => CreateGame (false), UIImage.FromFile ("cat2.jpg")),
				}
			};
		}

		void CreateGame (bool from_event = false)
		{
			DismissViewController (true, () =>
			{
				XamGame.CreateGame (from_event);
			});
		}

		void SelectWhite (StyledStringElement el, Peer peer = null, bool from_event = false)
		{
			XamGame.PlayerWhite = peer;
			selected_white = el;
			if (selected_white == white_computer)
				Game.PlayerWhite.Intelligence = Player.PlayerIntelligenceNames.Computer;
			else
				Game.PlayerWhite.Intelligence = Player.PlayerIntelligenceNames.Human;
			if (peer != null && !from_event)
				peer.SendColoursSelected (true);
			Update ();
		}

		void SelectBlack (StyledStringElement el, Peer peer = null, bool from_event = false)
		{
			XamGame.PlayerBlack = peer;
			selected_black = el;
			if (selected_black == black_computer)
				Game.PlayerBlack.Intelligence = Player.PlayerIntelligenceNames.Computer;
			else
				Game.PlayerBlack.Intelligence = Player.PlayerIntelligenceNames.Human;
			if (peer != null && !from_event)
				peer.SendColoursSelected (false);
			Update ();
		}

		void Update ()
		{
			foreach (StyledStringElement el in white.Elements) {
				el.BackgroundColor = el == selected_white ? UIColor.Green : UIColor.White;
			}
			foreach (StyledStringElement el in black.Elements) {
				el.BackgroundColor = el == selected_black ? UIColor.Green : UIColor.White;
			}

			ReloadData ();
		}
	}
}

