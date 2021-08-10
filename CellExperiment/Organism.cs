using System;
using System.Collections.Generic;
using System.Linq;

namespace CellExperiment
{

    public class Organism
    {
        int Id = 0;
        public int GetId()
        {
            return Id;
        }


        public Organism(bool spawnOnCreation = false)
        {
            Id = new Random().Next(0, 100);
            if (spawnOnCreation) SpawnAtRandom();
        }

        public int Energy = 1;

        public List<Cell> Body = new List<Cell>();
        public void SpawnAtRandom()
        {
            bool CellSpawned = false;
            while (!CellSpawned) // keep trying until a suitable space is found and occupied by new cell
            {
                int x = new Random().Next(1, Environment.Columns - 2);
                int y = new Random().Next(1, Environment.Rows - 1);
                var newPosAsString = Environment.ConvertPointToString(new Point(x, y));

                if (!Environment.BoardSpaceIsActive(newPosAsString))
                {
                    Cell newBoi = new Cell(this);

                    if (!Environment.Board[x, y].IsLocked()) Environment.Board[x, y].Occupy(newBoi);
                    Body.Add(newBoi);
                    CellSpawned = true;
                }
            }


        }
        public bool SpawnAtPoint(Point point)
        {
            bool CellSpawned = false;

            var x = point.getX();
            var y = point.getY();
            var newPointAsString = Environment.ConvertPointToString(new Point(x, y));

            if (!Environment.BoardSpaceIsActive(newPointAsString))
            {
                Cell newBoi = new Cell(this);

                if (!Environment.Board[x, y].IsLocked())
                    Environment.Board[x, y].Occupy(newBoi);
                else return CellSpawned;


                Body.Add(newBoi);
                CellSpawned = true;
                return CellSpawned;
            }
            return false;


        }
        public void MoveAllCellsTEST()
        {
            foreach (var c in Body)
            {
                var randomDirection = Environment.GetRandomDirection();
                c.Move(randomDirection, this);
            }
        }
        public void Eat(Cell foodSource)
        {
            AddEnergy(foodSource.GetEnergy());
            foodSource.Kill();
        }
        public void Move(Environment.Direction direction, bool testing = false)
        {
            if (testing) direction = Environment.GetRandomDirection();

            foreach (var c in Body)
            {
                c.Move(direction, this);
            }

        }
        public void MoveComplex(Environment.Direction direction)
        {

            var possibleDirections = new List<Environment.Direction>();



            foreach (Cell c in Body)
            {
                Space newspace = Environment.GetRelativeSpace(c.GetHostSpace(), direction);
                if (newspace == null || newspace.IsOccupied()) continue;
                bool safeToMove = false;



                if (newspace.GetAdjacentSpaces().Any(s => s.IsOccupied()) && newspace.GetAdjacentSpaces(occupiedOnly: true).Length == 8) safeToMove = true;

                if (safeToMove) c.Move(direction, this);

            }
        }
        public void MoveCellAlongBody(Cell mover)
        {

        }
        public int Grow()
        {
            Cell newCell = new Cell(this);
            if (SpawnOnBody(newCell)) Body.Add(newCell);
            return Body.Count;
        }
        public bool SpawnOnBody(Cell newBodyPart)
        {
            List<Space> BuddingPoints = new List<Space>();

            foreach (var cell in Body)
            {
                // get a full list of every possible adjacent space to every cell
                BuddingPoints.AddRange(cell.GetHostSpace().GetAdjacentSpaces(inactiveOnly: true));
            }
            //remove duplicates
            BuddingPoints = BuddingPoints.Distinct<Space>().ToList();

            // fail if there are no possible places
            if (BuddingPoints.Count == 0) return false;


            if (BuddingPoints.Any(s => s.GetAdjacentSpaces(occupiedOnly: true).Length > 1))
            {
                BuddingPoints = (from bp in BuddingPoints
                                 where bp.GetAdjacentSpaces().Length > 5
                                 select bp).ToList().OrderByDescending(space => space.GetAdjacentSpaces().Length).ToList();
            }

            // random number for picking random point
            // schewed toward lower end up buddingPoints, where cells with higher numbers of active adjacent spaces
            var rando = (new Random().Next(0, BuddingPoints.Count) + new Random().Next(0,BuddingPoints.Count/2))/2;


            // a random space that falls within the list of possible spaces
            int randSpace = new Random().Next(0, BuddingPoints.Count);

            Point attachmentPoint = null;

            // pick the new attachment point
           
            attachmentPoint = BuddingPoints[randSpace].Getposition();

            // place new cell at new space
            Environment.Board[attachmentPoint.getX(), attachmentPoint.getY()].Occupy(newBodyPart);
            return true;
        }

        public bool SpawnOnBodyNew(Cell newBodyPart)
        {

            List<Space> BuddingPoints = new List<Space>();

            foreach (var cell in Body)
            {
                // get a full list of every possible adjacent space to every cell
                BuddingPoints.AddRange(cell.GetHostSpace().GetAdjacentSpaces(inactiveOnly: true));
            }
            //remove duplicates
            BuddingPoints = BuddingPoints.Distinct<Space>().ToList();

            // fail if there are no possible places
            if (BuddingPoints.Count == 0) return false;

            BuddingPoints.OrderByDescending(s => s.GetScore());


            Point attachmentPoint = null;

            // pick the new attachment point

            attachmentPoint = BuddingPoints[0].Getposition();  

            // place new cell at new space
            Environment.Board[attachmentPoint.getX(), attachmentPoint.getY()].Occupy(newBodyPart);




            return true;
        }

        public int AddEnergy(int e)
        {
            if (e > 0) Energy += e;
            return Energy;
        }

        public void AddSight()
        {
            // pick cell at random to start scan thread
        }
        Queue<Point> TrackingPath = new Queue<Point>();
        public void Track(Cell target)
        {// use this to update tracking path in various ways

            TrackingPath.Enqueue(target.GetHostSpace().Getposition());
            // to convert List back to queue
            //TrackingPath = new Queue<Point>(TrackingPath.ToList());
            // update:
            // if a new point is added, and the queue aready contains it
            // remove points between the first, and second instance
            // of that point
            // convert to List for .IndexOf, convert back after alteration
        }
    }
}
