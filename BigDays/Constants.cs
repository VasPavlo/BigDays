using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BigDays
{
	#pragma warning disable 0219, 0649
	[Android.Runtime.Preserve(AllMembers=true)]
	public static class Constants
	{
		public static string VersionLink;
		public static readonly string StoreSearchLink;
		public static string CaptionName;

		public static Android.Net.Uri URI { get; set; }
		public static Bitmap ImageBtm { get; set; }

		static Constants()
		{
			VersionLink = "http://play.google.com/store/apps/details?id=com.prosellersworld.bigdaysapp&c=apps";
			StoreSearchLink = "https://play.google.com/store/search?q=com.prosellersworld.bigdaysapp";
			CaptionName = "Download from Google Play ...";


			#if _TRIAL_
			VersionLink = "http://play.google.com/store/apps/details?id=com.prosellersworld.bigdaysfree&c=apps";
			StoreSearchLink = "https://play.google.com/store/search?q=com.prosellersworld.bigdaysfree";
			CaptionName = "Download from Google Play...";
			#endif

			#if __AMAZON__
			VersionLink = "http://www.amazon.com/gp/mas/dl/android?p=com.prosellersworld.bigdays";
			StoreSearchLink = "http://www.amazon.com/gp/mas/dl/android?p=com.prosellersworld.bigdays";
			CaptionName = "Download from AMAZON ...";
			#endif
			//market://details?id=com.prosellersworld.bigdays
		}
	}
	#pragma warning restore 0219, 0649
}

