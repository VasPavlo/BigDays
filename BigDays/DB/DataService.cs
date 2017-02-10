using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using BigDays.DB;
using Mono.Data.Sqlite;
using System;
using BigDays.Models;

namespace BigDays.Services
{
    public class DataService : IBigDaysDB
    {
        private bool _FileExists;
        private string _pathToDatabase;
        private SQLiteConnection _database;

        public bool ConnectToDB(string sDBPath)
        {
            _pathToDatabase = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), sDBPath);
            _database = new SQLiteConnection(_pathToDatabase);
            _FileExists = System.IO.File.Exists(_pathToDatabase);                     
            return _FileExists;
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
                    _ImageStorage = 1,
                    _Repeat = 0,
                    _Alerts = "1;0#2;0#3;0#4;0#5;0",
                    _Active = 1,
                    _PosLeft = 0,
                    _PosTop = 0,
                    _ChangePos = 0
                };
                _database.Insert(firstItem);
            }
            //}
            return _FileExists;
        }


        //Use the old version
        public bool TableExists(string tableName, SqliteConnection connection)
        {
            throw new NotImplementedException();
        }


        public bool Insert(BigDaysItemModel BDItem)
        {
            return InsertUpdateData(BDItem);
        }

        public bool Insert(BigDaysItemModel[] BDItems)
        {
            return InsertUpdateAllData(BDItems);
        }


        public bool Update(BigDaysItemModel BDItem)
        {
            return InsertUpdateData(BDItem);
        }

        public bool UpdatePos(BigDaysItemModel BDItem)
        {
            BDItem._ChangePos = 1;
            return InsertUpdateData(BDItem);
        }

        public BigDaysItemModel SelectItem(int ID)
        {           
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == ID);
            return data;
        }

        public List<BigDaysItemModel> SelectBDItems()
        {                 
            var data = _database.Table<BigDaysItemModel>().OrderByDescending(x=>x._Name).ToList();
            return data;
        }


        public BigDaysItemModel GetLastAddItem()
        {            
            var data = _database.Table<BigDaysItemModel>().LastOrDefault();
            return data;
        }

        public int GetLastID()
        {          
            var data = _database.Table<BigDaysItemModel>().OrderBy(x=>x._ID).LastOrDefault();            
            return data != null ? data._ID : 0;
        }


        public BigDaysItemModel GetCurrentItem()
        {           
           //var n = _database.Table<BigDaysItem>();
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x=>x._Active == 1);
            return data;
        }


        public void Delete(int ID)
        {           
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == ID);
            _database.Delete(data);
        }


        public void SetActive(int ID)
        {
            SetDeActivate();                      
            var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == ID);
            data._Active = 1;
            _database.Update(data);           
        }

        public void SetDeActivate()
        {
            var CurrItem = GetCurrentItem();
            if (CurrItem != null)
            {
                var data = _database.Table<BigDaysItemModel>().FirstOrDefault(x => x._ID == CurrItem._ID);
                data._Active = 0;
                _database.Update(data);
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

       




        public bool InsertUpdateData(BigDaysItemModel data)
        {
            try
            {
                var db = new SQLiteConnection(_pathToDatabase);
                if (db.Insert(data) != 0)
                    db.Update(data);
                return true;
            }
            catch (SQLiteException ex)
            {
                return false;
            }
        }

        public bool InsertUpdateAllData(IEnumerable<BigDaysItemModel> data)
        {
            try
            {
                var db = new SQLiteConnection(_pathToDatabase);
                if (db.InsertAll(data) != 0)
                    db.UpdateAll(data);
                return true;
            }
            catch (SQLiteException ex)
            {
                return false;
            }
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