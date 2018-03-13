using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;


namespace Lab6
{
    class ServeurEcoute
    {
        //Déclaration des variables memebres
        public bool m_Fin;

        //Méthode de la thread
        public void MaThreadEcoute()
        {
            //Déclaration des variables
            Socket LeSocket;
            EndPoint PointLocal, PointDistant;
            PointLocal = new IPEndPoint(0, 69);
            PointDistant = new IPEndPoint(0, 0);
            byte[] bTexte = new byte[516];
            byte[] bNomFich = new byte[100];
            byte[] bErreur = new byte[100];
            byte[] MessageErreur = new byte[30];
            int NbrRecu, i;
            Thread LeThread;
            string NomFichier, sTexte;
            try
            {
                LeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                LeSocket.Bind(PointLocal);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            //Boucle de la thread
            while (!m_Fin)
            {
                if (LeSocket.Available > 0)
                {
                    NbrRecu = LeSocket.ReceiveFrom(bTexte, ref PointDistant);

                    sTexte = Encoding.ASCII.GetString(bTexte).Substring(0, NbrRecu);
                    //Conversion bytes en string
                    i = 2;
                    for (i = 2; bTexte[i] == 0; i++)
                        bNomFich[i - 2] = bTexte[i];
                    NomFichier = Encoding.ASCII.GetString(bNomFich).Substring(0, i - 2);

                    switch (ValiderTrame(bTexte))
                    {
                        //Pour obtenir le nom du fichier en bytes
                        //Si le code est 0001, soit un RRQ
                        case 1:
                            RRQ rrq = new RRQ();
                            rrq.SetPointDistant(PointDistant);
                            rrq.SetFichier(NomFichier);
                            LeThread = new Thread(new ThreadStart(rrq.MonThreadRRQ));
                            LeThread.Start();
                            break;
                        //Sinon, si le code est 0002, soit un WRQ
                        case 2:
                            WRQ wrq = new WRQ();
                            wrq.SetPointDistant(PointDistant);
                            wrq.SetFichier(NomFichier);
                            LeThread = new Thread(new ThreadStart(wrq.MonThreadWRQ));
                            LeThread.Start();
                            break;
                        //Construit un message d'erreur
                        case 0:
                            bErreur[0] = 0x00;
                            bErreur[1] = 0x05;
                            bErreur[2] = 0x00;
                            bErreur[3] = 0x04;
                            MessageErreur = Encoding.ASCII.GetBytes("Opération TFTP illégale.");
                            Buffer.BlockCopy(MessageErreur, 0, bErreur, 4, 30);
                            bErreur[33] = 0x00;
                            LeSocket.SendTo(bErreur, PointDistant); //ça plante surement donc j'aime bien les string, conversion byte[] --> string et string --> byte[]
                            break;
                    }
                }
            }
            LeSocket.Close();
        }


        //Méthode qui valide la trame
        private int ValiderTrame(byte[] bTrame)
        {
            //Validation au complet de la trame 
            if (bTrame[0] == 0 && bTrame[1] == 1)
                return 1;
            else if (bTrame[0] == 0 && bTrame[1] == 2)
                return 2;
            else
                return 0;
        }

    }
}