using LathBotBack.Base;
using LathBotBack.Models;
using LathBotBack.Services;
using MySqlConnector;
using System;

namespace LathBotBack.Repos
{
    public class VariableRepository(string connectionString) : RepositoryBase(connectionString)
    {
        public bool Create(ref Variable entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "INSERT INTO Variables (VarName, VarValue) OUTPUT INSERTED.VarId VALUES (@name, @val);";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("name", entity.Name);
                this.DbCommand.Parameters.AddWithValue("val", entity.Value);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity.ID = (int)reader["VarId"];
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

        public bool Read(int id, out Variable entity)
        {
            bool result = false;
            entity = null;

            try
            {
                this.DbCommand.CommandText = "SELECT VarName, VarValue FROM Variables WHERE VarId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("id", id);
                this.DbConnection.Open();
                using MySqlDataReader reader = this.DbCommand.ExecuteReader();
                reader.Read();
                entity = new Variable
                {
                    ID = id,
                    Name = (string)reader["VarName"],
                    Value = (string)reader["VarValue"]
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

        public bool Update(Variable entity)
        {
            bool result = false;

            try
            {
                this.DbCommand.CommandText = "UPDATE Variables SET VarName = @name, VarValue = @val WHERE VarId = @id;";
                this.DbCommand.Parameters.Clear();
                this.DbCommand.Parameters.AddWithValue("name", entity.Name);
                this.DbCommand.Parameters.AddWithValue("val", entity.Value);
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
                this.DbCommand.CommandText = "DELETE FROM Variables WHERE VarId = @id;";
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
