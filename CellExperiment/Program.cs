using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;




namespace CellExperiment
{
    class Program
    {


        // next work on seed() and divide()     NOT READY FOR ALL THAT YET  
        // Still need to focus on refining growing and moving

        // Update to reduce overhead
        // Function to update Space.GetAdjacentSpaces
        // -takes lots of time to get these spaces
        // -spaces are unchanging in position so this is inefficient
        // Replacement: 
        // At start up, load each space with it's adjacentSpaces
        // store in Space as Space[] AdjacentSpaces
        //



        static void Main(string[] args)
        {
            // SETUP
            Console.Title = "Cellular Experiments";
            Console.SetWindowSize(Environment.Columns + 5, Environment.Rows + 2);
            Console.SetWindowPosition(0, 0);
            Environment.Init_Board();
           

            Renderer.Build_Board_Image();
            Renderer.PrintBoard(true);
            Environment.SIMSPEED = 0;
            //Renderer.StartRenderThreadNew(Environment.SIMSPEED);

            // /SETUP

            List<Organism> LifeForms = new List<Organism>();

            for (int i = 0; i < 2; i++)
            {
                LifeForms.Add(new Organism(true));
            } // how many cells to spawn in TESTING

            long cntr = 0;
            for(int i = 0; i < 300; i++)
            {
                foreach (var l in LifeForms)
                {
                    //l.Grow();
                }
            }
            while (true) // test purposes.   movement
            {
                Thread.Sleep(Environment.SIMSPEED);
                cntr++;
                if (cntr < 100000000)
                {
                    foreach (var life in LifeForms)
                    {
                        //if (cntr % 1 == 0) life.Move(Environment.GetRandomDirection()) ;

                        if (cntr % 1 == 0)
                        {
                            life.Grow();
                        }

                    }
                }
            }
        }//endMain()
    }
}