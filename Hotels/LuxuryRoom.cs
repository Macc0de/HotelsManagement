using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels
{
    public class LuxuryRoom : Room
    {
        public LuxuryRoom(int number, int floor) : base(number, floor) { }
        public override string Type => "Люкс";

        public override string ToString()
        {
            return base.ToString() + $", Тип: {Type}";
        }
    }
}
