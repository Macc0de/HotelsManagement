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
    public partial class ViewRooms : Window
    {
        private Dictionary<string, Hotel> hotels;

        public ViewRooms(Dictionary<string, Hotel> allHotels)
        {
            InitializeComponent();

            hotels = allHotels;
            HotelName.SelectionChanged += HotelName_SelectionChanged;

            HotelName.Items.Clear();
            foreach (String name in hotels.Keys)
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

                    UpdateGrid();
                }
            }
        }

        private void Floor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            try
            {
                ComboBoxItem hotelItem = HotelName.SelectedItem as ComboBoxItem;
                if (hotelItem == null)
                    return;

                string hotelName = hotelItem.Content.ToString();

                Hotel hotel = hotels[hotelName];
                List<Room> rooms = hotel.Rooms;

                ComboBoxItem floorItem = Floor.SelectedItem as ComboBoxItem;
                if (floorItem != null)
                {
                    string floorText = floorItem.Content.ToString();
                    string digits = new string(floorText.Where(char.IsDigit).ToArray());
                    int floor = int.Parse(digits);

                    rooms = rooms.Where(r => r.Floor == floor).ToList();

                    int guestCount = hotel.Guests.Count(g => g.Room.Floor == floor);
                    GuestCountText.Text = guestCount.ToString();
                }

                var displayRooms = rooms.Select(i =>
                {
                    Guest guest = i.Status == "Занят" ? hotel[i.Number] : null;

                    return new
                    {
                        i.Number,
                        i.Type,
                        i.Status,
                        Guest = guest != null ? $"{guest.Name} {guest.Surname}" : "",
                        AllInclusive = guest != null ? (guest.AllInclusive ? "Да" : "Нет") : ""
                    };
                }).ToList();

                HotelGrid.ItemsSource = displayRooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
