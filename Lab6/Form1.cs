using System;
using System.Windows.Forms;
using System.Threading;

namespace Lab6
{/*****
    Charles Gourde-Talbot et William Lafontaine
    Lab 6
*****/
    public partial class Form1 : Form
    {
        //Déclaration du thread et le serveur
        Thread t;
        ServeurEcoute serveur;
        
        public Form1()
        {
            InitializeComponent();
            //Crée la classe serveurecoute et autorise l'exécution de la boucle dans la classe
            serveur = new ServeurEcoute();
            //Empêche la boucle de serveur écoute
            btnArret.Enabled = false;
        }

        //Code du bouton démarrer
        private void btnDemarrer_Click(object sender, EventArgs e)
        {
            //Désactiver le bouton démarrer
            btnDemarrer.Enabled = false;
            //Instancier l'objet t pour démarrer la thread «MaThreadEcoute» de la classe ServeurEcoute
            t = new Thread(new ThreadStart(serveur.MaThreadEcoute));
            //autorise la boucle de serveur écoute
            serveur.m_Fin = false;
            //Démarrer la thread
            t.Start();
            btnArret.Enabled = true;
        }

        private void btnArret_Click(object sender, EventArgs e)
        {
            //Empêche la continuation de la boucle de serveur écoute
            serveur.m_Fin = true;
            btnDemarrer.Enabled = true;
            btnArret.Enabled = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serveur.m_Fin = true;
        }
        
    }
}
