using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MapKit;
using System.Linq;
using CoreLocation;
using System.Threading.Tasks;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Osm.Interpreter;
using System.Collections.Generic;

namespace RITMaps.iOS
{
	partial class CampusViewController : UIViewController, IMKMapViewDelegate
	{
		public static BuildingAnnotation CurrentSelection { get; set; }

		MKPolyline CurrentRoute { get; set; }

		public double ZoomLevel {get;set;}

		bool IsAuthorized { get; set; }

		public CampusViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Task.Run (async () => await RequestTrackingAuthorization ());

			activeMapView.Delegate = this;
			activeMapView.ShowsUserLocation = true;
			activeMapView.SetUserTrackingMode (MKUserTrackingMode.Follow, true);
			activeMapView.UserInteractionEnabled = true;

			activeMapView.AddAnnotations (BuildingManager.Buildings.Where (p => p.Boundaries.Path.Length != 0).ToArray ());
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

		async Task RequestTrackingAuthorization ()
		{
			var locationManager = new CLLocationManager ();
			await Task.Factory.StartNew (locationManager.RequestWhenInUseAuthorization);
		}

		public void FilterAnnotations (BuildingAnnotation[] places)
		{
			var latDelta = activeMapView.Region.Span.LatitudeDelta;
			var longDelta = activeMapView.Region.Span.LongitudeDelta;

			var buildingsToDisplay = new List<MKAnnotation> ();
			for (int i = 0; i < places.Length; i++)
			{
				var loc = places[i];
				var latitude = loc.Coordinate.Latitude;
				var longitude = loc.Coordinate.Longitude;
				var found = false;
				foreach (var temp in buildingsToDisplay) {
					if (Math.Abs(temp.Coordinate.Latitude - latitude) < latDelta &&
						Math.Abs(temp.Coordinate.Longitude - longitude) < longDelta)
					{
						activeMapView.RemoveAnnotation(loc);
						found = true;
						break;
					}
				}
				if (!found) {
					buildingsToDisplay.Add(loc);
					activeMapView.AddAnnotation(loc);
				}
			}
		}

		[Export ("mapView:regionDidChangeAnimated:")]
		public void RegionChanged (MapKit.MKMapView mapView, bool animated)
		{
			/*if (ZoomLevel != activeMapView.Region.Span.LongitudeDelta) {
				FilterAnnotations (BuildingManager.Buildings.Where (p => p.Boundaries.Path.Length == 0).ToArray ());
				ZoomLevel = mapView.Region.Span.LongitudeDelta;
			}*/
		}

		[Export ("mapView:viewForAnnotation:")]
		public MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{			
			if (IsUserLocationAnnotation (mapView, annotation))
				return null;
			
			var annotationView = (MKPinAnnotationView)activeMapView.DequeueReusableAnnotation ("loc");
			if (annotationView == null) {
				annotationView = new MKPinAnnotationView (annotation, "loc");
				annotationView.CanShowCallout = true;
			} else {
				annotationView.Annotation = annotation;
			}

			var building = annotation as BuildingAnnotation;
			if (building != null) {
				/*
				//annotationView.Image = UIImage.FromBundle (string.Empty);
				if (building.Tags.Contains ("Restroom") ||
				    building.Tags.Contains ("Men's Restroom") ||
				    building.Tags.Contains ("Women's Restroom") ||
				    building.Tags.Contains ("Unisex Restroom")) {
					//annotationView.image = [UIImage imageNamed:@"toilets-map"];
				} else if (building.Tags.Contains ("Bike Rack") ||
				           building.Tags.Contains ("Motorcycle Parking")) {
					//annotationView.image = [UIImage imageNamed:@"bicycle-map"];
				} else if (building.Tags.Contains ("Academic Building")) {
					//annotationView.image = [UIImage imageNamed:@"college-map"];
				} else if (building.Tags.Contains ("Parking") ||
				           building.Tags.Contains ("General Parking") ||
				           building.Tags.Contains ("Academic Parking") ||
				           building.Tags.Contains ("Reserved Parking") ||
				           building.Tags.Contains ("Residential Parking") ||
				           building.Tags.Contains ("Short Term Parking") ||
				           building.Tags.Contains ("Visitor Parking")) {
					//annotationView.image = [UIImage imageNamed:@"parking-map"];
				} else if (building.Tags.Contains ("Dining Services") ||
				           building.Tags.Contains ("i_diningservicesplus") ||
				           building.Tags.Contains ("Restaurants") ||
				           building.Tags.Contains ("Food")) {
					//annotationView.image = [UIImage imageNamed:@"fast-food-map"];
				} else if (building.Tags.Contains ("Residential Building")) {
					//annotationView.image = [UIImage imageNamed:@"village-map"];
				} else if (building.Tags.Contains ("Shuttle Stop") ||
				           building.Tags.Contains ("Bus Stop")) {
					//annotationView.image = [UIImage imageNamed:@"bus-map"];
				} else if (building.Tags.Contains ("WAL")) {
					//annotationView.image = [UIImage imageNamed:@"library-map"];
				} else if (building.Tags.Contains ("Student Services") &&
				           (building.Tags.Contains ("NRH") || building.Tags.Contains ("GVP"))) {
					//annotationView.image = [UIImage imageNamed:@"post-map"];
				} else if (building.Tags.Contains ("Student Services") &&
				           building.Tags.Contains ("SMT")) {
					//annotationView.image = [UIImage imageNamed:@"worship-map"];
				} else if (building.Tags.Contains ("ATM")) {
					//annotationView.image = [UIImage imageNamed:@"atm-map"];
				} else if (building.Tags.Contains ("Retail")) {
					//annotationView.image = [UIImage imageNamed:@"hairdresser-map"];
				} else if (building.Tags.Contains ("Building") && building.Id == "57") {
					//annotationView.image = [UIImage imageNamed:@"misc-map"]; //Temporary for pitch
				} else {
					annotationView.UserInteractionEnabled = false;
				}
				*/
				annotationView.PinColor = MKPinAnnotationColor.Red;

				if (building.FullDescription != "No description found") {
					annotationView.RightCalloutAccessoryView = new UIButton (UIButtonType.DetailDisclosure);
				}
			}
			annotationView.LeftCalloutAccessoryView = new UIButton (UIButtonType.ContactAdd);
			return annotationView;
		}

		static bool IsUserLocationAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			var usrLoc = ObjCRuntime.Runtime.GetNSObject (annotation.Handle) as MKUserLocation;
			if (usrLoc != null) {
				return usrLoc == mapView.UserLocation;
			}
			return false;
		}

		[Export ("mapView:rendererForOverlay:")]
		public MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay)
		{
			if (overlay == null)
				throw new ArgumentNullException ("overlay");
			if (mapView == null)
				throw new ArgumentNullException ("mapView");
			var building = BuildingManager.Buildings.FirstOrDefault (b => b.Boundaries.Polygon.Equals (overlay));
			if (building != null) {
				var view = new MKPolygonRenderer (building.Boundaries.Polygon);
				view.LineWidth = 1;

				if (building.Boundaries.IsSelected) {
					view.StrokeColor = UIColor.Blue;
					view.FillColor = UIColor.Blue.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.IsInside) {
					view.StrokeColor = UIColor.Green;
					view.FillColor = UIColor.Green.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains ("Parking")) {
					view.StrokeColor = UIColor.Gray;
					view.FillColor = UIColor.Gray.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains ("Academic Building")) {
					view.StrokeColor = UIColor.Orange;
					view.FillColor = UIColor.Orange.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains ("Residential Building")) {
					view.StrokeColor = UIColor.Brown;
					view.FillColor = UIColor.Brown.ColorWithAlpha ((nfloat)0.5);
				} else if (building.Boundaries.Tags.Contains ("Building")) {
					view.StrokeColor = UIColor.Yellow;
					view.FillColor = UIColor.Yellow.ColorWithAlpha ((nfloat)0.5);
				}
				return view;
			} else if (overlay is MKPolyline) {
				var route = new MKPolylineRenderer (CurrentRoute);
				route.StrokeColor = UIColor.Orange;
				route.LineWidth = 5;
				return route;
			}
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
				DirectionsButton.Enabled = true;
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
			if (CurrentRoute != null) {
				activeMapView.RemoveOverlay (CurrentRoute);
				CurrentRoute = null;
			}
			DirectionsButton.Enabled = false;
			RefreshPolygons ();
		}

		[Export ("mapView:annotationView:calloutAccessoryControlTapped:")]
		public void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
		{
			
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
			RefreshPolygons ();
		}

		partial void ButtonTapped_ProcessDirections (UIBarButtonItem sender)
		{
			if (CurrentSelection == null)
				return;
			try {
				var start = new OsmSharp.Math.Geo.GeoCoordinate(activeMapView.UserLocation.Coordinate.Latitude,activeMapView.UserLocation.Coordinate.Longitude);
				var end = new OsmSharp.Math.Geo.GeoCoordinate(CurrentSelection.Coordinate.Latitude, CurrentSelection.Coordinate.Longitude);
				var route = RouteHelper.Calculate(start, end);
				CurrentRoute = MKPolyline.FromCoordinates (route.GetPoints ().Select (p => new CLLocationCoordinate2D (p.Longitude, p.Latitude)).ToArray ());
				if (CurrentRoute == null) {
					throw new NotSupportedException ("Route could not be drawn from the calculated route");
				}
				activeMapView.AddOverlay (CurrentRoute);
			} catch (NotSupportedException ex) {
				var alert = UIAlertController.Create ("Route Failed", ex.Message, UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (action) => {
					if (ex.InnerException != null)
						Console.WriteLine (ex.InnerException.Message);
				}
				));
				PresentViewController (alert, true, null);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				return;
			}
			
		}

		void RefreshPolygons ()
		{
			if (activeMapView.Overlays != null)
				activeMapView.RemoveOverlays (activeMapView.Overlays);
			foreach (var building in BuildingManager.Buildings) {
				building.Boundaries.IsInside = building.Boundaries.Polygon.PointInsidePolygon (activeMapView.UserLocation.Coordinate);
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
