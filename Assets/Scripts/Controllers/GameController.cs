using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Misc;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        public enum GameState
        {
            Uninitialized,Paused,Playing
        }

        public static GameController Instance { private set; get; }

        [SerializeField] private GameObject cellPrefab;

        public GameState State { get; private set; } = GameState.Uninitialized;

        private const float CellSize = 1f;

        private IEnumerator _playCoroutine;

        private Cell[,] _cells;

        private List<Generation> _generations = new();

        private int _generationRepetitionCount;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Cell.CellChanged += (row, column) =>
            {
                ClearRepetitionCount();
                _generations = new List<Generation> { GetLastGeneration() };

                var isAlive = !GetLastGeneration().IsAlive(row, column);
                GetLastGeneration().SetAlive(row, column, isAlive);
                _cells[row,column].SetAlive(isAlive);
            };
        }

        private void ClearRepetitionCount()
        {
            _generationRepetitionCount = -1;
        }

        public void Initialize(int rows, int columns)
        {
            State = GameState.Paused;

            if (_cells != null)
            {
                GridUtil.ForEach(_cells.GetLength(0), _cells.GetLength(1),
                    (row, column) => Destroy(_cells[row, column].gameObject));
            }

            _cells = new Cell[rows, columns];

            GridUtil.ForEach(rows, columns, (row,column) =>
            {
                var position = new Vector2((row - rows / 2f) * CellSize, (column - columns / 2f) * CellSize);
                var newCell = Instantiate(cellPrefab, position, Quaternion.identity);

                _cells[row, column] = newCell.GetComponent<Cell>();
                _cells[row, column].SetRowAndColumn(row, column);
                _cells[row, column].SetAlive(false);
            });

            _generations = new List<Generation> { new(rows, columns) };
        }

        public void Load(Generation generation)
        {
            ClearRepetitionCount();
            Initialize(generation.GetRows(), generation.GetColumns());
            GridUtil.ForEach(generation.GetRows(), generation.GetColumns(), (row, column) =>
            {
                SetAlive(row, column, generation.IsAlive(row, column));
                GetLastGeneration().SetAlive(row, column, generation.IsAlive(row, column));
            });
        }

        private void SetAlive(int row, int column, bool isAlive)
        {
            ClearRepetitionCount();
            _cells[row,column].SetAlive(isAlive);
        }

        public void SpawnCells(int spawnPercentage)
        {
            ClearRepetitionCount();
            _generations = new List<Generation> { GetLastGeneration() };

            GetLastGeneration().SpawnCells(spawnPercentage);
            GridUtil.ForEach(GetLastGeneration().GetRows(), GetLastGeneration().GetColumns(),
                (row,column) => SetAlive(row, column, GetLastGeneration().IsAlive(row,column)));
        }

        public void Play(float generationsPerSecond)
        {
            State = GameState.Playing;
            _playCoroutine = PlayCoroutine(generationsPerSecond);
            StartCoroutine(_playCoroutine);
        }

        private IEnumerator PlayCoroutine(float secondsPerGeneration)
        {
            while (true) {
                GotoNextGeneration();
                yield return new WaitForSeconds(secondsPerGeneration);
            }
        }

        public void Pause()
        {
            State = GameState.Paused;
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        public void GotoNextGeneration()
        {
            var generation = GetLastGeneration();
            var nextGeneration = generation.CalculateNextGeneration();
            _generations.Add(nextGeneration);

            GridUtil.ForEach(generation.GetRows(), generation.GetColumns(),
                (row, column) => SetAlive(row, column, nextGeneration.IsAlive(row, column)));

            _generationRepetitionCount =
                RepetitionUtil.GetRepetitionCount(_generations.ConvertAll(entry => entry.GetCode()));
        }

        public bool HasPreviousGeneration()
        {
            return _generations.Count > 1;
        }

        public void GotoPreviousGeneration()
        {
            ClearRepetitionCount();
            _generations.Remove(GetLastGeneration());
            GridUtil.ForEach(GetLastGeneration().GetRows(), GetLastGeneration().GetColumns(),
                (row,column) => SetAlive(row, column, GetLastGeneration().IsAlive(row,column)));
        }

        public Generation GetLastGeneration()
        {
            return _generations.Last();
        }

        public int GetGenerationRepetitionCount()
        {
            return _generationRepetitionCount;
        }
    }
}