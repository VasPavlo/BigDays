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
using Android.Graphics;

namespace BigDays
{
	public class RepeatListAdapter: BaseAdapter
	{
		private Activity _Context;
		private string[] _Items;
		private int _Checked;

		public RepeatListAdapter(Activity context, string[] items, int check = 0) : base() {
			this._Context = context;
			this._Items = items;
			this._Checked = check;
		}

		public override long GetItemId(int position){
			return position;
		}

		public override Java.Lang.Object GetItem(int position) {  
			return null; 
		}

		public override int Count {
			get { return _Items.Length; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			CheckedTextView _CheckedView;
			if (convertView == null) {
				_CheckedView = new CheckedTextView(_Context);
			} else {
				_CheckedView = (CheckedTextView) convertView;
			}

			// get screen dimensions
			if (_Checked == position) {
				_CheckedView.SetTextColor (Color.ParseColor ("#1a397a"));
			} else {
				_CheckedView.SetTextColor (Color.ParseColor ("#FFFFFF"));
			}

			_CheckedView.SetText(_Items[position].ToString(), TextView.BufferType.Normal);

			return _CheckedView;
		}
	}
}

