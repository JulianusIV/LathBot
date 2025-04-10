﻿using LathBotBack.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics;

namespace LathBotBack.Base
{
    public class RepositoryBase
    {
        public SqlConnection DbConnection { get; set; }
        public SqlCommand DbCommand { get; set; }

        public string ConnectionString { get; set; }

        public RepositoryBase(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.DbConnection = new(connectionString);
            this.DbCommand = new("", this.DbConnection);
        }

        public bool Check()
        {
            bool success = false;
            try
            {
                this.DbConnection.Open();
                this.DbConnection.Close();

                success = true;
            }
            catch (Exception ex)
            {
                SystemService.Instance.Logger.Log(ex.Message);
                Debug.WriteLine(ex);
            }

            return success;
        }
    }
}
