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

namespace BigDays
{
	[BroadcastReceiver]
	public class NotificationView: BroadcastReceiver
	{
		public NotificationView(){
		}

		public override void OnReceive(Context context, Intent intent) {
			NotificationManager mNM;
			mNM = (NotificationManager)context.GetSystemService (Context.NotificationService);

			int ID = intent.GetIntExtra ("ID", 0);
			int ItemID = intent.GetIntExtra ("ItemID", 0);
			string message = intent.GetStringExtra ("Message");
			string title = intent.GetStringExtra ("Title");
			MainActivity._BDDB.SetActive (ItemID);

			Notification n = new Notification(Resource.Drawable.Icon62, message, Java.Lang.JavaSystem.CurrentTimeMillis());
			Intent i = new Intent (context, typeof(MainActivity));
			i.PutExtra ("ItemID", ItemID);
			i.SetFlags (ActivityFlags.ClearTop | ActivityFlags.SingleTop);
			PendingIntent contentIntent = PendingIntent.GetActivity(context, 0, i, 0);
			n.SetLatestEventInfo (context,title,message,contentIntent);
			n.Defaults = NotificationDefaults.All;
			mNM.Notify(ID, n);
		}
	}
}

