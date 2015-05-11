using System;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Osm.Interpreter;

namespace RITMaps
{
	public static class RouteHelper
	{
		public static IBasicRouterDataSource<CHEdgeData> Graph { get; set; }

		public static Route Calculate (OsmSharp.Math.Geo.GeoCoordinate startCoord, OsmSharp.Math.Geo.GeoCoordinate endCoord)
		{
			try {
				lock (Graph) {
					var router = Router.CreateCHFrom (Graph, new CHRouter (), new OsmRoutingInterpreter ());

					var start = router.Resolve (
						            Vehicle.Pedestrian,
						            0.5f,
						            startCoord);
					if (start == null) {
						throw new NotSupportedException ("Route could not be calculated from your current location");
					}
					var end = router.Resolve (
						          Vehicle.Pedestrian,
						          0.5f,
						          endCoord);
					if (end == null) {
						throw new NotSupportedException ("Route could not be calculated from your current location");
					}
					var route = router.Calculate (Vehicle.Pedestrian, start, end);
					if (route == null) {
						throw new NotSupportedException ("Route could not be calculated from your current location", new Exception (string.Format ("route:{0},start:({1},{2}),end:({3},{4})", route, start.Location.Latitude, start.Location.Longitude, end.Location.Latitude, end.Location.Longitude)));
					} 
					return route;
				}
			} catch (Exception ex) {
				throw ex;
			}
			return null;
		}
	}
}

