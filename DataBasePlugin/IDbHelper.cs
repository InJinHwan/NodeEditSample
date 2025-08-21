using System;
using System.Collections.Generic;
using System.Data;

namespace DataBasePlugin
{
    public interface IDbHelper : IDisposable
    {
        bool IsConnected { get; }

        void Connect();
        void Disconnect();

        void BeginTransaction();
        void Commit();
        void Rollback();

        int ExecuteNonQuery(string sql, List<IDataParameter> parameters = null);
        object ExecuteScalar(string sql, List<IDataParameter> parameters = null);
        DataTable ExecuteSelect(string sql, List<IDataParameter> parameters = null);

        Dictionary<string, object> ExecuteStoredProcedure(string procName, List<IDataParameter> parameters = null);
    }
}
