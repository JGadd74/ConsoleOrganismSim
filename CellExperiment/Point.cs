using System;




namespace CellExperiment
{
    public class Point : IEquatable<Point>
    {
        int X;
        int Y;
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int getX()
        {
            return X;
        }
        public int getY()
        {
            return Y;
        }

        bool IEquatable<Point>.Equals(Point other)
        {
            return this.getX() == other.getX() && this.getY() == other.getY();
        }
    }
}