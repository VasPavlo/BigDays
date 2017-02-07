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

namespace BigDays
{
	[Activity (Label = "Select Repeat", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class RepeatSelect : Activity
	{
		private RepeatListAdapter _RepeatListAdapter;
		private ListView _RepeatList;
		private string[] _RepeatStrs = { "None", "Daily", "Weekly", "Monthly", "Yearly" };

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.RepeatSelect);
			var checkedNum = Intent.GetIntExtra ("Num", 0);
			_RepeatList = FindViewById<ListView> (Resource.Id.listRepeat);
			_RepeatList.SetPadding((int) _RepeatList.PaddingLeft, (int) _RepeatList.PaddingTop, (int) _RepeatList.PaddingRight, (int) 5);
			_RepeatListAdapter = new RepeatListAdapter (this, _RepeatStrs, checkedNum);
			_RepeatList.Adapter = _RepeatListAdapter;
			_RepeatList.ItemClick += OnListItemClick; 
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
			var t = _RepeatStrs[e.Position];
			Intent ParentIntent = new Intent (this, typeof(NewBigDays));
			ParentIntent.PutExtra ("Num", e.Position);
			SetResult (Result.Ok, ParentIntent);
			Finish();
		}
	}
}

