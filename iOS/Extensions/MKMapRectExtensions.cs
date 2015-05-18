using MapKit;
using RITMaps.Models;

namespace RITMaps.iOS.Extensions
{
	public static class MKMapRectExtensions
	{
		public static BoundingBox ToBoundingBox (this MKMapRect rect)
		{
			var topLeft = MKMapPoint.ToCoordinate (rect.Origin);
			var bottomRight = MKMapPoint.ToCoordinate (new MKMapPoint (rect.MaxX, rect.MaxY));
			var minLat = bottomRight.Latitude;
			var maxLat = topLeft.Latitude;
			var minLong = topLeft.Longitude;
			var maxLong = bottomRight.Longitude;
			return new BoundingBox (minLong, maxLong, minLat, maxLat);
		}
	}
}