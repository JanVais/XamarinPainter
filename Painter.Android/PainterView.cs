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

namespace Painter.Android
{
	public class PainterView : RelativeLayout
	{
		private Context context;

		public PainterView(Context context) : base (context)
		{
			this.context = context;
			Initialize();
		}

		public PainterView(Context context, IAttributeSet attrs) : base (context, attrs)
		{
			this.context = context;
			Initialize();
		}

		public PainterView(Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			this.context = context;
			Initialize();
		}

		private void Initialize()
		{

		}
	}
}