
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BigDays.Models;

namespace BigDays
{
	public class BigDaysListFragment : Fragment
	{
		private ListView _UiBigDaysListView;
		private BigDaysListAdapter _BigDaysListAdapter;
		private Handler _TimerHandler;
		private View _view;
		private LayoutInflater _inflater;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);	
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			_view = inflater.Inflate(Resource.Layout.BigDaysListFragmentLayout, container, false);
			_inflater = inflater;
			Init();
			return _view;// base.OnCreateView(inflater, container, savedInstanceState);
		}


		void Init()
		{

			_UiBigDaysListView = (ListView)_view.FindViewById(Resource.Id.BigDaysListView);
			_BigDaysListAdapter = new BigDaysListAdapter(_inflater, MainActivity._BDitems.ToArray());
#if _TRIAL_
				View trial = _inflater.Inflate (Resource.Layout.BigDaysListItemTrial, null);
				trial.Click += (sender, e) => {
				Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Constants.VersionLink));
					StartActivity(browserIntent);
				};
				_UiBigDaysListView.AddFooterView (trial);
#endif
			_UiBigDaysListView.Adapter = _BigDaysListAdapter;
			_UiBigDaysListView.ItemClick += OnListItemClick;
			var ui_addBigDaysBtn = _view.FindViewById<ImageButton>(Resource.Id.addBigDays);
			ui_addBigDaysBtn.Click += (sender, e) =>
			{
#if _TRIAL_
					if( MainActivity._BDitems.Count == 1 ){
						AlertDialog.Builder builder;
						builder = new AlertDialog.Builder (_view.Context);
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
						var IntentNewBigDaysActivity = new Intent(_view.Context, typeof(NewBigDays));
						StartActivityForResult(IntentNewBigDaysActivity, 0);
					}
#else
				var IntentNewBigDaysActivity = new Intent(_view.Context, typeof(NewBigDays));
				StartActivityForResult(IntentNewBigDaysActivity, 0);
#endif
			};
			_TimerHandler = new Handler();
			UpdateGeneration();
		}


		public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if(resultCode == Result.Ok)
			Init();
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = MainActivity._BDitems[e.Position];
			MainActivity._BDDB.SetActive(t._ID);
			for (int i = 0; i < MainActivity._BDitems.Count; i++)
				MainActivity._BDitems[i]._Active = false;
			MainActivity._BDitems[e.Position]._Active = true;
			Intent ParentIntent = new Intent(_view.Context, typeof(MainActivity));
			StartActivityForResult(ParentIntent, 0);
			//SetResult(Result.Ok, ParentIntent);
			//Finish();
		}

		void UpdateGeneration()
		{
			_BigDaysListAdapter.NotifyDataSetChanged();
			_TimerHandler.PostDelayed(UpdateGeneration, 1000);
		}
	}
}
