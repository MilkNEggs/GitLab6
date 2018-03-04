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
        private Socket LeSocket;
        private EndPoint PointLocal, PointDistant;


        //Méthode de la thread
        public void MaThreadEcoute()
        {
            //Déclaration des variables
            PointLocal = new IPEndPoint(0, 69);
            PointDistant = new IPEndPoint(0, 0);
            byte[] bTexte = new byte[516];
            int NbrRecu;
            Thread LeThread;

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

                    if (bTexte[1] == 49)
                    {
                        RRQ rrq = new RRQ();
                        LeThread = new Thread(new ThreadStart(rrq.MonThreadRRQ()));
                    }
                    else if (bTexte[1] == 2)
                    {
                        WRQ wrq = new WRQ();
                        LeThread = new Thread(new ThreadStart(wrq.MonThreadWRQ()));
                    }
                }
            }

            //EndPoint PointLocal = new IPEndPoint(...);
            //EndPoint PointDistant = new IPEndPoint(...);            

        }


        //Méthode qui valide la trame
        private int ValiderTrame(byte[] bTrame)
        {

            return 0;
        }

    }
}
