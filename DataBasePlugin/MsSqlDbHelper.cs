using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataBasePlugin
{
    /// <summary>
    /// Microsoft SQL Server 전용 DB 헬퍼 클래스
    /// </summary>
    public class MsSqlDbHelper : IDbHelper
    {
        private readonly string _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private bool _disposed = false;

        public MsSqlDbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool IsConnected => _connection != null && _connection.State == ConnectionState.Open;

        public void Connect()
        {
            if (_connection == null)
                _connection = new SqlConnection(_connectionString);

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
            using (var adapter = new SqlDataAdapter(cmd))
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
                cmd.ExecuteNonQuery();

                var outputValues = new Dictionary<string, object>();
                foreach (SqlParameter param in cmd.Parameters)
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

        private SqlCommand CreateCommand(string text, CommandType type, List<IDataParameter> parameters = null)
        {
            Connect();
            var cmd = _connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = type;
            cmd.Transaction = _transaction;

            if (parameters != null)
            {
                foreach (var param in parameters)
                    cmd.Parameters.Add((SqlParameter)param);
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
