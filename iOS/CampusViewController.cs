using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MapKit;

namespace RITMaps.iOS
{
	partial class CampusViewController : UIViewController, IMKMapViewDelegate, UIActionSheetDelegate
	{
		BuildingAnnotation CurrentSelection { get; set; }

		public CampusViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Code to start the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start ();
			#endif

			activeMapView.Delegate = this;
			activeMapView.ShowsUserLocation = true;
			activeMapView.SetUserTrackingMode (MKUserTrackingMode.Follow, true);

			activeMapView.AddAnnotations (BuildingManager.Buildings);
			activeMapView.ShowAnnotations (activeMapView.Annotations, true);
			activeMapView.SetRegion (MKCoordinateRegion.FromDistance (CurrentSelection.Coordinate, 0, 0), true);
			activeMapView.SelectAnnotation (CurrentSelection, true);

			activeMapView.ShowsBuildings = true;
			var camera = activeMapView.Camera;
			camera.Pitch = 45;
			activeMapView.SetCamera (camera, true);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		[Export ("mapView:viewForAnnotation:")]
		public MapKit.MKAnnotationView GetViewForAnnotation (MapKit.MKMapView mapView, MapKit.IMKAnnotation annotation)
		{
			throw new NotImplementedException ();
		}

		[Export ("mapView:rendererForOverlay:")]
		public MapKit.MKOverlayRenderer OverlayRenderer (MapKit.MKMapView mapView, MapKit.IMKOverlay overlay)
		{
			throw new NotImplementedException ();
		}

		[Export ("mapView:didSelectAnnotationView:")]
		public void DidSelectAnnotationView (MapKit.MKMapView mapView, MapKit.MKAnnotationView view)
		{
			throw new NotImplementedException ();
		}

		[Export ("mapView:didDeselectAnnotationView:")]
		public void DidDeselectAnnotationView (MapKit.MKMapView mapView, MapKit.MKAnnotationView view)
		{
			throw new NotImplementedException ();
		}

		[Export ("mapView:annotationView:calloutAccessoryControlTapped:")]
		public void CalloutAccessoryControlTapped (MapKit.MKMapView mapView, MapKit.MKAnnotationView view, UIKit.UIControl control)
		{
			throw new NotImplementedException ();
		}

		[Export ("mapViewWillStartLocatingUser:")]
		public void WillStartLocatingUser (MapKit.MKMapView mapView)
		{
			throw new NotImplementedException ();
		}

		[Export ("mapView:didUpdateUserLocation:")]
		public void DidUpdateUserLocation (MapKit.MKMapView mapView, MapKit.MKUserLocation userLocation)
		{
			throw new NotImplementedException ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
		}
	}
}
