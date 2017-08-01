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
}
