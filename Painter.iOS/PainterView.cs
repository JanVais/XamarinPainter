//
// PainterView.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Painter.Abstractions;
using UIKit;

namespace Painter.iOS
{
	class BezierView : UIView
	{
		public UIBezierPath CurrentPath { get; set; }
		private CAShapeLayer shapeLayer;
		private Color currentColor;

		public BezierView()
		{
			shapeLayer = (CAShapeLayer)CAShapeLayer.Create();
			shapeLayer.FillColor = new UIColor(0, 0, 0, 0).CGColor;
			Layer.AddSublayer(shapeLayer);
		}

		public void Stroke()
		{
			if (currentColor == null || CurrentPath == null)
				return;

			UIColor c = new UIColor((nfloat)currentColor.R, (nfloat)currentColor.G, (nfloat)currentColor.B, (nfloat)currentColor.A);

			shapeLayer.StrokeColor = c.CGColor;
			shapeLayer.LineWidth = CurrentPath.LineWidth;
			shapeLayer.Path = CurrentPath.CGPath;
		}

		public void CreateNewPath(Stroke stroke)
		{
			CurrentPath = UIBezierPath.Create();

			CurrentPath.LineWidth = (float)stroke.Thickness;
			CurrentPath.LineCapStyle = CGLineCap.Round;
			CurrentPath.LineJoinStyle = CGLineJoin.Bevel;

			currentColor = stroke.StrokeColor;
		}

		public void MoveTo(CGPoint p)
		{
			CurrentPath.MoveTo(p);
		}

		public void AddLineTo(CGPoint p)
		{
			CurrentPath.AddLineTo(p);
		}

		public void Clear()
		{
			CurrentPath = null;
			shapeLayer.Path = null;
		}
	}

	[Register("PainterView")]
	[DesignTimeVisible(true)]
	public class PainterView : UIView
    {
        //Public UI
        public Color StrokeColor { get; set; }
        public EventHandler FinishedStrokeEvent { get; set; }
        public double StrokeThickness { get; set; } = 1.0;
        private List<Stroke> _strokes;
        public List<Stroke> Strokes
        {
            get
            {
                return _strokes;
            }
            set
            {
                //TODO update view
                _strokes = value;
            }
        }

        //Private UI
        private Stroke CurrentStroke { get; set; }
		private UIImageView ImageView { get; set; }
		private UIImageView BackgroundImage { get; set; }
		private BezierView CurrentPathView { get; set; }

		//Constructors
		public PainterView()
		{
			Initialize();
		}

		public PainterView(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public PainterView(IntPtr ptr) : base(ptr)
		{
			Initialize();
		}

		public PainterView(CGRect frame)
		{
			Frame = frame;
			Initialize();
		}

		public PainterView(string imageUri, bool inResources = true)
		{
			Initialize();
		}

		public void LoadImage(string imageUri, bool inResources = true)
		{
			BackgroundImage = new UIImageView();
			BackgroundImage.Image = UIImage.FromFile(imageUri);
			AddSubview(BackgroundImage);
			SendSubviewToBack(BackgroundImage);
		}

		//Initialize
		private void Initialize()
		{
			Strokes = new List<Stroke>();

			StrokeColor = new Color(0, 0, 1, 1);

			Opaque = false;
			ImageView = new UIImageView();
			ImageView.Opaque = false;
			AddSubview(ImageView);
			SendSubviewToBack(ImageView);

			CurrentPathView = new BezierView();
			CurrentPathView.Opaque = false;
			AddSubview(CurrentPathView);
			BringSubviewToFront(CurrentPathView);

			NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, HandleOrientationChange);
		}

		~PainterView()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
		}

		void HandleOrientationChange(NSNotification obj)
		{
			drawPath();
		}

		//Exports
		public string GetJson()
		{
			return JsonConvert.SerializeObject(Strokes);
		}

		//Imports
		public void LoadJson(string json)
		{
			try
			{
				Strokes = JsonConvert.DeserializeObject<List<Stroke>>(json);
				drawPath();
			}
			catch (Exception e)
			{
				//Invalid json
			}
		}

		public void Clear()
		{
			Strokes.Clear();
			CurrentPathView.Clear();
			drawPath();
		}

		//Draw the paths to the UIImage view
		private void drawPath()
		{
			UIGraphics.BeginImageContextWithOptions(Frame.Size, false, UIScreen.MainScreen.Scale);//Support Retina when drawing
			CGContext context = UIGraphics.GetCurrentContext();

			context.SetLineCap(CGLineCap.Round);//TODO decide to make this an abstract (requires support on Android and WP)
			context.SetLineJoin(CGLineJoin.Round);//TODO decide to make this an abstract (requires support on Android and WP)

			foreach (var stroke in Strokes)
			{
				if (stroke.Points.Count == 0)
					continue;

				context.SetLineWidth((float)stroke.Thickness);
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

			//Get the image
			UIImage image = UIGraphics.GetImageFromCurrentImageContext();

			//End the CGContext
			UIGraphics.EndImageContext();

			//Display the image
			ImageView.Image = image;
		}


		//Drawing
		public override void Draw(CGRect rect)
		{
			if (CurrentPathView != null)
			{
				CurrentPathView.Stroke();
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

            //System.Diagnostics.Debug.WriteLine("Start touch");

			CurrentStroke = new Stroke()
			{
				StrokeColor = StrokeColor,
				Thickness = StrokeThickness
			};

			CurrentPathView.CreateNewPath(CurrentStroke);

			var loc = (touches.AnyObject as UITouch).LocationInView(this);
			CurrentPathView.MoveTo(loc);
			CurrentStroke.Points.Add(new Point(loc.X, loc.Y));
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			var loc = (touches.AnyObject as UITouch).LocationInView(this);
			CurrentPathView.AddLineTo(loc);
			CurrentStroke.Points.Add(new Point(loc.X, loc.Y));

			SetNeedsDisplay();
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			var loc = (touches.AnyObject as UITouch).LocationInView(this);
			CurrentStroke.Points.Add(new Point(loc.X, loc.Y));
			CurrentPathView.AddLineTo(loc);

			Strokes.Add(CurrentStroke);
			drawPath();

			CurrentPathView.Clear();
			SetNeedsDisplay();
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			ImageView.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
			if (BackgroundImage != null)
				BackgroundImage.Frame = new CGRect(0, 0, BackgroundImage.Image.Size.Width, BackgroundImage.Image.Size.Height);
			CurrentPathView.Frame = ImageView.Frame;
		}
	}
}
