using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RITMaps.Models
{
    public class BoundingBox
    {
        public double x0;
        public double y0;
        public double xf;
        public double yf;
        public BoundingBox(double x0, double y0, double xf, double yf)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.xf = xf;
            this.yf = yf;
        }

        public bool Contains(IRITBuilding data)
        {
            var containsX = x0 <= data.Longitude && data.Longitude <= xf;
            var containsY = y0 <= data.Latitude && data.Latitude <= yf;
            return containsX && containsY;
        }

        public bool Intersects(BoundingBox other)
        {
            return (x0 <= other.xf && xf >= other.x0 && y0 <= other.yf && yf >= other.y0);
        }

        public static bool Intersects(BoundingBox b1, BoundingBox b2)
        {
            return b1.Intersects(b2);
        }
    }
}
