using System.Text;

namespace Helpers
{
    public static class NumbersHelper
    {
        public static int GetProbability(int value)
        {
            int probability = value switch
            {
                2 or 12 => 1,
                3 or 11 => 2,
                4 or 10 => 3,
                5 or 9 => 4,
                6 or 8 => 5,
                _ => 0
            };

            return probability;
        }

        public static string GetProbabilityString(int probability)
        {
            var probabilityString = new StringBuilder();

            for (int i = 0; i < probability; i++)
            {
                probabilityString.Append('.');
            }
            
            return probabilityString.ToString();
        }
    }
}