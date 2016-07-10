using Common.Database;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using WorldServer.Game;
using Newtonsoft.Json;

namespace WorldServer.Storage
{
    public class GroupedDbSet<TKey, T> : BaseContext, IQueryable<KeyValuePair<TKey, T>> where TKey : new() where T : new()
    {
        private readonly string TableName;
        private readonly PropertyInfo Key;
        private readonly Type Type;
        private ConcurrentDictionary<TKey, List<object>> Data { get; set; }
        private Dictionary<TKey, ulong> Checksums = new Dictionary<TKey, ulong>();
        private readonly bool m_trackchanges;

        public List<TKey> Keys { get { return Data.Keys.ToList(); } }
        public ICollection<List<object>> Values { get { return Data.Values; } }

        public GroupedDbSet(string query, string tablename, bool trackchanges = false) : base(Globals.CONNECTION_STRING, query)
        {
            this.TableName = tablename;
            this.Type = GetType();
            this.Key = FindKey();
            this.Data = this.PopulateGroupedDBSet<TKey>(this.Key, Type, TableName);
            this.m_trackchanges = trackchanges;
            if (this.m_trackchanges)
                this.SetAllChecksums();
        }

        public void Reload()
        {
            this.Data = this.PopulateGroupedDBSet<TKey>(this.Key, Type, TableName);
        }

        public Expression Expression
        {
            get
            {
                return this.Data.AsQueryable().Expression;
            }
        }

        public Type ElementType
        {
            get
            {
                return this.Data.AsQueryable().ElementType;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.Data.AsQueryable().Provider;
            }
        }

        Expression IQueryable.Expression
        {
            get
            {
                return this.Data.AsQueryable().Expression;
            }
        }

        public T TryGet(TKey key)
        {
            T result = new T();
            if (this.Data.ContainsKey(key))
                foreach (var obj in this.Data[key])
                    ((IList)result).Add(obj);

            return result;
        }

        public bool TryAdd(T value)
        {
            if (this.Data.ContainsKey((TKey)this.Key.GetValue(value)))
            {
                this.Data[(TKey)this.Key.GetValue(value)].Add(value);
                return true;
            }                
            else
               return  this.Data.TryAdd((TKey)this.Key.GetValue(value), new List<object> { value });

        }

        public bool TryRemove(TKey key)
        {
            List<object> dump;
            if (!this.Data.ContainsKey(key))
                return false;

            return this.Data.TryRemove(key, out dump);
        }

        public bool TryRemove(T value)
        {
            return this.TryRemove((TKey)this.Key.GetValue(value));
        }

        public bool ContainsKey(TKey key)
        {
            return this.Data.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }



        private void SetAllChecksums()
        {
            foreach(var data in this.Data)
            {
                if (this.Checksums.ContainsKey(data.Key))
                    this.Checksums[data.Key] = GetHash(data.Value);
                else
                    this.Checksums.Add(data.Key, GetHash(data.Value));
            }
        }

        private PropertyInfo FindKey()
        {
            foreach (var property in this.Type.GetProperties())
                if (property.GetCustomAttribute<KeyAttribute>() != null)
                    return property;

            throw new Exception("No KeyAttribute found on class " + typeof(T).FullName);
        }

        private new Type GetType()
        {
            Type type = typeof(T);

            if (typeof(T).Name.StartsWith("List"))
                type = typeof(T).GetProperty("Item").PropertyType;

            return type;
        }

        private ulong GetHash(object obj)
        {
            string text = JsonConvert.SerializeObject(obj);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            ulong hash = 14695981039346656037;
            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash ^ bytes[i];
                hash *= 0x100000001b3;
            }
            return hash;
        }
    }
}
