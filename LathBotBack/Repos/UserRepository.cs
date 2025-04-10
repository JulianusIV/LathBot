using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

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
                this.DbCommand.CommandText = "SELECT UserDcId, UserDbId, EmbedBanned, LastPunish FROM Users;";
                this.DbCommand.Parameters.Clear();
                this.DbConnection.Open();
                using SqlDataReader reader = this.DbCommand.ExecuteReader();
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

        public bool CountAll(out int amount)
        {
            bool result = false;
            amount = 0;

            try
            {
                this.DbCommand.CommandText = "SELECT COUNT(UserDcId) FROM Users;";
                this.DbCommand.Parameters.Clear();
                this.DbConnection.Open();
                amount = (int)this.DbCommand.ExecuteScalar();
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

        public bool ExistsDcId(ulong dcid, out bool exists)
        {
            bool result = false;
            exists = false;

            try
            {
                this.DbCommand.CommandText = "SELECT COUNT(UserDcId) FROM Users WHERE UserDcId = @dcid;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("dcid", (long)dcid);
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

        public bool GetIdByDcId(ulong DcId, out int id)
        {
            bool result = false;
            id = 0;

            try
            {
                this.DbCommand.CommandText = "SELECT UserDbId FROM Users WHERE UserDcId = @dcid;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("dcid", (long)DcId);
                this.DbConnection.Open();
                using SqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                id = (int)reader["UserDbId"];
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

        public bool UpdateLastPunish(int id, DateTime timeStamp)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Users SET LastPunish = @LastPunish WHERE UserDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("LastPunish", timeStamp);
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

        public bool Create(ref User entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "INSERT INTO Users (UserDcId, EmbedBanned, LastPunish) OUTPUT INSERTED.UserDbId VALUES (@DcId, @EmbedBanned, @LastPunish);";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("DcId", (long)entity.DcID);
                this.DbCommand.Parameters.AddWithValue("EmbedBanned", entity.EmbedBanned);
                if (entity.LastPunish is null)
                    this.DbCommand.Parameters.AddWithValue("LastPunish", DBNull.Value);
                else
                    this.DbCommand.Parameters.AddWithValue("LastPunish", entity.LastPunish);
                this.DbConnection.Open();
                using SqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity.ID = (int)reader["UserDbId"];
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

        public bool Read(int id, out User entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT UserDcId, EmbedBanned, LastPunish FROM Users WHERE UserDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                using SqlDataReader reader = this.DbCommand.ExecuteReader();
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

        public bool Update(User entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Users SET UserDcId = @DcId, EmbedBanned = @EmbedBanned, LastPunish = @LastPunish WHERE UserDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("DcId", (long)entity.DcID);
                this.DbCommand.Parameters.AddWithValue("EmbedBanned", entity.EmbedBanned);
                if (entity.LastPunish is null)
                    this.DbCommand.Parameters.AddWithValue("LastPunish", DBNull.Value);
                else
                    this.DbCommand.Parameters.AddWithValue("LastPunish", entity.LastPunish);
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
                this.DbCommand.CommandText = "DELETE FROM Users WHERE UserDbId = @id;";
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
