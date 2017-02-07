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

namespace BigDays
{
	[Activity(Theme = "@style/Theme.ImagePreview", Label = "Image Preview", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ImagePreview : Activity
	{
		private Bitmap _bmp;
		private string _ImgPath;
		private string _imageName;
		private ImageHelpers _imageHelpers;
		private ScaleImageView _scaleImageView;
		private float _rotationDegrees = 0;

		protected override void OnCreate(Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.ImagePreview);

			var helpRelaLayout = FindViewById<RelativeLayout>(Resource.Id.helpRelaLayout);

			helpRelaLayout.Click += (sender, e) => 
			{
				helpRelaLayout.Visibility = ViewStates.Gone;
			}; 


			var btnRotate = FindViewById<ImageButton>(Resource.Id.btnRotate);
			//btnRotate.Click += (sender, e) => 
			//{
			//	try
			//	{
			//		_bmp = Settings.ImageBtm; //Compress(Settings.ImageBtm);

			//	Matrix matrix = new Matrix();
			//	matrix.PostRotate(_rotationDegrees +=90);

			//	_bmp = Bitmap.CreateBitmap(_bmp, 0, 0, this._bmp.Width, this._bmp.Height, matrix, true);
			//	_scaleImageView.SetImageBitmap(_bmp);

			//	var _timer2 = new System.Threading.Timer((o) =>
			//	 {
			//		 RunOnUiThread(() =>
			//		 {
			//			_bmp = _imageHelpers.BitmapFitScreenSize(_scaleImageView, _bmp);
			//			_scaleImageView.SetImageBitmap(_bmp);
			//		 });
			//	 }
			//	 , null, 30, 0);

			//	}
			//			catch (OutOfMemoryError)
			//{
			//	AlertDialog.Builder builder;
			//	builder = new AlertDialog.Builder(this);
			//	builder.SetTitle("Error");
			//	builder.SetMessage("Out Of Memory Error");
			//	builder.SetCancelable(false);
			//	builder.SetPositiveButton("OK", delegate { Finish(); });
			//	builder.Show();
			//}
			//};

			var btnSave = FindViewById<ImageButton>(Resource.Id.btnSave);
			btnSave.Click += (sender, e) =>
			{
				try
				{
					DisplayMetrics displaymetrics = new DisplayMetrics();
					WindowManager.DefaultDisplay.GetMetrics(displaymetrics);

					float WidthPixels = displaymetrics.WidthPixels;
					float HeightPixels = displaymetrics.HeightPixels;
					int x = ((int)(_scaleImageView.MtransX / _scaleImageView.Scale) * -1);
					int y = ((int)(_scaleImageView.MtransY / _scaleImageView.Scale) * -1);
					int Width = (int)(WidthPixels / _scaleImageView.Scale);
					int Height = (int)(HeightPixels / _scaleImageView.Scale);

					//TypedValue tv = new TypedValue();
					//if (Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true))
					//{
					//	var actionBarHeight = TypedValue.ComplexToDimensionPixelSize(tv.Data, Resources.DisplayMetrics);
					//}

					//Width = (Width > _bitmap.Width) ? _bitmap.Width : Width;
					//Height = (Height > _bitmap.Height) ? _bitmap.Height : Height;

					x = (x < 0) ? 0 : x;
					y = (y < 0) ? 0 : y;

					_bmp = Bitmap.CreateBitmap(_bmp, x, y, Width, Height);

					_bmp = Bitmap.CreateScaledBitmap(_bmp, (int)WidthPixels,(int) HeightPixels, true);
					//_scaleImageView.SetImageBitmap(_bmp);

					//var _timer2 = new System.Threading.Timer((o) =>
					// {
					//	 RunOnUiThread(() =>
					//	 {
					//		_scaleImageView.MinZoom();
					//	 });
					//	}
					// , null, 100, 0);


					var path=  saveToInternalStorage(_bmp);

					Intent returnIntent = new Intent();
					returnIntent.PutExtra("result", path);
					SetResult( Result.Ok, returnIntent);
					Finish();

				}
				catch (OutOfMemoryError)
				{
					AlertDialog.Builder builder;
					builder = new AlertDialog.Builder(this);
					builder.SetTitle("Error");
					builder.SetMessage("Out Of Memory Error");
					builder.SetCancelable(false);
					builder.SetPositiveButton("OK", delegate { Finish(); });
					builder.Show();
				}
				catch (System.Exception ex)
				{

				}
			};

			_scaleImageView = FindViewById<ScaleImageView>(Resource.Id.scaleImageView);		

			_imageHelpers = new ImageHelpers(this);

			var temp = _imageHelpers.getRealPathFromURI(Constants.URI);
			_ImgPath  = temp.Item2;
			_imageName = temp.Item1;


			if (Constants.ImageBtm != null)
			{

				_bmp = Compress(Constants.ImageBtm);

				_scaleImageView.SetImageBitmap(_bmp);

				var _timer = new System.Threading.Timer((o) =>
				 {
					 RunOnUiThread(() =>
					 {
						 _bmp = _imageHelpers.BitmapFitScreenSize(_scaleImageView, _bmp);
						 _scaleImageView.SetImageBitmap(_bmp);
					 });
				 }
				 , null, 30, 0);
			}
			else 
			{
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(this);
				builder.SetTitle("Error");
				builder.SetMessage("File not support");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
			}
		}

		private string saveToInternalStorage(Bitmap bitmapImage)
		{
			ContextWrapper cw = new ContextWrapper(this.ApplicationContext);
			// path to /data/data/yourapp/app_data/imageDir
			File directory = cw.GetDir("imageDir", FileCreationMode.Private);
			// Create imageDir
			File mypath = new File(directory, _imageName);

			try
			{
				 using (var os = new System.IO.FileStream(mypath.AbsolutePath, System.IO.FileMode.Create))
				{
					bitmapImage.Compress(Bitmap.CompressFormat.Png, 100, os);
				}
			}
			catch (System.Exception ex)
			{
				System.Console.Write(ex.Message);
			}


			return (string) mypath.AbsoluteFile;
		}
	

		Bitmap Compress(Bitmap btm , int quality = 80)
		{
			try
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					using (btm)
					{
						btm.Compress(Bitmap.CompressFormat.Jpeg, quality, stream);
						byte[] bitmapData = stream.ToArray();
						return BitmapFactory.DecodeByteArray(bitmapData, 0, bitmapData.Length);
					}
				}
			}
			catch (OutOfMemoryError)
			{
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(this);
				builder.SetTitle("Error");
				builder.SetMessage("Out Of Memory Error");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { Finish(); });
				builder.Show();
				return btm;
			}
		}


		//public static void initImageLoader(Context context)
		//{		
		//	var config = new ImageLoaderConfiguration.Builder(context);

		//	config.ThreadPriority(Thread.NormPriority - 2);
		//	config.DenyCacheImageMultipleSizesInMemory();
		//	config.DiskCacheFileNameGenerator(new Md5FileNameGenerator());
		//	config.DiskCacheSize(50 * 1024 * 1024);//50Mbi
		//	config.TasksProcessingOrder(QueueProcessingType.Lifo);
		//	config.WriteDebugLogs();
		
		//	ImageLoader.Instance.Init(config.Build());
		//}

	}
}
