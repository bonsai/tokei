using UnityEngine;

public class TimeLeftManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject timeLeftDisplayPrefab;
    
    void Start()
    {
        // カメラの設定
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        
        mainCamera.backgroundColor = Color.black;
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        
        // TimeLeftDisplayの作成
        if (timeLeftDisplayPrefab == null)
        {
            GameObject displayObj = new GameObject("TimeLeftDisplay");
            displayObj.AddComponent<TimeLeftDisplay>();
        }
        else
        {
            Instantiate(timeLeftDisplayPrefab);
        }
    }
}
