using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Graphics;

namespace Painter.Android
{
	public class PainterView : RelativeLayout
	{
		private Context context;
		private Canvas canvas;
		private ImageView imageView;

		public PainterView(Context context) : base(context)
		{
			this.context = context;
			Initialize();
		}

		public PainterView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			this.context = context;
			Initialize();
		}

		public PainterView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			this.context = context;
			Initialize();
		}

		private void Initialize()
		{
			imageView = new ImageView(context);
			imageView.SetBackgroundColor(Color.Transparent);
			imageView.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
			AddView(imageView);
		}

		private Abstractions.Stroke stroke;

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);

			if (canvas != null)
				return;

			IWindowManager windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			DisplayMetrics metrics = new DisplayMetrics();
			windowManager.DefaultDisplay.GetMetrics(metrics);

			var image = Bitmap.CreateBitmap(metrics, Width, Height, Bitmap.Config.Argb8888);
			canvas = new Canvas(image);
			imageView.SetImageBitmap(image);

			DrawStrokes();
		}

		private void DrawStrokes()
		{
			//canvas.DrawLine(0, 0, Width, Height, new Paint() { Color = Color.Black, StrokeWidth = 3.0f });

			if (stroke != null && stroke.Points.Count > 0)
			{
				double lastX = stroke.Points[0].X;
				double lastY = stroke.Points[0].Y;
				foreach (var p in stroke.Points)
				{
					canvas.DrawLine((float)lastX, (float)lastY, (float)p.X, (float)p.Y, new Paint() { Color = Color.Blue, StrokeWidth = 3.0f });
					lastX = p.X;
					lastY = p.Y;
				}
			}
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			switch (e.Action)
			{
				case MotionEventActions.Down:
					stroke = new Abstractions.Stroke();

					stroke.Points.Add(new Abstractions.Point(e.GetX(), e.GetY()));

					return true;
				case MotionEventActions.Move:

					stroke.Points.Add(new Abstractions.Point(e.GetX(), e.GetY()));
					DrawStrokes();
					Invalidate();
					break;
				case MotionEventActions.Up:

					break;
				default:
					return false;
			}

			return true;
		}
	}
}