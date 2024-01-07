using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoLab.Model
{
    public partial class Client
    {
        public string GetData
        {
            get
            {
                return $"{FirstName} {LastName} - {Phone} ";
            }
        }
    }
}
