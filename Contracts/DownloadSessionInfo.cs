using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Contracts
{
    [DataContract()]
    public class DownloadSessionInfo
    {
        [DataMember()]
        public long ChunkCount { get; set; }
        [DataMember()]
        public string SessionId { get; set; }
    }
}
