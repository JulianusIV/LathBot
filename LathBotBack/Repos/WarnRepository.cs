using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace LathBotBack.Repos
{
    public class WarnRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool GetAll(out List<Warn> list)
        {
            bool result = false;
            list = [];

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Warns;";
                this.DbCommand.Parameters.Clear();
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Warn
                    {
                        ID = (int)reader["WarnId"],
                        Level = (int)reader["WarnLevel"],
                        Mod = (int)reader["ModeratorDbId"],
                        Number = (int)reader["WarnNumber"],
                        Persistent = reader.GetBoolean("Persistent"),
                        Reason = (string)reader["Reason"],
                        Time = (DateTime)reader["WarnTime"],
                        User = (int)reader["UserDbId"],
                        ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool GetAllByUser(int UserDbId, out List<Warn> list)
        {
            bool result = false;
            list = [];

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Warns WHERE UserDbId = @id ORDER BY WarnNumber ASC;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", UserDbId);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Warn
                    {
                        ID = (int)reader["WarnId"],
                        Level = (int)reader["WarnLevel"],
                        Mod = (int)reader["ModeratorDbId"],
                        Number = (int)reader["WarnNumber"],
                        Persistent = reader.GetBoolean("Persistent"),
                        Reason = (string)reader["Reason"],
                        Time = (DateTime)reader["WarnTime"],
                        User = (int)reader["UserDbId"],
                        ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool GetWarnByUserAndNum(int UserDbId, int WarnNum, out Warn entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Warns WHERE UserDbId = @id AND WarnNumber = @num;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", UserDbId);
                this.DbCommand.Parameters.AddWithValue("num", WarnNum);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new Warn
                {
                    ID = (int)reader["WarnId"],
                    Level = (int)reader["WarnLevel"],
                    Mod = (int)reader["ModeratorDbId"],
                    Number = (int)reader["WarnNumber"],
                    Persistent = reader.GetBoolean("Persistent"),
                    Reason = (string)reader["Reason"],
                    Time = (DateTime)reader["WarnTime"],
                    User = (int)reader["UserDbId"],
                    ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool GetRemainingPoints(int UserDbId, out int amount)
        {
            bool result = false;
            amount = 15;

            try
            {
                this.DbCommand.CommandText = "SELECT WarnLevel FROM Warns WHERE UserDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", UserDbId);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    amount -= (int)reader["WarnLevel"];
                }

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

        public bool GetWarnAmount(int id, out int amount)
        {
            bool result = false;
            amount = 0;

            try
            {
                this.DbCommand.CommandText = "SELECT COUNT(UserDbId) FROM Warns WHERE UserDbId = @userid;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("userid", id);
                this.DbConnection.Open();
                amount = (int)(long)this.DbCommand.ExecuteScalar();
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

        public bool Create(ref Warn entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = $"INSERT INTO Warns (UserDbId, ModeratorDbId, Reason, WarnNumber, WarnLevel, WarnTime, Persistent) VALUES (@userid, @modid, @reason, @warnnum, @warnlevel, @warntime, @persistent) RETURNING WarnId;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("userid", entity.User);
                this.DbCommand.Parameters.AddWithValue("modid", entity.Mod);
                this.DbCommand.Parameters.AddWithValue("reason", entity.Reason);
                this.DbCommand.Parameters.AddWithValue("warnnum", entity.Number);
                this.DbCommand.Parameters.AddWithValue("warnlevel", entity.Level);
                this.DbCommand.Parameters.AddWithValue("warntime", entity.Time);
                this.DbCommand.Parameters.AddWithValue("persistent", entity.Persistent);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity.ID = (int)reader["WarnId"];
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

        public bool Read(int id, out Warn entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Warns WHERE WarnId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new Warn
                {
                    ID = (int)reader["WarnId"],
                    User = (int)reader["UserDbId"],
                    Mod = (int)reader["ModeratorDbId"],
                    Reason = (string)reader["Reason"],
                    Number = (int)reader["WarnNumber"],
                    Level = (int)reader["WarnLevel"],
                    Time = (DateTime)reader["WarnTime"],
                    Persistent = reader.GetBoolean("Persistent"),
                    ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool Update(Warn entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Warns SET UserDbId = @user, ModeratorDbId = @mod, Reason = @reason, WarnNumber = @num, WarnLevel = @level, WarnTime = @time, Persistent = @persistent, ExpirationTime = @expirationTime WHERE WarnId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("user", entity.User);
                this.DbCommand.Parameters.AddWithValue("mod", entity.Mod);
                this.DbCommand.Parameters.AddWithValue("reason", entity.Reason);
                this.DbCommand.Parameters.AddWithValue("num", entity.Number);
                this.DbCommand.Parameters.AddWithValue("level", entity.Level);
                this.DbCommand.Parameters.AddWithValue("time", entity.Time);
                this.DbCommand.Parameters.AddWithValue("persistent", entity.Persistent);
                if (entity.ExpirationTime is null)
                    this.DbCommand.Parameters.AddWithValue("expirationTime", DBNull.Value);
                else
                    this.DbCommand.Parameters.AddWithValue("expirationTime", entity.ExpirationTime);
                this.DbCommand.Parameters.AddWithValue("id", entity.ID);
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
                this.DbCommand.CommandText = "DELETE FROM Warns WHERE WarnId = @id;";
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
