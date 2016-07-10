using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnListAttribute : Attribute
    {
        public ColumnListAttribute(string[] names)
        {
            this.Names = names;
        }

        public ColumnListAttribute(string basename, int count)
        {
            this.Names = new string[count];
            for (int i = 1; i <= count; i++)
                this.Names[i - 1] = basename + i;
        }

        public string[] Names { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class KeyAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            this.Name = name;
        }

        public TableAttribute(string name, string query)
        {
            this.Name = name;
            this.Query = query;
        }

        public string Name { get; set; }

        public string Query { get; set; }
    }
}
