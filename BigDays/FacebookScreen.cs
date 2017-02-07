using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Model;
using Xamarin.Facebook.Widget;
using Android.Views.InputMethods;

#if DEBUG
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/facebook_app_id_debug")]
#elif _TRIAL_
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/facebook_app_id_free")]
#elif __AMAZON__
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/facebook_app_id_amazon")]
#else
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/facebook_app_id_full")]
#endif


namespace BigDays
{
	[Activity (Label = "@string/app_name", MainLauncher = false, WindowSoftInputMode = SoftInput.AdjustResize)]
	public class FacebookScreen : FragmentActivity
	{
		private int _ID;
		private BigDaysItem _Item;
		private UiLifecycleHelper _uiHelper;
		private static readonly string[] PERMISSIONS = new String [] { "publish_actions" };
		private readonly String PENDING_ACTION_BUNDLE_KEY = "com.prosellersworld.bigdaysfree:PendingAction";
		//com.prosellersworld.bigdaysfree.FacebookScreen

		private Button _postStatusUpdateButton;
		private LoginButton _loginButton;
		private PendingAction _pendingAction = PendingAction.NONE;
		private ViewGroup _controlsContainer;
		private IGraphUser _user;
		private EditText _textMessage;
		private TextView _textFacebookDescription;
		//private string _googlePlayLink;
		private string _message;

		public FacebookScreen ()
		{
			callback = new MyStatusCallback (this);
		}

		enum PendingAction
		{
			NONE,
			POST_PHOTO,
			POST_STATUS_UPDATE
		}

		class MyStatusCallback : Java.Lang.Object, Session.IStatusCallback
		{
			FacebookScreen owner;

			public MyStatusCallback (FacebookScreen owner)
			{
				this.owner = owner;
			}

			public void Call (Session session, SessionState state, Java.Lang.Exception exception)
			{
				owner.OnSessionStateChange (session, state, exception);
			}
		}

		private Session.IStatusCallback callback;

		class MyUserInfoChangedCallback : Java.Lang.Object, LoginButton.IUserInfoChangedCallback
		{
			FacebookScreen owner;

			public MyUserInfoChangedCallback (FacebookScreen owner)
			{
				this.owner = owner;
			}

			public void OnUserInfoFetched (IGraphUser user)
			{
				owner._user = user;
				owner.UpdateUI ();
				// It's possible that we were waiting for this.user to be populated in order to post a
				// status update.
				owner.HandlePendingAction ();
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			_uiHelper = new UiLifecycleHelper (this, callback);
			_uiHelper.OnCreate (savedInstanceState);

			if (savedInstanceState != null) {
				string name = savedInstanceState.GetString (PENDING_ACTION_BUNDLE_KEY);
				_pendingAction = (PendingAction)Enum.Parse (typeof(PendingAction), name);
			}

			SetContentView (Resource.Layout.FacebookScreen);

			//_googlePlayLink = "http://play.google.com/store/apps/details?id=" + this.PackageName;
			_ID = Intent.GetIntExtra ("ID", 0);
			_Item = MainActivity._BDDB.SelectItem (_ID);

			TimeSpan ts = _Item._EndDate.Subtract (DateTime.Now);
			string DaysTo = String.Format ("{0:0000}", ts.Days);
			string HoursTo = String.Format ("{0:00}", ts.Hours);
			string MinTo = String.Format ("{0:00}", ts.Minutes);
			string SecTo = String.Format ("{0:00}", ts.Seconds);
		 	_message = string.Format("{0} {1} {2} days, {3} hours, {4} minutes, {5} seconds {6}",
				_Item._Name,
				_Item._EndDate < DateTime.Now ? "was" : "is coming in",
				DaysTo, HoursTo, MinTo, SecTo,
				_Item._EndDate < DateTime.Now ? " ago" : "");


			_textFacebookDescription = (TextView)FindViewById (Resource.Id.textFacebookDescription);
			_textFacebookDescription.Text = _message;

			_textMessage = (EditText)FindViewById (Resource.Id.txtMessage);

			_loginButton = (LoginButton)FindViewById (Resource.Id.facebook_login_button);
			_loginButton.UserInfoChangedCallback = new MyUserInfoChangedCallback (this);

			_postStatusUpdateButton = FindViewById<Button> (Resource.Id.postStatusUpdateButton);
			_postStatusUpdateButton.Click += delegate {
				OnClickPostStatusUpdate ();
			};

			_controlsContainer = (ViewGroup)FindViewById (Resource.Id.main_ui_container);

			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			Android.Support.V4.App.Fragment fragment = fm.FindFragmentById (Resource.Id.fragment_container);
			if (fragment != null) {
				// If we're being re-created and have a fragment, we need to a) hide the main UI controls and
				// b) hook up its listeners again.
				_controlsContainer.Visibility = ViewStates.Gone;
			}

			fm.BackStackChanged += delegate {
				if (fm.BackStackEntryCount == 0) {
					// We need to re-show our UI.
					_controlsContainer.Visibility = ViewStates.Visible;
				}
			};
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			_uiHelper.OnResume ();

			UpdateUI ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			_uiHelper.OnSaveInstanceState (outState);

			outState.PutString (PENDING_ACTION_BUNDLE_KEY, _pendingAction.ToString ());
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			_uiHelper.OnActivityResult (requestCode, (int)resultCode, data);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_uiHelper.OnPause ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			_uiHelper.OnDestroy ();
		}

		private void OnSessionStateChange (Session session, SessionState state, Exception exception)
		{
			if (_pendingAction != PendingAction.NONE &&
				(exception is FacebookOperationCanceledException ||
					exception is FacebookAuthorizationException)) {
				new AlertDialog.Builder (this)
					.SetTitle (Resource.String.cancelled)
					.SetMessage (Resource.String.permission_not_granted)
					.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
					.Show ();
				_pendingAction = PendingAction.NONE;
			} else if (state == SessionState.OpenedTokenUpdated) {
				HandlePendingAction ();
			}
			UpdateUI ();
		}

		private void UpdateUI ()
		{
			Session session = Session.ActiveSession;
			bool enableButtons = (session != null && session.IsOpened);
			_postStatusUpdateButton.Enabled = (enableButtons);	
		}

		private void HandlePendingAction ()
		{
			PendingAction previouslyPendingAction = _pendingAction;
			// These actions may re-set pendingAction if they are still pending, but we assume they
			// will succeed.
			_pendingAction = PendingAction.NONE;

			switch (previouslyPendingAction) {
			case PendingAction.POST_PHOTO:
				break;
			case PendingAction.POST_STATUS_UPDATE:
				PostStatusUpdate ();
				break;
			}
		}

		private void ShowPublishResult (String message, IGraphObject result, FacebookRequestError error)
		{
			String title = null;
			String alertMessage = null;
			if (error == null) {
				title = GetString (Resource.String.success);

				var id = result.GetProperty("id").ToString();
				alertMessage = GetString (Resource.String.successfully_posted_post, message, id);
			} else {
				title = GetString (Resource.String.error);
				alertMessage = error.ErrorMessage;
			}

			new AlertDialog.Builder (this)
				.SetTitle (title)
				.SetMessage (alertMessage)
				.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {OnBackPressed ();})
				.Show ();		
		}

		private void OnClickPostStatusUpdate ()
		{
			InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(_textMessage.WindowToken, 0);
			_postStatusUpdateButton.Visibility = ViewStates.Invisible;
			PerformPublish (PendingAction.POST_STATUS_UPDATE);
		}

		class RequestCallback : Java.Lang.Object, Request.ICallback
		{
			Action<Response> action;

			public RequestCallback (Action<Response> action)
			{
				this.action = action;
			}

			public void OnCompleted (Response response)
			{
				action (response);
			}
		}

		//TODO: !!!!PostStatusUpdate 
		public void PostStatusUpdate ()
		{					
			if (_user != null && HasPublishPermission ()) {

				Request request = Request.NewStatusUpdateRequest (Session.ActiveSession, _textMessage.Text, new RequestCallback (response => ShowPublishResult ("Ok", response.GraphObject, response.Error)));

				Bundle parameters = request.Parameters;
				parameters.PutString("link", Constants.StoreSearchLink);
				parameters.PutString("caption", Constants.CaptionName);
				parameters.PutString("description", _message);
				parameters.PutString("name", "Big Days of Our Lives Countdown for Android");
				parameters.PutString("picture", "https://fbcdn-photos-g-a.akamaihd.net/hphotos-ak-xfa1/t39.2081-0/p128x128/10333116_436546179825969_1893993755_n.png");

				request.ExecuteAsync ();
			} else {
				_pendingAction = PendingAction.POST_STATUS_UPDATE;
			}
		}

		class ErrorListener : Java.Lang.Object, PickerFragment.IOnErrorListener
		{
			Action<PickerFragment, FacebookException> action;

			public ErrorListener (Action<PickerFragment, FacebookException> action)
			{
				this.action = action;
			}

			public void OnError (PickerFragment p0, FacebookException p1)
			{
				action (p0, p1);
			}
		}	

		private void OnFriendPickerDone (FriendPickerFragment fragment)
		{
			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			fm.PopBackStack ();

			String results = "";

			var selection = fragment.Selection;
			if (selection != null && selection.Count > 0) {
				List<String> names = new List<String> ();
				foreach (IGraphUser user in selection) {
					names.Add (user.Name);
				}
				results = string.Join (", ", names.ToArray ());
			} else {
				results = GetString (Resource.String.no_friends_selected);
			}

			ShowAlert (GetString (Resource.String.you_picked), results);
		}

		private void OnPlacePickerDone (PlacePickerFragment fragment)
		{
			Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
			fm.PopBackStack ();

			String result = "";

			IGraphPlace selection = fragment.Selection;
			if (selection != null) {
				result = selection.Name;
			} else {
				result = GetString (Resource.String.no_place_selected);
			}

			ShowAlert (GetString (Resource.String.you_picked), result);
		}

		private void ShowAlert (String title, String message)
		{
			new AlertDialog.Builder (this)
				.SetTitle (title)
				.SetMessage (message)
				.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
				.Show ();
		}

		private bool HasPublishPermission ()
		{
			Session session = Session.ActiveSession;
			return session != null && session.Permissions.Contains ("publish_actions");
		}

		private void PerformPublish (PendingAction action)
		{
			Session session = Session.ActiveSession;
			if (session != null) {
				_pendingAction = action;
				if (HasPublishPermission ()) {
					// We can do the action right away.
					HandlePendingAction ();
				} else {
					// We need to get new permissions, then complete the action when we get called back.
					session.RequestNewPublishPermissions (new Session.NewPermissionsRequest (this, PERMISSIONS));
				}
			}
		}
	}
}