using System;

namespace Utils
{
    public static class GridUtil
    {
        public static void ForEach(int rows, int columns, Action<int, int> action)
        {
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    action(row, column);
                }
            }
        }
    }
}