using Android.Content.Res;
using Android.App;
using Android.Graphics.Drawables;
using System.Collections.Generic;
using Java.Lang;

namespace BigDays
{
    using System;
    using System.IO;

    using Android.Graphics;
    using Android.Net;
    using Java.IO;
    using Models;

    public static class BitmapHelpers
    {
		public static int SAVE = 1;
		public static int InSampleSize = 0;


		public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);
            return resizedBitmap;
        }

		//public static BitmapFactory.Options getOptions(string uri)
		//{
		//	BitmapFactory.Options options = new BitmapFactory.Options();
		//	options.InJustDecodeBounds = true;
		//	try
		//	{
		//		BitmapFactory.DecodeFile(uri, options);

		//	}

		//	catch (Exception e2)
		//	{
		//		//e2.PrintStackTrace();
		//	}

		//	options.InSampleSize = getScale(options.OutWidth, options.OutHeight, (int)FinalPicture.getDisplayWidth(), (int)FinalPicture.getDisplayHeight());
		//	options.InJustDecodeBounds = false;
		//	return options;
		//}


		public static int getScale(int originalWidth, int originalHeight, int requiredWidth, int requiredHeight)
		{
			if (originalWidth <= requiredWidth && originalHeight <= requiredHeight)
			{
				return SAVE;
			}
			if (originalWidth < originalHeight)
			{
				return Java.Lang.Math.Round (((float)originalWidth) / ((float)requiredWidth));
			}

			var d = Convert.ToDouble(((float)originalHeight) / ((float)requiredHeight));		

			if (Math.Round((d - ((int)d)), 1) > 0)
			{
				return ((int)(((float)originalHeight) / ((float)requiredHeight)))+1;	
			}

			return Java.Lang.Math.Round(((float)originalHeight) / ((float)requiredHeight));
		}


		//public static Bitmap LoadBitmap(string uri)
		//{
		//	Bitmap returnBitmap = null;
		//	BitmapFactory.Options options = getOptions(uri);
		//	 returnBitmap =BitmapFactory.DecodeFile(uri, options);
		//	return returnBitmap;
		//}


		public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				}

			}

			return (int)inSampleSize;
		}

		public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, int reqWidth, int reqHeight, Activity context)
		{
			// First decode with inJustDecodeBounds=true to check dimensions
			var options = new BitmapFactory.Options {
				InJustDecodeBounds = true,
			};
			using (var dispose = BitmapFactory.DecodeResource(res, resId, options)) {
			}

			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

			try{
				return BitmapFactory.DecodeResource(res, resId, options);
			} catch (OutOfMemoryError em) {
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder (context);
				builder.SetTitle("Error");
				builder.SetMessage("Out Of Memory Error");
				builder.SetCancelable(false);
				builder.SetPositiveButton("OK", delegate { context.Finish(); });
				builder.Show();
			}
			return null;
		}

		public static void LoadImages(Activity context, List<BigDaysItemModel> items){
			foreach (var item in items) 
			{
				LoadImage(context, item);
			}
		}

		public static void LoadImage(Activity context, BigDaysItemModel item){
				switch (item._ImageStorage) {
				case 1:
					string path_main_bg_def = item._Image.Replace ("small", "");
					Resources res = context.Resources;
					int imageID = res.GetIdentifier (path_main_bg_def.Replace(".jpg", ""), "drawable", context.PackageName);
					int imageIDsmall = res.GetIdentifier (item._Image.Replace(".jpg", ""), "drawable", context.PackageName);
					item._BigImg = BitmapHelpers.DecodeSampledBitmapFromResource(res, imageID, (int)MainActivity._DisplayWidth, (int)MainActivity._DisplayWidth, context);
					item._SmallImg = BitmapHelpers.DecodeSampledBitmapFromResource(res, imageIDsmall,(int)70, (int)70, context );
					break;
				case 2:
				case 3:
					item._BigImg = BitmapHelpers.LoadAndResizeBitmap(item._Image,(int)MainActivity._DisplayWidth,(int)MainActivity._DisplayWidth);
					item._SmallImg = BitmapHelpers.LoadAndResizeBitmap (item._Image, 70, 70);
					break;
				}

		}

		public static Drawable LoadImages(string path){
			return new BitmapDrawable(BitmapHelpers.LoadAndResizeBitmap (path, (int)70, (int)70));
		}

    }
}
