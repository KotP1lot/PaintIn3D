using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Palete : MonoBehaviour
{
    [SerializeField] PaleteCell prefab;
    List<PaleteCell> cells;
    public event Action<Color> OnColorChanged;
    private readonly Color[] Colors = {
    Color.red,
    Color.green,
    Color.blue,
    Color.white,
    Color.black,
    Color.cyan,
    Color.magenta,
    Color.yellow,
    };
    void Start()
    {
        cells = new();
        foreach (var color in Colors)
        {
            PaleteCell cell = Instantiate(prefab, transform);
            cell.Setup(color);
            cell.OnCellClick += OnCellClickHandler;
            cells.Add(cell);
        }
        OnCellClickHandler(cells[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }
    void OnCellClickHandler(PaleteCell cell)
    {
        OnColorChanged?.Invoke(cell.color);
        foreach (var c in cells)
        {
            c.SetActive(c == cell);
        }
    }
}
