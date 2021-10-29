﻿using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;

namespace LathBotBack.Repos
{
	public class MuteRepository : RepositoryBase
	{
		public MuteRepository(string connectionString) : base(connectionString) { }

		public bool GetAll(out List<Mute> list)
		{
			bool result = false;
			list = new List<Mute>();

			try
			{
				DbCommand.CommandText = "SELECT * FROM Mutes;";
				DbCommand.Parameters.Clear();
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				while (reader.Read())
				{
					list.Add(new Mute
					{
						Id = (int)reader["Id"],
						User = (int)reader["UserDbId"],
						Mod = (int)reader["ModDbId"],
						Timestamp = (DateTime)reader["MuteTimestamp"],
						Duration = (int)reader["MuteDuration"],
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

		public bool IsUserMuted(int userId, out bool exists)
		{

			bool result = false;
			exists = false;

			try
			{
				DbCommand.CommandText = "SELECT COUNT(UserDbId) FROM Mutes WHERE UserDbId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", userId);
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

		public bool GetMuteByUser(int userId, out Mute entity)
		{

			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT * FROM Mutes WHERE UserDbId = @user;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("user", userId);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new Mute
				{
					Id = (int)reader["Id"],
					User = userId,
					Mod = (int)reader["ModDbId"],
					Timestamp = (DateTime)reader["MuteTimestamp"],
					Duration = (int)reader["MuteDuration"],
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

		public bool Create(ref Mute entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Mutes (UserDbId, ModDbId, MuteTimestamp, MuteDuration) OUTPUT INSERTED.Id VALUES (@user, @mod, @time, @duration);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("user", entity.User);
				DbCommand.Parameters.AddWithValue("mod", entity.Mod);
				DbCommand.Parameters.AddWithValue("time", entity.Timestamp);
				DbCommand.Parameters.AddWithValue("duration", entity.Duration);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity.Id = (int)reader["Id"];
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

		public bool Read(int id, out Mute entity)
		{
			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT * FROM Mutes WHERE Id = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new Mute
				{
					Id = id,
					User = (int)reader["UserDbId"],
					Mod = (int)reader["ModDbId"],
					Timestamp = (DateTime)reader["MuteTimestamp"],
					Duration = (int)reader["MuteDuration"],
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

		public bool Update(Mute entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "UPDATE Mutes SET UserDbId = @user, ModDbId = @mod, MuteTimestamp = @time, MuteDuration = @duration WHERE Id = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("user", entity.User);
				DbCommand.Parameters.AddWithValue("mod", entity.Mod);
				DbCommand.Parameters.AddWithValue("time", entity.Timestamp);
				DbCommand.Parameters.AddWithValue("duration", entity.Duration);
				DbCommand.Parameters.AddWithValue("id", entity.Id);
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
				DbCommand.CommandText = "DELETE FROM Mutes WHERE Id = @id;";
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
