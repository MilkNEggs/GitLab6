using System;
using System.Text;
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

            //Ouverture du socket
            try
            {
                LeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                LeSocket.Bind(PointLocal);
            }
            //Si la connexion a des problèmes
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            try
            {
                //Boucle de la thread
                while (!m_Fin)
                {
                    if (LeSocket.Available > 0)
                    {
                        //Reçois une trame
                        NbrRecu = LeSocket.ReceiveFrom(bTexte, ref PointDistant);
                        sTexte = Encoding.ASCII.GetString(bTexte).Substring(0, NbrRecu);
                        //Conversion bytes en string du fichier
                        for (i = 2; bTexte[i] != 0; i++)
                            bNomFich[i - 2] = bTexte[i];
                        NomFichier = Encoding.ASCII.GetString(bNomFich).Substring(0, i - 2);
                        //Valide une trame et envoie le client au rrq ou wrq, sinon envoie une erreur au client
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
                                Buffer.BlockCopy(MessageErreur, 0, bErreur, 4, 1);
                                bErreur[33] = 0x00;
                                //Envoie la trame erreur
                                LeSocket.SendTo(bErreur, PointDistant);
                                break;
                        }
                    }
                }
            }
            //Si la connexion a des problèmes
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            //Fini toujours par fermer le socket réseau
            finally
            {
                LeSocket.Close();
            }                
        }
        //Méthode qui valide la trame
        private int ValiderTrame(byte[] bTrame)
        {
            //Validation au complet de la trame 
            //En retournant une valeur qui égal à wrq ou rrq, sinon envoie un 0 pour erreur
            if (bTrame[0] == 0 && bTrame[1] == 1 && bTrame[2] != 0)
                return 1;
            else if (bTrame[0] == 0 && bTrame[1] == 2 && bTrame[2] != 0)
                return 2;
            else
                return 0;
        }
    }
}