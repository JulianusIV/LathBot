using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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
                this.DbCommand.CommandText = "SELECT * FROM Mods;";
                this.DbCommand.Parameters.Clear();
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
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

        public bool GetModById(int id, out Mod entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Mods WHERE ModDbId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
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

        public bool Create(ref Mod entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "INSERT INTO Mods (ModDbId, Timezone) OUTPUT INSERTED.Id VALUES (@dbid, @tz);";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("dbid", entity.DbId);
                this.DbCommand.Parameters.AddWithValue("tz", entity.Timezone);
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

        public bool Read(int id, out Mod entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT * FROM Mods WHERE Id = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
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

        public bool Update(Mod entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Mods SET ModDbId = @dbid, Timezone = @tz, TwoFAKey = @twofa, TwoFAKeySalt = @twofasalt WHERE Id = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("dbid", entity.DbId);
                this.DbCommand.Parameters.AddWithValue("tz", entity.Timezone);
                var param = this.DbCommand.Parameters.AddWithValue("twofa", entity.TwoFAKey);
                param.DbType = System.Data.DbType.Binary;
                this.DbCommand.Parameters.AddWithValue("twofasalt", entity.TwoFAKeySalt);
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
                this.DbCommand.CommandText = "DELETE FROM Mods WHERE Id = @id;";
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
