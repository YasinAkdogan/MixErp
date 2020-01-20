using MixErp302.Data;
using MixErp302.Fonksiyonlar;
using MixErp302.Print;
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
    public partial class frmUrunAlis : Form
    {
        Formlar F = new Formlar();
        MixErpDbEntities db = new MixErpDbEntities();
        Numaralar N = new Numaralar();
        int secimId = -1;
        bool edit = false;
        int UrnAlisId = -1;
        public string[] MyArray { get; set; }
        public frmUrunAlis()
        {
            InitializeComponent();
        }

        private void frmUrunAlis_Load(object sender, EventArgs e)
        {
            txtAlisGrupNo.Text = N.AlisGrupNo();
            Combo();
        }

        private void Combo()
        {
            txtCari.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtCari.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            AutoCompleteStringCollection veri = new AutoCompleteStringCollection();
            var lst = db.tblCaris.Select(x => x.CariAdi).Distinct();
            foreach (var cari in lst)
            {
                veri.Add(cari);
                txtCari.Items.Add(cari);
            }
            txtCari.AutoCompleteCustomSource = veri;
            txtVade.SelectedIndex = 0;

            txtOdemeTuru.DataSource = db.bOdemeTurleris.ToList();
            txtOdemeTuru.ValueMember = "Id";
            txtOdemeTuru.DisplayMember = "OdemeTipi";


            var srg = db.tblUrunlers.Select(x => x.UrunKodu);
            foreach (var k in srg)
            {
                txtUKod.Items.Add(k);
            }
            int dgv;
            dgv = txtUKod.Items.Count;
            MyArray = new string[dgv];
            for (int i = 0; i < dgv; i++)
            {
                MyArray[i] = txtUKod.Items[i].ToString();
            }
        }
        private void Liste_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox txt = e.Control as TextBox;
            if (Liste.CurrentCell.ColumnIndex==0 && txt !=null)
            {
                txt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txt.AutoCompleteCustomSource.AddRange(MyArray);
            }
            else if (Liste.CurrentCell.ColumnIndex!=0 && txt !=null)
            {
                txt.AutoCompleteMode = AutoCompleteMode.None;
            }
        }

        private void Liste_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex==0)
            {
                string a = Liste.CurrentRow.Cells[0].Value.ToString();
                var lst = (from s in db.tblUrunlers
                           where s.UrunKodu ==a
                           select s).First();
                Liste.CurrentRow.Cells[1].Value = lst.UrunAciklama;
                Liste.CurrentRow.Cells[2].Value = lst.bBirim.BirimAdi;
            }
            if (e.ColumnIndex==4)
            {
                if (Liste.CurrentRow.Cells[3].Value!=null)
                {
                    RowHesapla(); 
                }
            }
            if (e.ColumnIndex==3)
            {
                if (Liste.CurrentRow.Cells[4].Value!=null)
                {
                    RowHesapla(); 
                }
            }
        }

        private void RowHesapla()
        {
            if (Liste.CurrentRow.Cells[3].Value != null && Liste.CurrentRow.Cells[4].Value != null)
            {
                decimal a, b;
                a = Convert.ToDecimal(Liste.CurrentRow.Cells[3].Value);
                b = Convert.ToDecimal(Liste.CurrentRow.Cells[4].Value);
                Liste.CurrentRow.Cells[5].Value = a * b * 0.18M; 
                decimal aratoplam = 0;
                decimal kdvtoplam = 0;
                for (int i = 0; i < Liste.RowCount; i++)
                {
                    aratoplam += Convert.ToDecimal(Liste.Rows[i].Cells[3].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[4].Value);
                    kdvtoplam += Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                }

                txtAraToplam.Text = aratoplam.ToString();
                txtKdvToplam.Text = kdvtoplam.ToString();
                txtGenelToplam.Text = (aratoplam + kdvtoplam).ToString();
            }
            else
            {
                MessageBox.Show("Seçili alanı boş bırakmayın.");
                Liste.CurrentRow.Cells[5].Value = "";
            }
        }

        void YeniKaydet()
        {
            var srch = new tblUrunAlisUst();
            srch.AlisGrupNo = txtAlisGrupNo.Text;
            srch.AraToplam =Convert.ToDecimal(txtAraToplam.Text);
            srch.ATarih = Convert.ToDateTime(txtAlisTarihi.Text);
            srch.FaturaNo = txtFaturaNo.Text;
            srch.CariId = db.tblCaris.First(x => x.CariAdi == txtCari.Text).Id;
            srch.Vade = Convert.ToInt32(txtVade.Text);
            srch.OdemeId = db.bOdemeTurleris.First(x => x.OdemeTipi == txtOdemeTuru.Text).Id;
            srch.KdvToplam = Convert.ToDecimal(txtKdvToplam.Text);
            srch.GenelToplam = Convert.ToDecimal(txtGenelToplam.Text);

            db.tblUrunAlisUsts.Add(srch);
            db.SaveChanges();

            Liste.AllowUserToAddRows = false;


            tblUrunAlisAlt[] ualt = new tblUrunAlisAlt[Liste.RowCount];
            for (int i = 0; i < Liste.RowCount; i++)
            {
                ualt[i] = new tblUrunAlisAlt();
                ualt[i].Miktar = Convert.ToInt32(Liste.Rows[i].Cells[4].Value.ToString());
                ualt[i].AlisGrupNo = txtAlisGrupNo.Text;
                ualt[i].BFiyat = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value.ToString());
                string brm = Liste.Rows[i].Cells[2].Value.ToString();
                ualt[i].BirimId = db.bBirims.First(x => x.BirimAdi == brm).Id;      
                string urn = Liste.Rows[i].Cells[1].Value.ToString();
                ualt[i].UrunId = db.tblUrunlers.First(x => x.UrunAciklama == urn).Id;
                ualt[i].AToplam = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[4].Value);
                ualt[i].Kdv = Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                db.tblUrunAlisAlts.Add(ualt[i]);

                string uBarkod = Liste.Rows[i].Cells[0].Value.ToString() +"-"+ Liste.Rows[i].Cells[1].Value.ToString();
                var stokKontrol = db.tblStokDurums.First(x => x.Barkod == uBarkod);
               

                decimal obFiyat,YTopObFiyat,ETopObFiyat;
                if (stokKontrol.OBFiyat == null)
                {
                    obFiyat = 0;
                    ETopObFiyat = 0;
                }
                else
                {
                    obFiyat = stokKontrol.OBFiyat.Value;
                    ETopObFiyat = obFiyat * stokKontrol.Depo.Value;
                }
               
                YTopObFiyat = Convert.ToInt32(Liste.Rows[i].Cells[3].Value.ToString()) * Convert.ToInt32(Liste.Rows[i].Cells[4].Value.ToString());

                decimal TopEyfiyat = ETopObFiyat + YTopObFiyat;
                int EAdet = stokKontrol.Depo.Value;
                int YAdet = Convert.ToInt32(Liste.Rows[i].Cells[4].Value.ToString());
                int TopEYAdet = EAdet + YAdet;
                decimal SonucBirimFiyat = TopEyfiyat / TopEYAdet;

                stokKontrol.Ambar += 0;
                stokKontrol.Depo += Convert.ToInt32(Liste.Rows[i].Cells[4].Value.ToString());
                stokKontrol.Raf += Convert.ToInt32(Liste.Rows[i].Cells[4].Value.ToString());

                stokKontrol.OBFiyat = SonucBirimFiyat;
            }
            db.SaveChanges();

            
            MessageBox.Show("Kayıt Başarılı.");
        }
        void Guncelle()
        {
            var srch = db.tblUrunAlisUsts.First(x => x.AlisGrupNo == txtAlisGrupNo.Text);
            srch.AlisGrupNo = txtAlisGrupNo.Text;
            srch.AraToplam = Convert.ToDecimal(txtAraToplam.Text);
            srch.ATarih = Convert.ToDateTime(txtAlisTarihi.Text);
            srch.FaturaNo = txtFaturaNo.Text;
            srch.CariId = db.tblCaris.First(x => x.CariAdi == txtCari.Text).Id;
            srch.Vade = Convert.ToInt32(txtVade.Text);
            srch.OdemeId = db.bOdemeTurleris.First(x => x.OdemeTipi == txtOdemeTuru.Text).Id;
            srch.KdvToplam = Convert.ToDecimal(txtKdvToplam.Text);
            srch.GenelToplam = Convert.ToDecimal(txtGenelToplam.Text);

            db.SaveChanges();

            Liste.AllowUserToAddRows = false;


            tblUrunAlisAlt[] ualt = new tblUrunAlisAlt[Liste.RowCount];
            for (int i = 0; i < Liste.RowCount; i++)
            {
                var altId = Convert.ToInt32(Liste.Rows[i].Cells[6].Value);
                ualt[i] = db.tblUrunAlisAlts.First(x => x.AlisGrupNo == txtAlisGrupNo.Text && x.Id==altId);
                ualt[i].Miktar = Convert.ToInt32(Liste.Rows[i].Cells[4].Value.ToString());
                ualt[i].AlisGrupNo = txtAlisGrupNo.Text;
                ualt[i].BFiyat = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value.ToString());
                string brm = Liste.Rows[i].Cells[2].Value.ToString();
                ualt[i].BirimId = db.bBirims.First(x => x.BirimAdi == brm).Id;
                string urn = Liste.Rows[i].Cells[1].Value.ToString();
                ualt[i].UrunId = db.tblUrunlers.First(x => x.UrunAciklama == urn).Id;
                ualt[i].AToplam = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[4].Value);
                ualt[i].Kdv = Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
            }
            db.SaveChanges();
            MessageBox.Show("Güncelleme Başarılı.");
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (edit && UrnAlisId > 0) Guncelle();
            else if(edit==false)  YeniKaydet();
        }
        protected override void OnLoad(EventArgs e)
        {
            var btnUrunAlisNo = new Button();
            btnUrunAlisNo.Size = new Size(25, txtAlisGrupNo.ClientSize.Height + 2);
            btnUrunAlisNo.Location = new Point(txtAlisGrupNo.ClientSize.Width - btnUrunAlisNo.Width, -1);
            btnUrunAlisNo.Cursor = Cursors.Default;
            txtAlisGrupNo.Controls.Add(btnUrunAlisNo);
            btnUrunAlisNo.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            base.OnLoad(e);

            btnUrunAlisNo.Click += btnUrunAlisNo_Click;
        }

        private void btnUrunAlisNo_Click(object sender, EventArgs e)
        {
            int id = F.UrunAlisNo(true);
            if (id>0)
            {
                Ac(id);
            }
            frmAnaSayfa.AktarmaInt = -1;
        }

        private void Ac(int id)
        {
            edit = true;
            UrnAlisId = id;
            string ustNo = id.ToString().PadLeft(7, '0');
            tblUrunAlisUst ust = db.tblUrunAlisUsts.First(x => x.AlisGrupNo == ustNo);
            txtAlisGrupNo.Text = ust.AlisGrupNo;
            txtAraToplam.Text = ust.AraToplam.ToString();
            txtAlisTarihi.Text = ust.ATarih.ToString();
            txtCari.Text = ust.tblCari.CariAdi;
            txtFaturaNo.Text = ust.FaturaNo;
            txtGenelToplam.Text = ust.GenelToplam.ToString();
            txtKdvToplam.Text = ust.KdvToplam.ToString();
            txtOdemeTuru.Text = ust.bOdemeTurleri.OdemeTipi;
            txtVade.Text = ust.Vade.ToString();

            Liste.Rows.Clear();
            Liste.AllowUserToAddRows = false;
            int i = 0;
            var alt = (from s in db.tblUrunAlisAlts
                       where s.AlisGrupNo == ustNo
                       select s).ToList();
            foreach (var k in alt)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = k.tblUrunler.UrunKodu;
                Liste.Rows[i].Cells[1].Value = k.tblUrunler.UrunAciklama;
                Liste.Rows[i].Cells[2].Value = k.bBirim.BirimAdi;
                Liste.Rows[i].Cells[3].Value = k.BFiyat;
                Liste.Rows[i].Cells[4].Value = k.Miktar;
                Liste.Rows[i].Cells[5].Value = k.Kdv;
                Liste.Rows[i].Cells[6].Value = k.Id;
                i++;
            }
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Yaz();
        }

        private void Yaz()
        {
            frmPrint pr = new frmPrint();
            pr.GrupNo = txtAlisGrupNo.Text;
            pr.HangiListe = "UrunAlis";
            pr.Show();
        }
    }
}
