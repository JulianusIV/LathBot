using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LathBotBack.Repos
{
    public class UserRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool GetAll(out List<User> list)
        {
            bool result = false;
            list = [];

            try
            {
                DbCommand.CommandText = "SELECT UserDcId, UserDbId, EmbedBanned, LastPunish FROM Users;";
                DbCommand.Parameters.Clear();
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    long temp = reader.GetInt64(0);
                    var user = new User()
                    {
                        DcID = (ulong)temp,
                        ID = reader.GetInt32(1),
                        EmbedBanned = reader.GetBoolean(2)
                    };
                    if (reader.IsDBNull(3))
                        user.LastPunish = null;
                    else user.LastPunish = reader.GetDateTime(3);

                    list.Add(user);
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

        public bool UpdateLastPunish(int id, DateTime timeStamp)
        {
            bool result = false;

            try
            {
                DbCommand.CommandText = "UPDATE Users SET LastPunish = @LastPunish WHERE UserDbId = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("LastPunish", timeStamp);
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

        public bool Create(ref User entity)
        {
            bool result = false;

            try
            {
                DbCommand.CommandText = "INSERT INTO Users (UserDcId, EmbedBanned, LastPunish) OUTPUT INSERTED.UserDbId VALUES (@DcId, @EmbedBanned, @LastPunish);";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("DcId", (long)entity.DcID);
                DbCommand.Parameters.AddWithValue("EmbedBanned", entity.EmbedBanned);
                if (entity.LastPunish is null)
                    DbCommand.Parameters.AddWithValue("LastPunish", DBNull.Value);
                else
                    DbCommand.Parameters.AddWithValue("LastPunish", entity.LastPunish);
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
                DbCommand.CommandText = "SELECT UserDcId, EmbedBanned, LastPunish FROM Users WHERE UserDbId = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("id", id);
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                reader.Read();
                long temp = reader.GetInt64(0);
                entity = new User
                {
                    ID = id,
                    DcID = (ulong)temp,
                    EmbedBanned = reader.GetBoolean(1)
                };
                if (reader.IsDBNull(2))
                    entity.LastPunish = null;
                else entity.LastPunish = reader.GetDateTime(2);

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
                DbCommand.CommandText = "UPDATE Users SET UserDcId = @DcId, EmbedBanned = @EmbedBanned, LastPunish = @LastPunish WHERE UserDbId = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("DcId", (long)entity.DcID);
                DbCommand.Parameters.AddWithValue("EmbedBanned", entity.EmbedBanned);
                if (entity.LastPunish is null)
                    DbCommand.Parameters.AddWithValue("LastPunish", DBNull.Value);
                else
                    DbCommand.Parameters.AddWithValue("LastPunish", entity.LastPunish);
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
