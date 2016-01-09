using System;
using System.IO;

namespace Contracts
{
    public class ChunkingTransferServiceWrapper : IFileTransferServiceFacade, IDisposable
    {
        private IFileTransferService m_Proxy;

        public int SendChunkSize { get; set; }

        public ChunkingTransferServiceWrapper()
        {
            m_Proxy = new FileTransferServiceProxy();
        }
        public ChunkingTransferServiceWrapper(IFileTransferService proxy)
        {
            m_Proxy = proxy;
        }

        #region IObjectStoreService Members

        public void Upload(TransferInfo transferInfo, Stream data)
        {
            // Compute how much will be left over for the last chunk
            long dataLength = data.Length;
            long chunkSize = SendChunkSize == 0 ? 256000 : SendChunkSize;
            long remainder = dataLength % chunkSize;
            long chunkCount = dataLength / chunkSize;

            // Handle the case of small files
            if (chunkSize >= dataLength)
            {
                chunkSize = dataLength;
                remainder = 0;
                chunkCount = 1;
            }

            string sessionId = m_Proxy.BeginUploadSession(transferInfo);
            for (long index = 0; index < chunkCount; index++)
            {
                byte[] chunk = new byte[chunkSize];
                data.Read(chunk, (int)(index * chunkSize), (int)chunkSize);
                m_Proxy.UploadChunk(sessionId, chunk);
            }
            if (remainder > 0)
            {
                byte[] chunk = new byte[remainder];
                data.Read(chunk, (int)(chunkCount * chunkSize), (int)remainder);
                m_Proxy.UploadChunk(sessionId, chunk);
            }
            m_Proxy.CompleteUpload(sessionId);

        }

        public void Download(TransferInfo transferInfo, Stream stream)
        {
            DownloadSessionInfo sessionInfo = m_Proxy.StartDownload(transferInfo);

            for (int i = 0; i < sessionInfo.ChunkCount; i++)
            {
                byte[] chunk = m_Proxy.DownloadChunk(sessionInfo.SessionId, i);
                stream.Write(chunk, 0, chunk.Length);
            }
            m_Proxy.DownloadComplete(sessionInfo.SessionId);
        }

        #endregion


        public void Dispose()
        {
            using (m_Proxy as IDisposable) { }
        }
    }
}

