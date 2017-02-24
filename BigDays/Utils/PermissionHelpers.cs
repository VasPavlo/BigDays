using System;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Content.PM;
using BigDays.Enums;
using System.Threading.Tasks;

namespace BigDays.Helpers
{
	//[IntentFilter(new[] { MediaStore.IntentActionStillImageCamera }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryVoice })]
	//[IntentFilter(new[] { MediaStore.IntentActionStillImageCameraSecure }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryVoice })]
	public static class PermissionHelpers
	{
		public static readonly string[] PermissionsCamera =
			{
				Manifest.Permission.Camera,
				Manifest.Permission.WriteExternalStorage
			};
        

		private static bool GetwVersionM() 
		{
            return (Build.VERSION.SdkInt >= Build.VERSION_CODES.M); 			  
		}

        public static bool NeedPermissionsCamera(Activity _activity)
        {
            if (GetwVersionM())
            {
                return _activity.CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted || _activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted;
            }
            else return false;
        }


        public static void RequestPermissionssCamera(Activity _activity)
        {
            _activity.RequestPermissions(PermissionsCamera, (int)RequestCode.PermissionsCameraAndWriteExternalStorage);
        }

        public static bool NeedPermissionsWriteExternalStorage(Activity _activity)
        {
            if (GetwVersionM())
            {
                return _activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted;
            }
            else return false;
        }

        public static void RequestPermissionssWriteExternalStorage(Activity _activity)
        {
            _activity.RequestPermissions(new string[]{ Manifest.Permission.WriteExternalStorage }, (int)RequestCode.PermissionsWriteExternalStorage);

            return;
        }


        public static bool NeedPermissionsSendSms(Activity _activity)
        {
            if (GetwVersionM())
            {
                return _activity.CheckSelfPermission(Manifest.Permission.SendSms) != Permission.Granted || _activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted;
            }
            else return false;
        }


        public static void RequestPermissionssSendSms(Activity _activity)
        {
            _activity.RequestPermissions(new string[] { Manifest.Permission.SendSms }, (int)RequestCode.PermissionsCameraAndWriteExternalStorage);
        }
    }
}
