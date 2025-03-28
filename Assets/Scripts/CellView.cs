using System;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    public Text valueText;
    public Image backGround;

    public Color startColor = Color.white;
    public Color endColor = Color.red;

    public int minVal = 1;
    public int maxVal = 10;

    public void Init(Cell cell)
    {
        UpdateValue(cell.Num);

        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;

        transform.localPosition = new Vector3(-216 + cell.Coords.x * 144, 216 - cell.Coords.y * 144, 0);
    }

    public void UpdateValue(int val)
    {
        valueText.text = Math.Pow(2, val).ToString();

        float t = Mathf.InverseLerp(minVal, maxVal, val);
        backGround.color = Color.Lerp(startColor, endColor, t);
    }

    public void UpdatePosition(Vector2Int newPos)
    {
        transform.localPosition = new Vector3(-216 + newPos.x * 144, 216 - newPos.y * 144, 0);
    }

    private void Start()
    {
       backGround = GetComponent<Image>();
    }
}
