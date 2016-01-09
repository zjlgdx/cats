using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract()]
    public interface IFileTransferService
    {
        [OperationContract()]
        string BeginUploadSession(TransferInfo uploadInfo);

        [OperationContract()]
        void UploadChunk(string sessionId, byte[] chunk);

        [OperationContract()]
        void CompleteUpload(string sessionId);

        [OperationContract()]
        DownloadSessionInfo StartDownload(TransferInfo attachmentInfo);

        [OperationContract()]
        byte[] DownloadChunk(string sessionId, long chunkIndex);

        [OperationContract()]
        void DownloadComplete(string sessionId);

    }
}
