using System;
using Android.App;
using Android.Content;
using Android.Provider;
using BigDays.Enums;
using Java.IO;

namespace BigDays
{
	public class CameraHelpers
	{
		private Activity _activity;
		public Java.IO.File _file;
		public Java.IO.File _dir;

		private CameraHelpers() {}

		public CameraHelpers(Activity activity)
		{
			_activity = activity;
		}

		public void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			_file = new File(_dir, string.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_file));
			_activity.StartActivityForResult(intent, (int)RequestCode.CameraImage);
		}

		public void CreateDirectoryForPictures()
		{
			_dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "BigDays");
			if (!_dir.Exists())
			{
				_dir.Mkdirs();
			}
		}
	}
}
