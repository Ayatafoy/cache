using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internship.Cashe.Models;

namespace Internship.Cashe
{
    public class CachedUserDAO : IUserDAO
    {
        private readonly IUserDAO _userDao;
        private readonly IUserCache _cache;

        public CachedUserDAO(IUserDAO userDao, IUserCache cache)
        {
            if (userDao == null)
                throw new ArgumentNullException("userDao");
            if (cache == null)
                throw new ArgumentNullException("cache");

            _userDao = userDao;
            _cache = cache;
        }

        public User GetById(int id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            User user = _cache.GetUserById(id);
            if (user == null)
            {
                user = _userDao.GetById(id);
                _cache.Set(user);
            }
            return user;
        }

        public IList<User> GetUsersByRole(string role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            IEnumerable<User> users = _cache.GetUsersByRole(role);
            if (users == null)
            {
                users = _userDao.GetUsersByRole(role);
                _cache.Set(role, users);
            }
            return users.ToList();
        }
    }
}
