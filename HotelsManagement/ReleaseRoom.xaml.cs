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
using System.Xml.Linq;

namespace HotelsManagement
{
    public partial class ReleaseRoom : Window
    {
        private Dictionary<string, Hotel> hotels;

        public ReleaseRoom(Dictionary<string, Hotel> allHotels)
        {
            InitializeComponent();
            hotels = allHotels;

            HotelName.Items.Clear();
            foreach (String name in hotels.Keys)
            {
                HotelName.Items.Add(new ComboBoxItem { Content = name });
            }

            if (HotelName.Items.Count > 0)
            {
                HotelName.SelectedIndex = 0;
            }
        }

        private void ReleaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hotelName = ((ComboBoxItem)HotelName.SelectedItem)?.Content.ToString();
                string roomNumber = RoomNumber.Text.Trim();

                if (string.IsNullOrEmpty(roomNumber))
                {
                    MessageBox.Show("Введите номер комнаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(roomNumber, out int number))
                {
                    MessageBox.Show("Неверный формат номера", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Hotel hotel = hotels[hotelName];
                Guest guest = hotel[number];

                if (guest == null)
                {
                    MessageBox.Show("Номер не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                hotel.RemoveGuest(number); // В коллекции
                hotel.RemoveGuestFromXml(guest); // В БД

                MessageBox.Show($"Гость {guest.Name} {guest.Surname} выселен из комнаты {number}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
