//
// Stroke.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
using System.Collections.Generic;

namespace Painter.Abstractions
{
	public class Stroke
	{
		public List<Point> Points { get; set; }
		public Color StrokeColor { get; set; }
		public double Thickness { get; set; }
		public bool ClosePath { get; set; }

		public Stroke()
		{
			Points = new List<Point>();
			StrokeColor = new Color();
			Thickness = 1.0f;
			ClosePath = false;
		}

		public Stroke(List<Point> Points) : base()
		{
			this.Points = Points;
		}

		public Stroke(List<Point> Points, Color StrokeColor) : base()
		{
			this.Points = Points;
			this.StrokeColor = StrokeColor;
		}

		public Stroke(Color StrokeColor) : base()
		{
			Points = new List<Point>();
			this.StrokeColor = StrokeColor;
		}
	}
}
