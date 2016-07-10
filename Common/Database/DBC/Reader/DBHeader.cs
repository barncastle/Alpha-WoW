using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DBC.Reader
{
    public class DBHeader
    {
        public string Signature { get; set; }
        public uint RecordCount { get; set; }
        public uint FieldCount { get; set; }
        public uint RecordSize { get; set; }
        public uint StringBlockSize { get; set; }

        public bool IsValidDbcFile { get { return Signature == "WDBC"; } }
        public bool IsValidDb2File { get { return Signature == "WDB2"; } }
    }
}
