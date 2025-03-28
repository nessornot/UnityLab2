using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int score;
    public int bestScore;
    public List<CellData> cells;

    public GameData(int score, int bestScore, List<Cell> gameCells)
    {
        this.score = score;
        this.bestScore = bestScore;
        cells = new List<CellData>();

        foreach (Cell cell in gameCells)
        {
            cells.Add(new CellData(cell.Coords.x, cell.Coords.y, cell.Num));
        }
    }
}

[Serializable]
public class CellData
{
    public int x, y, num;

    public CellData(int x, int y, int num)
    {
        this.x = x;
        this.y = y;
        this.num = num;
    }
}
