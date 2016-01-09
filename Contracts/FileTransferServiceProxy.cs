using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contracts
{
    public class FileTransferServiceProxy : ClientBase<IFileTransferService>, IFileTransferService
    {
        public FileTransferServiceProxy() { }
        public FileTransferServiceProxy(string endpointName) : base(endpointName) { }
        public FileTransferServiceProxy(Binding binding, EndpointAddress endpointAddress) : base(binding,endpointAddress) { }

        public string BeginUploadSession(TransferInfo uploadInfo)
        {
            return Channel.BeginUploadSession(uploadInfo);
        }

        public void UploadChunk(string sessionId, byte[] chunk)
        {
            Channel.UploadChunk(sessionId, chunk);
        }

        public void CompleteUpload(string sessionId)
        {
            Channel.CompleteUpload(sessionId);
        }

        public DownloadSessionInfo StartDownload(TransferInfo transferInfo)
        {
            return Channel.StartDownload(transferInfo);
        }

        public byte[] DownloadChunk(string sessionId, long chunkIndex)
        {
            return Channel.DownloadChunk(sessionId, chunkIndex);
        }

        public void DownloadComplete(string sessionId)
        {
            Channel.DownloadComplete(sessionId);
        }
    }
}
