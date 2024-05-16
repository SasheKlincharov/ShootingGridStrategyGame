using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour {

    private static MouseWorld Instance { get; set; }


    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake() {
        Instance = this;
    }

    public static Vector3 GetPosition() {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.mousePlaneLayerMask);
        return raycastHit.point;
    }

    public static Vector3 GetPositionOnlyHitVisible() {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        RaycastHit[] raycastHitArray = Physics.RaycastAll(ray, float.MaxValue, Instance.mousePlaneLayerMask);
        // Sort by distance 
        System.Array.Sort(raycastHitArray, (RaycastHit raycastHitA, RaycastHit raycastHitB) => {
           return Mathf.RoundToInt(raycastHitA.distance - raycastHitB.distance);
        });

        foreach (RaycastHit raycastHit in raycastHitArray) {
            if (raycastHit.transform.TryGetComponent(out Renderer renderer)) {
                if (renderer.enabled == true) {
                    return raycastHit.point;
                } else {
                    // We zoom in and we hid it
                }
            }
        }
        return Vector3.zero;
    }

}

