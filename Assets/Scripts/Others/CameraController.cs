using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    public const float MIN_FOLLOW_Y_OFFSET = 2f;
    public const float MAX_FOLLOW_Y_OFFSET = 32f;

    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void UnitActionSystem_OnSelectedUnitChange(object sender, System.EventArgs e) {
        SetHighCameraHeight();
    }

    private void Update() {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement() {
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector();

        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
        float moveSpeed = 10f;
        transform.position += moveVector * Time.deltaTime * moveSpeed;
    }

    private void HandleRotation() {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * Time.deltaTime * rotationSpeed;
    }

    private void HandleZoom() {
        // Modify this value to increase the strength of zooming :)
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

    public float GetCameraHeight() {
        return targetFollowOffset.y;
    }

    public void SetLowCameraHeight() {
        targetFollowOffset.y = MIN_FOLLOW_Y_OFFSET;
    }
    public void SetHighCameraHeight() {
        targetFollowOffset.y = MAX_FOLLOW_Y_OFFSET;
    }
}
