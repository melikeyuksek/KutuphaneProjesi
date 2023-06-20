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
    public partial class formKitapTur : Form
    {
        public formKitapTur()
        {
            InitializeComponent();
        }
        VeriTabaniIslemleri vtIslemleri = new VeriTabaniIslemleri();//VeriTabaniIslemleri sınıfından bir nesne oluşturulur.
        MySqlConnection baglanti;//MySqlConnection sınıfından bir bağlantı nesnesi oluşturulur.
        MySqlCommand komut;//MySqlCommand sınıfından bir komut nesnesi oluşturulur.
        string komutSatiri;//Komutların yazılacağı sorgu ifadeleri bu değişken içerisinde tanımlanır.
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void formKitapTur_Load(object sender, EventArgs e)
        {
            TurleriListele();
        }
        public void TurleriListele() //TurleriListele metodu oluşturuldu
        {
            try
            {
                baglanti = vtIslemleri.baglan(); //veri tabanı bağlantı nesnesi oluşturuldu
                komutSatiri = " Select *From kitap_turleri"; //verileri listeleyecek sql sorgusu
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(komutSatiri, baglanti); //datadapter nesnesi oluşturuldu
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);//dönen kayıtlar datatable nesnesine aktarıldı
                gridKitapTur.DataSource = dataTable; //datatable nesnesindeki kayıtlar DataGridView de listelendi
                gridKitapTur.Columns["tur_id"].HeaderText = "ID";//sütun başlıkları
                gridKitapTur.Columns["tur_id"].Width = 100; //sütün genişliği
                gridKitapTur.Columns["tur_adi"].HeaderText = "Tür Adı";
                gridKitapTur.Columns["tur_adi"].Width = 300;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)//KAYDET
        {
            try
            {
                if (baglanti.State != ConnectionState.Open) //bağlantı durumu kontrol edilir
                {
                    baglanti.Open();//açık değilse açılır
                }
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                //Veritabanına kayıt eklemek için sorgu oluşturulur.
                komut.CommandText = "INSERT INTO kitap_turleri (tur_adi)VALUES(@tur_adi)";
                //Sorguda kullanılacak parametreler atanır.
                komut.Parameters.AddWithValue("@tur_adi", txtTurAdi.Text);

                komut.ExecuteNonQuery();//ekleme sorgusu
                baglanti.Close();
                txtTurAdi.Clear();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                TurleriListele(); //eklenenin datagridview de görünmesi için 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridKitapTur_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try 
            {
                txtTurAdi.Text = gridKitapTur.CurrentRow.Cells["tur_adi"].Value.ToString();
                //DataGridView nesnesindeki seçili türün textbox'a aktarılması

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSil_Click(object sender, EventArgs e)// SİL
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)// bağlantı kontrolü
                {
                    baglanti.Open();
                }
                komut = new MySqlCommand();
                komut.Connection = baglanti;
                komut.CommandText = "DELETE FROM kitap_turleri WHERE tur_id =@tur_id";
                komut.Parameters.AddWithValue("@tur_id", gridKitapTur.CurrentRow.Cells["tur_id"].Value.ToString());
                // sorguya parametre olarak datagridview de seçili olan tur_id bilgisi gönderildi
                komut.ExecuteNonQuery();
                baglanti.Close();
                txtTurAdi.Clear();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                TurleriListele();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try 
            
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open();
                }
                komut =new MySqlCommand();
                komut.Connection = baglanti;
                // Sorgu yazılır ve parametreler eklenir
                komut.CommandText = "UPDATE kitap_turleri SET tur_adi=@tur_adi where tur_id =@tur_id";

                // @tur_id parametresine seçili satırdan tur_id değeri eklenir
                komut.Parameters.AddWithValue("@tur_id", int.Parse(gridKitapTur.CurrentRow.Cells["tur_id"].Value.ToString()));
                // @tur_adi parametresine txtTurAdi textBox'ından girilen değer eklenir
                komut.Parameters.AddWithValue("@tur_adi", txtTurAdi.Text);

                komut.ExecuteNonQuery();// Sorgu çalıştırılır
                baglanti.Close();
                txtTurAdi.Clear();
                MessageBox.Show("İşlem başarılı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                TurleriListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
