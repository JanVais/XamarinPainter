//
// Color.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
namespace Painter
{
	public class Color
	{
		public double R { get; set; }
		public double G { get; set; }
		public double B { get; set; }
		public double A { get; set; }

		public Color()
		{
			this.R = 0.0f;
			this.G = 0.0f;
			this.B = 0.0f;
			this.A = 1.0f;
		}

		public Color(double R, double G, double B, double A = 1.0f) : base()
		{
			this.R = R;
			this.G = G;
			this.B = B;
			this.A = A;
		}
	}
}
