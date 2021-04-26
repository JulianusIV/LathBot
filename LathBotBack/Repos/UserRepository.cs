using System;
using System.Diagnostics;
using System.Data.SqlClient;

using LathBotBack.Base;
using LathBotBack.Models;

namespace LathBotBack.Repos
{
	public class UserRepository : RepositoryBase
	{
		public UserRepository(string connectionString) : base(connectionString) { }

		public bool Create(ref User entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Users (UserDcId) OUTPUT INSERTED.UserDbId VALUES (@DcId);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("DcId", entity.DcID);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				entity.ID = (int)reader["UserDbId"];
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}
			finally
			{
				if (DbConnection.State == System.Data.ConnectionState.Open)
				{
					DbConnection.Close();
				}
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
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new User
				{
					ID = id,
					DcID = (ulong)reader["UserDcId"]
				};
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}
			finally
			{
				if (DbConnection.State == System.Data.ConnectionState.Open)
				{
					DbConnection.Close();
				}
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
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}
			finally
			{
				if (DbConnection.State == System.Data.ConnectionState.Open)
				{
					DbConnection.Close();
				}
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
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}
			finally
			{
				if (DbConnection.State == System.Data.ConnectionState.Open)
				{
					DbConnection.Close();
				}
			}

			return result;
		}
	}
}
