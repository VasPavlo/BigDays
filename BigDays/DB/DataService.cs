using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Mono.Data.Sqlite;
using System;
using BigDays.Models;
using BigDays.Enums;

namespace BigDays.Services
{
    public class DataService 
    {
        private bool _FileExists;
        private string _pathToDatabase;
        private SQLiteConnection _database { get { return new SQLiteConnection(_pathToDatabase); } }

        public bool ConnectToDB(string sDBPath)
        {
            _pathToDatabase = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), sDBPath);         
            _FileExists = System.IO.File.Exists(_pathToDatabase);                     
            return _FileExists;
        }

        public bool CreateTableEnpty()
        {
            try
            {
                _database.CreateTable<BigDaysItemModel>();
                _database.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateTable()
        {
            // if (_FileExists)
            // {

            _database.CreateTable<BigDaysItemModel>();
            var count = _database.Table<BigDaysItemModel>().Count();
            if (count == 0)
            {               
                Random random = new Random();
                var firstItem = new BigDaysItemModel()
                {
                    _Name = "Weekend Paris",
                    _Notification = random.Next(0, 999999),
                    _EndDate = DateTime.Now.AddYears(1),
                    _Image = "img17small.jpg",
					_ImageStorage = (int)LocationPicture.ResourcesImage,
                    _Repeat = 0,
                    _Alerts = "1;0#2;0#3;0#4;0#5;0",
                    _Active = true,
                    _PosLeft = 0,
                    _PosTop = 0,
					_ChangePos = false
                };
                _database.Insert(firstItem);
                _database.Close();
            }
            //}
            return _FileExists;
        }

        public bool Insert(BigDaysItemModel BDItem)
        {
            try
            {
                var db = new SQLiteConnection(_pathToDatabase);
                db.Insert(BDItem);
                _database.Close();
                return true;
            }
            catch (SQLiteException ex)
            {
                return false;
            }          
        }

        public bool Insert(BigDaysItemModel[] BDItems)
        {
            try
            {
                var db = new SQLiteConnection(_pathToDatabase);
                db.InsertAll(BDItems);
                _database.Close();
                return true;
            }
            catch (SQLiteException ex)
            {
                return false;
            }
        }


        public bool Update(BigDaysItemModel BDItem)
        {
            try
            {
                var db = new SQLiteConnection(_pathToDatabase);
                db.Update(BDItem);
                _database.Close();
                return true;
            }
            catch (SQLiteException ex)
            {
                return false;
            }
        }

        public bool UpdatePos(BigDaysItemModel BDItem)
        {            
            try
            {
				BDItem._ChangePos = true;
                var db = new SQLiteConnection(_pathToDatabase);
                db.Update(BDItem);
                _database.Close();
                return true;
            }
            catch (SQLiteException ex)
            {
                return false;
            }            
        }

        public BigDaysItemModel SelectItem(int ID)
        {           
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == ID);
            _database.Close();
            return data;
        }

        public List<BigDaysItemModel> SelectBDItems()
        {                 
			var data = _database.Table<BigDaysItemModel>().OrderBy(x=>x._EndDate).ToList();
            _database.Close();
            return data;
        }


        public BigDaysItemModel GetLastAddItem()
        {            
            var data = _database.Table<BigDaysItemModel>().LastOrDefault();
            _database.Close();
            return data;
        }

        public int GetLastID()
        {          
            var data = _database.Table<BigDaysItemModel>().OrderBy(x=>x._ID).LastOrDefault();
            _database.Close();
            return data != null ? data._ID : 0;
        }


        public BigDaysItemModel GetCurrentItem()
        {           
           var n = _database.Table<BigDaysItemModel>();

			BigDaysItemModel data = n.FirstOrDefault(x=>x._Active == true);
			if (data == null)
			{
				 data = _database.Table<BigDaysItemModel>().FirstOrDefault();
			}

            _database.Close();
            return data;
        }


        public void Delete(int ID)
        {           
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == ID);
            _database.Delete(data);
            _database.Close();
        }


        public void SetActive(int ID)
        {
            SetDeActivate();                      
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == ID);
			if (data != null)
			{
				data._Active = true;
				_database.Update(data);
			}
            _database.Close();
        }

        public void SetDeActivate()
        {
            var CurrItem = GetCurrentItem();
            if (CurrItem != null)
            {
                var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == CurrItem._ID);
				if (data != null)
				{
					data._Active = false;
					_database.Update(data);
				}
                _database.Close();
            }
        }


        public void CheckRepeats()
        {
            List<BigDaysItemModel> BDitems = this.SelectBDItems();

            foreach (var item in BDitems)
            {
                if (item._Repeat != 0)
                {
                    var nItem = this.CheckRepeat(item);
                    this.Update(nItem);
                }
            }            
        }

        public BigDaysItemModel CheckRepeat(BigDaysItemModel item)
        {
            switch (item._Repeat)
            {
                case 1:
                    while (true)
                    {
                        if (item._EndDate < DateTime.Now)
                        {
                            item._EndDate = item._EndDate.AddDays(1);
                        }
                        else
                            break;
                    }
                    break;
                case 2:
                    while (true)
                    {
                        if (item._EndDate < DateTime.Now)
                        {
                            item._EndDate = item._EndDate.AddDays(7);
                        }
                        else
                            break;
                    }
                    break;
                case 3:
                    while (true)
                    {
                        if (item._EndDate < DateTime.Now)
                        {
                            item._EndDate = item._EndDate.AddMonths(1);
                        }
                        else
                            break;
                    }
                    break;
                case 4:
                    while (true)
                    {
                        if (item._EndDate < DateTime.Now)
                        {
                            item._EndDate = item._EndDate.AddYears(2);
                        }
                        else
                            break;
                    }
                    break;
            }
            return item;
        }


       //private bool _FileExists;
        //private string pathToDatabase;

        //public bool CreateDatabase(string path)
        //{
        //    try
        //    {
        //        var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        //        pathToDatabase = System.IO.Path.Combine(docsFolder, path);

        //        var connection = new SQLiteConnection(pathToDatabase);
        //        connection.CreateTable<BigDaysItemModel>();

        //        _FileExists = System.IO.File.Exists(pathToDatabase);
        //        return _FileExists;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        return false;
        //    }
        //}


        //public bool InsertUpdateData(BigDaysItemModel data)
        //{
        //    try
        //    {
        //        var db = new SQLiteConnection(pathToDatabase);
        //        if (db.Insert(data) != 0)
        //            db.Update(data);
        //        return true;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        return false;
        //    }
        //}

        //public string InsertUpdateAllData(IEnumerable<BigDaysItemModel> data)
        //{
        //    try
        //    {
        //        var db = new SQLiteConnection(pathToDatabase);
        //        if (db.InsertAll(data) != 0)
        //            db.UpdateAll(data);
        //        return "List of data inserted or updated";
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        return ex.Message;
        //    }
        //}


        //public IEnumerable<BigDaysItemModel> GetAll()
        //{
        //   // try
        //    //{
        //        var db = new SQLiteConnection(pathToDatabase);
        //        return db.Table<BigDaysItemModel>();
        //   // }
        //   // catch (SQLiteException)
        //   // {
        //   //     return -1;
        //   // }
        //}

        //public int FindNumberRecords()
        //{
        //    try
        //    {
        //        var db = new SQLiteConnection(pathToDatabase);
        //        // this counts all records in the database, it can be slow depending on the size of the database
        //        var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person");

        //        // for a non-parameterless query
        //        // var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");

        //        return count;
        //    }
        //    catch (SQLiteException)
        //    {
        //        return -1;
        //    }
        //}
    }
}