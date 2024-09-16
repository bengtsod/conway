using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Utils;
using Random = UnityEngine.Random;

namespace Misc
{
    public class Generation
    {
        private readonly bool[,] _isAlive;
        private string _code;

        public Generation(int rows, int columns)
        {
            _isAlive = new bool[rows, columns];
            GridUtil.ForEach(rows, columns, (row, column) => SetAlive(row, column, false));
        }

        public void SpawnCells(int spawnPercentage)
        {
            _code = null;
            GridUtil.ForEach(GetRows(), GetColumns(),
                (row, column) => SetAlive(row, column, Random.Range(0, 100) < spawnPercentage));
        }

        public Generation CalculateNextGeneration()
        {
            var nextGeneration = new Generation(GetRows(), GetColumns());

            GridUtil.ForEach(GetRows(), GetColumns(), (row, column) =>
            {
                var count = new[]
                {
                    GetIsAliveAtPositionOrFalseIfNonExistent(row - 1, column - 1),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row - 1, column),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row - 1, column + 1),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row, column - 1),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row, column + 1),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row + 1, column - 1),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row + 1, column),
                    GetIsAliveAtPositionOrFalseIfNonExistent(row + 1, column + 1),
                }.Count(value => value);

                if (IsAlive(row, column))
                {
                    nextGeneration.SetAlive(row, column, count is 2 or 3);
                }
                else
                {
                    nextGeneration.SetAlive(row, column, count is 3);
                }
            });

            return nextGeneration;
        }

        private bool GetIsAliveAtPositionOrFalseIfNonExistent(int row, int column)
        {
            return row >= 0 &&
                   column >= 0 &&
                   row < GetRows() &&
                   column < GetColumns() &&
                   IsAlive(row, column);
        }

        public bool IsAlive(int row, int column)
        {
            return _isAlive[row, column];
        }

        public void SetAlive(int row, int column, bool isAlive)
        {
            _code = null;
            _isAlive[row, column] = isAlive;
        }

        public int GetRows()
        {
            return _isAlive.GetLength(0);
        }

        public int GetColumns()
        {
            return _isAlive.GetLength(1);
        }

        public string GetCode()
        {
            return _code ??= ToCode(this);
        }

        public static Generation FromCode(string code)
        {
            var bytes = Convert.FromBase64String(code);
            using (var memoryStream = new MemoryStream(bytes, 0, bytes.Length))
            {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Position = 0;
                var restored = (bool[,])binaryFormatter.Deserialize(memoryStream);
                var generation = new Generation(restored.GetLength(0), restored.GetLength(1));
                GridUtil.ForEach(generation.GetRows(), generation.GetColumns(),
                    (row, column) => generation._isAlive[row, column] = restored[row, column]);
                return generation;
            }
        }

        public static string ToCode(Generation generation)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, generation._isAlive);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}