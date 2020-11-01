using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace serwerTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            BibliotekKlas.SerwerAsynchroniczny serwer = new BibliotekKlas.SerwerAsynchroniczny(IPAddress.Parse("127.0.0.1"),7777);
            serwer.start();
         
        }
    }
}
