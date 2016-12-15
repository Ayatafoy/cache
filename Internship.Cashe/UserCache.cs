using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Internship.Cashe.Models;

namespace Internship.Cashe
{
    public class UserCache : IUserCache
    {
        private readonly IDictionary<int, Tuple<User, DateTime>> _userLookup = new ConcurrentDictionary<int, Tuple<User, DateTime>>();
        private readonly IDictionary<string, Tuple<ISet<User>, DateTime>> _userByRoleLookup = new ConcurrentDictionary<string, Tuple<ISet<User>, DateTime>>();
        private readonly TimeSpan _timeout;

        public UserCache()
        {
            TimeSpan timeout = ApplicationConfiguration.GetSettingAsTimeSpan("timeout");
        }

        public User GetUserById(int id)
        {
            Tuple<User, DateTime> value;
            if (_userLookup.TryGetValue(id, out value) && value.Item2 >= DateTime.UtcNow)
            {
                return value.Item1;
            }
            return null;
        }

        public IEnumerable<User> GetUsersByRole(string role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            Tuple<ISet<User>, DateTime> value;
            if (_userByRoleLookup.TryGetValue(role, out value) && value.Item2 >= DateTime.UtcNow)
            {
                return value.Item1;
            }
            return null;
        }

        public void Set(string role, IEnumerable<User> users)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            if (users == null)
                throw new ArgumentNullException("users");
            DateTime ttl = DateTime.UtcNow.Add(_timeout);
            _userByRoleLookup[role] = new Tuple<ISet<User>, DateTime>(
                new HashSet<User>(users, UserEqualityComparer.Instance),
                ttl);
            foreach (User u in users)
            {
                _userLookup[u.Id] = new Tuple<User, DateTime>(u, ttl);
            }
        }

        public void Set(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            DateTime ttl = DateTime.UtcNow.Add(_timeout);
            Tuple<ISet<User>, DateTime> value;
            _userLookup[user.Id] = new Tuple<User, DateTime>(user, ttl);
            if (_userByRoleLookup.TryGetValue(user.Roles.FirstOrDefault().RoleName, out value))
            {
                value.Item1.Remove(user);
                value.Item1.Add(user);
            }
        }

        private static class ApplicationConfiguration
        {
            internal static TimeSpan GetSettingAsTimeSpan(string settingName)
            {
                return GetSettingAsType(settingName, value => TimeSpan.Parse(value));
            }

            private static T GetSettingAsType<T>(string settingName, Func<string, T> converter)
            {
                string value = ConfigurationManager.AppSettings[settingName];
                if (string.IsNullOrEmpty(value))
                    return default(T);

                return converter(value);
            }
        }

        private sealed class UserEqualityComparer : IEqualityComparer<User>
        {
            internal static UserEqualityComparer Instance = new UserEqualityComparer();

            public bool Equals(User x, User y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;

                return x.Id == y.Id;
            }

            public int GetHashCode(User obj)
            {
                return obj == null ? 0 : obj.Id;
            }
        }
    }
}

