// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace RITMaps.iOS
{
	[Register ("CampusViewController")]
	partial class CampusViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MapKit.MKMapView activeMapView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem DirectionsButton { get; set; }

		[Action ("ProcessDirections:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ButtonTapped_ProcessDirections (UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
			if (activeMapView != null) {
				activeMapView.Dispose ();
				activeMapView = null;
			}
			if (DirectionsButton != null) {
				DirectionsButton.Dispose ();
				DirectionsButton = null;
			}
		}
	}
}
