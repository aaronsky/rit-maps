using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using RITMaps.Models;
using RITMaps.Helpers;

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
			var dataList = new List<RITBuilding> ();
			var loadedMarkers = await BuildingManager.ResourceLoader.Load (ResourceFile.Markers);
			dataList.AddRange (loadedMarkers);
			var loadedPolygons = await BuildingManager.ResourceLoader.Load (ResourceFile.Polygons);
			dataList.AddRange (loadedPolygons);
			var world = new BoundingBox (-78, 43, -77, 44);
			BuildingManager.Buildings = QuadTree<RITBuilding>.Create (dataList.ToArray (), world, 4);

			var loadedTags = await BuildingManager.ResourceLoader.LoadTags (ResourceFile.Tags);
			foreach (var kvp in loadedTags) {
				if (BuildingManager.Tags.ContainsKey (kvp.Key)) {
					BuildingManager.Tags [kvp.Key] = kvp.Value;
				} else {
					BuildingManager.Tags.Add (kvp.Key, kvp.Value);
				}
			}
			using (var inputStream = 
				       new FileInfo (NSBundle.MainBundle.PathForResource (Resources.ResourceFileToFileName (ResourceFile.Routing), "routing")).OpenRead ()) {

			}
		}
	}
}
