using Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelsManagement
{
    public partial class EditGuestInfo : Window
    {
        private Dictionary<string, Hotel> hotels;
        private Guest currentGuest;

        public EditGuestInfo(Dictionary<string, Hotel> allHotels)
        {
            InitializeComponent();
            hotels = allHotels;

            HotelName.Items.Clear();
            foreach (string name in hotels.Keys)
            {
                HotelName.Items.Add(new ComboBoxItem { Content = name });
            }

            if (HotelName.Items.Count > 0)
            {
                HotelName.SelectedIndex = 0;
            }
        }

        private void FindGuest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentGuest = null;
                string hotelName = ((ComboBoxItem)HotelName.SelectedItem)?.Content.ToString();
                string roomNumber = RoomNumber.Text.Trim();

                if (!int.TryParse(roomNumber, out int number))
                {
                    MessageBox.Show("Введите корректный номер комнаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Hotel hotel = hotels[hotelName];
                Guest guest = hotel[number]; // Индексатор
                if (guest == null)
                {
                    MessageBox.Show("Гость не найден", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                currentGuest = guest;
                Name.Text = guest.Name;
                Surname.Text = guest.Surname;
                AllInclusiveCheck.IsChecked = guest.AllInclusive;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveGuest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentGuest == null)
                {
                    MessageBox.Show("Введите номер комнаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string name = Name.Text.Trim();
                string surname = Surname.Text.Trim();
                bool allInclusive = AllInclusiveCheck.IsChecked == true;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                {
                    MessageBox.Show("Имя и фамилия пустые", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                currentGuest.Name = name;
                currentGuest.Surname = surname;
                currentGuest.AllInclusive = allInclusive;

                string hotelName = ((ComboBoxItem)HotelName.SelectedItem)?.Content.ToString();
                Hotel hotel = hotels[hotelName];

                hotel.EditInfo(currentGuest);

                MessageBox.Show("Изменения внесены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RoomNumber.Text = "";
            Name.Text = "";
            Surname.Text = "";
            AllInclusiveCheck.IsChecked = false;
        }
    }
}
