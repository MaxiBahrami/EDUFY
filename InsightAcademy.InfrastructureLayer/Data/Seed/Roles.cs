using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Data.Seed
{
    public static class Roles
    {
        public static string SuperAdmin { get; set; } = "SuperAdmin";
        public  static string Teacher { get; set; } = "Teacher";
        public  static string Student { get; set; } = "Student";
        public  static string User { get; set; } = "User";
    }
}
