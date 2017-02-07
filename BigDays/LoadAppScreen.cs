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
using System.Threading;

namespace BigDays
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class LoadAppScreen : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here

			//Thread.Sleep(2000); // Simulate a long loading process on app startup.
			StartActivity(typeof(MainActivity));
		}
	}
}

