//William Lafontaine
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
    class RRQ
    {
        EndPoint m_PointDistantRRQ;
        string m_strFichierRRQ;

        public RRQ()
        {
            
        }

        public void SetPointDistant(EndPoint PointDistant)
        {
            m_PointDistantRRQ = PointDistant;
        }

        public void SetFichier(string NomFichier)
        {
            m_strFichierRRQ = NomFichier;
        }

        public void MonThreadRRQ()
        {
            //À Compléter ☺ 
        }        
    }
}
