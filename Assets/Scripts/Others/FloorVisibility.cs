using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    private Renderer[] rendererArray;
    private int floor;
    [SerializeField] private bool dynamicFloorPosition;
    [SerializeField] private List<Renderer> ignoreRendererList;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        rendererArray = GetComponentsInChildren<Renderer>(true);
    }

    private void Start() {
        floor = LevelGrid.Instance.GetFloor(transform.position);
        if (floor == 0 && !dynamicFloorPosition && TurnSystem.Instance.IsPlayerTurn()) {
            Destroy(this);
        }
    }

    private void Update() {
        if (dynamicFloorPosition) {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }
        float cameraHeight = CameraController.MAX_FOLLOW_Y_OFFSET;
        if (CameraController.Instance != null) {
            cameraHeight = CameraController.Instance.GetCameraHeight();
        }

        float floorHeightOffset = 2f;
        bool showObject = cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset;
       
        if (showObject || floor == 0) {
            Show();
        } else {
            Hide();
        }
    }

    public void Show() {
        foreach (Renderer renderer in rendererArray) {
            if (ignoreRendererList.Contains(renderer)) { continue; }
            renderer.enabled = true;
        }
    }

    public void Hide() {
        foreach (Renderer renderer in rendererArray) {
            if (ignoreRendererList.Contains(renderer)) { continue; }
            renderer.enabled = false;
        }
    }
}
