using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Threading.Tasks;
using System.Linq;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using System.IO;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.Serialization.Sorted;
using OsmSharp.Routing.CH.Serialization;

namespace RITMaps.iOS
{
	partial class LoadDataViewController : UIViewController
	{

		public LoadDataViewController (IntPtr handle) : base (handle)
		{
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			await LoadData ();

			PerformSegue ("loadSucceed", this);
		}

		public async Task LoadData ()
		{
			var loadedMarkers = await BuildingManager.ResourceLoader.Load (ResourceFile.Markers);
			BuildingManager.Buildings.AddRange (loadedMarkers.Cast<BuildingAnnotation> ());
			var loadedPolygons = await BuildingManager.ResourceLoader.Load (ResourceFile.Polygons);
			BuildingManager.Buildings.AddRange (loadedPolygons.Cast<BuildingAnnotation> ());
			var loadedTags = await BuildingManager.ResourceLoader.LoadTags (ResourceFile.Tags);
			foreach (var kvp in loadedTags) {
				if (BuildingManager.Tags.ContainsKey (kvp.Key)) {
					BuildingManager.Tags [kvp.Key] = kvp.Value;
				} else {
					BuildingManager.Tags.Add (kvp.Key, kvp.Value);
				}
			}
			using (var inputStream = 
				new FileInfo(NSBundle.MainBundle.PathForResource(Resources.ResourceFileToFileName(ResourceFile.Routing), "routing")).OpenRead())
			{
				var routingSerializer = new CHEdgeFlatfileSerializer ();
				RouteHelper.Graph = routingSerializer.Deserialize (inputStream);
			}
		}
	}
}
