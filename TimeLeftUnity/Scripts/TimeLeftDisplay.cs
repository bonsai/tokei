using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLeftUI : MonoBehaviour
{
    [Header("UI Setup")]
    public Canvas canvas; // Assign or it will be created

    [Header("Colors")]
    public Color hourColor = new Color(0.29f, 0.56f, 0.89f, 1f); // #4a90e2
    public Color minuteColor = new Color(0.31f, 0.82f, 0.76f, 1f); // #50d2c2
    public Color secondColor = new Color(0.91f, 0.30f, 0.24f, 1f); // #e74c3c
    public Color backgroundColor = Color.black;
    
    [Header("Layout Settings")]
    public Vector2 hourSquareSize = new Vector2(200, 200);
    public Vector2 minuteSquareSize = new Vector2(60, 60);
    public Vector2 secondSquareSize = new Vector2(10, 10);
    public int hourColumns = 4;
    public int minuteColumns = 15;
    public int secondColumns = 90;

    private List<GameObject> hourSquares = new List<GameObject>();
    private List<GameObject> minuteSquares = new List<GameObject>();
    private List<GameObject> secondSquares = new List<GameObject>();
    
    void Start()
    {
        SetupCanvas();
        
        // オブジェクトプールを作成
        float hourBlockHeight = CreateSquarePool("HourContainer", hourSquares, 24, hourColor, hourSquareSize, hourColumns, Vector2.zero);
        float minuteBlockHeight = CreateSquarePool("MinuteContainer", minuteSquares, 60, minuteColor, minuteSquareSize, minuteColumns, new Vector2(0, -hourBlockHeight));
        CreateSquarePool("SecondContainer", secondSquares, 60, secondColor, secondSquareSize, secondColumns, new Vector2(0, -hourBlockHeight - minuteBlockHeight));
        
        InvokeRepeating(nameof(UpdateDisplay), 0f, 1f);
    }
    
    void SetupCanvas()
    {
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TimeLeftCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject background = new GameObject("Background");
        background.transform.SetParent(canvas.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = backgroundColor;
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgRect.SetAsFirstSibling(); // Draw background behind everything
    }
    
    float CreateSquarePool(string containerName, List<GameObject> pool, int maxCount, Color color, Vector2 size, int columns, Vector2 containerPosition)
    {
        GameObject container = new GameObject(containerName);
        container.transform.SetParent(canvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 1f); // Top-Center
        containerRect.anchorMax = new Vector2(0.5f, 1f); // Top-Center
        containerRect.pivot = new Vector2(0.5f, 1f); // Top-Center
        containerRect.anchoredPosition = containerPosition;

        float totalWidth = columns * size.x;
        float startX = -totalWidth / 2f + size.x / 2f;
        int numRows = 0;

        for (int i = 0; i < maxCount; i++)
        {
            int row = i / columns;
            int col = i % columns;
            if (row + 1 > numRows) numRows = row + 1;

            GameObject square = new GameObject($"{containerName.Replace("Container", "")}_{i}");
            square.transform.SetParent(container.transform, false);
            
            Image image = square.AddComponent<Image>();
            image.color = color;

            RectTransform rect = square.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = new Vector2(startX + col * size.x, -(row * size.y) - size.y / 2f);
            
            square.SetActive(false);
            pool.Add(square);
        }

        return numRows * size.y; // Return total height of this block
    }

    void UpdateDisplay()
    {
        DateTime now = DateTime.Now;
        DateTime endOfDay = now.Date.AddDays(1).AddTicks(-1);
        TimeSpan timeLeft = endOfDay - now;

        UpdatePool(hourSquares, timeLeft.Hours);
        UpdatePool(minuteSquares, timeLeft.Minutes);
        UpdatePool(secondSquares, timeLeft.Seconds);
    }

    void UpdatePool(List<GameObject> pool, int count)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].activeSelf != (i < count))
            {
                pool[i].SetActive(i < count);
            }
        }
    }
    
    void OnDestroy()
    {
        CancelInvoke();
    }
}
