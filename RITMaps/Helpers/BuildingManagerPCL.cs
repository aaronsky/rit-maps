using System;
using System.Linq;
using System.Collections.Generic;

namespace RITMaps
{
	public abstract class BuildingManagerPCL
	{
		public static IResourceLoader ResourceLoader { get; protected set; }

		protected BuildingManagerPCL ()
		{
			ResourceLoader = BuildingFactory.Create ();
		}
	}
}

