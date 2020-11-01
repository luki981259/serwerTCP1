using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BibliotekKlas
{
    /// <summary>
    /// Abstrakcyjna klasa Serwer
    /// </summary>
    public abstract class Serwer
    {
        #region Pola

        IPAddress mojeIP;
        int mojPort;
        int rozmiarBufora;
        byte[] bufor;
        TcpListener serwer;
        TcpClient klient;
        NetworkStream stream;
        bool polaczony;
        #endregion

        #region Właściwości Pól
        protected TcpListener TcpListener { get => serwer; set => serwer = value; }

        protected TcpClient TcpClient { get => klient; set => klient = value; }

        protected NetworkStream Stream { get => stream; set => stream = value; }

        protected byte[] Bufor { get => bufor; set => bufor = value; }

        protected int RozmiarBufora { get => rozmiarBufora; set => rozmiarBufora = value; }

        protected bool Polaczony { get => polaczony; set => polaczony = value; }
        #endregion

        #region Konstruktory

        /// <summary>
        /// Konstruktor domyślny
        /// </summary>
        public Serwer()
        {
            mojeIP = IPAddress.Parse("127.0.0.1");
            mojPort = 7777;
            bufor = new byte[1024];
            rozmiarBufora = 1024;
            polaczony = false;
        }

        /// <summary>
        /// Konstruktor klasy SerwerTCP z dwoma argumentami: adres IP oraz numer portu.
        /// </summary>
        /// <param mojeIP="adresIP"></param>
        /// <param mojPort="port"></param>
        public Serwer(IPAddress adresIP, int port)
        {
            mojeIP = adresIP;
            mojPort = port;
            bufor = new byte[1024];
            rozmiarBufora = 1024;
            polaczony = false;
        }

        #endregion

        #region Funkcje
        /// <summary>
        /// Rozpoczyna nasłuchiwanie przez serwer
        /// </summary>
        protected void zacznijNasluchiwanie()
        {
            serwer = new TcpListener(mojeIP, mojPort);
            serwer.Start();
            Console.Out.WriteLine("Rozpoczecie nasluchiwania.");
        }

        /// <summary>
        /// Funkcja akceptująca połączenie z klientem
        /// </summary>
        protected abstract void akceptujPolaczenie();

        /// <summary>
        /// Rozpoczęcie transmisji pomiędzy serwerem i klientem
        /// </summary>
        protected abstract void rozpocznijTransmisje(NetworkStream stream);

        /// <summary>
        /// Wystartowanie działania
        /// </summary>
        public abstract void start();

        #endregion


    }
}
