using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicQuiz.GUI
{
    public static class RandomExtension
    {
        public static long NextLong(this Random random, long maxValue = long.MaxValue)
        {
            return (long)(random.NextDouble() * maxValue);
        }
    }
}
