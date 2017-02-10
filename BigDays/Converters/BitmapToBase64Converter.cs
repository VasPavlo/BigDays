using System;
using Android.Graphics;
using System.IO;
using Android.Util;

namespace BigDays.Converters
{
    public static class BitmapToBase64Converter
    {
        public static string BitmapToBase64(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                using (var stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);

                    var bytes = stream.ToArray();
                    var str = Convert.ToBase64String(bytes);
                    return str;
                }
            }
            else return "";
        }


        public static Bitmap Base64ToBitmap(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                Bitmap.Config conf = Bitmap.Config.Argb8888; // see other conf types
                Bitmap bmp = Bitmap.CreateBitmap(100, 100, conf); // this creates a MUTABLE bitmap
            }

            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }
    }
}
