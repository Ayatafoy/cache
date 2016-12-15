using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internship.Cashe;
using Moq;
using NUnit.Framework;

namespace TestsCache
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void GetByIdGotDataFromCache() {
            const int userId = 1;
            Mock<IUserCache> userCacheMock = new Mock<IUserCache>();
            userCacheMock.Setup(x => x.GetUserById(userId))
                .Returns(new Internship.Cashe.Models.User())
                .Verifiable();

            Mock<IUserDAO> userDaoMock = new Mock<IUserDAO>();
            userDaoMock.Setup(x => x.GetById(userId))
                .Verifiable();

            IUserDAO userDao = new CachedUserDAO(userDaoMock.Object, userCacheMock.Object);
            Internship.Cashe.Models.User user = userDao.GetById(userId);
            Assert.That(user, Is.Not.Null);
            userCacheMock.Verify(x => x.GetUserById(userId), Times.Once);
            userDaoMock.Verify(x => x.GetById(userId), Times.Never);
        }

        [Test]
        public void GetByIdGotDataFromDao()
        {
            const int userId = 1;
            Internship.Cashe.Models.User user = new Internship.Cashe.Models.User();
            Mock<IUserCache> userCacheMock = new Mock<IUserCache>();
            userCacheMock.Setup(x => x.GetUserById(userId))
                .Verifiable();
            Mock<IUserDAO> userDaoMock = new Mock<IUserDAO>();
            userDaoMock.Setup(x => x.GetById(userId))
                .Returns(user)
                .Verifiable();
            userCacheMock.Setup(x => x.Set(user))
                .Verifiable();

            IUserDAO userDao = new CachedUserDAO(userDaoMock.Object, userCacheMock.Object);
            Internship.Cashe.Models.User result = userDao.GetById(userId);
            Assert.That(result, Is.Not.Null);
            userCacheMock.Verify(x => x.GetUserById(userId), Times.Once);
            userDaoMock.Verify(x => x.GetById(userId), Times.Once);
            userCacheMock.Verify(x => x.Set(user), Times.Once);
        }

        [Test]
        public void GetByRoleGotDataFromCache()
        {
            const string usersRole = "Mentor";
            Mock<IUserCache> userCacheMock = new Mock<IUserCache>();
            userCacheMock.Setup(x => x.GetUsersByRole(usersRole))
                .Returns(new List<Internship.Cashe.Models.User> {
                    new Internship.Cashe.Models.User()
                })
                .Verifiable();

            Mock<IUserDAO> userDaoMock = new Mock<IUserDAO>();
            userDaoMock.Setup(x => x.GetUsersByRole(usersRole))
                .Verifiable();

            IUserDAO userDao = new CachedUserDAO(userDaoMock.Object, userCacheMock.Object);
            IList<Internship.Cashe.Models.User> users = userDao.GetUsersByRole(usersRole);
            Assert.That(users, Is.Not.Null);
            userCacheMock.Verify(x => x.GetUsersByRole(usersRole), Times.Once);
            userDaoMock.Verify(x => x.GetUsersByRole(usersRole), Times.Never);
        }

        [Test]
        public void GetByRoleGotDataFromDao()
        {
            const string usersRole = "Mentor";
            IList<Internship.Cashe.Models.User> users = new List<Internship.Cashe.Models.User>();
            Mock<IUserDAO> userDaoMock = new Mock<IUserDAO>();
            Mock<IUserCache> userCacheMock = new Mock<IUserCache>();
            userCacheMock.Setup(x => x.GetUsersByRole(usersRole))
                .Verifiable();
            userDaoMock.Setup(x => x.GetUsersByRole(usersRole))
                .Returns(users)
                .Verifiable();
            userCacheMock.Setup(x => x.Set(usersRole, users))
                .Verifiable();
            IUserDAO userDao = new CachedUserDAO(userDaoMock.Object, userCacheMock.Object);
            IList<Internship.Cashe.Models.User> result = userDao.GetUsersByRole(usersRole);
            Assert.That(result, Is.Not.Null);
            userCacheMock.Verify(x => x.GetUsersByRole(usersRole), Times.Once);
            userDaoMock.Verify(x => x.GetUsersByRole(usersRole), Times.Once);
            userCacheMock.Verify(x => x.Set(usersRole, result), Times.Once);
        }
    }
}
