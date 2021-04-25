using LathBotBack.Base;
using LathBotBack.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace LathBotBack.Repos
{
	class VariableRepository : RepositoryBase
	{
		public VariableRepository(string connectionString) : base(connectionString) { }

		public bool Create(Variable entity)
		{
			bool result = false;

			try
			{
				DbCommand.CommandText = "INSERT INTO Variables (VarName, VarValue) OUTPUT INSERTED.VarId VALUES (@name, @val);";
				DbCommand.Parameters.Clear();
				DbCommand.Parameters.AddWithValue("name", entity.Name);
				DbCommand.Parameters.AddWithValue("val", entity.Value);
				using (DbConnection)
				using (SqlDataReader reader = DbCommand.ExecuteReader())
				{
					DbConnection.Open();
					reader.Read();
					entity.ID = (int)reader["VarId"];
				}
				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
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
				using (DbConnection)
				using (SqlDataReader reader = DbCommand.ExecuteReader())
				{
					DbConnection.Open();
					reader.Read();
					entity = new Variable
					{
						ID = id,
						Name = (string)reader["VarName"],
						Value = (string)reader["VarValue"]
					};
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
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
				using (DbConnection)
				{
					DbConnection.Open();
					DbCommand.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
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
				using (DbConnection)
				{
					DbConnection.Open();
					DbCommand.ExecuteNonQuery();
				}

				result = true;
			}
			catch (Exception e)
			{
				//Add logging
				Debug.WriteLine(e.Message);
			}

			return result;
		}
	}
}
