using Mihenk.Text;

namespace Mihenk.Geography;

/// <summary>
/// TR: Türkiye'nin 7 coğrafi bölgesi.
/// <para/>
/// EN: The 7 geographical regions of Turkey.
/// </summary>
public enum GeographicalRegion
{
    Marmara = 1,
    Ege = 2,
    Akdeniz = 3,
    IcAnadolu = 4,
    Karadeniz = 5,
    DoguAnadolu = 6,
    GuneydoguAnadolu = 7
}

/// <summary>
/// TR: Bir ilin plaka, ad, telefon alan kodu, bölge, büyükşehir durumu, koordinat ve ilçe listesi bilgilerini temsil eder.
/// <para/>
/// EN: Represents a Turkish province's license plate code, name, telephone area code, geographical region, metropolitan status, coordinates, and district list.
/// </summary>
/// <param name="PlateCode">TR: İlin plaka kodu (1-81). / EN: License plate code of the province (1-81).</param>
/// <param name="Name">TR: İlin resmi adı. / EN: Official name of the province.</param>
/// <param name="AreaCode">TR: İlin sabit telefon alan kodu. / EN: Landline telephone area code of the province.</param>
/// <param name="Region">TR: İlin bulunduğu coğrafi bölge. / EN: Geographical region of the province.</param>
/// <param name="IsMetropolitan">TR: İlin büyükşehir belediyesi statüsünde olup olmadığı. / EN: Whether the province has metropolitan municipality status.</param>
/// <param name="Latitude">TR: İl merkezinin enlem koordinatı. / EN: Latitude coordinate of the province center.</param>
/// <param name="Longitude">TR: İl merkezinin boylam koordinatı. / EN: Longitude coordinate of the province center.</param>
/// <param name="Districts">TR: İle bağlı ilçelerin listesi. / EN: Read-only list of districts belonging to the province.</param>
public sealed record ProvinceInfo(
    int PlateCode,
    string Name,
    string AreaCode,
    GeographicalRegion Region,
    bool IsMetropolitan,
    double Latitude,
    double Longitude,
    IReadOnlyList<string> Districts);

/// <summary>
/// TR: Türkiye'nin 81 ili, ilçeleri, telefon alan kodları, bölgeleri ve iller arası kuş uçuşu mesafe hesaplamalarını
/// %100 offline (in-memory) olarak sunan coğrafi rehber.
/// <para/>
/// EN: Geographical directory providing all 81 Turkish provinces, districts, telephone area codes, regions,
/// and air-distance (great-circle) calculations 100% offline (in-memory).
/// </summary>
public static class Turkiye
{
    private static readonly ProvinceInfo[] ProvincesData =
    [
        new(1, "Adana", "322", GeographicalRegion.Akdeniz, true, 37.0000, 35.3213, ["Aladağ", "Ceyhan", "Çukurova", "Feke", "İmamoğlu", "Karaisalı", "Karataş", "Kozan", "Pozantı", "Saimbeyli", "Sarıçam", "Seyhan", "Tufanbeyli", "Yumurtalık", "Yüreğir"]),
        new(2, "Adıyaman", "416", GeographicalRegion.GuneydoguAnadolu, false, 37.7648, 38.2786, ["Merkez", "Besni", "Çelikhan", "Gerger", "Gölbaşı", "Kahta", "Samsat", "Sincik", "Tut"]),
        new(3, "Afyonkarahisar", "272", GeographicalRegion.Ege, false, 38.7507, 30.5567, ["Merkez", "Başmakçı", "Bayat", "Bolvadin", "Çay", "Çobanlar", "Dazkırı", "Dinar", "Emirdağ", "Evciler", "Hocalar", "İhsaniye", "İscehisar", "Kızılören", "Sandıklı", "Sinanpaşa", "Sultandağı", "Şuhut"]),
        new(4, "Ağrı", "472", GeographicalRegion.DoguAnadolu, false, 39.7191, 43.0503, ["Merkez", "Diyadin", "Doğubayazıt", "Eleşkirt", "Hamur", "Patnos", "Taşlıçay", "Tutak"]),
        new(5, "Amasya", "358", GeographicalRegion.Karadeniz, false, 40.6499, 35.8353, ["Merkez", "Göynücek", "Gümüşhacıköy", "Hamamözü", "Merzifon", "Suluova", "Taşova"]),
        new(6, "Ankara", "312", GeographicalRegion.IcAnadolu, true, 39.9334, 32.8597, ["Akyurt", "Altındağ", "Ayaş", "Bala", "Beypazarı", "Çamlıdere", "Çankaya", "Çubuk", "Elmadağ", "Etimesgut", "Evren", "Gölbaşı", "Güdül", "Haymana", "Kahramankazan", "Kalecik", "Keçiören", "Kızılcahamam", "Mamak", "Nallıhan", "Polatlı", "Pursaklar", "Sincan", "Şereflikoçhisar", "Yenimahalle"]),
        new(7, "Antalya", "242", GeographicalRegion.Akdeniz, true, 36.8969, 30.7133, ["Akseki", "Aksu", "Alanya", "Demre", "Döşemealtı", "Elmalı", "Finike", "Gazipaşa", "Gündoğmuş", "İbradı", "Kaş", "Kemer", "Kepez", "Konyaaltı", "Korkuteli", "Kumluca", "Manavgat", "Muratpaşa", "Serik"]),
        new(8, "Artvin", "466", GeographicalRegion.Karadeniz, false, 41.1828, 41.8183, ["Merkez", "Ardanuç", "Arhavi", "Borçka", "Hopa", "Kemalpaşa", "Murgul", "Şavşat", "Yusufeli"]),
        new(9, "Aydın", "256", GeographicalRegion.Ege, true, 37.8560, 27.8416, ["Bozdoğan", "Buharkent", "Çine", "Didim", "Efeler", "Germencik", "İncirliova", "Karacasu", "Karpuzlu", "Koçarlı", "Köşk", "Kuşadası", "Kuyucak", "Nazilli", "Söke", "Sultanhisar", "Yenipazar"]),
        new(10, "Balıkesir", "266", GeographicalRegion.Marmara, true, 39.6484, 27.8826, ["Altıeylül", "Ayvalık", "Balya", "Bandırma", "Bigadiç", "Burhaniye", "Dursunbey", "Edremit", "Erdek", "Gömeç", "Gönen", "Havran", "İvrindi", "Karesi", "Kepsut", "Manyas", "Marmara", "Savaştepe", "Sındırgı", "Susurluk"]),
        new(11, "Bilecik", "228", GeographicalRegion.Marmara, false, 40.1451, 29.9799, ["Merkez", "Bozüyük", "Gölpazarı", "İnhisar", "Osmaneli", "Pazaryeri", "Söğüt", "Yenipazar"]),
        new(12, "Bingöl", "426", GeographicalRegion.DoguAnadolu, false, 38.8854, 40.4980, ["Merkez", "Adaklı", "Genç", "Karlıova", "Kiğı", "Solhan", "Yayladere", "Yedisu"]),
        new(13, "Bitlis", "434", GeographicalRegion.DoguAnadolu, false, 38.4006, 42.1095, ["Merkez", "Adilcevaz", "Ahlat", "Güroymak", "Hizan", "Mutki", "Tatvan"]),
        new(14, "Bolu", "374", GeographicalRegion.Karadeniz, false, 40.7392, 31.6089, ["Merkez", "Dörtdivan", "Gerede", "Göynük", "Kıbrıscık", "Mengen", "Mudurnu", "Seben", "Yeniçağa"]),
        new(15, "Burdur", "248", GeographicalRegion.Akdeniz, false, 37.7203, 30.2908, ["Merkez", "Ağlasun", "Altınyayla", "Bucak", "Çavdır", "Çeltikçi", "Gölhisar", "Karamanlı", "Kemer", "Tefenni", "Yeşilova"]),
        new(16, "Bursa", "224", GeographicalRegion.Marmara, true, 40.1885, 29.0610, ["Büyükorhan", "Gemlik", "Gürsu", "Harmancık", "İnegöl", "İznik", "Karacabey", "Keles", "Kestel", "Mudanya", "Mustafakemalpaşa", "Nilüfer", "Orhaneli", "Orhangazi", "Osmangazi", "Yenişehir", "Yıldırım"]),
        new(17, "Çanakkale", "286", GeographicalRegion.Marmara, false, 40.1553, 26.4142, ["Merkez", "Ayvacık", "Bayramiç", "Biga", "Bozcaada", "Çan", "Eceabat", "Ezine", "Gelibolu", "Gökçeada", "Lapseki", "Yenice"]),
        new(18, "Çankırı", "376", GeographicalRegion.IcAnadolu, false, 40.6013, 33.6134, ["Merkez", "Atkaracalar", "Bayramören", "Çerkeş", "Eldivan", "Ilgaz", "Kızılırmak", "Korgun", "Kurşunlu", "Orta", "Şabanözü", "Yapraklı"]),
        new(19, "Çorum", "364", GeographicalRegion.Karadeniz, false, 40.5506, 34.9556, ["Merkez", "Alaca", "Bayat", "Boğazkale", "Dodurga", "İskilip", "Kargı", "Laçin", "Mecitözü", "Oğuzlar", "Ortaköy", "Osmancık", "Sungurlu", "Uğurludağ"]),
        new(20, "Denizli", "258", GeographicalRegion.Ege, true, 37.7765, 29.0864, ["Acıpayam", "Babadağ", "Baklan", "Bekilli", "Beyağaç", "Bozkurt", "Buldan", "Çal", "Çameli", "Çardak", "Çivril", "Güney", "Honaz", "Kale", "Merkezefendi", "Pamukkale", "Sarayköy", "Serinhisar", "Tavas"]),
        new(21, "Diyarbakır", "412", GeographicalRegion.GuneydoguAnadolu, true, 37.9144, 40.2306, ["Bağlar", "Bismil", "Çermik", "Çınar", "Çüngüş", "Dicle", "Eğil", "Ergani", "Hani", "Hazro", "Kayapınar", "Kocaköy", "Kulp", "Lice", "Silvan", "Sur", "Yenişehir"]),
        new(22, "Edirne", "284", GeographicalRegion.Marmara, false, 41.6771, 26.5557, ["Merkez", "Enez", "Havsa", "İpsala", "Keşan", "Lalapaşa", "Meriç", "Süloğlu", "Uzunköprü"]),
        new(23, "Elazığ", "424", GeographicalRegion.DoguAnadolu, false, 38.6810, 39.2264, ["Merkez", "Ağın", "Alacakaya", "Arıcak", "Baskil", "Karakoçan", "Keban", "Kovancılar", "Maden", "Palu", "Sivrice"]),
        new(24, "Erzincan", "446", GeographicalRegion.DoguAnadolu, false, 39.7500, 39.5000, ["Merkez", "Çayırlı", "İliç", "Kemah", "Kemaliye", "Otlukbeli", "Refahiye", "Tercan", "Üzümlü"]),
        new(25, "Erzurum", "442", GeographicalRegion.DoguAnadolu, true, 39.9043, 41.2679, ["Aşkale", "Aziziye", "Çat", "Hınıs", "Horasan", "İspir", "Karaçoban", "Karayazı", "Köprüköy", "Narman", "Oltu", "Olur", "Palandöken", "Pasinler", "Pazaryolu", "Şenkaya", "Tekman", "Tortum", "Uzundere", "Yakutiye"]),
        new(26, "Eskişehir", "222", GeographicalRegion.IcAnadolu, true, 39.7767, 30.5206, ["Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "İnönü", "Mahmudiye", "Mihalgazi", "Mihalıççık", "Odunpazarı", "Sarıcakaya", "Seyitgazi", "Sivrihisar", "Tepebaşı"]),
        new(27, "Gaziantep", "342", GeographicalRegion.GuneydoguAnadolu, true, 37.0662, 37.3833, ["Araban", "İslahiye", "Karkamış", "Nizip", "Nurdağı", "Oğuzeli", "Şahinbey", "Şehitkamil", "Yavuzeli"]),
        new(28, "Giresun", "454", GeographicalRegion.Karadeniz, false, 40.9128, 38.3895, ["Merkez", "Alucra", "Bulancak", "Çamoluk", "Çanakçı", "Dereli", "Doğankent", "Espiye", "Eynesil", "Görele", "Güce", "Keşap", "Piraziz", "Şebinkarahisar", "Tirebolu", "Yağlıdere"]),
        new(29, "Gümüşhane", "456", GeographicalRegion.Karadeniz, false, 40.4603, 39.4814, ["Merkez", "Kelkit", "Köse", "Kürtün", "Şiran", "Torul"]),
        new(30, "Hakkari", "438", GeographicalRegion.DoguAnadolu, false, 37.5833, 43.7333, ["Merkez", "Çukurca", "Derecik", "Şemdinli", "Yüksekova"]),
        new(31, "Hatay", "326", GeographicalRegion.Akdeniz, true, 36.2021, 36.1606, ["Altınözü", "Antakya", "Arsuz", "Belen", "Defne", "Dörtyol", "Erzin", "Hassa", "İskenderun", "Kırıkhan", "Kumlu", "Payas", "Reyhanlı", "Samandağ", "Yayladağı"]),
        new(32, "Isparta", "246", GeographicalRegion.Akdeniz, false, 37.7648, 30.5566, ["Merkez", "Aksu", "Atabey", "Eğirdir", "Gelendost", "Gönen", "Keçiborlu", "Senirkent", "Sütçüler", "Şarkikaraağaç", "Uluborlu", "Yalvaç", "Yenişarbademli"]),
        new(33, "Mersin", "324", GeographicalRegion.Akdeniz, true, 36.8000, 34.6333, ["Akdeniz", "Anamur", "Aydıncık", "Bozyazı", "Çamlıyayla", "Erdemli", "Gülnar", "Mezitli", "Mut", "Silifke", "Tarsus", "Toroslar", "Yenişehir"]),
        new(34, "İstanbul", "212/216", GeographicalRegion.Marmara, true, 41.0082, 28.9784, ["Adalar", "Arnavutköy", "Ataşehir", "Avcılar", "Bağcılar", "Bahçelievler", "Bakırköy", "Başakşehir", "Bayrampaşa", "Beşiktaş", "Beykoz", "Beylikdüzü", "Beyoğlu", "Büyükçekmece", "Çatalca", "Çekmeköy", "Esenler", "Esenyurt", "Eyüpsultan", "Fatih", "Gaziosmanpaşa", "Güngören", "Kadıköy", "Kağıthane", "Kartal", "Küçükçekmece", "Maltepe", "Pendik", "Sancaktepe", "Sarıyer", "Silivri", "Sultanbeyli", "Sultangazi", "Şile", "Şişli", "Tuzla", "Ümraniye", "Üsküdar", "Zeytinburnu"]),
        new(35, "İzmir", "232", GeographicalRegion.Ege, true, 38.4237, 27.1428, ["Aliağa", "Balçova", "Bayındır", "Bayraklı", "Bergama", "Beydağ", "Bornova", "Buca", "Çeşme", "Çiğli", "Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabağlar", "Karaburun", "Karşıyaka", "Kemalpaşa", "Kınık", "Kiraz", "Konak", "Menderes", "Menemen", "Narlıdere", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı", "Urla"]),
        new(36, "Kars", "474", GeographicalRegion.DoguAnadolu, false, 40.6013, 43.0975, ["Merkez", "Akyaka", "Arpaçay", "Digor", "Kağızman", "Sarıkamış", "Selim", "Susuz"]),
        new(37, "Kastamonu", "366", GeographicalRegion.Karadeniz, false, 41.3887, 33.7827, ["Merkez", "Abana", "Ağlı", "Araç", "Azdavay", "Bozkurt", "Cide", "Çatalzeytin", "Daday", "Devrekani", "Doğanyurt", "Hanönü", "İhsangazi", "İnebolu", "Küre", "Pınarbaşı", "Seydiler", "Şenpazar", "Taşköprü", "Tosya"]),
        new(38, "Kayseri", "352", GeographicalRegion.IcAnadolu, true, 38.7312, 35.4787, ["Akkışla", "Bünyan", "Develi", "Felahiye", "Hacılar", "İncesu", "Kocasinan", "Melikgazi", "Özvatan", "Pınarbaşı", "Sarıoğlan", "Sarız", "Talas", "Tomarza", "Yahyalı", "Yeşilhisar"]),
        new(39, "Kırklareli", "288", GeographicalRegion.Marmara, false, 41.7333, 27.2167, ["Merkez", "Babaeski", "Demirköy", "Kofçaz", "Lüleburgaz", "Pehlivanköy", "Pınarhisar", "Vize"]),
        new(40, "Kırşehir", "386", GeographicalRegion.IcAnadolu, false, 39.1425, 34.1709, ["Merkez", "Akçakent", "Akpınar", "Boztepe", "Çiçekdağı", "Kaman", "Mucur"]),
        new(41, "Kocaeli", "262", GeographicalRegion.Marmara, true, 40.8533, 29.8815, ["Başiskele", "Çayırova", "Darıca", "Derince", "Dilovası", "Gebze", "Gölcük", "İzmit", "Kandıra", "Karamürsel", "Kartepe", "Körfez"]),
        new(42, "Konya", "332", GeographicalRegion.IcAnadolu, true, 37.8746, 32.4932, ["Ahırlı", "Akören", "Akşehir", "Altınekin", "Beyşehir", "Bozkır", "Cihanbeyli", "Çeltik", "Çumra", "Derbent", "Derebucak", "Doğanhisar", "Emirgazi", "Ereğli", "Güneysınır", "Hadim", "Halkapınar", "Hüyük", "Ilgın", "Kadınhanı", "Karapınar", "Karatay", "Kulu", "Meram", "Sarayönü", "Selçuklu", "Seydişehir", "Taşkent", "Tuzlukçu", "Yalıhüyük", "Yunak"]),
        new(43, "Kütahya", "274", GeographicalRegion.Ege, false, 39.4167, 29.9833, ["Merkez", "Altıntaş", "Aslanapa", "Çavdarhisar", "Domaniç", "Dumlupınar", "Emet", "Gediz", "Hisarcık", "Pazarlar", "Simav", "Şaphane", "Tavşanlı"]),
        new(44, "Malatya", "422", GeographicalRegion.DoguAnadolu, true, 38.3552, 38.3095, ["Akçadağ", "Arapgir", "Arguvan", "Battalgazi", "Darende", "Doğanşehir", "Doğanyol", "Hekimhan", "Kale", "Kuluncak", "Pütürge", "Yazıhan", "Yeşilyurt"]),
        new(45, "Manisa", "236", GeographicalRegion.Ege, true, 38.6191, 27.4289, ["Ahmetli", "Akhisar", "Alaşehir", "Demirci", "Gölmarmara", "Gördes", "Kırkağaç", "Köprübaşı", "Kula", "Salihli", "Sarıgöl", "Saruhanlı", "Selendi", "Soma", "Şehzadeler", "Turgutlu", "Yunusemre"]),
        new(46, "Kahramanmaraş", "344", GeographicalRegion.Akdeniz, true, 37.5858, 36.9371, ["Afşin", "Andırın", "Çağlayancerit", "Dulkadiroğlu", "Ekinözü", "Elbistan", "Göksun", "Nurhak", "Onikişubat", "Pazarcık", "Türkoğlu"]),
        new(47, "Mardin", "482", GeographicalRegion.GuneydoguAnadolu, true, 37.3212, 40.7245, ["Artuklu", "Dargeçit", "Derik", "Kızıltepe", "Mazıdağı", "Midyat", "Nusaybin", "Ömerli", "Savur", "Yeşilli"]),
        new(48, "Muğla", "252", GeographicalRegion.Ege, true, 37.2153, 28.3636, ["Bodrum", "Dalaman", "Datça", "Fethiye", "Kavaklıdere", "Köyceğiz", "Marmaris", "Menteşe", "Milas", "Ortaca", "Seydikemer", "Ula", "Yatağan"]),
        new(49, "Muş", "436", GeographicalRegion.DoguAnadolu, false, 38.7432, 41.5064, ["Merkez", "Bulanık", "Hasköy", "Korkut", "Malazgirt", "Varto"]),
        new(50, "Nevşehir", "384", GeographicalRegion.IcAnadolu, false, 38.6244, 34.7239, ["Merkez", "Acıgöl", "Avanos", "Derinkuyu", "Gülşehir", "Hacıbektaş", "Kozaklı", "Ürgüp"]),
        new(51, "Niğde", "388", GeographicalRegion.IcAnadolu, false, 37.9667, 34.6833, ["Merkez", "Altunhisar", "Bor", "Çamardı", "Çiftlik", "Ulukışla"]),
        new(52, "Ordu", "452", GeographicalRegion.Karadeniz, true, 40.9839, 37.8764, ["Akkuş", "Altınordu", "Aybastı", "Çamaş", "Çatalpınar", "Çaybaşı", "Fatsa", "Gölköy", "Gülyalı", "Gürgentepe", "İkizce", "Kabadüz", "Kabataş", "Korgan", "Kumru", "Mesudiye", "Perşembe", "Ulubey", "Ünye"]),
        new(53, "Rize", "464", GeographicalRegion.Karadeniz, false, 41.0201, 40.5234, ["Merkez", "Ardeşen", "Çamlıhemşin", "Çayeli", "Derepazarı", "Fındıklı", "Güneysu", "Hemşin", "İkizdere", "İyidere", "Kalkandere", "Pazar"]),
        new(54, "Sakarya", "264", GeographicalRegion.Marmara, true, 40.7569, 30.3783, ["Adapazarı", "Akyazı", "Arifiye", "Erenler", "Ferizli", "Geyve", "Hendek", "Karapürçek", "Karasu", "Kaynarca", "Kocaali", "Pamukova", "Sapanca", "Serdivan", "Söğütlü", "Taraklı"]),
        new(55, "Samsun", "362", GeographicalRegion.Karadeniz, true, 41.2867, 36.3300, ["19 Mayıs", "Alaçam", "Asarcık", "Atakum", "Ayvacık", "Bafra", "Canik", "Çarşamba", "Havza", "İlkadım", "Kavak", "Ladik", "Salıpazarı", "Tekkeköy", "Terme", "Vezirköprü", "Yakakent"]),
        new(56, "Siirt", "484", GeographicalRegion.GuneydoguAnadolu, false, 37.9333, 41.9500, ["Merkez", "Baykan", "Eruh", "Kurtalan", "Pervari", "Şirvan", "Tillo"]),
        new(57, "Sinop", "368", GeographicalRegion.Karadeniz, false, 42.0231, 35.1531, ["Merkez", "Ayancık", "Boyabat", "Dikmen", "Durağan", "Erfelek", "Gerze", "Saraydüzü", "Türkeli"]),
        new(58, "Sivas", "346", GeographicalRegion.IcAnadolu, false, 39.7477, 37.0179, ["Merkez", "Akıncılar", "Altınyayla", "Divriği", "Doğanşar", "Gemerek", "Gölova", "Gürün", "Hafik", "İmranlı", "Kangal", "Koyulhisar", "Suşehri", "Şarkışla", "Ulaş", "Yıldızeli", "Zara"]),
        new(59, "Tekirdağ", "282", GeographicalRegion.Marmara, true, 40.9833, 27.5167, ["Çerkezköy", "Çorlu", "Ergene", "Hayrabolu", "Kapaklı", "Malkara", "Marmaraereğlisi", "Muratlı", "Saray", "Süleymanpaşa", "Şarköy"]),
        new(60, "Tokat", "356", GeographicalRegion.Karadeniz, false, 40.3167, 36.5500, ["Merkez", "Almus", "Artova", "Başçiftlik", "Erbaa", "Niksar", "Pazar", "Reşadiye", "Sulusaray", "Turhal", "Yeşilyurt", "Zile"]),
        new(61, "Trabzon", "462", GeographicalRegion.Karadeniz, true, 41.0015, 39.7178, ["Akçaabat", "Araklı", "Arsin", "Beşikdüzü", "Çarşıbaşı", "Çaykara", "Dernekpazarı", "Düzköy", "Hayrat", "Köprübaşı", "Maçka", "Of", "Ortahisar", "Sürmene", "Şalpazarı", "Tonya", "Vakfıkebir", "Yomra"]),
        new(62, "Tunceli", "428", GeographicalRegion.DoguAnadolu, false, 39.1079, 39.5401, ["Merkez", "Çemişgezek", "Hozat", "Mazgirt", "Nazımiye", "Ovacık", "Pertek", "Pülümür"]),
        new(63, "Şanlıurfa", "414", GeographicalRegion.GuneydoguAnadolu, true, 37.1591, 38.7969, ["Akçakale", "Birecik", "Bozova", "Ceylanpınar", "Eyyübiye", "Halfeti", "Haliliye", "Harran", "Hilvan", "Karaköprü", "Siverek", "Suruç", "Viranşehir"]),
        new(64, "Uşak", "276", GeographicalRegion.Ege, false, 38.6823, 29.4082, ["Merkez", "Banaz", "Eşme", "Karahallı", "Sivaslı", "Ulubey"]),
        new(65, "Van", "432", GeographicalRegion.DoguAnadolu, true, 38.4891, 43.4089, ["Bahçesaray", "Başkale", "Çaldıran", "Çatak", "Edremit", "Erciş", "Gevaş", "Gürpınar", "İpekyolu", "Muradiye", "Özalp", "Saray", "Tuşba"]),
        new(66, "Yozgat", "354", GeographicalRegion.IcAnadolu, false, 39.8181, 34.8147, ["Merkez", "Akdağmadeni", "Aydıncık", "Boğazlıyan", "Çandır", "Çayıralan", "Çekerek", "Kadışehri", "Saraykent", "Sarıkaya", "Sorgun", "Şefaatli", "Yenifakılı", "Yerköy"]),
        new(67, "Zonguldak", "372", GeographicalRegion.Karadeniz, false, 41.4564, 31.7987, ["Merkez", "Alaplı", "Çaycuma", "Devrek", "Ereğli", "Gökçebey", "Kilimli", "Kozlu"]),
        new(68, "Aksaray", "382", GeographicalRegion.IcAnadolu, false, 38.3687, 34.0370, ["Merkez", "Ağaçören", "Eskil", "Gülağaç", "Güzelyurt", "Ortaköy", "Sarıyahşi", "Sultanhanı"]),
        new(69, "Bayburt", "458", GeographicalRegion.Karadeniz, false, 40.2552, 40.2249, ["Merkez", "Aydıntepe", "Demirözü"]),
        new(70, "Karaman", "338", GeographicalRegion.IcAnadolu, false, 37.1759, 33.2287, ["Merkez", "Ayrancı", "Başyayla", "Ermenek", "Kazımkarabekir", "Sarıveliler"]),
        new(71, "Kırıkkale", "318", GeographicalRegion.IcAnadolu, false, 39.8468, 33.5153, ["Merkez", "Bahşılı", "Balışeyh", "Çelebi", "Delice", "Karakeçili", "Keskin", "Sulakyurt", "Yahşihan"]),
        new(72, "Batman", "488", GeographicalRegion.GuneydoguAnadolu, false, 37.8812, 41.1351, ["Merkez", "Beşiri", "Gercüş", "Hasankeyf", "Kozluk", "Sason"]),
        new(73, "Şırnak", "486", GeographicalRegion.GuneydoguAnadolu, false, 37.5164, 42.4611, ["Merkez", "Beytüşşebap", "Cizre", "Güçlükonak", "İdil", "Silopi", "Uludere"]),
        new(74, "Bartın", "378", GeographicalRegion.Karadeniz, false, 41.6344, 32.3375, ["Merkez", "Amasra", "Kurucaşile", "Ulus"]),
        new(75, "Ardahan", "478", GeographicalRegion.DoguAnadolu, false, 41.1105, 42.7022, ["Merkez", "Çıldır", "Damal", "Göle", "Hanak", "Posof"]),
        new(76, "Iğdır", "476", GeographicalRegion.DoguAnadolu, false, 39.9167, 44.0333, ["Merkez", "Aralık", "Karakoyunlu", "Tuzluca"]),
        new(77, "Yalova", "226", GeographicalRegion.Marmara, false, 40.6500, 29.2667, ["Merkez", "Altınova", "Armutlu", "Çınarcık", "Çiftlikköy", "Termal"]),
        new(78, "Karabük", "370", GeographicalRegion.Karadeniz, false, 41.2061, 32.6204, ["Merkez", "Eflani", "Eskipazar", "Ovacık", "Safranbolu", "Yenice"]),
        new(79, "Kilis", "348", GeographicalRegion.GuneydoguAnadolu, false, 36.7184, 37.1212, ["Merkez", "Elbeyli", "Musabeyli", "Polateli"]),
        new(80, "Osmaniye", "328", GeographicalRegion.Akdeniz, false, 37.0742, 36.2478, ["Merkez", "Bahçe", "Düziçi", "Hasanbeyli", "Kadirli", "Sumbas", "Toprakkale"]),
        new(81, "Düzce", "380", GeographicalRegion.Karadeniz, false, 40.8438, 31.1565, ["Merkez", "Akçakoca", "Cumayeri", "Çilimli", "Gölyaka", "Gümüşova", "Kaynaşlı", "Yığılca"])
    ];

    /// <summary>
    /// TR: Türkiye'nin 81 ilinin tamamını döner.
    /// <para/>
    /// EN: Returns all 81 provinces of Turkey.
    /// </summary>
    public static IReadOnlyList<ProvinceInfo> GetProvinces() => ProvincesData;

    /// <summary>
    /// TR: Plaka koduna (1-81 arası <see cref="int"/>) göre il bilgisini döner. Geçersizse <see langword="null"/> döner.
    /// <para/>
    /// EN: Returns province information by license plate code (<see cref="int"/> between 1 and 81), or <see langword="null"/> if invalid.
    /// </summary>
    /// <param name="plateCode">TR: İl plaka kodu (1-81). / EN: Province license plate code (1-81).</param>
    public static ProvinceInfo? GetProvince(int plateCode)
        => (plateCode >= 1 && plateCode <= 81) ? ProvincesData[plateCode - 1] : null;

    /// <summary>
    /// TR: İl adına (Türkçe büyük/küçük harf duyarsız, örn: "istanbul" veya "İstanbul") göre il bilgisini döner.
    /// <para/>
    /// EN: Returns province information by province name (Turkish case-insensitive, e.g., "istanbul" or "İstanbul").
    /// </summary>
    /// <param name="provinceName">TR: İl adı veya plaka kodu metni. / EN: Province name or plate code string.</param>
    public static ProvinceInfo? GetProvince(string? provinceName)
    {
        if (string.IsNullOrWhiteSpace(provinceName))
            return null;

        string trimmed = provinceName.Trim();
        if (int.TryParse(trimmed, out int plateCode))
            return GetProvince(plateCode);

        foreach (ProvinceInfo province in ProvincesData)
        {
            if (TurkceMetin.EqualsIgnoreCaseTr(province.Name, trimmed))
                return province;
        }

        return null;
    }

    /// <summary>
    /// TR: Plaka koduna göre il bilgisini almayı dener.
    /// <para/>
    /// EN: Attempts to get province information by license plate code.
    /// </summary>
    /// <param name="plateCode">TR: İl plaka kodu (1-81). / EN: Province license plate code (1-81).</param>
    /// <param name="province">TR: İl bilgisi çıktısı. / EN: The output province information.</param>
    public static bool TryGetProvince(int plateCode, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ProvinceInfo? province)
    {
        province = GetProvince(plateCode);
        return province is not null;
    }

    /// <summary>
    /// TR: İl adına göre il bilgisini almayı dener.
    /// <para/>
    /// EN: Attempts to get province information by province name.
    /// </summary>
    /// <param name="provinceName">TR: İl adı. / EN: Province name.</param>
    /// <param name="province">TR: İl bilgisi çıktısı. / EN: The output province information.</param>
    public static bool TryGetProvince(string? provinceName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ProvinceInfo? province)
    {
        province = GetProvince(provinceName);
        return province is not null;
    }

    /// <summary>
    /// TR: Plaka koduna (1-81) göre o ilin ilçelerini döner.
    /// <para/>
    /// EN: Returns the districts of the province with the specified license plate code (1-81).
    /// </summary>
    /// <param name="plateCode">TR: İl plaka kodu (1-81). / EN: Province license plate code (1-81).</param>
    public static IReadOnlyList<string> GetDistricts(int plateCode)
        => GetProvince(plateCode)?.Districts ?? [];

    /// <summary>
    /// TR: İl adına göre o ilin ilçelerini döner.
    /// <para/>
    /// EN: Returns the districts of the province with the specified name.
    /// </summary>
    /// <param name="provinceName">TR: İl adı. / EN: Province name.</param>
    public static IReadOnlyList<string> GetDistricts(string? provinceName)
        => GetProvince(provinceName)?.Districts ?? [];

    /// <summary>
    /// TR: Verilen ilçe adının belirtilen plaka kodlu ilde bulunup bulunmadığını kontrol eder.
    /// <para/>
    /// EN: Checks whether the specified district name exists in the province with the given license plate code.
    /// </summary>
    /// <param name="plateCode">TR: İl plaka kodu (1-81). / EN: Province license plate code (1-81).</param>
    /// <param name="districtName">TR: Kontrol edilecek ilçe adı. / EN: District name to check.</param>
    public static bool IsValidDistrict(int plateCode, string? districtName)
    {
        if (string.IsNullOrWhiteSpace(districtName))
            return false;

        ProvinceInfo? province = GetProvince(plateCode);
        return CheckDistrictInProvince(province, districtName.Trim());
    }

    /// <summary>
    /// TR: Verilen ilçe adının belirtilen ilde bulunup bulunmadığını kontrol eder.
    /// <para/>
    /// EN: Checks whether the specified district name exists in the given province.
    /// </summary>
    /// <param name="provinceName">TR: İl adı. / EN: Province name.</param>
    /// <param name="districtName">TR: Kontrol edilecek ilçe adı. / EN: District name to check.</param>
    public static bool IsValidDistrict(string? provinceName, string? districtName)
    {
        if (string.IsNullOrWhiteSpace(districtName))
            return false;

        ProvinceInfo? province = GetProvince(provinceName);
        return CheckDistrictInProvince(province, districtName.Trim());
    }

    /// <summary>
    /// TR: İki il merkezi arasındaki kuş uçuşu mesafeyi (Haversine formülü ile) kilometre cinsinden hesaplar.
    /// <para/>
    /// EN: Calculates the air (great-circle) distance in kilometers between two province centers using the Haversine formula.
    /// </summary>
    /// <param name="firstProvince">TR: Birinci il adı. / EN: First province name.</param>
    /// <param name="secondProvince">TR: İkinci il adı. / EN: Second province name.</param>
    public static double? GetAirDistanceKm(string? firstProvince, string? secondProvince)
    {
        ProvinceInfo? a = GetProvince(firstProvince);
        ProvinceInfo? b = GetProvince(secondProvince);
        if (a is null || b is null)
            return null;

        return CalculateHaversineKm(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
    }

    /// <summary>
    /// TR: İki plaka kodlu il merkezi arasındaki kuş uçuşu mesafeyi (Haversine formülü ile) kilometre cinsinden hesaplar.
    /// <para/>
    /// EN: Calculates the air (great-circle) distance in kilometers between two province centers by license plate codes using the Haversine formula.
    /// </summary>
    /// <param name="firstPlateCode">TR: Birinci il plaka kodu (1-81). / EN: First province plate code (1-81).</param>
    /// <param name="secondPlateCode">TR: İkinci il plaka kodu (1-81). / EN: Second province plate code (1-81).</param>
    public static double? GetAirDistanceKm(int firstPlateCode, int secondPlateCode)
    {
        ProvinceInfo? a = GetProvince(firstPlateCode);
        ProvinceInfo? b = GetProvince(secondPlateCode);
        if (a is null || b is null)
            return null;

        return CalculateHaversineKm(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
    }

    /// <summary>
    /// TR: İki coğrafi koordinat (enlem/boylam) arasındaki kuş uçuşu mesafeyi (Haversine formülü ile) kilometre cinsinden hesaplar.
    /// <para/>
    /// EN: Calculates the air (great-circle) distance in kilometers between two geographic coordinates (latitude/longitude) using the Haversine formula.
    /// </summary>
    /// <param name="lat1">TR: Birinci noktanın enlemi. / EN: Latitude of the first point.</param>
    /// <param name="lon1">TR: Birinci noktanın boylamı. / EN: Longitude of the first point.</param>
    /// <param name="lat2">TR: İkinci noktanın enlemi. / EN: Latitude of the second point.</param>
    /// <param name="lon2">TR: İkinci noktanın boylamı. / EN: Longitude of the second point.</param>
    public static double CalculateHaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(EarthRadiusKm * c, 2);
    }

    private static bool CheckDistrictInProvince(ProvinceInfo? province, string trimmedDistrict)
    {
        if (province is null)
            return false;

        foreach (string district in province.Districts)
        {
            if (TurkceMetin.EqualsIgnoreCaseTr(district.Trim(), trimmedDistrict))
                return true;
        }

        return false;
    }

    private static double DegreesToRadians(double deg) => deg * (Math.PI / 180.0);
}

