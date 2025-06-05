using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hotels
{
    public abstract class Room
    {
        public int Number { get; set; }
        public int Floor { get; set; }
        public bool IsUsed { get; set; }
        public abstract string Type { get; }
        public string Status => IsUsed ? "Занят" : "Свободен";

        public Room(int number, int floor)
        {
            Number = number;
            Floor = floor;
            IsUsed = false;
        }

        public override string ToString()
        {
            return $"Номер: {Number}, Этаж: {Floor}, Статус: {Status}";
        }
    }
}
