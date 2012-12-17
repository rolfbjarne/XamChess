/*
* MoveHistoryView.cs
* 
* Authors:
*     Rolf Bjarne Kvinge <rolf@xamarin.com>
* 
* Copyright (C) 2012 Xamarin Inc. All rights reserved.
*/

using System;
using System.Drawing;
using System.Text;

using SharpChess.Model;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace XamChess.iOS
{
	public class MoveHistoryView : UITableView
	{
		static UIFont CellFont = UIFont.FromName ("Helvetica", 10);

		public MoveHistoryView (RectangleF frame)
			: base (frame)
		{
			this.Source = new HistorySource ();
			RowHeight = this.StringSize ("e2-e4", CellFont).Height;
		}

		/*
		 * This crashed under heavy load (machine-vs-machine)
		public new void ReloadData ()
		{
			base.ReloadData ();
			if (Game.MoveHistory.Count > 0) {
				BeginInvokeOnMainThread (() =>
				{
					ScrollToRow (NSIndexPath.FromRowSection ((Game.MoveHistory.Count - 1) / 2, 0), UITableViewScrollPosition.Bottom, true);
				});
			}
		}
		*/

		class HistorySource : UITableViewSource {
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var cell = (MoveCell) tableView.DequeueReusableCell ("History");
				if (cell == null) {
					cell = new MoveCell (tableView.RowHeight, MoveHistoryView.CellFont);
				}

				var white = Game.MoveHistory [indexPath.Row * 2];
				var black = indexPath.Row * 2 + 1 < Game.MoveHistory.Count ? Game.MoveHistory [indexPath.Row * 2 + 1] : null;

				var text = new StringBuilder ();
				text.Append (white.Description);
				if (black != null) {
					text.Append ("    ");
					text.Append (black.Description);
				}

				cell.Number.Text = (indexPath.Row + 1).ToString ();
				cell.White.Text = white.Description;
				cell.Black.Text = black == null ? string.Empty : black.Description;

				return cell;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return (Game.MoveHistory.Count + 1) / 2;
			}
		}

		class MoveCell : UITableViewCell {
			public UILabel Number;
			public UILabel White;
			public UILabel Black;

			public MoveCell (float height, UIFont font)
			{
				float textWidth = this.StringSize ("Rdab-ab  xyz", font).Width;
				Number = new UILabel (new RectangleF (0, 0, this.StringSize ("000 ", font).Width, height));
				Number.Font = font;
				White = new UILabel (new RectangleF (Number.Frame.Right, 0, textWidth, height));
				White.Font = font;
				Black = new UILabel (new RectangleF (White.Frame.Right, 0, White.Frame.Width, White.Frame.Height));
				Black.Font = font;

				ContentView.AddSubview (Number);
				ContentView.AddSubview (White);
				ContentView.AddSubview (Black);
			}
		}
	}
}

