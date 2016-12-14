using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internship.Cashe.Models;

namespace Internship.Cashe
{
    public interface IUserDAO
    {
        User GetById(int id);
        IList<User> GetUsersByRole(string role);
    }
}
