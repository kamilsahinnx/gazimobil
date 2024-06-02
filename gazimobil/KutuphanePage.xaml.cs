namespace gazimobil
{
    public partial class KutuphanePage : ContentPage
    {
        public KutuphanePage()
        {
            InitializeComponent();
        }

        private async void KutuphaneButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.DisplayAlert("Fiziki K�t�phane �al��ma Saatleri", "Hafta ��i | Cumartesi : 08:30 - 22:00\r\nZemin Kat Okuma Salonu : 7/24\r\nS�nav D�nemlerinde : 7/24", "Kapat");
            await Navigation.PushAsync(new WebViewPage("https://kutuphane.gazi.edu.tr"));
        }
    }
}
