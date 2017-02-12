﻿using Android.App;
using Android.Content;
using BigDays.Services;
using BigDays.DB;

namespace BigDays
{
	[BroadcastReceiver]
	class NotificationMainView: BroadcastReceiver
	{
        public IBigDaysDB _BDDB;
        public NotificationMainView(){
		}

		public override void OnReceive(Context context, Intent intent) {
			NotificationManager mNM;
			mNM = (NotificationManager)context.GetSystemService (Context.NotificationService);

			int ID = intent.GetIntExtra ("ID", 0);
			int ItemID = intent.GetIntExtra ("ItemID", 0);
			//int garbage = intent.GetIntExtra ("Garbage", 0);
			string message = intent.GetStringExtra ("Message");
			string title = intent.GetStringExtra ("Title");
            //mNM.Cancel (garbage);
                       
            _BDDB = new DataService();
            _BDDB.ConnectToDB("BigDaysNew.db3");
            _BDDB.CreateTable();
            _BDDB.SetActive(ItemID);

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

