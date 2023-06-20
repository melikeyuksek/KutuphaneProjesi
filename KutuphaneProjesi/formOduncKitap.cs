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
    public partial class formOduncKitap : Form
    {
        public formOduncKitap()
        {
            InitializeComponent();
        }
        VeriTabaniIslemleri vtIslemleri = new VeriTabaniIslemleri();//VeriTabaniIslemleri sınıfından bir nesne oluşturulur.
        MySqlConnection baglanti;//MySqlConnection sınıfından bir bağlantı nesnesi oluşturulur.
        MySqlCommand komut;//MySqlCommand sınıfından bir komut nesnesi oluşturulur.
        string komutSatiri;//Komutların yazılacağı sorgu ifadeleri bu değişken içerisinde tanımlanır.
        private void formOduncKitap_Load(object sender, EventArgs e)
        {
            Listele();
            KitapYukle();

        }
        public void temizle()
        {
            txtNo.Clear();
            txtAciklama.Clear();
        }
        public void KitapYukle()
        {
            try
            {
               
                komutSatiri = "SELECT * FROM kitaplar WHERE kitap_id NOT IN (SELECT kitap_id FROM odunc_kitaplar WHERE teslim_tarihi IS NULL)"; 
                //comboKitap nesnesinde veriler, ödünç verilip geri getirilmemiş kitaplar yer almayacak şekilde listelenir
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri, baglanti);//dataadapter nesnesi
                DataTable dataTable = new DataTable();//sorgudan dönen sonuçları DataTable nesnesine aktarılır
                dataAdapter.Fill(dataTable);//sorgu sonucunda dönen kayıtlar DataTable nesnesine aktarılır
                comboKitap.DataSource = dataTable;//DataTable nesnesindeki kayıtlar comboKitap da listelenir.
                comboKitap.ValueMember = "kitap_id";
                comboKitap.DisplayMember = "kitap_adi";
                // // comboKitap nesnesindeki her bir öğe için, değer olarak kitap_id, görüntü olarak da kitap_adi kullanılacak şekilde ayarlamalar yapılır



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Listele()
        {

            try
            {
                baglanti = vtIslemleri.baglan();//veri tabanı bağlantı nesnesi
                komutSatiri = "SELECT id, ogrenci_no, ad,soyad,kitap_adi, verilis_tarihi,teslim_tarihi,aciklama " +
                  "FROM kitaplar,ogrenciler,odunc_kitaplar " +
                  "WHERE ogr_no=ogrenci_no AND kitaplar.kitap_id=odunc_kitaplar.kitap_id";
                // Ödünç verilen kitapların ve öğrencilerin bilgilerini listeleyecek SQL sorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri, baglanti);//dataadapter nesnesi
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);//sorgu sonucunda dönen kayıtlar DataTable nesnesine aktarılır
                gridOduncKitaplar.DataSource = dataTable;//DataTable nesnesindeki kayıtlar DataGridView de listelenir.
                //DataGrid nesnesinde sütun başlıkları ve genişlikleri belirlenir
                gridOduncKitaplar.Columns["id"].HeaderText = "ID";
                gridOduncKitaplar.Columns["id"].Width = 30;
                gridOduncKitaplar.Columns["ogrenci_no"].HeaderText = "Öğrenci No";
                gridOduncKitaplar.Columns["ogrenci_no"].Width = 40;
                gridOduncKitaplar.Columns["ad"].HeaderText = "Ad";
                gridOduncKitaplar.Columns["ad"].Width = 70;
                gridOduncKitaplar.Columns["soyad"].HeaderText = "Soyad";
                gridOduncKitaplar.Columns["soyad"].Width = 70;
                gridOduncKitaplar.Columns["kitap_adi"].HeaderText = "Kitap Adı";
                gridOduncKitaplar.Columns["kitap_adi"].Width = 100;
                gridOduncKitaplar.Columns["verilis_tarihi"].HeaderText = "Veriliş Tarihi";
                gridOduncKitaplar.Columns["verilis_tarihi"].Width = 70;
                gridOduncKitaplar.Columns["teslim_tarihi"].HeaderText = "Teslim Tarihi";
                gridOduncKitaplar.Columns["teslim_tarihi"].Width = 70;
                gridOduncKitaplar.Columns["aciklama"].HeaderText = "Açıklama";
                gridOduncKitaplar.Columns["aciklama"].Width = 150;
                



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnKitapVer_Click(object sender, EventArgs e) //KİTAP VER
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) baglanti.Open();//veri tabanı bağlantısı açılır



                //veri tabanına eklenecek ödünç kitap kaydının SQL sorgusu
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                komut.CommandText = "INSERT INTO odunc_kitaplar (ogr_no,kitap_id,verilis_tarihi,aciklama)" +
                    "VALUES(@ogr_no,@kitap_id,@verilis_tarihi,@aciklama)";
                // @ogr_no parametresi, "txtNo" adlı metin kutusundan kullanıcının girdiği öğrenci numarasını alır ve tamsayıya dönüştürür.
                komut.Parameters.AddWithValue("@ogr_no", int.Parse(txtNo.Text)); 
                komut.Parameters.AddWithValue("@kitap_id", int.Parse(comboKitap.SelectedValue.ToString()));
                komut.Parameters.AddWithValue("@verilis_tarihi", DateTime.Now.ToString("yyyy/MM/dd"));
                komut.Parameters.AddWithValue("@aciklama", txtAciklama.Text);

                komut.ExecuteNonQuery();//ekleme sorgusu
                baglanti.Close();
                temizle();
                KitapYukle();
                Listele();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void gridOduncKitaplar_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
               
                txtAciklama.Text = gridOduncKitaplar.CurrentRow.Cells["aciklama"].Value.ToString();//seçili satırı datagridde gösterme
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSil_Click(object sender, EventArgs e) //SİL
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) baglanti.Open();
                // yeni bir MySqlCommand nesnesi oluşturuluyor ve bağlantı bilgisi veriliyor
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                komut.CommandText = "DELETE FROM odunc_kitaplar WHERE id = @id";//@id yerine parametre gelecek
                komut.Parameters.AddWithValue("@id", gridOduncKitaplar.CurrentRow.Cells["id"].Value.ToString());
                // sorguya parametre olarak datagridview de seçili olan id bilgisi gönderildi
                komut.ExecuteNonQuery();// sorgu çalıştırılıyor
                baglanti.Close();
                temizle();
                KitapYukle();
                Listele();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKitapAl_Click(object sender, EventArgs e) //KİTAP AL
        {
            try
            {
                if (gridOduncKitaplar.CurrentRow.Cells["teslim_tarihi"].Value.ToString() != String.Empty)
                //teslim tarihi boş değilse yani kitap teslim alınmışsa işlem yapmadan click olayından çıkılır
                {
                    MessageBox.Show("Bu kitap teslim alınmış", "uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
                if (baglanti.State != ConnectionState.Open) baglanti.Open();
                // Kitap teslim alındığında güncellenecek alanlar belirtilir
                komutSatiri = "UPDATE odunc_kitaplar SET teslim_tarihi=@teslim_tarihi,aciklama=@aciklama WHERE id=@id";
                komut = new MySqlCommand(komutSatiri, baglanti);
                //Seçili satırın "id" değerini sorguya parametre olarak ekler
                komut.Parameters.AddWithValue("@id", int.Parse(gridOduncKitaplar.CurrentRow.Cells["id"].Value.ToString()));
                // Kitabın teslim tarihi olarak günün tarihini sorguya parametre olarak ekler
                komut.Parameters.AddWithValue("@teslim_tarihi", DateTime.Now.ToString("yyyy/MM/dd"));
                // Kitabın teslimiyle ilgili açıklama metnini sorguya parametre olarak ekler
                komut.Parameters.AddWithValue("@aciklama", txtAciklama.Text);
                komut.ExecuteNonQuery();
                temizle();
                KitapYukle();
                Listele();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAramaOgrenci_TextChanged(object sender, EventArgs e)
        {
            OduncKitapOgrenciArama(txtAramaOgrenci.Text);
        }
        public void OduncKitapOgrenciArama(string aranacakKelime)
        {

            try
            {
                if (baglanti.State != ConnectionState.Open)  baglanti.Open();
                //Öğrenci adına göre arama yapılacak şekilde sorgu oluşturulur
                komutSatiri = "SELECT id, ogrenci_no,ad,soyad,kitap_adi,verilis_tarihi,teslim_tarihi,aciklama " +
                    "FROM kitaplar, ogrenciler, odunc_kitaplar " +
                    "WHERE ogr_no=ogrenci_no and kitaplar.kitap_id=odunc_kitaplar.kitap_id and ad LIKE '" + aranacakKelime + "%'";//aramasorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri,baglanti);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable); //sorgudan dönen değer datatable nesnesine doldurulur
               
                gridOduncKitaplar.DataSource = dataTable; //datatable nesnesine aktarılan veriler datagridview de listelenir

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
