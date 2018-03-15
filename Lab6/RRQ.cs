//William Lafontaine
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
    class RRQ
    {
        //Déclaration des variables

        EndPoint m_PointDistantRRQ;
        string m_strFichierRRQ;
        Socket SocketThread = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        long m_LongueurFichier;

        public RRQ()
        {
            m_LongueurFichier = 0;
        }
        //Méthode qui détermine le point distant

        public void SetPointDistant(EndPoint PointDistant)
        {
            m_PointDistantRRQ = PointDistant;
        }
        //Méthode qui détermine le nom du fichier à envoyer 

        public void SetFichier(string NomFichier)
        {
            m_strFichierRRQ = NomFichier;
        }
        //Thread de RRQ
        public void MonThreadRRQ()
        {
            //Déclaration des variables
            EndPoint PointLocalThread = new IPEndPoint(0, 0);
            byte[] bTrame = new byte[516];
            int NbrRecu, Arrets = 0, ErreurACK = 0;
            int NoBloc = 1, NoBloc2 = 0;
            FileStream fsRRQ;

            //Bind du socket sur le point local et ouverture du filestream
            try
            {
                SocketThread.Bind(PointLocalThread);
                fsRRQ = new FileStream(m_strFichierRRQ, FileMode.Open,
                FileAccess.Read, FileShare.None);
            }
            catch (Exception Erreur)
            {
            //    MessageBox.Show(Erreur.ToString());
                EnvoieErreur(1);
                SocketThread.Close();
                return;
            }
            

            //Traitement 
            //Détermine le nombre de blocs à envoyés
            m_LongueurFichier = fsRRQ.Length;
            int NbreBloc = (int)m_LongueurFichier / 512;
            if ((int)m_LongueurFichier % 512 != 0)
            {
                NbreBloc++;
            }

            //Boucle jusqu'à temps que tous les bloc sont 
            //envoyés ou que 3 ack ou que le transfert soit trop long

            while(NbreBloc >= NoBloc && ErreurACK < 3 && Arrets < 10 )
            {
                EnvoyerBloc(fsRRQ, (byte)NoBloc, (byte)NoBloc2);
                if (SocketThread.Poll(5000000, SelectMode.SelectRead))
                {
                    NbrRecu = SocketThread.ReceiveFrom(bTrame, ref m_PointDistantRRQ);
                    //Ne correcspond pas au bon ack
                    if (bTrame[2] != (NoBloc & 0xFF00) || bTrame[3] != (NoBloc & 0xFF))
                        ErreurACK++;
                    //Si ça marché
                    else
                    {
                        NoBloc++;
                        if(NoBloc == 128)
                        {
                            NoBloc = 0;
                            NoBloc2++;
                        }
                    }

                }
                //Le temps de l'envoie a été trop long
                else
                    Arrets++;
            }
            //Termine le socket et le filestream
            fsRRQ.Close();
            SocketThread.Close();
        }
        private void EnvoieErreur(byte NoErreur)
        {
            //Envoie une erreur fichier inconnu
            byte[] bErreur = new byte[516];
            bErreur[0] = 0;
            bErreur[1] = 5;
            bErreur[2] = 0;
            bErreur[3] = NoErreur;
            byte[] MessageErreur = new byte[30];
            MessageErreur = Encoding.ASCII.GetBytes("Fichier inconnu");
            Buffer.BlockCopy(MessageErreur, 0, bErreur, 4, MessageErreur.Length);
            SocketThread.SendTo(bErreur, m_PointDistantRRQ);
        }

        private void EnvoyerBloc(FileStream fsRRQ, byte NoBloc, byte NoBloc2)
        {
            byte[] Donnees;

            if (((NoBloc2 * 128) + NoBloc) == (m_LongueurFichier / 512) + 1)
                Donnees = new byte[m_LongueurFichier % 512];
            else
                Donnees = new byte[512];
            byte[] bEnvoie = new byte[4 + Donnees.Length];
            //Envoie un bloc de données selon le no. de bloc
            bEnvoie[0] = 0;
            bEnvoie[1] = 3;
            bEnvoie[2] = NoBloc2;
            bEnvoie[3] = NoBloc;
            fsRRQ.Seek(512 * (((NoBloc2 * 128) + NoBloc) - 1), SeekOrigin.Begin);
            fsRRQ.Read(Donnees,0, Donnees.Length);
            Buffer.BlockCopy(Donnees, 0, bEnvoie, 4, Donnees.Length);
            SocketThread.SendTo(bEnvoie, m_PointDistantRRQ);
        }
    }
}
