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
using SQLite;
using Android.Graphics;

namespace BigDays.Models
{
    public class BigDaysItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int _ID { get; set; }
        public string _Name { get; set; }
        public int _Notification { get; set; }
        public DateTime _EndDate { get; set; }
        public string _Image { get; set; }
        public int _ImageStorage { get; set; }
        public int _Repeat { get; set; }
        public string _Alerts { get; set; }
        public bool _IsSetImage = false;
        public int _Active { get; set; }
        public string ImageBase64 { get; set; }
        [Ignore]
        public Bitmap _BigImg { get; set; }
        [Ignore]
        public Bitmap _SmallImg { get; set; }
        public int _PosTop { get; set; }
        public int _PosLeft { get; set; }
        public int _PosRight { get; set; }
        public int _PosBottom { get; set; }
        public int _ChangePos { get; set; }
        public bool empty { get; set; }
    }
}