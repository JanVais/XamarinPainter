//
// CatmullRomSmoothing.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
using System.Collections.Generic;
using Painter.Abstractions;

namespace Painter
{
	public class CatmullRomSmoothing
	{
		public static List<Point> SmoothPath(List<Point> path, int granularity)
		{
			//If there are less then 4 points we can't smooth it
			if (path.Count < 4)
				return path;

			//Create the return object
			List<Point> returnPath = new List<Point>();

			//Insert the first points
			for (int i = 0; i < 3; i++)
				returnPath.Add(path[i]);

			for (int i = 4; i < path.Count; i++)
			{
				//Grab the 4 points
				var p0 = path[i - 3];
				var p1 = path[i - 2];
				var p2 = path[i - 1];
				var p3 = path[i];

				//Catmull-Rom magic
				for (int j = 0; j < granularity; j++)
				{
					double t = (double)j * (1.0 / (double)granularity);
					double tt = t * t;
					double ttt = tt * t;

					Point pi = new Point()
					{
						X = 0.5 * (2.0 * p1.X + (p2.X - p0.X) * t + (2.0 * p0.X - 5.0 * p1.X + 4.0 * p2.X - p3.X) * tt + (3.0 * p1.X - p0.X - 3.0 * p2.X + p3.X) * ttt),
						Y = 0.5 * (2.0 * p1.Y + (p2.Y - p0.Y) * t + (2.0 * p0.Y - 5.0 * p1.Y + 4.0 * p2.Y - p3.Y) * tt + (3.0 * p1.Y - p0.Y - 3.0 * p2.Y + p3.Y) * ttt)
					};
					returnPath.Add(pi);
				}

				returnPath.Add(p2);
			}

			//Add the last point
			returnPath.Add(path[path.Count - 1]);

			return returnPath;
		}
	}
}
