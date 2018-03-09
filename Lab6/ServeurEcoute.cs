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
            //ÇA MARCHE???            
            //Déclaration des variables
            Socket LeSocket;
            EndPoint PointLocal, PointDistant;
            PointLocal = new IPEndPoint(0, 69);
            PointDistant = new IPEndPoint(0, 0);
            byte[] bTexte = new byte[516];
            byte[] bNomFich = new byte[100];
            int NbrRecu, IndiceTableau = 0;
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

                    if(!ValiderTrame(bTexte))
                    {                        
                        //Envoie un zéro
                        LeSocket.SendTo(new byte[0], PointDistant);
                    }
                    else
                    {                        
                        //Pour obtenir le nom du fichier en bytes
                        //2 PREMIER BYTES C'EST LA DEMANDE
                        for(int i = 2; bTexte[i] != 0x00; i++)
                        {
                            bNomFich[IndiceTableau] = bTexte[i];
                            IndiceTableau++;
                        }
                        //Conversion bytes en string
                        NomFichier = Encoding.ASCII.GetString(bNomFich).Substring(0, IndiceTableau);    

                        //Si le code est 0001, soit un RRQ
                        if (bTexte[1] == 0x01)
                        {
                            RRQ rrq = new RRQ();
                            rrq.SetPointDistant(PointDistant);
                            rrq.SetFichier(NomFichier);
                            LeThread = new Thread(new ThreadStart(rrq.MonThreadRRQ));
                            // LeThread = new Thread(() => rrq.MonThreadRRQ();         //Pas certain de ce passage, trouvé sur stackoverflow
                            LeThread.Start();
                        }


                        //Sinon, si le code est 0002, soit un WRQ
                        else if (bTexte[1] == 0x02)
                        {
                            WRQ wrq = new WRQ();
                            wrq.SetPointDistant(PointDistant);
                            wrq.SetFichier(NomFichier);
                            LeThread = new Thread(new ThreadStart(wrq.MonThreadWRQ));
                            // LeThread = new Thread(() => wrq.MonThreadWRQ(LeSocket, bTexte, NomFichier));             //Pas certain de ce passage, trouvé sur stackoverflow
                            LeThread.Start();
                        }
                    }
                }
            }            

        }


        //Méthode qui valide la trame
        private bool ValiderTrame(byte[] bTrame)
        {
            //Validation au complet de la trame 
            return false;
        }

    }
}
