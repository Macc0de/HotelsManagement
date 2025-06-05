using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels
{
    public class StandardRoom : Room
    {
        public StandardRoom(int number, int floor) : base(number, floor) { }
        public override string Type => "Стандартный";

        public override string ToString()
        {
            return base.ToString() + $", Тип: {Type}";
        }
    }
}
