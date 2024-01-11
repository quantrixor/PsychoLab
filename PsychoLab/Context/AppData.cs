using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PsychoLab.Model;

namespace PsychoLab.Context
{
    internal class AppData
    {
        public static dbPsychoLabAppEntities db = new dbPsychoLabAppEntities();
    }
}
