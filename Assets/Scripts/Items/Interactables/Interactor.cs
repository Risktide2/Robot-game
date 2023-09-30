using UnityEngine;

namespace Items.Interactables
{
    /// <summary>
    /// Raycasts for Interactables, interact with E
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        public Transform raycastPoint;
        public float interactRange;

        void Update()
        {
            Ray ray = new Ray(raycastPoint.position, raycastPoint.forward);
            if (Input.GetKeyDown(KeyCode.E) &&
                Physics.Raycast(ray, out RaycastHit hitInfo, interactRange) &&
                hitInfo.collider.gameObject.TryGetComponent(out Interactable interactObj))
            {
                interactObj.Interact();
            }
        }
    }
}