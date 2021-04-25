﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace LathBotBack.Base
{
	class RepositoryBase
	{
		public SqlConnection DbConnection { get; set; }
		public SqlCommand DbCommand { get; set; }

		public string ConnectionString { get; set; }

		public RepositoryBase(string connectionString)
		{
			ConnectionString = connectionString;
			DbConnection = new SqlConnection(connectionString);
			DbCommand = new SqlCommand("", DbConnection);
		}

		public bool Check()
		{
			bool success = false;
			try
			{
				DbConnection.Open();
				DbConnection.Close();

				success = true;
			}
			catch (Exception ex)
			{
				//add logging
				Debug.WriteLine(ex);
			}

			return success;
		}
	}
}
