using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace LathBotBack.Repos
{
    public class DiscordObjectRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool GetByName(string name, out DiscordObject entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT Id, ObjectId FROM DiscordObject WHERE ObjectName = @name;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("name", name);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new DiscordObject
                {
                    Id = (int)reader["Id"],
                    ObjectName = name,
                    ObjectId = (ulong)reader.GetInt64(1)
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
                    this.DbConnection.Close();
            }

            return result;
        }

        public bool GetAll(out List<DiscordObject> list)
        {
            bool result = false;
            list = [];
            try
            {
                this.DbCommand.CommandText = "SELECT Id, ObjectName, ObjectId FROM DiscordObject;";
                this.DbCommand.Parameters.Clear();
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DiscordObject
                    {
                        Id = (int)reader["Id"],
                        ObjectName = reader["ObjectName"].ToString(),
                        ObjectId = (ulong)reader.GetInt64(2)
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
                    this.DbConnection.Close();
            }
            return result;
        }
    }
}
