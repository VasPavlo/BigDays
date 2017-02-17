using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Graphics;
using Java.Lang;
using Android.Gms.Ads;
using System.IO;
using Android.Util;
using BigDays.Services;
using BigDays.DB;
using BigDays.Models;
using System.Linq;
using BigDays.Enums;

namespace BigDays
{
	[Activity(Label = "BigDays", Theme = "@style/CustomActionBarTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity, View.IOnTouchListener
	{
		private System.Timers.Timer _timer;
		RelativeLayout _MainLayout;
		ImageView _MainImage;
		InfoBoxControl _infoBoxControl;
		private float _viewX;
		private float _viewY;
		public static int _DisplayWidth;
		public static int _DisplayHeight;
		public static IBigDaysDB _BDDB;
		private Handler _TimerHandler;
		public BigDaysItemModel _ActiveBD;
		private BigDaysItemModel _CurrentItem;
		private static string _CurrentImgStr;
		public static int _InfoBoxLeft;
		public static int _InfoBoxTop;
		public static int _InfoBoxRight;
		public static int _InfoBoxBottom;
		public static int _InfoBoxWidth;
		public static int _InfoBoxHeight;
		public static int _InfoBoxChangePosInd;
		public static int _FirstAppOpen;
		public static List<AlarmManager> _amMains = new List<AlarmManager>();
		public static List<PendingIntent> _PIMains = new List<PendingIntent>();
		public static List<BigDaysItemModel> _BDitems;
		public ImageView _trialImg;
		public InterstitialAd interstitialAds = null;
		protected AdView mAdView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
			{
				System.IO.File.AppendAllText("tmp.txt", args.Exception.ToString());
			};

			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			{
				Toast.MakeText(this, "Test", ToastLength.Short).Show();
			};

			Window.RequestFeature(WindowFeatures.NoTitle);
			SetContentView(Resource.Layout.Main);

			_trialImg = FindViewById<ImageView>(Resource.Id.trialMainImg);
			_infoBoxControl = (InfoBoxControl)FindViewById(Resource.Id.NewInfoBoxControl);
			_timer = new System.Timers.Timer();

			mAdView = FindViewById<Android.Gms.Ads.AdView>(Resource.Id.adView);
			mAdView.Visibility = ViewStates.Invisible;

			_MainLayout = (RelativeLayout)FindViewById(Resource.Id.main_layout);
			_MainImage = (ImageView)FindViewById(Resource.Id.mainImgBase);
			_MainImage.SetScaleType(ImageView.ScaleType.CenterCrop);
#if _TRIAL_
			try
			{
				interstitialAds = new InterstitialAd(this);   // initializing interstitial ads 
				mAdView.Visibility = ViewStates.Visible;
				var adRequest = new Android.Gms.Ads.AdRequest.Builder();
#if DEBUG
				adRequest.AddTestDevice(Android.Gms.Ads.AdRequest.DeviceIdEmulator);//"TEST_EMULATOR"
#endif
				var build = adRequest.Build();
				mAdView.LoadAd(build);

#if DEBUG
				//Test
				interstitialAds.AdUnitId = "ca-app-pub-3940256099942544/1033173712";
#else
					interstitialAds.AdUnitId = Resources.GetString(Resource.String.adMob_api_interstitial_key);
#endif

				// loading test ad using adrequest
				interstitialAds.LoadAd(build);

				var ThisAdListener = new BigDays.AdListener(this);
				interstitialAds.AdListener = ThisAdListener;

				ThisAdListener.AdLoaded += () =>
				{
					var trialImg = FindViewById<ImageView>(Resource.Id.trialMainImg);
					trialImg.Visibility = ViewStates.Invisible;
					mAdView.Visibility = ViewStates.Visible;
				};
			}
			catch
			{
			}
#endif

			var trialMainImg = (ImageView)FindViewById(Resource.Id.trialMainImg);
			var shopping = FindViewById<ImageButton>(Resource.Id.shopping);

#if _TRIAL_
			trialMainImg.Visibility = ViewStates.Visible;
			shopping.Visibility = ViewStates.Visible;
			Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Constants.VersionLink));
			trialMainImg.Click += (sender, e) =>
			{
				StartActivity(browserIntent);
			};
			shopping.Click += (sender, e) =>
			{
				StartActivity(browserIntent);
			};
#else
			trialMainImg.Visibility = ViewStates.Gone;
			shopping.Visibility = ViewStates.Gone;
#endif


			long max_memory = Runtime.GetRuntime().MaxMemory();
			long total_memory = Runtime.GetRuntime().TotalMemory();

			new MigrationDB_OldInNew("BigDays.db3", "BigDaysNew.db3");

			_BDDB = new DataService();
			_BDDB.ConnectToDB("BigDaysNew.db3");

			//_BDDB = new BigDaysDB_Old();
			//_BDDB.ConnectToDB("BigDays.db3");

			_BDDB.CreateTable();
			_BDDB.CheckRepeats();
			_BDitems = MainActivity._BDDB.SelectBDItems();
			_ActiveBD = _BDitems.FirstOrDefault(x => x._Active == true);


			var Display = WindowManager.DefaultDisplay;
			_DisplayHeight = Display.Height;
			_DisplayWidth = Display.Width;
			_infoBoxControl.SetOnTouchListener(this);
			GlobalLayoutListener l = null;
			l = new GlobalLayoutListener(() =>
		   {
			   if (_FirstAppOpen == 0)
			   {
				   RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
				   infoBoxParams.LeftMargin = _ActiveBD._PosLeft;
				   infoBoxParams.TopMargin = _ActiveBD._PosTop;
				   if (_ActiveBD._ChangePos)
					   _infoBoxControl.LayoutParameters = infoBoxParams;
				   else
				   {
					   RelativeLayout.LayoutParams infoBoxParamsDef = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
					   infoBoxParamsDef.LeftMargin = 0;
					   infoBoxParamsDef.AddRule(LayoutRules.CenterVertical);
					   _infoBoxControl.LayoutParameters = infoBoxParamsDef;
				   }

				   _FirstAppOpen = 1;
			   }
			   _infoBoxControl.ViewTreeObserver.RemoveGlobalOnLayoutListener(l);
		   });
			_infoBoxControl.ViewTreeObserver.AddOnGlobalLayoutListener(l);

			int _ItemID = Intent.GetIntExtra("ItemID", 0);
			if (_BDitems.Count > 0)
			{
				BitmapHelpers.LoadImages(this, _BDitems);
				_CurrentItem = _BDDB.GetCurrentItem();
				if (_ItemID != 0)
					_CurrentItem._ID = _ItemID;

				ShowImage(_BDitems.FirstOrDefault(n => n._ID == _CurrentItem._ID));
				_infoBoxControl.Visibility = ViewStates.Visible;
			}
			else
			{
				ShowDefImage();
				_infoBoxControl.Visibility = ViewStates.Gone;
			}	
				

			if (_infoBoxControl.Visibility != ViewStates.Gone)
				_infoBoxControl.Title = _CurrentItem._Name;


			var ui_showListButton = FindViewById<ImageButton>(Resource.Id.showListButton);
			ui_showListButton.Click += (sender, e) =>
			{
				var IntentListActivity = new Intent(this, typeof(ListActivity));
			StartActivityForResult(IntentListActivity, (int)RequestCode.List_BigDays);
			};

			var ui_addBigDaysBtn = FindViewById<ImageButton>(Resource.Id.mainAddBigDays);
			ui_addBigDaysBtn.Click += (sender, e) =>
			{
#if _TRIAL_
				if (_BDitems.Count == 1)
				{
					AlertDialog.Builder builder;
					builder = new AlertDialog.Builder(this);
					builder.SetTitle("Free Version");
					builder.SetMessage("You can add only 1 Big Day item in free version. Please purchase full version to enable adding unlimited Big Days items and Facebook share function.\n\nBy clicking \"Buy Now\" you will redirect to Full version (no ad banner) purchase page.");
					builder.SetCancelable(false);
					builder.SetPositiveButton("Buy Now", delegate
					{
						StartActivity(browserIntent);
					});
					builder.SetNegativeButton("Continue", delegate { });
					builder.Show();
				}
				else {
					var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
					StartActivityForResult(IntentNewBigDaysActivity, (int)RequestCode.AddNew_BigDay);
				}
#else
				var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
			StartActivityForResult(IntentNewBigDaysActivity, (int)RequestCode.AddNew_BigDay);
#endif
			};

			var ui_Feedback = FindViewById<ImageButton>(Resource.Id.Feedback);
			ui_Feedback.Click += (sender, e) =>
			{
				var IntentFeedbackActivity = new Intent(this, typeof(Feedback));
				StartActivity(IntentFeedbackActivity);
			};

			_infoBoxControl.EditBigDaysBtn.Click += (sender, e) =>
			{
				var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
				IntentNewBigDaysActivity.PutExtra("Edit", true);
				IntentNewBigDaysActivity.PutExtra("ID", _CurrentItem._ID);
			StartActivityForResult(IntentNewBigDaysActivity, (int)RequestCode.Edit_BigDay);
			};

			_infoBoxControl.ShareBigDaysBtn.Click += (sender, e) =>
			{
				var IntentShareActivity = new Intent(this, typeof(Share));
				IntentShareActivity.PutExtra("ID", _CurrentItem._ID);
				StartActivity(IntentShareActivity);
			};

			if (_infoBoxControl.Visibility != ViewStates.Gone)
			{
				if (_ActiveBD._ChangePos)
				{
					RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
					infoBoxParams.LeftMargin = _ActiveBD._PosLeft;
					infoBoxParams.TopMargin = _ActiveBD._PosTop;

					_infoBoxControl.LayoutParameters = infoBoxParams;
				}
				else {
					RelativeLayout.LayoutParams infoBoxParamsDef = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
					infoBoxParamsDef.LeftMargin = 0;
					infoBoxParamsDef.AddRule(LayoutRules.CenterVertical);
					_infoBoxControl.LayoutParameters = infoBoxParamsDef;
				}
			}

			_TimerHandler = new Handler();
			UpdateGeneration();
		}


		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

		if ((requestCode == (int)RequestCode.List_BigDays || requestCode == (int)RequestCode.Edit_BigDay) && (resultCode == Result.Ok))
			{
				_BDDB.CheckRepeats();
				_CurrentItem = _BDDB.GetCurrentItem();
				_infoBoxControl.Visibility = ViewStates.Visible;
			if (requestCode == (int)RequestCode.Edit_BigDay)
				{
					for (int i = 0; i < _BDitems.Count; i++)
						if (_BDitems[i]._ID == _CurrentItem._ID)
							{
								BigDaysItemModel item = _BDDB.SelectItem(_CurrentItem._ID);
								BitmapHelpers.LoadImage(this, item);
								_BDitems[i] = item;
								ShowImage(item);
							}
						if (_BDitems.Count == 0)
						{
							ShowDefImage();
							_infoBoxControl.Visibility = ViewStates.Gone;
						}
				}
			else 
				{
					foreach (var i in _BDitems)
						if (i._ID == _CurrentItem._ID)
							ShowImage(i);
				}

				_ActiveBD = _BDitems.FirstOrDefault(x => x._Active == true);

				if (_infoBoxControl.Visibility != ViewStates.Gone)
				{
					_infoBoxControl.Title = _CurrentItem._Name;

					if (_ActiveBD._ChangePos)
					{
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = _ActiveBD._PosLeft;
						infoBoxParams.TopMargin = _ActiveBD._PosTop;
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
					else
					{
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = 0;
						infoBoxParams.AddRule(LayoutRules.CenterVertical);
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
				}
			}
			else if ((requestCode == (int)RequestCode.AddNew_BigDay) && (resultCode == Result.Ok))
			{
				BigDaysItemModel item = _BDDB.GetLastAddItem();
				BitmapHelpers.LoadImage(this, item);
				_BDitems.Add(item);
				_infoBoxControl.Visibility = ViewStates.Visible;
				if (_BDitems.Count == 1)
				{
					ShowImage(_BDitems[0]);
				}
				_CurrentItem = _BDDB.GetCurrentItem();

			_ActiveBD = _BDitems.FirstOrDefault(x => x._Active == true);

				if (_infoBoxControl.Visibility != ViewStates.Gone)
				{
					if (_ActiveBD._ChangePos)
					{
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = _ActiveBD._PosLeft;
						infoBoxParams.TopMargin = _ActiveBD._PosTop;
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
					else {
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent - 30, ViewGroup.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = 0;
						infoBoxParams.AddRule(LayoutRules.CenterVertical);
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
				}
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
#if _TRIAL_
			_timer.Interval = 5000;
			_timer.AutoReset = false;
			_timer.Start();
#endif
		}

		protected override void OnStop()
		{
			_timer.Stop();
			base.OnStop();
		}

		void ShowImage(BigDaysItemModel item)
		{
			//if (_CurrentImgStr != item._Image) {
			//_MainLayout.SetBackgroundDrawable (item._BigImg);
			try
			{
				_MainImage.SetImageBitmap(item._BigImg);
			}
			catch (OutOfMemoryError)
			{
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(this);
				builder.SetTitle("Error");
				builder.SetMessage("Out Of Memory Error");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
			}
			_CurrentImgStr = item._Image;
			//}		
		}

		void ShowDefImage()
		{
			try
			{
				_MainImage.SetImageResource(Resource.Drawable.Background);
			}
			catch (OutOfMemoryError em)
			{
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(this);
				builder.SetTitle("Error");
				builder.SetMessage("Out Of Memory Error");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
			}
		}

		void UpdateGeneration()
		{
			if (_infoBoxControl.Visibility != ViewStates.Gone)
			{
				if (_CurrentItem._Repeat != 0)
				{
					_CurrentItem = _BDDB.CheckRepeat(_CurrentItem);
				}

				TimeSpan ts = _CurrentItem._EndDate.Subtract(DateTime.Now);
				if (ts.Ticks < 0)
				{
					_infoBoxControl.SetTextColorInAllTextView(Color.Red);
					_infoBoxControl.Days = string.Format("{0:0000}", -ts.Days);
					_infoBoxControl.Hours = string.Format("{0:00}", -ts.Hours);
					_infoBoxControl.Min = string.Format("{0:00}", -ts.Minutes);
					_infoBoxControl.Sec = string.Format("{0:00}", -ts.Seconds);
				}
				else {

					_infoBoxControl.SetTextColorInAllTextView(Color.White);
					_infoBoxControl.Days = string.Format("{0:0000}", ts.Days);
					_infoBoxControl.Hours = string.Format("{0:00}", ts.Hours);
					_infoBoxControl.Min = string.Format("{0:00}", ts.Minutes);
					_infoBoxControl.Sec = string.Format("{0:00}", ts.Seconds);
				}
			}
			_TimerHandler.PostDelayed(UpdateGeneration, 1000);
		}

		private PointF last = new PointF();
		public bool OnTouch(View view, MotionEvent mEvent)
		{

			PointF curr = new PointF(mEvent.GetX(), mEvent.GetY());

			var left = (int)(mEvent.RawX - _viewX);
			var right = (int)(mEvent.RawX + (_InfoBoxWidth - _viewX));
			var top = _ActiveBD._PosTop = (int)(mEvent.RawY - _viewY);
			var bottom = _ActiveBD._PosBottom = (int)(mEvent.RawY + (_InfoBoxHeight - _viewY));

			// get Screen Size
			var metrics = Resources.DisplayMetrics;
			var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
			var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);
			//-----------------
			switch (mEvent.Action)
			{
				case MotionEventActions.Down:

					last.Set(curr);

					_viewX = mEvent.GetX();
					_viewY = mEvent.GetY();
					if (_InfoBoxWidth == 0 && _InfoBoxHeight == 0)
					{
						_InfoBoxWidth = (int)view.Width;
						_InfoBoxHeight = (int)view.Height;
					}
					break;

				case MotionEventActions.Move:

					float deltaX = curr.X - last.X;
					float deltaY = curr.Y - last.Y;

					_InfoBoxChangePosInd = 1;
					RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

					if (left >= 0 && right <= _MainLayout.Width)
					{
						infoBoxParams.LeftMargin = _ActiveBD._PosLeft;
						_ActiveBD._PosLeft = left;
						_ActiveBD._PosRight = right;
					}

					if (right >= _MainLayout.Width)
					{
						infoBoxParams.LeftMargin = _MainLayout.Width - _infoBoxControl.Width;
						_ActiveBD._PosLeft = left;
						_ActiveBD._PosRight = right;
					}

					if (_ActiveBD._PosTop > 0)
						infoBoxParams.TopMargin = _ActiveBD._PosTop;

					view.LayoutParameters = infoBoxParams;
					break;

				case MotionEventActions.Up:
					_ActiveBD._ChangePos = true;
					_BDDB.UpdatePos(_ActiveBD);
					break;
			}
			return true;
		}

		private int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
			return dp;
		}

		class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
		{
			public GlobalLayoutListener(System.Action onGlobalLayout)
			{
				this.onGlobalLayout = onGlobalLayout;
			}
			System.Action onGlobalLayout;
			public void OnGlobalLayout()
			{
				onGlobalLayout();
			}
		}
	}
}
