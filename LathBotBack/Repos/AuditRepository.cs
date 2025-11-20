using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using MySql.Data.MySqlClient;
using System;

namespace LathBotBack.Repos
{
    public class AuditRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool Create(Audit entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "INSERT INTO Audits (ModDbId, WarnAmount, PardonAmount, MuteAmount, UnmuteAmount, KickAmount, BanAmount, TimeoutAmount) VALUES (@mod, @warns, @pardons, @mutes, @unmutes, @kicks, @bans, @timeouts);";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("mod", entity.Mod);
                this.DbCommand.Parameters.AddWithValue("warns", entity.Warns);
                this.DbCommand.Parameters.AddWithValue("pardons", entity.Pardons);
                this.DbCommand.Parameters.AddWithValue("mutes", entity.Mutes);
                this.DbCommand.Parameters.AddWithValue("unmutes", entity.Unmutes);
                this.DbCommand.Parameters.AddWithValue("kicks", entity.Kicks);
                this.DbCommand.Parameters.AddWithValue("bans", entity.Bans);
                this.DbCommand.Parameters.AddWithValue("timeouts", entity.Timeouts);
                this.DbConnection.Open();
                this.DbCommand.ExecuteNonQuery();
                this.DbConnection.Close();

                result = true;
            }
            catch (Exception e)
            {
                SystemService.Instance.Logger.Log(e.Message);
            }
            finally
            {
                if (this.DbConnection.State == System.Data.ConnectionState.Open)
                {
                    this.DbConnection.Close();
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
                this.DbCommand.CommandText = "SELECT WarnAmount, PardonAmount, MuteAmount, UnmuteAmount, KickAmount, BanAmount, TimeoutAmount FROM Audits WHERE ModDbId = @mod;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("mod", id);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new()
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
                this.DbConnection.Close();

                result = true;
            }
            catch (Exception e)
            {
                SystemService.Instance.Logger.Log(e.Message);
            }
            finally
            {
                if (this.DbConnection.State == System.Data.ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return result;
        }

        public bool Update(Audit entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Audits SET WarnAmount = @warns, PardonAmount = @pardons, MuteAmount = @mutes, UnmuteAmount = @unmutes, KickAmount = @kicks, BanAmount = @bans, TimeoutAmount = @timeouts WHERE ModDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("warns", entity.Warns);
                this.DbCommand.Parameters.AddWithValue("pardons", entity.Pardons);
                this.DbCommand.Parameters.AddWithValue("mutes", entity.Mutes);
                this.DbCommand.Parameters.AddWithValue("unmutes", entity.Unmutes);
                this.DbCommand.Parameters.AddWithValue("kicks", entity.Kicks);
                this.DbCommand.Parameters.AddWithValue("bans", entity.Bans);
                this.DbCommand.Parameters.AddWithValue("timeouts", entity.Timeouts);
                this.DbCommand.Parameters.AddWithValue("id", entity.Mod);
                this.DbConnection.Open();
                this.DbCommand.ExecuteNonQuery();
                this.DbConnection.Close();

                result = true;
            }
            catch (Exception e)
            {
                SystemService.Instance.Logger.Log(e.Message);
            }
            finally
            {
                if (this.DbConnection.State == System.Data.ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return result;
        }

        public bool Delete(int id)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "DELETE FROM Audits WHERE ModDbId = @id";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                this.DbCommand.ExecuteNonQuery();
                this.DbConnection.Close();

                result = true;
            }
            catch (Exception e)
            {
                SystemService.Instance.Logger.Log(e.Message);
            }
            finally
            {
                if (this.DbConnection.State == System.Data.ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return result;
        }
    }
}
