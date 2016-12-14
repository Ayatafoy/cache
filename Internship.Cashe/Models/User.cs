using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship.Cashe.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string FullName { get; set; }

        public virtual IList<Role> Roles { get; set; }
    }
}
