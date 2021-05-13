﻿using System;
using System.Data.SqlClient;

using LathBotBack.Base;
using LathBotBack.Models;

namespace LathBotBack.Repos
{
	public class VariableRepository : RepositoryBase
	{
		public VariableRepository(string connectionString) : base(connectionString) { }

		public bool Create(ref Variable entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Variables (VarName, VarValue) OUTPUT INSERTED.VarId VALUES (@name, @val);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("name", entity.Name);
				DbCommand.Parameters.AddWithValue("val", entity.Value);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity.ID = (int)reader["VarId"];
				DbConnection.Close();
				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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

		public bool Read(int id, out Variable entity)
		{
			bool result = false;
			entity = null;

			try
			{
				DbCommand.CommandText = "SELECT VarName, VarValue FROM Variables WHERE VarId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				DbConnection.Open();
				using SqlDataReader reader = DbCommand.ExecuteReader();
				reader.Read();
				entity = new Variable
				{
					ID = id,
					Name = (string)reader["VarName"],
					Value = (string)reader["VarValue"]
				};
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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

		public bool Update(Variable entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "UPDATE Variables SET VarName = @name, VarValue = @val WHERE VarId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("name", entity.Name);
				DbCommand.Parameters.AddWithValue("val", entity.Value);
				DbCommand.Parameters.AddWithValue("id", entity.ID);
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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
				DbCommand.CommandText = "DELETE FROM Variables WHERE VarId = @id;";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("id", id);
				DbConnection.Open();
				DbCommand.ExecuteNonQuery();
				DbConnection.Close();

				result = true;
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
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
