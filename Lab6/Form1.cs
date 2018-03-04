using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Lab6
{/*****
    Charles Gourde-Talbot et William Lafontaine
    Lab 6
*****/
    public partial class Form1 : Form
    {
        Thread t;
        ServeurEcoute serveur;
        
        public Form1()
        {
            InitializeComponent();
            serveur = new ServeurEcoute();
            btnArret.Enabled = false;
        }

        //Code du bouton démarrer
        private void btnDemarrer_Click(object sender, EventArgs e)
        {
            //Désactiver le bouton démarrer
            btnDemarrer.Enabled = false;

            //Instancier l'objet t pour démarrer la thread «MaThreadEcoute» de la classe ServeurEcoute
            t = new Thread(new ThreadStart(serveur.MaThreadEcoute));
            serveur.m_Fin = false;

            //Démarrer la thread
            t.Start();
            btnArret.Enabled = true;
        }

        private void btnArret_Click(object sender, EventArgs e)
        {
            serveur.m_Fin = true;
            //Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serveur.m_Fin = true;
        }
        
    }
}
