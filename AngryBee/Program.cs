using System;
using MCTProcon29Protocol;
using MCTProcon29Protocol.Methods;

namespace AngryBee
{
    class Program : IIPCClientReader
    {
        IPCManager manager;
        bool[] calledFlag;
        Boards.BoardSetting board;

        public Program()
        {
            manager = new IPCManager(this);
            calledFlag = new bool[7];
            for (int i = 0; i < 7; i++) { calledFlag[i] = false; }
        }


        public void OnGameInit(GameInit init)
        {
            calledFlag[0] = true;
        }

        public void OnTurnStart(TurnStart turn)
        {
            calledFlag[1] = true;
        }

        public void OnTurnEnd(TurnEnd turn)
        {
            calledFlag[2] = true;
        }

        public void OnGameEnd(GameEnd end)
        {
            calledFlag[3] = true;
        }

        public void OnPause(Pause pause)
        {
            calledFlag[4] = true;
        }

        public void OnInterrupt(Interrupt interrupt)
        {
            calledFlag[5] = true;
        }

        public void OnRebaseByUser(RebaseByUser rebase)
        {
            calledFlag[6] = true;
        }

        static void Main(string[] args)
        {
            /*byte width = 12;
            byte height = 12;

            var ai = new AI.AI();
            var game = Boards.BoardSetting.Generate(height, width);

            var meBoard = new Boards.ColoredBoardSmallBigger(height, width);
            var enemyBoard = new Boards.ColoredBoardSmallBigger(height, width);

            meBoard[game.me.Agent1] = true;
            meBoard[game.me.Agent2] = true;

            enemyBoard[game.enemy.Agent1] = true;
            enemyBoard[game.enemy.Agent2] = true;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var res = ai.Begin(3, game.setting, meBoard, enemyBoard, game.me, game.enemy);

            sw.Stop();
			Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < game.setting.ScoreBoard.GetLength(0); ++i)
            {
                for (int m = 0; m < game.setting.ScoreBoard.GetLength(1); ++m)
                {
                    string strr = game.setting.ScoreBoard[m, i].ToString();
                    int hoge = 4 - strr.Length;

                    if (meBoard[(uint)m, (uint)i])
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    else if (enemyBoard[(uint)m, (uint)i])
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;

                    for (int n = 0; n < hoge; ++n)
                        Console.Write(" ");
                    Console.Write(strr);
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            //Console.WriteLine(res);

            Console.WriteLine(res.Item1);

            for (int i = 0; i < game.setting.ScoreBoard.GetLength(1); ++i)
            {
                for (int m = 0; m < game.setting.ScoreBoard.GetLength(0); ++m)
                {
                    string strr = game.setting.ScoreBoard[m, i].ToString();
                    int hoge = 4 - strr.Length;

                    if (res.Item2[(uint)m, (uint)i])
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    else if (res.Item3[(uint)m, (uint)i])
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;

                    for (int n = 0; n < hoge; ++n)
                        Console.Write(" ");
                    Console.Write(strr);
                }
                Console.WriteLine();
            }

            Console.WriteLine("End Nodes:{0}[nodes]", ai.ends);
            Console.WriteLine("Time Elasped:{0}[ms]", sw.ElapsedMilliseconds);*/
        }
    }
}
