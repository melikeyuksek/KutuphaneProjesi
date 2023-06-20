using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KutuphaneProjesi
{
    public partial class formKitap : Form
    {
        public formKitap()
        {
            InitializeComponent();
        }

        VeriTabaniIslemleri vtIslemleri = new VeriTabaniIslemleri();//veri tabanı işlemleri sınıfından bir nesne örneği oluşturuldu
        MySqlConnection baglanti;
        MySqlCommand komut;
        string komutSatiri;
        private void formKitap_Load(object sender, EventArgs e)
        {
            KitapTurYukle();//metod form yüklendiğinde çağrılır
            KitapListele();//metod form yüklendiğinde çağrılır
        }
        public void Temizle()
        {
            txtKitapAdi.Clear();
            txtSayfaSayisi.Clear();
            txtYayinEvi.Clear();
            txtYazar.Clear();
        }
        public void KitapTurYukle()
        {
            try
            {

                baglanti = vtIslemleri.baglan();//veri tabanı bağlantı nesnesi
                komutSatiri = "Select * From kitap_turleri"; //verileri listeleyecek sql sorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri, baglanti);//dataadapter nesnesi
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);//sorgu sonucunda dönen kayıtlar DataTable nesnesine aktarılır
                comboKitapTur.DataSource = dataTable; //combobox nesnesinin veri kaynağı 
                comboKitapTur.ValueMember = "tur_id";//arka planda tutulup veri tabanına kaydedilecek alan
                comboKitapTur.DisplayMember = "tur_adi";//kullancıya gösterilecek alan


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void KitapListele()
        {
            try
            {

                baglanti = vtIslemleri.baglan();//veri tabanı bağlantı nesnesi
                komutSatiri = "Select kitap_id,tur_adi,kitap_adi,yazar,yayinevi,sayfa_sayisi From kitaplar,kitap_turleri where kitaplar.tur_id=kitap_turleri.tur_id";

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri, baglanti);//dataadapter nesnesi
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);//sorgu sonucunda dönen kayıtlar DataTable nesnesine aktarılır

                gridKitap.DataSource = dataTable; //DataTable nesnesindeki kayıtlar DataGridView de listelenir.

                gridKitap.Columns["kitap_id"].HeaderText = "ID";//DataGrid nesnesinde sütun başlıkları belirlenir
                gridKitap.Columns["kitap_id"].Width = 20;
                gridKitap.Columns["tur_adi"].HeaderText = "Tür";
                gridKitap.Columns["tur_adi"].Width = 30;
                gridKitap.Columns["kitap_adi"].HeaderText = "Adı";
                gridKitap.Columns["kitap_adi"].Width = 90;
                gridKitap.Columns["yazar"].HeaderText = "Yazar";
                gridKitap.Columns["yazar"].Width = 80;
                gridKitap.Columns["yayinevi"].HeaderText = "Yayınevi";
                gridKitap.Columns["yayinevi"].Width = 80;
                gridKitap.Columns["sayfa_sayisi"].HeaderText = "Sayfa Sayısı";
                gridKitap.Columns["sayfa_sayisi"].Width = 50;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) //bağlantı durumu kontrol edilir
                {
                    baglanti.Open();//açık değilse açılır
                }
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                komut.CommandText = "INSERT INTO kitaplar (tur_id, kitap_adi,yazar,yayinevi,sayfa_sayisi) " +
                    "VALUES(@tur_id,@kitap_adi,@yazar,@yayinevi,@sayfa_sayisi)";
                //alttaki satırda combobox nesnesinin arka planda tuttuğu id değeri tur_id olarak kayıt edilir
                komut.Parameters.AddWithValue("@tur_id", int.Parse(comboKitapTur.SelectedValue.ToString()));
                komut.Parameters.AddWithValue("@kitap_adi", txtKitapAdi.Text);//sorguda verilen parametrelerin değerleri belirlendi
                komut.Parameters.AddWithValue("@yazar", txtYazar.Text);
                komut.Parameters.AddWithValue("@yayinevi", txtYayinEvi.Text);
                komut.Parameters.AddWithValue("@sayfa_sayisi", int.Parse(txtSayfaSayisi.Text));

                komut.ExecuteNonQuery();//ekleme sorgusu
                baglanti.Close();
                Temizle();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                KitapListele(); //eklenenin datagridview de görünmesi için 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridKitap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtKitapAdi.Text = gridKitap.CurrentRow.Cells["kitap_adi"].Value.ToString();//seçili satırı datagridde gösterme
                txtSayfaSayisi.Text = gridKitap.CurrentRow.Cells["sayfa_sayisi"].Value.ToString();
                txtYayinEvi.Text = gridKitap.CurrentRow.Cells["yayinevi"].Value.ToString();
                txtYazar.Text = gridKitap.CurrentRow.Cells["yazar"].Value.ToString();
                comboKitapTur.SelectedItem = gridKitap.CurrentRow.Cells["tur_adi"].Value.ToString();
                
            }
            catch (Exception)
            {
                MessageBox.Show("Hata oluştu", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)// bağlantı kontrolü
                {
                    baglanti.Open();
                }
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                komut.CommandText = "DELETE FROM kitaplar WHERE kitap_id =@kitap_id";
                komut.Parameters.AddWithValue("@kitap_id", gridKitap.CurrentRow.Cells["kitap_id"].Value.ToString());
                // sorguya parametre olarak datagridview de seçili olan kitap_id bilgisi gönderildi
                komut.ExecuteNonQuery();// sorgu çalıştırılır ve hata oluşmazsa  silinir
                baglanti.Close();
                Temizle();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                KitapListele();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)//GÜNCELLEME
        {
            try

            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open();
                }
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                komut.CommandText = "UPDATE kitaplar SET tur_id=@tur_id, kitap_adi=@kitap_adi,"+
                    "yazar=@yazar, yayinevi=@yayinevi, sayfa_sayisi=@sayfa_sayisi where kitap_id=@kitap_id";
                //sorgudaki parametrelere değerler atanır
                komut.Parameters.AddWithValue("@kitap_id", int.Parse(gridKitap.CurrentRow.Cells["kitap_id"].Value.ToString()));
                // kitap_id parametresine gridKitap'taki seçili satırdaki kitap_id değeri atanır
                komut.Parameters.AddWithValue("@tur_id", int.Parse(comboKitapTur.SelectedValue.ToString()));
                // tur_id parametresine comboKitapTur'daki seçili değer atanır
                komut.Parameters.AddWithValue("@kitap_adi", txtKitapAdi.Text);
                komut.Parameters.AddWithValue("@yazar", txtYazar.Text);
                komut.Parameters.AddWithValue("@yayinevi", txtYayinEvi.Text);
                komut.Parameters.AddWithValue("@sayfa_sayisi", int.Parse(txtSayfaSayisi.Text));

                komut.ExecuteNonQuery();// sorgu çalıştırılır
                baglanti.Close();
                Temizle();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                KitapListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtKitapAra_TextChanged(object sender, EventArgs e) //KİTAP ARAMA
        {
            KitapArama(txtKitapAra.Text);
        }
        public void KitapArama(string aranacakKelime)
        {

            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open();
                }
               
                
                komutSatiri = "SELECT kitap_id, tur_adi, kitap_adi, yazar, yayinevi, sayfa_sayisi FROM kitaplar, kitap_turleri " +
                    "WHERE kitaplar.tur_id=kitap_turleri.tur_id AND  kitap_adi LIKE '" + aranacakKelime + "%'";//aramasorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri,baglanti);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable); //sorgudan dönen değer datatable nesnesine doldurulur
                baglanti.Close();
                gridKitap.DataSource = dataTable; //datatable nesnesine aktarılan veriler datagridview de listelenir

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
