/*
 * GameActivity.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using SharpChess.Model;

using XamChess.Common;

namespace XamChess.Android
{
	[Activity (Label = "XamChess", MainLauncher = true)]
	public class GameActivity : Activity
	{
		RadioButton selected_white;
		RadioButton selected_black;

		RadioGroup white_group;
		RadioGroup black_group;
		RadioButton white_computer;
		RadioButton white_you;
		RadioButton black_computer;
		RadioButton black_you;

		List<RadioButton> white_buttons;
		List<RadioButton> black_buttons;

		Dictionary<string, PeerWrapper> tags = new Dictionary<string, PeerWrapper> ();

		bool created;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (created)
				return;
			created = true;

			XamGame.InvokeOnMainThread = (Action action) =>
			{
				RunOnUiThread (() => action ());
			};
			XamGame.Ended += (bool you_won) =>
			{				
				var alert = new AlertDialog.Builder (this);
				alert.SetTitle ("Game Finished");
				if (you_won) {
					switch (Game.PlayerToPlay.Status) {
					case Player.PlayerStatusNames.InCheckMate:
						alert.SetMessage ("You won!");
						break;
					case Player.PlayerStatusNames.InStalemate:
						alert.SetMessage ("Stalemate!");
						break;
					}
				} else {
					switch (Game.PlayerToPlay.Status) {
					case Player.PlayerStatusNames.InCheckMate:
						alert.SetMessage ("You lost!");
						break;
					case Player.PlayerStatusNames.InStalemate:
						alert.SetMessage ("Stalemate!");
						break;
					}
				}
				alert.SetPositiveButton ("New Game", (a, b) => { NewGame (); });
				alert.SetNeutralButton ("Review Game", (a, b) => { });
				alert.Show ();
			};
			XamGame.Initialize ();
			Network.DiscoverPeers ();

			NewGame ();
		}

		void CreateGame (bool from_event = false)
		{
			XamGame.CreateGame (from_event);
			SetContentView (new GameView (this, this.BaseContext, null));
		}

		public void NewGame ()
		{
			SetContentView (Resource.Layout.NewGameLayout);

			var button = (Button) FindViewById (Resource.Id.buttonNewGame);
			button.Click += (object sender, EventArgs e) => 
			{
				CreateGame ();
			};

			white_buttons = new List<RadioButton> ();
			black_buttons = new List<RadioButton> ();
			
			white_group = (RadioGroup) FindViewById (Resource.Id.radioGroupWhite);
			black_group = (RadioGroup) FindViewById (Resource.Id.radioGroupBlack);

			white_computer = (RadioButton) FindViewById (Resource.Id.radioButtonWhiteComputer);
			white_buttons.Add (white_computer);
			white_you = (RadioButton) FindViewById (Resource.Id.radioButtonWhiteHuman);
			white_buttons.Add (white_you);

			black_computer = (RadioButton) FindViewById (Resource.Id.radioButtonBlackComputer);
			black_buttons.Add (black_computer);
			black_you = (RadioButton) FindViewById (Resource.Id.radioButtonBlackHuman);
			black_buttons.Add (black_you);

			foreach (var b in white_buttons) {
				var capture = b;
				b.Click += (sender, e) => {	SelectWhite (capture); };
			}
			foreach (var b in black_buttons) {
				var capture = b;
				b.Click += (sender, e) => {	SelectBlack (capture); };
			}
		
			foreach (var peer in Network.Peers)
				AddPeer (peer);

			Network.PeerAdded += (Peer obj) => 
			{
				RunOnUiThread (() =>
				{
					AddPeer (obj);
				});
			};

			Network.PeerRemoved += (Peer obj) => 
			{
				RunOnUiThread (() =>
				{
					try {
						var white = (RadioButton) white_group.FindViewWithTag (tags [obj.Id]);
						white_group.RemoveView (white);
						white_buttons.Remove (white);
						var black = (RadioButton) black_group.FindViewWithTag (tags [obj.Id]);
						black_group.RemoveView (black);
						black_buttons.Remove (black);
					} catch (Exception ex) {
						Console.WriteLine ("Could not remove peer: {0}", ex.Message);
					}
				});
			};
		}

		void AddPeer (Peer obj)
		{
			var tag = new PeerWrapper () { WrappedPeer = obj };

			var rgw = new RadioButton (this);
			rgw.Text = obj.DisplayName;
			rgw.Tag = tag;
			rgw.Click += (sender, e) => { SelectWhite (rgw, obj); };
			white_group.AddView (rgw);
			white_buttons.Add (rgw);

			var rgb = new RadioButton (this);
			rgb.Text = obj.DisplayName;
			rgb.Tag = tag;
			rgb.Click += (sender, e) => { SelectBlack (rgb, obj); };
			black_group.AddView (rgb);
			black_buttons.Add (rgb);

			tags [obj.Id] = tag;
			
			Peer.ColoursSelectedDelegate selectColours = (Peer sender, bool is_white) =>
			{
				if (is_white) {
					SelectWhite (white_you, null, true);
					SelectBlack (rgb, sender, true);
				} else {
					SelectWhite (rgw, sender, true);
					SelectBlack (black_you, null, true);
				}
			};

			obj.ColoursSelected += (Peer sender, bool white) => 
			{
				RunOnUiThread (() => selectColours (sender, white));
			};

			obj.StartGame += (Peer sender, bool white) => 
			{
				RunOnUiThread (() =>
				{
					CreateGame (true);
				});
			};
		}

		void SelectWhite (RadioButton el, Peer peer = null, bool from_event = false)
		{
			XamGame.PlayerWhite = peer;
			selected_white = el;
			selected_white.Checked = true;
			if (selected_white == white_computer) {
				Game.PlayerWhite.Intelligence = Player.PlayerIntelligenceNames.Computer;
			} else {
				Game.PlayerWhite.Intelligence = Player.PlayerIntelligenceNames.Human;
			}
			if (peer != null && !from_event)
				peer.SendColoursSelected (true);
		}

		void SelectBlack (RadioButton el, Peer peer = null, bool from_event = false)
		{
			XamGame.PlayerBlack = peer;
			selected_black = el;
			selected_black.Checked = true;
			if (selected_black == black_computer) {
				Game.PlayerBlack.Intelligence = Player.PlayerIntelligenceNames.Computer;
			} else {
				Game.PlayerBlack.Intelligence = Player.PlayerIntelligenceNames.Human;
			}
			if (peer != null && !from_event)
				peer.SendColoursSelected (false);
		}

		class PeerWrapper : Java.Lang.Object {
			public Peer WrappedPeer;
		}
	}
}


