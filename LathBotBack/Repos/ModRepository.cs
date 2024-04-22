using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LathBotBack.Repos
{
    public class ModRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool GetAll(out List<Mod> list)
        {
            bool result = false;
            list = null;

            try
            {
                DbCommand.CommandText = "SELECT * FROM Mods;";
                DbCommand.Parameters.Clear();
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                list = [];
                while (reader.Read())
                {
                    var mod = new Mod
                    {
                        Id = (int)reader["Id"],
                        DbId = (int)reader["ModDbId"],
                        Timezone = (string)reader["Timezone"]
                    };

                    if (reader.IsDBNull(3))
                        mod.TwoFAKey = null;
                    else
                        mod.TwoFAKey = (byte[])reader["TwoFAKey"];

                    if (reader.IsDBNull(4))
                        mod.TwoFAKeySalt = null;
                    else
                        mod.TwoFAKeySalt = (string)reader["TwoFAKeySalt"];

                    list.Add(mod);
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

        public bool GetModById(int id, out Mod entity)
        {
            bool result = false;
            entity = null;

            try
            {
                DbCommand.CommandText = "SELECT * FROM Mods WHERE ModDbId = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("id", id);
                DbConnection.Open();
                using SqlDataReader reader = DbCommand.ExecuteReader();
                reader.Read();
                entity = new()
                {
                    Id = (int)reader["Id"],
                    DbId = id,
                    Timezone = (string)reader["Timezone"]
                };

                if (reader.IsDBNull(3))
                    entity.TwoFAKey = null;
                else
                    entity.TwoFAKey = (byte[])reader["TwoFAKey"];

                if (reader.IsDBNull(4))
                    entity.TwoFAKeySalt = null;
                else
                    entity.TwoFAKeySalt = (string)reader["TwoFAKeySalt"];

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

                if (reader.IsDBNull(3))
                    entity.TwoFAKey = null;
                else
                    entity.TwoFAKey = (byte[])reader["TwoFAKey"];

                if (reader.IsDBNull(4))
                    entity.TwoFAKeySalt = null;
                else
                    entity.TwoFAKeySalt = (string)reader["TwoFAKeySalt"];

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

        public bool Update(Mod entity)
        {
            bool result = false;

            try
            {
                DbCommand.CommandText = "UPDATE Mods SET ModDbId = @dbid, Timezone = @tz, TwoFAKey = @twofa, TwoFAKeySalt = @twofasalt WHERE Id = @id;";
                DbCommand.Parameters.Clear();
                DbCommand.Parameters.AddWithValue("dbid", entity.DbId);
                DbCommand.Parameters.AddWithValue("tz", entity.Timezone);
                var param = DbCommand.Parameters.AddWithValue("twofa", entity.TwoFAKey);
                param.DbType = System.Data.DbType.Binary;
                DbCommand.Parameters.AddWithValue("twofasalt", entity.TwoFAKeySalt);
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
