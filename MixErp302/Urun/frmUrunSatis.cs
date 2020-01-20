using MixErp302.Data;
using MixErp302.Fonksiyonlar;
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
    public partial class frmUrunSatis : Form
    {
        MixErpDbEntities db = new MixErpDbEntities();
        Formlar F = new Formlar();
        Numaralar N = new Numaralar();
        int SecimId = -1;
        bool edit = false;
        int UrnSatisId = -1;
        decimal bfyt, adet, korn;
        public string[] MyArray { get; set; }
        public frmUrunSatis()
        {
            InitializeComponent();
        }

        private void frmUrunSatis_Load(object sender, EventArgs e)
        {
            txtSatisGrupNo.Text = N.SatisGrupNo();
            Combo();
            txtKarOranı.SelectedIndex = 0;
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
            if (Liste.CurrentCell.ColumnIndex == 0 && txt != null)
            {
                txt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txt.AutoCompleteCustomSource.AddRange(MyArray);
            }
            else if (Liste.CurrentCell.ColumnIndex != 0 && txt != null)
            {
                txt.AutoCompleteMode = AutoCompleteMode.None;
            }
        }

        private void Liste_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string a = Liste.CurrentRow.Cells[0].Value.ToString();
                var lst = (from s in db.tblUrunlers
                           where s.UrunKodu == a
                           select s).First();
                Liste.CurrentRow.Cells[1].Value = lst.UrunAciklama;
                Liste.CurrentRow.Cells[2].Value = lst.bBirim.BirimAdi;
                Liste.CurrentRow.Cells[3].Value = db.tblStokDurums.First(x => x.UrunId == lst.Id).OBFiyat;
                Liste.CurrentRow.Cells[8].Value = db.tblStokDurums.First(x => x.UrunId == lst.Id).Raf;

                decimal bfyt = Convert.ToDecimal(Liste.CurrentRow.Cells[3].Value);
                int korn = Convert.ToInt32(txtKarOranı.Text);
                Liste.CurrentRow.Cells[4].Value = bfyt + (bfyt *korn )/100;
            }
            if (e.ColumnIndex == 5)
            {
                if (Liste.CurrentRow.Cells[4].Value != null)
                {
                    RowHesapla();
                }
                
            }
            if (e.ColumnIndex == 4)
            {
                if (Liste.CurrentRow.Cells[5].Value != null)
                {
                    RowHesapla();
                }
               
            }
        }
        private void RowHesapla()
        {
            if (Liste.CurrentRow.Cells[4].Value != null && Liste.CurrentRow.Cells[5].Value != null)
            {
                decimal birimfiyat, adet, karoran;

                birimfiyat = Convert.ToDecimal(Liste.CurrentRow.Cells[3].Value);
                karoran = Convert.ToDecimal(txtKarOranı.Text) / 100;
                adet = Convert.ToDecimal(Liste.CurrentRow.Cells[5].Value);

                Liste.CurrentRow.Cells[4].Value = birimfiyat + birimfiyat * karoran;
                Liste.CurrentRow.Cells[6].Value = Convert.ToDecimal(Liste.CurrentRow.Cells[4].Value) * adet * 0.18M;
                decimal aratoplam = 0;
                decimal kdvtoplam = 0;
                for (int i = 0; i < Liste.RowCount; i++)
                {
                    aratoplam += Convert.ToDecimal(Liste.Rows[i].Cells[4].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                    kdvtoplam += Convert.ToDecimal(Liste.Rows[i].Cells[6].Value);
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
        bool KayitKont = false;
        void YeniKaydet()
        {
            for (int i = 0; i < Liste.RowCount; i++)
            {
                if (Convert.ToInt32(Liste.Rows[i].Cells[5].Value) > Convert.ToInt32(Liste.Rows[i].Cells[8].Value))
                {
                    MessageBox.Show("Yeterli ürün adedi yoktur.Stok adedini kontrol ediniz.");
                    KayitKont = true;
                    Liste.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    Liste.Rows[i].Cells[5].Style.ForeColor = Color.White;
                }
                else
                {
                    Liste.Rows[i].Cells[5].Style.BackColor = SystemColors.Window;
                    Liste.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                }
            }
            if (KayitKont)
            {
                KayitKont = false;
                return;
            }
            var srch = new tblUrunSatisUst();
            srch.SatisGrupNo = txtSatisGrupNo.Text;
            srch.AraToplam = Convert.ToDecimal(txtAraToplam.Text);
            srch.STarih = Convert.ToDateTime(txtSatisTarihi.Text);
            srch.CariId = db.tblCaris.First(x => x.CariAdi == txtCari.Text).Id;
            srch.Vade = Convert.ToInt32(txtVade.Text);
            srch.OdemeId = db.bOdemeTurleris.First(x => x.OdemeTipi == txtOdemeTuru.Text).Id;
            srch.KdvToplam = Convert.ToDecimal(txtKdvToplam.Text);
            srch.GenelToplam = Convert.ToDecimal(txtGenelToplam.Text);
            srch.Durum = false;

            db.tblUrunSatisUsts.Add(srch);
            db.SaveChanges();

            Liste.AllowUserToAddRows = false;


            tblUrunSatisAlt[] ualt = new tblUrunSatisAlt[Liste.RowCount];
            for (int i = 0; i < Liste.RowCount; i++)
            {
                ualt[i] = new tblUrunSatisAlt();
                ualt[i].Miktar = Convert.ToInt32(Liste.Rows[i].Cells[5].Value.ToString());
                ualt[i].SatisGrupNo = txtSatisGrupNo.Text;
                ualt[i].BFiyat = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value.ToString());
                string brm = Liste.Rows[i].Cells[2].Value.ToString();
                ualt[i].BirimId = db.bBirims.First(x => x.BirimAdi == brm).Id;
                string urn = Liste.Rows[i].Cells[1].Value.ToString();
                ualt[i].UrunId = db.tblUrunlers.First(x => x.UrunAciklama == urn).Id;
                ualt[i].AToplam = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                ualt[i].Kdv = Convert.ToDecimal(Liste.Rows[i].Cells[6].Value);
                db.tblUrunSatisAlts.Add(ualt[i]);

                string uBarkod = Liste.Rows[i].Cells[0].Value.ToString() + "-" + Liste.Rows[i].Cells[1].Value.ToString();
                var stokKontrol = db.tblStokDurums.First(x => x.Barkod == uBarkod);
                stokKontrol.Ambar += 0;
                stokKontrol.Depo += Convert.ToInt32(Liste.Rows[i].Cells[5].Value.ToString());
                stokKontrol.Raf += Convert.ToInt32(Liste.Rows[i].Cells[5].Value.ToString());
            }
            db.SaveChanges();


            MessageBox.Show("Kayıt Başarılı.");
        }
        void Guncelle()
        {
            var srch = db.tblUrunSatisUsts.First(x => x.SatisGrupNo == txtSatisGrupNo.Text);
            srch.SatisGrupNo = txtSatisGrupNo.Text;
            srch.AraToplam = Convert.ToDecimal(txtAraToplam.Text);
            srch.STarih = Convert.ToDateTime(txtSatisTarihi.Text);
            srch.CariId = db.tblCaris.First(x => x.CariAdi == txtCari.Text).Id;
            srch.Vade = Convert.ToInt32(txtVade.Text);
            srch.OdemeId = db.bOdemeTurleris.First(x => x.OdemeTipi == txtOdemeTuru.Text).Id;
            srch.KdvToplam = Convert.ToDecimal(txtKdvToplam.Text);
            srch.GenelToplam = Convert.ToDecimal(txtGenelToplam.Text);

            db.SaveChanges();

            Liste.AllowUserToAddRows = false;


            tblUrunSatisAlt[] ualt = new tblUrunSatisAlt[Liste.RowCount];
            for (int i = 0; i < Liste.RowCount; i++)
            {
                var altId = Convert.ToInt32(Liste.Rows[i].Cells[7].Value);
                ualt[i] = db.tblUrunSatisAlts.First(x => x.SatisGrupNo == txtSatisGrupNo.Text && x.Id == altId);
                ualt[i].Miktar = Convert.ToInt32(Liste.Rows[i].Cells[5].Value.ToString());
                ualt[i].SatisGrupNo = txtSatisGrupNo.Text;
                ualt[i].BFiyat = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value.ToString());
                string brm = Liste.Rows[i].Cells[2].Value.ToString();
                ualt[i].BirimId = db.bBirims.First(x => x.BirimAdi == brm).Id;
                string urn = Liste.Rows[i].Cells[1].Value.ToString();
                ualt[i].UrunId = db.tblUrunlers.First(x => x.UrunAciklama == urn).Id;
                ualt[i].AToplam = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                ualt[i].Kdv = Convert.ToDecimal(Liste.Rows[i].Cells[6].Value);
            }
            db.SaveChanges();
            MessageBox.Show("Güncelleme Başarılı.");
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (edit && UrnSatisId > 0) Guncelle();
            else if (edit == false) YeniKaydet();
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            Close();
        }
        protected override void OnLoad(EventArgs e)
        {
            var btnUrunSatisNo = new Button();
            btnUrunSatisNo.Size = new Size(25, txtSatisGrupNo.ClientSize.Height + 2);
            btnUrunSatisNo.Location = new Point(txtSatisGrupNo.ClientSize.Width - btnUrunSatisNo.Width, -1);
            btnUrunSatisNo.Cursor = Cursors.Default;
            txtSatisGrupNo.Controls.Add(btnUrunSatisNo);
            btnUrunSatisNo.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            base.OnLoad(e);

            btnUrunSatisNo.Click += btnUrunSastisNo_Click;
        }

        private void btnUrunSastisNo_Click(object sender, EventArgs e)
        {
            int id = F.UrunSatisNo(true);
            if (id > 0)
            {
                Ac(id);
            }
            frmAnaSayfa.AktarmaInt = -1; new NotImplementedException();
        }



        private void Ac(int id)
        {
            edit = true;
            UrnSatisId = id;
            string ustNo = id.ToString().PadLeft(7, '0');
            tblUrunSatisUst ust = db.tblUrunSatisUsts.First(x => x.SatisGrupNo == ustNo);
            txtSatisGrupNo.Text = ust.SatisGrupNo;
            txtAraToplam.Text = ust.AraToplam.ToString();
            txtSatisTarihi.Text = ust.STarih.ToString();
            txtCari.Text = ust.tblCari.CariAdi;
            txtGenelToplam.Text = ust.GenelToplam.ToString();
            txtKdvToplam.Text = ust.KdvToplam.ToString();
            txtOdemeTuru.Text = ust.bOdemeTurleri.OdemeTipi;
            txtVade.Text = ust.Vade.ToString();

            Liste.Rows.Clear();
            Liste.AllowUserToAddRows = false;
            int i = 0;
            var alt = (from s in db.tblSatisAlisAlts
                       where s.SatisGrupNo == ustNo
                       select s).ToList();
            foreach (var k in alt)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = k.tblUrunler.UrunKodu;
                Liste.Rows[i].Cells[1].Value = k.tblUrunler.UrunAciklama;
                Liste.Rows[i].Cells[2].Value = k.bBirim.BirimAdi;
                Liste.Rows[i].Cells[3].Value = k.BFiyat;
                Liste.Rows[i].Cells[5].Value = k.Miktar;
                Liste.Rows[i].Cells[6].Value = k.Kdv;
                Liste.Rows[i].Cells[7].Value = k.Id;
                Liste.Rows[i].Cells[4].Value = k.SFiyat;
                i++;
            }
        }
        private void txtKarOranı_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Liste.RowCount > 1)
            {
                Liste.AllowUserToAddRows = false;
                decimal aratoplam = 0;
                decimal kdvtoplam = 0;
                decimal birimfiyat, adet, karoran;
                for (int i = 0; i < Liste.RowCount; i++)
                {
                    bfyt = Convert.ToDecimal(Liste.Rows[i].Cells[3].Value);
                    korn = Convert.ToDecimal(txtKarOranı.Text) / 100;
                    adet = Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                    

                    birimfiyat = Convert.ToDecimal(Liste.CurrentRow.Cells[3].Value);
                    karoran = Convert.ToDecimal(txtKarOranı.Text) / 100;
                    adet = Convert.ToDecimal(Liste.CurrentRow.Cells[5].Value);

                    Liste.Rows[i].Cells[4].Value = birimfiyat + birimfiyat * karoran;
                    Liste.Rows[i].Cells[6].Value = Convert.ToDecimal(Liste.CurrentRow.Cells[4].Value) * adet * 0.18M;

                    Liste.Rows[i].Cells[4].Value = bfyt + bfyt * korn;               
                        aratoplam += Convert.ToDecimal(Liste.Rows[i].Cells[4].Value) * Convert.ToDecimal(Liste.Rows[i].Cells[5].Value);
                        kdvtoplam += Convert.ToDecimal(Liste.Rows[i].Cells[6].Value);

                    txtAraToplam.Text = aratoplam.ToString();
                    txtKdvToplam.Text = kdvtoplam.ToString();
                    txtGenelToplam.Text = (aratoplam + kdvtoplam).ToString();

                }
                Liste.AllowUserToAddRows = true;
            }
        }
    }
}
