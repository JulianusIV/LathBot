using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace LathBotBack.Repos
{
    public class MuteRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool GetAll(out List<Mute> list)
        {
            bool result = false;
            list = [];

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Mutes;";
                this.DbCommand.Parameters.Clear();
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
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

        public bool IsUserMuted(int userId, out bool exists)
        {

            bool result = false;
            exists = false;

            try
            {
                this.DbCommand.CommandText = "SELECT COUNT(UserDbId) FROM Mutes WHERE UserDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", userId);
                this.DbConnection.Open();
                exists = (int)this.DbCommand.ExecuteScalar() > 0;
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

        public bool GetMuteByUser(int userId, out Mute entity)
        {

            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Mutes WHERE UserDbId = @user;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("user", userId);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new Mute
                {
                    Id = (int)reader["Id"],
                    User = userId,
                    Mod = (int)reader["ModDbId"],
                    Timestamp = (DateTime)reader["MuteTimestamp"],
                    Duration = (int)reader["MuteDuration"],
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

        public bool Create(ref Mute entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "INSERT INTO Mutes (UserDbId, ModDbId, MuteTimestamp, MuteDuration) VALUES (@user, @mod, @time, @duration) RETURNING Id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("user", entity.User);
                this.DbCommand.Parameters.AddWithValue("mod", entity.Mod);
                this.DbCommand.Parameters.AddWithValue("time", entity.Timestamp);
                this.DbCommand.Parameters.AddWithValue("duration", entity.Duration);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity.Id = (int)reader["Id"];
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

        public bool Read(int id, out Mute entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Mutes WHERE Id = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new Mute
                {
                    Id = id,
                    User = (int)reader["UserDbId"],
                    Mod = (int)reader["ModDbId"],
                    Timestamp = (DateTime)reader["MuteTimestamp"],
                    Duration = (int)reader["MuteDuration"],
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

        public bool Update(Mute entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Mutes SET UserDbId = @user, ModDbId = @mod, MuteTimestamp = @time, MuteDuration = @duration WHERE Id = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("user", entity.User);
                this.DbCommand.Parameters.AddWithValue("mod", entity.Mod);
                this.DbCommand.Parameters.AddWithValue("time", entity.Timestamp);
                this.DbCommand.Parameters.AddWithValue("duration", entity.Duration);
                this.DbCommand.Parameters.AddWithValue("id", entity.Id);
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
                this.DbCommand.CommandText = "DELETE FROM Mutes WHERE Id = @id;";
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
