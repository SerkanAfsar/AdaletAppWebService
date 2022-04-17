using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Utilites
{
    public static class Helper
    {
        public static string MsSqlConnectionString { get; set; } = @"Data Source=.\SQLEXPRESS;Initial Catalog=db_AdaletHaberleri;Integrated Security=True";
    }
}
