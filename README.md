****************************************************************************************************


********* öğrenci numaralı Miraç Abay'a ait hastane veritabanı yönetim sitesi
projesine ait readme belgesidir.

Default Yönetici için kullanıcı adı: admin
Şifre: asdqwe

Proje üzerindeki silme işlemleri hatalı veri eklenme durumu için konmuştur.
Diğer tablolarla bağlantılı bir veri için silme işlemi yapılırsa sistem hata verecek ve veri silinmeyecektir.
Projeyi çalıştırmak için herhangi bir view üzerinde CTRL+F5 tuşuna basarak çalıştırma işlemi gerçekleştirebilirsiniz.

****************************************************************************************************


Gerekli yazılımlar:
- Microsoft SQL Server (Veritabanı yükleme ve yönetimi için)
- SQL Server Management Studio (SSMS) (Veritabanı yedeği ve sorgular için)
- Visual Studio (ASP.NET MVC projesini çalıştırmak ve kaynak kodlarını incelemek için)


****************************************************************************************************


Kurulum Adımları:
-SSMS'i açın ve veritabanları üzerine sağ tıklayarak restore databese seçeneğini seçin.
-Device seçeneği ardından add tuşuna basın ve HastaneVeritabani.bak dosyasını seçerek işlemi tamamlayın.
-Veritabanının, tabloların, sp yapılarının, trigger ve fonksiyonların yüklendiğinden emin olun.

-Visaul Studio projesini çalıştırmak için öncelikle HastaneVeritabani isimli klasörü masaüstüne çıkarın.
-Ardından HastaneVeritabani klasörüne giriş yapın ve HastaneVeritabani.sln dosyasını çalıştırın.
-Bağlantı dizesi Web.config içerisinde connectionStrings adı altında bulunmaktadır.
-Bu bağlantı dizesini kendi bilgisayarınıza göre düzenleyin.


****************************************************************************************************

-ilgili satır:

<connectionStrings><add name="HastaneVeritabaniEntities" connectionString="metadata=res://*/Models.Entity.Model1.csdl|res://*/Models.Entity.Model1.ssdl|res://*/Models.Entity.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=localhost;initial catalog=HastaneVeritabani;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" /><add name="HastaneVeritabaniEntities1" connectionString="metadata=res://*/Models.Entity.Model2.csdl|res://*/Models.Entity.Model2.ssdl|res://*/Models.Entity.Model2.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=localhost;initial catalog=HastaneVeritabani;integrated security=True;multipleactiveresultsets=True;trustservercertificate=True;application name=EntityFramework&quot;" providerName="System.Data.EntityClient" /><add name="HastaneVeritabaniEntities2" connectionString="metadata=res://*/Models.Entity.Model1.csdl|res://*/Models.Entity.Model1.ssdl|res://*/Models.Entity.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=localhost;initial catalog=HastaneVeritabani;integrated security=True;multipleactiveresultsets=True;trustservercertificate=True;application name=EntityFramework&quot;" providerName="System.Data.EntityClient" /></connectionStrings>

****************************************************************************************************


->Veritabanı entegrasyon işlemi için olası bir hata durumu için aşağıdaki adımları izleyebilirsiniz.

-Proje içerisinde Models>Entitiy içerisine girin ve Model1.edmx isimli dosyayı sağ tıklayarak silin.
-Ardından Entitiy klasörüne sağ tıklayın
-New İtem yoluna gidin
-Çıkan menüde sol kısımda Data yolunu seçin
-ADO.NET Entity Data Model seçin, isim olarak Model1 girin ve add tuşuna basın
-Çıkan menüde EF Designer from database seçeneği seçiliyken next tuşuna basın.
-Çıkan Entity Data Model Wizard sayfasında "Save connection settings in Web.Config as:" kısmının altında bulunan alanı HastaneVeritabaniEntities2 olarak ayarlayın ve next tuşuna basın.
-Ardından çıkan menüde Tables, Views, Stored Procedures and Functions tümünü seçerek Finish tuşuna basın.
-Bir uyarı çıkarsa "Yes to all" seçin ve devam edin.
-Entegrasyon doğru bir şekilde yapılarak model proje içerisine yerleştirilecektir.


****************************************************************************************************


VİDEO İÇİN BİLGİLENDİRME
https://drive.google.com/file/b/1GWJ60GbQTw3nRTQUYjNNxsoGuPepW4Hw/view?usp=sharing

proje videosuna yukarıda verdiğim link aracılığıyla erişilebilir.

00.00 - 03.12  =  Ön bilgilendirme ve CSS tanıtım
03.31 - 06.57  =  ER diyagramı tanıtım
06.58 - 17.20  =  Tablolar ve triggerlar tanıtım
17.21 - 22.04  =  Stored Procedurler ve fonksiyonlar tanıtım
22.05 - 34.04  =  ASP.NET MVC yapısı için küçük bir tanıtım ve site üzerinde birkaç örnek


****************************************************************************************************
