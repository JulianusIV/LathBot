using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;

using NUnit.Framework;

namespace LathBotTest
{
	class VariableRepoTest
	{
		Variable _obj;
		VariableRepository _objRepo;

		public VariableRepoTest()
		{
			_obj = new Variable
			{
				Name = "UnitTest",
				Value = "test"
			};
		}

		[SetUp]
		public void Setup()
		{
			ReadConfig.Read();
			_objRepo = new VariableRepository(ReadConfig.configJson.ConnectionString);
		}

		[Test]
		public void TestVariableRepository()
		{
			TestCreate();

			TestRead();

			TestUpdate();

			TestDelete();
		}

		private void TestCreate()
		{
			bool result = _objRepo.Create(ref _obj);

			Assert.IsTrue(result);
			Assert.NotNull(_obj.ID);
		}

		private void TestRead()
		{
			bool result = _objRepo.Read(_obj.ID, out Variable entity);

			Assert.IsTrue(result);
			Assert.AreEqual(_obj.ID, entity.ID);
			Assert.AreEqual(_obj.Name, entity.Name);
			Assert.AreEqual(_obj.Value, entity.Value);
		}

		private void TestUpdate()
		{
			_obj.Name = "test";
			_obj.Value = "UT";

			bool result = _objRepo.Update(_obj);

			Assert.IsTrue(result);

			_ = _objRepo.Read(_obj.ID, out Variable entity);

			Assert.AreEqual(_obj.ID, entity.ID);
			Assert.AreEqual(_obj.Name, entity.Name);
			Assert.AreEqual(_obj.Value, entity.Value);
		}

		private void TestDelete()
		{
			bool result = _objRepo.Delete(_obj.ID);

			Assert.IsTrue(result);

			result = _objRepo.Read(_obj.ID, out _);

			Assert.IsFalse(result);
		}
	}
}
