using System;
using System.Diagnostics;
using System.Data.SqlClient;

using LathBotBack.Repos;
using LathBotBack.Config;

using NUnit.Framework;
using LathBotBack;

namespace LathBotTest
{
	public class WarnRepoTest
	{
		LathBotBack.Models.Warn _obj;
		WarnRepository _objRepo;

		public WarnRepoTest()
		{
			_obj = new LathBotBack.Models.Warn
			{
				Reason = "UnitTest",
				Number = 1,
				Level = 1,
				Time = new DateTime(year: 2020, month: 10, day: 5)
			};
		}

		[SetUp]
		public void Setup()
		{
			ReadConfig.Read();
			_objRepo = new WarnRepository(ReadConfig.configJson.ConnectionString);
		}

		[Test]
		public void TestWarnRepository()
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
				_obj.User = (int)reader["UserDbId"];
				_obj.Mod = _obj.User;
				_objRepo.DbConnection.Close();
			}
			catch (Exception e)
			{
				Holder.Instance.Logger.Log(e.Message);
			}
		}

		private void TestCreate()
		{
			bool result = _objRepo.Create(ref _obj);

			Assert.IsTrue(result);
			Assert.NotNull(_obj.ID);
		}

		private void TestRead()
		{
			bool result = _objRepo.Read(_obj.ID, out LathBotBack.Models.Warn entity);

			Assert.IsTrue(result);
			Assert.AreEqual(entity.ID, _obj.ID);
			Assert.AreEqual(entity.User, _obj.User);
			Assert.AreEqual(entity.Mod, _obj.Mod);
			Assert.AreEqual(entity.Reason, _obj.Reason);
			Assert.AreEqual(entity.Number, _obj.Number);
			Assert.AreEqual(entity.Level, _obj.Level);
			Assert.AreEqual(entity.Time, _obj.Time);
			Assert.AreEqual(entity.Persistent, _obj.Persistent);
		}

		private void TestUpdate()
		{
			_obj.Reason = "NewTestNow";
			_obj.Number = 2;
			_obj.Level = 2;

			bool result = _objRepo.Update(_obj);

			Assert.IsTrue(result);

			_ = _objRepo.Read(_obj.ID, out LathBotBack.Models.Warn entity);

			Assert.AreEqual(entity.ID, _obj.ID);
			Assert.AreEqual(entity.User, _obj.User);
			Assert.AreEqual(entity.Mod, _obj.Mod);
			Assert.AreEqual(entity.Reason, _obj.Reason);
			Assert.AreEqual(entity.Number, _obj.Number);
			Assert.AreEqual(entity.Level, _obj.Level);
			Assert.AreEqual(entity.Time, _obj.Time);
			Assert.AreEqual(entity.Persistent, _obj.Persistent);
		}

		private void TestDelete()
		{
			bool result = _objRepo.Delete(_obj.ID);

			Assert.IsTrue(result);

			result = _objRepo.Read(_obj.ID, out _);

			Assert.IsFalse(result);
		}

		private void Cleanup()
		{
			try
			{
				_objRepo.DbCommand.CommandText = "DELETE FROM Users WHERE UserDbId = @id";
				_objRepo.DbCommand.Parameters.Clear();
				_objRepo.DbCommand.Parameters.AddWithValue("id", _obj.User);
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