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
            EndPoint PointLocalThread = new IPEndPoint(0, 0);
            Socket SocketThread = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] bTrame = new byte[516];
            byte[] bEnvoie = new byte[516];
            bool Fin = false;
            int NbrRecu, Arrets = 0, ErreurACK = 0;
            byte NoBloc = 0;
            byte[] Donnees = new byte[512];
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
                return;
            }
            //Traitement 
            bEnvoie[0] = 0;
            bEnvoie[1] = 3;
            bEnvoie[2] = 0;
            bEnvoie[3] = NoBloc;
            fsRRQ.Read(Donnees, 0, 512);

            SocketThread.SendTo(bEnvoie, m_PointDistantRRQ);

            while (!Fin || ErreurACK < 3 || Arrets < 10)
            {
                //Vérifie que une trame a été envoyée
                if (SocketThread.Poll(5000000, SelectMode.SelectRead)) //(SocketThread.Available > 0)
                {

                }
                else
                {
                    Arrets++;
                }
                //Si le numéro de bloc est valide
                //Écritue dans le fichier
                //Si le numéro de bloc est plus élevé
                //Si le numéro de bloc est inférieur
                //Envoie de la trame au client
                //Vérifie si la dernière trame a été envoyée du client vers le serveur
            }

        }
        private void EnvoieErreur(int NoErreur)
        {
            //Envoie un erreur fichier inconnu
        }
    }
}
