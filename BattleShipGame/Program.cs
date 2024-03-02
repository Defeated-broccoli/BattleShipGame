using BattleShipGame.Engines;
using BattleShipGame.Models;

namespace BattleShipGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var engine = new BattleshipEngine();

            engine.Run();
        }
    }
}
