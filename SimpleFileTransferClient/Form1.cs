using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contracts;
using System.IO;

namespace SimpleFileTransferClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            using (ChunkingTransferServiceWrapper smartProxy = new ChunkingTransferServiceWrapper())
            {
                FileStream stream = File.OpenRead("Uploadtest.txt");
                smartProxy.Upload(new TransferInfo { FilePath = @"C:\test\uploadtest.txt", SizeBytes = stream.Length }, stream);
            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            using (ChunkingTransferServiceWrapper smartProxy = new ChunkingTransferServiceWrapper())
            {
                FileStream stream = File.OpenWrite("DownloadTest.txt");
                smartProxy.Download(new TransferInfo { FilePath = @"C:\test\DownloadTest.txt" }, stream);
            }
        }
    }
}
