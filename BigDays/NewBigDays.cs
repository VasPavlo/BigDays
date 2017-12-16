using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Provider;
using Android.Graphics;
using Android.Content.Res;
using Java.Lang;
using Genetics.Attributes;
using Genetics;
using BigDays.Enums;
using UniversalImageLoader.Core;
using BigDays.Models;
using BigDays.Converters;
using BigDays.Pixabay;
using Java.IO;
using BigDays.Helpers;
using Android.Content.PM;
using Android.Runtime;
using BigDays.Utils;

namespace BigDays
{
	[Activity(Theme = "@style/CustomActionBarTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class NewBigDays : Activity
	{
		[Splice(Resource.Id.pixabayImagesBtn)]
        private Button pixabayImagesBtn;

        [SpliceClick(Resource.Id.pixabayImagesBtn)]
        void OnPixabayClicked(object sender, EventArgs e)
        {
            var IntentDefaultImagesSelect = new Intent(this, typeof(PixabayView));
            StartActivityForResult(IntentDefaultImagesSelect, (int)RequestCode.PickImage);
        }
        

        [Splice(Resource.Id.defImagesBtn)]
        private Button defImagesBtn;

        [SpliceClick(Resource.Id.defImagesBtn)]
        void OnDefImagesBtnClicked(object sender, EventArgs e)
        {
            var IntentDefaultImagesSelect = new Intent(this, typeof(DefaultImagesSelect));
            StartActivityForResult(IntentDefaultImagesSelect, 0);
        }


        [Splice(Resource.Id.photo_AlbumBtn)]
        private Button photo_AlbumBtn;

        [SpliceClick(Resource.Id.photo_AlbumBtn)]
        void OnPhotoClicked(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            Intent.AddCategory(Intent.CategoryOpenable);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)RequestCode.PickImage);
        }


        [Splice(Resource.Id.CameraBtn)]
        private Button CameraBtn;

        [SpliceClick(Resource.Id.CameraBtn)]
        void OnCameraBtnClicked(object sender, EventArgs e)
        {
            if (PermissionHelpers.NeedPermissionsCamera(this))
            {
                PermissionHelpers.RequestPermissionssCamera(this);
            }
            else
            {
                _cameraHelpers.TakeAPicture(_cameraHelpers);
            }
        }      


        private bool _Edit = false;

		private BigDaysItemModel _Item;
		private int _ID;
	
		const int TIME_DIALOG_ID = 0;
		const int DATE_DIALOG_ID = 1;
	
		private LocationPicture _ImageStorageNum;
		private string _ImgPath;
		private string _ImageBase64;

		private EditText _UiName;
		private ImageView _ImageArea;

		private EditText _UiTimeEdit;
		private EditText _UiDateEdit;

		private ImageButton _UiSeveOrEdit;
		private ImageButton _UiCancelOrDelete;

		private string _AlertStr = "1;0#2;0#3;0#4;0#5;0";
		private string _AlertStrOld = "1;0#2;0#3;0#4;0#5;0";

		private EditText _UiEditRepeat;
		private int _RepeatNum = 0;
		private string[] _RepeatStrs = { "None", "Daily", "Weekly", "Monthly", "Yearly" };

		private Button _UiAlerts;

		private int _Notification;

		private int hour;
		private int minute;
		private int seconds;
		private DateTime date;

		private int[] garbage = new int[6];
		private CameraHelpers _cameraHelpers;
		NotificationManager _NM;
        
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);	
			if (resultCode == Result.Ok )
			{
				switch (requestCode)
				{
					case 0:
						if (data == null) break;
#if _TRIAL_
					int imgPos = data.GetIntExtra ("pos", 0);
					if( imgPos > 1 ){
						AlertDialog.Builder builder;
						builder = new AlertDialog.Builder (this);
						builder.SetTitle("Free Version");
						builder.SetMessage("You can select only 2 first images in free version. Please purchase full version (no ad banner) to select any images.\n\nBy clicking \"Buy Now\" you will redirect to Full version (no ad banner) purchase page.");
						builder.SetCancelable(false);
						builder.SetPositiveButton("Buy Now", delegate { 
							Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Constants.VersionLink));
							StartActivity(browserIntent);
						});
						builder.SetNegativeButton("Continue", delegate {  });
						builder.Show();
					}else{
						_ImgPath = data.GetStringExtra ("image");
						try{
							Resources res = Resources;
							int imageID = res.GetIdentifier (_ImgPath.Replace(".jpg", ""), "drawable", PackageName);
							_ImageArea.SetImageDrawable (res.GetDrawable(imageID));
						} catch (OutOfMemoryError em) {
							AlertDialog.Builder builder;
							builder = new AlertDialog.Builder(this);
							builder.SetTitle("Error");
							builder.SetMessage("Out Of Memory Error №98");
							builder.SetCancelable(false);
							builder.SetPositiveButton("OK", delegate { Finish(); });
							builder.Show();
						}
						_ImageStorageNum = LocationPicture.ResourcesImage;
					}
#else
                        _ImgPath = data.GetStringExtra("image");
						try
						{
							Resources res = Resources;
							int imageID = res.GetIdentifier(_ImgPath.Replace(".jpg", ""), "drawable", PackageName);
							_ImageArea.SetImageDrawable(res.GetDrawable(imageID));
						}
						catch (OutOfMemoryError )
						{
							AlertDialog.Builder builder;
							builder = new AlertDialog.Builder(this);
							builder.SetTitle("Error CodeBlock №99");
							builder.SetMessage("Out Of Memory Error");
							builder.SetCancelable(false);
							builder.SetPositiveButton("OK", delegate { Finish(); });
							builder.Show();
						}
						_ImageStorageNum = LocationPicture.ResourcesImage;
						#endif
						break;
					case (int) RequestCode.PickImage:
                        try
                        {
                            if (data == null) break;
							var imagesURl = data.GetStringExtra("image");

							Constants.ImageBtmUri = string.IsNullOrEmpty(imagesURl) ? data.DataString : imagesURl;

						    var IntentShareActivity = new Intent(this, typeof(ImagePreview));
						    StartActivityForResult(IntentShareActivity, (int)RequestCode.ReturnPickImagePath);
                        }
                        catch (OutOfMemoryError)
                        {
                            AlertDialog.Builder builder;
                            builder = new AlertDialog.Builder(this);
                            builder.SetTitle("Error CodeBlock №100");
                            builder.SetMessage("Out Of Memory Error");
                            builder.SetCancelable(false);
                            builder.SetPositiveButton("OK", delegate { Finish(); });
                            builder.Show();
                        }
                        break;

					case (int)RequestCode.ReturnPickImagePath:

						if (data != null)
						{
							string result = data.GetStringExtra("result");
							_ImageArea.SetImageBitmap(Constants.ImageBtm);
							_ImageStorageNum = LocationPicture.Base64Image;

							_ImageBase64 = BitmapToBase64Converter.BitmapToBase64(Constants.ImageBtm);
						}
						else 
						{
							AlertDialog.Builder builder;
							builder = new AlertDialog.Builder(this);
							builder.SetTitle("Error CodeBlock №66");
							builder.SetMessage("ReturnPickImagePath is null");
							builder.SetCancelable(false);
							builder.SetPositiveButton("OK", delegate { Finish(); });
							builder.Show();
						}
						break;
					case (int) RequestCode.CameraImage :
                        try
                        {
                            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
						    Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
						    mediaScanIntent.SetData(contentUri);
						    SendBroadcast(mediaScanIntent);

                            Constants.ImageBtmUri = contentUri.ToString();

						    var IntentShareActivity2 = new Intent(this, typeof(ImagePreview));
						    StartActivityForResult(IntentShareActivity2, (int)RequestCode.ReturnPickImagePath);
                        }
                        catch (OutOfMemoryError)
                        {
                            AlertDialog.Builder builder;
                            builder = new AlertDialog.Builder(this);
                            builder.SetTitle("Error CodeBlock №101");
                            builder.SetMessage("Out Of Memory Error");
                            builder.SetCancelable(false);
                            builder.SetPositiveButton("OK", delegate { Finish(); });
                            builder.Show();
                        }
                        break;
					case (int) RequestCode.Repeat:
						_RepeatNum = data.GetIntExtra("Num", 0);
						_UiEditRepeat.Text = _RepeatStrs[_RepeatNum];
						break;
					case (int) RequestCode.Alerts: 
						_AlertStr = data.GetStringExtra("Alert");
						break;				
					default:
						break;
				}
			}
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.NewBigDays);

			Geneticist.Splice(this);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			ActionBar.Title = "Edit Big Days";

			_cameraHelpers = new CameraHelpers(this);
			MainActivity._BDDB.CheckRepeats();
			_Edit = Intent.GetBooleanExtra("Edit", false);

			_ImageArea = FindViewById<ImageView>(Resource.Id.imageArea);
			_ImageArea.SetScaleType(ImageView.ScaleType.CenterCrop);


			_UiTimeEdit = FindViewById<EditText>(Resource.Id.timeEdit);
			_UiTimeEdit.Click += (o, e) => ShowDialog(TIME_DIALOG_ID);

			_UiDateEdit = FindViewById<EditText>(Resource.Id.dateEdit);
			_UiDateEdit.Click += (o, e) => { ShowDialog(DATE_DIALOG_ID); };

			_UiEditRepeat = FindViewById<EditText>(Resource.Id.editRepeat);
			_UiEditRepeat.Click += (sender, e) =>
			{
				var IntentRepeatSelect = new Intent(this, typeof(RepeatSelect));
				IntentRepeatSelect.PutExtra("Num", _RepeatNum);
				StartActivityForResult(IntentRepeatSelect,(int)RequestCode.Repeat);
			};

			_ImgPath = "img17.jpg";
			_ImageStorageNum = LocationPicture.ResourcesImage;

			// Get the current time
			DateTime now = DateTime.Now;
			now = now.AddHours(1);
			hour = now.Hour;
			minute = DateTime.Now.Minute;
			seconds = DateTime.Now.Second;

			// get the current date
			date = DateTime.Today;

			_UiName = FindViewById<EditText>(Resource.Id.BigDayName);

			if (_Edit)
			{
				_ID = Intent.GetIntExtra("ID", 0);
				if (_ID != 0)
				{
					_Item = MainActivity._BDDB.SelectItem(_ID);

					_UiName.Text = _Item._Name;

					// Get the current time
					hour = _Item._EndDate.Hour;
					minute = _Item._EndDate.Minute;

					// get the current date
					date = _Item._EndDate;

					_RepeatNum = _Item._Repeat;
					_Notification = _Item._Notification;
					_UiEditRepeat.Text = _RepeatStrs[_RepeatNum].ToString();

					_AlertStr = _AlertStrOld = _Item._Alerts;

					_ImageStorageNum = (LocationPicture)_Item._ImageStorage;
					_ImgPath = _Item._Image;
                    _ImageBase64 = _Item.ImageBase64;

                    foreach (var i in MainActivity._BDitems)
						if (i._ID == _ID)
							_ImageArea.SetImageBitmap(i._BigImg);
				}
			}
			else {
				Resources res = Resources;
				int imageID = res.GetIdentifier(_ImgPath.Replace(".jpg", ""), "drawable", PackageName);
				Drawable def = new BitmapDrawable(BitmapHelpers.DecodeSampledBitmapFromResource(res, imageID, (int)MainActivity._DisplayWidth, (int)MainActivity._DisplayWidth, this));
				_ImageArea.SetImageDrawable(def);
			}

			_NM = (NotificationManager)GetSystemService(NotificationService);

			_UiAlerts = FindViewById<Button>(Resource.Id.Alerts);
			_UiAlerts.Click += (sender, e) =>
			{
				string[] alerts = _AlertStr.Split('#');
				int i = 0;
				foreach (var a in alerts)
				{
					string[] alertStr = a.Split(';');
					garbage[i] = 0;
					if (alertStr[1] == "1")
					{
						garbage[i] = Convert.ToInt32(alertStr[2]);
					}
					i++;
				}
				var IntentAlerts = new Intent(this, typeof(Alerts));
				IntentAlerts.PutExtra("Alert", _AlertStr);
				StartActivityForResult(IntentAlerts, (int)RequestCode.Alerts);
			};

			// Display the current date
			UpdateDisplayTime();

			// display the current date (this method is below)
			UpdateDisplayDate();

			_UiSeveOrEdit = FindViewById<ImageButton>(Resource.Id.SeveOrEdit);
			_UiSeveOrEdit.Click += (sender, e) =>
			{
				if (_Edit)
				{					
					_Item._ID = _ID;
					_Item._Name = _UiName.Text.ToString();
					DateTime d = Convert.ToDateTime(_UiDateEdit.Text.ToString());
					DateTime t = Convert.ToDateTime(_UiTimeEdit.Text.ToString());
					_Item._EndDate = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
					_Item._Image = _ImgPath;
					_Item._ImageStorage = (int)_ImageStorageNum;
					_Item._Repeat = _RepeatNum;
					_Item.ImageBase64 = _ImageBase64;
					int garbageMain = _Notification;

					_Item._Notification = new System.Random().Next(0, 999999);

					AlarmHelpers.UpdateAlarm(this, _Item, garbageMain);

					_Item._Alerts = _AlertStr;
					string[] alertOld = _AlertStrOld.Split('#');
					NotificationManager NM = (NotificationManager)GetSystemService(Context.NotificationService);
					foreach (var ao in alertOld)
					{
						string[] ao2 = ao.Split(';');
						if (ao2[1] == "1")
						{
							int ID = Convert.ToInt32(ao2[2]);
							NM.Cancel(ID);
						}
					}
					string[] alerts = _AlertStr.Split('#');
					int i = 0;
					foreach (var a in alerts)
					{
						string[] alertStr = a.Split(';');
						if (alertStr[1] == "1")
						{
							int ID = Convert.ToInt32(alertStr[2]);						
							AlarmHelpers.UpdateAlertsAlarm(this, _Item, ID, alertStr, garbage[i]);
						}

						i++;
					}
					MainActivity._BDDB.Update(_Item);
					Intent ParentIntent = new Intent(this, typeof(MainActivity));
					SetResult(Result.Ok, ParentIntent);
					Finish();
				}
				else {
					_Item = new BigDaysItemModel();
					_Item._Name = _UiName.Text.ToString();
					DateTime d = Convert.ToDateTime(_UiDateEdit.Text.ToString());
					DateTime t = Convert.ToDateTime(_UiTimeEdit.Text.ToString());
					_Item._EndDate = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
					_Item._Image = _ImgPath;
					_Item._ImageStorage = (int)_ImageStorageNum;
					_Item._Repeat = _RepeatNum;
					_Item.ImageBase64 = _ImageBase64;


					_Item._Notification = new System.Random().Next(0, 999999);
					_Item._ID= MainActivity._BDDB.GetLastID() + 1;

					AlarmHelpers.SetAlarm(this, _Item);

					_Item._Alerts = _AlertStr;
					string[] alerts = _AlertStr.Split('#');
					foreach (var a in alerts)
					{
						string[] alertStr = a.Split(';');
						if (alertStr[1] == "1")
						{
							int ID = Convert.ToInt32(alertStr[2]);
							_Item._ID= MainActivity._BDDB.GetLastID() + 1;

							AlarmHelpers.SetAlertsAlarm(this, _Item, ID, alertStr);						
						}
					}

					MainActivity._BDDB.Insert(_Item);

					List<BigDaysItemModel> items = MainActivity._BDDB.SelectBDItems();
					if (items.Count == 1)
						MainActivity._BDDB.SetActive(items[0]._ID);

					Intent ParentIntent = new Intent(this, typeof(ListActivity));
					SetResult(Result.Ok, ParentIntent);
					Finish();
				}
			};
			_UiCancelOrDelete = FindViewById<ImageButton>(Resource.Id.CancelOrDelete);
			if (_Edit)
				_UiCancelOrDelete.SetImageResource(Resource.Drawable.ic_action_discard);

			_UiCancelOrDelete.Click += (sender, e) =>
			{
				if (_Edit)
				{AlarmHelpers.RemoveAlarm(this, _Notification);

					foreach (var g in garbage)
						if (g != 0)
						{
							Intent IntentNot = new Intent(this, typeof(NotificationView));
							PendingIntent mAlarmSenderCansel = PendingIntent.GetBroadcast(this, g, IntentNot, PendingIntentFlags.UpdateCurrent);
							AlarmManager am = (AlarmManager)GetSystemService(Context.AlarmService);
							am.Cancel(mAlarmSenderCansel);
						}

					if (_ID != 0)
						MainActivity._BDDB.Delete(_ID);

					for (int i = 0; i < MainActivity._BDitems.Count; i++)
						if (MainActivity._BDitems[i]._ID == _ID)
							MainActivity._BDitems.RemoveAt(i);


					List<BigDaysItemModel> items = MainActivity._BDDB.SelectBDItems();
					if (items.Count > 0)
						MainActivity._BDDB.SetActive(items[0]._ID);

					Intent ParentIntent = new Intent(this, typeof(ListActivity));
					SetResult(Result.Ok, ParentIntent);
					Finish();
				}
				else {
					Intent ParentIntent = new Intent(this, typeof(ListActivity));
					SetResult(Result.Canceled, ParentIntent);
					Finish();
				}
			};

		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}

		protected override void OnRestoreInstanceState(Bundle savedState)
		{
			base.OnRestoreInstanceState(savedState);
		}


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
          
            if (requestCode == (int)Enums.RequestCode.PermissionsCameraAndWriteExternalStorage && grantResults[0] == Permission.Granted)
            {
                if (_cameraHelpers != null)
                {
                    _cameraHelpers.TakeAPicture(_cameraHelpers);
                }
                else
                {

                }
            }
            else
            {     var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Need Permissions Camera");
                    builder.SetMessage("To interact with the camera requires permission");
                    builder.SetCancelable(false);
                    builder.SetPositiveButton("OK", delegate { Finish(); });                   
                    builder.Show();               
            }
        }

        public string getRealPathFromURI(Android.Net.Uri contentUri)
		{
			string[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
			var cursor = ManagedQuery(contentUri, proj, null, null, null);
			int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
			cursor.MoveToFirst();
			return cursor.GetString(column_index);
		}

		public static long UnixTimestampFromDateTime(DateTime date)
		{
			long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
			unixTimestamp /= TimeSpan.TicksPerSecond;
			return unixTimestamp;
		}

		private void UpdateDisplayTime()
		{
			string time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
			_UiTimeEdit.Text = time;
		}

		private void UpdateDisplayDate()
		{
			_UiDateEdit.Text = date.ToString("d");
		}

		void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
		{
			this.date = e.Date;
			UpdateDisplayDate();
		}

		private void TimePickerCallback(object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			hour = e.HourOfDay;
			minute = e.Minute;
			UpdateDisplayTime();
		}

		protected override Dialog OnCreateDialog(int id)
		{
			if (id == TIME_DIALOG_ID)
				return new TimePickerDialog(this, TimePickerCallback, hour, minute, false);
			else if (id == DATE_DIALOG_ID)
				return new DatePickerDialog(this, OnDateSet, date.Year, date.Month - 1, date.Day);

			return null;
		}


		private string GetPathToImage(global::Android.Net.Uri uri)
		{
			string doc_id = "";
			using (var c1 = ContentResolver.Query(uri, null, null, null, null))
			{
				c1.MoveToFirst();
				string document_id = c1.GetString(0);
				doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
			}

			string path = null;

			// The projection contains the columns we want to return in our query.
			string selection = MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
			using (var cursor = ManagedQuery(MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null))
			{
				if (cursor == null) return path;
				var columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
				cursor.MoveToFirst();
				path = cursor.GetString(columnIndex);
			}
			return path;
		}
	}
}

