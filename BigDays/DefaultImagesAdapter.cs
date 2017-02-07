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
using Java.Lang;

namespace BigDays
{
	public class DefaultImagesAdapter: BaseAdapter
	{
		private Activity _Context;
		private string[] _SmallImagesList;
		private int _ColumnWidth;

		public DefaultImagesAdapter(Activity context, string[] items, int column_width) : base() {
			this._Context = context;
			this._SmallImagesList = items;
			this._ColumnWidth = column_width;
		}

		public override long GetItemId(int position){
			return position;
		}

		public override Java.Lang.Object GetItem(int position) {  
			return null; 
		}

		public override int Count {
			get { return _SmallImagesList.Length; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			ImageView imageView;
			if (convertView == null) {
				imageView = new ImageView(_Context);
			} else {
				imageView = (ImageView) convertView;
			}

			// get screen dimensions
			Resources res = _Context.Resources;
			int imageID = res.GetIdentifier (_SmallImagesList[position].Replace(".jpg", ""), "drawable", _Context.PackageName);
			Drawable img = res.GetDrawable (imageID);
			imageView.SetScaleType (ImageView.ScaleType.CenterCrop);
			imageView.LayoutParameters = new GridView.LayoutParams(_ColumnWidth, _ColumnWidth + 30);
			try{
				imageView.SetImageDrawable (img);
			} catch (OutOfMemoryError em) {
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(_Context);
				builder.SetTitle("Error");
				builder.SetMessage("Out Of Memory Error");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { /*Finish();*/ });
				builder.Show();
			}

			return imageView;
		}
			
			
	}
}

