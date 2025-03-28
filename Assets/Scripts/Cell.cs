using System;
using UnityEngine;

[Serializable]
public class Cell
{
    private Vector2Int coords;
    public Vector2Int Coords
    {
        get => coords;
        set
        {
            if (value != coords)
            {
                coords = value;
                OnPositionChanged?.Invoke(coords);
            }
        }
    }

    private int num;
    public int Num
    {
        get => num;
        set
        {
            if (num != value)
            {
                num = value;
                OnValueChanged?.Invoke(num);
            }
        }
    }

    public event Action<int> OnValueChanged;
    public event Action<Vector2Int> OnPositionChanged;

    public Cell(Vector2Int pos, int val)
    {
        Coords = pos;
        num = val;
    }
}
