using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
                DbCommand.CommandText = "SELECT * FROM Warns;";
                DbCommand.Parameters.Clear();
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Warn
                    {
                        ID = (int)reader["WarnId"],
                        Level = (int)reader["WarnLevel"],
                        Mod = (int)reader["ModeratorDbId"],
                        Number = (int)reader["WarnNumber"],
                        Persistent = (bool)reader["Persistent"],
                        Reason = (string)reader["Reason"],
                        Time = (DateTime)reader["WarnTime"],
                        User = (int)reader["UserDbId"],
                        ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool GetAllByUser(int UserDbId, out List<Warn> list)
        {
            bool result = false;
            list = [];

            try
            {
                DbCommand.CommandText = "SELECT * FROM Warns WHERE UserDbId = @id ORDER BY WarnNumber ASC;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("id", UserDbId);
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Warn
                    {
                        ID = (int)reader["WarnId"],
                        Level = (int)reader["WarnLevel"],
                        Mod = (int)reader["ModeratorDbId"],
                        Number = (int)reader["WarnNumber"],
                        Persistent = (bool)reader["Persistent"],
                        Reason = (string)reader["Reason"],
                        Time = (DateTime)reader["WarnTime"],
                        User = (int)reader["UserDbId"],
                        ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool GetWarnByUserAndNum(int UserDbId, int WarnNum, out Warn entity)
        {
            bool result = false;
            entity = null;

            try
            {
                DbCommand.CommandText = "SELECT * FROM Warns WHERE UserDbId = @id AND WarnNumber = @num;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("id", UserDbId);
                DbCommand.Parameters.AddWithValue("num", WarnNum);
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                reader.Read();
                entity = new Warn
                {
                    ID = (int)reader["WarnId"],
                    Level = (int)reader["WarnLevel"],
                    Mod = (int)reader["ModeratorDbId"],
                    Number = (int)reader["WarnNumber"],
                    Persistent = (bool)reader["Persistent"],
                    Reason = (string)reader["Reason"],
                    Time = (DateTime)reader["WarnTime"],
                    User = (int)reader["UserDbId"],
                    ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool GetRemainingPoints(int UserDbId, out int amount)
        {
            bool result = false;
            amount = 15;

            try
            {
                DbCommand.CommandText = "SELECT WarnLevel FROM Warns WHERE UserDbId = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("id", UserDbId);
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
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
                if (DbConnection.State == System.Data.ConnectionState.Open)
                {
                    DbConnection.Close();
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
                DbCommand.CommandText = "SELECT COUNT(UserDbId) FROM Warns WHERE UserDbId = @userid;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("userid", id);
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

        public bool Create(ref Warn entity)
        {
            bool result = false;

            try
            {
                DbCommand.CommandText = $"INSERT INTO Warns (UserDbId, ModeratorDbId, Reason, WarnNumber, WarnLevel, WarnTime, Persistent) OUTPUT INSERTED.WarnId VALUES (@userid, @modid, @reason, @warnnum, @warnlevel, @warntime, @persistent);";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("userid", entity.User);
                DbCommand.Parameters.AddWithValue("modid", entity.Mod);
                DbCommand.Parameters.AddWithValue("reason", entity.Reason);
                DbCommand.Parameters.AddWithValue("warnnum", entity.Number);
                DbCommand.Parameters.AddWithValue("warnlevel", entity.Level);
                DbCommand.Parameters.AddWithValue("warntime", entity.Time);
                DbCommand.Parameters.AddWithValue("persistent", entity.Persistent);
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                reader.Read();
                entity.ID = (int)reader["WarnId"];
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
                    Time = (DateTime)reader["WarnTime"],
                    Persistent = (bool)reader["Persistent"],
                    ExpirationTime = (int?)(reader["ExpirationTime"] is DBNull ? null : reader["ExpirationTime"])
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

        public bool Update(Warn entity)
        {
            bool result = false;

            try
            {
                DbCommand.CommandText = "UPDATE Warns SET UserDbId = @user, ModeratorDbId = @mod, Reason = @reason, WarnNumber = @num, WarnLevel = @level, WarnTime = @time, Persistent = @persistent, ExpirationTime = @expirationTime WHERE WarnId = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("user", entity.User);
                DbCommand.Parameters.AddWithValue("mod", entity.Mod);
                DbCommand.Parameters.AddWithValue("reason", entity.Reason);
                DbCommand.Parameters.AddWithValue("num", entity.Number);
                DbCommand.Parameters.AddWithValue("level", entity.Level);
                DbCommand.Parameters.AddWithValue("time", entity.Time);
                DbCommand.Parameters.AddWithValue("persistent", entity.Persistent);
                if (entity.ExpirationTime is null)
                    DbCommand.Parameters.AddWithValue("expirationTime", DBNull.Value);
                else
                    DbCommand.Parameters.AddWithValue("expirationTime", entity.ExpirationTime);
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
