//
// PainterView.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using Painter.Abstractions;
using UIKit;

namespace Painter.iOS
{
	[Register("PainterView")]
	[DesignTimeVisible(true)]
	public class PainterView : UIView
	{
		//Public UI
		public UIColor StrokeColor { get; set; }

		//Private UI
		private UIBezierPath CurrentPath { get; set; }
		private List<Stroke> Strokes { get; set; }
		private Stroke CurrentStroke { get; set; }
		private UIImageView ImageView { get; set; }

		//Constructors
		public PainterView()
		{
			Initialize();
		}

		public PainterView(NSCoder coder) : base (coder)
		{
			Initialize();
		}

		public PainterView(IntPtr ptr) : base (ptr)
		{
			Initialize();
		}

		public PainterView(CGRect frame)
		{
			Frame = frame;
			Initialize();
		}


		//Initialize
		private void Initialize()
		{
			Strokes = new List<Stroke>();

			StrokeColor = UIColor.Blue;

			ImageView = new UIImageView();
			AddSubview(ImageView);
		}


		//Draw the paths
		private void drawPath()
		{
			UIGraphics.BeginImageContextWithOptions(Frame.Size, false, UIScreen.MainScreen.Scale);//Support Retina when drawing

			BackgroundColor = UIColor.White;
			Layer.BorderColor = UIColor.Black.CGColor;

			CGContext context = UIGraphics.GetCurrentContext();
			context.SetLineCap(CGLineCap.Butt);//TODO decide to make this an abstract (requires support on Android and WP)
			context.SetLineJoin(CGLineJoin.Round);//TODO decide to make this an abstract (requires support on Android and WP)

			foreach (var stroke in Strokes)
			{
				if (stroke.Points.Count == 0)
					continue;

				context.SetLineWidth(stroke.Thickness);
				context.SetStrokeColor(new UIColor((float)stroke.StrokeColor.R, (float)stroke.StrokeColor.G, (float)stroke.StrokeColor.B, (float)stroke.StrokeColor.A).CGColor);

				var p = UIBezierPath.Create();
				p.MoveTo(new CGPoint(stroke.Points[0].X, stroke.Points[0].Y));
				for (int i = 1; i < stroke.Points.Count; i++)
					p.AddLineTo(new CGPoint(stroke.Points[i].X, stroke.Points[i].Y));

				if (stroke.ClosePath)
					p.AddLineTo(new CGPoint(stroke.Points[0].X, stroke.Points[0].Y));

				context.AddPath(p.CGPath);
				context.StrokePath();
			}

			UIImage image = UIGraphics.GetImageFromCurrentImageContext();

			UIGraphics.EndImageContext();

			ImageView.Image = image;
		}


		//Drawing
		public override void Draw(CGRect rect)
		{
			if (CurrentPath != null && !CurrentPath.Empty)
			{
				StrokeColor.SetStroke();
				CurrentPath.Stroke();
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			nfloat r, g, b, a;
			StrokeColor.GetRGBA(out r, out g, out b, out a);

			CurrentStroke = new Stroke()
			{
				StrokeColor = new Color(r, g, b, a)
			};

			CurrentPath = UIBezierPath.Create();
			CurrentPath.LineWidth = 1.0f;
			CurrentPath.LineCapStyle = CGLineCap.Round;
			CurrentPath.LineJoinStyle = CGLineJoin.Bevel;

			var loc = (touches.AnyObject as UITouch).GetPreciseLocation(this);
			CurrentPath.MoveTo(loc);
			CurrentStroke.Points.Add(new Point(loc.X, loc.Y));
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			var loc = (touches.AnyObject as UITouch).GetPreciseLocation(this);
			CurrentPath.AddLineTo(loc);
			CurrentStroke.Points.Add(new Point(loc.X, loc.Y));

			SetNeedsDisplay();
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			var loc = (touches.AnyObject as UITouch).GetPreciseLocation(this);
			CurrentStroke.Points.Add(new Point(loc.X, loc.Y));

			Strokes.Add(CurrentStroke);
			drawPath();

			CurrentPath = null;
			SetNeedsDisplay();
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
		}

		//Layout
		public override void LayoutSubviews()
		{
			var w = Frame.Width;
			var h = Frame.Height;

			ImageView.Frame = Frame;
		}
	}
}
