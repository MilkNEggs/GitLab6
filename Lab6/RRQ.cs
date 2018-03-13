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

        public RRQ()
        {
            
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
            EndPoint PointLocalThread = new IPEndPoint(0, 100);
            byte[] bTrame = new byte[516];
            int NbrRecu, Arrets = 0, ErreurACK = 0;
            int NoBloc = 1;
            FileStream fsRRQ;

            //Bind du socket sur le point local
            try
            {
                SocketThread.Bind(PointLocalThread);
                fsRRQ = new FileStream(m_strFichierRRQ, FileMode.Open,
                FileAccess.Read, FileShare.None);
            }
            catch (Exception Erreur)
            {
                MessageBox.Show(Erreur.ToString());
                EnvoieErreur(1);
                SocketThread.Close();
                return;
            }
            

            //Traitement 
            //Détermine le nombre de blocs à envoyés
            long LongueurFichier = fsRRQ.Length;
            int NbreBloc = (int)LongueurFichier / 512;
            if ((int)LongueurFichier % 512 != 0)
            {
                NbreBloc++;
            }

            //Boucle jusqu'à temps que tous les bloc sont 
            //envoyés ou que 3 ack ou que le transfert soit trop long

            while(NbreBloc >= NoBloc && ErreurACK < 3 && Arrets < 10 )
            {
                EnvoyerBloc(fsRRQ, (byte)NoBloc);
                if (SocketThread.Poll(5000000, SelectMode.SelectRead))
                {
                    NbrRecu = SocketThread.ReceiveFrom(bTrame, ref m_PointDistantRRQ);
                    //Ne correcspond pas au bon ack
                    if (bTrame[6] != '0' || bTrame[7] != NoBloc+48)
                        ErreurACK++;
                    //Si ça marché
                    else
                        NoBloc++;
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
            Buffer.BlockCopy(MessageErreur, 0, bErreur, 4, 30);
            SocketThread.SendTo(bErreur, m_PointDistantRRQ);
        }

        private void EnvoyerBloc(FileStream fsRRQ, byte NoBloc)
        {
            byte[] bEnvoie = new byte[516];
            byte[] Donnees = new byte[512];
            //Envoie un bloc de données selon le no. de bloc
            bEnvoie[0] = 0;
            bEnvoie[1] = 3;
            bEnvoie[2] = 0;
            bEnvoie[3] = NoBloc;
            fsRRQ.Seek(512 * (NoBloc - 1), SeekOrigin.Begin);
            fsRRQ.Read(Donnees,0, 512);
            Buffer.BlockCopy(Donnees, 0, bEnvoie, 4, Donnees.Length);
            SocketThread.SendTo(bEnvoie, m_PointDistantRRQ);
        }
    }
}
