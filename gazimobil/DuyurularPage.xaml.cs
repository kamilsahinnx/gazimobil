using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Maui.Controls;
using System.Web;
using System.Windows.Input;

namespace gazimobil
{
    public class DuyuruModel
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }
    }

    public partial class DuyurularPage : ContentPage
    {
        private int �uankiSayfa = 1;

        public ICommand OpenWebViewCommand { get; private set; }

        public DuyurularPage()
        {
            InitializeComponent();
            BindingContext = this; // BindingContext'i ayarlad�k
            OpenWebViewCommand = new Command<string>(async (url) => await OpenWebView(url));
            DuyuruY�kle();
            SayfaEtiketleriniG�ncelle();
        }

        private async void DuyuruY�kle(string searchString = "", int page = 1)
        {
            var url = $"https://gazi.edu.tr/view/announcement-list?id={page}&type=1&SearchString={searchString}&dates=&date=";
            var duyurular = await Duyurular�Getir(url);
            if (duyurular.Count > 0)
            {
                DuyurularCollectionView.ItemsSource = duyurular;
            }
            else
            {
                await DisplayAlert("Bilgi", "G�sterilecek duyuru bulunamad�.", "Tamam");
            }
        }

        private void AramaButonu(object sender, EventArgs e)
        {
            var searchString = SearchEntry.Text;
            �uankiSayfa = 1; // Arama yap�ld���nda sayfa numaras�n� s�f�rl�yoruz
            DuyuruY�kle(searchString, �uankiSayfa);
            SayfaEtiketleriniG�ncelle();
        }

        private void Aramay�TemizleButonu(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            �uankiSayfa = 1; // Arama s�f�rland���nda sayfa numaras�n� s�f�rl�yoruz
            DuyuruY�kle(page: �uankiSayfa);
            SayfaEtiketleriniG�ncelle();
        }

        private void OncekiButonuT�kland���nda(object sender, EventArgs e)
        {
            if (�uankiSayfa > 1)
            {
                �uankiSayfa--;
                DuyuruY�kle(SearchEntry.Text, �uankiSayfa);
                SayfaEtiketleriniG�ncelle();
            }
        }

        private void SonrakiButonuT�kland���nda(object sender, EventArgs e)
        {
            �uankiSayfa++;
            DuyuruY�kle(SearchEntry.Text, �uankiSayfa);
            SayfaEtiketleriniG�ncelle();
        }

        private void SayfaEtiketleriniG�ncelle()
        {
            PageLabel1.Text = (�uankiSayfa - 1).ToString();
            PageLabel2.Text = �uankiSayfa.ToString();
            PageLabel3.Text = (�uankiSayfa + 1).ToString();

            PageLabel1.IsVisible = �uankiSayfa > 1;
            PageLabel1.TextColor = Colors.Blue;
            PageLabel2.TextColor = Colors.Red;
            PageLabel3.TextColor = Colors.Blue;
        }

        private async Task OpenWebView(string url)
        {
            await Navigation.PushAsync(new WebViewPage(url));
        }

        private async Task<List<DuyuruModel>> Duyurular�Getir(string url)
        {
            List<DuyuruModel> duyurular = new List<DuyuruModel>();

            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetByteArrayAsync(url);
                var html = Encoding.UTF8.GetString(response);

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                var duyurularMainNode = document.DocumentNode.SelectSingleNode("//*[@id='subpage-maindiv']/div/div/div[1]");

                if (duyurularMainNode != null)
                {
                    foreach (var node in duyurularMainNode.SelectNodes(".//div[contains(@class, 'subpage-ann-single')]"))
                    {
                        var dateNode = node.SelectSingleNode(".//div[contains(@class, 'subpage-ann-date')]");
                        var titleNode = node.SelectSingleNode(".//div[contains(@class, 'subpage-ann-link')]/a");

                        if (titleNode != null && dateNode != null)
                        {
                            var title = HttpUtility.HtmlDecode(titleNode.InnerText.Trim());
                            var month = HttpUtility.HtmlDecode(dateNode.SelectSingleNode(".//span[contains(@class, 'ann-month')]").InnerText.Trim());
                            var day = HttpUtility.HtmlDecode(dateNode.SelectSingleNode(".//span[contains(@class, 'ann-day')]").InnerText.Trim());
                            var year = HttpUtility.HtmlDecode(dateNode.SelectSingleNode(".//span[contains(@class, 'ann-year')]").InnerText.Trim());
                            var date = $"{day} {month} {year}";
                            var link = "https://gazi.edu.tr" + titleNode.GetAttributeValue("href", string.Empty);
                            duyurular.Add(new DuyuruModel { Title = title, Date = date, Url = link });
                        }
                        else if (titleNode != null)
                        {
                            var title = HttpUtility.HtmlDecode(titleNode.InnerText.Trim());
                            var link = "https://gazi.edu.tr" + titleNode.GetAttributeValue("href", string.Empty);
                            duyurular.Add(new DuyuruModel { Title = title, Date = "Tarih Yok", Url = link });
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Hata", "Duyurular b�l�m� bulunamad�.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Duyurular al�n�rken bir hata olu�tu: {ex.Message}", "Tamam");
            }

            return duyurular;
        }
    }
}
