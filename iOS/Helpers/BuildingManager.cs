using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RITMaps.iOS
{
	public class BuildingManager : BuildingManagerBase
	{
		public static List<BuildingAnnotation> Buildings { get; set; }
		public static Dictionary<int, string> Tags { get; set; }
		static BuildingManager ()
		{
			ResourceLoader = BuildingFactory.Create ();
			Buildings = new List<BuildingAnnotation> ();
		}

		public static async void LoadData()
		{
			var loadedMarkers = ResourceLoader.Load (ResourceFile.Markers);
			Buildings.AddRange ((await loadedMarkers).Cast<BuildingAnnotation>());
			var loadedPolygons = ResourceLoader.Load (ResourceFile.Polygons);
			Buildings.AddRange ((await loadedPolygons).Cast<BuildingAnnotation>());
			var loadedTags = await ResourceLoader.LoadTags (ResourceFile.Tags);
			foreach (var kvp in loadedTags) {
				if (Tags.ContainsKey (kvp.Key)) {
					Tags [kvp.Key] = kvp.Value;
				} else {
					Tags.Add (kvp.Key, kvp.Value);
				}
			}
		}
	}
}

