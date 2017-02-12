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
using Android.Text;
using Android.Content.PM;
using Java.Security;

namespace BigDays
{
	[Activity (Theme = "@style/CustomActionBarTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class Feedback : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Feedback);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarFeedback);
			SetActionBar(toolbar);
			ActionBar.Title = "Feedback";

			// Create your application here

			var ui_rateBtn = FindViewById<Button> (Resource.Id.rateBtn);
			ui_rateBtn.Click += (sender, e) => {
				Android.Net.Uri uri = Android.Net.Uri.Parse("market://details?id=" + this.PackageName);
				Intent goToMarket = new Intent(Intent.ActionView, uri);
				try {
					StartActivity(goToMarket);
				} catch (ActivityNotFoundException em) {
					Toast.MakeText(this, "Couldn't launch the market", ToastLength.Short).Show();
				}
			};
				
			var ui_tellFriends = FindViewById<Button> (Resource.Id.tellFriends);

			var googlePlayLink = "http://play.google.com/store/apps/details?id=" + this.PackageName;
			#if __AMAZON__
			googlePlayLink = "http://www.amazon.com/gp/mas/dl/android?p=" + this.PackageName;
			#endif

			ui_tellFriends.Click += (sender, e) => {
				string market_link = googlePlayLink;
				string message = string.Format("<!DOCTYPE html><html><body>Hey, <br/>I just downloaded {0} on my Android Phone!<br/>It's a great app that helps you Countdown your Big Days or Events!<br/>You can download it from google play: <a href='{1}'>App in market</a></body></html>", "\"Big Days of Our Lives Countdown\"", market_link);
				Intent i = new Intent(Intent.ActionSend);
				i.SetType("text/plain");
				i.PutExtra(Intent.ExtraSubject, "Check out this app");
				i.PutExtra(Intent.ExtraText   , Html.FromHtml(message));
				try {
					StartActivityForResult(Intent.CreateChooser(i, "Send mail..."), 0);
				} catch (Android.Content.ActivityNotFoundException ex) {
					Toast.MakeText(this, "There are no email clients installed.", ToastLength.Short).Show();
				}
			};

			var ui_Report = FindViewById<Button> (Resource.Id.Report);
			ui_Report.Click += (sender, e) => {
				Intent i = new Intent(Intent.ActionSend);
				i.SetType("message/rfc822");
				i.PutExtra(Intent.ExtraEmail, new String[]{"info@bigday-countdown.com"});
				i.PutExtra(Intent.ExtraSubject, "Bug Report Android Phone");
				try {
					StartActivityForResult(Intent.CreateChooser(i, "Send mail..."), 0);
				} catch (Android.Content.ActivityNotFoundException ex) {
					Toast.MakeText(this, "There are no email clients installed.", ToastLength.Short).Show();
				}
			};
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
		}
	}
}

