using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ChunkingFileTransferServices;

namespace DebugHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(FileTransferService));
            host.Open();
            Console.WriteLine("Press Enter to Exit...");
            Console.ReadLine();
            host.Close();
        }
    }
}
