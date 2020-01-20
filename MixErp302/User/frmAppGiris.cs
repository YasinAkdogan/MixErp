using MixErp302.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MixErp302.User
{
    public partial class frmAppGiris : Form
    {
        MixErpDbEntities db = new MixErpDbEntities();
        public frmAppGiris()
        {
            InitializeComponent();
        }
        void GirisKontrol()
        {
            try
            {
                int srg = (from s in db.tblUsers
                           where s.KulAdi == txtKulAdi.Text && s.Sifre == txtSifre.Text
                           select s).First().Id;
                if (srg > 0)
                {
                    frmAnaSayfa ana = new frmAnaSayfa();
                    ana.WindowState = FormWindowState.Maximized;
                    ana.RoleId = db.tblUsers.Find(srg).Role.Value;
                    ana.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kullanıcı Adı veya şifre yanlış.");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Kullanıcı Adı veya şifre yanlış.");
            }
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            GirisKontrol();
        }
    }
}
