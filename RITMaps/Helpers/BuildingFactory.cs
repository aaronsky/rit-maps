using System;

namespace RITMaps.Helpers
{
	public static class BuildingFactory
	{
		public static Func<IResourceLoader> Create { get; set; }
	}
}

