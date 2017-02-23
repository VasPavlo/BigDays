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

namespace BigDays.Pixabay.Models
{
    public class PixabayResult
    {
        public int totalHits { get; set; }
        public List<Hit> hits { get; set; }
        public int total { get; set; }
    }

    public class Hit
    {
        public int previewHeight { get; set; }
        public int likes { get; set; }
        public int favorites { get; set; }
        public string tags { get; set; }
        public int webformatHeight { get; set; }
        public int views { get; set; }
        public int webformatWidth { get; set; }
        public int previewWidth { get; set; }
        public int comments { get; set; }
        public int downloads { get; set; }
        public string pageURL { get; set; }
        public string previewURL { get; set; }
        public string webformatURL { get; set; }
        public int imageWidth { get; set; }
        public int user_id { get; set; }
        public string user { get; set; }
        public string type { get; set; }
        public int id { get; set; }
        public string userImageURL { get; set; }
        public int imageHeight { get; set; }
    }
}