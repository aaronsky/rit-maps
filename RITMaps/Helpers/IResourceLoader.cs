using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RITMaps
{
	public interface IResourceLoader
	{
		IRITBuilding[] Load(ResourceFile resource);
	}
}

