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

namespace RITMaps.iOS
{
	public class ResourceLoader : IResourceLoader
	{
		public async Task<IEnumerable<IRITBuilding>> Load (ResourceFile resource)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			var json = await LoadJsonFromResource (resource);
			var buildingsJObj = json ["response"] ["docs"];
			BuildingAnnotation[] buildings = buildingsJObj.Select (b => new BuildingAnnotation(
				new CLLocationCoordinate2D((int)b["latitude"], (int)b["longitude"]),
				(string)b["name"] == null ? (string)b["name"] : "No name found",
				(string)b["description"] == null ? (string)b["description"] : "No description found") {
				Id = (string)b["mdo_id"] == null ? (string)b["mdo_id"] : "ID not found",
				Name = (string)b["name"] == null ? (string)b["name"] : "No name found",
				BuildingId = (string)b["building_number"] == null ? (string)b["building_number"] : "Building number not found",
				ShortDescription = (string)b["description"] == null ? (string)b["description"] : "No description found",
				ImageUrl = (string)b["image"] == null ? (string)b["image"] : "Image not found",
				Abbreviation = (string)b["abbreviation"] == null ? (string)b["abbreviation"] : "UNKNOWN",
				History = (string)b["history"] == null ? (string)b["history"] : "No history found",
				FullDescription = (string)b["full_description"] == null ? (string)b["full_description"] : "No description found",
				Tags = b["tag"].ToObject<string[]>() == null ? b["tag"].ToObject<string[]>() : new string[1],
				Boundaries = BuildingPolygon.Create(
					(string)b["polygon_id"], 
					(string)b["path"], 
					b["tag"].ToObject<string[]>() == null ? b["tag"].ToObject<string[]>() : new string[1])
			}).ToArray ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			return buildings;
		}

		public async Task<IDictionary<int, string>> LoadTags(ResourceFile resource)
		{
			if (resource != ResourceFile.Tags)
				return new Dictionary<int, string>();
			var json = await LoadJsonFromResource (resource);
			JArray tagsJObj = (JArray)json ["facet_counts"] ["facet_fields"] ["tag"];
			Dictionary<int, string> tags = new Dictionary<int, string>();
			for (int progress = 0, total = tagsJObj.Count; progress < total; progress++) {
				tags.Add((int)tagsJObj[progress+1], (string)tagsJObj[progress]);
			}
			return tags;
		}

		public async Task<JObject> LoadJsonFromResource(ResourceFile resource) {
			string jsonStr;
			if (resource == ResourceFile.Markers) {
				var path = NSBundle.MainBundle.PathForResource (Resources.ResourceFileToFileName (resource), "js");
				jsonStr = File.ReadAllText (path);
			} else {
				using (var client = new HttpClient ()) {
					var uri = new Uri (Resources.ResourceFileToFileName (resource));
					jsonStr = await client.GetStringAsync (uri);
				}
			}
			return await Task.Run(() => JObject.Parse(jsonStr));
		}
	}
}

