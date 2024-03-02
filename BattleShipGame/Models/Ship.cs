using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Models
{
    public class Ship
    {
        public string? Name { get; set; }
        public int Length { get; set; }
        public (int X, int Y) Position { get; set; }
        public bool IsHorizontal { get; set; }
    }
}
