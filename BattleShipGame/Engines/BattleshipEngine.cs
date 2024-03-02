using BattleShipGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Engines
{
    public class BattleshipEngine
    {
        private readonly Player playerA;
        private readonly Player playerB;

        private bool isFirstPlayersTurn = true;
        private bool isFinished = false;

        private int roundCounter = 0;

        public BattleshipEngine()
        {
            playerA = new Player(10, "Player A");
            playerB = new Player(10, "Player B");

            playerA.EnemyPlayer = playerB;
            playerB.EnemyPlayer = playerA;
        }
        public int Run()
        {
            InitializeGame();

            DrawGame();

            Console.WriteLine("Press any key to continue");
            Console.ReadLine();

            while (!isFinished)
            {
                UpdateGame();
                DrawGame();

                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }

            return roundCounter;
        }

        public void InitializeGame()
        {
            playerA.RandomizeShips();
            playerB.RandomizeShips();
        }

        public void UpdateGame()
        {
            Console.Clear();

            if (isFirstPlayersTurn)
            {
                roundCounter++;
                if(playerA.Shoot() && playerA.CalculateWin())
                {
                    Console.WriteLine($"{playerA.Name} won the game in {roundCounter} moves!");
                    isFinished = true;
                }
            } 
            else
            {
                if (playerB.Shoot() && playerB.CalculateWin())
                {
                    Console.WriteLine($"{playerB.Name} won the game in {roundCounter} moves!");
                    isFinished = true;
                }
            }

            isFirstPlayersTurn = !isFirstPlayersTurn;
        }

        public void DrawGame()
        {
            DrawRoundCounter();
            DrawPlayersTurn();

            playerA.DrawBoard();
            playerB.DrawBoard();
        }
        
        public void DrawRoundCounter()
        {
            Console.WriteLine("Round: " + roundCounter);
        }

        public void DrawPlayersTurn()
        {
            Console.WriteLine($"Turn: {(isFirstPlayersTurn ? $"{playerA.Name}" : $"{playerB.Name}")}");
        }

    }
}
