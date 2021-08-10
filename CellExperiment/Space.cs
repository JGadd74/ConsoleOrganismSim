using System;
using System.Collections.Generic;
using System.Linq;

namespace CellExperiment
{
    public class SpaceEventArgs
    {
        public Point point;
        public SpaceEventArgs(Point p)
        {
            point = p;
        }
    }
    public class Space 
    {
        private protected void OnSpaceOccupied(SpaceEventArgs e)
        {
            if (SpaceOccupied != null) SpaceOccupied(this, e);
        }
        private protected void OnSpaceCleared(SpaceEventArgs e)
        {
            if (SpaceCleared != null) SpaceCleared(this, e);
        }
        public EventHandler<SpaceEventArgs> SpaceOccupied;
        public EventHandler<SpaceEventArgs> SpaceCleared;

        public List<Space> AdjacentSpaces = new List<Space>();
        public Space(int x, int y)
        {

            SpaceOccupied += Renderer.SpaceOccupationChanged;
            SpaceCleared += Renderer.SpaceOccupationChanged;
            Coordinate_X = x;
            Coordinate_Y = y;
            //AdjacentSpaces = InitializeAdjacentSpaces().ToList();

        }
        bool Locked ;
        bool Occupied = false;
        Cell Occupant = null;
        public Cell GetOccupant()
        {
            return Occupant;
        }
        public bool LockDown()
        {
            Space_Image = '\u2593';
            Locked = true;
            return Locked;
        }

        int Coordinate_X = 0;
        int Coordinate_Y = 0;
        public Point Getposition()
        {
            return new Point(Coordinate_X, Coordinate_Y);
        }
        public bool IsOccupied()
        {
            return Occupied;
        }

        public char Space_Image = Environment.SpaceImages[Environment.Images.Base];
        public void Occupy(Cell newOccupant)
        {
            
            Environment.AddActiveSpace(this); // how to remove an active space??
            newOccupant.SetHostSpace(this);
            Occupied = true;
            Occupant = newOccupant;
            Space_Image = newOccupant.GetOwner().GetId().ToString()[0]; //'X';
            OnSpaceOccupied(new SpaceEventArgs(Getposition()));
            
        }
        public void Clear()
        {
            Occupied = false;
            Environment.RemoveActiveSpace(this);
            Occupant = null;
            Space_Image = '\u2591';
            OnSpaceCleared(new SpaceEventArgs(Getposition()));
        }
        public bool IsLocked()
        {
            return Locked;
        }
        public bool IsWithinBounds()
        {
            return (Coordinate_X > 0 && Coordinate_X < Environment.Columns) && (Coordinate_Y > 0 && Coordinate_Y < Environment.Rows);
        }
        //public Space[] GetAdjacentSpaces(bool activeOnly = false, bool inactiveOnly = false)
        //{
        //    if (activeOnly) return (from aS in AdjacentSpaces where aS.IsOccupied() == true select aS).ToArray();
        //    else if (inactiveOnly) return (from aS in AdjacentSpaces where aS.IsLocked() == false select aS).ToArray();
        //    else return AdjacentSpaces.ToArray();
        //}
        public Space[] GetAdjacentSpaces(bool occupiedOnly = false, bool inactiveOnly = false)
        {
            if (this.IsLocked()) return null;
            List<Space> adjacentSpaces = new List<Space>();
            for (int i = 0; i < 8; i++) // check all 8 directions
            {
                var dir = (Environment.Direction)i; // direction being checked

                var AdjacentSpace = Environment.GetRelativeSpace(this, dir); // CRASH HERE

                if (AdjacentSpace == null || !AdjacentSpace.IsWithinBounds()) continue;
                else adjacentSpaces.Add(AdjacentSpace);
            }
            return adjacentSpaces.ToArray();
        }

        public int GetScore()
        {
            // score based on number of adjacent spaces have occupants
            return GetAdjacentSpaces(occupiedOnly: true).Length;
        }

        internal Point[] GetAdjacentPoints()
        {
             List<Point> adjacentPoints = new List<Point>();

            for (int i = 0; i < 8; i++) // check all 8 directions
            {
                var directionOfNewSpace = (Environment.Direction)i; // direction being checked

                Point? adjacentPoint = Environment.GetRelativeSpace(this, directionOfNewSpace) != null ? Getposition() : null;
                if (adjacentPoint != null)
                {
                    adjacentPoints.Add(adjacentPoint); // add space to list of inhabitedSpaces
                }
                else return null;
            }
            return adjacentPoints.ToArray();
        }
    }
}
