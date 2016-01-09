using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using System.IO;
using ChunkingFileTransferServices.Properties;

namespace ChunkingFileTransferServices
{
    public class FileTransferService : IFileTransferService
    {
        static Dictionary<Guid, Tuple<TransferInfo, Stream>> _BufferedSessions = new Dictionary<Guid, Tuple<TransferInfo, Stream>>();

        string IFileTransferService.BeginUploadSession(TransferInfo uploadInfo)
        {
            Guid sessionId = Guid.NewGuid();
            lock (_BufferedSessions)
            {
                FileStream stream = File.OpenWrite(uploadInfo.FilePath);
                _BufferedSessions.Add(sessionId, new Tuple<TransferInfo,Stream>(uploadInfo,stream));
            }
            return sessionId.ToString();
        }

        void IFileTransferService.UploadChunk(string sessionId, byte[] chunk)
        {
            Stream chunkBuffer = GetSession(sessionId).Item2;
            // Go to the end of the stream and write in the chunk 
            chunkBuffer.Seek(chunkBuffer.Length, SeekOrigin.Begin);
            chunkBuffer.Write(chunk, 0, chunk.Length);
            chunkBuffer.Flush();
        }

        void IFileTransferService.CompleteUpload(string sessionId)
        {
            Tuple<TransferInfo, Stream> session = GetSession(sessionId);
            if (session.Item2.Length != session.Item1.SizeBytes)
            {
                throw new InvalidOperationException("Number of bytes uploaded is not equal to the number of bytes specified in the AttachmentInfo");
            }
            session.Item2.Flush();
            session.Item2.Close();
            lock (_BufferedSessions)
            {
                _BufferedSessions.Remove(GetSessionId(sessionId));
            }
        }

        DownloadSessionInfo IFileTransferService.StartDownload(TransferInfo downloadInfo)
        {
            Guid sessionId = Guid.NewGuid();
            FileStream stream = File.OpenRead(downloadInfo.FilePath);
            long remainder;
            long chunkCount;
            long chunkSize;
            CalculateChunkCountSizeAndRemainder(stream.Length, out remainder, out chunkCount, out chunkSize);
            lock (_BufferedSessions)
            {
                _BufferedSessions.Add(sessionId, new Tuple<TransferInfo,Stream>(downloadInfo,stream));
            }
            DownloadSessionInfo downloadSessionInfo = new DownloadSessionInfo
            {
                ChunkCount = chunkCount + (remainder > 0 ? 1 : 0),
                SessionId = sessionId.ToString()
            };
            return downloadSessionInfo;
        }

        byte[] IFileTransferService.DownloadChunk(string sessionId, long chunkIndex)
        {
            Tuple<TransferInfo, Stream> session = GetSession(sessionId);
            Stream chunkBuffer = session.Item2;
            long remainder;
            long chunkCount;
            long chunkSize;
            CalculateChunkCountSizeAndRemainder(chunkBuffer.Length, out remainder, out chunkCount, out chunkSize);

            long totalChunks = chunkCount + (remainder > 0 ? 1 : 0);
            if (chunkIndex >= totalChunks)
                throw new ArgumentException(string.Format("Cannot download chunk index {0} for file because it there are only {1} chunks total", chunkIndex, totalChunks));

            long thisChunkSize = chunkSize;

            if (chunkIndex == chunkCount && remainder > 0)
            {
                thisChunkSize = remainder;
            }

            byte[] chunk = new byte[thisChunkSize];
            chunkBuffer.Read(chunk, 0, (int)thisChunkSize);
            return chunk;
        }

        void IFileTransferService.DownloadComplete(string sessionId)
        {
            var session = GetSession(sessionId);
            session.Item2.Flush();
            session.Item2.Close();
            lock (_BufferedSessions)
            {
                _BufferedSessions.Remove(GetSessionId(sessionId));
            }
        }

        private static Guid GetSessionId(string sessionId)
        {
            Guid sessionIdKey;
            try
            {
                sessionIdKey = new Guid(sessionId);

            }
            catch (Exception ex)
            {
                throw new ArgumentException("sessionId must be parsable into a GUID.", ex);
            }
            return sessionIdKey;
        }

        private static Tuple<TransferInfo,Stream> GetSession(string sessionId)
        {
            lock (_BufferedSessions)
            {
                Guid sessionIdKey = GetSessionId(sessionId);
                if (!_BufferedSessions.ContainsKey(sessionIdKey))
                {
                    throw new ArgumentException("Invalid sessionId");
                }
                return _BufferedSessions[sessionIdKey];
            }
        }
        
        internal static void CalculateChunkCountSizeAndRemainder(long dataLength, out long remainder, out long chunkCount, out long chunkSize)
        {
            // Compute how much will be left over for the last chunk
            chunkSize = Settings.Default.ChunkSizeBytes;
            remainder = dataLength % chunkSize;
            chunkCount = dataLength / chunkSize;

            // Handle the case of small files
            if (chunkSize >= dataLength)
            {
                chunkSize = dataLength;
                remainder = 0;
                chunkCount = 1;
            }
        }


    }
}
