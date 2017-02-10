using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Timers;
using BigDays.Models;

namespace BigDays
{
	[Activity (Label = "Big Days", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class ListActivity : Activity
	{
		private ListView _UiBigDaysListView;
		private BigDaysListAdapter _BigDaysListAdapter;
		private Handler _TimerHandler;

		class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
		{
			public GlobalLayoutListener (System.Action onGlobalLayout)
			{
				this.onGlobalLayout = onGlobalLayout;
			}

			System.Action onGlobalLayout;

			public void OnGlobalLayout ()
			{
				onGlobalLayout ();
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if ((requestCode == 0) &&(resultCode == Result.Ok) && (data != null)) {
				List<BigDaysItemModel> items = MainActivity._BDDB.SelectBDItems ();
                BigDaysItemModel item = MainActivity._BDDB.GetLastAddItem ();
				BitmapHelpers.LoadImage (this, item);
				MainActivity._BDitems.Add( item );
				MainActivity._BDitems = MainActivity._BDitems.OrderBy(o=>o._EndDate).ToList();
				_BigDaysListAdapter.Clear ();
				_BigDaysListAdapter.Update (MainActivity._BDitems.ToArray());
				_BigDaysListAdapter.NotifyDataSetChanged();
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.List);
			_UiBigDaysListView = (ListView) FindViewById(Resource.Id.BigDaysListView);
			_BigDaysListAdapter = new BigDaysListAdapter (this, MainActivity._BDitems.ToArray());
			#if _TRIAL_
				View trial = this.LayoutInflater.Inflate (Resource.Layout.BigDaysListItemTrial, null);
				trial.Click += (sender, e) => {
				Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Constants.VersionLink));
					StartActivity(browserIntent);
				};
				_UiBigDaysListView.AddFooterView (trial);
			#endif
			_UiBigDaysListView.Adapter = _BigDaysListAdapter;
			_UiBigDaysListView.ItemClick += OnListItemClick; 
			var ui_addBigDaysBtn = FindViewById<ImageButton> (Resource.Id.addBigDays);
			ui_addBigDaysBtn.Click += (sender, e) => {
				#if _TRIAL_
					if( MainActivity._BDitems.Count == 1 ){
						AlertDialog.Builder builder;
						builder = new AlertDialog.Builder (this);
						builder.SetTitle("Free Version");
						builder.SetMessage("You can add only 1 Big Day item in free version. Please purchase full version to enable adding unlimited Big Days items and Facebook share function.\n\nBy clicking \"Buy Now\" you will redirect to Full version (no ad banner) purchase page.");
						builder.SetCancelable(false);
						builder.SetPositiveButton("Buy Now", delegate { 
							Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Constants.VersionLink));
							StartActivity(browserIntent);
						});
						builder.SetNegativeButton("Continue", delegate {  });
						builder.Show();
					}else{
						var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
						StartActivityForResult(IntentNewBigDaysActivity, 0);
					}
				#else
					var IntentNewBigDaysActivity = new Intent(this, typeof(NewBigDays));
					StartActivityForResult(IntentNewBigDaysActivity, 0);
				#endif
			};
			_TimerHandler = new Handler();
			UpdateGeneration ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
		}

		protected override void OnRestoreInstanceState(Bundle savedState)
		{
			base.OnRestoreInstanceState (savedState);
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = MainActivity._BDitems[e.Position];
			MainActivity._BDDB.SetActive ( t._ID );
			for (int i = 0; i < MainActivity._BDitems.Count; i++)
				MainActivity._BDitems [i]._Active = 0;
			MainActivity._BDitems [e.Position]._Active = 1;
			Intent ParentIntent = new Intent (this, typeof(MainActivity));
			SetResult (Result.Ok, ParentIntent);
			Finish();
		}

		void UpdateGeneration()
		{
			_BigDaysListAdapter.NotifyDataSetChanged();
			_TimerHandler.PostDelayed(UpdateGeneration, 1000);
		}
			
	}
}

