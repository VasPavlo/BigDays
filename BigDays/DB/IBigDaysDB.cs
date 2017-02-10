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
using Mono.Data.Sqlite;
using BigDays.Models;

namespace BigDays.DB
{
    public interface IBigDaysDB
    {
        bool ConnectToDB(string sDBPath);
        bool CreateTable();
        bool TableExists(String tableName, SqliteConnection connection);
        bool Insert(BigDaysItemModel[] BDItems);
        bool Insert(BigDaysItemModel BDItems);
        bool Update(BigDaysItemModel BDItems);
        bool UpdatePos(BigDaysItemModel BDItems);
        List<BigDaysItemModel> SelectBDItems();
        BigDaysItemModel SelectItem(int ID);
        BigDaysItemModel GetLastAddItem();
        int GetLastID();
        BigDaysItemModel GetCurrentItem();
        void Delete(int ID);
        void SetActive(int ID);
        void CheckRepeats();
        BigDaysItemModel CheckRepeat(BigDaysItemModel item);
    }        
}