using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ezDeamon.cls
{
    interface iDbControl
    {
        string OLEDBCONNECTION_STRING { get; }

        string PROVIDER { get; }

        bool IsDBConnected { get; }

        void CreateConnectionStr(string sProvider, string sDataSource, string sUser, string sPwd, string sDefaultDB);

        bool DBConnect(out string sMsg);

        bool DBConnect(int iDelaySecond, out string sMsg);

        void DBDisconnect(out string sMsg);

        DbTransaction DBBeginTrans(out string sMsg);

        int DBRollBack(DbTransaction dbTrans, out string sMsg);

        int DBCommit(DbTransaction dbTrans, out string sMsg);
    }
}
