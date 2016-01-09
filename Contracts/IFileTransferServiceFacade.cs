using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Contracts
{
    public interface IFileTransferServiceFacade
    {
        void Upload(TransferInfo transferInfo, Stream stream);
        void Download(TransferInfo attachInfo, Stream stream);
    }
}
