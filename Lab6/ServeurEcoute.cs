using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace Lab6
{
    class ServeurEcoute
    {
        //Déclaration des variables memebres
        public bool m_Fin;


        //Méthode de la thread
        public void MaThreadEcoute()
        {
            EndPoint PointLocal = new IPEndPoint(...);
            EndPoint PointDistant = new IPEndPoint(...);
            
        }


        //Méthode qui valide la trame
        private int ValiderTrame(byte[] bTrame)
        {

            return 0;
        }

    }
}
