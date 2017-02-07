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
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Graphics;
using Android.Provider;

namespace BigDays
{
	public class BigDaysListAdapter: BaseAdapter
	{
		BigDaysItem[] items;
		Activity context;

		public BigDaysListAdapter(Activity context, BigDaysItem[] items) : base() {
			this.context = context;
			this.items = items;
		}

		public override long GetItemId(int position){
			return position;
		}

		public override Java.Lang.Object GetItem(int position) {  
			return null; 
		}

		public override int Count {
			get { return items.Length; }
		}

		public void Clear(){
			Array.Clear(items, 0, items.Length);
		}

		public void Update(BigDaysItem[] items){
			this.items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available

			if (view == null) // otherwise create a new one
				view = context.LayoutInflater.Inflate (Resource.Layout.BigDaysListItem, null);

			//if (items [position]._Name.Length < 15)
				view.FindViewById<TextView> (Resource.Id.name).Text = items [position]._Name;
			/*else {
				string title = items [position]._Name.Substring (0, 12);
				title += "...";
				view.FindViewById<TextView> (Resource.Id.name).Text = title;
			}*/

			if (items [position]._Repeat != 0) {
				items [position] = MainActivity._BDDB.CheckRepeat (items [position]);
			}

			TimeSpan ts = items [position]._EndDate.Subtract (DateTime.Now);
			if (ts.Ticks < 0) {
				view.FindViewById<TextView> (Resource.Id.CountDownDays).SetTextColor (Color.Red);
				view.FindViewById<TextView> (Resource.Id.CountDownHours).SetTextColor (Color.Red);
				view.FindViewById<TextView> (Resource.Id.CountDownMin).SetTextColor (Color.Red);
				view.FindViewById<TextView> (Resource.Id.CountDownSec).SetTextColor (Color.Red);

				string DaysTo = String.Format ("{0:0000}", -ts.Days);
				view.FindViewById<TextView> (Resource.Id.CountDownDays).Text = DaysTo;
				string HoursTo = String.Format ("{0:00}", -ts.Hours);
				view.FindViewById<TextView> (Resource.Id.CountDownHours).Text = HoursTo;
				string MinTo = String.Format ("{0:00}", -ts.Minutes);
				view.FindViewById<TextView> (Resource.Id.CountDownMin).Text = MinTo;
				string SecTo = String.Format ("{0:00}", -ts.Seconds);
				view.FindViewById<TextView> (Resource.Id.CountDownSec).Text = SecTo;
			} else {
				view.FindViewById<TextView> (Resource.Id.CountDownDays).SetTextColor (Color.White);
				view.FindViewById<TextView> (Resource.Id.CountDownHours).SetTextColor (Color.White);
				view.FindViewById<TextView> (Resource.Id.CountDownMin).SetTextColor (Color.White);
				view.FindViewById<TextView> (Resource.Id.CountDownSec).SetTextColor (Color.White);

				string DaysTo = String.Format ("{0:0000}", ts.Days);
				view.FindViewById<TextView> (Resource.Id.CountDownDays).Text = DaysTo;
				string HoursTo = String.Format ("{0:00}", ts.Hours);
				view.FindViewById<TextView> (Resource.Id.CountDownHours).Text = HoursTo;
				string MinTo = String.Format ("{0:00}", ts.Minutes);
				view.FindViewById<TextView> (Resource.Id.CountDownMin).Text = MinTo;
				string SecTo = String.Format ("{0:00}", ts.Seconds);
				view.FindViewById<TextView> (Resource.Id.CountDownSec).Text = SecTo;
			}

			view.FindViewById<TextView> (Resource.Id.EndDate).Text = items [position]._EndDate.ToString ("dd/MM/yyyy hh:mm:ss tt");
			if (!items [position]._IsSetImage) {
				view.FindViewById<ImageView> (Resource.Id.ItemImage).SetScaleType (ImageView.ScaleType.CenterCrop);
				view.FindViewById<ImageView> (Resource.Id.ItemImage).SetImageBitmap (MainActivity._BDitems [position]._SmallImg);
			}

			return view;
		}
	}
}

