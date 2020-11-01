using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace BibliotekKlas
{
    /// <summary>
    /// Klasa SerwerTCP implementuje działanie serwera TCP, pozwala na wysłanie
    /// serwerowi zapytania o wartość n-tego wyrazu ciągu tribonaciego
    /// </summary>
    public class SerwerTCP
    {
        IPAddress mojeIP;
        int mojPort;
        byte[] bufor;
        private string witaj = "Witaj! \r\n";
        private string podajLiczbe = "Podaj ktory element ciagu Tribonacciego mam obliczyc (od 0): \r\n";
        private string koniec = "koniec";
        int wyrazCiagu;
        int wynik;

        TcpListener serwer;
        TcpClient klient;

        /// <summary>
        /// Konstruktor domyślny
        /// </summary>
        public SerwerTCP()
        {
            mojeIP = IPAddress.Parse("127.0.0.1");
            mojPort = 7777;
            bufor = new byte[1024];

            serwer = new TcpListener(mojeIP, mojPort); 
        }

        /// <summary>
        /// Konstruktor klasy SerwerTCP z dwoma argumentami: adres IP oraz numer portu.
        /// </summary>
        /// <param mojeIP="adresIP"></param>
        /// <param mojPort="port"></param>
        public SerwerTCP(IPAddress adresIP, int port)
        {
            mojeIP = adresIP;
            mojPort = port;
            bufor = new byte[1024];

            serwer = new TcpListener(mojeIP, mojPort);
        }


        
        /// <summary>
        /// Metoda Start() uruchamia serwer, łączy się z klientem, wysyła powitalną wiadomość, odpowiada na zapytanie.
        /// </summary>
        public void Start()
        {
            serwer.Start();
            Console.WriteLine("Oczekiwanie na klienta.");
            klient = serwer.AcceptTcpClient();
            Console.WriteLine("Połączono.");

            //Przywitanie z użytkownikiem
            bufor = Encoding.ASCII.GetBytes(witaj);
            klient.GetStream().Write(bufor, 0, bufor.Length);
            Array.Clear(bufor, 0, bufor.Length);


            while (true)
            {
                if (klient.Connected)
                {
                    //Porśba o podanie liczy
                    bufor = Encoding.ASCII.GetBytes(podajLiczbe);
                    klient.GetStream().Write(bufor, 0, bufor.Length);
                    Array.Clear(bufor, 0, bufor.Length);

                    klient.GetStream().Read(bufor, 0, bufor.Length);

                    if (bufor[0] != 13 && bufor[0] != 0)
                    {
                        if (Encoding.ASCII.GetString(bufor).Contains(koniec))
                        {
                            klient.Close();
                            Console.WriteLine("Połączenie zakończono.");
                            break;
                        }
                        else
                        {
                            wyrazCiagu = ZamienByteNaInt(bufor);
                            wynik = Tribonacci(wyrazCiagu);
                            bufor = Encoding.ASCII.GetBytes(wyrazCiagu + " wyraz ciagu Tribonacciego to: " + wynik + ".\r\n" + "Aby zakonczyc wyslij 'koniec'.\r\n");
                            klient.GetStream().Write(bufor, 0, bufor.Length);

                        }

                        Array.Clear(bufor, 0, bufor.Length);
                    }
                }
                else
                    Console.WriteLine("Przerwano połączenie.");
            }

        }

        /// <summary>
        /// Funkcja zmienia otrzymany ciąg bytów na wartość int.
        /// </summary>
        /// <param tymczasowy="bufor"></param>
        /// <returns></returns>
        int ZamienByteNaInt(byte[] bufor)
        {
            string tymczasowy = ASCIIEncoding.ASCII.GetString(bufor);

            return Int32.Parse(tymczasowy);
        }

        /// <summary>
        /// Obliczanie kolejnych wyrazów ciągu Tribonacciego
        /// </summary>
        /// <param name="wyraz"></param>
        /// <returns></returns>
        public int Tribonacci(int wyraz)
        {

            if (wyraz == 0)
                return 0;
            else if (wyraz == 1)
                return 1;
            else if (wyraz == 2)
                return 1;
            else
                return Tribonacci(wyraz - 1) + Tribonacci(wyraz - 2) + Tribonacci(wyraz - 3);
        }

    }
}
