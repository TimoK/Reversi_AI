﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReversiAI.Evolutionary_Algorithm;

namespace ReversiAI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RunGame();
        }

        static void RunGame()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ReversiGame game = new ReversiGame();

            Application.Run(new ReversiVisualiser(game));
        }

        static void RunGA()
        {
            GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(new PercentMutater(0.2), new TournamentSelecter(), new UniformCrossover(), OptimisationOption.ScoreWeights);

            Solution bestSolution = geneticAlgorithm.performGA();

            bool firstValue = true;
            Console.Write("{");
            foreach (double value in bestSolution.values)
            {
                if (!firstValue)
                {
                    Console.Write(", ");
                }
                else firstValue = false;
                Console.Write(value.ToString().Replace(',', '.'));
            } 
            Console.WriteLine("}");
            Console.ReadLine();
        }
    }
}
