using Hotels;
using System;
using System.IO;
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
    public partial class BookRoom : Window
    {
        private Dictionary<string, Hotel> hotels;

        public BookRoom(Dictionary<string, Hotel> allHotels)
        {
            InitializeComponent();
            hotels = allHotels;

            HotelName.SelectionChanged += HotelName_SelectionChanged;
            Floor.SelectionChanged += UpdateAvailableRooms;
            Type.SelectionChanged += UpdateAvailableRooms;

            HotelName.Items.Clear();
            foreach (string name in hotels.Keys)
            {
                HotelName.Items.Add(new ComboBoxItem { Content = name });
            }

            if (HotelName.Items.Count > 0)
            {
                HotelName.SelectedIndex = 0;
                HotelName_SelectionChanged(HotelName, null);
            }
        }

        private void HotelName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HotelName.SelectedItem is ComboBoxItem selectedHotelItem)
            {
                string hotelName = selectedHotelItem.Content.ToString();
                if (hotels.TryGetValue(hotelName, out var hotel))
                {
                    Floor.Items.Clear();
                    for (int i = 1; i <= hotel.FloorsCount; i++)
                    {
                        Floor.Items.Add(new ComboBoxItem { Content = $"{i} этаж" });
                    }

                    if (Floor.Items.Count > 0)
                        Floor.SelectedIndex = 0; 
                }
            }

            AvailableRooms();
        }

        private void UpdateAvailableRooms(object sender, SelectionChangedEventArgs e)
        {
            AvailableRooms();
        }

        private void AvailableRooms()
        {
            RoomSelection.Items.Clear();
            RoomSelection.IsEnabled = false;

            string hotelName = ((ComboBoxItem)HotelName.SelectedItem)?.Content.ToString();
            string floorText = ((ComboBoxItem)Floor.SelectedItem)?.Content.ToString();
            string type = ((ComboBoxItem)Type.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(hotelName) || string.IsNullOrEmpty(floorText) || string.IsNullOrEmpty(type))
                return;

            string digits = new string(floorText.Where(char.IsDigit).ToArray());
            int floor = int.Parse(digits);

            Hotel hotel = hotels[hotelName];
            List<Room> availableRooms = hotel.GetAvailableRooms(floor, type);

            if (availableRooms.Count == 0)
                return;

            RoomSelection.Items.Add("Авто");
            foreach (Room room in availableRooms)
            {
                RoomSelection.Items.Add(room.Number.ToString());
            }

            RoomSelection.SelectedIndex = 0;
            RoomSelection.IsEnabled = true;
        }

        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hotelName = ((ComboBoxItem)HotelName.SelectedItem)?.Content.ToString();
                string floorText = ((ComboBoxItem)Floor.SelectedItem)?.Content.ToString();
                string roomType = ((ComboBoxItem)Type.SelectedItem)?.Content.ToString();

                if (string.IsNullOrEmpty(hotelName) || string.IsNullOrEmpty(floorText) || string.IsNullOrEmpty(roomType))
                {
                    MessageBox.Show("Выберите отель, этаж и тип номера", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string digits = new string(floorText.Where(char.IsDigit).ToArray());
                int floor = int.Parse(digits);

                string name = Name.Text.Trim();
                string surname = Surname.Text.Trim();
                bool allInclusive = AllInclusiveCheck.IsChecked == true;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                {
                    MessageBox.Show("Введите имя и фамилию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!name.All(char.IsLetter) || !surname.All(char.IsLetter))
                {
                    MessageBox.Show("Имя и фамилия должны содержать только буквы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Hotel hotel = hotels[hotelName];
                List<Room> availableRooms = hotel.GetAvailableRooms(floor, roomType);

                if (availableRooms.Count == 0)
                {
                    MessageBox.Show($"Свободных {roomType} номеров на {floor} этаже нет", "Все номера забронированы", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Room room = null;
                if (RoomSelection.SelectedIndex > 0) // Номера на выбор
                {
                    int roomNumber = int.Parse(RoomSelection.SelectedItem.ToString());
                    room = hotel.GetAvailableRooms(floor, roomType)
                                                .FirstOrDefault(i => i.Number == roomNumber);
                }
                else // Авто: первый свободный номер
                {
                    room = hotel.GetAvailableRooms(floor, roomType).FirstOrDefault();
                }

                Guest guest = new Guest(name, surname, allInclusive, room);

                hotel.AddGuest(guest); // Коллекция
                hotel.AddGuestToXml(guest);

                MessageBox.Show($"Гость {name} {surname} заселён в комнату {room.Number}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearFields();
                AvailableRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            Name.Text = "";
            Surname.Text = "";
            AllInclusiveCheck.IsChecked = false;
        }
    }
}
