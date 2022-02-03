using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordlele
{
    public interface IWordleSolver
    {
        /// <summary>
        /// Reset the internal state for a new game
        /// </summary>
        void Reset();

        /// <summary>
        /// Returns and int representing the number of possible words
        /// </summary>
        /// <returns></returns>
        int GetPossibleGuessCount();

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
        string GenerateGuess(byte[] lastState);

        /// <summary>
        /// Stateless method
        /// Generate next guess based on previous outcomes
        /// </summary>
        /// <param name="lastStates"></param>
        /// <returns></returns>
        string GenerateGuess(List<byte[]> lastStates, List<string> previouesGuesses);
    }
}
