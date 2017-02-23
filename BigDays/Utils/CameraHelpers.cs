using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Provider;
using BigDays.Enums;
using Java.IO;

namespace BigDays
{
	public static class App
	{
		public static File _file;
		public static File _dir;
		public static Bitmap bitmap;
	}


	public class CameraHelpers
	{
		private Activity _activity;

		private CameraHelpers() {}

		public CameraHelpers(Activity activity)
		{
			_activity = activity;
		}

		public void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			App._file = new File(App._dir, string.Format("BigDaysPhoto_{0}.jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
			_activity.StartActivityForResult(intent, (int)RequestCode.CameraImage);
		}

		public void CreateDirectoryForPictures()
		{
			App._dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "BigDays");
			if (!App._dir.Exists())
			{
				App._dir.Mkdirs();
			}
		}

		public bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities = _activity.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

	}
}
