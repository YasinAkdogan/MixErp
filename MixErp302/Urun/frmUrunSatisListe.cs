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

namespace MixErp302.Urun
{
    public partial class frmUrunSatisListe : Form
    {
        MixErpDbEntities db = new MixErpDbEntities();
        public bool Secim = false;
        public frmUrunSatisListe()
        {
            InitializeComponent();
        }

        private void frmUrunSatisListe_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            Liste.Rows.Clear();
            int i = 0;
            var alt = (from s in db.tblStokDurums
                       where s.tblUrunler.UrunKodu.Contains(txtBul.Text)
                       select s).ToList();
            foreach (var k in alt)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = k.Id;
                Liste.Rows[i].Cells[1].Value = k.StokKodu;
                Liste.Rows[i].Cells[2].Value = k.tblUrunler.UrunKodu; ;
                Liste.Rows[i].Cells[3].Value = k.tblUrunler.UrunAciklama;
                Liste.Rows[i].Cells[4].Value = k.Depo;
                Liste.Rows[i].Cells[5].Value = k.Raf;
                Liste.Rows[i].Cells[6].Value = k.Ambar;
                Liste.Rows[i].Cells[7].Value = k.Barkod;
                i++;
            }
            Liste.AllowUserToAddRows = false;
        }
    }
}
