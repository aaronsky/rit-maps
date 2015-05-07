using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Threading.Tasks;
using System.Linq;

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
			await LoadData();

			PerformSegue ("loadSucceed", this);
		}

		public async Task LoadData()
		{
			var loadedMarkers = await BuildingManager.ResourceLoader.Load (ResourceFile.Markers);
			loadProgressView.SetProgress (0.2f, true);
			loadProgressLabel.Text = "20%";
			BuildingManager.Buildings.AddRange (loadedMarkers.Cast<BuildingAnnotation>());
			loadProgressView.SetProgress (0.3f, true);
			loadProgressLabel.Text = "30%";
			var loadedPolygons = await BuildingManager.ResourceLoader.Load (ResourceFile.Polygons);
			loadProgressView.SetProgress (0.5f, true);
			loadProgressLabel.Text = "50%";
			BuildingManager.Buildings.AddRange (loadedPolygons.Cast<BuildingAnnotation>());
			loadProgressView.SetProgress (0.6f, true);
			loadProgressLabel.Text = "60%";
			var loadedTags = await BuildingManager.ResourceLoader.LoadTags (ResourceFile.Tags);
			loadProgressView.SetProgress (0.8f, true);
			loadProgressLabel.Text = "80%";
			foreach (var kvp in loadedTags) {
				if (BuildingManager.Tags.ContainsKey (kvp.Key)) {
					BuildingManager.Tags [kvp.Key] = kvp.Value;
				} else {
					BuildingManager.Tags.Add (kvp.Key, kvp.Value);
				}
			}
			loadProgressView.SetProgress (1.0f, true);
			loadProgressLabel.Text = "100%";
		}
	}
}
