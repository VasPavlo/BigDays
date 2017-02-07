using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Util;
using BigDays.Controls;

namespace BigDays
{
	public class ImageHelpers
	{
		private Activity _activity;

		private ImageHelpers(){}

		public ImageHelpers(Activity activity)
		{
			_activity = activity;
		}


		public Bitmap LoadImage(string uri)
		{
			BitmapFactory.Options options = new BitmapFactory.Options();
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeFile(uri, options);

			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 3;

			//if (outHeight > height || outWidth > width)
			//{
			//	inSampleSize = outWidth > outHeight
			//					   ? outHeight / height
			//					   : outWidth / width;
			//}

			// Now we will load the image and have BitmapFactory resize it for us.
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			return BitmapFactory.DecodeFile(uri, options);
		}


		public Bitmap LoadImage(Android.Net.Uri uri)
		{
			string pathUri = getRealPathFromURI(uri).Item2;
			return LoadImage(pathUri);
		}


		public Tuple<string, string> GetPathToImage(Android.Net.Uri uri)
		{
			string path = null;
			string nameFile = null;
			try
			{			
			string doc_id = "";
			using (var c1 = _activity.ContentResolver.Query(uri, null, null, null, null))
			{
				c1.MoveToFirst();
				string document_id = c1.GetString(0);
				doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
			}			

			// The projection contains the columns we want to return in our query.
			string selection = MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
			using (var cursor = _activity.ManagedQuery(MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null))
			{
				if (cursor == null) Tuple.Create(nameFile, path);
				var columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
				var columnIndexzz = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.DisplayName);
				cursor.MoveToFirst();
				path = cursor.GetString(columnIndex);

				nameFile = cursor.GetString(columnIndexzz);
			}

				}
			catch (Exception ex)
			{
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(_activity);
				builder.SetTitle("Error");
				builder.SetMessage("File not support.");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { });
				builder.Show();
			}
			return Tuple.Create(nameFile, path);
		}


		public Tuple<string, string> getRealPathFromURI(Android.Net.Uri contentUri)
		{
			string[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
			var cursor = _activity.ManagedQuery(contentUri, proj, null, null, null);
			int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
			//int column_DisplayName = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.DisplayName);

			cursor.MoveToFirst();

			var path =  cursor.GetString(column_index);
			var nameFile = "jnhguigh";//cursor.GetString(column_DisplayName);

			return Tuple.Create(nameFile, path);
		}



		public Bitmap Compress(Bitmap bitmap ,int quality = 80)
		{
			MemoryStream stream = new MemoryStream();
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, quality, stream);
			byte[] bitmapData = stream.ToArray();
			return BitmapFactory.DecodeByteArray(bitmapData, 0, bitmapData.Length);
		}


		public Bitmap BitmapFitScreenSize(ScaleImageView scaleImageView, Bitmap bitmap)
		{
			return BitmapFitScreenSize(scaleImageView, bitmap, Color.Black);
		}

		public Bitmap BitmapFitScreenSize(ScaleImageView scaleImageView, Bitmap bitmap, Color color )
		{
			DisplayMetrics displaymetrics = new DisplayMetrics();
			_activity.WindowManager.DefaultDisplay.GetMetrics(displaymetrics);

			var WidthPixels = ((float)displaymetrics.WidthPixels);
			var HeightPixels = (float)displaymetrics.HeightPixels;
			var x = ((int)(scaleImageView.MtransX / scaleImageView.Scale) * -1);
			var y = ((int)(scaleImageView.MtransY / scaleImageView.Scale) * -1);
			var Width = (int)(WidthPixels / scaleImageView.Scale);
			var Height = (int)(HeightPixels / scaleImageView.Scale);

			Bitmap target = Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888);

			Canvas canvas = new Canvas(target);
			Paint paint = new Paint();

			canvas.DrawColor(color);

			canvas.DrawBitmap(bitmap, 0, (int)((float)(target.Height / 2) - (float)(bitmap.Height / 2)), paint);

			return target;
		}

	}
}
