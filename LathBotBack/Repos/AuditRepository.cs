using System;
using System.Data.SqlClient;

using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;

namespace LathBotBack.Repos
{
	public class AuditRepository : RepositoryBase
	{
		public AuditRepository(string connectionString) : base(connectionString) { }

		public bool Create(Audit entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Audits (ModDbId, WarnAmount, PardonAmount, MuteAmount, UnmuteAmount, KickAmount, BanAmount, TimeoutAmount) VALUES (@mod, @warns, @pardons, @mutes, @unmutes, @kicks, @bans, @timeouts);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("mod", entity.Mod);
				DbCommand.Parameters.AddWithValue("warns", entity.Warns);
				DbCommand.Parameters.AddWithValue("pardons", entity.Pardons);
				DbCommand.Parameters.AddWithValue("mutes", entity.Mutes);
				DbCommand.Parameters.AddWithValue("unmutes", entity.Unmutes);
				DbCommand.Parameters.AddWithValue("kicks", entity.Kicks);
				DbCommand.Parameters.AddWithValue("bans", entity.Bans);
				DbCommand.Parameters.AddWithValue("timeouts", entity.Timeouts);
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

		public bool Read(int id, out Audit entity)
		{
			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT WarnAmount, PardonAmount, MuteAmount, UnmuteAmount, KickAmount, BanAmount, TimeoutAmount FROM Audits WHERE ModDbId = @mod;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("mod", id);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new Audit
				{
					Mod = id,
					Warns = (int)reader["WarnAmount"],
					Pardons = (int)reader["PardonAmount"],
					Mutes = (int)reader["MuteAmount"],
					Unmutes = (int)reader["UnmuteAmount"],
					Kicks = (int)reader["KickAmount"],
					Bans = (int)reader["BanAmount"],
					Timeouts = (int)reader["TimeoutAmount"]
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

		public bool Update(Audit entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "UPDATE Audits SET WarnAmount = @warns, PardonAmount = @pardons, MuteAmount = @mutes, UnmuteAmount = @unmutes, KickAmount = @kicks, BanAmount = @bans, TimeoutAmount = @timeouts WHERE ModDbId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("warns", entity.Warns);
				DbCommand.Parameters.AddWithValue("pardons", entity.Pardons);
				DbCommand.Parameters.AddWithValue("mutes", entity.Mutes);
				DbCommand.Parameters.AddWithValue("unmutes", entity.Unmutes);
				DbCommand.Parameters.AddWithValue("kicks", entity.Kicks);
				DbCommand.Parameters.AddWithValue("bans", entity.Bans);
				DbCommand.Parameters.AddWithValue("timeouts", entity.Timeouts);
				DbCommand.Parameters.AddWithValue("id", entity.Mod);
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
				DbCommand.CommandText = "DELETE FROM Audits WHERE ModDbId = @id";
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
