using System;
using System.Collections.Generic;

namespace RITMaps
{
	public interface IResourceLoader
	{
		IEnumerable<RITBuilding> Load(ResourceFile resource);
	}
}

