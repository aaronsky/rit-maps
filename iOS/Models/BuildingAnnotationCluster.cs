using CoreLocation;
using MapKit;
using RITMaps.Helpers;
using RITMaps.iOS.Extensions;
using RITMaps.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RITMaps.iOS
{
	public class BuildingAnnotationCluster : BuildingAnnotation
	{
		public BuildingAnnotationCluster(CLLocationCoordinate2D coord, IList<RITBuilding> buildings) : base (coord)
		{
			var annotations = new List<BuildingAnnotation> ();
			foreach (var building in buildings) {
				annotations.Add (new BuildingAnnotation (building));
			}
			Annotations = annotations.ToArray();
		}

		public BuildingAnnotation[] Annotations { get; set; }

		public int Count { get { return Annotations.Length; } }

	}

}

