using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

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
        public void MonThreadWRQ(Socket Serveur, byte[] Trame, string NomFichier)
        {
            //Déclaration des variables
            EndPoint PointLocalThread = new IPEndPoint(0, 0);
            Socket SocketThread = new Socket();
            bool Fin = false;
            string Chemin = @"F:\LesFichiers\" + NomFichier, Donnees;
            FileStream fsWRQ = new FileStream(Chemin, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter swWRQ = new StreamWriter(fsWRQ);
            byte[] bTrame = new byte[516];
            byte[] bEnvoie = new byte[25];
            int NoBloc = 1, NbrRecu;

            //Traitement 
            while(!Fin)
            {
                //Vérifie que une trame a été envoyée
                if(Serveur.Available > 0)
                {
                    NbrRecu = Serveur.ReceiveFrom(bTrame, ref m_PointDistantWRQ);

                    if(bTrame[1] == 03)
                    {
                        //Si le numéro de bloc est valide
                        if(bTrame[2] == NoBloc)
                        {
                            //Écritue dans le fichier
                            Donnees = Encoding.ASCII.GetString(bTrame).Substring(4, NbrRecu);
                            swWRQ.WriteLine(Donnees);

                            //Envoie du ACK au client

                        }
                        else
                        {
                            //Si le numéro de bloc est plus élevé
                            if(bTrame[2] < NoBloc)
                            {
                                bEnvoie[0] = 00;
                                bEnvoie[1] = 05;
                                bEnvoie[2] = 00;
                                bEnvoie = Encoding.ASCII.GetBytes("Le paquet précédent est manquant.");
                                bEnvoie[bEnvoie.Length] = 00;
                            }

                            //Si le numéro de bloc est inférieur
                            else if(bTrame[2] > NoBloc)
                            {
                                bEnvoie[0] = 00;
                                bEnvoie[1] = 05;
                                bEnvoie[2] = 00;
                                bEnvoie = Encoding.ASCII.GetBytes("Le paquet a déjà été envoyé.");
                                bEnvoie[bEnvoie.Length] = 00;
                            }
                        }
                    }


                    //Vérifie si la dernière trame a été envoyée
                    if (Trame.Length < 516)
                    {
                        Fin = false;
                    }
                }
            }

        }

    }
}
