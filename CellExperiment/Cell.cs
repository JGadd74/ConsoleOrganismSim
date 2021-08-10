using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CellExperiment
{

    
    public class Cell
    {

       // testing
     
        // upon movement create cellmovementeventargs e, call OnMovedToNewSpace(e)
        //testing


        Organism Corpus;
        bool IsSensingOrgan = false;

        public Cell(Organism corpus, int startingEnergy = 1, bool sensingOrgan = false)
        {
            Id = corpus.GetId();
            Corpus = corpus;
            Energy = startingEnergy;
            IsSensingOrgan = sensingOrgan;
            //StartScanThread();
        }

        public Organism GetOwner()
        {
            return Corpus;
        } // Make private?
        public void TakeOwnership(Organism o)
        {
            if (o != Corpus)
            {
                Corpus = o;
                Id = o.GetId();
            }
        }
        private Space HostSpace = null;
        public void SetHostSpace(Space newHostSpace)
        {
            HostSpace = newHostSpace;
            if (IsSensingOrgan) StartScanThread();
        }
        public Space GetHostSpace() => HostSpace;
        int Id = 0;
        int Energy = 1;
        public int GetEnergy() => Energy;
        // Death--------------------------------------------------------------------------------------
        public void Kill()
        {
            HostSpace.Clear();
        }

        // movement-----------------------------------------------------------------------------------
        Environment.Direction DirectionOfMovement = Environment.Direction.NODIRECTION;
        public bool Move(Environment.Direction direction, Organism owner) 
        {
            if (owner != Corpus) return false;
            Space newSpace = Environment.GetRelativeSpace(HostSpace, direction);
            if (newSpace == null || direction == Environment.Direction.NODIRECTION)
            {
                return false;
            }
            else if (newSpace.IsOccupied()) return false;
            else DirectionOfMovement = direction;

            bool spaceIsLocked = newSpace.IsLocked();
            if (!spaceIsLocked)
            {
               

                var tmp  = HostSpace;
                HostSpace = newSpace;
                newSpace.Occupy(this);
                tmp.Clear();

            }


            return true;
        }
        public bool Move(Space s, Organism owner)
        {
            if (owner != Corpus) return false;

            Space newSpace = s;
            if (newSpace == null) return false;

            bool spaceIsLocked = newSpace.IsLocked();
            if (!spaceIsLocked)
            {
                var tmp = HostSpace;
                HostSpace = newSpace;
                newSpace.Occupy(this);
                tmp.Clear();
            }
            
            HostSpace.Space_Image = DetectedCells.Count.ToString()[0];
            return true;
        } // thus far unused. untested
        public Environment.Direction GetDirectionOfMovement()
        {
            return DirectionOfMovement;
        }
        public void ResetDirection()
        {
            DirectionOfMovement = Environment.Direction.NODIRECTION;
        }

        //sensing ------------------------------------------------------------------------------------
        List<Space> DetectedCells = new List<Space>();
        bool ScanContinuously = true;
        void StopScanningThread()
        {
            ScanContinuously = false;
        }
        public void StartScanThread(int dist = 10)
        {
            Thread t = new Thread(() =>
            {
                while (ScanContinuously)
                {
                    DetectedCells = ScanFar(distance: dist).ToList();
                    HostSpace.Space_Image = DetectedCells.Count.ToString()[0]; //testing
                    Thread.Sleep(Environment.SIMSPEED);
                }
            });
            t.Start();
        }
        public Space[] Scan(bool getOnlyActiveSpaces = true)
        { // expand this to check more spaces???
            // get info from surrounding spaces
            List<Space> activeSpaces = new List<Space>();
            List<Space> inactiveSpaces = new List<Space>();

            activeSpaces.AddRange(from spc in HostSpace.GetAdjacentSpaces()
                                  where spc.IsOccupied()
                                  select spc);
            inactiveSpaces.AddRange(from spc in HostSpace.GetAdjacentSpaces()
                                    where !spc.IsOccupied()
                                    select spc);
            if (!getOnlyActiveSpaces)
            {
                activeSpaces.AddRange(inactiveSpaces);
            }
            return activeSpaces.ToArray();
        }
        public Space[] ScanFar(bool getOnlyActiveSpaces = true, int distance = 2)
        {
            // this does seem to be working to see occupied spaces but not unoccuppied outside of
            // the immediately adjacent spaces??


            // crashes if cell is not spawned in

            Space[]? shortRangeSpaces = null;
            try
            { 
                shortRangeSpaces = HostSpace.GetAdjacentSpaces();
            }
            catch (System.Exception e)
            {
                return null;
            }
            if (shortRangeSpaces == null) return null;


            // foreach space in shortrangespaces, get all
            // nonoverlapping adjacent spaces
            List<Space> distantSpaces = shortRangeSpaces.ToList();
            List<Space> returnList = shortRangeSpaces.ToList();



            for (int i = 0; i <= distance; i++)
            {
                foreach (var space in shortRangeSpaces)
                {
                    var adjacents = space.GetAdjacentSpaces();
                    distantSpaces.AddRange(
                        from s in adjacents
                        where !distantSpaces.Contains(s) && s != null
                        select s);
                }
                shortRangeSpaces = distantSpaces.ToArray();
            }

            if (getOnlyActiveSpaces) distantSpaces = (from s in shortRangeSpaces
                                                      where s.IsOccupied() && s.GetOccupant().Id != this.Id
                                                      select s).ToList<Space>();



            return distantSpaces.ToArray();
        }
    }
}
