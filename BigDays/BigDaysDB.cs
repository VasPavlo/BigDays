using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;

namespace BigDays
{
	public class BigDaysDB
	{
		private SqliteConnection _connection;
		private bool _FileExists;

		public bool ConnectToDB(string sDBPath){
			string dbPath = System.IO.Path.Combine ( System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), sDBPath );
			_FileExists = System.IO.File.Exists (dbPath);
			if (!_FileExists) {
				SqliteConnection.CreateFile (dbPath);
				_FileExists = System.IO.File.Exists (dbPath);
			}
			
			_connection = new SqliteConnection ("Data Source=" + dbPath);
			return _FileExists;
		}

		public bool CreateTable(){
			if (_FileExists) {
				if (!TableExists ("BigDays", _connection)) {
					_connection.Open ();
					// This is the first time the app has run and/or that we need the DB.
					// Copy a "template" DB from your assets, or programmatically create one like this:
					var command = "CREATE TABLE IF NOT EXISTS BigDays (_ID INTEGER PRIMARY KEY NOT NULL, _Name ntext, _Notification INTEGER, _EndDate ntext, _Image ntext, _ImageStorage INTEGER, _Repeat INTEGER, _Alerts ntext, _Active INTEGER, _PosTop INTEGER DEFAULT 0, _PosLeft INTEGER DEFAULT 0, _ChangePos INTEGER DEFAULT 0);";
					using (var c = _connection.CreateCommand ()) {
						c.CommandText = command;
						c.ExecuteNonQuery ();
					}
					_connection.Close ();
					Random random = new Random();
					BigDaysItem test = new BigDaysItem () {
						_Name = "Weekend Paris",
						_Notification = random.Next(0, 999999),
						_EndDate = DateTime.Now.AddYears(1),
						_Image = "img17small.jpg",
						_ImageStorage = 1,
						_Repeat = 0,
						_Alerts = "1;0#2;0#3;0#4;0#5;0",
						_Active = 1,
						_PosLeft = 0,
						_PosTop = 0,
						_ChangePos = 0
					};
					this.Insert (test);
				}
			}
			return _FileExists;
		}

		public bool TableExists (String tableName, SqliteConnection connection)
		{
			_connection.Open ();
			using (SqliteCommand cmd = new SqliteCommand())
			{
				cmd.CommandType = CommandType.Text;
				cmd.Connection = connection;
				cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = @name";
				cmd.Parameters.AddWithValue("@name", tableName);

				using (SqliteDataReader sqlDataReader = cmd.ExecuteReader())
				{
					if (sqlDataReader.Read ()) {
						_connection.Close ();
						return true;
					} else {
						_connection.Close ();
						return false;
					}
				}
			}
		}
			
		public bool Insert(BigDaysItem[] BDItems){
			if (_FileExists) {
				_connection.Open ();

				// This is the first time the app has run and/or that we need the DB.
				// Copy a "template" DB from your assets, or programmatically create one like this:
				foreach (var item in BDItems) {
					var command = "INSERT INTO BigDays (_Name, _Notification, _EndDate, _Image, _ImageStorage, _Repeat, _Alerts, _Active, _ChangePos, _PosLeft, _PosTop) VALUES ('" + item._Name + "', '" + item._Notification + "', '" + item._EndDate.ToString() + "', '" + item._Image + "', '" + item._ImageStorage + "', '" + item._Repeat + "', '" + item._Alerts + "', '" + item._Active + "', '0', '0', '0');";
					using (var c = _connection.CreateCommand ()) {
						c.CommandText = command;
						c.ExecuteNonQuery ();
					}
				}

				_connection.Close ();
			}
			return _FileExists;
		}

		public bool Insert(BigDaysItem BDItems){
			if (_FileExists) {
				_connection.Open ();

				// This is the first time the app has run and/or that we need the DB.
				// Copy a "template" DB from your assets, or programmatically create one like this:
				var command = "INSERT INTO BigDays (_Name, _Notification, _EndDate, _Image, _ImageStorage, _Repeat, _Alerts, _Active, _ChangePos, _PosLeft, _PosTop) VALUES ('" + BDItems._Name + "', '" + BDItems._Notification + "', '" + BDItems._EndDate + "', '" + BDItems._Image + "', '" + BDItems._ImageStorage + "', '" + BDItems._Repeat + "', '" + BDItems._Alerts + "', '" + BDItems._Active + "', '0', '0', '0');";
				using (var c = _connection.CreateCommand ()) {
					c.CommandText = command;
					c.ExecuteNonQuery ();
				}

				_connection.Close ();
			}
			return _FileExists;
		}

		public bool Update(BigDaysItem BDItems){
			if (_FileExists) {
				_connection.Open ();

				var command = "UPDATE BigDays SET _Name = '" + BDItems._Name + "', _Notification = '" + BDItems._Notification + "', _EndDate = '" + BDItems._EndDate + "', _Image = '" + BDItems._Image + "', _ImageStorage = '" + BDItems._ImageStorage + "', _Repeat = '" + BDItems._Repeat + "', _Alerts = '" + BDItems._Alerts + "' WHERE _ID = '" + BDItems._ID + "'";
				using (var c = _connection.CreateCommand ()) {
					c.CommandText = command;
					c.ExecuteNonQuery ();
				}

				_connection.Close ();
			}
			return _FileExists;
		}

		public bool UpdatePos(BigDaysItem BDItems){
			if (_FileExists) {
				_connection.Open ();

				var command = "UPDATE BigDays SET _ChangePos = '1', _PosLeft = '" + BDItems._PosLeft + "', _PosTop = '" + BDItems._PosTop + "'  WHERE _ID = '" + BDItems._ID + "'";
				using (var c = _connection.CreateCommand ()) {
					c.CommandText = command;
					c.ExecuteNonQuery ();
				}

				_connection.Close ();
			}
			return _FileExists;
		}

		public List<BigDaysItem> SelectBDItems(){
			List<BigDaysItem> BDitems = new List<BigDaysItem>();

			if (_FileExists) {
				_connection.Open ();

				using (var contents = _connection.CreateCommand ()) {
					contents.CommandText = "SELECT * FROM BigDays ORDER BY _Name DESC";

					var r = contents.ExecuteReader ();
					while (r.Read ())
						BDitems.Add ( new BigDaysItem(){ _ID = Convert.ToInt16(r ["_ID"].ToString()), 
							_Name = r ["_Name"].ToString (), 
							_Notification = Convert.ToInt32(r ["_Notification"].ToString ()),
							_EndDate = Convert.ToDateTime(r ["_EndDate"].ToString ()), 
							_Image = r ["_Image"].ToString (), 
							_ImageStorage = Convert.ToInt16(r ["_ImageStorage"].ToString ()), 
							_Repeat = Convert.ToInt16(r ["_Repeat"].ToString ()), 
							_Alerts = r ["_Alerts"].ToString (),
							_Active = Convert.ToInt16(r ["_Active"].ToString ()),
							_PosLeft = Convert.ToInt16(r ["_PosLeft"].ToString ()),
							_PosTop = Convert.ToInt16(r ["_PosTop"].ToString ()),
							_ChangePos = Convert.ToInt16(r ["_ChangePos"].ToString ()) } );

				}
				_connection.Close ();
				if( BDitems.Count > 0 )
					BDitems = BDitems.OrderBy(o=>o._EndDate).ToList();
			}
			return BDitems;
		}

		public BigDaysItem SelectItem(int ID){
			BigDaysItem BDItem = new BigDaysItem();
			if (_FileExists) {
				_connection.Open ();

				using (var contents = _connection.CreateCommand ()) {
					contents.CommandText = "SELECT * FROM BigDays WHERE _ID='" + ID.ToString() + "' LIMIT 1";

					var r = contents.ExecuteReader ();
					while (r.Read ())
						BDItem = new BigDaysItem(){ _ID = Convert.ToInt16(r ["_ID"].ToString()), 
							_Name = r ["_Name"].ToString (), 
							_Notification = Convert.ToInt32(r ["_Notification"].ToString ()),
							_EndDate = Convert.ToDateTime(r ["_EndDate"].ToString ()), 
							_Image = r ["_Image"].ToString (), 
							_ImageStorage = Convert.ToInt16(r ["_ImageStorage"].ToString ()), 
							_Repeat = Convert.ToInt16(r ["_Repeat"].ToString ()), 
							_Alerts = r ["_Alerts"].ToString (),
							_Active = Convert.ToInt16(r ["_Active"].ToString ()),
							_PosLeft = Convert.ToInt16(r ["_PosLeft"].ToString ()),
							_PosTop = Convert.ToInt16(r ["_PosTop"].ToString ()),
							_ChangePos = Convert.ToInt16(r ["_ChangePos"].ToString ())};

				}
				_connection.Close ();
			}

			return BDItem;
		}

		public BigDaysItem GetLastAddItem(){
			BigDaysItem BDItem = new BigDaysItem();
			if (_FileExists) {
				_connection.Open ();

				using (var contents = _connection.CreateCommand ()) {
					contents.CommandText = "SELECT * FROM BigDays WHERE _ID = (SELECT MAX(_ID) FROM BigDays)";

					var r = contents.ExecuteReader ();
					while (r.Read ())
						BDItem = new BigDaysItem(){ _ID = Convert.ToInt16(r ["_ID"].ToString()), 
						_Name = r ["_Name"].ToString (), 
						_Notification = Convert.ToInt32(r ["_Notification"].ToString ()),
						_EndDate = Convert.ToDateTime(r ["_EndDate"].ToString ()), 
						_Image = r ["_Image"].ToString (), 
						_ImageStorage = Convert.ToInt16(r ["_ImageStorage"].ToString ()), 
						_Repeat = Convert.ToInt16(r ["_Repeat"].ToString ()), 
						_Alerts = r ["_Alerts"].ToString (),
						_Active = Convert.ToInt16(r ["_Active"].ToString ()),
						_PosLeft = Convert.ToInt16(r ["_PosLeft"].ToString ()),
						_PosTop = Convert.ToInt16(r ["_PosTop"].ToString ()),
						_ChangePos = Convert.ToInt16(r ["_ChangePos"].ToString ())};

				}
				_connection.Close ();
			}

			return BDItem;
		
		}

		public int GetLastID(){
			int ret = 0;

			if (_FileExists) {
				_connection.Open ();

				using (var contents = _connection.CreateCommand ()) {
					contents.CommandText = "SELECT * FROM BigDays WHERE _ID = (SELECT MAX(_ID) FROM BigDays)";

					var r = contents.ExecuteReader ();
					while (r.Read ())
						ret = Convert.ToInt16(r ["_ID"].ToString());

				}
				_connection.Close ();
			}

			return ret;
		}

		public BigDaysItem GetCurrentItem(){
			BigDaysItem BDItem = new BigDaysItem();
			if (_FileExists) {
				_connection.Open ();

				using (var contents = _connection.CreateCommand ()) {
					contents.CommandText = "SELECT * FROM BigDays WHERE _Active = '1' LIMIT 1";

					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						BDItem = new BigDaysItem () { _ID = Convert.ToInt16 (r ["_ID"].ToString ()), 
							_Name = r ["_Name"].ToString (), 
							_Notification = Convert.ToInt32(r ["_Notification"].ToString ()),
							_EndDate = Convert.ToDateTime (r ["_EndDate"].ToString ()), 
							_Image = r ["_Image"].ToString (), 
							_ImageStorage = Convert.ToInt16 (r ["_ImageStorage"].ToString ()), 
							_Repeat = Convert.ToInt16 (r ["_Repeat"].ToString ()), 
							_Alerts = r ["_Alerts"].ToString (),
							_Active = Convert.ToInt16(r ["_Active"].ToString ()),
							_PosLeft = Convert.ToInt16(r ["_PosLeft"].ToString ()),
							_PosTop = Convert.ToInt16(r ["_PosTop"].ToString ()),
							_ChangePos = Convert.ToInt16(r ["_ChangePos"].ToString ())
						};
					}
						

				}
				_connection.Close ();
			}

			return BDItem;
		}

		public void Delete(int ID){
			if (_FileExists) {
				_connection.Open ();
				if (ID != 0) {
					var command = "DELETE FROM BigDays WHERE _ID='" + ID.ToString () + "'";

					using (var c = _connection.CreateCommand ()) {
						c.CommandText = command;
						c.ExecuteNonQuery ();
					}
				}
				_connection.Close ();
			}
		}

		public void SetActive(int ID){
			BigDaysItem CurrItem = GetCurrentItem ();
			if (_FileExists) {
				_connection.Open ();

				var commands = new[] { "UPDATE BigDays SET _Active = '0' WHERE _ID = '" + CurrItem._ID + "'",
					"UPDATE BigDays SET _Active = '1' WHERE _ID = '" + ID + "'"};
				foreach (var command in commands) {
					using (var c = _connection.CreateCommand ()) {
						c.CommandText = command;
						c.ExecuteNonQuery ();
					}
				}

				_connection.Close ();
			}
		}

		public void CheckRepeats(){
			List<BigDaysItem> BDitems = this.SelectBDItems ();

			foreach (var item in BDitems) {
				if (item._Repeat != 0) {
					BigDaysItem nItem = this.CheckRepeat (item);
					this.Update (nItem);
				}
			}
		}

		public BigDaysItem CheckRepeat( BigDaysItem item ){
			switch (item._Repeat) {
			case 1:
				while (true) {
					if (item._EndDate < DateTime.Now) {
						item._EndDate = item._EndDate.AddDays (1);
					} else
						break;
				}
				break;
			case 2:
				while (true) {
					if (item._EndDate < DateTime.Now) {
						item._EndDate = item._EndDate.AddDays (7);
					} else
						break;
				}
				break;
			case 3:
				while (true) {
					if (item._EndDate < DateTime.Now) {
						item._EndDate = item._EndDate.AddMonths (1);
					} else
						break;
				}
				break;
			case 4:
				while (true) {
					if (item._EndDate < DateTime.Now) {
						item._EndDate = item._EndDate.AddYears (2);
					} else
						break;
				}
				break;
			}
			return item;
		}
			
	}
}

