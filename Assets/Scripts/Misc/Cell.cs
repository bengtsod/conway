using System;
using UnityEngine;

namespace Misc
{
    public class Cell : MonoBehaviour
    {
        public static event Action<int,int> CellChanged;

        private int _row;
        private int _column;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAlive(bool alive)
        {
            _spriteRenderer.enabled = alive;
        }

        public void SetRowAndColumn(int row, int column)
        {
            _row = row;
            _column = column;
        }

        private void OnMouseDown()
        {
            CellChanged?.Invoke(_row, _column);
        }
    }
}