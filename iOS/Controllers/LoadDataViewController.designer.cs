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
	[Register ("LoadDataViewController")]
	partial class LoadDataViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel loadProgressLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIProgressView loadProgressView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (loadProgressLabel != null) {
				loadProgressLabel.Dispose ();
				loadProgressLabel = null;
			}
			if (loadProgressView != null) {
				loadProgressView.Dispose ();
				loadProgressView = null;
			}
		}
	}
}
