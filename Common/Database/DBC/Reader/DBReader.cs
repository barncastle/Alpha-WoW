using Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Database.DBC.Reader
{
    public class DBReader
    {
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                string path = Uri.UnescapeDataString(new UriBuilder(codeBase).Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static DBHeader ExtractHeader(BinaryReader dbReader)
        {
            return new DBHeader
            {
                Signature = dbReader.ReadString(4),
                RecordCount = dbReader.Read<uint>(),
                FieldCount = dbReader.Read<uint>(),
                RecordSize = dbReader.Read<uint>(),
                StringBlockSize = dbReader.Read<uint>()
            };
        }

        public static HashSet<T> Read<T>(string dbcFile) where T : new()
        {
            HashSet<T> tempList = new HashSet<T>();

            try
            {
                using (var dbReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(AssemblyDirectory + "/dbc/" + dbcFile))))
                {
                    DBHeader header = ExtractHeader(dbReader);

                    if (header.IsValidDbcFile || header.IsValidDb2File)
                    {
                        tempList = new HashSet<T>();
                        var fields = typeof(T).GetFields();
                        var lastString = "";

                        for (int i = 0; i < header.RecordCount; i++)
                        {
                            T newObj = new T();

                            foreach (var f in fields)
                                ExtractFields<T>(f, ref lastString, dbReader, ref newObj, ref header);

                            tempList.Add(newObj);
                        }
                    }
                }

                ++DBC.Count;
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "Error while loading {0}: {1}", dbcFile, ex.Message);
            }

            return tempList;
        }

        public static ConcurrentDictionary<TKey,T> Read<TKey,T>(string dbcFile, string key) where T : new()
        {
            ConcurrentDictionary<TKey, T> tempList = new ConcurrentDictionary<TKey, T>();

            try
            {
                using (var dbReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(AssemblyDirectory + "/dbc/" + dbcFile))))
                {
                    DBHeader header = ExtractHeader(dbReader);

                    if (header.IsValidDbcFile || header.IsValidDb2File)
                    {
                        tempList = new ConcurrentDictionary<TKey, T>();
                        var fields = typeof(T).GetFields();
                        var lastString = "";

                        for (int i = 0; i < header.RecordCount; i++)
                        {
                            T newObj = new T();

                            foreach (var f in fields)
                                ExtractFields<T>(f, ref lastString, dbReader, ref newObj, ref header);

                            TKey keyItem = (TKey)(newObj.GetType().GetField(key).GetValue(newObj));
                            tempList.TryAdd(keyItem, newObj);
                        }
                    }
                }

                ++DBC.Count;
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "Error while loading {0}: {1}", dbcFile, ex.Message);
            }

            return tempList;
        }

        private static void ExtractFields<T>(FieldInfo f, ref string lastString, BinaryReader dbReader, ref T newObj, ref DBHeader header)
        {
            switch (f.FieldType.Name)
            {
                case "SByte":
                    f.SetValue(newObj, dbReader.ReadSByte());
                    break;
                case "Byte":
                    f.SetValue(newObj, dbReader.ReadByte());
                    break;
                case "Int32":
                    f.SetValue(newObj, dbReader.ReadInt32());
                    break;
                case "UInt32":
                    f.SetValue(newObj, dbReader.ReadUInt32());
                    break;
                case "Int64":
                    f.SetValue(newObj, dbReader.ReadInt64());
                    break;
                case "UInt64":
                    f.SetValue(newObj, dbReader.ReadUInt64());
                    break;
                case "Single":
                    f.SetValue(newObj, dbReader.ReadSingle());
                    break;
                case "Boolean":
                    f.SetValue(newObj, dbReader.ReadBoolean());
                    break;
                case "SByte[]":
                    f.SetValue(newObj, dbReader.ReadSByte(((sbyte[])f.GetValue(newObj)).Length));
                    break;
                case "Byte[]":
                    f.SetValue(newObj, dbReader.ReadByte(((byte[])f.GetValue(newObj)).Length));
                    break;
                case "Int32[]":
                    f.SetValue(newObj, dbReader.ReadInt32(((int[])f.GetValue(newObj)).Length));
                    break;
                case "UInt32[]":
                    f.SetValue(newObj, dbReader.ReadUInt32(((uint[])f.GetValue(newObj)).Length));
                    break;
                case "Single[]":
                    f.SetValue(newObj, dbReader.ReadSingle(((float[])f.GetValue(newObj)).Length));
                    break;
                case "Int64[]":
                    f.SetValue(newObj, dbReader.ReadInt64(((long[])f.GetValue(newObj)).Length));
                    break;
                case "UInt64[]":
                    f.SetValue(newObj, dbReader.ReadUInt64(((ulong[])f.GetValue(newObj)).Length));
                    break;
                case "String":
                    {
                        var stringOffset = dbReader.ReadUInt32();
                        var currentPos = dbReader.BaseStream.Position;
                        var stringStart = (header.RecordCount * header.RecordSize) + 20 + stringOffset;
                        dbReader.BaseStream.Seek(stringStart, 0);
                        f.SetValue(newObj, lastString = dbReader.ReadCString());
                        dbReader.BaseStream.Seek(currentPos, 0);
                        break;
                    }
                default:
                    dbReader.BaseStream.Position += 4;
                    break;
            }
        }
    }
}
