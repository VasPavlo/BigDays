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
using Android.Content.Res;
using Android.Util;

namespace BigDays
{
	[Activity (Label = "Select from default images", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class DefaultImagesSelect : Activity
	{
		// Number of columns of Grid View
		private static int NUM_OF_COLUMNS = 3;
		// Gridview image padding
		private static int GRID_PADDING = 10; // in dp

		private GridView _UiDefImages;
		private DefaultImagesAdapter _DefImagesAdapter;
		private string[] _SmallImages = {"img1small.jpg", "img2small.jpg", "img3small.jpg", 
			"img4small.jpg", "img5small.jpg", "img6small.jpg", "img7small.jpg", 
			"img8small.jpg", "img9small.jpg", "img10small.jpg", "img11small.jpg", 
			"img12small.jpg", "img13small.jpg", "img14small.jpg", "img15small.jpg", 
			"img16small.jpg", "img17small.jpg"};

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.DefaultImagesSelect);
			// Create your application here

			_UiDefImages = (GridView) FindViewById(Resource.Id.DefImagesGrid);
			_UiDefImages.SetPadding((int) GRID_PADDING, (int) GRID_PADDING, (int) GRID_PADDING, (int) GRID_PADDING);

			var columnWidth = (int) ((this.getScreenWidth() - ((NUM_OF_COLUMNS + 1) * GRID_PADDING)) / NUM_OF_COLUMNS);
			_UiDefImages.SetColumnWidth(columnWidth);
			_UiDefImages.SetVerticalSpacing (GRID_PADDING);
			_DefImagesAdapter = new DefaultImagesAdapter (this, _SmallImages, columnWidth);
			_UiDefImages.Adapter = _DefImagesAdapter;
			_UiDefImages.ItemClick += OnListItemClick; 
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
			var t = _SmallImages[e.Position];
			Intent ParentIntent = new Intent (this, typeof(NewBigDays));
			ParentIntent.PutExtra ("pos", e.Position);
			ParentIntent.PutExtra ("image", t);
			SetResult (Result.Ok, ParentIntent);
			Finish();
		}

		public int getScreenWidth() {
			int columnWidth;
			var Display = WindowManager.DefaultDisplay;

			Point point = new Point();

			Display.GetSize(point);
			point.X = Display.Width;
			columnWidth = point.X;
			return columnWidth;
		}

	}
}

