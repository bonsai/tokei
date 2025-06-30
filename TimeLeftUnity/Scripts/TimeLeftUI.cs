using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TimeLeftUI : MonoBehaviour
{
    [Header("UI Setup")]
    public Canvas canvas; // Assign or it will be created
    
    [Header("Countdown Target")]
    [Tooltip("If true, counts down to end of day. If false, use targetTime")]
    public bool useEndOfDay = true;
    [Tooltip("Specific time to count down to (only used if useEndOfDay is false)")]
    public DateTime targetTime;

    [Header("Colors")]
    public Color hourColor = new Color(0.29f, 0.56f, 0.89f, 1f); // #4a90e2
    public Color minuteColor = new Color(0.31f, 0.82f, 0.76f, 1f); // #50d2c2
    public Color secondColor = new Color(0.91f, 0.30f, 0.24f, 1f); // #e74c3c
    public Color backgroundColor = Color.black;
    
    [Header("Layout Settings")]
    public Vector2 minuteContainerOffset = Vector2.zero;
    public Vector2 secondContainerOffset = Vector2.zero;
    public Vector2 hourSquareSize = new Vector2(150, 150);
    public Vector2 minuteSquareSize = new Vector2(40, 40);
    public Vector2 secondSquareSize = new Vector2(8, 8);
    public int hourColumns = 4;
    public int minuteColumns = 15;
    public int secondColumns = 60;
    
    [Header("Update Settings")]
    [Range(0.1f, 5f)]
    public float updateInterval = 1f;

    private List<GameObject> hourSquares = new List<GameObject>();
    private List<GameObject> minuteSquares = new List<GameObject>();
    private List<GameObject> secondSquares = new List<GameObject>();
    
    private int lastHours = -1;
    private int lastMinutes = -1;
    private int lastSeconds = -1;
    
    void Start()
    {
        SetupCanvas();
        CreateSquarePools();
        
        // Start updating display
        InvokeRepeating(nameof(UpdateDisplay), 0f, updateInterval);
    }
    
    void OnDestroy()
    {
        // Clean up repeating invoke to prevent memory leaks
        CancelInvoke(nameof(UpdateDisplay));
    }
    
    void SetupCanvas()
    {
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TimeLeftCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Ensure it's on top
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Ensure EventSystem exists for UI interactions
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        CreateBackground();
    }
    
    void CreateBackground()
    {
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
    
    void CreateSquarePools()
    {
        // Create object pools for each time unit with better spacing
        float hourBlockHeight = CreateSquarePool("HourContainer", hourSquares, 24, hourColor, hourSquareSize, hourColumns, new Vector2(0, -50f));
        float minuteBlockHeight = CreateSquarePool("MinuteContainer", minuteSquares, 60, minuteColor, minuteSquareSize, minuteColumns, new Vector2(0, -hourBlockHeight - 100f) + minuteContainerOffset);
        CreateSquarePool("SecondContainer", secondSquares, 60, secondColor, secondSquareSize, secondColumns, new Vector2(0, -hourBlockHeight - minuteBlockHeight - 150f) + secondContainerOffset);
        
        Debug.Log($"Layout created - Hour block height: {hourBlockHeight}, Minute block height: {minuteBlockHeight}");
    }
    
    float CreateSquarePool(string containerName, List<GameObject> pool, int maxCount, Color color, Vector2 size, int columns, Vector2 containerPosition)
    {
        if (pool == null)
        {
            Debug.LogError($"Pool for {containerName} is null!");
            return 0f;
        }
        
        GameObject container = new GameObject(containerName);
        container.transform.SetParent(canvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 1f); // Top-Center
        containerRect.anchorMax = new Vector2(0.5f, 1f); // Top-Center
        containerRect.pivot = new Vector2(0.5f, 1f); // Top-Center
        containerRect.anchoredPosition = containerPosition;

        // Calculate layout
        int numRows = Mathf.CeilToInt((float)maxCount / columns);
        float totalWidth = columns * size.x;
        float startX = -totalWidth / 2f + size.x / 2f;
        
        // Add spacing between squares
        float spacingX = size.x * 0.1f;
        float spacingY = size.y * 0.1f;

        for (int i = 0; i < maxCount; i++)
        {
            int row = i / columns;
            int col = i % columns;

            GameObject square = new GameObject($"{containerName.Replace("Container", "")}_{i}");
            square.transform.SetParent(container.transform, false);
            
            Image image = square.AddComponent<Image>();
            image.color = color;

            RectTransform rect = square.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = new Vector2(
                startX + col * (size.x + spacingX), 
                -(row * (size.y + spacingY)) - size.y / 2f
            );
            
            square.SetActive(false);
            pool.Add(square);
        }
        
        float totalHeight = numRows * (size.y + spacingY);
        Debug.Log($"{containerName} created: {maxCount} squares, {numRows} rows, height: {totalHeight}, position: {containerPosition}");

        return totalHeight;
    }

    void UpdateDisplay()
    {
        try
        {
            DateTime now = DateTime.Now;
            DateTime targetDateTime;
            
            if (useEndOfDay)
            {
                // Fixed: Use proper end of day calculation
                targetDateTime = now.Date.AddDays(1);
            }
            else
            {
                targetDateTime = targetTime;
            }
            
            TimeSpan timeLeft = targetDateTime - now;
            
            // Handle negative time (past target)
            if (timeLeft.TotalSeconds <= 0)
            {
                timeLeft = TimeSpan.Zero;
            }
            
            // Extract and bound time components
            int hours = Mathf.Clamp(timeLeft.Hours + (timeLeft.Days * 24), 0, 24);
            int minutes = Mathf.Clamp(timeLeft.Minutes, 0, 59);
            int seconds = Mathf.Clamp(timeLeft.Seconds, 0, 59);
            
            // Only update if values changed (performance optimization)
            if (hours != lastHours)
            {
                UpdateSquarePool(hourSquares, hours);
                lastHours = hours;
            }
            
            if (minutes != lastMinutes)
            {
                UpdateSquarePool(minuteSquares, minutes);
                lastMinutes = minutes;
            }
            
            if (seconds != lastSeconds)
            {
                UpdateSquarePool(secondSquares, seconds);
                lastSeconds = seconds;
            }
            
            // Debug output (remove in production)
            if (Application.isEditor)
            {
                Debug.Log($"Time Left: {hours:D2}h:{minutes:D2}m:{seconds:D2}s (H:{hours} squares, M:{minutes} squares, S:{seconds} squares)");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating display: {ex.Message}");
        }
    }

    void UpdateSquarePool(List<GameObject> pool, int count)
    {
        if (pool == null)
        {
            Debug.LogWarning("Pool is null in UpdateSquarePool!");
            return;
        }
        
        // Ensure count is within valid range
        count = Mathf.Clamp(count, 0, pool.Count);
        
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null)
            {
                bool shouldBeActive = i < count;
                if (pool[i].activeSelf != shouldBeActive)
                {
                    pool[i].SetActive(shouldBeActive);
                }
            }
            else
            {
                Debug.LogWarning($"Null object found in pool at index {i}");
            }
        }
    }
    
    // Public methods for runtime control - all with unique names
    public void SetNewTargetTime(DateTime newTarget)
    {
        targetTime = newTarget;
        useEndOfDay = false;
    }
    
    public void ToggleEndOfDayMode(bool enabled)
    {
        useEndOfDay = enabled;
    }
    
    public void ChangeUpdateInterval(float interval)
    {
        updateInterval = Mathf.Clamp(interval, 0.1f, 5f);
        CancelInvoke(nameof(UpdateDisplay));
        InvokeRepeating(nameof(UpdateDisplay), 0f, updateInterval);
    }
    
    // Utility method for getting current time remaining
    public TimeSpan GetRemainingTime()
    {
        DateTime now = DateTime.Now;
        DateTime targetDateTime = useEndOfDay ? now.Date.AddDays(1) : targetTime;
        TimeSpan timeLeft = targetDateTime - now;
        return timeLeft.TotalSeconds <= 0 ? TimeSpan.Zero : timeLeft;
    }
}