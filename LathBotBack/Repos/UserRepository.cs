using LathBotBack.Base;
using LathBotBack.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace LathBotBack.Repos
{
	class UserRepository : RepositoryBase
	{
		public UserRepository(string connectionString) : base(connectionString) { }

		public bool Create(User entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Users (UserDcId) OUTPUT INSERTED.UserDbId VALUES (@DcId);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("DcId", entity.DcID);
				using (DbConnection)
				using (SqlDataReader reader = DbCommand.ExecuteReader())
				{
					DbConnection.Open();
					entity.ID = (int)reader["UserDbId"];
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}

			return result;
		}

		public bool Read(int id, out User entity)
		{
			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT UserDcId FROM Users WHERE UserDbId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				using (DbConnection)
				using (SqlDataReader reader = DbCommand.ExecuteReader())
				{
					DbConnection.Open();
					reader.Read();
					entity = new User
					{
						ID = id,
						DcID = (ulong)reader["UserDcId"]
					};
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}

			return result;
		}

		public bool Update(User entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "UPDATE Users SET UserDcId = @DcId WHERE UserDbId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("DcId", entity.DcID);
				DbCommand.Parameters.AddWithValue("id", entity.ID);
				using (DbConnection)
				{
					DbConnection.Open();
					DbCommand.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}

			return result;
		}

		public bool Delete(int id)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "DELETE FROM Users WHERE UserDbId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				using (DbConnection)
				{
					DbConnection.Open();
					DbCommand.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}

			return result;
		}
	}
}
