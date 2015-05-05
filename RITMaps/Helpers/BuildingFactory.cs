using System;

namespace RITMaps
{
	public static class BuildingFactory
	{
		public static Func<IResourceLoader> Create { get; set; }
	}
}

