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
	class ModRepoTest
	{
		Mod _obj;
		ModRepository _objRepo;

		public ModRepoTest()
		{
			_obj = new Mod
			{
				Timezone = "Europe/Berlin"
			};
		}

		[SetUp]
		public void Setup()
		{
			ReadConfig.Read();
			_objRepo = new ModRepository(ReadConfig.configJson.ConnectionString);
		}

		[Test]
		public void TestModRepository()
		{
			CreateUser();

			TestCreate();

			TestRead();

			TestUpdate();

			TestDelete();
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
				_obj.DbId = (int)reader["UserDbId"];
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
			Assert.NotNull(_obj.Id);
		}

		private void TestRead()
		{
			bool result = _objRepo.Read(_obj.Id, out Mod entity);

			Assert.IsTrue(result);
			Assert.AreEqual(_obj.Id, entity.Id);
			Assert.AreEqual(_obj.DbId, entity.DbId);
			Assert.AreEqual(_obj.Timezone, entity.Timezone);
		}

		private void TestUpdate()
		{
			_obj.Timezone = "Europe/Amsterdam";
			bool result = _objRepo.Update(_obj);

			Assert.IsTrue(result);

			_ = _objRepo.Read(_obj.Id, out Mod entity);

			Assert.AreEqual(_obj.Id, entity.Id);
			Assert.AreEqual(_obj.DbId, entity.DbId);
			Assert.AreEqual(_obj.Timezone, entity.Timezone);
		}

		private void TestDelete()
		{
			bool result = _objRepo.Delete(_obj.Id);

			Assert.IsTrue(result);

			result = _objRepo.Read(_obj.Id, out _);

			Assert.IsFalse(result);
		}
	}
}
