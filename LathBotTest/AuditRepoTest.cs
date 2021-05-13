using System;
using System.Data.SqlClient;

using LathBotBack;
using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;

using NUnit.Framework;

namespace LathBotTest
{
	class AuditRepoTest
	{
		readonly Audit _obj;
		AuditRepository _objRepo;

		public AuditRepoTest()
		{
			_obj = new Audit
			{
				Warns = 0,
				Pardons = 0,
				Mutes = 0,
				Unmutes = 0,
				Kicks = 0,
				Bans = 0
			};
		}

		[SetUp]
		public void Setup()
		{
			ReadConfig.Read();
			_objRepo = new AuditRepository(ReadConfig.configJson.ConnectionString);
		}

		[Test]
		public void TestAuditRepository()
		{
			CreateUser();

			TestCreate();

			TestRead();

			TestUpdate();

			TestDelete();

			Cleanup();
		}

		private void CreateUser()
		{
			try
			{
				_objRepo.DbCommand.CommandText = "INSERT INTO Users (UserDcId) OUTPUT INSERTED.UserDbId VALUES (111111111111111111);";
				_objRepo.DbCommand.Parameters.Clear();
				_objRepo.DbConnection.Open();
				using SqlDataReader reader = _objRepo.DbCommand.ExecuteReader();
				reader.Read();
				_obj.Mod = (int)reader["UserDbId"];
				_objRepo.DbConnection.Close();
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
			}
		}

		private void TestCreate()
		{
			bool result = _objRepo.Create(_obj);

			Assert.IsTrue(result);
		}

		private void TestRead()
		{
			bool result = _objRepo.Read(_obj.Mod, out Audit entity);

			Assert.IsTrue(result);
			Assert.AreEqual(_obj.Mod, entity.Mod);
			Assert.AreEqual(_obj.Warns, entity.Warns);
			Assert.AreEqual(_obj.Pardons, entity.Pardons);
			Assert.AreEqual(_obj.Mutes, entity.Mutes);
			Assert.AreEqual(_obj.Unmutes, entity.Unmutes);
			Assert.AreEqual(_obj.Kicks, entity.Kicks);
			Assert.AreEqual(_obj.Bans, entity.Bans);
		}

		private void TestUpdate()
		{
			_obj.Warns = 1;
			_obj.Pardons = 1;
			_obj.Mutes = 1;
			_obj.Unmutes = 1;
			_obj.Kicks = 1;
			_obj.Bans = 1;

			bool result = _objRepo.Update(_obj);

			Assert.IsTrue(result);

			_ = _objRepo.Read(_obj.Mod, out Audit entity);

			Assert.AreEqual(_obj.Mod, entity.Mod);
			Assert.AreEqual(_obj.Warns, entity.Warns);
			Assert.AreEqual(_obj.Pardons, entity.Pardons);
			Assert.AreEqual(_obj.Mutes, entity.Mutes);
			Assert.AreEqual(_obj.Unmutes, entity.Unmutes);
			Assert.AreEqual(_obj.Kicks, entity.Kicks);
			Assert.AreEqual(_obj.Bans, entity.Bans);
		}

		private void TestDelete()
		{
			bool result = _objRepo.Delete(_obj.Mod);

			Assert.IsTrue(result);

			result = _objRepo.Read(_obj.Mod, out _);

			Assert.IsFalse(result);
		}

		private void Cleanup()
		{
			try
			{
				_objRepo.DbCommand.CommandText = "DELETE FROM Users WHERE UserDbId = @id";
				_objRepo.DbCommand.Parameters.Clear();
				_objRepo.DbCommand.Parameters.AddWithValue("id", _obj.Mod);
				_objRepo.DbConnection.Open();
				_objRepo.DbCommand.ExecuteNonQuery();
				_objRepo.DbConnection.Close();
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
			}
		}
	}
}
