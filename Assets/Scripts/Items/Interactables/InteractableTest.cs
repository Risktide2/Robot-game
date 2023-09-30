using UnityEngine;

namespace Items.Interactables
{
    public class InteractableTest : MonoBehaviour, Interactable
    {
        public void Interact()
        {
            Debug.Log(Random.Range(0, 100));
        }
    }
}


