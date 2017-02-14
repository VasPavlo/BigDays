﻿using System;
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
using AppfireworksXamarin;
using System.IO;
using Android.Util;
using BigDays.Services;
using BigDays.DB;
using BigDays.Models;
using BigDays.Helpers;

namespace BigDays
{
	[Activity (Label = "BigDays", Theme = "@style/CustomActionBarTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]	
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
		public int _ActiveBD;
		private BigDaysItemModel _CurrentItem;
		private static string _CurrentImgStr;
		public static int _InfoBoxLeft = 0;
		public static int _InfoBoxTop = 0;
		public static int _InfoBoxRight = 0;
		public static int _InfoBoxBottom = 0;
		public static int _InfoBoxWidth = 0;
		public static int _InfoBoxHeight = 0;
		public static int _InfoBoxChangePosInd = 0; 
		public static int _FirstAppOpen = 0;
		const int LIST_ID = 0;
		const int EDIT_ID = 1;
		const int ADDNEW_ID = 2;
		public static List<AlarmManager> _amMains = new List<AlarmManager> (); 
		public static List<PendingIntent> _PIMains = new List<PendingIntent> ();
		public static List<BigDaysItemModel> _BDitems;
		public ImageView _trialImg;
		public InterstitialAd interstitialAds = null;
		protected AdView mAdView;


        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) => {
				System.IO.File.AppendAllText("tmp.txt", args.Exception.ToString());
			};
			AppDomain.CurrentDomain.UnhandledException += (s, e) => {
				Toast.MakeText(this, "Test", ToastLength.Short).Show();
			};
			Window.RequestFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.Main);

			_trialImg = FindViewById<ImageView>(Resource.Id.trialMainImg);
			_infoBoxControl = (InfoBoxControl)FindViewById(Resource.Id.NewInfoBoxControl);
			_timer = new System.Timers.Timer();  

			mAdView = FindViewById<Android.Gms.Ads.AdView>(Resource.Id.adView);
			mAdView.Visibility = ViewStates.Invisible;

#if _TRIAL_
			try
			{
				interstitialAds = new InterstitialAd(this);   // initializing interstitial ads 

				mAdView.Visibility = ViewStates.Visible;
				var adRequest = new Android.Gms.Ads.AdRequest.Builder();
					#if DEBUG
					adRequest.AddTestDevice("TEST_EMULATOR");
					#endif
				var build = adRequest.Build();
				mAdView.LoadAd(build);

				#if DEBUG
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
				int i = 5;
			}
#endif

            var trialMainImg = (ImageView)FindViewById (Resource.Id.trialMainImg);
			var shopping = FindViewById<ImageButton> (Resource.Id.shopping);

			#if _TRIAL_
			trialMainImg.Visibility = ViewStates.Visible;
			shopping.Visibility = ViewStates.Visible;
			Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Constants.VersionLink));
			trialMainImg.Click += (sender, e) => {
				StartActivity(browserIntent);
			};
			shopping.Click += (sender, e) => {
				StartActivity(browserIntent);
				LocalStorage.SetShowABS(false, this);
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

            _BDDB.CreateTable ();
			_BDDB.CheckRepeats ();
			_BDitems = MainActivity._BDDB.SelectBDItems ();
			for (int i = 0; i < _BDitems.Count; i++)
				if (_BDitems [i]._Active == 1)
					_ActiveBD = i;
			
			var Display = WindowManager.DefaultDisplay;
			_DisplayHeight = Display.Height;
			_DisplayWidth = Display.Width;
			_infoBoxControl.SetOnTouchListener(this);
			GlobalLayoutListener l = null;
			l = new GlobalLayoutListener (() => {
				if( _FirstAppOpen == 0 )
				{
					/*ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this); 
					_InfoBoxLeft = prefs.GetInt ("IBLeft", 0);
					_InfoBoxRight = prefs.GetInt ("IBRight", 0);
					_InfoBoxTop = prefs.GetInt ("IBTop", 0);
					_InfoBoxBottom = prefs.GetInt ("IBBottom", 0);
					_InfoBoxChangePosInd = prefs.GetInt ("IBChangePosInd", 0);*/
					RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
					infoBoxParams.LeftMargin = _BDitems[_ActiveBD]._PosLeft;
					infoBoxParams.TopMargin = _BDitems[_ActiveBD]._PosTop;
					if(  _BDitems[_ActiveBD]._ChangePos == 1 )
						_infoBoxControl.LayoutParameters = infoBoxParams;
					else 
					{
						RelativeLayout.LayoutParams infoBoxParamsDef = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
						infoBoxParamsDef.LeftMargin = 0;
						infoBoxParamsDef.AddRule(LayoutRules.CenterVertical);
						_infoBoxControl.LayoutParameters = infoBoxParamsDef;
					}

					_FirstAppOpen = 1;
				}
				_infoBoxControl.ViewTreeObserver.RemoveGlobalOnLayoutListener (l);
			});
			_infoBoxControl.ViewTreeObserver.AddOnGlobalLayoutListener (l);
			_MainLayout = (RelativeLayout) FindViewById(Resource.Id.main_layout);
			_MainImage = (ImageView)FindViewById (Resource.Id.mainImgBase);
			_MainImage.SetScaleType (ImageView.ScaleType.CenterCrop);
			int _ItemID = Intent.GetIntExtra ("ItemID", 0);
			if (_BDitems.Count > 0) {
				BitmapHelpers.LoadImages (this, _BDitems);
				_CurrentItem = _BDDB.GetCurrentItem ();
				if (_ItemID != 0)
					_CurrentItem._ID = _ItemID;
				_infoBoxControl.Visibility = ViewStates.Visible;
			} else {
				this.ShowDefImage ();
				_infoBoxControl.Visibility = ViewStates.Gone;
			}
			foreach( var i in _BDitems )
				if( i._ID == _CurrentItem._ID )
					this.ShowImage ( i );

			if( _infoBoxControl.Visibility != ViewStates.Gone )
				_infoBoxControl.Title =_CurrentItem._Name;


			var ui_showListButton = FindViewById<ImageButton> (Resource.Id.showListButton);
			ui_showListButton.Click += (sender, e) => {
				var IntentListActivity = new Intent(this, typeof(ListActivity));
				StartActivityForResult(IntentListActivity, LIST_ID);

				//IntentListActivity.
				LocalStorage.SetShowABS(false, this);
			};
			var ui_addBigDaysBtn = FindViewById<ImageButton> (Resource.Id.mainAddBigDays);
			ui_addBigDaysBtn.Click += (sender, e) => {
				#if _TRIAL_
				if( MainActivity._BDitems.Count == 1 ){
					AlertDialog.Builder builder;
					builder = new AlertDialog.Builder (this);
					builder.SetTitle("Free Version");
					builder.SetMessage("You can add only 1 Big Day item in free version. Please purchase full version to enable adding unlimited Big Days items and Facebook share function.\n\nBy clicking \"Buy Now\" you will redirect to Full version (no ad banner) purchase page.");
					builder.SetCancelable(false);
					builder.SetPositiveButton("Buy Now", delegate { 
						StartActivity(browserIntent);
					});
					builder.SetNegativeButton("Continue", delegate {  });
					builder.Show();
				}else{
					var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
					StartActivityForResult(IntentNewBigDaysActivity, ADDNEW_ID);
					LocalStorage.SetShowABS(false, this);
				}
				#else
				var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
				StartActivityForResult(IntentNewBigDaysActivity, ADDNEW_ID);
				#endif
			};

			var ui_Feedback = FindViewById<ImageButton> (Resource.Id.Feedback);
			ui_Feedback.Click += (sender, e) => {
				var IntentFeedbackActivity = new Intent(this, typeof(Feedback));
				StartActivity(IntentFeedbackActivity);
				LocalStorage.SetShowABS(false, this);
			};

			_infoBoxControl.EditBigDaysBtn.Click += (sender, e) => {
				var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
				IntentNewBigDaysActivity.PutExtra("Edit", true);
				IntentNewBigDaysActivity.PutExtra("ID", _CurrentItem._ID);
				StartActivityForResult(IntentNewBigDaysActivity, EDIT_ID);
				LocalStorage.SetShowABS(false, this);
			};

			_infoBoxControl.ShareBigDaysBtn.Click += (sender, e) => {
				var IntentShareActivity = new Intent(this, typeof(Share));
				IntentShareActivity.PutExtra("ID", _CurrentItem._ID);
				StartActivity(IntentShareActivity);
				LocalStorage.SetShowABS(false, this);
			};	

			if (_infoBoxControl.Visibility != ViewStates.Gone) {
				if (_BDitems[_ActiveBD]._ChangePos == 1) {
					RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
					infoBoxParams.LeftMargin = _BDitems[_ActiveBD]._PosLeft;
					infoBoxParams.TopMargin = _BDitems[_ActiveBD]._PosTop;

					_infoBoxControl.LayoutParameters = infoBoxParams;
				} else {
					RelativeLayout.LayoutParams infoBoxParamsDef = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
					infoBoxParamsDef.LeftMargin = 0;
					infoBoxParamsDef.AddRule(LayoutRules.CenterVertical);
					_infoBoxControl.LayoutParameters = infoBoxParamsDef;
				}
			}

			_TimerHandler = new Handler();
			UpdateGeneration ();
		}


		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if ((requestCode == LIST_ID || requestCode == EDIT_ID) && (resultCode == Result.Ok))
			{
				_BDDB.CheckRepeats();
				_CurrentItem = _BDDB.GetCurrentItem();
				_infoBoxControl.Visibility = ViewStates.Visible;
				if (requestCode == EDIT_ID)
				{
					for (int i = 0; i < _BDitems.Count; i++)
						if (_BDitems[i]._ID == _CurrentItem._ID)
						{
                            BigDaysItemModel item = _BDDB.SelectItem(_CurrentItem._ID);
							BitmapHelpers.LoadImage(this, item);
							_BDitems[i] = item;
							this.ShowImage(item);
							//this.Recreate();
						}
					if (_BDitems.Count == 0)
					{
						ShowDefImage();
						_infoBoxControl.Visibility = ViewStates.Gone;
					}
				}
				else {
					foreach (var i in _BDitems)
						if (i._ID == _CurrentItem._ID)
							this.ShowImage(i);
					//this.Recreate();
				}
				for (int i = 0; i < _BDitems.Count; i++)
					if (_BDitems[i]._Active == 1)
						_ActiveBD = i;

				if (_infoBoxControl.Visibility != ViewStates.Gone)
				{
					_infoBoxControl.Title = _CurrentItem._Name;

					if (_BDitems[_ActiveBD]._ChangePos == 1)
					{
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = _BDitems[_ActiveBD]._PosLeft;
						infoBoxParams.TopMargin = _BDitems[_ActiveBD]._PosTop;
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
					else {
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = 0;
						infoBoxParams.AddRule(LayoutRules.CenterVertical);
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
				}
			}
			else if ((requestCode == ADDNEW_ID) && (resultCode == Result.Ok))
			{
                BigDaysItemModel item = MainActivity._BDDB.GetLastAddItem();
				BitmapHelpers.LoadImage(this, item);
				MainActivity._BDitems.Add(item);
				_infoBoxControl.Visibility = ViewStates.Visible;
				if (MainActivity._BDitems.Count == 1)
				{
					this.ShowImage(MainActivity._BDitems[0]);
				}
				_CurrentItem = _BDDB.GetCurrentItem();
				for (int i = 0; i < _BDitems.Count; i++)
					if (_BDitems[i]._Active == 1)
						_ActiveBD = i;

				if (_infoBoxControl.Visibility != ViewStates.Gone)
				{
					if (_BDitems[_ActiveBD]._ChangePos == 1)
					{
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = _BDitems[_ActiveBD]._PosLeft;
						infoBoxParams.TopMargin = _BDitems[_ActiveBD]._PosTop;
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
					else {
						RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent - 30, RelativeLayout.LayoutParams.WrapContent);
						infoBoxParams.LeftMargin = 0;
						infoBoxParams.AddRule(LayoutRules.CenterVertical);
						_infoBoxControl.LayoutParameters = infoBoxParams;
					}
				}
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			#if _TRIAL_
			_timer.Interval = 5000;
			_timer.AutoReset = false;
			_timer.Start();
			#endif
		}

		protected override void OnStop ()
		{
			_timer.Stop ();
			base.OnStop ();
		}	

		void ShowImage(BigDaysItemModel item){
			//if (_CurrentImgStr != item._Image) {
			//_MainLayout.SetBackgroundDrawable (item._BigImg);
			try{
				_MainImage.SetImageBitmap(item._BigImg);
			} catch (OutOfMemoryError em) {
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

		void ShowDefImage(){
			try{
				_MainImage.SetImageResource(Resource.Drawable.Background);
			} catch (OutOfMemoryError em) {
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
			if (_infoBoxControl.Visibility != ViewStates.Gone) {
				if (_CurrentItem._Repeat != 0) {
					_CurrentItem = _BDDB.CheckRepeat (_CurrentItem);
				}

				TimeSpan ts = _CurrentItem._EndDate.Subtract (DateTime.Now);
				if (ts.Ticks < 0) 
				{
					_infoBoxControl.SetTextColorInAllTextView(Color.Red);
					_infoBoxControl.Days = string.Format("{0:0000}", -ts.Days);
					_infoBoxControl.Hours = string.Format("{0:00}", -ts.Hours);
					_infoBoxControl.Min = string.Format("{0:00}", -ts.Minutes);
					_infoBoxControl.Sec = string.Format("{0:00}", -ts.Seconds);
				} else {

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
			var right =  (int)(mEvent.RawX + (_InfoBoxWidth - _viewX));
			var top = _BDitems[_ActiveBD]._PosTop = (int)(mEvent.RawY - _viewY);
			var bottom = _BDitems[_ActiveBD]._PosBottom = (int)(mEvent.RawY + (_InfoBoxHeight - _viewY));


			// get Screen Size
			var metrics = Resources.DisplayMetrics;
			var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
			var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);
			//-----------------
			switch (mEvent.Action) {
			case MotionEventActions.Down:
					
					last.Set(curr);

				_viewX = mEvent.GetX ();
				_viewY = mEvent.GetY ();
				if (_InfoBoxWidth == 0 && _InfoBoxHeight == 0) {
						_InfoBoxWidth = (int)view.Width;
						_InfoBoxHeight = (int)view.Height;
				}
				break;
					
			case MotionEventActions.Move:

					float deltaX = curr.X - last.X;
					float deltaY = curr.Y - last.Y;
				


				_InfoBoxChangePosInd = 1;
				//v.Layout (left, top, right, bottom);
				RelativeLayout.LayoutParams infoBoxParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);

				if (left >= 0 && right <= _MainLayout.Width)
				{
					infoBoxParams.LeftMargin = _BDitems[_ActiveBD]._PosLeft;
					_BDitems[_ActiveBD]._PosLeft = left;
					_BDitems[_ActiveBD]._PosRight = right;
				}

				if(right>= _MainLayout.Width)
				{
					infoBoxParams.LeftMargin = _MainLayout.Width -_infoBoxControl.Width;	
					_BDitems[_ActiveBD]._PosLeft = left;
					_BDitems[_ActiveBD]._PosRight = right;
				}

				if(_BDitems[_ActiveBD]._PosTop>0)
					infoBoxParams.TopMargin = _BDitems[_ActiveBD]._PosTop;

					view.LayoutParameters = infoBoxParams;
				break;
					
			case MotionEventActions.Up:
				/*ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (this); 
				ISharedPreferencesEditor editor = prefs.Edit ();
				editor.PutInt ("IBLeft", _InfoBoxLeft);
				editor.PutInt ("IBRight", _InfoBoxRight);
				editor.PutInt ("IBTop", _InfoBoxTop);
				editor.PutInt ("IBBottom", _InfoBoxBottom);
				editor.PutInt ("IBChangePosInd", 1);
				//editor.Apply();
				editor.Commit ();*/
					
				_BDitems [_ActiveBD]._ChangePos = 1;
				_BDDB.UpdatePos(_BDitems[_ActiveBD]);
				break;
			}
			return true;
		}

		bool temp = true;
		protected override void OnPostResume()
		{
			base.OnPostResume();

				#if _TRIAL_
					//bool abs = LocalStorage.GetShowABS(this);
					//if (temp & abs)
					//{
						RunInterstitialer();
					//}
					//else
					//{
					//	temp = true;
					//	LocalStorage.SetShowABS(true, this);
					//}
				#endif
		}


		public void RunInterstitialer() 
		{
			try
			{
				//_timer = new System.Timers.Timer();
				//_timer.Interval = 300;
				//_timer.AutoReset = false;
				//#if _TRIAL_
				//_timer.Start();
				//#endif

				//_timer.Elapsed += delegate
				//{
					this.RunOnUiThread(() =>
				{
						//TODO:Issue #3
						//Admob ads settings change (full screen ad showing every time the go to the full screen countdown)
						//- Change settings Admob ads: Program it so that every time someone open the full screen countdown the full screen ad appears (now its programmed once a day per user?)

						//// 1. Load Date setting
						//DateTime getDate = DateTime.Now.AddDays(3);
						//var	prefsGet = this.GetSharedPreferences("BigDay.advertisting", FileCreationMode.Private);
						//if (prefsGet.Contains ("DateAdvertisting"))
						//{
						//	getDate =  DateTime.Parse(prefsGet.GetString ("DateAdvertisting", ""));
						//}
						//2. Run Interstitialer 
						//if (getDate.Day != DateTime.Now.Day)
						//{
						//var FinalAd = AdWrapper.ConstructFullPageAdd (this, Resources.GetString(Resource.String.adMob_api_interstitial_key));

						//	var intlistener = new MyAdListener();
						//	intlistener.AdLoaded += () =>
						//	{
						//		if (FinalAd.IsLoaded)
						//			FinalAd.Show ();
						//	};
						//	FinalAd.AdListener = intlistener;
						//	FinalAd.CustomBuild ();

						//intlistener.AdClosed+= () => 
						//{
						////3. Save Date in setting
						//var prefsSet = this.GetSharedPreferences("BigDay.advertisting", FileCreationMode.Private);
						//var editor = prefsSet.Edit ();
						//editor.PutString ("DateAdvertisting", DateTime.Now.ToString());
						//editor.Commit ();							 
						//};
						//}


					//	if (LocalStorage.GetShowABS(this) == true)
					//{
						//var FinalAd = AdWrapper.ConstructFullPageAdd(this, Resources.GetString(Resource.String.adMob_api_interstitial_key));

						//intlistener = new MyAdListener();
						//intlistener.AdLoaded += () =>
						//{
						//	if (FinalAd.IsLoaded)
						//		FinalAd.Show();
						//};
						//FinalAd.AdListener = intlistener;
						//FinalAd.CustomBuild();

						//intlistener.AdClosed += () =>
						// {
						//	 LocalStorage.SetShowABS(false, this);
						//	 temp = false;
						// };
					//}
				});


				//	};
			}
			catch (System.Exception e)
			{
				var ee = e;
			}
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
