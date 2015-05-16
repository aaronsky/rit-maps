using CoreLocation;
using MapKit;
using RITMaps.Models;
using System;

namespace RITMaps.iOS.Extensions
{
    public static class BoundingBoxExtensions
    {
        public static MKMapRect ToMapRect (this BoundingBox box)
        {
            var topLeft = MKMapPoint.FromCoordinate(new CLLocationCoordinate2D(box.x0, box.y0));
            var bottomRight = MKMapPoint.FromCoordinate(new CLLocationCoordinate2D(box.xf, box.yf));
            return new MKMapRect(topLeft.X, bottomRight.Y, Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y));
        }
    }
}