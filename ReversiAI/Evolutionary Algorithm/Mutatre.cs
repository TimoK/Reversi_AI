using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiAI.Evolutionary_Algorithm
{
    public interface Mutater
    {
        Solution Mutate(Solution solutionToMutate);
    }

    public class PercentMutater : Mutater
    {
        private double percentRange;

        public PercentMutater(double percentRange)
        {
            this.percentRange = percentRange;
        }

        public Solution Mutate(Solution solutionToMutate)
        {
            Solution mutatedSolution = new Solution(solutionToMutate);
            int randomIndex = RandomGenerator.Instance.Next(mutatedSolution.values.Count);

            double currentValue = mutatedSolution.values[randomIndex];
            double rangeFromCurrentValue = currentValue * percentRange;

            mutatedSolution.values[randomIndex] = randomDoubleWithinRange(currentValue - rangeFromCurrentValue, currentValue + rangeFromCurrentValue);

            return mutatedSolution;
        }

        double randomDoubleWithinRange(double lowerBound, double upperBound)
        {
            double randomDouble = RandomGenerator.Instance.NextDouble();
            return lowerBound + (upperBound - lowerBound) * randomDouble;
        }
    }
}
