using System;
using System.Collections.Generic;

namespace RITMaps.iOS
{
	public class ResourceLoader : IResourceLoader
	{
		public IEnumerable<IRITBuilding> Load (ResourceFile resource)
		{
			return new List<BuildingAnnotation> ();
		}
	}
}

