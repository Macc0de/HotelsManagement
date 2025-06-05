using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels
{
    interface IHotel
    {
        string Name { get; set; }
        string Country { get; set; }
        int Stars { get; set; }
        string Rating { get; }
        int FloorsCount { get; set; }
        int TotalRooms { get; }
        int UsedRooms { get; }
        int FreeRooms { get; }
        string XmlFilePath { get; }

        Guest this[int roomNumber] { get; }

        List<Guest> Guests { get; }
        List<Room> Rooms { get; }

        void CreateFloors();
        List<Room> GetAvailableRooms(int floor, string roomType);
        void LoadFromXml();
        void AddGuestToXml(Guest guest);
        void RemoveGuestFromXml(Guest guest);
        void EditInfo(Guest guest);
        void AddGuest(Guest guest);
        void RemoveGuest(int roomNumber);
    }
}
