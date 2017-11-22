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
using Painter.Interfaces;
using System.ComponentModel;
using Android.Media;

namespace Painter.Droid
{
	public class PainterView : RelativeLayout
	{
		//Public UI
		public Abstractions.Color StrokeColor { get; set; }
		public Abstractions.Color BackgroundColor { get; set; } = new Abstractions.Color(0, 0, 0, 0); //TODO expose to Forms


		public double StrokeThickness { get; set; }
		private List<Abstractions.Stroke> _strokes;
		public List<Abstractions.Stroke> Strokes
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
		public Abstractions.Scaling BackgroundScaling
		{
			get
			{
				return backgroundScaling;
			}
			set
			{
				backgroundScaling = value;

				//TODO re-draw
			}
		}

		public EventHandler FinishedStrokeEvent { get; set; }

		//Private
		private Context context;
		private Canvas canvas;
		private Canvas drawingCanvas;
		private ImageView imageView;
		private ImageView drawingImageView;
		private Bitmap image;
		private Bitmap drawingImage;
		private DisplayMetrics metrics;
		private Abstractions.Stroke currentStroke;
		private Bitmap backgroundBitmap = null;
		private Abstractions.Scaling backgroundScaling = Abstractions.Scaling.Absolute_None;
		private IPainterExport export = new PainterExport();
		private int deviceOrientation = 0;

        private double originalWidth;
        private double originalHeight;

		public PainterView(Context context) : base(context)
		{
			Strokes = new List<Abstractions.Stroke>();
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

			drawingImageView = new ImageView(context);
			drawingImageView.SetBackgroundColor(Color.Transparent);

			StrokeColor = new Abstractions.Color(0, 0, 0, 1);
			StrokeThickness = 1.0;

			IWindowManager windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			metrics = new DisplayMetrics();
			windowManager.DefaultDisplay.GetMetrics(metrics);

			AddView(imageView);
			AddView(drawingImageView);
		}

		//Exports
		public string GetJson()
		{
			return JsonConvert.SerializeObject(Strokes);
		}

		public async Task<byte[]> GetCurrentImageAsPNG(int width, int height, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null)
		{
			return await export.GetCurrentImageAsPNG(width, height, Strokes, scaling, quality, BackgroundColor);
		}

		public async Task<byte[]> GetCurrentImageAsJPG(int width, int height, Abstractions.Scaling scaling = Abstractions.Scaling.Relative_None, int quality = 80, Painter.Abstractions.Color BackgroundColor = null)
		{
			return await export.GetCurrentImageAsJPG(width, height, Strokes, scaling, quality, BackgroundColor);
		}

		//Imports
		public void LoadJson(string json)
		{
			try
			{
				Strokes = JsonConvert.DeserializeObject<List<Abstractions.Stroke>>(json);
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
			Strokes.Clear();
			canvas.DrawColor(BackgroundColor != null ? new Color((byte)(BackgroundColor.R * 255.0), (byte)(BackgroundColor.G * 255.0), (byte)(BackgroundColor.B * 255.0), (byte)(BackgroundColor.A * 255.0)) : Color.Transparent, PorterDuff.Mode.Clear);
			if (backgroundBitmap != null)
				DrawStrokes();
			Invalidate();
		}

		//Background image
		public void LoadImage(string path, bool IsInResources, Abstractions.Scaling Scaling = Abstractions.Scaling.Absolute_None)
		{
			backgroundScaling = Scaling;
			if (IsInResources)
			{
				string file = path.Split('.')[0];
				var id = Resources.GetIdentifier(file.ToLower(), "drawable", Context.PackageName);
				backgroundBitmap = BitmapFactory.DecodeResource(Resources, id);
			}
			else
			{
				backgroundBitmap = BitmapFactory.DecodeFile(path);
				ExifInterface exif = new ExifInterface(path);
				var orientationAttribute = exif.GetAttribute(ExifInterface.TagOrientation);

				Android.Media.Orientation orientationRotate = (Android.Media.Orientation)int.Parse(orientationAttribute);
				int imageRotation = 0;
				switch (orientationRotate)
				{
					case Android.Media.Orientation.Rotate90:
						imageRotation = 90;
						break;
					case Android.Media.Orientation.Rotate180:
						imageRotation = 180;
						break;
					case Android.Media.Orientation.Rotate270:
						imageRotation = 270;
						break;
				}

				backgroundBitmap = RotateBitmap(backgroundBitmap, imageRotation);
			}
		}

		public byte[] BackgroundImageToByte()
		{
			byte[] bitmapData;
			using (var stream = new MemoryStream())
			{
				backgroundBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
				bitmapData = stream.ToArray();
			}
			return bitmapData;
		}

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (image != null && canvas != null)
            { 
                //Destroy the current views
                //imageView.SetImageBitmap(null);
                //drawingImageView.SetImageBitmap(null);

                //image.Recycle();
                //image.Dispose();
                //image = null;

                //drawingImage.Recycle();
                //drawingImage.Dispose();
                //drawingImage = null;

                //canvas.Dispose();
                //canvas = null;

                //drawingCanvas.Dispose();
                //drawingCanvas = null;

                //RemoveView(imageView);
                //RemoveView(drawingImageView);

                //imageView.Dispose();
                //imageView = null;

                //drawingImageView.Dispose();
                //drawingImageView = null;

                ////Collect the GC
                //GC.Collect();

                //Initialize();
                //OnLayout(true, Left, Top, Right, Bottom);
            }
        }
        

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);

			if (canvas != null)
				return;
            
			if (image == null && Width != 0 && Height != 0)
			{
                originalWidth = Width;
                originalHeight = Height;
                
				image = Bitmap.CreateBitmap(metrics, Width, Height, Bitmap.Config.Argb8888);
				drawingImage = Bitmap.CreateBitmap(metrics, Width, Height, Bitmap.Config.Argb8888);

				canvas = new Canvas(image);
				drawingCanvas = new Canvas(drawingImage);
			}

            imageView.SetImageBitmap(image);
            drawingImageView.SetImageBitmap(drawingImage);

			DrawStrokes();
		}

		protected override IParcelable OnSaveInstanceState()
		{
			var state = base.OnSaveInstanceState();

			var savedState = new SavedImageState(state)
			{
				Json = JsonConvert.SerializeObject(Strokes)
			};

			imageView.SetImageBitmap(null);
			imageView = null;

			drawingImageView.SetImageBitmap(null);
			drawingImageView = null;

			image.Recycle();
			image.Dispose();
			image = null;

			drawingImage.Recycle();
			drawingImage.Dispose();
			drawingImage = null;

			canvas.Dispose();
			canvas = null;

            drawingCanvas.Dispose();
            drawingCanvas = null;

			GC.Collect();

			return savedState;
		}

		protected override void OnRestoreInstanceState(IParcelable state)
		{
			var savedState = state as SavedImageState;
			if (savedState != null)
			{
				base.OnRestoreInstanceState(savedState.SuperState);
				Strokes = JsonConvert.DeserializeObject<List<Abstractions.Stroke>>(savedState.Json);
			}
			else
				base.OnRestoreInstanceState(state);
			RequestLayout();
		}

		public static Bitmap RotateBitmap(Bitmap source, float angle)
		{
			Matrix matrix = new Matrix();
			matrix.PostRotate(angle);
			return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
		}

		private void DrawStrokes()
		{
			if (canvas == null)
				return;

			//TODO check the replacement for canvas.Save
			canvas.Save(SaveFlags.Matrix);
			canvas.Translate(canvas.Width / 2f, canvas.Height / 2f);
			canvas.Rotate(deviceOrientation);
			canvas.Translate(-(canvas.Width / 2f), -(canvas.Height / 2f));

            canvas.DrawColor(new Color((byte)(BackgroundColor.R * 255), (byte)(BackgroundColor.G * 255), (byte)(BackgroundColor.B * 255), (byte)(BackgroundColor.A * 255)), PorterDuff.Mode.Src);
            if (backgroundBitmap != null)
			{
				switch (backgroundScaling)
				{
					case Abstractions.Scaling.Absolute_None:
					case Abstractions.Scaling.Relative_None:
						canvas.DrawBitmap(backgroundBitmap, 0, 0, new Paint());
						break;
					case Abstractions.Scaling.Absolute_Fit:
					case Abstractions.Scaling.Relative_Fit:
                        float scale = GetDrawingScale();

						canvas.DrawBitmap(backgroundBitmap, new Rect(0, 0, backgroundBitmap.Width, backgroundBitmap.Height), new Rect(0, 0, (int)(backgroundBitmap.Width * scale), (int)(backgroundBitmap.Height * scale)), new Paint());
						break;
					case Abstractions.Scaling.Absolute_Fill:
					case Abstractions.Scaling.Relative_Fill:
						canvas.DrawBitmap(backgroundBitmap, new Rect(0, 0, backgroundBitmap.Width, backgroundBitmap.Height), new Rect(0, 0, Width, Height), new Paint());
						break;
				}
			}

			if (Strokes == null)
			{
				Strokes = new List<Abstractions.Stroke>();
			}

			foreach (var stroke in Strokes)
			{
				double lastX = stroke.Points[0].X;
				double lastY = stroke.Points[0].Y;

				var paint = new Paint()
				{
					Color = new Color((byte)(stroke.StrokeColor.R * 255.0), (byte)(stroke.StrokeColor.G * 255.0), (byte)(stroke.StrokeColor.B * 255.0), (byte)(stroke.StrokeColor.A * 255.0)),
					StrokeWidth = (float)stroke.Thickness * metrics.Density,
					AntiAlias = true,
					StrokeCap = Paint.Cap.Round,
				};
				paint.SetStyle(Paint.Style.Stroke);

				var path = new Android.Graphics.Path();
				path.MoveTo((float)stroke.Points[0].X, (float)stroke.Points[0].Y);

				foreach (var p in stroke.Points)
					path.LineTo((float)p.X, (float)p.Y);

				canvas.DrawPath(path, paint);
			}

            canvas.Restore();
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
					StrokeCap = Paint.Cap.Round,
				};
				paint.SetStyle(Paint.Style.Stroke);

				var path = new Android.Graphics.Path();
				path.MoveTo((float)currentStroke.Points[0].X, (float)currentStroke.Points[0].Y);

				foreach (var p in currentStroke.Points)
					path.LineTo((float)p.X, (float)p.Y);

				_canvas.DrawPath(path, paint);
			}
		}

		private float GetDrawingScale()
		{
			float scale = 1.0f;

			switch (backgroundScaling)
			{
				case Abstractions.Scaling.Absolute_None:
				case Abstractions.Scaling.Relative_None:
					scale = 1.0f;
					break;
				case Abstractions.Scaling.Absolute_Fit:
				case Abstractions.Scaling.Relative_Fit:
                    if (backgroundBitmap.Height > Height)
                    {
                        scale = (float)Height / (float)backgroundBitmap.Height;
                    }
                    else
                    {
                        scale = (float)Width / (float)backgroundBitmap.Width;
                    }
                    break;
				case Abstractions.Scaling.Absolute_Fill:
				case Abstractions.Scaling.Relative_Fill:
					scale = 1.0f;
					break;
			}

            Log.Debug("PainterWidget", "Current scale: " + scale.ToString());
			return scale;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (!Enabled)
				return true;

			float x = e.GetX();
			float y = e.GetY();

			float scale = GetDrawingScale();
			if (x < 0)
				x = 0;

			if (y < 0)
				y = 0;

			if (x > backgroundBitmap.Width * scale)
				x = backgroundBitmap.Width * scale;

			if (y > backgroundBitmap.Height * scale)
				y = backgroundBitmap.Height * scale;
			
			switch (e.Action)
			{
				case MotionEventActions.Down:
					currentStroke = new Abstractions.Stroke()
					{
						StrokeColor = StrokeColor,
						Thickness = StrokeThickness
					};
					currentStroke.Points.Add(new Abstractions.Point(x, y));

					return true;
				case MotionEventActions.Move:
					currentStroke.Points.Add(new Abstractions.Point(x, y));

					drawingCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
					DrawCurrentStroke(drawingCanvas);
					Invalidate();
					break;
				case MotionEventActions.Up:
					currentStroke.Points.Add(new Abstractions.Point(x, y));
					drawingCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

					var smooth = CatmullRomSmoothing.SmoothPath(currentStroke.Points, 8);
					//Clamp the smooth strokes to the view
					foreach (var p in smooth)
					{
						if (p.X < 0)
							p.X = 0;
						if (p.Y < 0)
							p.Y = 0;
						if (p.X > backgroundBitmap.Width * scale)
							p.X = backgroundBitmap.Width * scale;
						if (p.Y > backgroundBitmap.Height * scale)
							p.Y = backgroundBitmap.Height * scale;
					}
					currentStroke.Points = smooth;

					Strokes.Add(currentStroke);

					DrawStrokes();
					Invalidate();
					FinishedStrokeEvent?.Invoke(this, null);
					break;
				default:
					return false;
			}

			return true;
		}


	}
}