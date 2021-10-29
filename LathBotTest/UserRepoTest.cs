using NUnit.Framework;

using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;

namespace LathBotTest
{
	class UserRepoTest
	{
		User _obj;
		UserRepository _objRepo;

		public UserRepoTest()
		{
			_obj = new User
			{
				DcID = 111111111111111111
			};
		}

		[SetUp]
		public void Setup()
		{
			ReadConfig.Read();
			_objRepo = new UserRepository(ReadConfig.Config.ConnectionString);
		}

		[Test]
		public void TestUserRepository()
		{
			TestCreate();

			TestRead();

			TestUpdate();

			TestDelete();
		}

		[Test]
		public void TestExists()
		{
			bool result = _objRepo.ExistsDcId(387325006176059394, out bool exists);
			Assert.IsTrue(result);
			Assert.IsTrue(exists);
		}

		private void TestCreate()
		{
			bool result = _objRepo.Create(ref _obj);

			Assert.IsTrue(result);
			Assert.NotNull(_obj.ID);
		}

		private void TestRead()
		{
			bool result = _objRepo.Read(_obj.ID, out User entity);

			Assert.IsTrue(result);
			Assert.AreEqual(_obj.ID, entity.ID);
			Assert.AreEqual(_obj.DcID, entity.DcID);
		}

		private void TestUpdate()
		{
			_obj.DcID = 222222222222222222;
			bool result = _objRepo.Update(_obj);

			Assert.IsTrue(result);

			_ = _objRepo.Read(_obj.ID, out User entity);

			Assert.AreEqual(_obj.ID, entity.ID);
			Assert.AreEqual(_obj.DcID, entity.DcID);
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
