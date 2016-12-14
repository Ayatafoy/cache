using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internship.Cashe.Models;

namespace Internship.Cashe
{
    public interface IUserCache
    {
        IEnumerable<User> GetUsersByRole(string role);
        User GetUserById(int id);

        void Set(string role, IEnumerable<User> users);
        void Set(User user);
    }
}
