using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Contracts
{
    [DataContract()]
    public class TransferInfo
    {
        [DataMember()]
        public string FilePath { get; set; }
        [DataMember()]
        public DateTime CreationDate { get; set; }
        [DataMember()]
        public long SizeBytes { get; set; }
    }
}
