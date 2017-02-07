
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BigDays
{
	public class InfoBoxControl : RelativeLayout
	{
		LayoutInflater mInflater;
		TextView _countDownTitle;
		TextView _countDownDays;
		TextView _countDownHours;
		TextView _countDownMin;
		TextView _countDownSec;
		public ImageButton EditBigDaysBtn;
		public ImageButton ShareBigDaysBtn;
		RelativeLayout _newInfoBox;


		public InfoBoxControl(Context context) : base(context)
		{
			mInflater = LayoutInflater.From(context);
			Init();
		}

		public InfoBoxControl(Context context, IAttributeSet attrs):base( context, attrs)
		{
			mInflater = LayoutInflater.From(context);
			Init();
		}

		public InfoBoxControl(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) :base(context, attrs ,defStyleAttr ,defStyleRes)
		{
			mInflater = LayoutInflater.From(context);
			Init();
		}	

		void Init()
		{
			View v = mInflater.Inflate(Resource.Layout.InfoBoxViewControl,this,true);
			_newInfoBox =  FindViewById<RelativeLayout>(Resource.Id.NewInfoBox);
			_countDownTitle = FindViewById<TextView>(Resource.Id.NewCountDownTitle);
			_countDownDays = FindViewById<TextView>(Resource.Id.CountDownDays);
			_countDownHours = FindViewById< TextView > (Resource.Id.CountDownHours);
			_countDownMin = FindViewById<TextView>(Resource.Id.CountDownMin);
			_countDownSec = FindViewById<TextView>(Resource.Id.CountDownSec);
			EditBigDaysBtn = FindViewById<ImageButton>(Resource.Id.editBigDaysBtn);
			ShareBigDaysBtn = FindViewById<ImageButton>(Resource.Id.shareBigDaysBtn);
		}

		public string Title
		{
			get { return _countDownTitle.Text;}
			set { _countDownTitle.Text = value;}
		}

		public string Days
		{
			get { return _countDownDays.Text; }
			set { _countDownDays.Text = value; }
		}

		public string Hours
		{
			get { return _countDownHours.Text; }
			set { _countDownHours.Text = value; }
		}

		public string Min
		{
			get { return _countDownMin.Text; }
			set { _countDownMin.Text = value; }
		}

		public string Sec
		{
			get { return _countDownSec.Text; }
			set { _countDownSec.Text = value; }
		}
	
		public void SetTextColorInAllTextView(Color color)
		{
			_countDownDays.SetTextColor(color);
			_countDownHours.SetTextColor(color);
			_countDownMin.SetTextColor(color);
			_countDownSec.SetTextColor(color);
		}
  }
}
