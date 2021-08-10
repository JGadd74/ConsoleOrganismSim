using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;




namespace CellExperiment
{
    public static partial class Environment
    {
        public static int SIMSPEED = 0;
        public static int Rows = 45;//max 45
        public static int Columns = 190; // max 190

        public static Space[,] Board = new Space[Columns, Rows];
        public static Dictionary<string, Space> ActiveBoardSpaces = new Dictionary<string, Space>();

        public static Dictionary<Images, char> SpaceImages = new Dictionary<Images, char>()
        {
            { Images.Org1,'X' },
            { Images.Org2,' ' },
            { Images.Org3,' ' },
            { Images.Base,'\u2591' },
            { Images.Food,' ' },
            { Images.Locked,' ' },
        };
        public enum Images
        {
            Org1,
            Org2,
            Org3,
            Base,
            Food,
            Locked
        }


        public static bool BoardSpaceIsActive(string pointKey)
        {
            return ActiveBoardSpaces.ContainsKey(pointKey);
        }
        public static bool AddActiveSpace(Space newActiveSpace)
        {
            string positionAsString = Environment.ConvertPointToString(newActiveSpace.Getposition());
            try
            {
                ActiveBoardSpaces.Add(positionAsString, newActiveSpace);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public static void RemoveActiveSpace(Space oldSpace)
        {
            string positionAsString = Environment.ConvertPointToString(oldSpace.Getposition());
            ActiveBoardSpaces.Remove(positionAsString);
        }
        public static void Init_Board()
        {
            for (int j = 0; j < Rows; j++)
            {
                for (int i = 0; i < Columns; i++)
                {
                    if (i == 0 || j == 0 || i == Columns - 1 || j == Rows - 1  /*|| j == Rows/2 || i == Columns /3 || i== (Columns/3)*2*/)
                    {
                        Board[i, j] = new Space(i, j);
                        bool successfullyLocked = Board[i, j].LockDown();
                        //Console.SetCursorPosition(0, 38);
                    }

                    else Board[i, j] = new Space(i, j);

                }
            }

        }


        public static string ConvertPointToString(Point point)
        { // works. Takes ints x and y and combines into string delimited by ','
            int x = point.getX();
            int y = point.getY();
            return x.ToString() + ',' + y.ToString();
        }
        public static Space ConvertPointToSpace(Point p)
        {
            return Board[p.getX(), p.getY()];
        }
        public static Point ParsePointFromString(string coordString)
        { // works.  Takes string and returns int[] where 0 =x, 1=y
            var ints = coordString.Split(',');

            return new Point(int.Parse(ints[0]), int.Parse(ints[1]));
        }
        public static Space GetRelativeSpace(Space startingPoint, Direction d)
        {
            // get the space relative to the startingPoint
            // return null if space is Locked


            if (startingPoint == null) return null;


            var startingPOS = startingPoint.Getposition();
            var xMod = 0;
            var yMod = 0;

            switch (d)
            {
                case Direction.Left:
                    xMod = -1;
                    break;
                case Direction.Right:
                    xMod = 1;
                    break;
                case Direction.Up:
                    yMod = -1;
                    break;
                case Direction.Down:
                    yMod = 1;
                    break;
                case Direction.UpLeft:
                    yMod = -1;
                    xMod = -1;
                    break;
                case Direction.UpRight:
                    yMod = -1;
                    xMod = 1;
                    break;
                case Direction.DownLeft:
                    yMod = 1;
                    xMod = -1;
                    break;
                case Direction.DownRight:
                    yMod = 1;
                    xMod = 1;
                    break;
            }
            var newX = startingPOS.getX() + xMod;
            var newY = startingPOS.getY() + yMod;


            try
            {
                if (Board[newX, newY] != null ? !Board[newX, newY].IsLocked() : false) // heavy fisted but functional, try to find true issue
                {
                    return Board[startingPOS.getX() + xMod, startingPOS.getY() + yMod];
                }
                else return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public static int[] GetSlope(Point pointOne, Point pointTwo) // [0] rise, [1] run
        {
            int rise = pointOne.getY() - pointTwo.getY();
            int run = pointOne.getX() - pointTwo.getX();
            // if it doesn't reduce then this is 
            // also a measure of distance
            // but if it doesn't reduce then this
            // creates a very unnatural path
            // for a cell to follow.

            // how best to return this information???
            var divisor = gcd(rise, run);
            if (divisor > 1)
            {
                rise /= divisor;
                run /= divisor;
            }

            return new int[] { rise, run };
        }
        static int gcd(int a, int b)
        {
            //find the gcd using the Euclid’s algorithm
            while (a != b)
                if (a < b) b -= a;
                else a -= b;
            //since at this point a=b, the gcd can be either of them
            //it is necessary to pass the gcd to the main function
            return (a);
        }
        public static Space[] GetPath(Point start, Point finish)
        {
            /*
             find slope between start and finish
             check if path is clear
             if clear return all points along slope
             if !clear then find a path???
             for any slope other than 0 or 1 there will be missing steps along the line
             this will require extra work to find 
             also this is BROKEN
             */

            var slope = GetSlope(start, finish); // [0] = rise, [1] = run
            var pointsOnPath = new List<Point>(); // list of points that will make up
                                                  // the path from start to finish
            var lastPoint = start;
            while (!pointsOnPath.Contains(finish)) // keep going until the finish GETTING STUCK??
            {
                int ymod = -slope[0], xmod = slope[1];
                var newPoint = new Point(lastPoint.getX() + xmod, lastPoint.getY() + ymod);
                pointsOnPath.Add(newPoint);
                lastPoint = newPoint;
                Console.WriteLine("x:" + lastPoint.getX() + " y:" + lastPoint.getY());
                Thread.Sleep(10000);
            }
            var spaces = from p in pointsOnPath
                         select ConvertPointToSpace(p);
            return spaces.ToArray();
        }
        public static Direction GetRandomDirection()
        {
            int[] wtf = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var r = new Random().Next(0, 8);
            var randomDirection = (Environment.Direction)wtf[r];
            return randomDirection;
        }
        public static Space[] GetQuickPath(Point Start, Point Finish) // 
        {
            // 5,5 to 100,40  horizontal motion not working??

            List<Space> AtoB = new List<Space>(); // need not include A
            int newX = Start.getX();
            int newY = Start.getY();


            while (true)
            {
                if (newX < Finish.getX()) ++newX;
                else if (newX > Finish.getX()) --newX;
                if (newY < Finish.getY()) ++newY;
                else if (newY > Finish.getY()) --newY;

                //Console.WriteLine(newX.ToString() + ',' + newY.ToString());
                //Thread.Sleep(500);
                if (AtoB.Contains(Board[newX, newY])) break;
                AtoB.Add(Board[newX, newY]);
            }
            return AtoB.ToArray();

        }
        public static Space[] GetQuirkyPath(Point Start, Point Finish)
        {// like quick path but with some randomization
            List<Space> AtoB = new List<Space>(); // need not include A
            int newX = Start.getX();
            int newY = Start.getY();

            while (true)
            {
                var rando = new Random().Next(0, 10);
                bool pointChanged = false;
                if (rando % 2 == 0)
                {
                    if (newX < Finish.getX()) ++newX;
                    else if (newX > Finish.getX()) --newX;
                    pointChanged = true;
                }
                else if (rando == 3 || rando == 5 || rando == 7)// if prime do both
                {
                    if (newX < Finish.getX()) ++newX;
                    else if (newX > Finish.getX()) --newX;
                    if (newY < Finish.getY()) ++newY;
                    else if (newY > Finish.getY()) --newY;
                    pointChanged = true;

                }
                else
                {
                    if (newY < Finish.getY()) ++newY;
                    else if (newY > Finish.getY()) --newY;
                    pointChanged = true;

                }
                if (AtoB.Contains(Board[newX, newY])) break;
                if (pointChanged)
                {
                    //Console.WriteLine(newX.ToString() + "," + newY.ToString());
                    AtoB.Add(Board[newX, newY]);

                }
            }
            return AtoB.ToArray();
        } // FIX, doesn't go all the way
        public static void WalkPath(Cell mover, Space[] path)
        {
            foreach (var pointOnPath in path)
            {
                Thread.Sleep(SIMSPEED);
                var spacePoint = pointOnPath.Getposition();
                var cellPoint = mover.GetHostSpace().Getposition();
                //left or right
                if (spacePoint.getX() < cellPoint.getX()) mover.Move(Direction.Left, mover.GetOwner());
                else if (spacePoint.getX() > cellPoint.getX()) mover.Move(Direction.Right, mover.GetOwner());
                //up or down
                if (spacePoint.getY() < cellPoint.getY()) mover.Move(Direction.Up, mover.GetOwner());
                else if (spacePoint.getY() > cellPoint.getY()) mover.Move(Direction.Down, mover.GetOwner());

            }
        } // move func to cell, make alternative 


        public enum Direction
        {
            Left = 0,
            Right, //1
            Up, //2
            Down, //3
            UpLeft, //4
            UpRight,//5
            DownLeft,//6
            DownRight,//7
            NODIRECTION = 404
        }
        public static bool DirectionsAreSimilar(Direction dir1, Direction dir2)
        {
           

            return false;
        }
    }
}