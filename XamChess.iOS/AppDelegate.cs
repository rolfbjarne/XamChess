/*
* AppDelegate.cs
* 
* Authors:
*     Rolf Bjarne Kvinge <rolf@xamarin.com>
* 
* Copyright (C) 2012 Xamarin Inc. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using XamChess.Common;

namespace XamChess.iOS
{
	/// <summary>
	/// The UIApplicationDelegate for the application. This class is responsible for launching the 
	/// User Interface of the application, as well as listening (and optionally responding) to 
	/// application events from iOS.
	/// </summary>
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UIViewController viewController;
		
		// This method is invoked when the application has loaded its UI and is ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Network.DiscoverPeers ();

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			viewController = new GameViewController ();

			window.RootViewController = viewController;

			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
