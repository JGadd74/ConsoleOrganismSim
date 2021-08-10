using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;

namespace CellExperiment
{
    public static class Renderer
    {
        public static string? Board_Image = null;
        public static string TopM, MidM, BottomM;

        public static ConcurrentQueue<Point> NewPointsToRender = new ConcurrentQueue<Point>();


        public static void PrintBoard(bool color = false)
        {
            Console.SetCursorPosition(0, 0);

            int cntr = 0;
            if (color)
            {
                foreach (var c in Board_Image)
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.CursorVisible = false;
                    if (c == 'X') Console.ForegroundColor = ConsoleColor.Red;
                    else if (c == '\u2591') Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(c);
                }
                //ShowTopRightMessage(Environment.ActiveBoardSpaces[0].GetPOS());
                ShowTopRightMessage(TopM);
                ShowMidRightMessage(MidM);
                ShowBottomRightMessage(BottomM);

            }
            else
            {
                Console.CursorVisible = false;
                Console.WriteLine(Board_Image);
                ShowTopRightMessage(TopM);
                ShowMidRightMessage(MidM);
                ShowBottomRightMessage(BottomM);

            }
            Console.SetCursorPosition(0, 0);
        }
        internal static void SpaceOccupationChanged(object sender, SpaceEventArgs e)
        {
            OneTimeRender(e.point);
        }



        public static void StartRenderThread(int delay = 0)
        { //only update active spaces
          // without touching the whole map image
          // **efficiency update suggestion
          //    instead of checking all surrounding spaces
          //    have cell broadcast its intended direction
          //    of movement, update new and old points


            // get a collection of all the points as int arrays, [0]=x,[1]=y
            Thread updater = new Thread(() =>
            {
                while (true)
                {
                    var points = new List<Point>();
                    try
                    {
                        points = (from p in Environment.ActiveBoardSpaces.Keys
                                  select Environment.ParsePointFromString(p)).ToList();
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    var adjacentPoints = new List<Point>();
                    foreach (var point in points)
                    {
                        //add all points adjacent to target point so they can be updated
                        // this is to compensate for unpredictable(currently) movement
                        var x = point.getX();
                        var y = point.getY();
                        adjacentPoints.Add(new Point(x + 1, y + 1));
                        adjacentPoints.Add(new Point(x - 1, y - 1));
                        adjacentPoints.Add(new Point(x + 1, y - 1));
                        adjacentPoints.Add(new Point(x - 1, y + 1));
                        adjacentPoints.Add(new Point(x + 0, y + 1));
                        adjacentPoints.Add(new Point(x + 1, y + 0));
                        adjacentPoints.Add(new Point(x + 0, y - 1));
                        adjacentPoints.Add(new Point(x - 1, y + 0));
                    }

                    points.AddRange(adjacentPoints);

                    foreach (var point in points)
                    {
                        var x = point.getX();
                        var y = point.getY();
                        Console.SetCursorPosition(x, y);
                        var charImage = Environment.Board[x, y].Space_Image;
                        switch (charImage) // update to access a broader set of chars
                        {
                            case 'X':
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case '\u2591':
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                break;
                            case '\u2593':
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                        }
                        Console.CursorVisible = false;
                        Console.Write(charImage);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.SetCursorPosition(0, 0);
                }
            });
            updater.Start();
        }
        public static void StartRenderThreadNew(int delay = 0)
        {
            Thread updater = new Thread( () => 
            {
                while (true)
                {
                    Console.CursorVisible = false;

                    if (NewPointsToRender.Count == 0) continue;
                   
                    while(NewPointsToRender.Count > 0)
                    {
                        Point NewPointToRender;
                        bool Continue = NewPointsToRender.TryDequeue(out NewPointToRender);
                        if (!Continue) continue;

                        var x = NewPointToRender.getX();
                        var y = NewPointToRender.getY();

                        Console.SetCursorPosition(x, y);
                        char image = Environment.Board[x, y].Space_Image;

                        switch (image)
                        {
                            case 'X':
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case '\u2591':
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                break;
                            case '\u2593':
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                        }

                        Console.CursorVisible = false;
                        Console.Write(image);
                        Console.ForegroundColor = ConsoleColor.White;


                        //Thread.Sleep(delay);
                    }

                }
            });
            updater.Start();
        }
        public static void OneTimeRender(Queue<Point> points)
        {
            while (points.Count > 0)
            {
                var NewPointToRender = points.Dequeue();
                var x = NewPointToRender.getX();
                var y = NewPointToRender.getY();

                Console.SetCursorPosition(x, y);
                char image = Environment.Board[x, y].Space_Image;

                switch (image)
                {
                    case 'X':
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case '\u2591':
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case '\u2593':
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.CursorVisible = false;
                Console.Write(image);
                Console.ForegroundColor = ConsoleColor.White;
            }
            

        }
        public static void OneTimeRender(Point point)
        {
            if (point == null) return;
            var NewPointToRender = point;
            var x = NewPointToRender.getX();
            var y = NewPointToRender.getY();

            Console.SetCursorPosition(x, y);
            char image = Environment.Board[x, y].Space_Image;

            switch (image)
            {
                case 'X':
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case '\u2591':
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case '\u2593':
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.CursorVisible = false;
            Console.Write(image);

        }


        public static void ShowTopRightMessage(string message)
        {
            Console.SetCursorPosition(Environment.Columns+1, 0);
            Console.Write(message);
        }
        public static void ShowMidRightMessage(string message)
        {
            Console.SetCursorPosition(Environment.Columns+1, Environment.Rows / 2);
            Console.Write(message);
        }
        public static void ShowBottomRightMessage(string message)
        {
            Console.SetCursorPosition(Environment.Columns+1, Environment.Rows - 1);
            Console.Write(message);
        }
        public static void Build_Board_Image()
        {
            Board_Image = "";
            for (int j = 0; j < Environment.Rows; j++)
            {
                for (int i = 0; i <= Environment.Columns; i++)
                {
                    if (i == Environment.Columns) Board_Image += '\n';
                    //else if (Environment.Board[i, j].AdjInitd) Board_Image += '+';
                    else Board_Image += Environment.Board[i, j].Space_Image;
                }
            }
        }
    }
}
