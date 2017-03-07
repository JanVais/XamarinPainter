//
// SavedImageState.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using Android.OS;
using Android.Views;

namespace Painter.Android
{
	public class SavedImageState : View.BaseSavedState
	{
		public string Json { get; set; }

		public SavedImageState(IParcelable superState) : base(superState)
		{
		}

		private SavedImageState(Parcel parcel) : base(parcel)
		{
			Json = parcel.ReadString();
		}

		public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
		{
			base.WriteToParcel(dest, flags);
			dest.WriteString(Json);
		}
		[Java.Interop.ExportField("CREATOR")]
		public static TitleSavedStateCreator InitializeCreator()
		{
			return new TitleSavedStateCreator();
		}

		public class TitleSavedStateCreator : Java.Lang.Object, IParcelableCreator
		{
			public Java.Lang.Object CreateFromParcel(Parcel source)
			{
				return new SavedImageState(source);
			}

			public Java.Lang.Object[] NewArray(int size)
			{
				return new SavedImageState[size];
			}
		}
	}
}
