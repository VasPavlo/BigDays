using System;
using Android.App;
using Android.Content;
using BigDays.Services;
using BigDays.Utils;
using Android;
using Android.OS;

namespace BigDays
{
	[Service]
	[IntentFilter(new string[] { "com.prosellersworld.bigdaysfree.AfterRebootDeviceRevertAlertService" })]
	public class AfterRebootDeviceRevertAlertService : IntentService
	{
		public DataService _BDDB;
		protected override void OnHandleIntent(Intent intent)
		{
			try
			{
				_BDDB = new DataService();
				_BDDB.ConnectToDB("BigDaysNew.db3");

				var _BDitems = _BDDB.SelectBDItems();

				foreach (var item in _BDitems)
				{
					AlarmHelpers.SetAlarm(this, item);


					string[] alerts = item._Alerts.Split('#');
					foreach (var a in alerts)
					{
						string[] alertStr = a.Split(';');

						if (alertStr[1] == "1")
						{
							int ID = Convert.ToInt32(alertStr[2]);
							item._ID = _BDDB.GetLastID() + 1;
							AlarmHelpers.SetAlertsAlarm(this, item, ID, alertStr);

							//Test
							//Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
							//vibrator.Vibrate(2000);
						}
					}

				}
				

				//NotificInPanelAlarm              
				////AlarmManager alarmRepeating = (AlarmManager)GetSystemService(AlarmService);
				////Notifications.NotificInPanelAlarm.SetAlarm(alarmRepeating, this);
				//////-------------------

				////var DataServices = new DataService(new PlatformService().GetSQLiteConnection());
				////var items = DataServices.GetAllBigDaysItems();
				////foreach (var item in items)
				////{
				////	AlarmManager alarm = (AlarmManager)GetSystemService(AlarmService);
				////	if (item.EndDate >= DateTime.Now.AddSeconds(10))
				////	{
				////		Notifications.AlarmService.SetAlarm(item, alarm, this);
				////	}
				////	try
				////	{
				////		AlarmManager alarm2 = (AlarmManager)GetSystemService(AlarmService);
				////		Notifications.AlarmService.SetReminderAlarm(item, alarm2, this);
				////	}
				////	catch (System.Exception e)
				////	{
				////	}
				///}
			}
			catch (Java.Lang.Exception)
			{
				//var currentTime = Java.Lang.JavaSystem.CurrentTimeMillis();
				//var nMgr = (NotificationManager)GetSystemService(NotificationService);
				//var notification = new Notification(Resource.Drawable.icon, "Problem", currentTime);
				//var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);
				//notification.SetLatestEventInfo(this, "Reminder Service Notification", "Exaption", pendingIntent);
				//nMgr.Notify(0, notification);
			}
		}
	}
}