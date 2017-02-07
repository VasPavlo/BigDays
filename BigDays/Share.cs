using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Content.PM;

namespace BigDays
{
	[Activity (Label = "Share", ScreenOrientation = ScreenOrientation.Portrait)]			
	public class Share : Activity
	{
		private int _ID;
		private BigDaysItem _Item;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Share);

			_ID = Intent.GetIntExtra ("ID", 0);

			_Item = MainActivity._BDDB.SelectItem (_ID);

			var googlePlayLink = "http://play.google.com/store/apps/details?id=" + this.PackageName;
			#if __AMAZON__
			googlePlayLink = "http://www.amazon.com/gp/mas/dl/android?p=" + this.PackageName;
			#endif

			var ui_sendEmailBtn = FindViewById<Button> (Resource.Id.sendEmail);
			ui_sendEmailBtn.Click += (sender, e) => {
				TimeSpan ts = _Item._EndDate.Subtract (DateTime.Now);
				string DaysTo = String.Format ("{0:0000}", ts.Days);
				string HoursTo = String.Format ("{0:00}", ts.Hours);
				string MinTo = String.Format ("{0:00}", ts.Minutes);
				string SecTo = String.Format ("{0:00}", ts.Seconds);
				String message = string.Format("{0} {1} {2} days, {3} hours, {4} minutes, {5} seconds {6} \n{7}",
					_Item._Name,
					_Item._EndDate < DateTime.Now ? "was" : "is coming in",
					DaysTo, HoursTo, MinTo, SecTo,
					_Item._EndDate < DateTime.Now ? " ago" : "",
					googlePlayLink);

				Intent i = new Intent(Intent.ActionSend);
				i.SetType("message/rfc822");
				i.PutExtra(Intent.ExtraSubject, _Item._Name);
				i.PutExtra(Intent.ExtraText   , message);
				try {
					StartActivityForResult(Intent.CreateChooser(i, "Send mail..."), 0);
				} catch (Android.Content.ActivityNotFoundException ex) {
					Toast.MakeText(this, "There are no email clients installed.", ToastLength.Short).Show();
				}
			};

			var ui_sendSMSBtn = FindViewById<Button> (Resource.Id.sendSMS);
			ui_sendSMSBtn.Click += (sender, e) => {
				TimeSpan ts = _Item._EndDate.Subtract (DateTime.Now);
				string DaysTo = String.Format ("{0:0000}", ts.Days);
				string HoursTo = String.Format ("{0:00}", ts.Hours);
				string MinTo = String.Format ("{0:00}", ts.Minutes);
				string SecTo = String.Format ("{0:00}", ts.Seconds);
				String message = string.Format("{0} {1} {2} days, {3} hours, {4} minutes, {5} seconds {6} \n{7}",
					_Item._Name,
					_Item._EndDate < DateTime.Now ? "was" : "is coming in",
					DaysTo, HoursTo, MinTo, SecTo,
					_Item._EndDate < DateTime.Now ? " ago" : "",
					googlePlayLink);

				try {
					Intent sendIntent = new Intent(Intent.ActionView);
					sendIntent.PutExtra("sms_body", message); 
					sendIntent.SetType("vnd.android-dir/mms-sms");
					StartActivity(sendIntent);
				} catch (Exception em) {
					Toast.MakeText(this, "SMS faild, please try again later!", ToastLength.Long).Show();
				}					
			};


			var ui_FacebookBtn = FindViewById<Button> (Resource.Id.FacebookBtn);
			ui_FacebookBtn.Click += (sender, e) => 
			{
				var IntentShareActivity = new Intent(this, typeof(FacebookScreen));
				IntentShareActivity.PutExtra("ID", _ID);
				StartActivity(IntentShareActivity);
			};

			var ui_TwitterBtn = FindViewById<Button> (Resource.Id.TwitterBtn);
			ui_TwitterBtn.Click += (sender, e) =>
			{
				TimeSpan ts = _Item._EndDate.Subtract (DateTime.Now);
				string DaysTo = String.Format ("{0:0000}", ts.Days);
				string HoursTo = String.Format ("{0:00}", ts.Hours);
				string MinTo = String.Format ("{0:00}", ts.Minutes);
				string SecTo = String.Format ("{0:00}", ts.Seconds);
				String message = string.Format("{0} {1} {2} days, {3} hours, {4} minutes, {5} seconds {6}",
					_Item._Name,
					_Item._EndDate < DateTime.Now ? "was" : "is coming in",
					DaysTo, HoursTo, MinTo, SecTo,
					_Item._EndDate < DateTime.Now ? " ago" : "");			  


				Intent browserIntent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(
					string.Format("http://www.twitter.com/intent/tweet?&text={0}&url={1}",message, googlePlayLink )));				
				StartActivity(browserIntent); 

			};
		}	
	}
}