using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace RecipeManager
{
    public class MySqlProvider : IDisposable
    {
        private static MySqlProvider _instance;
        private MySqlConnection _connection;

        private MySqlProvider()
        {
            OpenNewConnection();
        }

        public static MySqlConnection Connection
        {
            get
            {
                if (_instance == null)
                    _instance = new MySqlProvider();
                if (_instance._connection.State == ConnectionState.Closed
                    || _instance._connection.State == ConnectionState.Broken)
                    _instance.OpenNewConnection();
                return _instance._connection;
            }
        }

        public void OpenNewConnection()
        {
            _connection?.Close();

            _connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);
            _connection.Open();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _connection.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MySqlProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}