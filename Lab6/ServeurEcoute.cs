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
        private Socket m_LeSocket;
        private EndPoint m_PointLocal, m_PointDistant;


        //Méthode de la thread
        public void MaThreadEcoute()
        {
            //Déclaration des variables
            m_PointLocal = new IPEndPoint(0, 69);
            m_PointDistant = new IPEndPoint(0, 0);
            byte[] bTexte = new byte[516];
            byte[] bNomFich = new byte[100];
            int NbrRecu, IndiceTableau = 0;
            Thread LeThread;
            string NomFichier, sTexte;

            try
            {
                m_LeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_LeSocket.Bind(m_PointLocal);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            //Boucle de la thread
            while (!m_Fin)
            {
                if (m_LeSocket.Available > 0)
                {
                    NbrRecu = m_LeSocket.ReceiveFrom(bTexte, ref m_PointDistant);

                    sTexte = Encoding.ASCII.GetString(bTexte).Substring(0, NbrRecu);         

                    //Pour obtenir le nom du fichier en bytes
                    for(int i = 2; bTexte[i] != 00; i++)
                    {
                        bNomFich[IndiceTableau] = bTexte[i];
                        IndiceTableau++;
                    }
                    //Conversion bytes en string
                    NomFichier = Encoding.ASCII.GetString(bNomFich).Substring(0, IndiceTableau);    

                    //Si le code est 0001, soit un RRQ
                    if (bTexte[1] == 49)
                    {
                        RRQ rrq = new RRQ();
                        rrq.SetPointDistant(m_PointDistant);
                        rrq.SetFichier(NomFichier);
                        LeThread = new Thread(() => rrq.MonThreadRRQ());         //Pas certain de ce passage, trouvé sur stackoverflow
                        LeThread.Start();
                    }


                    //Sinon, si le code est 0002, soit un WRQ
                    else if (bTexte[1] == 2)
                    {
                        WRQ wrq = new WRQ();
                        wrq.SetPointDistant(m_PointDistant);
                        wrq.SetFichier(NomFichier);
                        LeThread = new Thread(() => wrq.MonThreadWRQ(m_LeSocket, bTexte, NomFichier));             //Pas certain de ce passage, trouvé sur stackoverflow
                        LeThread.Start();
                    }
                }
            }            

        }


        //Méthode qui valide la trame
        private int ValiderTrame(byte[] bTrame)
        {

            return 0;
        }

    }
}
