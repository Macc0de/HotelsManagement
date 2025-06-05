using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels
{
    public class Guest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool AllInclusive { get; set; }
        public Room Room { get; set; }

        public Guest(string name, string surname, bool allInclusive, Room room)
        {
            Name = name;
            Surname = surname;
            AllInclusive = allInclusive;
            Room = room;
            room.IsUsed = true;
        }

        public override string ToString()
        {
            return $"Имя: {Name}, Фамилия: {Surname}, Комната: {Room.Number}, All Inclusive: {AllInclusive}";
        }
    }
}
