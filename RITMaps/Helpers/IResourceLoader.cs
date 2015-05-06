using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RITMaps
{
	public interface IResourceLoader
	{
		Task<IRITBuilding[]> Load(ResourceFile resource);
	}
}

