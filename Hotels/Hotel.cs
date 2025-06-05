using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hotels
{
    public class Hotel : IHotel
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Stars { get; set; }
        public string Rating => string.Concat(Enumerable.Range(0, 5)
            .Select(i => i < Stars ? "★" : "☆"));
        public int FloorsCount { get; set; }
        public int TotalRooms => FloorsCount * 10;
        public int UsedRooms => Guests.Count;
        public int FreeRooms => TotalRooms - UsedRooms;
        public string XmlFilePath { get; private set; }

        public Guest this[int roomNumber] => Guests.FirstOrDefault(i => i.Room.Number == roomNumber);
        // Номер комнаты гостя

        public List<Guest> Guests { get; private set; } = new List<Guest>();
        public List<Room> Rooms { get; private set; } = new List<Room>();

        public Hotel(string xmlFilePath)
        {
            XmlFilePath = xmlFilePath;
        }

        public void CreateFloors()
        {
            Rooms.Clear();

            for (int floor = 1; floor <= FloorsCount; floor++)
            {
                for (int number = 1; number <= 10; number++)
                {
                    int roomNumber = (floor - 1) * 10 + number + 9;
                    string type = (number <= 7) ? "Стандартный" : "Люкс";

                    Room room = CreateRoom(type, roomNumber, floor);
                    Rooms.Add(room);
                }
            }

            foreach (Guest guest in Guests)
            {
                Room room = Rooms.FirstOrDefault(i => i.Number == guest.Room.Number);
                if (room != null)
                {
                    room.IsUsed = true;
                }
            }
        }

        private Room CreateRoom(string type, int number, int floor)
        {
            switch (type)
            {
                case "Стандартный":
                    return new StandardRoom(number, floor);
                case "Люкс":
                    return new LuxuryRoom(number, floor);
                default:
                    throw new Exception($"Неизвестный тип комнаты: {type}");
            }
        }

        public void LoadFromXml()
        {
            if (string.IsNullOrEmpty(XmlFilePath))
                throw new InvalidOperationException("Путь к XML отсутствует");

            XDocument doc = XDocument.Load(XmlFilePath);
            XElement root = doc.Element("HotelGuests");

            Name = root.Attribute("Hotel").Value;
            Country = root.Attribute("Country")?.Value;
            Stars = int.Parse(root.Attribute("Stars")?.Value ?? "0");
            FloorsCount = int.Parse(root.Attribute("Floors")?.Value ?? "0");

            Guests.Clear();

            foreach (XElement guestElem in root.Elements("Guest"))
            {
                string name = guestElem.Element("Name")?.Value;
                string surname = guestElem.Element("Surname")?.Value;
                bool allInclusive = bool.Parse(guestElem.Element("AllInclusive").Value);

                XElement roomElem = guestElem.Element("Room");
                int number = int.Parse(roomElem.Element("Number")?.Value ?? "0");
                int floor = int.Parse(roomElem.Element("Floor")?.Value ?? "0");
                string type = roomElem.Element("Type")?.Value;

                Room room = CreateRoom(type, number, floor);
                Guest guest = new Guest(name, surname, allInclusive, room);
                Guests.Add(guest);
            }

            CreateFloors();
        }

        public void AddGuestToXml(Guest guest)
        {
            if (string.IsNullOrEmpty(XmlFilePath))
                throw new InvalidOperationException("Путь к XML отсутствует");

            XDocument doc = XDocument.Load(XmlFilePath);

            XElement guestElem = new XElement("Guest",
                new XElement("Name", guest.Name),
                new XElement("Surname", guest.Surname),
                new XElement("AllInclusive", guest.AllInclusive),
                new XElement("Room",
                    new XElement("Number", guest.Room.Number),
                    new XElement("Floor", guest.Room.Floor),
                    new XElement("Type", guest.Room.Type)
                )
            );

            doc.Root.Add(guestElem);
            doc.Save(XmlFilePath);
        }

        public void RemoveGuestFromXml(Guest guest)
        {
            if (guest == null)
                throw new ArgumentNullException(nameof(guest));

            Room room = Rooms.FirstOrDefault(i => i.Number == guest.Room.Number);
            if (room != null)
                room.IsUsed = false;

            XDocument doc = XDocument.Load(XmlFilePath);

            XElement guestElem = doc.Root.Elements("Guest")
                .FirstOrDefault(i => (int)i.Element("Room").Element("Number") == guest.Room.Number);

            if (guestElem != null)
            {
                guestElem.Remove();
                doc.Save(XmlFilePath);
            }
        }

        public void EditInfo(Guest guest)
        {
            XDocument doc = XDocument.Load(XmlFilePath);
            XElement guestElem = doc.Root.Elements("Guest")
                .FirstOrDefault(i => (int)i.Element("Room").Element("Number") == guest.Room.Number);

            if (guestElem != null)
            {
                guestElem.Element("Name").Value = guest.Name;
                guestElem.Element("Surname").Value = guest.Surname;
                guestElem.Element("AllInclusive").Value = guest.AllInclusive.ToString().ToLower();

                doc.Save(XmlFilePath);
            }

            Guest guestList = this[guest.Room.Number];
            if (guestList != null)
            {
                guestList.Name = guest.Name;
                guestList.Surname = guest.Surname;
                guestList.AllInclusive = guest.AllInclusive;
            }
        }

        public List<Room> GetAvailableRooms(int floor, string roomType)
        {
            return Rooms.Where(i => i.Floor == floor && i.Type == roomType && i.IsUsed == false).ToList();
        }

        public void AddGuest(Guest guest)
        {
            Guests.Add(guest);
            guest.Room.IsUsed = true;
        }

        public void RemoveGuest(int roomNumber)
        {
            Guest guest = this[roomNumber];
            if (guest != null)
            {
                Guests.Remove(guest);
                guest.Room.IsUsed = false;
            }
        }
    }
}
