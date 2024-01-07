﻿using System;
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
    }
}
