using UnityEngine;

namespace Entities.Players
{
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector] public Vector2 look;
        [HideInInspector] public Vector2 move;
        [HideInInspector] public bool jumping;
        [HideInInspector] public bool crouching;
        [HideInInspector] public bool sprinting;

        private void Update()
        {
            look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            jumping = Input.GetKey(KeyCode.Space);
            crouching = Input.GetKey(KeyCode.LeftControl);
            sprinting = Input.GetKey(KeyCode.LeftShift);
        }
    }
}