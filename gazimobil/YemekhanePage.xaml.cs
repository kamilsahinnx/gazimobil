using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace gazimobil.Views
{
    public partial class YemekhanePage : ContentPage
    {
        private readonly string dosyaYolu;

        public YemekhanePage()
        {
            InitializeComponent();
            dosyaYolu = Path.Combine(AppContext.BaseDirectory, "Resources", "YemekhaneMenusu.txt");
            LoadMenu();
        }

        private async void LoadMenu()
        {
            try
            {
                var menu = await BugununMenusuAsync();
                if (!string.IsNullOrEmpty(menu))
                {
                    BugununMenusuLabel.Text = menu;
                }
                else
                {
                    BugununMenusuLabel.Text = "Bug�n i�in men� bulunamad�.";
                }
            }
            catch (Exception ex)
            {
                BugununMenusuLabel.Text = $"Men� y�klenirken hata olu�tu: {ex.Message}";
            }
        }

        public async Task<string> BugununMenusuAsync()
        {
            string bugun = DateTime.Now.ToString("dd.MM.yyyy dddd", new CultureInfo("tr-TR"));

            if (!File.Exists(dosyaYolu))
            {
                throw new FileNotFoundException($"Dosya bulunamad�: {dosyaYolu}");
            }

            string[] satirlar = await File.ReadAllLinesAsync(dosyaYolu);
            for (int i = 0; i < satirlar.Length; i++)
            {
                if (satirlar[i].Trim().StartsWith(bugun))
                {
                    return string.Join("\n", satirlar, i + 1, 6);
                }
            }

            return "Bug�n i�in men� bulunamad�.";
        }
    }
}
