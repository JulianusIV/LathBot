using LathBotBack.Base;
using LathBotBack.Models;
using System;
using System.Data.SqlClient;

namespace LathBotBack.Repos
{
	public class ModRepository : RepositoryBase
	{
		public ModRepository(string connectionString) : base(connectionString) { }

		public bool Create(ref Mod entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Mods (ModDbId, Timezone) OUTPUT INSERTED.Id VALUES (@dbid, @tz);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("dbid", entity.DbId);
				DbCommand.Parameters.AddWithValue("tz", entity.Timezone);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity.Id = (int)reader["Id"];
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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

		public bool Read(int id, out Mod entity)
		{
			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT * FROM Mods WHERE Id = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new Mod
				{
					Id = id,
					DbId = (int)reader["ModDbId"],
					Timezone = (string)reader["Timezone"]
				};
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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

		public bool Update(Mod entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "UPDATE Mods SET ModDbId = @dbid, Timezone = @tz WHERE Id = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("dbid", entity.DbId);
				DbCommand.Parameters.AddWithValue("tz", entity.Timezone);
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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
				DbCommand.CommandText = "DELETE FROM Mods WHERE Id = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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
