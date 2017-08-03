using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiAI.Evolutionary_Algorithm
{
    public interface Crossover
    {
        Solution Crossover(Solution parent1, Solution parent2);
    }

    public class UniformCrossover : Crossover
    {
        public Solution Crossover(Solution parent1, Solution parent2)
        {
            Solution childSolution = new Solution(parent1);
            for (int i = 0; i < childSolution.values.Count; ++i)
            {
                if (RandomGenerator.Instance.Next(2) == 1)
                    childSolution.values[i] = parent2.values[i];
            }
            return childSolution;
        }
    }
}
