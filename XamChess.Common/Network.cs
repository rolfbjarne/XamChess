/*
 * Network.cs
 * 
 * Authors:
 *     Rolf Bjarne Kvinge <rolf@xamarin.com>
 * 
 * Copyright (C) 2012 Xamarin Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text;

using SharpChess.Model;

namespace XamChess.Common
{
	public class Peer {
		public bool Connecting;
		public string Id;
		public string DisplayName;
		public DateTime Stamp;
		public IPAddress Address;

		public delegate void ReceivedDelegate (Peer sender, string data);
		public event ReceivedDelegate Received;
		public void OnReceived (string data)
		{
			if (Received != null)
				Received (this, data);
		}

		public delegate void ColoursSelectedDelegate (Peer sender, bool white);
		public event ColoursSelectedDelegate ColoursSelected;
		public void OnColoursSelected (bool white)
		{
			if (ColoursSelected != null)
				ColoursSelected (this, white);
		}
		
		public void SendColoursSelected (bool white)
		{
			Network.Send (this, string.Format ("ColourSelected: {0}", white ? "White" : "Black"));
		}

		public delegate void StartGameDelegate (Peer sender, bool white);
		public event StartGameDelegate StartGame;
		public void OnStartGame (bool white)
		{
			if (StartGame != null)
				StartGame (this, white);
		}

		public void SendStartGame (bool white)
		{
			Network.Send (this, string.Format ("StartGame: {0}", white ? "White" : "Black"));
		}

		public delegate void MoveCompletedDelegate (Peer sender, Move.MoveNames name, Piece from_piece, Square to_square);
		public event MoveCompletedDelegate MoveCompleted;
		public void OnMoveCompleted (string move)
		{
			if (MoveCompleted != null) {
				var split = move.Split ('-');
				var nm = split [0];
				var name = (Move.MoveNames) Enum.Parse (typeof (Move.MoveNames), nm);
				var from_ordinal = int.Parse (split [1]);
				var to_ordinal = int.Parse (split [2]);
				var from_piece = Board.GetPiece (from_ordinal);
				var to_square = Board.GetSquare (to_ordinal);
				MoveCompleted (this, name, from_piece, to_square);
				lastFen = Fen.GetBoardPosition ();
			}
		}

		string lastFen;
		public void SendMoveCompleted ()
		{
			var fen = Fen.GetBoardPosition ();
			if (lastFen == fen)
				return;
			lastFen = fen;

			if (Game.MoveHistory.Count == 0)
				return;

			var move = Game.MoveHistory.Last;
			var str = move.Name + "-" + move.From.Ordinal.ToString () + "-" + move.To.Ordinal.ToString ();

			Network.Send (this, string.Format ("MoveCompleted: {0}", str));
		}

		public delegate void ConnectedDelegate (Peer sender);
		public event ConnectedDelegate Connected;
		public void OnConnected ()
		{
			if (Connected != null)
				Connected (this);
		}
	}

	public static class Network
	{
		static Dictionary<string, Peer> peers;

		public static event Action<Peer> PeerAdded;
		public static event Action<Peer> PeerRemoved;

		public static int Port = 8155;

		public static IEnumerable<Peer> Peers {
			get {
				lock (peers) {
					// Return a copy of the list so that the caller doesn't have to lock anything.
					return new List<Peer> (peers.Values);
				}
			}
		}

		static string CalculateId (IPAddress address)
		{
			var id = new StringBuilder ();
			foreach (var b in address.GetAddressBytes ()) {
				if (id.Length > 0)
					id.Append (':');
				id.Append (b);
			}
			return id.ToString ();
		}

		static void Process (IPAddress from, string id, string message)
		{
			string c;

			if (!message.StartsWith ("XamChess: "))
				return;

			message = message.Substring ("XamChess: ".Length);

			if (!message.StartsWith ("C: "))
				return;

			c = message.Substring (3, message.IndexOf (' ', 3) - 3);
			message = message.Substring (3 + c.Length + 1);

			Peer peer = null;
			lock (peers) {
				peers.TryGetValue (id, out peer);
			}

			if (message.StartsWith ("Peer: ")) {
				// Peer broadcasting.
				lock (peers) {
					if (peer == null) {
						peer = new Peer ()
						{
							Id = id,
							DisplayName = message.Substring ("Peer: ".Length),
							Address = from,
						};
						peers.Add (id, peer);
						if (PeerAdded != null)
							PeerAdded (peer);
					}
					peer.Stamp = DateTime.Now;
				}
			} else if (message.StartsWith ("ColourSelected: ")) {
				peer.OnColoursSelected (message.Substring ("ColourSelected: ".Length) == "White");
			} else if (message.StartsWith ("StartGame: ")) {
				peer.OnStartGame (message.Substring ("StartGame: ".Length) == "White");
			} else if (message.StartsWith ("MoveCompleted: ")) {
				peer.OnMoveCompleted (message.Substring ("MoveCompleted: ".Length));
			}
		}

		static void Listen ()
		{
			try {
				bool reconnect = true;
				while (reconnect) {
					reconnect = false;

					var myIps = GetCurrentIPAddresses ();

					using (var listener = new UdpClient (Port)) {
						listener.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
						while (true) {
							try {
								IPEndPoint group = new IPEndPoint (IPAddress.Any, Port);
								//Console.WriteLine ("Receiving...");
								Stopwatch watch = new Stopwatch  ();
								watch.Start ();

								var data = listener.Receive (ref group);
								var str = Encoding.UTF8.GetString (data);

								// Check if it's a message from ourselves
								bool self_message = false;
								foreach (var myIp in myIps) {
									var a = myIp.GetAddressBytes ();
									var b = group.Address.GetAddressBytes ();
									if (a.Length == b.Length) {
										bool equal = true;
										for (int i = 0; i < a.Length && equal; i++) {
											equal = a [i] == b [i];
										}
										if (equal) {
											self_message = true;
											break;
										}
									}
								}
								if (self_message)
									continue;
								
								var id = CalculateId (group.Address);
								Process (group.Address, id, str);
							} catch (Exception ex) {
								Console.WriteLine ("Receive exception: {0}", ex);
								reconnect = true;
								break;
							}
						}
					}
				}
			} catch (Exception ex) {
				Console.WriteLine ("Listen exception: {0}", ex);
			}
		}

		static string DisplayName {
			get {
#if ANDROID
				return "Rolf's Android Phone";
				// FIXME...
				// return Android.Accounts.AccountManager.Get (null).GetAccountsByType ("com.google") [0].Name;
#elif IOS
				return MonoTouch.UIKit.UIDevice.CurrentDevice.Name;
#endif
			}
		}

		static void Publish ()
		{
			try {
				while (true) {
					Broadcast ("Peer: " + DisplayName);
					// Find disconnected peers.
					lock (peers) {
						var tmp = new List<Peer> (peers.Values);
						foreach (var peer in tmp) {
							if ((DateTime.Now - peer.Stamp).TotalSeconds > 10) {
								peers.Remove (peer.Id);
								if (PeerRemoved != null)
									PeerRemoved (peer);
							}
						}
					}
					Thread.Sleep (2000); // publish ourselves every 2 seconds
				}
			} catch (Exception ex) {
				Console.WriteLine ("Publish exception: {0}", ex);
			}
		}
		
		public static void Send (Peer peer, string data)
		{
			Send (peer.Address, data);
		}

		static int send_counter;
		static void Send (IPAddress target, string message)
		{
			try {
				message = "XamChess: C: " + (++send_counter).ToString () + " " + message;

				using (var socket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
					var buffer = Encoding.UTF8.GetBytes (message);
					var target_endpoint = new IPEndPoint (target, Port);
					socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
					socket.SendTo (buffer, target_endpoint);
					socket.Close ();
				}
			} catch (Exception ex) {
				Console.WriteLine ("Send failed: {0}", ex);
			}
		}

		static void Broadcast (string message)
		{
			Send (IPAddress.Broadcast, message);
		}

		static List<IPAddress> GetCurrentIPAddresses ()
		{
			// http://www.grumpydev.com/2010/03/27/locating-network-services-on-the-iphoneipad-with-monotouch/
			IPAddress[] localIPs = null;
			List<IPAddress> result = new List<IPAddress> ();

			try {
				foreach (var iff in NetworkInterface.GetAllNetworkInterfaces ()) {
					foreach (var a in iff.GetIPProperties ().UnicastAddresses)
						result.Add (a.Address);
				}
			} catch (Exception e) {
				Console.WriteLine ("Couldn't iterate over all network interfaces: {0}", e.Message);
			}

			if (result.Count > 0)
				return result;

			// Try with GetHostName, add .local if that fails
			// We need to do this as there seems to be a bug / inconsistency
			// between the simulator and the device itself
			try {
				localIPs = Dns.GetHostAddresses (Dns.GetHostName ());
			} catch (SocketException se) {
				Console.WriteLine (se);
			}
			
			if (localIPs == null) {
				try {
					localIPs = Dns.GetHostAddresses (Dns.GetHostName () + ".local");
				} catch (SocketException se) {
					// fallback to loopback
					Console.WriteLine (se);
					result.Add (IPAddress.Loopback);
					return result;
				}
			}
			
			// Work through the IPs reported and return the first one
			// that is IPv4 and not a loopback
			foreach (IPAddress localIP in localIPs) {
				if ((localIP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) && (!IPAddress.IsLoopback (localIP)))
					result.Add (localIP);
			}

			if (result.Count == 0) {
				// Fallback to loopback if necessary
				result.Add (IPAddress.Loopback);
			}

			return result;
		}

		public static void DiscoverPeers ()
		{
			peers = new Dictionary<string, Peer> ();

			new Thread (Listen)
			{
				IsBackground = true,
			}.Start ();

			new Thread (Publish)
			{
				IsBackground = true,
			}.Start ();
		}
	}
}


