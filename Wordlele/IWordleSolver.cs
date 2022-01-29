using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordlele
{
    internal interface IWordleSolver
    {
        /// <summary>
        /// Reset the internal state for a new game
        /// </summary>
        void Reset();

        /// <summary>
        /// Generate first guess
        /// </summary>
        /// <returns></returns>
        string GenerateGuess();

        /// <summary>
        /// Stateful method
        /// Generate next guess based on internal state and previous guess outcome
        /// </summary>
        /// <param name="lastState"></param>
        /// <returns></returns>
        string GenerateGuess(int[] lastState);

        /// <summary>
        /// Stateless method
        /// Generate next guess based on previous outcomes
        /// </summary>
        /// <param name="lastStates"></param>
        /// <returns></returns>
        string GenerateGuess(List<int[]> lastStates, List<string> previouesGuesses);
    }
}
