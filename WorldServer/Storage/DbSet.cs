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
    public class DbSet<TKey, T> : BaseContext, IQueryable<KeyValuePair<TKey, T>> where TKey : new() where T : new()
    {
        private readonly PropertyInfo Key;
        private ConcurrentDictionary<TKey, T> Data { get; set; }
        private Dictionary<TKey, ulong> Checksums = new Dictionary<TKey, ulong>();
        private readonly bool m_trackchanges;
        private readonly bool m_constructorload;

        public List<TKey> Keys { get { return Data.Keys.ToList(); } }
        public List<T> Values { get { return Data.Values.ToList(); } }

        public DbSet(bool constructorload = false, bool trackchanges = false) : base(Globals.CONNECTION_STRING)
        {
            this.Key = FindKey();
            this.m_trackchanges = trackchanges;
            this.m_constructorload = constructorload;
            this.Data = this.PopulateDBSet<TKey, T>(this.Key, constructorload);

            if (this.m_trackchanges)
                this.SetAllChecksums();
        }

        public void Reload()
        {
            this.Data = this.PopulateDBSet<TKey, T>(this.Key, m_constructorload);
        }

        #region IQueryable Methdods
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

        public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }
        #endregion


        public T TryGet(TKey key)
        {
            if (this.Data.ContainsKey(key))
                return this.Data[key];

            return default(T);
        }

        public TType TryGet<TType>(TKey key)
        {
            if (this.Data.ContainsKey(key))
                return (TType)(object)this.Data[key];

            return default(TType);
        }

        public bool TryAdd(T value)
        {
            TKey key = (TKey)this.Key.GetValue(value);
            if (this.Data.TryAdd(key, value))
            {
                if (this.Checksums.ContainsKey(key))
                    this.Checksums[key] = GetHash(value);
                else
                    this.Checksums.Add((TKey)this.Key.GetValue(value), GetHash(value));

                return true;
            }

            return false;
        }

        public bool TryRemove(TKey key)
        {
            T dump;
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


        public void UpdateChanges()
        {
            if (!m_trackchanges)
                return;

            List<TKey> todelete = Checksums.Where(x => !Data.ContainsKey(x.Key)).Select(x => x.Key).ToList();
            List<T> toadd = Data.Where(x => !Checksums.ContainsKey(x.Key)).Select(x => x.Value).ToList();
            Dictionary<TKey, T> toupdate = Data.Where(x => Checksums.ContainsKey(x.Key) && Checksums[x.Key] != GetHash(x.Value)).ToDictionary(x => x.Key, y => y.Value);
            
            if(todelete.Count > 0)
            {
                foreach (TKey val in todelete)
                    Checksums.Remove(val);

                DeleteEntity<TKey, T>(Key, todelete);
            }

            foreach (T val in toadd)
            {
                SaveEntity(Key, val);
                Checksums.Add((TKey)this.Key.GetValue(val), GetHash(val));
            }
                
            foreach (var val in toupdate)
            {
                SaveEntity(Key, val.Value);
                Checksums[val.Key] = GetHash(val.Value);
            }
        }

        public bool Save(T toupdate)
        {
            SaveEntity(Key, toupdate);
            return true;
        }


        private void SetAllChecksums()
        {
            foreach (var data in this.Data)
            {
                if (this.Checksums.ContainsKey(data.Key))
                    this.Checksums[data.Key] = GetHash(data.Value);
                else
                    this.Checksums.Add(data.Key, GetHash(data.Value));
            }
        }

        private PropertyInfo FindKey()
        {
            foreach (var property in typeof(T).GetProperties())
                if (property.GetCustomAttribute<KeyAttribute>() != null)
                    return property;

            throw new Exception("No KeyAttribute found on class " + typeof(T).FullName);
        }

        private ulong GetHash(object obj)
        {
            string text = JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { MaxDepth = 1 });
            byte[] bytes = Encoding.UTF8.GetBytes(text);
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
