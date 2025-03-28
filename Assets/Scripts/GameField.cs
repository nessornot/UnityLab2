using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    public GameObject cellViewPrefab;
    public List<Cell> cells = new();
    private Dictionary<Cell, CellView> cellViews = new();
    public Text scoreText;
    public Text bestText;

    public int score = 0;
    public int bestScore = 0;

    public Vector2Int GetEmptyPosition()
    {
        List<Vector2Int> emptyPos = new();

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Vector2Int pos = new(y, x);
                bool occup = false;

                foreach (Cell cell in cells)
                {
                    if (cell.Coords == pos)
                    {
                        occup = true;
                        break;
                    }
                }

                if (!occup) { emptyPos.Add(pos); }
            }
        }

        if (emptyPos.Count == 0)
        {
            return new Vector2Int(-1, 0);
        }

        return emptyPos[Random.Range(0, emptyPos.Count)];
    }

    public void CreateCell()
    {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x == -1) { return; }

        int val = (Random.value <= 0.2f) ? 2 : 1;

        Cell newCell = new(pos, val);
        cells.Add(newCell);

        GameObject cellViewObject = Instantiate(cellViewPrefab, transform);
        CellView cellView = cellViewObject.GetComponent<CellView>();
        cellView.Init(newCell);

        cellViews[newCell] = cellView;

        UpdateScore();
    }

    public void ProcessMove(Vector2Int direction)
    {
        bool hasMoved = false;
        Dictionary<Cell, bool> mergedFlag = new();
        foreach (Cell cell in cells)
        {
            mergedFlag[cell] = false;
        }

        if (direction.x > 0) // вправо
            cells.Sort((a, b) => b.Coords.x.CompareTo(a.Coords.x));
        else if (direction.x < 0) // влево
            cells.Sort((a, b) => a.Coords.x.CompareTo(b.Coords.x));
        else if (direction.y > 0) // вверх
            cells.Sort((a, b) => b.Coords.y.CompareTo(a.Coords.y));
        else if (direction.y < 0) // вниз
            cells.Sort((a, b) => a.Coords.y.CompareTo(b.Coords.y));

        List<Cell> cellsToRemove = new();

        foreach (Cell cell in cells)
        {
            if (cellsToRemove.Contains(cell))
                continue;

            bool moved = true;
            while (moved)
            {
                moved = false;
                Vector2Int nextPos = cell.Coords + direction;
                if (!IsInsideField(nextPos))
                    break;

                Cell other = GetCellAt(nextPos);
                if (other == null)
                {
                    cell.Coords = nextPos;
                    moved = true;
                    hasMoved = true;
                }
                else if (other.Num == cell.Num && !mergedFlag[other] && !mergedFlag[cell])
                {
                    other.Num++;
                    mergedFlag[other] = true;

                    cellsToRemove.Add(cell);
                    hasMoved = true;
                    break;
                }
                else { break; }
            }
        }

        foreach (Cell cell in cellsToRemove)
        {
            if (cellViews.TryGetValue(cell, out CellView cellView))
            {
                Destroy(cellView.gameObject);
                cellViews.Remove(cell);
            }

            cells.Remove(cell);
        }

        if (hasMoved)
        {
            CreateCell();
            UpdateScore();

            CheckGameOver();
        }
    }

    public bool IsInsideField(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 4 && pos.y >= 0 && pos.y < 4;
    }

    public Cell GetCellAt(Vector2Int pos)
    {
        return cells.Find(cell => cell.Coords == pos);
    }

    public void UpdateScore()
    {
        score = 0;
        foreach (Cell cell in cells)
        {
            score += Mathf.RoundToInt(Mathf.Pow(2, cell.Num));
        }

        scoreText.text = "Score\n" + score;

        if (score > bestScore)
        {
            bestScore = score;
            bestText.text = "Top Score\n" + bestScore;
        }

        SaveManager.SaveGame(score, bestScore, cells);
    }

    private void Awake()
    {
        CreateCell();
        CreateCell();
        UpdateScore();
    }

    private void Start() { LoadGame(); }

    private void OnApplicationQuit()
    {
        SaveManager.SaveGame(score, bestScore, cells);
    }

    public void LoadGame()
    {
        GameData data = SaveManager.LoadGame();

        if (data != null)
        {
            score = data.score;
            bestScore = data.bestScore;
            scoreText.text = "Score\n" + score;
            bestText.text = "Top Score\n" + bestScore;

            cells.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (CellData cellData in data.cells)
            {
                Cell cell = new Cell(new Vector2Int(cellData.x, cellData.y), cellData.num);
                cells.Add(cell);

                GameObject cellViewObject = Instantiate(cellViewPrefab, transform);
                CellView cellView = cellViewObject.GetComponent<CellView>();
                cellView.Init(cell);

                cellViews[cell] = cellView;
            }
        }
        else
        {
            bestScore = 0;
            bestText.text = "Top Score\n0";
        }
    }

    public bool CanMakeMove()
    {
        if (GetEmptyPosition().x != -1) { return true; }
            
        foreach (Cell cell in cells)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int dir in directions)
            {
                Cell neighbor = GetCellAt(cell.Coords + dir);
                if (neighbor != null && neighbor.Num == cell.Num)
                    return true;
            }
        }

        return false;
    }

    private void CheckGameOver()
    {
        if (!CanMakeMove())
        {
            Debug.Log("Game Over");

            if (score > bestScore)
            {
                bestScore = score;
                bestText.text = "Top Score\n" + bestScore;
            }

            SaveManager.SaveGame(score, bestScore, cells);
            RestartGame();
        }
    }

    public void RestartGame()
    {
        score = 0;
        scoreText.text = "Score\n0";

        foreach (Cell cell in cells)
        {
            if (cellViews.TryGetValue(cell, out CellView cellView))
            {
                Destroy(cellView.gameObject);
            }
        }

        cells.Clear();
        cellViews.Clear();

        SaveManager.DeleteSave();

        CreateCell();
        CreateCell();
    }
}