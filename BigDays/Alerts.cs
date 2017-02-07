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

namespace BigDays
{
	[Activity (Label = "Alerts", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class Alerts : Activity
	{
		private string _AlertStr;

		private TextView _UiAlertOneIndTitle;
		private ToggleButton _UiAlertOneInd;
		private EditText _UiAlertOneNum;
		private EditText _UiAlertOneStr;
		private int _AlertOneSelectStr;

		private TextView _UiAlertTwoIndTitle;
		private ToggleButton _UiAlertTwoInd;
		private EditText _UiAlertTwoNum;
		private EditText _UiAlertTwoStr;
		private int _AlertTwoSelectStr;

		private TextView _UiAlertThreeIndTitle;
		private ToggleButton _UiAlertThreeInd;
		private EditText _UiAlertThreeNum;
		private EditText _UiAlertThreeStr;
		private int _AlertThreeSelectStr;

		private TextView _UiAlertForeIndTitle;
		private ToggleButton _UiAlertForeInd;
		private EditText _UiAlertForeNum;
		private EditText _UiAlertForeStr;
		private int _AlertForeSelectStr;

		private TextView _UiAlertFiveIndTitle;
		private ToggleButton _UiAlertFiveInd;
		private EditText _UiAlertFiveNum;
		private EditText _UiAlertFiveStr;
		private int _AlertFiveSelectStr;

		//NotificationManager _NM;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Alerts);
			// Create your application here
			_AlertStr = Intent.GetStringExtra ("Alert");
			string[] StrValueNames = { "Secs", "Mins", "Hours", "Days", "Weeks", "Months", "Years" };

			_UiAlertOneIndTitle = FindViewById<TextView> (Resource.Id.alertOneIndTitle);
			_UiAlertOneInd = FindViewById<ToggleButton> (Resource.Id.alertOneInd);
			_UiAlertOneInd.Click += (sender, e) => {
				bool AlertOn = _UiAlertOneInd.Checked;

				if( AlertOn ){
					_UiAlertOneNum.Visibility = ViewStates.Visible;
					_UiAlertOneStr.Visibility = ViewStates.Visible;
					_UiAlertOneIndTitle.Text = "On";
				}else{
					_UiAlertOneNum.Visibility = ViewStates.Gone;
					_UiAlertOneStr.Visibility = ViewStates.Gone;
					_UiAlertOneIndTitle.Text = "Off";
				}
			};


			_UiAlertOneNum = FindViewById<EditText> (Resource.Id.alertOneNum);
			_UiAlertOneNum.Click += (sender, e) => {
				this.CreateNumberPickerDialog(bundle, 1, Convert.ToInt16 (_UiAlertOneNum.Text)).Show();
			};

			_UiAlertOneStr = FindViewById<EditText> (Resource.Id.alertOneStr);
			_UiAlertOneStr.Click += (sender, e) => {
				int ind = 1;
				foreach( var names in StrValueNames ){
					if( names == _UiAlertOneStr.Text )
						break;
					ind++;
				}
				this.CreateStringPickerDialog(bundle, 1, ind).Show();
			};

			//Two alert
			_UiAlertTwoIndTitle = FindViewById<TextView> (Resource.Id.alertTwoIndTitle);
			_UiAlertTwoInd = FindViewById<ToggleButton> (Resource.Id.alertTwoInd);
			_UiAlertTwoInd.Click += (sender, e) => {
				bool AlertOn = _UiAlertTwoInd.Checked;

				if( AlertOn ){
					_UiAlertTwoNum.Visibility = ViewStates.Visible;
					_UiAlertTwoStr.Visibility = ViewStates.Visible;
					_UiAlertTwoIndTitle.Text = "On";
				}else{
					_UiAlertTwoNum.Visibility = ViewStates.Gone;
					_UiAlertTwoStr.Visibility = ViewStates.Gone;
					_UiAlertTwoIndTitle.Text = "Off";
				}
			};


			_UiAlertTwoNum = FindViewById<EditText> (Resource.Id.alertTwoNum);
			_UiAlertTwoNum.Click += (sender, e) => {
				this.CreateNumberPickerDialog(bundle, 2, Convert.ToInt16 (_UiAlertTwoNum.Text)).Show();
			};

			_UiAlertTwoStr = FindViewById<EditText> (Resource.Id.alertTwoStr);
			_UiAlertTwoStr.Click += (sender, e) => {
				int ind = 1;
				foreach( var names in StrValueNames ){
					if( names == _UiAlertTwoStr.Text )
						break;
					ind++;
				}
				this.CreateStringPickerDialog(bundle, 2, ind).Show();
			};

			//Three alert
			_UiAlertThreeIndTitle = FindViewById<TextView> (Resource.Id.alertThreeIndTitle);
			_UiAlertThreeInd = FindViewById<ToggleButton> (Resource.Id.alertThreeInd);
			_UiAlertThreeInd.Click += (sender, e) => {
				bool AlertOn = _UiAlertThreeInd.Checked;

				if( AlertOn ){
					_UiAlertThreeNum.Visibility = ViewStates.Visible;
					_UiAlertThreeStr.Visibility = ViewStates.Visible;
					_UiAlertThreeIndTitle.Text = "On";
				}else{
					_UiAlertThreeNum.Visibility = ViewStates.Gone;
					_UiAlertThreeStr.Visibility = ViewStates.Gone;
					_UiAlertThreeIndTitle.Text = "Off";
				}
			};


			_UiAlertThreeNum = FindViewById<EditText> (Resource.Id.alertThreeNum);
			_UiAlertThreeNum.Click += (sender, e) => {
				this.CreateNumberPickerDialog(bundle, 3, Convert.ToInt16 (_UiAlertThreeNum.Text)).Show();
			};

			_UiAlertThreeStr = FindViewById<EditText> (Resource.Id.alertThreeStr);
			_UiAlertThreeStr.Click += (sender, e) => {
				int ind = 1;
				foreach( var names in StrValueNames ){
					if( names == _UiAlertThreeStr.Text )
						break;
					ind++;
				}
				this.CreateStringPickerDialog(bundle, 3, ind).Show();
			};

			//Fore alert
			_UiAlertForeIndTitle = FindViewById<TextView> (Resource.Id.alertForeIndTitle);
			_UiAlertForeInd = FindViewById<ToggleButton> (Resource.Id.alertForeInd);
			_UiAlertForeInd.Click += (sender, e) => {
				bool AlertOn = _UiAlertForeInd.Checked;

				if( AlertOn ){
					_UiAlertForeNum.Visibility = ViewStates.Visible;
					_UiAlertForeStr.Visibility = ViewStates.Visible;
					_UiAlertForeIndTitle.Text = "On";
				}else{
					_UiAlertForeNum.Visibility = ViewStates.Gone;
					_UiAlertForeStr.Visibility = ViewStates.Gone;
					_UiAlertForeIndTitle.Text = "Off";
				}
			};


			_UiAlertForeNum = FindViewById<EditText> (Resource.Id.alertForeNum);
			_UiAlertForeNum.Click += (sender, e) => {
				this.CreateNumberPickerDialog(bundle, 4, Convert.ToInt16 (_UiAlertForeNum.Text)).Show();
			};

			_UiAlertForeStr = FindViewById<EditText> (Resource.Id.alertForeStr);
			_UiAlertForeStr.Click += (sender, e) => {
				int ind = 1;
				foreach( var names in StrValueNames ){
					if( names == _UiAlertForeStr.Text )
						break;
					ind++;
				}
				this.CreateStringPickerDialog(bundle, 4, ind).Show();
			};

			//Five alert
			_UiAlertFiveIndTitle = FindViewById<TextView> (Resource.Id.alertFiveIndTitle);
			_UiAlertFiveInd = FindViewById<ToggleButton> (Resource.Id.alertFiveInd);
			_UiAlertFiveInd.Click += (sender, e) => {
				bool AlertOn = _UiAlertFiveInd.Checked;

				if( AlertOn ){
					_UiAlertFiveNum.Visibility = ViewStates.Visible;
					_UiAlertFiveStr.Visibility = ViewStates.Visible;
					_UiAlertFiveIndTitle.Text = "On";
				}else{
					_UiAlertFiveNum.Visibility = ViewStates.Gone;
					_UiAlertFiveStr.Visibility = ViewStates.Gone;
					_UiAlertFiveIndTitle.Text = "Off";
				}
			};


			_UiAlertFiveNum = FindViewById<EditText> (Resource.Id.alertFiveNum);
			_UiAlertFiveNum.Click += (sender, e) => {
				this.CreateNumberPickerDialog(bundle, 5, Convert.ToInt16 (_UiAlertFiveNum.Text)).Show();
			};

			_UiAlertFiveStr = FindViewById<EditText> (Resource.Id.alertFiveStr);
			_UiAlertFiveStr.Click += (sender, e) => {
				int ind = 1;
				foreach( var names in StrValueNames ){
					if( names == _UiAlertFiveStr.Text )
						break;
					ind++;
				}
				this.CreateStringPickerDialog(bundle, 5, ind).Show();
			};



			string[] alerts = _AlertStr.Split ('#');

			foreach (var a in alerts) {
				string[] alertStr = a.Split (';');
				if (alertStr [1] == "1") {
					switch (Convert.ToInt16 (alertStr [0])) {
					case 1:
						_UiAlertOneNum.Visibility = ViewStates.Visible;
						_UiAlertOneStr.Visibility = ViewStates.Visible;
						_UiAlertOneNum.Text = alertStr [3];
						_UiAlertOneStr.Text = StrValueNames [Convert.ToInt16 (alertStr [4])];
						_AlertOneSelectStr = Convert.ToInt16 (alertStr [4]);
						_UiAlertOneIndTitle.Text = "On";
						_UiAlertOneInd.Checked = true;
						break;
					case 2:
						_UiAlertTwoNum.Visibility = ViewStates.Visible;
						_UiAlertTwoStr.Visibility = ViewStates.Visible;
						_UiAlertTwoNum.Text = alertStr [3];
						_UiAlertTwoStr.Text = StrValueNames[Convert.ToInt16 (alertStr [4])];
						_AlertTwoSelectStr = Convert.ToInt16 (alertStr [4]);
						_UiAlertTwoIndTitle.Text = "On";
						_UiAlertTwoInd.Checked = true;
						break;
					case 3:
						_UiAlertThreeNum.Visibility = ViewStates.Visible;
						_UiAlertThreeStr.Visibility = ViewStates.Visible;
						_UiAlertThreeNum.Text = alertStr [3];
						_UiAlertThreeStr.Text = StrValueNames[Convert.ToInt16 (alertStr [4])];
						_AlertThreeSelectStr = Convert.ToInt16 (alertStr [4]);
						_UiAlertThreeIndTitle.Text = "On";
						_UiAlertThreeInd.Checked = true;
						break;
					case 4:
						_UiAlertForeNum.Visibility = ViewStates.Visible;
						_UiAlertForeStr.Visibility = ViewStates.Visible;
						_UiAlertForeNum.Text = alertStr [3];
						_UiAlertForeStr.Text = StrValueNames[Convert.ToInt16 (alertStr [4])];
						_AlertForeSelectStr = Convert.ToInt16 (alertStr [4]);
						_UiAlertForeIndTitle.Text = "On";
						_UiAlertForeInd.Checked = true;
						break;
					case 5:
						_UiAlertFiveNum.Visibility = ViewStates.Visible;
						_UiAlertFiveStr.Visibility = ViewStates.Visible;
						_UiAlertFiveNum.Text = alertStr [3];
						_UiAlertFiveStr.Text = StrValueNames[Convert.ToInt16 (alertStr [4])];
						_AlertFiveSelectStr = Convert.ToInt16 (alertStr [4]);
						_UiAlertFiveIndTitle.Text = "On";
						_UiAlertFiveInd.Checked = true;
						break;
					}
					NotificationManager mNM;
					mNM = (NotificationManager)GetSystemService (Context.NotificationService);
					mNM.Cancel (Convert.ToInt32( alertStr[2] ));
				}


			}

			//_NM = (NotificationManager)GetSystemService (NotificationService);

			var ui_AlertOk = FindViewById<ImageButton> (Resource.Id.AlertOk);
			ui_AlertOk.Click += (sender, e) => {
				bool AlertOneOn = _UiAlertOneInd.Checked;
				bool AlertTwoOn = _UiAlertTwoInd.Checked;
				bool AlertThreeOn = _UiAlertThreeInd.Checked;
				bool AlertForeOn = _UiAlertForeInd.Checked;
				bool AlertFiveOn = _UiAlertFiveInd.Checked;

				string ResultStr = "";

				//Intent Intent = new Intent (this, typeof(MainActivity));
				//PendingIntent pi = PendingIntent.GetActivity(this, 0, Intent, 0);
				//string body = "Test message";
				//string title = "Big Day";

				if( AlertOneOn )
				{
					Random random = new Random();
					int ID = random.Next(0, 999999);
					int Num = Convert.ToInt16( _UiAlertOneNum.Text.ToString() );
					int Str = _AlertOneSelectStr;
					ResultStr += String.Format("1;1;{0};{1};{2}", ID, Num, Str);
				}else ResultStr += "1;0";

				if( AlertTwoOn )
				{
					Random random = new Random();
					int ID = random.Next(0, 999999);
					int Num = Convert.ToInt16( _UiAlertTwoNum.Text.ToString() );
					int Str = _AlertTwoSelectStr;
					ResultStr += String.Format("#2;1;{0};{1};{2}", ID, Num, Str);
				}else ResultStr += "#2;0";

				if( AlertThreeOn )
				{
					Random random = new Random();
					int ID = random.Next(0, 999999);
					int Num = Convert.ToInt16( _UiAlertThreeNum.Text.ToString() );
					int Str = _AlertThreeSelectStr;
					ResultStr += String.Format("#3;1;{0};{1};{2}", ID, Num, Str);
				}else ResultStr += "#3;0";

				if( AlertForeOn )
				{
					Random random = new Random();
					int ID = random.Next(0, 999999);
					int Num = Convert.ToInt16( _UiAlertForeNum.Text.ToString() );
					int Str = _AlertForeSelectStr;
					ResultStr += String.Format("#4;1;{0};{1};{2}", ID, Num, Str);
				}else ResultStr += "#4;0";

				if( AlertFiveOn )
				{
					Random random = new Random();
					int ID = random.Next(0, 999999);
					int Num = Convert.ToInt16( _UiAlertFiveNum.Text.ToString() );
					int Str = _AlertFiveSelectStr;
					ResultStr += String.Format("#5;1;{0};{1};{2}", ID, Num, Str);
				}else ResultStr += "#5;0";

				Intent ParentIntent = new Intent (this, typeof(NewBigDays));
				ParentIntent.PutExtra("Alert", ResultStr);
				SetResult (Result.Ok, ParentIntent);
				Finish();

			};
		}
			
		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
		}

		protected override void OnRestoreInstanceState(Bundle savedState)
		{
			base.OnRestoreInstanceState (savedState);
		}

		public Dialog CreateNumberPickerDialog(Bundle savedInstanceState, int AlertInd, int Value)
		{
			NumberPicker numPicker = new NumberPicker(this);
			numPicker.MinValue = 1;
			numPicker.Value = Value;
			numPicker.MaxValue = 60;
			var builder = new AlertDialog.Builder(this)
				.SetView(numPicker)
				.SetNegativeButton("Cancel", (sender, args) =>
					{
						// Do something when this button is clicked.
					})
				.SetPositiveButton("Set", (sender, args) =>
					{
						if( AlertInd == 1 )
							_UiAlertOneNum.Text = numPicker.Value.ToString();
						else if( AlertInd == 2 )
							_UiAlertTwoNum.Text = numPicker.Value.ToString();
						else if( AlertInd == 3 )
							_UiAlertThreeNum.Text = numPicker.Value.ToString();
						else if( AlertInd == 4 )
							_UiAlertForeNum.Text = numPicker.Value.ToString();
						else if( AlertInd == 5 )
							_UiAlertFiveNum.Text = numPicker.Value.ToString();
					})
				.SetTitle("Select");
			return builder.Create();
		}

		public Dialog CreateStringPickerDialog(Bundle savedInstanceState, int AlertInd, int Value)
		{
			NumberPicker strPicker = new NumberPicker(this);
			strPicker.MinValue = 0;
			strPicker.MaxValue = 6;
			string[] StrValueNames = { "Secs", "Mins", "Hours", "Days", "Weeks", "Months", "Years" };
			strPicker.SetDisplayedValues( StrValueNames );
			strPicker.Value = Value;
			var builder = new AlertDialog.Builder(this)
				.SetView(strPicker)
				.SetNegativeButton("Cancel", (sender, args) =>
					{
						// Do something when this button is clicked.
					})
				.SetPositiveButton("Set", (sender, args) =>
					{
						if( AlertInd == 1 ){
							_AlertOneSelectStr = strPicker.Value;
							_UiAlertOneStr.Text = StrValueNames[_AlertOneSelectStr];
						}else if( AlertInd == 2 ){
							_AlertTwoSelectStr = strPicker.Value;
							_UiAlertTwoStr.Text = StrValueNames[_AlertTwoSelectStr];
						}else if( AlertInd == 3 ){
							_AlertThreeSelectStr = strPicker.Value;
							_UiAlertThreeStr.Text = StrValueNames[_AlertThreeSelectStr];
						}else if( AlertInd == 4 ){
							_AlertForeSelectStr = strPicker.Value;
							_UiAlertForeStr.Text = StrValueNames[_AlertForeSelectStr];
						}else if( AlertInd == 5 ){
							_AlertFiveSelectStr = strPicker.Value;
							_UiAlertFiveStr.Text = StrValueNames[_AlertFiveSelectStr];
						}

					})
				.SetTitle("Select");
			return builder.Create();
		}

	}
}

