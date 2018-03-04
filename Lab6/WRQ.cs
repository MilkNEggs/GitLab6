using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Lab6
{
    class WRQ
    {
        //Déclaration des variables
        EndPoint m_PointDistantWRQ;
        string m_StrFichierWRQ;

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
            
        }

    }
}
