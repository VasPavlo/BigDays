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
	public class AlertDlg: DialogFragment
	{
		public string _Message = "";

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			EventHandler<DialogClickEventArgs> okhandler;
			var builder = new AlertDialog.Builder(Activity)
				.SetMessage(_Message)
				.SetPositiveButton("Ok", (sender, args) =>
					{
						// Do something when this button is clicked.
					})
				.SetTitle("Error!!!");
			return builder.Create();
		}
	}
}

