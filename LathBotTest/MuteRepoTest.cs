using LathBotBack;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace LathBotTest
{
	class MuteRepoTest
	{
		Mute _obj;
		MuteRepository _objRepo;

		public MuteRepoTest()
		{
			_obj = new Mute
			{
				Timestamp = DateTime.Parse("2020-01-01T01:01:01.0000000Z"),
				Duration = 1
			};
		}

		[SetUp]
		public void Setup()
		{
			ReadConfig.Read();
			_objRepo = new MuteRepository(ReadConfig.configJson.ConnectionString);
		}

		[Test]
		public void TestMuteRepository()
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
			bool result = _objRepo.Create(ref _obj);

			Assert.IsTrue(result);
		}

		private void TestRead()
		{
			bool result = _objRepo.Read(_obj.Id, out Mute entity);

			Assert.IsTrue(result);
			Assert.AreEqual(_obj.Id, entity.Id);
			Assert.AreEqual(_obj.User, entity.User);
			Assert.AreEqual(_obj.Mod, entity.Mod);
			Assert.AreEqual(_obj.Timestamp, entity.Timestamp);
			Assert.AreEqual(_obj.Duration, entity.Duration);
		}

		private void TestUpdate()
		{
			_obj.Timestamp = DateTime.Parse("2022-12-22T22:22:22");
			_obj.Duration = 14;

			bool result = _objRepo.Update(_obj);

			Assert.IsTrue(result);

			_ = _objRepo.Read(_obj.Id, out Mute entity);

			Assert.AreEqual(_obj.Id, entity.Id);
			Assert.AreEqual(_obj.User, entity.User);
			Assert.AreEqual(_obj.Mod, entity.Mod);
			Assert.AreEqual(_obj.Timestamp, entity.Timestamp);
			Assert.AreEqual(_obj.Duration, entity.Duration);
		}

		private void TestDelete()
		{
			bool result = _objRepo.Delete(_obj.Id);

			Assert.IsTrue(result);

			result = _objRepo.Read(_obj.Id, out _);

			Assert.IsFalse(result);
		}

		private void Cleanup()
		{
			try
			{
				_objRepo.DbCommand.CommandText = "DELETE FROM Users WHERE UserDbId = @id";
				_objRepo.DbCommand.Parameters.Clear();
				_objRepo.DbCommand.Parameters.AddWithValue("id", _obj.Id);
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
