using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;

namespace LathBotBack.Repos
{
	public class UserRepository : RepositoryBase
	{
		public UserRepository(string connectionString) : base(connectionString) { }

		public bool GetAll(out List<User> list)
		{
			bool result = false;
			list = new List<User>();

			try
			{
				DbCommand.CommandText = "SELECT * FROM Users;";
				DbCommand.Parameters.Clear();
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				while (reader.Read())
				{
					long temp = (long)reader["UserDcId"];
					list.Add(new User
					{
						DcID = (ulong)temp,
						ID = (int)reader["UserDbId"]
					});
				}
				DbConnection.Close();
				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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

		public bool CountAll(out int amount)
		{
			bool result = false;
			amount = 0;

			try
			{
				DbCommand.CommandText = "SELECT COUNT(UserDcId) FROM Users;";
				DbCommand.Parameters.Clear();
				DbConnection.Open();
				amount = (int)DbCommand.ExecuteScalar();
				DbConnection.Close();
				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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

		public bool ExistsDcId(ulong dcid, out bool exists)
		{
			bool result = false;
			exists = false;

			try
			{
				DbCommand.CommandText = "SELECT COUNT(UserDcId) FROM Users WHERE UserDcId = @dcid;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("dcid", (long)dcid);
				DbConnection.Open();
				exists = (int)DbCommand.ExecuteScalar() > 0;
				DbConnection.Close();
				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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

		public bool GetIdByDcId(ulong DcId, out int id)
		{
			bool result = false;
			id = 0;

			try
			{
				DbCommand.CommandText = "SELECT UserDbId FROM Users WHERE UserDcId = @dcid;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("dcid", (long)DcId);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				id = (int)reader["UserDbId"];
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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

		public bool Create(ref User entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Users (UserDcId) OUTPUT INSERTED.UserDbId VALUES (@DcId);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("DcId", (long)entity.DcID);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity.ID = (int)reader["UserDbId"];
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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
				long temp = (long)reader["UserDcId"];
				entity = new User
				{
					ID = id,
					DcID = (ulong)temp
				};
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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
				DbCommand.Parameters.AddWithValue("DcId", (long)entity.DcID);
				DbCommand.Parameters.AddWithValue("id", entity.ID);
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				SystemService.Instance.Logger.Log(e.Message);
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
				SystemService.Instance.Logger.Log(e.Message);
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
