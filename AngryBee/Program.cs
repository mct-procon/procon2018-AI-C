using System;

namespace AngryBee
{
    class Program
    {
        static void Main(string[] args)
        {
            var ai = new AI.AI();
            var game = Boards.BoardSetting.Generate();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var meBoard = new Boards.ColoredBoardSmallBigger(12, 12);
            var enemyBoard = new Boards.ColoredBoardSmallBigger(12, 12);

            meBoard[game.me.Agent1] = true;
            meBoard[game.me.Agent2] = true;

            enemyBoard[game.enemy.Agent1] = true;
            enemyBoard[game.enemy.Agent2] = true;

            int res = ai.Mini(1, game.setting,meBoard, enemyBoard, game.me, game.enemy);

            

            sw.Stop();
            for (int i = 0; i < game.setting.ScoreBoard.GetLength(0); ++i)
            {
                for (int m = 0; m < game.setting.ScoreBoard.GetLength(1); ++m)
                {
                    string strr = game.setting.ScoreBoard[i, m].ToString();
                    int hoge = 4 - strr.Length;

                    if ((game.me.Agent1.X == i && game.me.Agent1.Y == m) || (game.me.Agent2.X == i && game.me.Agent2.Y == m))
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    else if ((game.enemy.Agent1.X == i && game.enemy.Agent1.Y == m) || (game.enemy.Agent2.X == i && game.enemy.Agent2.Y == m))
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;

                    for (int n = 0; n < hoge; ++n)
                        Console.Write(" ");
                    Console.Write(strr);
                }
                Console.WriteLine();
            }
            Console.WriteLine(res);
            Console.WriteLine("End Nodes:{0}[nodes]", ai.ends);
            Console.WriteLine("Time Elasped:{0}[ms]", sw.ElapsedMilliseconds);
        }
    }
}
