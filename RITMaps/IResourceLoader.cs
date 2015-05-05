using System;
using System.Collections.Generic;

namespace RITMaps
{
	public interface IResourceLoader
	{
		IEnumerable<IRITBuilding> Load(ResourceFile resource);
	}
}

