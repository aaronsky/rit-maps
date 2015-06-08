using Foundation;
using System;
using UIKit;
using MapKit;
using System.Linq;
using CoreLocation;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoreAnimation;

namespace RITMaps.iOS
{
	partial class CampusViewController : UIViewController, IMKMapViewDelegate
	{
		public static RITBuilding CurrentBuilding { get; set; }

		MKPolyline CurrentRoute { get; set; }

		bool IsAuthorized { get; set; }

		UIPopoverController PopOver { get; set; }

		public CampusViewController (IntPtr handle)
			: base (handle)
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

			CurrentBuilding = BuildingManager.Buildings.Traverse ().FirstOrDefault (b => b.Id == "6"); 

			if (CurrentBuilding != null) {
				activeMapView.SetRegion (MKCoordinateRegion.FromDistance (
					new CLLocationCoordinate2D (CurrentBuilding.Latitude, CurrentBuilding.Longitude), 0, 0), true);
			} else {
				RefreshPolygons ();
			}

			activeMapView.ShowsBuildings = true;
			//var camera = activeMapView.Camera;
			//camera.Pitch = 45;
			//activeMapView.SetCamera (camera, true);
		}

		async Task RequestTrackingAuthorization ()
		{
			var locationManager = new CLLocationManager ();
			await Task.Factory.StartNew (locationManager.RequestWhenInUseAuthorization);
		}

		[Export ("mapView:regionDidChangeAnimated:")]
		public void RegionChanged (MKMapView mapView, bool animated)
		{
			double zoomScale = activeMapView.Bounds.Size.Width / activeMapView.VisibleMapRect.Size.Width;
			var annotations = BuildingManager.ClusteredAnnotations (mapView.VisibleMapRect, zoomScale);
			UpdateMapViewWithAnnotations (annotations);
		}

		public void UpdateMapViewWithAnnotations (BuildingAnnotation[] annotations)
		{
			var before = activeMapView.Annotations.ToList ();
			var after = annotations.Cast<IMKAnnotation> ().ToList ();

			var toKeep = before.ToArray ();
			toKeep = toKeep.Intersect (after).ToArray ();

			var toAdd = after.ToArray ();
			toAdd = toAdd.Except (toKeep).ToArray ();

			var toRemove = before.ToArray ();
			toRemove = toRemove.Except (after).ToArray ();

			activeMapView.AddAnnotations (toAdd);
			activeMapView.RemoveAnnotations (toRemove);
		}

		[Export ("mapView:viewForAnnotation:")]
		public MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			if (IsUserLocationAnnotation (mapView, annotation))
				return null;
			
			string reuseId = @"loc";
			var annotationView = (BuildingAnnotationView)activeMapView.DequeueReusableAnnotation (reuseId);

			if (annotationView == null) {
				annotationView = new BuildingAnnotationView (annotation, reuseId);
			} else {
				annotationView.Annotation = annotation;
			}

			var annotationCluster = annotation as BuildingAnnotationCluster;
			if (annotationCluster != null) {
				annotationView.Count = annotationCluster.Count;
			} else {
				var building = annotation as BuildingAnnotation;
				if (building != null) {
					annotationView.CanShowCallout = true;
					if (building.Building.FullDescription != "No description found") {
						annotationView.RightCalloutAccessoryView = new UIButton (UIButtonType.DetailDisclosure);
					}
				}
			}
			annotationView.LeftCalloutAccessoryView = new UIButton (UIButtonType.ContactAdd);
			return annotationView;

//			var annotationView = (MKPinAnnotationView)activeMapView.DequeueReusableAnnotation ("loc");
//			if (annotationView == null) {
//				annotationView = new MKPinAnnotationView (annotation, "loc");
//				annotationView.CanShowCallout = true;
//			} else {
//				annotationView.Annotation = annotation;
//			}
//
//			var building = annotation as BuildingAnnotation;
//			if (building != null) {
//				/*if (building.Tags.Contains ("Restroom") ||
//				    building.Tags.Contains ("Men's Restroom") ||
//				    building.Tags.Contains ("Women's Restroom") ||
//				    building.Tags.Contains ("Unisex Restroom")) {
//					annotationView.Image = UIImage.FromFile ("restroom_unfilled.png");
//				} else if (building.Tags.Contains ("Bike Rack") ||
//				           building.Tags.Contains ("Motorcycle Parking")) {
//					annotationView.Image = UIImage.FromFile ("bicycle_unfilled.png");
//				} else if (building.Tags.Contains ("Academic Building")) {
//					annotationView.Image = UIImage.FromFile ("misc_map_unfilled.png");
//				} else if (building.Tags.Contains ("Parking") ||
//				           building.Tags.Contains ("General Parking") ||
//				           building.Tags.Contains ("Academic Parking") ||
//				           building.Tags.Contains ("Reserved Parking") ||
//				           building.Tags.Contains ("Residential Parking") ||
//				           building.Tags.Contains ("Short Term Parking") ||
//				           building.Tags.Contains ("Visitor Parking")) {
//					annotationView.Image = UIImage.FromFile ("parking_unfilled.png");
//				} else if (building.Tags.Contains ("Dining Services") ||
//				           building.Tags.Contains ("i_diningservicesplus") ||
//				           building.Tags.Contains ("Restaurants") ||
//				           building.Tags.Contains ("Food")) {
//					annotationView.Image = UIImage.FromFile ("dining_unfilled.png");
//				} else if (building.Tags.Contains ("Residential Building")) {
//					annotationView.Image = UIImage.FromFile ("residential_unfilled.png");
//				} else if (building.Tags.Contains ("Shuttle Stop") ||
//				           building.Tags.Contains ("Bus Stop")) {
//					annotationView.Image = UIImage.FromFile ("bus_unfilled.png");
//				} else if (building.Tags.Contains ("WAL")) {
//					annotationView.Image = UIImage.FromFile ("library_unfilled.png");
//				} else if (building.Tags.Contains ("Student Services") &&
//				           (building.Tags.Contains ("NRH") || building.Tags.Contains ("GVP"))) {
//					annotationView.Image = UIImage.FromFile ("post_office_unfilled.png");
//				} else if (building.Tags.Contains ("Student Services") &&
//				           building.Tags.Contains ("SMT")) {
//					annotationView.Image = UIImage.FromFile ("worship_unfilled.png");
//				} else if (building.Tags.Contains ("ATM")) {
//					annotationView.Image = UIImage.FromFile ("bank_unfilled.png");
//				} else if (building.Tags.Contains ("Retail")) {
//					annotationView.Image = UIImage.FromFile ("barber_unfilled.png");
//				} else {
//					annotationView.Image = UIImage.FromFile ("misc_map_unfilled.png");
//				}
//				*/
//
//				if (building.Building.FullDescription != "No description found") {
//					annotationView.RightCalloutAccessoryView = new UIButton (UIButtonType.DetailDisclosure);
//				}
//			}
//			annotationView.LeftCalloutAccessoryView = new UIButton (UIButtonType.ContactAdd);
//			return annotationView;
		}

		static bool IsUserLocationAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			var usrLoc = ObjCRuntime.Runtime.GetNSObject (annotation.Handle) as MKUserLocation;
			if (usrLoc != null) {
				return usrLoc == mapView.UserLocation;
			}
			return false;
		}

		void AddBounceAnimation (UIView view)
		{
			var bounceAnimation = new CAKeyFrameAnimation () {
				KeyPath = "transform.scale",
				Values = new NSObject[] {
					NSNumber.FromFloat (0.05f), 
					NSNumber.FromFloat (1.1f), 
					NSNumber.FromFloat (0.9f), 
					NSNumber.FromFloat (1)
				},
				Duration = 0.6,
				TimingFunctions = new [] {
					CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut),
					CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut),
					CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut),
					CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut)
				},
				RemovedOnCompletion = false
			};
			view.Layer.AddAnimation (bounceAnimation, "bounce");
		}

		[Export ("mapView:didAddAnnotationViews:")]
		public void DidAddAnnotationViews (MapKit.MKMapView mapView, MapKit.MKAnnotationView[] views)
		{
			foreach (var view in views)
				AddBounceAnimation (view);
		}

		[Export ("mapView:rendererForOverlay:")]
		public MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay)
		{
			if (overlay == null)
				throw new ArgumentNullException ("overlay");
			if (mapView == null)
				throw new ArgumentNullException ("mapView");

			var building = BuildingManager.Buildings.Traverse ().FirstOrDefault (b => {
				var poly = (b.Boundaries as BuildingPolygon)?.Polygon;
				return poly.Equals (overlay);
			});

			if (building != null) {
				var polygon = (building.Boundaries as BuildingPolygon)?.Polygon;
				var view = new MKPolygonRenderer (polygon);
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
			if (building != null && (building as BuildingAnnotationCluster) == null) {
				building.Boundaries.IsSelected = true;
				CurrentBuilding = building.Building;
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
			var building = view.Annotation as BuildingAnnotation;
			if (building != null && CurrentBuilding != null) {
				if (building.Equals (CurrentBuilding)) {
					building.Boundaries.IsSelected = false;
					CurrentBuilding = null;
				}
			}
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
			BuildingViewController buildingDetail = new BuildingViewController (CurrentBuilding);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				PopOver = new UIPopoverController (buildingDetail);
				PopOver.PresentFromRect (view.RightCalloutAccessoryView.Bounds,
					view,
					UIPopoverArrowDirection.Any,
					true);
			} else if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				NavigationController.PushViewController (buildingDetail, true);
			}
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
			if (CurrentBuilding == null)
				return;
			try {
				CurrentRoute = null;
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
			foreach (var building in BuildingManager.Buildings.Traverse()) {
				if (building != null) {
					var poly = (building.Boundaries as BuildingPolygon);
					if (poly != null) {
						building.Boundaries.IsInside = poly.Polygon.PointInsidePolygon (activeMapView.UserLocation.Coordinate);
						if (poly.Path.Length != 0) {
							activeMapView.AddOverlay (poly.Polygon);
						}
					}
				}
			}

		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
		}
	}
}
