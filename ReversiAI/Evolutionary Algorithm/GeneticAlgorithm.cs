using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiAI.Evolutionary_Algorithm
{
    public class GeneticAlgorithm
    {
        public static int populationSize = 128;
        public static int selectionSize = 32;
        public static int numGenerations = 30;

        Mutater mutater;
        Selecter selecter;
        Crossover crossover;
        OptimisationOption optimisationOption;

        public GeneticAlgorithm(Mutater mutater, Selecter selecter, Crossover crossover, OptimisationOption optimisationOption)
        {
            this.mutater = mutater;
            this.selecter = selecter;
            this.crossover = crossover;
            this.optimisationOption = optimisationOption;
        }

        public Solution performGA()
        {
            List<Solution> currentPopulation = getInitialPopulation();

            for (int generationNum = 0; generationNum < numGenerations; ++generationNum)
            {
                Console.WriteLine(generationNum);

                List<Solution> selection = selecter.Select(currentPopulation, selectionSize, optimisationOption);
                List<Solution> nextPopulation = new List<Solution>();

                foreach (Solution solution in selection) nextPopulation.Add(solution);
                for (int i = 0; i < (populationSize - selectionSize) / 2; ++i)
                {
                    Solution randomSelectedSolution = selection[RandomGenerator.Instance.Next(selectionSize)];
                    Solution mutatedSolution = mutater.Mutate(randomSelectedSolution);
                    nextPopulation.Add(mutatedSolution);

                    Solution randomSelectedSolution1 = selection[RandomGenerator.Instance.Next(selectionSize)];
                    Solution randomSelectedSolution2 = selection[RandomGenerator.Instance.Next(selectionSize)];
                    Solution crossedSolution = crossover.Crossover(randomSelectedSolution1, randomSelectedSolution2);
                    nextPopulation.Add(crossedSolution);
                }
                currentPopulation = nextPopulation;
            }
            
            // Select the very best solution using our selection method, returning a list of size one
            List<Solution> finalSolutionList = selecter.Select(currentPopulation, 2, optimisationOption);
            return finalSolutionList[0];
        }

        private List<Solution> getInitialPopulation()
        {
            List<Solution> initialPopulation = new List<Solution>();
            for (int i = 0; i < populationSize / 2; ++i) initialPopulation.Add(getOriginalSolution());
            for (int i = 0; i < populationSize / 2; ++i) initialPopulation.Add(getRandomSolution());
            return initialPopulation;
        }

        private Solution getOriginalSolution()
        {
            Solution solution = new Solution();

            if (optimisationOption == OptimisationOption.TileValuations)
            {
                double[] tileValuations = { 20, -3, 11, 8, -7, -4, 1, 2, 2, -3 };
                foreach (double tileValuation in tileValuations) solution.values.Add(tileValuation);
            }
            if (optimisationOption == OptimisationOption.ScoreWeights)
            {
                double[] scoreWeights = { 10, 801.724, 382.026, 78.922, 74.396, 10 };
                foreach (double scoreWeight in scoreWeights) solution.values.Add(scoreWeight);
            }
            return solution;
        }

        private Solution getRandomSolution()
        {
            Solution randomSolution = new Solution();

            int doublesInSolution = 0;
            if (optimisationOption == OptimisationOption.TileValuations) doublesInSolution = 10;
            if (optimisationOption == OptimisationOption.ScoreWeights) doublesInSolution = 6;
            for (int i = 0; i < doublesInSolution; ++i) randomSolution.values.Add(RandomGenerator.Instance.NextDouble() * 100);

            return randomSolution;
        }
    }
    
    public enum OptimisationOption { TileValuations, ScoreWeights }

    public class Solution
    {
        public Solution()
        {
        }

        public Solution(Solution toCopy)
        {
            values = new List<double>();
            foreach (double value in toCopy.values) values.Add(value);
        }

        public List<double> values = new List<double>();
    }



    // A singleton for the random generator
    public class RandomGenerator
    {
        private Random randomGenerator = new Random();

        private static RandomGenerator instance;

        private RandomGenerator()
        {

        }

        public int Next(int i)
        {
            return randomGenerator.Next(i);
        }

        public double NextDouble()
        {
            return randomGenerator.NextDouble();
        }

        public static RandomGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RandomGenerator();
                }
                return instance;
            }
        }
    }
}
