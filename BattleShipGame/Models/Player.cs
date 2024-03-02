using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Models
{
    public class Player
    {
        readonly List<Ship> Ships =
        [
            new Ship
            {
                Name = "Carrier",
                Length = 5,
            },
            new Ship
            {
                Name = "Battleship",
                Length = 4,
            },
            new Ship
            {
                Name = "Cruiser",
                Length = 3,
            },
            new Ship
            {
                Name = "Submarine",
                Length = 2,
            },
            new Ship
            {
                Name = "Destroyer",
                Length = 1,
            },
        ];

        public int SideLength { get; set; }
        public char[,] Tiles { get; set; }
        public string Name { get; set; }
        public int HitCounter { get; set; } = 0;
        public Player? EnemyPlayer { get; set; }

        public Player(int sideLength, string name)
        {
            SideLength = sideLength;

            Name = name;

            Tiles = SetupTiles(sideLength);
        }

        #region Setup
        public char[,] SetupTiles(int sideLength)
        {
            var chars = new char[sideLength, sideLength];

            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    chars[i,j] = ' ';
                }
            }

            return chars;
        }
        public void RandomizeShips()
        {
            var random = new Random();

            foreach(var ship in Ships)
            {
                
                while(true)
                {
                    ship.IsHorizontal = random.Next(0, 2) == 0;
                    ship.Position = (random.Next(0, SideLength), random.Next(0, SideLength));

                    if(IsValidPosition(ship))
                    {
                        for (int i = 0; i < ship.Length; i++)
                        {
                            if (ship.IsHorizontal)
                            {
                                Tiles[ship.Position.X + i, ship.Position.Y] = 'S';
                            }
                            else
                            {
                                Tiles[ship.Position.X, ship.Position.Y + i] = 'S';
                            }
                        }
                        break;
                    }
                }

            }
        }
        public bool IsValidPosition(Ship ship)
        {
            if (ship.IsHorizontal)
            {
                if (ship.Position.X + ship.Length > SideLength)
                {
                    return false; 
                }

                for (int i = 0; i < ship.Length; i++)
                {
                    if (Tiles[ship.Position.X + i, ship.Position.Y] != ' ')
                    {
                        return false; 
                    }
                }
            }
            else
            {
                if (ship.Position.Y + ship.Length > SideLength)
                {
                    return false; 
                }
                for (int i = 0; i < ship.Length; i++)
                {
                    if (Tiles[ship.Position.X, ship.Position.Y + i] != ' ')
                    {
                        return false; 
                    }
                }
            }
            return true;
        }
        #endregion

        #region Update 
        public bool Shoot()
        {
            var bestTarget = CalculateBestTarget(EnemyPlayer.Tiles);

            if(bestTarget == null)
            {
                Console.WriteLine("Something went wrong!");
                return false;
            }

            return Catch(EnemyPlayer.Tiles, ((int X, int Y))bestTarget);

        }
        public List<(int X, int Y)> GetUnknowns(char[,] tileSet)
        {
            List<(int X, int Y)> unknownTiles = [];

            for(int i = 0; i < tileSet.GetLength(0); i++)
            {
                for(int j = 0; j < tileSet.GetLength(1); j++)
                {
                    if(tileSet[i, j] != 'H' && tileSet[i,j] != 'M')
                    {
                        unknownTiles.Add((i, j));
                    }
                    
                }
            }

            return unknownTiles;
        }
        public List<(int X, int Y)> GetEveryNTile(char[,] tileSet, int everyNColumn, int everyNRow)
        {
            List<(int X, int Y)> tiles = [];

            int columns = tileSet.GetLength(0) / (everyNColumn > 0 ? everyNColumn : 1);
            int rows = tileSet.GetLength(1) / (everyNRow > 0 ? everyNRow : 1);    

            for(int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (tileSet[i * everyNColumn, j * everyNRow] != 'M' && tileSet[i * everyNColumn, j * everyNRow] != 'H')
                        tiles.Add((i * everyNColumn, j * everyNRow));
                }
            }

            return tiles;
        }
        public List<(int X, int Y)> GetAdjacentUnknowns(char[,] tileSet, (int X, int Y) position)
        {
            List<(int X, int Y)> tiles = [];

            if (position.X >= 1 && tileSet[position.X - 1, position.Y] != 'H' && tileSet[position.X - 1, position.Y] != 'M')
            {
                tiles.Add((position.X - 1, position.Y));
            }

            if (position.X < SideLength - 1 && tileSet[position.X + 1, position.Y] != 'H' && tileSet[position.X + 1, position.Y] != 'M')
            {
                tiles.Add((position.X + 1, position.Y));
            }

            if (position.Y >= 1 && tileSet[position.X, position.Y - 1] != 'H' && tileSet[position.X, position.Y - 1] != 'M')
            {
                tiles.Add((position.X, position.Y - 1));
            }

            if (position.Y < SideLength - 1 && tileSet[position.X, position.Y + 1] != 'H' && tileSet[position.X, position.Y + 1] != 'M')
            {
                tiles.Add((position.X, position.Y + 1));
            }

            return tiles;
        }
        public (int X, int Y)? CalculateBestTarget(char[,] tileSet)
        {
            List<(int X, int Y)> tiles = [];

            for (int i = 0; i < tileSet.GetLength(0); i++)
            {
                for (int j = 0; j < tileSet.GetLength(1); j++)
                {
                    if (tileSet[i,j] == 'H')
                    {
                        tiles.AddRange(EnemyPlayer.GetAdjacentUnknowns(tileSet, (i, j)));
                    }
                }
            }

            var rand = new Random();

            //doesn't seem to have any impact on average rounds count
            //if (tiles.Count == 0)
            //    tiles = GetEveryNTile(tileSet, 2, 2);

            if (tiles.Count == 0)
                tiles = GetUnknowns(tileSet);

            if (tiles.Count == 0)
                return null;

            return tiles[rand.Next(0, tiles.Count)];
        }
        public bool Catch(char[,] tileSet, (int X, int Y) position)
        {
            switch (tileSet[position.X, position.Y])
            {
                case ' ':
                    tileSet[position.X, position.Y] = 'M';
                    Console.WriteLine($"{Name} shot tile [{(char)('A' + position.X)}, {position.Y + 1}] and missed!");
                    break;
                case 'S':
                    tileSet[position.X, position.Y] = 'H';
                    Console.WriteLine($"{Name} shot tile [{(char)('A' + position.X)}, {position.Y + 1}] and hit enemy ship!");
                    HitCounter++;
                    return true;
                case 'M':
                    Console.WriteLine($"{Name} shot tile [{(char)('A' + position.X)}, {position.Y + 1}] and hit already missed tile!");
                    break;
                case 'H':
                    Console.WriteLine($"{Name} shot tile [{(char)('A' + position.X)}, {position.Y + 1}] and hit the same ship tile again!");
                    break;
            }
            return false;
        }
        public bool CalculateWin()
        {
            return EnemyPlayer.Ships.Sum(ship => ship.Length) <= HitCounter;
        }
        #endregion

        #region Draw
        public void DrawBoard()
        {
            Console.WriteLine("--- " + Name + " ---");

            Console.Write($"-  ");

            for (int j = 0; j < SideLength; j++)
            {
                Console.Write($" {j+1} ");
            }
            Console.WriteLine();

            for (int i = 0; i < SideLength; i++)
            {
                Console.Write($"{(char)('A' + i)}: ");
                for (int j = 0; j < SideLength; j++)
                {
                    Console.Write($"[{Tiles[i,j]}]");
                }
                Console.WriteLine(); 
            }
        }
        #endregion
    }
}
