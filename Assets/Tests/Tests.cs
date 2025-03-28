using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Tests
{
    #region Helper Methods
    private GameObject CreateMockCellViewPrefab()
    {
        var prefabObj = new GameObject();
        prefabObj.AddComponent<CellView>().valueText = prefabObj.AddComponent<Text>();
        prefabObj.AddComponent<Image>();
        return prefabObj;
    }

    private List<Cell> CreateFullCellsList()
    {
        var cells = new List<Cell>();
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                cells.Add(new Cell(new Vector2Int(x, y), 1));
        return cells;
    }
    #endregion

    #region Cell Tests
    [Test]
    public void Cell_Constructor_InitializesProperties()
    {
        var cell = new Cell(new Vector2Int(2, 3), 5);
        Assert.AreEqual(new Vector2Int(2, 3), cell.Coords);
        Assert.AreEqual(5, cell.Num);
    }

    [Test]
    public void Cell_CoordsChange_TriggersEvent()
    {
        var cell = new Cell(Vector2Int.zero, 1);
        bool triggered = false;
        cell.OnPositionChanged += _ => triggered = true;

        cell.Coords = Vector2Int.one;
        Assert.IsTrue(triggered);
    }
    #endregion

    #region CellView Tests
    [Test]
    public void CellView_UpdateValue_SetsCorrectTextAndColor()
    {
        var cellViewObj = new GameObject();
        var cellView = cellViewObj.AddComponent<CellView>();
        cellView.valueText = cellViewObj.AddComponent<Text>();
        cellView.backGround = cellViewObj.AddComponent<Image>();
        cellView.minVal = 1;
        cellView.maxVal = 10;
        cellView.startColor = Color.white;
        cellView.endColor = Color.red;

        cellView.UpdateValue(5);

        Assert.AreEqual("32", cellView.valueText.text);
        Assert.AreEqual(Color.Lerp(Color.white, Color.red, 0.444f), cellView.backGround.color);
    }
    #endregion

    #region GameField Tests
    [Test]
    public void GameField_CreateCell_AddsNewCell()
    {
        var gameField = new GameObject().AddComponent<GameField>();
        gameField.cellViewPrefab = CreateMockCellViewPrefab();

        gameField.CreateCell();
        Assert.AreEqual(1, gameField.cells.Count);
    }

    [Test]
    public void GameField_ProcessMove_MergesEqualCells()
    {
        var gameField = new GameObject().AddComponent<GameField>();
        gameField.cellViewPrefab = CreateMockCellViewPrefab();
        gameField.cells = new List<Cell> {
            new Cell(new Vector2Int(0, 0), 1),
            new Cell(new Vector2Int(1, 0), 1)
        };

        gameField.ProcessMove(Vector2Int.right);
        Assert.AreEqual(1, gameField.cells.Count);
        Assert.AreEqual(2, gameField.cells[0].Num);
    }
    #endregion

    #region SaveManager Tests
    [Test]
    public void SaveManager_SaveAndLoad_PreservesData()
    {
        var testCells = new List<Cell> { new Cell(Vector2Int.zero, 1) };
        SaveManager.SaveGame(100, 200, testCells);

        var loadedData = SaveManager.LoadGame();
        Assert.AreEqual(100, loadedData.score);
        Assert.AreEqual(1, loadedData.cells.Count);

        SaveManager.DeleteSave();
    }
    #endregion

    #region Data Tests
    [Test]
    public void GameData_ConvertsCellsCorrectly()
    {
        var cells = new List<Cell> { new Cell(new Vector2Int(1, 2), 3) };
        var gameData = new GameData(100, 200, cells);

        Assert.AreEqual(1, gameData.cells[0].x);
        Assert.AreEqual(3, gameData.cells[0].num);
    }
    #endregion

    #region InputManager Tests
    [Test]
    public void InputManager_DetectsKeyPresses()
    {
        var inputManager = new GameObject().AddComponent<InputManager>();
        inputManager.gameField = new GameObject().AddComponent<GameField>();
    }
    #endregion

    #region Cell Tests
    [Test]
    public void Cell_NumChange_SameValueDoesNotTriggerEvent()
    {
        var cell = new Cell(Vector2Int.zero, 1);
        bool triggered = false;
        cell.OnValueChanged += _ => triggered = true;

        cell.Num = 1;
        Assert.IsFalse(triggered);
    }

    [Test]
    public void Cell_CoordsChange_SameValueDoesNotTriggerEvent()
    {
        var cell = new Cell(Vector2Int.zero, 1);
        bool triggered = false;
        cell.OnPositionChanged += _ => triggered = true;

        cell.Coords = Vector2Int.zero;
        Assert.IsFalse(triggered);
    }
    #endregion

    #region GameField Tests
    [Test]
    public void GameField_GetEmptyPosition_ReturnsValidWhenEmpty()
    {
        GameField gameField = new();
        var pos = gameField.GetEmptyPosition();

        Assert.AreNotEqual(new Vector2Int(-1, 0), pos);
    }

    [Test]
    public void GameField_GetEmptyPosition_ReturnsInvalidWhenFull()
    {
        GameField gameField = new();
        gameField.cells = CreateFullCellsList();

        var pos = gameField.GetEmptyPosition();
        Assert.AreEqual(new Vector2Int(-1, 0), pos);
    }

    [Test]
    public void GameField_IsInsideField_DetectsBoundaries()
    {
        GameField gameField = new();

        Assert.IsTrue(gameField.IsInsideField(new Vector2Int(0, 0)));
        Assert.IsFalse(gameField.IsInsideField(new Vector2Int(-1, 0)));
        Assert.IsFalse(gameField.IsInsideField(new Vector2Int(4, 0)));
    }

    [Test]
    public void GameField_GetCellAt_FindsCorrectCell()
    {
        GameField gameField = new();
        var testCell = new Cell(new Vector2Int(1, 1), 1);
        gameField.cells.Add(testCell);

        Assert.AreEqual(testCell, gameField.GetCellAt(new Vector2Int(1, 1)));
    }

    [Test]
    public void GameField_UpdateScore_CalculatesCorrectly()
    {
        GameField gameField = new();
        gameField.cells.Add(new Cell(Vector2Int.zero, 1)); // 2^1 = 2
        gameField.cells.Add(new Cell(Vector2Int.one, 2));  // 2^2 = 4

        gameField.UpdateScore();
        Assert.AreEqual(6, gameField.score);
    }

    [Test]
    public void GameField_CheckGameOver_DetectsGameOver()
    {
        GameField gameField = new();
        gameField.cells = CreateFullCellsList();

        for (int i = 0; i < gameField.cells.Count; i++)
        {
            gameField.cells[i].Num = i + 1;
        }

        Assert.IsFalse(gameField.CanMakeMove());
    }
    #endregion

    #region SaveManager Tests
    [Test]
    public void SaveManager_LoadGame_ReturnsNullWhenNoFile()
    {
        if (File.Exists(SaveManager.savePath))
        {
            File.Delete(SaveManager.savePath);
        }

        Assert.IsNull(SaveManager.LoadGame());
    }
    #endregion

    #region CellView Tests
    [Test]
    public void CellView_Init_SubscribesToEvents()
    {
        var cell = new Cell(Vector2Int.zero, 1);
        var cellView = new GameObject().AddComponent<CellView>();
        cellView.valueText = new GameObject().AddComponent<Text>();
        cellView.backGround = new GameObject().AddComponent<Image>();

        cellView.Init(cell);

        cell.Num = 2;
        Assert.AreEqual("4", cellView.valueText.text);
    }

    [Test]
    public void CellView_UpdatePosition_MovesCorrectly()
    {
        var cellView = new GameObject().AddComponent<CellView>();
        var newPos = new Vector2Int(2, 3);

        cellView.UpdatePosition(newPos);

        var expectedPos = new Vector3(-216 + 2 * 144, 216 - 3 * 144, 0);
        Assert.AreEqual(expectedPos, cellView.transform.localPosition);
    }
    #endregion
}