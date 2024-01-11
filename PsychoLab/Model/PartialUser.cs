using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoLab.Model
{
    public partial class User
    {
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName} {MiddleName}";
            }
            set
            {
                FullName = value;
            }
        }
        public virtual ICollection<Role> Role { get; set; }

        // Свойство, которое определяет, является ли пользователь администратором
        public bool IsAdmin
        {
            get
            {
                return this.Roles.Any(r => r.RoleName == "Administrator");
            }
        }

        public string RolesDisplay => string.Join(", ", this.Roles.Select(r => r.RoleName));

    }
}
