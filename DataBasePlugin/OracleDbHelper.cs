using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace DataBasePlugin
{
    public class OracleDbHelper : IDbHelper
    {
        private readonly string _connectionString;
        private OracleConnection _connection;
        private OracleTransaction _transaction;
        private bool _disposed = false;

        public OracleDbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool IsConnected => _connection != null && _connection.State == ConnectionState.Open;

        public void Connect()
        {
            if (_connection == null)
                _connection = new OracleConnection(_connectionString);

            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        public void Disconnect()
        {
            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();

                _connection.Dispose();
                _connection = null;
            }

            _transaction?.Dispose();
            _transaction = null;
        }

        public void BeginTransaction()
        {
            Connect();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        // [수정] OracleParameter → IDataParameter
        public int ExecuteNonQuery(string sql, List<IDataParameter> parameters = null)
        {
            using (var cmd = CreateCommand(sql, CommandType.Text, parameters))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string sql, List<IDataParameter> parameters = null)
        {
            using (var cmd = CreateCommand(sql, CommandType.Text, parameters))
            {
                return cmd.ExecuteScalar();
            }
        }

        public DataTable ExecuteSelect(string sql, List<IDataParameter> parameters = null)
        {
            using (var cmd = CreateCommand(sql, CommandType.Text, parameters))
            using (var adapter = new OracleDataAdapter(cmd))
            {
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public Dictionary<string, object> ExecuteStoredProcedure(string procName, List<IDataParameter> parameters = null)
        {
            if (parameters == null)
                parameters = new List<IDataParameter>();

            using (var cmd = CreateCommand(procName, CommandType.StoredProcedure, parameters))
            {
                var outputValues = new Dictionary<string, object>();
                OracleParameter refCursorParam = null;

                // [수정] cmd.Parameters 에서 직접 검사
                foreach (OracleParameter param in cmd.Parameters)
                {
                    if (param.OracleDbType == OracleDbType.RefCursor)
                    {
                        refCursorParam = param;
                        break;
                    }
                }

                if (refCursorParam != null)
                {
                    using (var adapter = new OracleDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        outputValues["__REFCURSOR__"] = dt;
                    }
                }
                else
                {
                    cmd.ExecuteNonQuery();
                }

                foreach (OracleParameter param in cmd.Parameters)
                {
                    if (param.Direction == ParameterDirection.Output ||
                        param.Direction == ParameterDirection.InputOutput ||
                        param.Direction == ParameterDirection.ReturnValue)
                    {
                        outputValues[param.ParameterName] = param.Value;
                    }
                }

                return outputValues;
            }
        }

        // [수정] OracleParameter → IDataParameter
        private OracleCommand CreateCommand(string text, CommandType type, List<IDataParameter> parameters = null)
        {
            Connect();
            var cmd = _connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = type;
            cmd.Transaction = _transaction;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add((OracleParameter)param); // [수정] 명시적 캐스팅
                }
            }

            return cmd;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                _transaction = null;

                Disconnect();
                _disposed = true;
            }
        }
    }
}
