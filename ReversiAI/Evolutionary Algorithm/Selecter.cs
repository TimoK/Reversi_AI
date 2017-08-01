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
}
