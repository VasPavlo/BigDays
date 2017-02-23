
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using System.Net;
using Android.Graphics;
using BigDays.Pixabay.Models;
using BigDays.Enums;
using Genetics.Attributes;
using Genetics;
using System;
using System.Collections.Generic;
using Android.Support.V4.Widget;

namespace BigDays.Pixabay
{
    [Activity(Theme = "@style/CustomActionBarTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PixabayView : Activity
	{
		private GridView gallery;
		private EditText searchText;
		private Activity _activity;
		private List<Hit> PixabayHit;
		private PixabayService pixabayService;

		[Splice(Resource.Id.pixabaySearchBtn)]
		private Button searchBtn;

		[SpliceClick(Resource.Id.pixabaySearchBtn)]
		void OnSearchClicked(object sender, EventArgs e)
		{
			Init(_activity, searchText.Text);
		}

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			_activity = this;

            SetContentView(Resource.Layout.PixabayImageGrid);

			Geneticist.Splice(this);

            gallery = FindViewById<GridView>(Resource.Id.gridImages);

			searchText = FindViewById<EditText>(Resource.Id.pixabaySearchText);

			pixabayService = new PixabayService();
			Init(_activity);
        }

		async void Init(Activity activity, string searchtext = "")
        {
			PixabayResult result = await pixabayService.Init(searchtext);
			PixabayHit = result.hits;
            if (result != null)
            {
                gallery.Adapter = new ImageAdapter(activity, result.hits);
                gallery.ItemClick += (sender, e) =>
                {
                    var element =  PixabayHit[e.Position];
					Constants.ImageBtmUri = element.webformatURL;

                    Intent ParentIntent = new Intent(this, typeof(NewBigDays));

					ParentIntent.PutExtra("image", element.webformatURL);

					SetResult(Result.Ok, ParentIntent);
					Finish();                
                };
            }
        }


		private class ImageAdapter : BaseAdapter
        {        

            private DisplayImageOptions options;

            private readonly Activity context;           

            public override int Count
            {
                get { return _pixabayResult.Count; }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return position;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            ViewHolder holder;
            private List<Hit> _pixabayResult;


            public ImageAdapter(Activity c, List<Hit> pixabayResult)
            {
                _pixabayResult = pixabayResult;

                context = c;
                options = new DisplayImageOptions.Builder()
                    .ShowImageOnLoading(Resource.Drawable.ic_stub)
                    .ShowImageForEmptyUri(Resource.Drawable.ic_empty)
                    .ShowImageOnFail(Resource.Drawable.ic_error)
				                                 .CacheInMemory(false)
				                                 .CacheOnDisk(false)
                    .ConsiderExifParams(true)
                    .BitmapConfig(Bitmap.Config.Rgb565)
                    .Build();
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ImageView imageView;

                View view = convertView;

                if (convertView == null)
                {                    
                    imageView = new ImageView(context);
                    view = context.LayoutInflater.Inflate(Resource.Layout.PixabayImageGridItem, parent, false);
                    holder = new ViewHolder();
                    holder.imageView = view.FindViewById<ImageView>(Resource.Id.image);
                    holder.progressBar = view.FindViewById<ProgressBar>(Resource.Id.progress);
                    holder.hit = _pixabayResult[position];
                    view.Tag = holder;                   
                }
                else
                {
                    holder = (ViewHolder)view.Tag;
                }

                ImageLoader.Instance.DisplayImage(
                    holder.hit.previewURL,                    
                   holder.imageView,
                   options,
                   new ImageLoadingListener(
                       loadingStarted: delegate
                       {
                           holder.progressBar.Progress = 0;
                           holder.progressBar.Visibility = ViewStates.Visible;
                       },
                       loadingComplete: delegate
                       {
                           holder.progressBar.Visibility = ViewStates.Gone;
                       },
                       loadingFailed: delegate
                       {
                           holder.progressBar.Visibility = ViewStates.Gone;
                       }),
                   new ImageLoadingProgressListener(
                       progressUpdate: (imageUri, _view, current, total) =>
                       {
                           holder.progressBar.Progress = (int)(100.0f * current / total);
                       }));

                return view;               
            }
        }

       
        internal class ViewHolder : Java.Lang.Object
        {
            internal ImageView imageView;
            internal ProgressBar progressBar;
            internal Hit hit;
         }
    }
}
