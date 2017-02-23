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
using Newtonsoft.Json;

namespace BigDays.Pixabay.Models
{
    public class PixabayRequest
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("q")]
        public string Q { get; set; }
    }
}