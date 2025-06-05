using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Hotels;
using Microsoft.Win32;

namespace HotelsManagement
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Hotel> Hotels = new Dictionary<string, Hotel>();

        public MainWindow()
        {
            InitializeComponent();

            string xmlPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Database\TurkeyHotel.xml"));

            Hotel turkeyHotel = new Hotel(xmlPath);
            turkeyHotel.LoadFromXml();
            Hotels.Add(turkeyHotel.Name, turkeyHotel);

            xmlPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Database\GreeceHotel.xml"));

            Hotel greeceHotel = new Hotel(xmlPath);
            greeceHotel.LoadFromXml();
            Hotels.Add(greeceHotel.Name, greeceHotel);

            xmlPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Database\RussiaHotel.xml"));

            Hotel russiaHotel = new Hotel(xmlPath);
            russiaHotel.LoadFromXml();
            Hotels.Add(russiaHotel.Name, russiaHotel);

            xmlPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Database\ItalyHotel.xml"));

            Hotel italyHotel = new Hotel(xmlPath);
            italyHotel.LoadFromXml();
            Hotels.Add(italyHotel.Name, italyHotel);

            LoadTable();
        }

        private void LoadTable()
        {
            HotelsGrid.ItemsSource = Hotels.Values.ToList();
        }

        private void bookRoom_Click(object sender, RoutedEventArgs e)
        {
            BookRoom window = new BookRoom(Hotels);
            window.ShowDialog();

            LoadTable();
        }

        private void releaseRoom_Click(object sender, RoutedEventArgs e)
        {
            ReleaseRoom window = new ReleaseRoom(Hotels);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                LoadTable();
            }
        }

        private void viewRooms_Click(object sender, RoutedEventArgs e)
        {
            ViewRooms window = new ViewRooms(Hotels);
            window.ShowDialog();
        }

        private void EditInfo_Click(object sender, RoutedEventArgs e)
        {
            EditGuestInfo window = new EditGuestInfo(Hotels);
            window.ShowDialog();
        }

        private void ExportDb_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                FileName = "Data.txt",
                InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."))
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (Hotel hotel in Hotels.Values)
                    {
                        sb.AppendLine($"Отель: {hotel.Name}");
                        sb.AppendLine();

                        foreach (Room room in hotel.Rooms.Where(i => i.IsUsed))
                        {
                            Guest guest = hotel[room.Number]; // Индексатор
                            if (guest == null) 
                                continue;

                            sb.AppendLine($"Гость: {guest.Name} {guest.Surname}");
                            sb.AppendLine($"Номер: {room.Number}");
                            sb.AppendLine($"Тип: {room.Type}");
                            sb.AppendLine($"Этаж: {room.Floor}");
                            sb.AppendLine($"All Inclusive: {(guest.AllInclusive ? "Да" : "Нет")}");
                            sb.AppendLine();
                        }
                        sb.AppendLine("---------------------------\n");
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Данные экспортированы в файл:\n{saveFileDialog.FileName}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
