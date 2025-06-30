using System;
using UnityEngine;

public class TimeLeft2D : MonoBehaviour
{
    [Header("Display Settings")]
    public SpriteRenderer backgroundRenderer;
    
    [Header("Colors")]
    public Color hourColor = new Color(0.29f, 0.56f, 0.89f, 1f); // #4a90e2
    public Color minuteColor = new Color(0.31f, 0.82f, 0.76f, 1f); // #50d2c2
    public Color secondColor = new Color(0.91f, 0.30f, 0.24f, 1f); // #e74c3c
    
    [Header("Layout Settings")]
    public float hourSquareSize = 2f;
    public float minuteSquareSize = 0.6f;
    public float secondSquareSize = 0.1f;
    public int hourColumns = 4;
    public int minuteColumns = 15;
    public int secondColumns = 90;
    
    private GameObject hourContainer;
    private GameObject minuteContainer;
    private GameObject secondContainer;
    
    void Start()
    {
        Debug.Log("TimeLeft2D Start() called");
        SetupCamera();
        SetupContainers();
        SetupBackground();
        
        // 1秒ごとに更新
        InvokeRepeating(nameof(UpdateDisplay), 0f, 1f);
        Debug.Log("TimeLeft2D initialization complete");
    }
    
    void SetupCamera()
    {
        // メインカメラの背景色を黒に設定
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = Color.black;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            Debug.Log("Camera background set to black");
        }
        else
        {
            Debug.LogWarning("Main camera not found!");
            // メインカメラが見つからない場合は作成
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            mainCamera.backgroundColor = Color.black;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            Debug.Log("Created new main camera with black background");
        }
    }
    
    void SetupBackground()
    {
        Debug.Log("Setting up background...");
        
        if (backgroundRenderer == null)
        {
            Debug.Log("Creating new background GameObject");
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(transform);
            bg.transform.localPosition = Vector3.zero;
            backgroundRenderer = bg.AddComponent<SpriteRenderer>();
        }
        
        // 背景レンダラーの設定
        backgroundRenderer.color = Color.black;
        backgroundRenderer.sortingOrder = -100; // 最背面に配置
        
        // 大きな背景スプライトを作成
        Texture2D bgTexture = new Texture2D(1, 1);
        bgTexture.SetPixel(0, 0, Color.white);
        bgTexture.Apply();
        
        Sprite bgSprite = Sprite.Create(bgTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        backgroundRenderer.sprite = bgSprite;
        backgroundRenderer.size = new Vector2(50, 50); // より大きなサイズ
        
        Debug.Log($"Background sprite created with size: {backgroundRenderer.size}");
        Debug.Log($"Background color: {backgroundRenderer.color}");
    }
    
    void SetupContainers()
    {
        hourContainer = new GameObject("HourContainer");
        hourContainer.transform.SetParent(transform);
        
        minuteContainer = new GameObject("MinuteContainer");
        minuteContainer.transform.SetParent(transform);
        
        secondContainer = new GameObject("SecondContainer");
        secondContainer.transform.SetParent(transform);
    }
    
    void UpdateDisplay()
    {
        DateTime now = DateTime.Now;
        DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999);
        TimeSpan timeLeft = endOfDay - now;
        
        int hoursLeft = (int)timeLeft.TotalHours;
        int minutesLeft = timeLeft.Minutes;
        int secondsLeft = timeLeft.Seconds;
        
        // 現在時刻が23時台の場合の処理
        if (hoursLeft < 0)
        {
            hoursLeft = 0;
            minutesLeft = 0;
            secondsLeft = 0;
        }
        
        // デバッグ情報を出力
        Debug.Log($"Current time: {now:HH:mm:ss}");
        Debug.Log($"Time left: {hoursLeft}h {minutesLeft}m {secondsLeft}s");
        Debug.Log($"Total seconds left: {timeLeft.TotalSeconds}");
        
        ClearContainers();
        DrawHours(hoursLeft);
        DrawMinutes(minutesLeft, hoursLeft);
        DrawSeconds(secondsLeft, hoursLeft, minutesLeft);
    }
    
    void ClearContainers()
    {
        foreach (Transform child in hourContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in minuteContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in secondContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    
    void DrawHours(int hoursLeft)
    {
        Debug.Log($"Drawing {hoursLeft} hour squares");
        
        if (hoursLeft <= 0)
        {
            Debug.Log("No hours to draw");
            return;
        }
        
        for (int i = 0; i < hoursLeft; i++)
        {
            int row = i / hourColumns;
            int col = i % hourColumns;
            
            Vector3 position = new Vector3(
                col * hourSquareSize - 2f, // 中央寄りに調整
                6f - row * hourSquareSize,  // 上部から配置
                0
            );
            
            GameObject hourSquare = CreateSprite($"Hour_{i}", hourColor, hourSquareSize);
            hourSquare.transform.SetParent(hourContainer.transform);
            hourSquare.transform.localPosition = position;
            
            Debug.Log($"Hour {i} created at position: {position}");
        }
        Debug.Log($"Hour squares created: {hourContainer.transform.childCount}");
    }
    
    void DrawMinutes(int minutesLeft, int hoursLeft)
    {
        // 時間の行数を計算（最低1行は確保）
        int hourRows = Math.Max(1, (hoursLeft + hourColumns - 1) / hourColumns);
        float minuteStartY = 8f - hourRows * hourSquareSize - 0.5f; // 時間エリアの下に配置
        
        Debug.Log($"Drawing {minutesLeft} minute squares at Y: {minuteStartY}");
        
        for (int i = 0; i < minutesLeft; i++)
        {
            int row = i / minuteColumns;
            int col = i % minuteColumns;
            
            GameObject minuteSquare = CreateSprite($"Minute_{i}", minuteColor, minuteSquareSize);
            minuteSquare.transform.SetParent(minuteContainer.transform);
            minuteSquare.transform.localPosition = new Vector3(
                col * minuteSquareSize - 4.5f,
                minuteStartY - row * minuteSquareSize,
                0
            );
        }
        Debug.Log($"Minute squares created: {minuteContainer.transform.childCount}");
    }
    
    void DrawSeconds(int secondsLeft, int hoursLeft, int minutesLeft)
    {
        // 時間の行数を計算
        int hourRows = Math.Max(1, (hoursLeft + hourColumns - 1) / hourColumns);
        float minuteStartY = 8f - hourRows * hourSquareSize - 0.5f;
        
        // 分の行数を計算
        int minuteRows = Math.Max(1, (minutesLeft + minuteColumns - 1) / minuteColumns);
        float secondStartY = minuteStartY - minuteRows * minuteSquareSize - 0.3f; // 分エリアの下に配置
        
        Debug.Log($"Drawing {secondsLeft} second squares at Y: {secondStartY}");
        
        for (int i = 0; i < secondsLeft; i++)
        {
            int row = i / secondColumns;
            int col = i % secondColumns;
            
            GameObject secondSquare = CreateSprite($"Second_{i}", secondColor, secondSquareSize);
            secondSquare.transform.SetParent(secondContainer.transform);
            secondSquare.transform.localPosition = new Vector3(
                col * secondSquareSize - 4.5f,
                secondStartY - row * secondSquareSize,
                0
            );
        }
        Debug.Log($"Second squares created: {secondContainer.transform.childCount}");
    }
    
    GameObject CreateSprite(string name, Color color, float size)
    {
        GameObject square = new GameObject(name);
        SpriteRenderer renderer = square.AddComponent<SpriteRenderer>();
        
        // 1x1の白いテクスチャを作成
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.size = new Vector2(size, size);
        
        return square;
    }
    
    void OnDestroy()
    {
        CancelInvoke();
    }
}
