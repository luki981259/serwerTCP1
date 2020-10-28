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
            BibliotekKlas.SerwerTCP serwer = new BibliotekKlas.SerwerTCP();
            serwer.Start();
         
        }
    }
}
