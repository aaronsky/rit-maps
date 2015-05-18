using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace RITMaps.iOS
{
	partial class BuildingViewController : UITableViewController
	{
		public RITBuilding Building { get; set; }

		public BuildingViewController (RITBuilding building) : base (UITableViewStyle.Grouped)
		{
			Building = building;
		}
	}
}
