using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MapKit;
using System.Linq;

namespace RITMaps.iOS
{
	partial class CampusViewController : UIViewController, IMKMapViewDelegate, IUIActionSheetDelegate
	{
		BuildingAnnotation CurrentSelection { get; set; }

		public CampusViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			activeMapView.Delegate = this;
			activeMapView.ShowsUserLocation = true;
			activeMapView.SetUserTrackingMode (MKUserTrackingMode.Follow, true);

			var pins = BuildingManager.Buildings.ToArray();
			activeMapView.AddAnnotations (pins);
			activeMapView.ShowAnnotations (activeMapView.Annotations, true);
			CurrentSelection = BuildingManager.Buildings.FirstOrDefault (b => b.Id == "6");
			if (CurrentSelection != null) {
				activeMapView.SetRegion (MKCoordinateRegion.FromDistance (CurrentSelection.Coordinate, 0, 0), true);
				activeMapView.SelectAnnotation (CurrentSelection, true);
			} else {
				RefreshPolygons ();
			}
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
		public MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			if (annotation == null)
				throw new ArgumentNullException ("annotation");
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			
			if (annotation is MKUserLocation)
				return null;

			var annotationView = new MKPinAnnotationView (annotation, "loc");
			var building = annotation as BuildingAnnotation;
			if (building != null) {
				annotationView.PinColor = MKPinAnnotationColor.Red;
				if (building.FullDescription != "No description found") {
					annotationView.RightCalloutAccessoryView = new UIButton (UIButtonType.DetailDisclosure);
				}
			}
			annotationView.CanShowCallout = true;
			return annotationView;
		}

		[Export ("mapView:rendererForOverlay:")]
		public MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay)
		{
			if (overlay == null)
				throw new ArgumentNullException ("overlay");
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			var building = BuildingManager.Buildings.FirstOrDefault(b => b.Boundaries.Polygon.Equals(overlay));
			if (building != null) {
				var view = new MKPolygonRenderer(building.Boundaries.Polygon);
				view.LineWidth = 1;

				if (building.Boundaries.IsSelected) {
					view.StrokeColor = UIColor.Blue;
					view.FillColor = UIColor.Blue.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.IsInside) {
					view.StrokeColor = UIColor.Green;
					view.FillColor = UIColor.Green.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains("Parking")) {
					view.StrokeColor = UIColor.Gray;
					view.FillColor = UIColor.Gray.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains("Academic Building")) {
					view.StrokeColor = UIColor.Orange;
					view.FillColor = UIColor.Orange.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains("Residential Building")) {
					view.StrokeColor = UIColor.Brown;
					view.FillColor = UIColor.Brown.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains("Building")) {
					view.StrokeColor = UIColor.Yellow;
					view.FillColor = UIColor.Yellow.ColorWithAlpha ((nfloat)0.5);
				}
				return view;
			}
			/*
			else if (overlay is MKPolyline) {
				var route = new MKPolylineRenderer (routeDetails.polyline);
				route.StrokeColor = UIColor.Orange;
				route.LineWidth = 5;
				return route;
			}
			*/
			return null;
		}

		[Export ("mapView:didSelectAnnotationView:")]
		public void DidSelectAnnotationView (MKMapView mapView, MKAnnotationView view)
		{
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			if (view == null)
				throw new ArgumentNullException ("view");

			activeMapView.SetRegion (new MKCoordinateRegion (view.Annotation.Coordinate, new MKCoordinateSpan (0, 0)), true);
			var building = view.Annotation as BuildingAnnotation;
			if (building != null) {
				CurrentSelection = mapView.SelectedAnnotations.FirstOrDefault () as BuildingAnnotation;
				CurrentSelection.Boundaries.IsSelected = true;
			}
			RefreshPolygons ();
		}

		[Export ("mapView:didDeselectAnnotationView:")]
		public void DidDeselectAnnotationView (MKMapView mapView, MKAnnotationView view)
		{
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			if (view == null)
				throw new ArgumentNullException ("view");
			
			CurrentSelection.Boundaries.IsSelected = false;
			CurrentSelection = null;
			RefreshPolygons ();
		}

		[Export ("mapView:annotationView:calloutAccessoryControlTapped:")]
		public void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
		{
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			if (view == null)
				throw new ArgumentNullException ("view");
			if (control == null)
				throw new ArgumentNullException ("control");
			//throw new NotImplementedException ();
			RefreshPolygons();
		}

		[Export ("mapViewWillStartLocatingUser:")]
		public void WillStartLocatingUser (MKMapView mapView)
		{
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			RefreshPolygons ();
		}

		[Export ("mapView:didUpdateUserLocation:")]
		public void DidUpdateUserLocation (MKMapView mapView, MKUserLocation userLocation)
		{
			if (userLocation == null)
				throw new ArgumentNullException ("userLocation");
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			RefreshPolygons ();
		}

		[Export ("mapView:didFailToLocateUserWithError:")]
		public void DidFailToLocateUser (MKMapView mapView, NSError error)
		{
			RefreshPolygons();
		}

		void RefreshPolygons() 
		{
			if (activeMapView.Overlays != null)
				activeMapView.RemoveOverlays (activeMapView.Overlays);
			foreach (var building in BuildingManager.Buildings) {
				building.Boundaries.PointInsidePolygon (activeMapView.UserLocation.Coordinate);
				if (building.Boundaries.Path.Length != 0) {
					activeMapView.AddOverlay (building.Boundaries.Polygon);
				}
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
		}
	}
}
