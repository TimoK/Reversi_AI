using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiAI.Evolutionary_Algorithm
{
    public interface Selecter
    {
        List<Solution> Select(List<Solution> population, int selectionSize, OptimisationOption optimisationOption);
    }

    public class TournamentSelecter : Selecter
    {
        public List<Solution> Select(List<Solution> population, int selectionSize, OptimisationOption optimisationOption)
        {
            population = Shuffle(population);
            Dictionary<Solution, AI> aiForSolution = getAIs(population, optimisationOption);
            
            while (population.Count >= selectionSize * 2)
            {
                List<Solution> halfPopulation = new List<Solution>();

                for (int i = 0; i < population.Count; i += 2)
                {
                    AI ai1 = aiForSolution[population[i]];
                    AI ai2 = aiForSolution[population[i + 1]];
                    ReversiGame tournamentGame = new ReversiGame(ai1, ai2);

                    if (tournamentGame.board.WhiteScore > tournamentGame.board.BlackScore) halfPopulation.Add(population[i]);
                    else halfPopulation.Add(population[i + 1]);
                }

                population = halfPopulation;
            }

            List<Solution> selection = new List<Solution>();
            for (int i = 0; i < selectionSize; ++i) selection.Add(population[i]);

            return selection;
        }

        private Dictionary<Solution, AI> getAIs(List<Solution> population, OptimisationOption optimisationOption)
        {
            Dictionary<Solution, AI> aisForSolution = new Dictionary<Solution, AI>();
            foreach (Solution solution in population)
            {
                double[] valuesArray = new double[solution.values.Count];
                for (int i = 0; i < solution.values.Count; ++i) valuesArray[i] = solution.values[i];

                Heuristic heuristic = null;
                if(optimisationOption == OptimisationOption.TileValuations)
                    heuristic = new DynamicHeuristic(mirroredTileValuation: valuesArray);
                if (optimisationOption == OptimisationOption.ScoreWeights)
                    heuristic = new DynamicHeuristic(scoreWeights: valuesArray);

                AI ai = new MinMax(heuristic, 1, true);

                aisForSolution.Add(solution, ai);
            }

            return aisForSolution;
        }

        private List<Solution> Shuffle(List<Solution> population)
        {
            List<Solution> shuffledList = new List<Solution>();

            for (int i = population.Count; i > 0; --i)
            {
                int randomIndex = RandomGenerator.Instance.Next(i);
                shuffledList.Add(population[randomIndex]);
                population.RemoveAt(randomIndex);
            }

            return shuffledList;
        }
    }
}
