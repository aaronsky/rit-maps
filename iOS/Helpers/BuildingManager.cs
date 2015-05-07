using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RITMaps.iOS
{
	public static class BuildingManager
	{
		public static List<BuildingAnnotation> Buildings { get; set; }
		public static Dictionary<int, string> Tags { get; set; }
		public static IResourceLoader ResourceLoader { get; set; }

		static BuildingManager ()
		{
			ResourceLoader = BuildingFactory.Create ();
			Buildings = new List<BuildingAnnotation> ();
			Tags = new Dictionary<int, string> ();
		}


	}
}

