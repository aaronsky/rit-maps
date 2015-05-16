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
using RITMaps.Helpers;

namespace RITMaps.iOS
{
    partial class CampusViewController : UIViewController, IMKMapViewDelegate
    {
        public static BuildingAnnotation CurrentSelection { get; set; }

        MKPolyline CurrentRoute { get; set; }

        public double ZoomLevel { get; set; }

        bool IsAuthorized { get; set; }

        public CampusViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Task.Run(async () => await RequestTrackingAuthorization());

            activeMapView.Delegate = this;
            activeMapView.ShowsUserLocation = true;
            activeMapView.SetUserTrackingMode(MKUserTrackingMode.Follow, true);
            activeMapView.UserInteractionEnabled = true;

            BuildingManager.Buildings.Traverse((currentNode) =>
            {
                var current = currentNode.Points.FirstOrDefault(b => b.Id == "6");
                if (current == null)
                    return false;
                CurrentSelection = current;
                return true;
            });

            if (CurrentSelection != null)
            {
                activeMapView.SetRegion(MKCoordinateRegion.FromDistance(CurrentSelection.Coordinate, 0, 0), true);
                activeMapView.SelectAnnotation(CurrentSelection, true);
            }
            else
            {
                RefreshPolygons();
            }

            activeMapView.ShowsBuildings = true;
            var camera = activeMapView.Camera;
            camera.Pitch = 45;
            activeMapView.SetCamera(camera, true);
        }

        async Task RequestTrackingAuthorization()
        {
            var locationManager = new CLLocationManager();
            await Task.Factory.StartNew(locationManager.RequestWhenInUseAuthorization);
        }

        [Export("mapView:regionDidChangeAnimated:")]
        public void RegionChanged(MapKit.MKMapView mapView, bool animated)
        {
            Task.Run(() =>
            {
                double zoomScale = activeMapView.Bounds.Size.Width / activeMapView.VisibleMapRect.Size.Width;
                var annotations = BuildingManager.ClusteredAnnotations(mapView.VisibleMapRect, zoomScale);
                UpdateMapViewWithAnnotations(annotations);
            });
        }

        public void UpdateMapViewWithAnnotations(BuildingAnnotation[] annotations)
        {
            var before = activeMapView.Annotations.ToList();
            before.Remove((IMKAnnotation)activeMapView.UserLocation);
            var after = annotations.Cast<IMKAnnotation>().ToList();

            var toKeep = before.ToArray();
            toKeep = toKeep.Intersect(after).ToArray();

            var toAdd = after.ToArray();
            toAdd = toAdd.Except(toKeep).ToArray();

            var toRemove = before.ToArray();
            toRemove = toRemove.Except(after).ToArray();

            Task.Run(() =>
            {
                activeMapView.AddAnnotations(toAdd);
                activeMapView.RemoveAnnotations(toRemove);
            });
        }

        [Export("mapView:viewForAnnotation:")]
        public MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (IsUserLocationAnnotation(mapView, annotation))
                return null;

            var annotationView = activeMapView.DequeueReusableAnnotation("loc");
            if (annotationView == null)
            {
                annotationView = new MKAnnotationView(annotation, "loc");
                annotationView.CanShowCallout = true;
            }
            else
            {
                annotationView.Annotation = annotation;
            }

            var building = annotation as BuildingAnnotation;
            if (building != null)
            {
                if (building.Tags.Contains("Restroom") ||
                    building.Tags.Contains("Men's Restroom") ||
                    building.Tags.Contains("Women's Restroom") ||
                    building.Tags.Contains("Unisex Restroom"))
                {
                    annotationView.Image = UIImage.FromFile("restroom_unfilled.png");
                }
                else if (building.Tags.Contains("Bike Rack") ||
                         building.Tags.Contains("Motorcycle Parking"))
                {
                    annotationView.Image = UIImage.FromFile("bicycle_unfilled.png");
                }
                else if (building.Tags.Contains("Academic Building"))
                {
                    annotationView.Image = UIImage.FromFile("misc_map_unfilled.png");
                }
                else if (building.Tags.Contains("Parking") ||
                         building.Tags.Contains("General Parking") ||
                         building.Tags.Contains("Academic Parking") ||
                         building.Tags.Contains("Reserved Parking") ||
                         building.Tags.Contains("Residential Parking") ||
                         building.Tags.Contains("Short Term Parking") ||
                         building.Tags.Contains("Visitor Parking"))
                {
                    annotationView.Image = UIImage.FromFile("parking_unfilled.png");
                }
                else if (building.Tags.Contains("Dining Services") ||
                         building.Tags.Contains("i_diningservicesplus") ||
                         building.Tags.Contains("Restaurants") ||
                         building.Tags.Contains("Food"))
                {
                    annotationView.Image = UIImage.FromFile("dining_unfilled.png");
                }
                else if (building.Tags.Contains("Residential Building"))
                {
                    annotationView.Image = UIImage.FromFile("residential_unfilled.png");
                }
                else if (building.Tags.Contains("Shuttle Stop") ||
                         building.Tags.Contains("Bus Stop"))
                {
                    annotationView.Image = UIImage.FromFile("bus_unfilled.png");
                }
                else if (building.Tags.Contains("WAL"))
                {
                    annotationView.Image = UIImage.FromFile("library_unfilled.png");
                }
                else if (building.Tags.Contains("Student Services") &&
                         (building.Tags.Contains("NRH") || building.Tags.Contains("GVP")))
                {
                    annotationView.Image = UIImage.FromFile("post_office_unfilled.png");
                }
                else if (building.Tags.Contains("Student Services") &&
                         building.Tags.Contains("SMT"))
                {
                    annotationView.Image = UIImage.FromFile("worship_unfilled.png");
                }
                else if (building.Tags.Contains("ATM"))
                {
                    annotationView.Image = UIImage.FromFile("bank_unfilled.png");
                }
                else if (building.Tags.Contains("Retail"))
                {
                    annotationView.Image = UIImage.FromFile("barber_unfilled.png");
                }
                else
                {
                    annotationView.Image = UIImage.FromFile("misc_map_unfilled.png");
                }

                if (building.FullDescription != "No description found")
                {
                    annotationView.RightCalloutAccessoryView = new UIButton(UIButtonType.DetailDisclosure);
                }
            }
            annotationView.LeftCalloutAccessoryView = new UIButton(UIButtonType.ContactAdd);
            return annotationView;
        }

        static bool IsUserLocationAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            var usrLoc = ObjCRuntime.Runtime.GetNSObject(annotation.Handle) as MKUserLocation;
            if (usrLoc != null)
            {
                return usrLoc == mapView.UserLocation;
            }
            return false;
        }

        [Export("mapView:rendererForOverlay:")]
        public MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");
            if (mapView == null)
                throw new ArgumentNullException("mapView");

            BuildingAnnotation building = null;

            BuildingManager.Buildings.Traverse((currentNode) =>
            {
                var current = currentNode.Points.FirstOrDefault(b => b.Boundaries.Polygon.Equals(overlay));
                if (current == null)
                    return false;
                building = current;
                return true;
            });

            if (building != null)
            {
                var view = new MKPolygonRenderer(building.Boundaries.Polygon);
                view.LineWidth = 1;

                if (building.Boundaries.IsSelected)
                {
                    view.StrokeColor = UIColor.Blue;
                    view.FillColor = UIColor.Blue.ColorWithAlpha((nfloat)0.5);
                }
                else if (building.Boundaries.IsInside)
                {
                    view.StrokeColor = UIColor.Green;
                    view.FillColor = UIColor.Green.ColorWithAlpha((nfloat)0.5);
                }
                else if (building.Boundaries.Tags.Contains("Parking"))
                {
                    view.StrokeColor = UIColor.Gray;
                    view.FillColor = UIColor.Gray.ColorWithAlpha((nfloat)0.5);
                }
                else if (building.Boundaries.Tags.Contains("Academic Building"))
                {
                    view.StrokeColor = UIColor.Orange;
                    view.FillColor = UIColor.Orange.ColorWithAlpha((nfloat)0.5);
                }
                else if (building.Boundaries.Tags.Contains("Residential Building"))
                {
                    view.StrokeColor = UIColor.Brown;
                    view.FillColor = UIColor.Brown.ColorWithAlpha((nfloat)0.5);
                }
                else if (building.Boundaries.Tags.Contains("Building"))
                {
                    view.StrokeColor = UIColor.Yellow;
                    view.FillColor = UIColor.Yellow.ColorWithAlpha((nfloat)0.5);
                }
                return view;
            }
            else if (overlay is MKPolyline)
            {
                var route = new MKPolylineRenderer(CurrentRoute);
                route.StrokeColor = UIColor.Orange;
                route.LineWidth = 5;
                return route;
            }
            return null;
        }

        [Export("mapView:didSelectAnnotationView:")]
        public void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            if (mapView == null)
                throw new ArgumentNullException("mapView");
            if (view == null)
                throw new ArgumentNullException("view");

            activeMapView.SetRegion(new MKCoordinateRegion(view.Annotation.Coordinate, new MKCoordinateSpan(0, 0)), true);
            var building = view.Annotation as BuildingAnnotation;
            if (building != null)
            {
                CurrentSelection = mapView.SelectedAnnotations.FirstOrDefault() as BuildingAnnotation;
                CurrentSelection.Boundaries.IsSelected = true;
                DirectionsButton.Enabled = true;
            }
            RefreshPolygons();
        }

        [Export("mapView:didDeselectAnnotationView:")]
        public void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            if (mapView == null)
                throw new ArgumentNullException("mapView");
            if (view == null)
                throw new ArgumentNullException("view");

            CurrentSelection.Boundaries.IsSelected = false;
            CurrentSelection = null;
            if (CurrentRoute != null)
            {
                activeMapView.RemoveOverlay(CurrentRoute);
                CurrentRoute = null;
            }
            DirectionsButton.Enabled = false;
            RefreshPolygons();
        }

        [Export("mapView:annotationView:calloutAccessoryControlTapped:")]
        public void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
        {

        }

        [Export("mapViewWillStartLocatingUser:")]
        public void WillStartLocatingUser(MKMapView mapView)
        {
            if (mapView == null)
                throw new ArgumentNullException("mapView");
            RefreshPolygons();
        }

        [Export("mapView:didUpdateUserLocation:")]
        public void DidUpdateUserLocation(MKMapView mapView, MKUserLocation userLocation)
        {
            if (userLocation == null)
                throw new ArgumentNullException("userLocation");
            if (mapView == null)
                throw new ArgumentNullException("mapView");
            RefreshPolygons();
        }

        [Export("mapView:didFailToLocateUserWithError:")]
        public void DidFailToLocateUser(MKMapView mapView, NSError error)
        {
            RefreshPolygons();
        }

        partial void ButtonTapped_ProcessDirections(UIBarButtonItem sender)
        {
            if (CurrentSelection == null)
                return;
            try
            {
                var start = new OsmSharp.Math.Geo.GeoCoordinate(activeMapView.UserLocation.Coordinate.Latitude, activeMapView.UserLocation.Coordinate.Longitude);
                var end = new OsmSharp.Math.Geo.GeoCoordinate(CurrentSelection.Coordinate.Latitude, CurrentSelection.Coordinate.Longitude);
                var route = RouteHelper.Calculate(start, end);
                CurrentRoute = MKPolyline.FromCoordinates(route.GetPoints().Select(p => new CLLocationCoordinate2D(p.Longitude, p.Latitude)).ToArray());
                if (CurrentRoute == null)
                {
                    throw new NotSupportedException("Route could not be drawn from the calculated route");
                }
                activeMapView.AddOverlay(CurrentRoute);
            }
            catch (NotSupportedException ex)
            {
                var alert = UIAlertController.Create("Route Failed", ex.Message, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
                {
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                }
                ));
                PresentViewController(alert, true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

        }

        void RefreshPolygons()
        {
            if (activeMapView.Overlays != null)
                activeMapView.RemoveOverlays(activeMapView.Overlays);
            BuildingManager.Buildings.Traverse((currentNode) =>
            {

                foreach (var building in currentNode.Points)
                {
                    building.Boundaries.IsInside = building.Boundaries.Polygon.PointInsidePolygon(activeMapView.UserLocation.Coordinate);
                    if (building.Boundaries.Path.Length != 0)
                    {
                        activeMapView.AddOverlay(building.Boundaries.Polygon);
                    }
                }
                return true;
            });

        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
        }
    }
}
