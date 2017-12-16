using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
namespace BigDays
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
	public class NotificationReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			Toast.MakeText(context, "!!!!BigDays - Countdown: Received AutoStartReceiver.", ToastLength.Short).Show();
			Intent pushIntent = new Intent(context, typeof(AfterRebootDeviceRevertAlertService));
			context.StartService(pushIntent);
		}
	}
}