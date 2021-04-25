using LathBotBack.Base;
using LathBotBack.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace LathBotBack.Repos
{
	public class WarnRepository : RepositoryBase
	{
		public WarnRepository(string connectionString) : base(connectionString) { }

		public bool Create(ref Warn entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Warns (UserDbId, ModeratorDbId, Reason, WarnNumber, WarnLevel, WarnTime) OUTPUT INSERTED.WarnId VALUES (@userid, @modid, @reason, @warnnum, @warnlevel, @warntime);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("userid", entity.User);
				DbCommand.Parameters.AddWithValue("modid", entity.Mod);
				DbCommand.Parameters.AddWithValue("reason", entity.Reason);
				DbCommand.Parameters.AddWithValue("warnnum", entity.Number);
				DbCommand.Parameters.AddWithValue("warnlevel", entity.Level);
				DbCommand.Parameters.AddWithValue("warntime", entity.Time);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity.ID = (int)reader["WarnId"];
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

		public bool Read(int id, out Warn entity)
		{
			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT * FROM Warns WHERE WarnId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new Warn
				{
					ID = (int)reader["WarnId"],
					User = (int)reader["UserDbId"],
					Mod = (int)reader["ModeratorDbId"],
					Reason = (string)reader["Reason"],
					Number = (int)reader["WarnNumber"],
					Level = (int)reader["WarnLevel"],
					Time = (DateTime)reader["WarnTime"]
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

		public bool Update(Warn entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "UPDATE Warns SET UserDbId = @user, ModeratorDbId = @mod, Reason = @reason, WarnNumber = @num, WarnLevel = @level, WarnTime = @time WHERE WarnId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("user", entity.User);
				DbCommand.Parameters.AddWithValue("mod", entity.Mod);
				DbCommand.Parameters.AddWithValue("reason", entity.Reason);
				DbCommand.Parameters.AddWithValue("num", entity.Number);
				DbCommand.Parameters.AddWithValue("level", entity.Level);
				DbCommand.Parameters.AddWithValue("time", entity.Time);
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
				DbCommand.CommandText = "DELETE FROM Warns WHERE WarnId = @id;";
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
