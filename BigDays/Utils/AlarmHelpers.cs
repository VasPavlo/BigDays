using System;
using Android.App;
using Android.Content;
using BigDays.Models;

namespace BigDays.Utils
{
	public static class AlarmHelpers
	{
		public static void UpdateAlarm(Context context, BigDaysItemModel item,  int garbageMain)
		{
			string MainMessage = string.Format("BIG DAYS OF OUR LIVES COUNTDOWN\r\n{0}\r\n\r\nCongratulation!\r\n{0} has com.", item._Name);
			DateTime d1Main = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan tsMain = new TimeSpan(item._EndDate.ToFileTimeUtc() - d1Main.Ticks);

			long whensysMain = (long)item._EndDate.ToUniversalTime().Subtract(d1Main).TotalMilliseconds;
			Intent IntentMainNot = new Intent(context, typeof(NotificationView));


			IntentMainNot.PutExtra("ID", item._Notification);
			IntentMainNot.PutExtra("ItemID", item._ID);
			IntentMainNot.PutExtra("Title", item._Name);
			IntentMainNot.PutExtra("Message", MainMessage);
			IntentMainNot.PutExtra("Garbage", garbageMain);
			PendingIntent mAlarmMainSender = PendingIntent.GetBroadcast(context, item._Notification, IntentMainNot, PendingIntentFlags.UpdateCurrent);
			PendingIntent mAlarmMainSenderCansel = PendingIntent.GetBroadcast(context, garbageMain, IntentMainNot, PendingIntentFlags.UpdateCurrent);
			AlarmManager amMain = (AlarmManager)context.GetSystemService(Context.AlarmService);
			amMain.Cancel(mAlarmMainSenderCansel);
			amMain.Set(AlarmType.RtcWakeup, whensysMain, mAlarmMainSender);
		}

		public static void SetAlarm(Context context, BigDaysItemModel item)
		{
			string MainMessage = string.Format("BIG DAYS OF OUR LIVES COUNTDOWN\r\n{0}\r\n\r\nCongratulation!\r\n{0} has com.", item._Name);

			DateTime d1Main = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan tsMain = new TimeSpan(item._EndDate.ToFileTimeUtc() - d1Main.Ticks);

			long whensysMain = (long)item._EndDate.ToUniversalTime().Subtract(d1Main).TotalMilliseconds;
			Intent IntentMainNot = new Intent(context, typeof(NotificationView));

			IntentMainNot.PutExtra("ID", item._Notification);
			IntentMainNot.PutExtra("ItemID", item._ID);
			IntentMainNot.PutExtra("Title", item._Name);
			IntentMainNot.PutExtra("Message", MainMessage);
			PendingIntent mAlarmMainSender = PendingIntent.GetBroadcast(context, item._Notification, IntentMainNot, PendingIntentFlags.UpdateCurrent);
			AlarmManager amMain = (AlarmManager)context.GetSystemService(Context.AlarmService);
			amMain.Set(AlarmType.RtcWakeup, whensysMain, mAlarmMainSender);
		}


		public static void RemoveAlarm(Context context, int notification)
		{
			Intent IntentMainNot = new Intent(context, typeof(NotificationView));
			PendingIntent mAlarmMainSenderCansel = PendingIntent.GetBroadcast(context, notification, IntentMainNot, PendingIntentFlags.UpdateCurrent);
			AlarmManager amMain = (AlarmManager)context.GetSystemService(Context.AlarmService);
			amMain.Cancel(mAlarmMainSenderCansel);
		}


		public static void SetAlertsAlarm(Context context, BigDaysItemModel item, int ID, string[] alertStr)
		{
			var MainMessage = string.Format("{0} - {1}/{2}/{3} {4}:{5}", item._Name, item._EndDate.Day, item._EndDate.Month, item._EndDate.Year, item._EndDate.Hour, item._EndDate.Minute);

			DateTime when = CalcDateAlerts(alertStr, item);

				DateTime d1Main = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				TimeSpan tsMain = new TimeSpan(when.ToFileTimeUtc() - d1Main.Ticks);

				long whensysMain = (long)when.ToUniversalTime().Subtract(d1Main).TotalMilliseconds;
				Intent IntentMainNot = new Intent(context, typeof(NotificationView));


				IntentMainNot.PutExtra("ID", ID);
				IntentMainNot.PutExtra("ItemID", item._ID);
				IntentMainNot.PutExtra("Title", item._Name);
				IntentMainNot.PutExtra("Message", MainMessage);

				PendingIntent mAlarmMainSender = PendingIntent.GetBroadcast(context, ID, IntentMainNot, PendingIntentFlags.UpdateCurrent);
				AlarmManager amMain = (AlarmManager)context.GetSystemService(Context.AlarmService);
				amMain.Set(AlarmType.RtcWakeup, whensysMain, mAlarmMainSender);
		}


		public static void UpdateAlertsAlarm(Context context, BigDaysItemModel item, int ID, string[] alertStr, int garbage)
		{
			string message = string.Format("{0} - {1}/{2}/{3} {4}:{5}", item._Name, item._EndDate.Day, item._EndDate.Month, item._EndDate.Year, item._EndDate.Hour, item._EndDate.Minute);

			DateTime when = CalcDateAlerts(alertStr, item);

			DateTime d1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan ts = new TimeSpan(when.ToFileTimeUtc() - d1.Ticks);
			long whensys = (long)when.ToUniversalTime().Subtract(d1).TotalMilliseconds;

			Intent IntentNot = new Intent(context, typeof(NotificationView));
			IntentNot.PutExtra("ID", ID);
			IntentNot.PutExtra("Title", item._Name);
			IntentNot.PutExtra("Message", message);
			IntentNot.PutExtra("ItemID", item._ID);
			PendingIntent mAlarmSender = PendingIntent.GetBroadcast(context, ID, IntentNot, PendingIntentFlags.UpdateCurrent);
			PendingIntent mAlarmSenderCansel = PendingIntent.GetBroadcast(context, garbage, IntentNot, PendingIntentFlags.UpdateCurrent);
			AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
			am.Cancel(mAlarmSenderCansel);
			am.Set(AlarmType.RtcWakeup, whensys, mAlarmSender);
		}

		private static DateTime CalcDateAlerts(string[] alertStr, BigDaysItemModel item) 
		{
			DateTime when = item._EndDate;
			switch (Convert.ToInt16(alertStr[4]))
			{
				case 0:
					when = item._EndDate.AddSeconds(-Convert.ToInt16(alertStr[3]));
					break;
				case 1:
					when = item._EndDate.AddMinutes(-Convert.ToInt16(alertStr[3]));
					break;
				case 2:
					when = item._EndDate.AddHours(-Convert.ToInt16(alertStr[3]));
					break;
				case 3:
					when = item._EndDate.AddDays(-Convert.ToInt16(alertStr[3]));
					break;
				case 4:
					when = item._EndDate.AddDays(-(Convert.ToInt16(alertStr[3]) * 7));
					break;
				case 5:
					when = item._EndDate.AddMonths(-Convert.ToInt16(alertStr[3]));
					break;
				case 6:
					when = item._EndDate.AddYears(-Convert.ToInt16(alertStr[3]));
					break;
			}
			return when;
		}
	}
}