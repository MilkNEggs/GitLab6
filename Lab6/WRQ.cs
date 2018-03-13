using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Lab6
{
    class WRQ
    {
        //Déclaration des variables
        EndPoint m_PointDistantWRQ;
        string m_StrFichierWRQ;

        //Méthode qui détermine le point distant
        public void SetPointDistant(EndPoint PointDistant)
        {
            m_PointDistantWRQ = PointDistant;
        }

        //Méthode qui détermine le nom du fichier à envoyer 
        public void SetFichier(string NomFichier)
        {
            m_StrFichierWRQ = NomFichier;
        }

        //Thread de WRQ
        public void MonThreadWRQ()
        {
            //Déclaration des variables
            EndPoint PointLocalThread = new IPEndPoint(0, 0);
            Socket SocketThread = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            bool Fin = false, Lire;
            string Chemin = @"F:\LesFichiers\" + m_StrFichierWRQ, Donnees;
            FileStream fsWRQ = new FileStream(Chemin, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter swWRQ = new StreamWriter(fsWRQ);
            byte[] bTrame = new byte[516];
            byte[] bEnvoie = new byte[25];
            int NoBloc = 1, NbrRecu, Arrets = 0, ErreurACK = 0;

            //Bind du socket sur le point local
            try
            {
                SocketThread.Bind(PointLocalThread);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //Traitement 
            while(!Fin || ErreurACK < 3 || Arrets < 10)
            {
                //Vérifie que une trame a été envoyée
                if(Lire = SocketThread.Poll(5000000, SelectMode.SelectRead)) //(SocketThread.Available > 0)
                {
                    NbrRecu = SocketThread.ReceiveFrom(bTrame, ref m_PointDistantWRQ);

                    if(bTrame[1] == 03)
                    {
                        //Si le numéro de bloc est valide
                        if(bTrame[2] == NoBloc)
                        {
                            //Écritue dans le fichier
                            Donnees = Encoding.ASCII.GetString(bTrame).Substring(4, NbrRecu);
                            swWRQ.WriteLine(Donnees);

                            //Écriture de la trame du ACK 
                            bEnvoie[0] = 0x00;
                            bEnvoie[1] = 0x04;
                            bEnvoie[2] = 0x00;
                            bEnvoie[3] = 0x00;

                        }
                        else
                        {
                            //Si le numéro de bloc est plus élevé
                            if(bTrame[2] < NoBloc)
                            {
                                bEnvoie[0] = 0x00;
                                bEnvoie[1] = 0x05;
                                bEnvoie[2] = 0x00;
                                bEnvoie = Encoding.ASCII.GetBytes("Le paquet précédent est manquant.");
                                bEnvoie[bEnvoie.Length] = 0x00;
                                ErreurACK++;
                            }

                            //Si le numéro de bloc est inférieur
                            else if(bTrame[2] > NoBloc)
                            {
                                bEnvoie[0] = 0x00;
                                bEnvoie[1] = 0x05;
                                bEnvoie[2] = 0x00;
                                bEnvoie = Encoding.ASCII.GetBytes("Le paquet a déjà été envoyé.");
                                bEnvoie[bEnvoie.Length] = 0x00;
                                ErreurACK++;
                            }
                        }

                        //Envoie de la trame au client
                        SocketThread.SendTo(bEnvoie, m_PointDistantWRQ);
                    }

                    if(NoBloc == 128)
                    {
                        NoBloc = 0;
                    }

                    //Vérifie si la dernière trame a été envoyée du client vers le serveur
                    if (bTrame.Length < 516)
                    {
                        Fin = true;
                    }
                }
                else
                {
                    Arrets++;
                }
            }
        }

        //Conversion du numéro de bloc en hexadécimal
        private byte[] NoBlocHexa(int NoBloc)
        {
            //Déclaration des variables
            byte[] NoHexa = new byte[2];
            byte[] bTampon = new byte[1];
            int Indice = 1, Result;
            string[] LettreHexa = new string[] { "A", "B", "C", "D", "E", "F" };


            //Traitement
            while (NoBloc > 0)
            {
                Result = (byte)(NoBloc % 16);
                if (Result >= 10)
                {
                    bTampon = Encoding.ASCII.GetBytes(LettreHexa[Result - 10]);
                    NoHexa[Indice] = bTampon[0];
                }
                else
                {
                    NoHexa[Indice] = (byte)Result;
                }
                NoBloc = NoBloc / 16;
                Indice--;
            }

            //Retour
            return NoHexa;
        }

    }
}
