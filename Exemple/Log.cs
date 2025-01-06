using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple
{
    public enum EnumCategory
    {
        Info = 0,
        Error = 1,
        Process = 2,
        Communication = 3,
    }

    public class Log
    {

        public static IEnumerable<string> GetCategory() => Enum.GetNames(typeof(EnumCategory));

        public string Date { get; set; }

        public EnumCategory Category { get; set; }

        public string Type { get; set; }
        public string Description { get; set; }

        public Log(string date, EnumCategory category, string type, string description)
        {
            Date = date;
            Type = type;
            Category = category;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Date};{Category};{Description}";
        }

    }
}

