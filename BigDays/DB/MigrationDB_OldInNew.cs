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
using BigDays.Services;

namespace BigDays.DB
{
    /// <summary>
    /// https://components.xamarin.com/view/sqlite-net
    /// Not an ADO.NET implementation. This is not a full SQLite driver. If you need that, use Mono.Data.SQLite or csharp-sqlite.
    /// </summary>

    public class MigrationDB_OldInNew
    {
        private bool _FileExists;
        public MigrationDB_OldInNew(string sDBPathOld, string sDBPathNew)
        {
            try
            {            
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), sDBPathOld);
            _FileExists = System.IO.File.Exists(dbPath);

            if (_FileExists)
            {
                var bigDaysDB_Old = new BigDaysDB_Old();

                bigDaysDB_Old.ConnectToDB(sDBPathOld);
                var Items_Old = bigDaysDB_Old.SelectBDItems();

                var bigDaysDB_New =new DataService();
                bigDaysDB_New.ConnectToDB(sDBPathNew);
                if (bigDaysDB_New.CreateTableEnpty())
                {
                    bigDaysDB_New.Insert(Items_Old.ToArray());

                    System.IO.File.Delete(dbPath);
                }
            }

            }
            catch (Exception)
            {
                //              
            }
        }
    }
}