using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BibliotekKlas
{
    public class SerwerAsynchroniczny : Serwer
    {
        #region Zmienne
        Dictionary<String, String> uzytkownicy; 
        String podajLogin;
        String podajHaslo;
        String witaj;

        /// <summary>
        /// Delegatura watku dla każdego klienta
        /// </summary>
        /// <param name="stream"></param>
        public delegate void watekTransmisji(NetworkStream stream);
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy SerwerAsynchroniczny dziedziczącej po klasie Serwer
        /// </summary>
        /// <param IP="IP"></param>
        /// <param port="port"></param>
        public SerwerAsynchroniczny(IPAddress IP, int port) : base(IP, port)
        {
            podajLogin = "Podaj login: ";
            podajHaslo = "Podaj haslo: ";
            witaj = "Witaj uzytkowniku! \n\r";
            uzytkownicy = new Dictionary<String, String>();
            uzytkownicy.Add("admin", "admin");
            uzytkownicy.Add("root", "admin");
            uzytkownicy.Add("user","user");
        }

        #endregion

        #region Funkcje
        /// <summary>
        /// Funkcja akceptująca połączenie z klientem oraz tworząca wątek dla każdego klienta.
        /// </summary>
        protected override void akceptujPolaczenie()
        {
            
            while (true)

            {
                TcpClient tcpKlient = TcpListener.AcceptTcpClient();

                Stream = tcpKlient.GetStream();

                watekTransmisji watek = new watekTransmisji(rozpocznijTransmisje);
                watek.BeginInvoke(Stream, Callback,tcpKlient);
            }
        }

        /// <summary>
        /// Callback wywoływany po zakończeniu wątku
        /// </summary>
        protected void Callback(IAsyncResult ar)
        {
            Console.WriteLine("Koniec polaczenia");
        }


        /// <summary>
        /// Funkcja wykonywana w każdym wątku, dla każdego klienta. 
        /// </summary>
        /// <param name="stream"></param>
        protected override void rozpocznijTransmisje(NetworkStream stream)
        {
            logowanie(stream);
            Bufor = Encoding.ASCII.GetBytes("Witaj w serwerze echo!");
            stream.Write(Bufor, 0, Bufor.Length);
            Array.Clear(Bufor, 0, Bufor.Length);

            while (true)
            {
                try
                {
                    Bufor = new byte[RozmiarBufora];
                    int message_size = stream.Read(Bufor, 0, RozmiarBufora);
          
                    String wiad = dekodowanieWiadomosci(message_size);
                    if (wiad.ToString().Equals(":q!"))
                    {
                        Bufor = Encoding.ASCII.GetBytes("Koniec polaczenia.");
                        stream.Write(Bufor, 0, Bufor.Length);
                        Array.Clear(Bufor, 0, Bufor.Length);

                        break;
                    }
                    stream.Write(Bufor, 0, message_size);
                    Array.Clear(Bufor, 0, RozmiarBufora);
                }
                catch (IOException e)
                {
                    break;
                }

            }
        }


        /// <summary>
        /// Wywoływana przez rozpocznijTransmisję, po poprawnym zalogowaniu, powraca do funkcji wywołującej.
        /// </summary>
        /// <param name="stream"></param>
        protected void logowanie(NetworkStream stream)
        {
            Bufor = Encoding.ASCII.GetBytes(witaj);
            stream.Write(Bufor, 0, Bufor.Length);
            Array.Clear(Bufor, 0, Bufor.Length);

            while (true)
            {
                try
                {
                   
                    Bufor = Encoding.ASCII.GetBytes(podajLogin);
                    stream.Write(Bufor, 0, Bufor.Length);
                    Array.Clear(Bufor, 0, Bufor.Length);

                    Bufor = new byte[RozmiarBufora];

                    int message_size = stream.Read(Bufor, 0, RozmiarBufora);


                    if (Bufor[0] == 13)
                    {
                        message_size = stream.Read(Bufor, 0, RozmiarBufora);
                    }

                    String login = dekodowanieWiadomosci(message_size);

                    if (uzytkownicy.ContainsKey(login))
                    {
                        Bufor = Encoding.ASCII.GetBytes(podajHaslo);
                        stream.Write(Bufor, 0, Bufor.Length);
                        Array.Clear(Bufor, 0, Bufor.Length);

                        Bufor = new byte[RozmiarBufora];
                        message_size = stream.Read(Bufor, 0, RozmiarBufora);
                        if(Bufor[0] == 13)
                        {
                            message_size = stream.Read(Bufor, 0, RozmiarBufora);
                            String haslo = dekodowanieWiadomosci(message_size);
                            String temp;
                            uzytkownicy.TryGetValue(login, out temp);
                            if (temp == haslo)
                            {
                                return;
                            }
                            else
                            {
                                Bufor = Encoding.ASCII.GetBytes("Bledne haslo. Sprobuj jeszcze raz.\r\n");
                                stream.Write(Bufor, 0, Bufor.Length);
                                Array.Clear(Bufor, 0, Bufor.Length);
                            }
                        }
                    }
                    else
                    {
                        Bufor = Encoding.ASCII.GetBytes("Bledny login. \r\n");
                        stream.Write(Bufor, 0, Bufor.Length);
                        Array.Clear(Bufor, 0, Bufor.Length);
                    }
                    

                }
                catch (IOException e)
                {
                    return;
                }
            }

        }


        /// <summary>
        /// Pomocnicza funkcja zamieniająca tablicę bytów na String bez białych znaków i zer 
        /// </summary>
        /// <param name="rozmiarWiadomosci"></param>
        /// <returns></returns>
        private String dekodowanieWiadomosci(int rozmiarWiadomosci)
        {
            char[] wiadomosc = new char[rozmiarWiadomosci];
            for(int i = 0; i < rozmiarWiadomosci; i++)
            {
                wiadomosc[i] = (char)Bufor[i]; 
            }
            String wynik = new String(wiadomosc);
            return wynik;
        }


        /// <summary>
        /// Odpowiada za uruchomienie instancji serwera.
        /// </summary>
        public override void start()
        {
            zacznijNasluchiwanie();
            akceptujPolaczenie();
        }

    }
    #endregion
}
