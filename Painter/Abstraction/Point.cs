//
// Point.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
namespace Painter.Abstractions
{
	public class Point
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Point()
		{

		}

		public Point(double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}
	}
}
