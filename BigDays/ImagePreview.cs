using System;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using BigDays.Controls;
using Java.IO;
using Java.Lang;
using UniversalImageLoader.Core;
using Genetics;
using Genetics.Attributes;
using UniversalImageLoader.Core.Listener;
using UniversalImageLoader.Core.Assist;

namespace BigDays
{
	[Activity(Theme = "@style/CustomActionBarTheme.ImagePreview", Label = "Image Preview", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ImagePreview : Activity
	{
		private Bitmap _bmp;
		private string _ImgPath;
		private string _imageName;
		private ImageHelpers _imageHelpers;
		private ScaleImageView _scaleImageView;
		private float _rotationDegrees = 0;


        [Splice(Resource.Id.btnSave)]
        private ImageButton btnSave;

        [SpliceClick(Resource.Id.btnSave)]
        void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                _bmp = ScreenShot(_scaleImageView);
                Constants.ImageBtm = _bmp;

				Intent ParentIntent = new Intent(this, typeof(NewBigDays));
				SetResult(Result.Ok, ParentIntent);
				Finish();

                //Intent returnIntent = new Intent();           
                //returnIntent.PutExtra("result", "");
                //SetResult(Result.Ok, returnIntent);
                //Finish();
            }
            catch (System.Exception ex)
            {
                AlertDialog.Builder builder;
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error CodeBloc №104");
				builder.SetMessage($"Error{ex.Message}");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { Finish(); });
                builder.Show();                
            }
        }


        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.ImagePreview);

            Geneticist.Splice(this);

            var helpRelaLayout = FindViewById<RelativeLayout>(Resource.Id.helpRelaLayout);

			helpRelaLayout.Click += (sender, e) => 
			{
				helpRelaLayout.Visibility = ViewStates.Gone;
			};
            _scaleImageView = FindViewById<ScaleImageView>(Resource.Id.scaleImageView);

          
			if (!string.IsNullOrEmpty(Constants.ImageBtmUri))
			{                
               var options = new DisplayImageOptions.Builder()
                   .ShowImageOnLoading(Resource.Drawable.ic_stub)
                   .ShowImageForEmptyUri(Resource.Drawable.ic_empty)
                   .ShowImageOnFail(Resource.Drawable.ic_error)
				                                    .CacheInMemory(false)
				                                    .CacheOnDisk(false)                  
                   .BitmapConfig(Bitmap.Config.Rgb565)
                   .Build();

                ImageLoader.Instance.DisplayImage(
                    Constants.ImageBtmUri,
                    _scaleImageView,
                    options,
                    new ImageLoadingListener(
                        loadingStarted: delegate
                        {
                            //spinner.Visibility = ViewStates.Visible;
                        },
                        loadingComplete: delegate
                        {
                            //spinner.Visibility = ViewStates.Gone;
                        },
                        loadingFailed: (imageUri, _view, failReason) =>
                        {
                            string message = null;
                            if (failReason.Type == FailReason.FailType.IoError)
                            {
                                message = "Input/Output error";
                            }
                            else if (failReason.Type == FailReason.FailType.DecodingError)
                            {
                                message = "Image can't be decoded";
                            }
                            else if (failReason.Type == FailReason.FailType.NetworkDenied)
                            {
                                message = "Downloads are denied";
                            }
                            else if (failReason.Type == FailReason.FailType.OutOfMemory)
                            {
                                message = "Out Of Memory error";
                            }
                            else
                            {
                                message = "Unknown error";
                            }

                            AlertDialog.Builder builder;
                            builder = new AlertDialog.Builder(this);
                            builder.SetTitle("Error CodeBloc №102/1");
                            builder.SetMessage("message");
                            builder.SetCancelable(false);
                            builder.SetPositiveButton("OK", delegate { Finish(); });
                            builder.Show();

                            //spinner.Visibility = ViewStates.Gone;
                        }));                
            }			
			else 
			{
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(this);
				builder.SetTitle("Error CodeBlock №102/2");
				builder.SetMessage("File not support");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
			}
		}

        public Bitmap ScreenShot(View view)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            view.Draw(canvas);
            return bitmap;
        }     

    }
}
