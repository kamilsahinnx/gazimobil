using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace gazimobil
{
    public partial class DersprogramiPage : ContentPage
    {
        private List<Ders> _dersList;
        private readonly List<string> _timeSlots = new List<string>
        {
            "08.30 - 09.20",
            "09.30 - 10.20",
            "10.30 - 11.20",
            "11.30 - 12.20",
            "12.30 - 13.20",
            "13.30 - 14.20",
            "14.30 - 15.20",
            "15.30 - 16.20"
        };

        private readonly List<string> _daysOfWeek = new List<string>
        {
            "Pazartesi",
            "Sal�",
            "�ar�amba",
            "Per�embe",
            "Cuma"
        };


        public DersprogramiPage()
        {
            InitializeComponent();
            _dersList = new List<Ders>();
            TimePicker.ItemsSource = _timeSlots; // Picker'a saat aral�klar�n� ekleme
            InitializePickers(); // G�nleri Picker'a eklemek i�in InitializePickers metodunu �a��r�n
        }
        private void InitializePickers()
        {
            // G�nlerin Picker'a eklenmesi
            foreach (var day in _daysOfWeek)
            {
                DayPicker.Items.Add(day);
            }
        }

        private void OnFabButtonClicked(object sender, EventArgs e)
        {
            AddLessonFrame.IsVisible = true;
            FabButton.IsVisible = false;
        }

        private void OnCloseButtonClicked(object sender, EventArgs e)
        {
            AddLessonFrame.IsVisible = false;
            FabButton.IsVisible = true;
        }

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            string day = DayPicker.SelectedItem?.ToString();
            string time = TimePicker.SelectedItem?.ToString();
            string subject = SubjectEntry.Text;

            if (string.IsNullOrWhiteSpace(day) || !_daysOfWeek.Contains(day))
            {
                DisplayAlert("Hata", "Ge�ersiz g�n se�ildi.", "Tamam");
                return;
            }

            if (string.IsNullOrWhiteSpace(time))
            {
                DisplayAlert("Hata", "L�tfen bir saat aral��� se�in.", "Tamam");
                return;
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                DisplayAlert("Hata", "Ders ad� bo� olamaz.", "Tamam");
                return;
            }

            var ders = new Ders
            {
                Day = day,
                Time = time,
                Subject = subject
            };

            _dersList.Add(ders);
            UpdateScheduleTable();

            // Formu kapat ve FAB'� tekrar g�ster
            AddLessonFrame.IsVisible = false;
            FabButton.IsVisible = true;
        }

        private void UpdateScheduleTable()
        {
            ScheduleTableRoot.Clear();

            foreach (var group in _dersList.GroupBy(d => d.Day))
            {
                var section = new TableSection(group.Key);

                foreach (var ders in group)
                {
                    section.Add(new TextCell
                    {
                        Text = ders.Time,
                        Detail = ders.Subject,
                        
                    });
                }

                ScheduleTableRoot.Add(section);
            }
        }
    }

    public class Ders
    {
        public string Day { get; set; }
        public string Time { get; set; }
        public string Subject { get; set; }
    }
}
