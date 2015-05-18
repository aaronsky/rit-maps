using System;
using MapKit;
using UIKit;
using CoreGraphics;

namespace RITMaps.iOS
{
	public class BuildingAnnotationView : MKAnnotationView
	{
		UILabel countLabel;
		int count;
		public int Count {
			get { return count; }
			set {
				count = value;
				SetCount (value);
			}
		}

		public BuildingAnnotationView (IMKAnnotation annotation, string reuseIdentifier) : base (annotation, reuseIdentifier)
		{
			BackgroundColor = UIColor.Clear;

			countLabel = new UILabel (Frame) {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Center,
				ShadowColor = UIColor.FromWhiteAlpha (0.0f, 0.75f),
				ShadowOffset = new CoreGraphics.CGSize (0, -1),
				AdjustsFontSizeToFitWidth = true,
				Lines = 1,
				Font = UIFont.BoldSystemFontOfSize (12),
				BaselineAdjustment = UIBaselineAdjustment.AlignCenters
			};
			AddSubview (countLabel);
			SetCount (1);
		}

		void SetCount (int count)
		{
			var newBounds = new CGRect (0, 0, Math.Round (44 * ScaledValueForValue (count)), Math.Round (44 * ScaledValueForValue (count)));
			Frame = CenterRect (newBounds, Center);

			var newLabelBounds = new CGRect (0, 0, newBounds.Size.Width / 1.3, newBounds.Size.Height / 1.3);
			countLabel.Frame = CenterRect (newLabelBounds, RectCenter (newBounds));
			countLabel.Text = Count.ToString ();

			SetNeedsDisplay ();
		}

		public override void Draw (CGRect rect)
		{
			var context = UIGraphics.GetCurrentContext ();

			context.SetAllowsAntialiasing (true);

			var outerCircleStrokeColor = UIColor.FromWhiteAlpha (0.0f, 0.25f);
			var innerCircleStrokeColor = UIColor.White;
			var innerCircleFillColor = UIColor.FromRGBA (1.0f, 0.37f, 0.16f, 1.0f);

			var circleFrame = rect.Inset (4, 4);

			outerCircleStrokeColor.SetStroke ();
			context.SetLineWidth (5);
			context.StrokeEllipseInRect (circleFrame);

			innerCircleStrokeColor.SetStroke ();
			context.SetLineWidth (4);
			context.StrokeEllipseInRect (circleFrame);

			innerCircleFillColor.SetFill ();
			context.StrokeEllipseInRect (circleFrame);
		}

		CGPoint RectCenter (CGRect rect)
		{
			return new CGPoint (rect.GetMidX (), rect.GetMidY ());
		}

		CGRect CenterRect (CGRect rect, CGPoint center)
		{
			CGRect r = new CGRect (center.X - rect.Size.Width / 2.0,
				           center.Y - rect.Size.Height / 2.0,
				           rect.Size.Width,
				           rect.Size.Height);
			return r;
		}

		double ScaledValueForValue (double value)
		{
			return 1.0 / (1.0 + Math.Exp (-1 * 0.3 * Math.Pow (value, 0.4)));
		}
	}
}

