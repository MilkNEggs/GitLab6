using System;
using System.Text;
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

        //Constructeur
        public WRQ()
        {

        }

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
            string Chemin = m_StrFichierWRQ;
            byte[] bTrame = new byte[516];
            byte[] bEnvoie = new byte[25];
            byte[] bNoBloc = new byte[2];
            byte[] bErreur = new byte[100];
            byte[] MessageErreur = new byte[30];
            BinaryWriter bwWRQ = null;
            int NoBloc = 1, NbrRecu, Arrets = 0, ErreurACK = 0, NoBloc2 = 0;            

            //Vérification si le fichier existe déjà, envoie d'un message d'erreur si oui
            if (!File.Exists(Chemin))
            {
                //Création du fichier
                try
                {
                    bwWRQ = new BinaryWriter(File.OpenWrite(m_StrFichierWRQ));

                    //Bind du socket sur le point local
                    SocketThread.Bind(PointLocalThread);
                }
                catch (Exception ex)
                {
                    return;
                }                
                //Boucle d'écriture dans le fichier voulu
                try
                {
                    //Écriture de la trame du premier ACK et envoie
                    bEnvoie[0] = 0;
                    bEnvoie[1] = 4;
                    bEnvoie[2] = 0;
                    bEnvoie[3] = 0;
                    SocketThread.SendTo(bEnvoie, m_PointDistantWRQ);

                    while (!Fin && ErreurACK < 3 && Arrets < 10)
                    {
                        //Vérifie que une trame a été envoyée
                        if (Lire = SocketThread.Poll(5000000, SelectMode.SelectRead))
                        {                            
                            NbrRecu = SocketThread.ReceiveFrom(bTrame, ref m_PointDistantWRQ);
                            if (bTrame[1] == 3)
                            {
                                //Si le numéro de bloc est valide
                                if (bTrame[2] == (NoBloc2 & 0xFF) && bTrame[3] == (NoBloc & 0xFF))
                                {
                                    //Écriture de la trame du ACK 
                                    bEnvoie[0] = 0;
                                    bEnvoie[1] = 4;
                                    bEnvoie[2] = (byte)NoBloc2;
                                    bEnvoie[3] = (byte)NoBloc;
                                    SocketThread.SendTo(bEnvoie, m_PointDistantWRQ);

                                    //Écritue dans le fichier
                                    bwWRQ.Write(bTrame, 4, NbrRecu - 4);
                                    
                                    NoBloc++;
                                    //Si le numéro de bloc atteint sa capacité maximale (FF FF ou 65535)
                                    if (NoBloc == 256)
                                    {
                                        NoBloc2++;
                                        NoBloc = 0;
                                        if (NoBloc2 == 256)
                                            NoBloc2 = 0;
                                    }
                                    //Vérifie si la dernière trame a été envoyée du client vers le serveur
                                    if (NbrRecu < 516)
                                        Fin = true;
                                }
                                else
                                    ErreurACK++;
                            }                                                       
                        }
                        else
                            Arrets++;
                    }

                }                
                catch (Exception ex)
                {
                    return;
                }
                finally
                {
                    bwWRQ.Close();
                    SocketThread.Close();
                }
            }            
            //Si le nom de fichier spécifié est non valide
            else
            {
                bErreur[0] = 0;
                bErreur[1] = 5;
                bErreur[2] = 0;
                bErreur[3] = 6;
                MessageErreur = Encoding.ASCII.GetBytes("Le fichier existe deja.");
                Buffer.BlockCopy(MessageErreur, 0, bErreur, 4, MessageErreur.Length);
                bErreur[33] = 0;
                SocketThread.SendTo(bErreur, m_PointDistantWRQ);
            }            
        }
    }
}
