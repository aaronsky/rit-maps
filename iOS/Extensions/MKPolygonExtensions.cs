using CoreLocation;
using MapKit;

namespace RITMaps.iOS
{
	public static class MKPolygonExtensions
	{
		public static bool PointInsidePolygon (this MKPolygon polygon, CLLocationCoordinate2D point)
		{
			var mapPoint = MKMapPoint.FromCoordinate (point);
			var polygonView = new MKPolygonRenderer (polygon);
			var polygonViewPoint = polygonView.PointForMapPoint (mapPoint);
			return polygonView.Path.ContainsPoint (polygonViewPoint, false);
		}
	}
}

