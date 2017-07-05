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
using Newtonsoft.Json;
using Java.Nio;
using static Android.Graphics.Bitmap;
using System.Threading.Tasks;
using System.IO;

namespace Painter.Android
{
	public class PainterView : RelativeLayout
	{
		//Public UI
		public Abstractions.Color StrokeColor { get; set; }
        public Abstractions.Color BackgroundColor { get; set; } = new Abstractions.Color(0, 0, 0, 0); //TODO expose to Forms
		public double StrokeThickness { get; set; }

		//Private
		private Context context;
		private Canvas canvas;
		private ImageView imageView;
		private Bitmap image;
		private DisplayMetrics metrics;
		private Abstractions.Stroke currentStroke;
		private List<Abstractions.Stroke> strokes = new List<Abstractions.Stroke>();
        private PainterExport export = new PainterExport();

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

		public PainterView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			this.context = context;
			Initialize();
		}

		private void Initialize()
		{
			imageView = new ImageView(context);
			imageView.SetBackgroundColor(Color.Transparent);

			StrokeColor = new Abstractions.Color(0, 0, 1);
			StrokeThickness = 1.0;

			AddView(imageView);
		}


		//Exports
		public string GetJson()
		{
			return JsonConvert.SerializeObject(strokes);
		}

        public async Task<byte[]> GetCurrentImageAsPNG(int width, int height, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null)
        {
            return await export.GetCurrentImageAsPNG(width, height, strokes, scaling, quality, BackgroundColor);
        }

        public async Task<byte[]> GetCurrentImageAsJPG(int width, int height, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null)
        {
            return await export.GetCurrentImageAsJPG(width, height, strokes, scaling, quality, BackgroundColor);
        }


        //Imports
        public void LoadJson(string json)
		{
			try
			{
				strokes = JsonConvert.DeserializeObject<List<Abstractions.Stroke>>(json);
				DrawStrokes();
				Invalidate();
			}
			catch (System.Exception e)
			{
				//Invalid json
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

		public void Clear()
		{
			strokes.Clear();
			canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
			Invalidate();
		}


		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);

			if (canvas != null)
				return;

			IWindowManager windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			metrics = new DisplayMetrics();
			windowManager.DefaultDisplay.GetMetrics(metrics);
            
            if (image == null && Width != 0 && Height != 0)
			{
				image = Bitmap.CreateBitmap(metrics, Width, Height, Bitmap.Config.Argb8888);
				canvas = new Canvas(image);

				imageView.SetImageBitmap(image);
			}

			DrawStrokes();
		}

		protected override IParcelable OnSaveInstanceState()
		{
			var state = base.OnSaveInstanceState();

			var savedState = new SavedImageState(state)
			{
				Json = JsonConvert.SerializeObject(strokes)
			};

			imageView.SetImageBitmap(null);
			imageView = null;

			image.Recycle();
			image.Dispose();
			image = null;

			canvas.Dispose();
			canvas = null;

			GC.Collect();

			return savedState;
		}

		protected override void OnRestoreInstanceState(IParcelable state)
		{
			var savedState = state as SavedImageState;
			if (savedState != null)
			{
				base.OnRestoreInstanceState(savedState.SuperState);
				strokes = JsonConvert.DeserializeObject<List<Abstractions.Stroke>>(savedState.Json);
			}
			else
				base.OnRestoreInstanceState(state);
			RequestLayout();
		}

		private void DrawStrokes()
		{
            canvas.DrawColor(new Color((byte)(BackgroundColor.R * 255), (byte)(BackgroundColor.G * 255), (byte)(BackgroundColor.B * 255), (byte)(BackgroundColor.A * 255)), PorterDuff.Mode.Src);
            
			foreach (var stroke in strokes)
			{
				double lastX = stroke.Points[0].X;
				double lastY = stroke.Points[0].Y;

				var paint = new Paint()
				{
					Color = new Color((byte)(stroke.StrokeColor.R * 255.0), (byte)(stroke.StrokeColor.G * 255.0), (byte)(stroke.StrokeColor.B * 255.0), (byte)(stroke.StrokeColor.A * 255.0)),
					StrokeWidth = (float)stroke.Thickness * metrics.Density,
					AntiAlias = true,
					StrokeCap = Paint.Cap.Round
				};

				foreach (var p in stroke.Points)
				{
                    canvas.DrawLine((float)lastX, (float)lastY, (float)p.X, (float)p.Y, paint);
					lastX = p.X;
					lastY = p.Y;
				}
			}
		}

		private void DrawCurrentStroke(Canvas _canvas)
		{
			if (currentStroke != null && currentStroke.Points.Count > 0)
			{
				double lastX = currentStroke.Points[0].X;
				double lastY = currentStroke.Points[0].Y;

				var paint = new Paint()
				{
					Color = new Color((byte)(currentStroke.StrokeColor.R * 255.0), (byte)(currentStroke.StrokeColor.G * 255.0), (byte)(currentStroke.StrokeColor.B * 255.0), (byte)(currentStroke.StrokeColor.A * 255.0)),
					StrokeWidth = (float)currentStroke.Thickness * metrics.Density,
					AntiAlias = true,
					StrokeCap = Paint.Cap.Round
				};

				foreach (var p in currentStroke.Points)
				{
                    _canvas.DrawLine((float)lastX, (float)lastY, (float)p.X, (float)p.Y, paint);
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
					currentStroke = new Abstractions.Stroke()
					{
						StrokeColor = StrokeColor,
						Thickness = StrokeThickness,
					};
					currentStroke.Points.Add(new Abstractions.Point(e.GetX(), e.GetY()));

					return true;
				case MotionEventActions.Move:
					currentStroke.Points.Add(new Abstractions.Point(e.GetX(), e.GetY()));

					DrawCurrentStroke(canvas);
					Invalidate();
					break;
				case MotionEventActions.Up:
					currentStroke.Points.Add(new Abstractions.Point(e.GetX(), e.GetY()));
					strokes.Add(currentStroke);
					break;
				default:
					return false;
			}

			return true;
		}
	}
}