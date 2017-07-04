# XamarinPainter
An easy to use view in which you can draw with multiple colors and thicknesses for Xamarin and Xamarin.Forms

# How to use it
## Installation
### Android
Install the nuget package and add a reference to Mono.Android.Export to your Android project (will be done via the Nuget package in the future)
### iOS
Install the nuget package and call "PainterRenderer.Init();" Inside the FinishedLaunching of your app delegate
### UWP (untested atm)
Install the nuget package
## Simple example
Creating a PainterView in code is done like this

`Painter.Forms.PainterView v = new Painter.Forms.PainterView()  
{  
  HorizontalOptions = LayoutOptions.FillAndExpand,  
  VerticalOptions = LayoutOptions.FillAndExpand  
};  `

In XAML you will need to add the reference `xmlns:painter="clr-namespace:Painter.Forms;assembly=Painter.Forms"` to your top class and then you can create a PainterView like this `<painter:PainterView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" />`
## To keep in mind
You Must define a Width and Height on Android or the plugin will crash since it can't create a Bitmap of 0 by 0 pixels, this bug will be fixed in a future release.


# Currently in development
This project is nowhere near a finished product at the moment, keep in mind that the code may be refactored in the future which can break your current implementation (though I'll try not to break it)

# Expected features:
## Platforms
* Xamarin.iOS
* Xamarin.Droid
* Xamarin.UWP
* Xamarin.Forms (iOS, Android and UWP)

## Customization
* Stroke color
* Stroke thickness
* Background image

## Exporting
* Save an image directly to the device
* Retrieve an image object
* Retrieve a MemoryStream with the image data (i.e. for networking)
* Retrieve a JSON string
* Save a JSON string directly to the device

## Importing
* Load a JSON string
* Load a file from the device
* Load a background image
