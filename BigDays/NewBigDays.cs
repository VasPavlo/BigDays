﻿using System;
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

namespace BigDays
{
	[Activity(Theme = "@style/CustomActionBarTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class NewBigDays : Activity
	{
		[Splice(Resource.Id.photo_AlbumBtn)]
		private Button photo_AlbumBtn;

		[SpliceClick(Resource.Id.photo_AlbumBtn)]
		void OnPhotoClicked(object sender, EventArgs e)
		{
            Intent = new Intent();
			Intent.SetType("image/*");
			Intent.SetAction(Intent.ActionGetContent);
			Intent.AddCategory(Intent.CategoryOpenable);
			StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int) RequestCode.PickImage);
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

		private Button _UiDefImagesBtn;
		private Button _UiCameraBtn;

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
							builder.SetMessage("Out Of Memory Error");
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
							builder.SetTitle("Error");
							builder.SetMessage("Out Of Memory Error");
							builder.SetCancelable(false);
							builder.SetPositiveButton("OK", delegate { Finish(); });
							builder.Show();
						}
						_ImageStorageNum = LocationPicture.ResourcesImage;
						#endif
						break;
					case (int) RequestCode.PickImage:
						if (data == null) break;
						Constants.URI= data.Data;

						var config = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
						//Initialize ImageLoader with configuration.
						ImageLoader.Instance.Init(config);
						ImageLoader imageLoader = ImageLoader.Instance;
						Bitmap bmp = imageLoader.LoadImageSync(data.DataString);

						Constants.ImageBtm = bmp;

						_ImgPath = data.DataString;

						var IntentShareActivity = new Intent(this, typeof(ImagePreview));
						StartActivityForResult(IntentShareActivity, (int)RequestCode.ReturnPickImagePath);
						break;

					case (int)RequestCode.ReturnPickImagePath:

						if (data == null) break;

						string result = data.GetStringExtra("result");
						//Android.Net.Uri contentUri2 = Android.Net.Uri.FromFile(new File(result));
						//_ImgPath = contentUri2.Path;

						//var config2 = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
						////Initialize ImageLoader with configuration.
						//ImageLoader.Instance.Init(config2);
						//ImageLoader imageLoader2 = ImageLoader.Instance;
						//Bitmap bmp2 = imageLoader2.LoadImageSync(contentUri2.ToString());

						_ImageArea.SetImageBitmap(Constants.ImageBtm);
						_ImageStorageNum = LocationPicture.Base64Image;

						_ImageBase64 = BitmapToBase64Converter.BitmapToBase64(Constants.ImageBtm);

						break;
					case (int) RequestCode.CameraImage : 
						
						Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
						Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
						mediaScanIntent.SetData(contentUri);
						SendBroadcast(mediaScanIntent);

						var config2 = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
						//Initialize ImageLoader with configuration.
						ImageLoader.Instance.Init(config2);
						ImageLoader imageLoader3 = ImageLoader.Instance;
						Bitmap bmp2 = imageLoader3.LoadImageSync(contentUri.ToString());
						Constants.ImageBtm = bmp2;

						var IntentShareActivity2 = new Intent(this, typeof(ImagePreview));
						StartActivityForResult(IntentShareActivity2, (int)RequestCode.ReturnPickImagePath);
						//_ImgPath = _cameraHelpers._file.Path;
						//_ImageStorageNum = LocationPicture.FileImage;
						//try
						//{
						//	Drawable camera = BitmapHelpers.LoadImage(_ImgPath);
						//	_ImageArea.SetImageDrawable(camera);
						//}
						//catch (OutOfMemoryError)
						//{
						//	AlertDialog.Builder builder;
						//	builder = new AlertDialog.Builder(this);
						//	builder.SetTitle("Error");
						//	builder.SetMessage("Out Of Memory Error");
						//	builder.SetCancelable(false);
						//	builder.SetPositiveButton("OK", delegate { Finish(); });
						//	builder.Show();
						//}
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


			_UiDefImagesBtn = FindViewById<Button>(Resource.Id.defImagesBtn);
			_UiDefImagesBtn.Click += (sender, e) =>
			{
				var IntentDefaultImagesSelect = new Intent(this, typeof(DefaultImagesSelect));
				StartActivityForResult(IntentDefaultImagesSelect, 0);
			};

			if (_cameraHelpers.IsThereAnAppToTakePictures())
			{
				_cameraHelpers.CreateDirectoryForPictures();

				_UiCameraBtn = FindViewById<Button>(Resource.Id.CameraBtn);
				_UiCameraBtn.Click += _cameraHelpers.TakeAPicture;
			}

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
					string MainMessage = string.Format("BIG DAYS OF OUR LIVES COUNTDOWN\r\n{0}\r\n\r\nCongratulation!\r\n{0} has com.", _Item._Name);
					System.Random random = new System.Random();
					_Item._Notification = random.Next(0, 999999);
					DateTime d1Main = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					TimeSpan tsMain = new TimeSpan(_Item._EndDate.ToFileTimeUtc() - d1Main.Ticks);
					long whensysMain = (long)_Item._EndDate.ToUniversalTime().Subtract(d1Main).TotalMilliseconds;
					Intent IntentMainNot = new Intent(this, typeof(NotificationView));
					IntentMainNot.PutExtra("ID", _Item._Notification);
					IntentMainNot.PutExtra("ItemID", _Item._ID);
					IntentMainNot.PutExtra("Title", _Item._Name);
					IntentMainNot.PutExtra("Message", MainMessage);
					IntentMainNot.PutExtra("Garbage", garbageMain);
					PendingIntent mAlarmMainSender = PendingIntent.GetBroadcast(this, _Item._Notification, IntentMainNot, PendingIntentFlags.UpdateCurrent);
					PendingIntent mAlarmMainSenderCansel = PendingIntent.GetBroadcast(this, garbageMain, IntentMainNot, PendingIntentFlags.UpdateCurrent);
					AlarmManager amMain = (AlarmManager)GetSystemService(Context.AlarmService);
					amMain.Cancel(mAlarmMainSenderCansel);
					amMain.Set(AlarmType.RtcWakeup, whensysMain, mAlarmMainSender);
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
					string message = string.Format("{0} - {1}/{2}/{3} {4}:{5}", _Item._Name, _Item._EndDate.Day, _Item._EndDate.Month, _Item._EndDate.Year,
						_Item._EndDate.Hour, _Item._EndDate.Minute);
					int i = 0;
					foreach (var a in alerts)
					{
						string[] alertStr = a.Split(';');
						if (alertStr[1] == "1")
						{
							DateTime when = _Item._EndDate;
							switch (Convert.ToInt16(alertStr[4]))
							{
								case 0:
									when = _Item._EndDate.AddSeconds(-Convert.ToInt16(alertStr[3]));
									break;
								case 1:
									when = _Item._EndDate.AddMinutes(-Convert.ToInt16(alertStr[3]));
									break;
								case 2:
									when = _Item._EndDate.AddHours(-Convert.ToInt16(alertStr[3]));
									break;
								case 3:
									when = _Item._EndDate.AddDays(-Convert.ToInt16(alertStr[3]));
									break;
								case 4:
									when = _Item._EndDate.AddDays(-(Convert.ToInt16(alertStr[3]) * 7));
									break;
								case 5:
									when = _Item._EndDate.AddMonths(-Convert.ToInt16(alertStr[3]));
									break;
								case 6:
									when = _Item._EndDate.AddYears(-Convert.ToInt16(alertStr[3]));
									break;
							}
							DateTime d1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
							TimeSpan ts = new TimeSpan(when.ToFileTimeUtc() - d1.Ticks);
							long whensys = (long)when.ToUniversalTime().Subtract(d1).TotalMilliseconds;
							int ID = Convert.ToInt32(alertStr[2]);
							Intent IntentNot = new Intent(this, typeof(NotificationView));
							IntentNot.PutExtra("ID", ID);
							IntentNot.PutExtra("Title", _Item._Name);
							IntentNot.PutExtra("Message", message);
							IntentNot.PutExtra("ItemID", _Item._ID);
							PendingIntent mAlarmSender = PendingIntent.GetBroadcast(this, ID, IntentNot, PendingIntentFlags.UpdateCurrent);
							PendingIntent mAlarmSenderCansel = PendingIntent.GetBroadcast(this, garbage[i], IntentNot, PendingIntentFlags.UpdateCurrent);
							AlarmManager am = (AlarmManager)GetSystemService(Context.AlarmService);
							am.Cancel(mAlarmSenderCansel);
							am.Set(AlarmType.RtcWakeup, whensys, mAlarmSender);
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
					string MainMessage = string.Format("BIG DAYS OF OUR LIVES COUNTDOWN\r\n{0}\r\n\r\nCongratulation!\r\n{0} has com.", _Item._Name);
					System.Random random = new System.Random();
					_Item._Notification = random.Next(0, 999999);
					DateTime d1Main = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					TimeSpan tsMain = new TimeSpan(_Item._EndDate.ToFileTimeUtc() - d1Main.Ticks);
					long whensysMain = (long)_Item._EndDate.ToUniversalTime().Subtract(d1Main).TotalMilliseconds;
					Intent IntentMainNot = new Intent(this, typeof(NotificationView));
					int ItemID = MainActivity._BDDB.GetLastID() + 1;
					IntentMainNot.PutExtra("ID", _Item._Notification);
					IntentMainNot.PutExtra("ItemID", ItemID);
					IntentMainNot.PutExtra("Title", _Item._Name);
					IntentMainNot.PutExtra("Message", MainMessage);
					PendingIntent mAlarmMainSender = PendingIntent.GetBroadcast(this, _Item._Notification, IntentMainNot, PendingIntentFlags.UpdateCurrent);
					AlarmManager amMain = (AlarmManager)GetSystemService(Context.AlarmService);
					amMain.Set(AlarmType.RtcWakeup, whensysMain, mAlarmMainSender);
					_Item._Alerts = _AlertStr;
					string[] alerts = _AlertStr.Split('#');
					string message = string.Format("{0} - {1}/{2}/{3} {4}:{5}", _Item._Name, _Item._EndDate.Day, _Item._EndDate.Month, _Item._EndDate.Year,
						_Item._EndDate.Hour, _Item._EndDate.Minute);
					foreach (var a in alerts)
					{
						string[] alertStr = a.Split(';');
						if (alertStr[1] == "1")
						{
							DateTime when = _Item._EndDate;
							switch (Convert.ToInt16(alertStr[4]))
							{
								case 0:
									when = _Item._EndDate.AddSeconds(-Convert.ToInt16(alertStr[3]));
									break;
								case 1:
									when = _Item._EndDate.AddMinutes(-Convert.ToInt16(alertStr[3]));
									break;
								case 2:
									when = _Item._EndDate.AddHours(-Convert.ToInt16(alertStr[3]));
									break;
								case 3:
									when = _Item._EndDate.AddDays(-Convert.ToInt16(alertStr[3]));
									break;
								case 4:
									when = _Item._EndDate.AddDays(-(Convert.ToInt16(alertStr[3]) * 7));
									break;
								case 5:
									when = _Item._EndDate.AddMonths(-Convert.ToInt16(alertStr[3]));
									break;
								case 6:
									when = _Item._EndDate.AddYears(-Convert.ToInt16(alertStr[3]));
									break;
							}
							DateTime d1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
							TimeSpan ts = new TimeSpan(when.ToFileTimeUtc() - d1.Ticks);
							long whensys = (long)when.ToUniversalTime().Subtract(d1).TotalMilliseconds;

							int ID = Convert.ToInt32(alertStr[2]);

							Intent IntentNot = new Intent(this, typeof(NotificationView));
							IntentNot.PutExtra("ID", ID);
							IntentNot.PutExtra("ItemID", ItemID);
							IntentNot.PutExtra("Title", _Item._Name);
							IntentNot.PutExtra("Message", message);
							PendingIntent mAlarmSender = PendingIntent.GetBroadcast(this, ID, IntentNot, PendingIntentFlags.UpdateCurrent);
							AlarmManager am = (AlarmManager)GetSystemService(Context.AlarmService);
							am.Set(AlarmType.RtcWakeup, whensys, mAlarmSender);
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
				{
					Intent IntentMainNot = new Intent(this, typeof(NotificationView));
					PendingIntent mAlarmMainSenderCansel = PendingIntent.GetBroadcast(this, _Notification, IntentMainNot, PendingIntentFlags.UpdateCurrent);
					AlarmManager amMain = (AlarmManager)GetSystemService(Context.AlarmService);
					amMain.Cancel(mAlarmMainSenderCansel);

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

