# JWT Korumalı MP3 Streaming – Ders Notu

## 1. Bu çalışmada ne yaptık?

Bu çalışmada MP3 dosyalarını müzik platformuna güvenli biçimde eklemek için bir altyapı hazırladık.

Temel amaç şuydu:

- MP3 dosyaları projede hazır bulunsun.
- Kullanıcı şarkıya tıkladığında önce JWT kontrol edilsin.
- Kullanıcının üyelik paketi, şarkının istediği paketle karşılaştırılsın.
- Yetkisiz kullanıcı MP3 dosyasını doğrudan açamasın.
- Yetkili kullanıcı şarkıyı tarayıcıdaki müzik çalar üzerinden dinleyebilsin.

Bu amaçla MP3 dosyaları `wwwroot` içine konulmadı. Bunun yerine Web API projesindeki korumalı `MusicFiles` klasörüne yerleştirildi.

---

## 2. Neden MP3 dosyalarını `wwwroot` içine koymadık?

ASP.NET Core'da `wwwroot` klasörü herkese açık statik dosyalar için kullanılır.

Örneğin MP3 dosyasını aşağıdaki konuma koysaydık:

```text
wwwroot/music/song.mp3
```

kullanıcı dosyanın adresini bildiğinde doğrudan şu şekilde açabilirdi:

```text
https://localhost:7292/music/song.mp3
```

Bu durumda:

- JWT kontrolü çalışmazdı.
- Paket kontrolü atlanabilirdi.
- Basic kullanıcı, Elit pakete ait bir şarkıyı URL üzerinden açabilirdi.

Case'in ana konusu JWT ve paket yetkilendirmesi olduğu için MP3 dosyalarını herkese açık klasörde tutmak doğru olmazdı.

Bu nedenle kullandığımız yapı şöyledir:

```text
JwtMusicNight.WebApi/
└── MusicFiles/
    ├── song-1.mp3
    ├── song-2.mp3
    └── ...
```

`MusicFiles` klasörü `wwwroot` dışında olduğu için ASP.NET Core bu dosyaları otomatik olarak internete açmaz.

---

## 3. MP3 dosyaları nereye kopyalandı?

Kaynak klasör:

```text
C:\Users\TUFAN\Desktop\MP3
```

Projedeki hedef klasör:

```text
C:\Users\TUFAN\source\repos\JwtMusicNight\JwtMusicNight.WebApi\MusicFiles
```

Toplam 30 MP3 dosyası kopyalandı. Toplam boyut yaklaşık 178 MB'dır.

Önemli noktalar:

- Masaüstündeki kaynak dosyalar silinmedi.
- Kaynak dosyaların adları değiştirilmedi.
- Dosyalar yalnızca proje klasörüne kopyalandı.
- Bu aşamada veritabanına şarkı kaydı eklenmedi.

---

## 4. Genel çalışma akışı

Sistemin hedeflenen çalışma sırası şöyledir:

```text
Kullanıcı şarkının oynat düğmesine basar
                  ↓
WebUI: POST /Song/Listen/{id}
                  ↓
WebUI, Session içindeki JWT'yi alır
                  ↓
API: POST /api/Song/listen/{id}
                  ↓
API, JWT ve PackageLevel kontrolü yapar
           ↙                    ↘
      Yetkisiz                Yetkili
     403 Forbidden      ListeningHistory kaydı
                                  ↓
                     WebUI stream adresi döner
                                  ↓
                     GET /Song/Stream/{id}
                                  ↓
                 WebUI JWT'yi API'ye tekrar iletir
                                  ↓
                 GET /api/Song/stream/{id}
                                  ↓
                   API MP3 dosyasını yayınlar
```

Burada iki ayrı kontrol vardır:

1. `Listen` endpoint'i şarkıyı dinleme iznini kontrol eder ve dinleme geçmişi oluşturur.
2. `Stream` endpoint'i dosyanın kendisini göndermeden önce JWT ve paket seviyesini tekrar kontrol eder.

İkinci kontrol çok önemlidir. Çünkü kullanıcı `Listen` isteğini kullanmadan doğrudan stream adresini denemiş olabilir.

---

## 5. Web API tarafında yapılan değişiklikler

Değiştirilen dosya:

```text
JwtMusicNight.WebApi/Controllers/SongController.cs
```

### 5.1. `IWebHostEnvironment` neden eklendi?

Controller'a şu alan eklendi:

```csharp
private readonly IWebHostEnvironment _environment;
```

Constructor da şu şekilde değiştirildi:

```csharp
public SongController(
    ISongService songService,
    IWebHostEnvironment environment)
{
    _songService = songService;
    _environment = environment;
}
```

`IWebHostEnvironment`, uygulamanın çalıştığı ana klasörün yolunu öğrenmemizi sağlar.

Bu projede aşağıdaki değer kullanılır:

```csharp
_environment.ContentRootPath
```

Bu değer Web API projesinin kök klasörünü temsil eder. Böylece `MusicFiles` klasörünün tam yolunu sabit olarak yazmak zorunda kalmayız.

Yanlış yaklaşım:

```csharp
var path = "C:\\Users\\TUFAN\\source\\repos\\...";
```

Bu yol yalnızca belirli bir bilgisayarda çalışır.

Doğru yaklaşım:

```csharp
var musicRoot = Path.Combine(
    _environment.ContentRootPath,
    "MusicFiles");
```

Bu yaklaşım proje başka bilgisayara taşındığında da çalışabilir.

### 5.2. `Listen` cevabında ne değişti?

Önceden veritabanındaki `song.AudioUrl` değeri doğrudan dönüyordu.

Yeni yapıda kullanıcıya gerçek dosya yolu verilmez. Bunun yerine korumalı endpoint adresi döner:

```csharp
AudioUrl = $"/api/Song/stream/{song.SongId}"
```

Örnek cevap:

```json
{
  "songId": 5,
  "songName": "Believer",
  "audioUrl": "/api/Song/stream/5",
  "artistName": "Imagine Dragons"
}
```

Bu adres tek başına yeterli değildir. İstek sırasında geçerli JWT gönderilmesi gerekir.

### 5.3. API stream endpoint'i

Eklenen endpoint:

```csharp
[HttpGet("stream/{id}")]
[Authorize]
public async Task<IActionResult> Stream(int id)
```

Tam adres örneği:

```text
GET https://localhost:7292/api/Song/stream/5
```

`[Authorize]` nedeniyle geçerli JWT olmayan istekler içeri alınmaz.

### 5.4. Şarkı kontrolü

Önce veritabanından şarkı ve ilişkili paket bilgisi alınır:

```csharp
var song = await _songService.GetSongByIdAsync(id);
```

Şarkı bulunamazsa:

```csharp
return NotFound("Şarkı bulunamadı.");
```

HTTP sonucu:

```text
404 Not Found
```

### 5.5. JWT içindeki paket seviyesinin okunması

Login sırasında token içerisine `PackageLevel` claim'i eklenmektedir.

Stream endpoint'inde bu değer şu şekilde okunur:

```csharp
var packageLevelClaim = User.FindFirst("PackageLevel")?.Value;
```

Değer güvenli biçimde sayıya çevrilir:

```csharp
if (!int.TryParse(packageLevelClaim, out var userLevel))
{
    return Forbid();
}
```

`int.TryParse` kullanılmasının sebebi, claim eksik veya hatalı olduğunda uygulamanın exception vermesini önlemektir.

### 5.6. Paket kontrolü

Kullanıcının paket seviyesi şarkının paket seviyesinden düşükse dosya gönderilmez:

```csharp
if (userLevel < song.Package.Level)
{
    return StatusCode(StatusCodes.Status403Forbidden, new
    {
        message = "Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin."
    });
}
```

Örnek:

```text
Kullanıcı paketi: Basic   → Level 1
Şarkı paketi:    Premium → Level 3

1 < 3 olduğu için sonuç: 403 Forbidden
```

Başka bir örnek:

```text
Kullanıcı paketi: Elit → Level 4
Şarkı paketi:    Gold → Level 2

4 < 2 yanlış olduğu için kullanıcı şarkıyı dinleyebilir.
```

### 5.7. Güvenli dosya adının oluşturulması

Veritabanındaki `AudioUrl` alanında yalnızca MP3 dosyasının adı tutulmalıdır:

```text
Imagine-Dragons-Believer-Official-Music-Video_009_128k.mp3
```

Kod şu şekilde güvenli dosya adı oluşturur:

```csharp
var safeFileName = Path.GetFileName(song.AudioUrl);
```

`Path.GetFileName` kullanılmasının amacı path traversal saldırılarına karşı önlem almaktır.

Örneğin kötü niyetli bir değer şu olabilir:

```text
../../appsettings.json
```

`Path.GetFileName` bu değerden yalnızca dosya adı kısmını almaya çalışır ve üst klasörlere çıkılmasını engeller.

Tam dosya yolu daha sonra oluşturulur:

```csharp
var filePath = Path.Combine(musicRoot, safeFileName);
```

### 5.8. Dosyanın varlığının kontrol edilmesi

```csharp
if (string.IsNullOrWhiteSpace(safeFileName) ||
    !System.IO.File.Exists(filePath))
{
    return NotFound("MP3 dosyası bulunamadı.");
}
```

Veritabanında şarkı olsa bile karşılık gelen MP3 bulunmuyorsa API `404 Not Found` döner.

### 5.9. MP3 dosyasının gönderilmesi

```csharp
return PhysicalFile(
    filePath,
    "audio/mpeg",
    enableRangeProcessing: true);
```

Parametrelerin anlamı:

- `filePath`: Sunucudaki MP3 dosyasının tam yolu.
- `audio/mpeg`: Tarayıcıya bunun MP3 ses dosyası olduğunu söyler.
- `enableRangeProcessing: true`: Tarayıcının dosyanın belirli bölümünü istemesine izin verir.

Range processing özellikle şu işlemler için önemlidir:

- İlerleme çubuğundan ileri sarma
- Geri sarma
- Dosyanın tamamı inmeden belirli bir bölümden oynatma

---

## 6. Neden WebUI proxy endpoint'i gerekli oldu?

Değiştirilen dosya:

```text
JwtMusicNight.WebUI/Controllers/SongController.cs
```

API, JWT'yi `Authorization` header'ında bekler:

```text
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

WebUI'da JWT, Session içinde saklanmaktadır:

```csharp
HttpContext.Session.GetString("JwtToken")
```

JavaScript ile yapılan normal `fetch` isteğine header ekleyebiliriz. Ancak HTML `<audio>` elementi bir URL'yi açarken Session'daki JWT değerini otomatik olarak API'nin Bearer header'ına koyamaz.

Bu nedenle araya WebUI controller'ı konuldu:

```text
Tarayıcıdaki <audio>
        ↓
/Song/Stream/{id}
        ↓
WebUI Session'dan JWT'yi alır
        ↓
Authorization: Bearer ...
        ↓
/api/Song/stream/{id}
```

Bu yapıya proxy yaklaşımı denebilir. WebUI, tarayıcı ile korumalı API arasında aracılık eder.

---

## 7. WebUI `Listen` action'ında yapılan değişiklik

WebUI önce API'nin `Listen` endpoint'ini çağırır:

```csharp
var response = await client.PostAsync(
    "https://localhost:7292/api/Song/listen/" + id,
    null);
```

API başarılı cevap verirse JSON içerisindeki API stream adresi, WebUI stream adresi ile değiştirilir:

```csharp
var json = Newtonsoft.Json.Linq.JObject.Parse(body);
json["audioUrl"] = Url.Action(
    nameof(Stream),
    "Song",
    new { id });
```

Tarayıcıya gönderilen örnek adres:

```text
/Song/Stream/5
```

Böylece `<audio>` etiketi aynı WebUI uygulamasına istek gönderir. Tarayıcı Session cookie'sini aynı uygulamaya otomatik gönderir.

---

## 8. WebUI stream action'ı

Eklenen action:

```csharp
[HttpGet]
public async Task<IActionResult> Stream(int id)
```

İlk olarak Session'daki JWT alınır:

```csharp
var token = HttpContext.Session.GetString("JwtToken");
```

Token yoksa:

```csharp
return Unauthorized();
```

HTTP sonucu:

```text
401 Unauthorized
```

Token API isteğine eklenir:

```csharp
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);
```

Daha sonra korumalı API stream endpoint'i çağrılır:

```csharp
var response = await client.GetAsync(
    "https://localhost:7292/api/Song/stream/" + id);
```

API hata döndürürse aynı HTTP durum kodu WebUI tarafından tarayıcıya iletilir:

```csharp
if (!response.IsSuccessStatusCode)
{
    return StatusCode((int)response.StatusCode);
}
```

Başarılı cevapta MP3 byte dizisi okunur:

```csharp
var audioBytes = await response.Content.ReadAsByteArrayAsync();
```

Tarayıcıya ses dosyası olarak gönderilir:

```csharp
return File(
    audioBytes,
    "audio/mpeg",
    enableRangeProcessing: true);
```

### Mevcut yaklaşımın geliştirme notu

Bu yaklaşım küçük/orta boyutlu eğitim projesi için çalışır. Ancak `ReadAsByteArrayAsync`, MP3 dosyasının tamamını WebUI belleğine alır.

Gerçek ve yoğun trafikli bir projede:

- API cevabı parça parça aktarılmalı,
- Range header'ları WebUI tarafından API'ye iletilmeli,
- Dosyanın tamamı belleğe yüklenmemelidir.

Bu konu daha sonra performans iyileştirmesi olarak ele alınabilir.

---

## 9. `.csproj` dosyasında yapılan değişiklik

Değiştirilen dosya:

```text
JwtMusicNight.WebApi/JwtMusicNight.WebApi.csproj
```

Eklenen bölüm:

```xml
<ItemGroup>
  <Content Include="MusicFiles\**\*.mp3">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

Bu ayarların anlamı:

### `Content Include`

```xml
<Content Include="MusicFiles\**\*.mp3">
```

`MusicFiles` klasörünün bütün alt klasörlerindeki MP3 dosyalarını proje içeriğine dahil eder.

- `**` bütün alt klasörleri ifade eder.
- `*.mp3` yalnızca MP3 dosyalarını seçer.

### `CopyToOutputDirectory`

```xml
<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
```

Build işleminde MP3 dosyalarının `bin` çıktısına kopyalanmasını sağlar.

`PreserveNewest`, yalnızca kaynak dosya daha yeniyse tekrar kopyalama yapar.

### `CopyToPublishDirectory`

```xml
<CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
```

Uygulama publish edildiğinde MP3 dosyalarının yayın klasöründe bulunmasını sağlar.

Bu ayar olmazsa uygulama kendi bilgisayarımızda çalışabilir fakat publish sonrasında MP3 dosyaları eksik kalabilir.

---

## 10. Veritabanında ne saklanacak?

MP3'ün kendisi veritabanına kaydedilmeyecek.

`Songs` tablosundaki `AudioUrl` alanına yalnızca dosya adı yazılacak:

```text
Imagine-Dragons-Believer-Official-Music-Video_009_128k.mp3
```

Örnek şarkı kaydı mantığı:

```text
SongName:       Believer
AudioUrl:       Imagine-Dragons-Believer-Official-Music-Video_009_128k.mp3
ArtistId:       Imagine Dragons kaydının ID'si
GenreId:        Rock türünün ID'si
PackageId:      Gold paketinin ID'si
CoverImageUrl:  /covers/believer.jpg
Duration:       Şarkının gerçek süresi
```

Neden yalnızca dosya adı?

- Bilgisayara özel fiziksel yol veritabanına yazılmaz.
- Proje başka klasöre taşındığında kayıt bozulmaz.
- Dosyanın gerçek yolu çalışma anında `ContentRootPath` ile oluşturulur.

---

## 11. HTTP durum kodları

Bu sistemde karşılaşılabilecek temel cevaplar:

| Kod | Anlamı | Örnek durum |
|---|---|---|
| 200 | Başarılı | Kullanıcı şarkıyı dinleyebilir |
| 206 | Partial Content | Tarayıcı MP3'ün belirli bölümünü ister |
| 401 | Unauthorized | JWT veya Session token yok |
| 403 | Forbidden | JWT var fakat paket seviyesi yetersiz |
| 404 | Not Found | Şarkı kaydı veya MP3 dosyası bulunamadı |

`401` ile `403` arasındaki fark önemlidir:

- `401`: Kim olduğun bilinmiyor veya giriş yapmamışsın.
- `403`: Kim olduğun biliniyor fakat bu içerik için yetkin yok.

---

## 12. Postman ile nasıl test edilir?

### Adım 1 – Login

Login endpoint'ine kullanıcı adı ve parola gönderilir.

Başarılı cevapta JWT alınır.

### Adım 2 – Listen isteği

```text
POST /api/Song/listen/{id}
```

Headers:

```text
Authorization: Bearer TOKEN_BURAYA
```

Beklenen sonuçlar:

- Paket yeterliyse `200 OK`
- Paket yetersizse `403 Forbidden`
- Token yoksa `401 Unauthorized`

### Adım 3 – Stream isteği

```text
GET /api/Song/stream/{id}
```

Aynı Bearer token gönderilir.

Yetkili kullanıcıda cevap türü şu olmalıdır:

```text
Content-Type: audio/mpeg
```

### Adım 4 – Yetkisiz paket testi

Basic kullanıcının token'ı ile Premium veya Elit şarkı denenir.

Beklenen sonuç:

```text
403 Forbidden
```

Bu test hem `listen` hem de `stream` endpoint'i için ayrı ayrı yapılmalıdır.

---

## 13. Henüz yapılmayan işlemler

Bu çalışma sırasında aşağıdaki işlemler yapılmadı:

- Veritabanına 30 şarkı eklenmedi.
- Mevcut şarkı kayıtlarının `AudioUrl` alanları güncellenmedi.
- Sanatçı eşleşmeleri yapılmadı.
- Genre eşleşmeleri yapılmadı.
- Basic, Gold, Premium ve Elit dağılımı yapılmadı.
- Kapak görselleri eklenmedi.
- Şarkı süreleri otomatik çıkarılmadı.
- Önceki/sonraki şarkı gibi oynatıcı özellikleri tamamlanmadı.

Bu nedenle kod altyapısı hazırlanmış olsa da veritabanı kayıtları doğru MP3 adlarına bağlanmadan yeni dosyalar oynatılamaz.

---

## 14. Bundan sonra yapılacaklar

Önerilen sıra:

1. Kullanılacak 20 veya 30 şarkıya karar ver.
2. Her şarkı için şarkı adı ve sanatçıyı belirle.
3. Her şarkıya genre ata.
4. Her şarkıya paket seviyesi ata.
5. Kapak görsellerini hazırla.
6. `Songs.AudioUrl` alanına MP3 dosya adını yaz.
7. Şarkı sürelerini ekle.
8. Postman ile JWT/paket testlerini yap.
9. WebUI müzik çalarında süre, seek, ses ve sonraki/önceki kontrollerini tamamla.

---

## 15. Kısa özet

Bu yapıda MP3 dosyası doğrudan kullanıcıya açık değildir.

```text
MP3 dosyası → MusicFiles klasöründe
Dosya adı   → Songs.AudioUrl alanında
Kimlik      → JWT ile doğrulanır
Yetki       → PackageLevel ile kontrol edilir
Dosya       → Stream endpoint'i ile gönderilir
Tarayıcı    → WebUI proxy üzerinden dosyayı oynatır
```

Bu yapı Case'deki en önemli gereksinimi karşılamayı hedefler:

> Paket kontrolü yalnızca arayüzde değil, API tarafında da güvenli biçimde yapılmalıdır.

