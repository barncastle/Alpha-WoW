using Common.Helpers.Extensions;
using Common.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Database
{
    public class BaseContext
    {
        public const string SAVE_QUERY = "INSERT INTO {0} ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3}";
        private readonly string connectionstring;
        private readonly string BASE_QUERY = "SELECT * FROM ";
        private const string DELETE_QUERY = "DELETE FROM {0} WHERE {1} IN ({2})";
        private readonly Type[] arrayTypes = new[] { typeof(ItemAttribute), typeof(SpellStat), typeof(DamageStat) };
        private readonly Type[] customTypes = new[] { typeof(TStat), typeof(TRandom), typeof(TResistance) };

        public BaseContext(string connection, string query = "")
        {
            this.connectionstring = connection;

            if (!string.IsNullOrWhiteSpace(query))
                this.BASE_QUERY = query;
        }

        public ConcurrentDictionary<TKey, T> PopulateDBSet<TKey, T>(PropertyInfo key, bool constructorload = false) where T : new()
        {
            ConcurrentDictionary<TKey, T> dataset = new ConcurrentDictionary<TKey, T>();
            string tableName = GetTableName<T>();

            Dictionary<PropertyInfo, ColumnInfo> properties = BuildPropertyList<T>();

            using (var conn = new MySqlConnection(this.connectionstring))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(BASE_QUERY + tableName, conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        T newobj;
                        TKey newkey = default(TKey);

                        if (constructorload)
                        {
                            newobj = (T)CreateType(typeof(T), new object[] { dr });
                            newkey = (TKey)typeof(T).GetProperty(key.Name, BindingFlags.Public | BindingFlags.Instance).GetValue(newobj);
                        }
                        else
                        {
                            newobj = new T();

                            foreach (var property in properties)
                            {
                                TypeCode typeCode = Type.GetTypeCode(property.Key.PropertyType);
                                if (!property.Value.IsArray)
                                {
                                    string col = property.Value.Columns[0];
                                    var prop = Convert.ChangeType(dr[col], typeCode);
                                    if (property.Key == key)
                                        newkey = (TKey)prop;

                                    if (customTypes.Contains(property.Key.PropertyType))
                                    {
                                        var newprop = CreateType(property.Key.PropertyType, new object[] { Convert.ToUInt32(dr[col]) });
                                        property.Key.SetValue(newobj, newprop);
                                    }
                                    else
                                        property.Key.SetValue(newobj, prop);
                                }
                                else
                                {
                                    string[] cols = property.Value.Columns.ToArray();

                                    if (customTypes.Contains(property.Key.PropertyType))
                                    {
                                        var newprop = CreateType(property.Key.PropertyType, new object[] {
                                        Convert.ToUInt32(dr[cols[0]]),
                                        Convert.ToUInt32(dr[cols[1]])
                                    });

                                        property.Key.SetValue(newobj, newprop);
                                    }
                                    else
                                    {
                                        Type elementtype = property.Key.PropertyType.GetElementType();
                                        var array = Array.CreateInstance(elementtype, cols.Length);

                                        if (arrayTypes.Contains(elementtype))
                                        {
                                            for (int i = 0; i < cols.Length; i++)
                                                array.SetValue(CreateType(elementtype, new object[] { dr, i }), i);
                                        }
                                        else
                                        {
                                            for (int i = 0; i < cols.Length; i++)
                                                array.SetValue(Convert.ChangeType(dr[cols[i]], elementtype), i);
                                        }

                                        property.Key.SetValue(newobj, array);
                                    }
                                }
                            }
                        }

                        if (typeof(T).GetMethod("OnDbLoad") != null)
                            typeof(T).GetMethod("OnDbLoad").Invoke(newobj, null);

                        if (!dataset.ContainsKey(newkey))
                            dataset.TryAdd(newkey, newobj);
                    }
                }

                conn.Close();
            }

            Log.Message(LogType.NORMAL, typeof(T).Name + " Loaded - " + dataset.Count);

            return dataset;
        }

        public ConcurrentDictionary<TKey, List<object>> PopulateGroupedDBSet<TKey>(PropertyInfo key, Type type, string tableName) where TKey : new()
        {
            ConcurrentDictionary<TKey, List<object>> dataset = new ConcurrentDictionary<TKey, List<object>>();
            Dictionary<PropertyInfo, ColumnInfo> properties = BuildPropertyList(type);

            using (var conn = new MySqlConnection(this.connectionstring))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(BASE_QUERY, conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        object newobj = type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                        TKey newkey = default(TKey);

                        foreach (var property in properties)
                        {
                            TypeCode typeCode = Type.GetTypeCode(property.Key.PropertyType);
                            if (!property.Value.IsArray)
                            {
                                var prop = Convert.ChangeType(dr[property.Value.Columns[0]], typeCode);
                                if (property.Key == key)
                                    newkey = (TKey)prop;

                                property.Key.SetValue(newobj, prop);
                            }
                            else
                            {
                                var array = Array.CreateInstance(property.Key.PropertyType.GetElementType(), property.Value.Columns.Count);
                                for (int i = 0; i < property.Value.Columns.Count; i++)
                                    array.SetValue(dr[property.Value.Columns[i]], i);

                                property.Key.SetValue(newobj, array);
                            }
                        }

                        if (!dataset.ContainsKey(newkey))
                        {
                            List<object> newList = new List<object>() { newobj };
                            dataset.TryAdd(newkey, newList);
                        }
                        else
                            dataset[newkey].Add(newobj);
                    }
                }

                conn.Close();
            }

            Log.Message(LogType.NORMAL, tableName + " Loaded - " + dataset.Count);
            return dataset;
        }

        public bool ExecuteCommand(string query, List<MySqlParameter> parameters)
        {
            using (var conn = new MySqlConnection(this.connectionstring))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                if(parameters != null && parameters.Count > 0)
                    cmd.Parameters.AddRange(parameters.ToArray());
                try
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    conn.Close();
                    Log.Message(LogType.ERROR, e.Message);
                    return false;
                }
            }
        }

        public static bool ExecuteCommand(string connection, string query, List<MySqlParameter> parameters)
        {
            using (var conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                if (parameters != null && parameters.Count > 0)
                    cmd.Parameters.AddRange(parameters.ToArray());
                try
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    conn.Close();
                    Log.Message(LogType.ERROR, e.Message);
                    return false;
                }
            }
        }

        public bool SaveEntity<T>(PropertyInfo key, T obj)
        {
            if (typeof(T).GetMethod("Save") != null)
            {
                typeof(T).GetMethod("Save").Invoke(obj, null);
                return true;
            }

            Dictionary<PropertyInfo, ColumnInfo> properties = BuildPropertyList<T>();
            string tablename = GetTableName<T>();
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            List<string> columns = properties.Values.SelectMany(x => x.Columns).ToList();
            List<string> values = columns.Select(x => "@" + x).ToList();
            List<string> dupevals = columns.Select(x => x + " = values(" + x + ")").ToList();

            string query = string.Format(SAVE_QUERY, 
                                         tablename, 
                                         string.Join(",", columns),
                                         string.Join(",", values),
                                         string.Join(",", dupevals));

            foreach (var prop in properties)
                parameters.Add(new MySqlParameter(prop.Key.Name, prop.Key.GetValue(obj)));

            return ExecuteCommand(query, parameters);
        }

        public static bool SaveEntity(string tablename, List<string> columns, List<MySqlParameter> parameters, string connection)
        {
            List<string> values = columns.Select(x => "@" + x).ToList();
            List<string> dupevals = columns.Select(x => x + " = values(" + x + ")").ToList();
            string query = string.Format(SAVE_QUERY,
                                         tablename,
                                         string.Join(",", columns),
                                         string.Join(",", values),
                                         string.Join(",", dupevals));

            return ExecuteCommand(connection, query, parameters);
        }

        public bool DeleteEntity<TKey, T>(PropertyInfo key, List<TKey> objs)
        {
            string ids = string.Join(",", objs);
            string tablename = GetTableName<T>();
            string query = string.Format(DELETE_QUERY, tablename, key.Name, ids);

            return ExecuteCommand(query, null);
        }

        
        private string GetTableName<T>()
        {
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
            if (!string.IsNullOrWhiteSpace(tableName))
                return tableName;

            throw new System.Exception("No TableAttribute found on class " + typeof(T).FullName);
        }

        private Dictionary<PropertyInfo, ColumnInfo> BuildPropertyList(Type T)
        {
            Dictionary<PropertyInfo, ColumnInfo> properties = new Dictionary<PropertyInfo, ColumnInfo>();

            foreach (var property in T.GetProperties())
            {
                var column = property.GetCustomAttribute<ColumnAttribute>();
                var columnlist = property.GetCustomAttribute<ColumnListAttribute>();

                if (column != null && !string.IsNullOrWhiteSpace(column.Name))
                    properties.Add(property, new ColumnInfo(column.Name));
                else if (columnlist != null && columnlist.Names.Length > 0)
                    properties.Add(property, new ColumnInfo(columnlist.Names));
            }

            return properties;
        }

        private Dictionary<PropertyInfo, ColumnInfo> BuildPropertyList<T>()
        {
            return BuildPropertyList(typeof(T));
        }

        private object CreateType(Type type, params object[] args)
        {
            return Convert.ChangeType(Activator.CreateInstance(type, args), type);
        }

        private class ColumnInfo
        {
            public List<string> Columns { get; private set; } = new List<string>();
            public bool IsArray { get; private set; } = false;

            public ColumnInfo(string column)
            {
                this.Columns.Add(column);
            }

            public ColumnInfo(string[] columns)
            {
                this.Columns.AddRange(columns);
                this.IsArray = true;
            }
        }
    }
}
