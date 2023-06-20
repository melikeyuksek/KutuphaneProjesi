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
    public partial class formOgrenciii : Form
    {
        VeriTabaniIslemleri vtIslemleri = new VeriTabaniIslemleri(); //veri tabanı işlemleri sınıfından bir nesne örneği oluşturuldu
        MySqlConnection baglanti;//MySqlConnection sınıfından bir bağlantı nesnesi oluşturulur.
        MySqlCommand komut;//MySqlCommand sınıfından bir komut nesnesi oluşturulur.
        string komutSatiri;//Komutların yazılacağı sorgu ifadeleri bu değişken içerisinde tanımlanır.
        public formOgrenciii()
        {
            InitializeComponent();
        }

        private void formOgrenciii_Load_1(object sender, EventArgs e)
        {
            Listele(); //metod form yüklendiğinde çağrılır
        }
        public void Temizle() //formun içeriğinin temizlenmesi
        {
            txtAd.Clear();
            txtSoyad.Clear();
            txtNo.Clear();
            txtTelefon.Clear();
        }

        public void Listele()
        {
            try
            {
                baglanti = vtIslemleri.baglan();//veri tabanı bağlantı nesnesi
                komutSatiri = "SELECT * FROM ogrenciler"; //verileri listeleyecek sql sorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri, baglanti);//DataAdapter nesnesi oluşturuldu
                //bir DataTable nesnesi oluşturur ve verilerin bu nesneye aktarılması için kullanılır.
                //Daha sonra veritabanından sorgudan dönen sonuçlar bu nesneye aktarılır ve DataGridView'de görüntülenir.
                DataTable dataTable = new DataTable(); 
                dataAdapter.Fill(dataTable);//sorgu sonucunda dönen kayıtlar DataTable nesnesine aktarılır.
                gridOgrenci.DataSource = dataTable;//DataTable nesnesindeki kayıtlar DataGridView de listelenir.
                //DataGrid nesnesinde sütun başlıkları belirlenir
                gridOgrenci.Columns["ogrenci_no"].HeaderText = "Öğrenci Numarası";
                gridOgrenci.Columns["ad"].HeaderText = "Ad";
                gridOgrenci.Columns["soyad"].HeaderText = "Soyad";
                gridOgrenci.Columns["sinif"].HeaderText = "Sınıf";
                gridOgrenci.Columns["cinsiyet"].HeaderText = "Cinsiyet";
                gridOgrenci.Columns["telefon"].HeaderText = "Telefon";



            }
            catch (Exception ex) //hata mesajı
            {
                MessageBox.Show(ex.Message, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        
        private void btnKaydet_Click(object sender, EventArgs e) // KAYDET
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) //bağlantının durumu kontrol edildi
                {
                    baglanti.Open();// bağlantı açık değilse açılır
                }
                komutSatiri = "INSERT INTO ogrenciler (ogrenci_no, ad, soyad, sinif,cinsiyet,telefon) "+
                    "VALUES (@no,@ad,@soyad,@sinif,@cinsiyet,@telefon)";
                komut = new MySqlCommand(komutSatiri, baglanti);//komut çalıştırmak için mysqlcommand nesnesi oluşturuldu
                komut.Parameters.AddWithValue("@no", int.Parse(txtNo.Text));//sorguda verilen parametrelerin değerleri belirlendi
                komut.Parameters.AddWithValue("@ad", txtAd.Text);
                komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                komut.Parameters.AddWithValue("@sinif", int.Parse(comboSinif.SelectedItem.ToString()));
                komut.Parameters.AddWithValue("@cinsiyet", comboCinsiyet.SelectedItem.ToString());
                komut.Parameters.AddWithValue("@telefon", txtTelefon.Text);

                komut.ExecuteNonQuery(); //ekleme sorgusu çalıştırıldı ve hata oluşmazsa öğrenci eklenir
                baglanti.Close(); //bağlantı kapatılır
                Temizle(); //formun içeriği temizlenir 
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Listele();//eklenen öğrencinin DataGridView de görülebilmesi için veriler tekrar listelendi.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }



        private void gridOgrenci_CellContentClick(object sender, DataGridViewCellEventArgs e) //DATAGRİD FORM ELAMANINA YAZDIRMA
        {
            try
            {   //DataGridView de seçili olan öğrenciye ait bilgiler form elemanına yazdırılır
                txtNo.Text = gridOgrenci.CurrentRow.Cells["ogrenci_no"].Value.ToString();//seçili satırı datagridde gösterme
                txtAd.Text = gridOgrenci.CurrentRow.Cells["ad"].Value.ToString();
                txtSoyad.Text = gridOgrenci.CurrentRow.Cells["soyad"].Value.ToString();
                txtTelefon.Text = gridOgrenci.CurrentRow.Cells["telefon"].Value.ToString();
                comboSinif.SelectedItem = gridOgrenci.CurrentRow.Cells["sinif"].Value.ToString();
                comboCinsiyet.SelectedItem = gridOgrenci.CurrentRow.Cells["cinsiyet"].Value.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Hata oluştu", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSil_Click(object sender, EventArgs e) //SİL
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) //bağlantı durumu kontrol edildi
                {
                    baglanti.Open(); //eğer açık değilse bağlantı açılır

                }
                komutSatiri = "DELETE FROM ogrenciler WHERE ogrenci_no =@no"; //@no yerine parametre gelecek
                komut = new MySqlCommand(komutSatiri, baglanti);
                komut.Parameters.AddWithValue("@no", gridOgrenci.CurrentRow.Cells["ogrenci_no"].Value.ToString());
                // sorguya parametre olarak datagridview de seçili olan ogrenci_no bilgisi gönderildi
                komut.ExecuteNonQuery();// sorgu çalıştırılır ve hata oluşmazsa öğrenci silinir
                baglanti.Close();//baglanti kapatılır
                Temizle();
                MessageBox.Show("İşlem Başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Listele(); //silinen kayıtın görünmemesi için tekrar listeleme yapıldı

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e) //GÜNCELLE
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) 
                {
                    baglanti.Open();


                }
                komutSatiri = "UPDATE ogrenciler SET ad=@ad,soyad=@soyad,sinif=@sinif,cinsiyet=@cinsiyet,telefon=@telefon where ogrenci_no=@no";
                komut = new MySqlCommand(komutSatiri, baglanti); //komut çalıştırmak için mysqlcommand nesnesi oluşturuldu
                komut.Parameters.AddWithValue("@no", int.Parse(gridOgrenci.CurrentRow.Cells["ogrenci_no"].Value.ToString()));
                komut.Parameters.AddWithValue("@ad", txtAd.Text);//sorgudaki parametrelerin değerlerinin belirlenmesi
                komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                komut.Parameters.AddWithValue("@sinif", int.Parse(comboSinif.SelectedItem.ToString()));
                komut.Parameters.AddWithValue("@cinsiyet", comboCinsiyet.SelectedItem.ToString());
                komut.Parameters.AddWithValue("@telefon", txtTelefon.Text);
                komut.ExecuteNonQuery(); // sorgu çalıştırılır
                baglanti.Close();
                Temizle();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Listele();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtOgrenciAra_TextChanged(object sender, EventArgs e)
        {
            OgrenciArama(txtOgrenciAra.Text);// arama metodu
        }
        public void OgrenciArama(string aranacakKelime)//ÖĞRENCİ ARA
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open();
                }
                komut = new MySqlCommand(); //
                komut.Connection = baglanti;// mysql nesnesinin bağlantısı belirlendi
                komut.CommandText = "Select * From ogrenciler Where ad LIKE '" + aranacakKelime + "%'";//aramasorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komut);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable); //sorgudan dönen değer datatable nesnesine doldurulur
                baglanti.Close();
                gridOgrenci.DataSource = dataTable; //datatable nesnesine aktarılan veriler datagridview de listelenir

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
        }
    }

}
