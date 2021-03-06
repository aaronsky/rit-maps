﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RITMaps.Helpers
{
	public interface IResourceLoader
	{
		Task<IEnumerable<RITBuilding>> Load(ResourceFile resource);
		Task<IDictionary<int, string>> LoadTags (ResourceFile resource);
		Task<JObject> LoadJsonFromResource(ResourceFile resource);
		//Stream LoadStreamFromResource(ResourceFile resource);
	}
}

