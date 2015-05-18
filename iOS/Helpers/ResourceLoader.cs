#define LOCAL_ONLY

using System;
using System.Collections.Generic;
using UIKit;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Foundation;
using CoreLocation;
using System.Net.Http;
using System.Threading.Tasks;
using RITMaps.Helpers;

namespace RITMaps.iOS
{
	public class ResourceLoader : IResourceLoader
	{
		public ResourceLoader ()
		{
			Console.WriteLine ("ResourceLoader iOS initialized");
		}

		public async Task<IEnumerable<RITBuilding>> Load (ResourceFile resource)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			var json = await LoadJsonFromResource (resource);
			var buildingsJObj = json ["response"] ["docs"];
			var buildings = buildingsJObj.Select (b => new RITBuilding {
				Id = (string)b ["mdo_id"] ?? "ID not found",
				Name = (string)b ["name"] ?? "No name found",
				BuildingId = (string)b ["building_number"] ?? "Building number not found",
				ShortDescription = (string)b ["description"] ?? "No description found",
				ImageUrl = (string)b ["image"] ?? "Image not found",
				Latitude = (double)b ["latitude"],
				Longitude = (double)b ["longitude"],
				Abbreviation = (string)b ["abbreviation"] ?? "UNKNOWN",
				History = (string)b ["history"] ?? "No history found",
				FullDescription = (string)b ["full_description"] ?? "No description found",
				Tags = b ["tag"].ToObject<string[]> () ?? new string[1],
				Boundaries = BuildingPolygon.Create(
					(string)b ["polygon_id"], 
					(string)b ["path"], 
					b ["tag"].ToObject<string[]> ())
			});
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			return buildings;
		}

		public async Task<IDictionary<int, string>> LoadTags (ResourceFile resource)
		{
			if (resource != ResourceFile.Tags)
				return new Dictionary<int, string> ();
			var json = await LoadJsonFromResource (resource);
			JArray tagsJObj = (JArray)json ["facet_counts"] ["facet_fields"] ["tag"];
			Dictionary<int, string> tags = new Dictionary<int, string> ();
			for (int progress = 0, total = tagsJObj.Count; progress < total; progress += 2) {
				var kvp = new KeyValuePair<int, string> ((int)tagsJObj [progress + 1], (string)tagsJObj [progress]);
				if (!tags.ContainsKey (kvp.Key)) {
					tags.Add (kvp.Key, kvp.Value);
				}
			}
			return tags;
		}

		public async Task<JObject> LoadJsonFromResource (ResourceFile resource)
		{
			string jsonStr;
			#if LOCAL_ONLY
			var path = NSBundle.MainBundle.PathForResource (Resources.ResourceFileToFileName (resource), "js");
			jsonStr = File.ReadAllText (path);
			#else
			if (resource == ResourceFile.Markers) {
				var path = NSBundle.MainBundle.PathForResource (Resources.ResourceFileToFileName (resource), "js");
				jsonStr = File.ReadAllText (path);
			} else {
				using (var client = new HttpClient ()) {
					var uri = new Uri (Resources.ResourceFileToFileName (resource));
					jsonStr = await client.GetStringAsync (uri);
				}
			}
			#endif
			return await Task.Run (() => JObject.Parse (jsonStr));
		}
	}
}

