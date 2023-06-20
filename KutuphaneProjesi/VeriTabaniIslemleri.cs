using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace KutuphaneProjesi
{
    internal class VeriTabaniIslemleri
    {
        string baglantiCumlesi = ConfigurationManager.ConnectionStrings["kutuphaneBaglantiCumlesi"].ConnectionString;
        //App.config dosyasının içindeki veri tabanı bağlantı cümlesi bir değişkene aktarıldı.
        public MySqlConnection baglan()
        { 
        MySqlConnection baglanti = new MySqlConnection(baglantiCumlesi);
            //veri tabanı bağlantısı oluşturuldu.
            MySqlConnection.ClearPool(baglanti);//önceki bağlantılar temizlendi.
            return baglanti;//oluşturulan bağlantı fonksiyonun çağırıldığı yere gönderildi.
        }
    }
}
