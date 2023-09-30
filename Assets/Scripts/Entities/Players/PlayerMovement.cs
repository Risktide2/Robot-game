using System.Collections;
using UnityEngine;

namespace Entities.Players
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform playerCam;
        [SerializeField] private Transform orientation;
        [SerializeField] private PlayerInput input;

        [Header("Floor")] 
        [SerializeField] private Transform floorRaycast;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float checkRadius = 0.2f;
        [SerializeField] private float maxSlopeAngle = 35f;
        [SerializeField] private float maxFloorDistance = 0.1f;

        [Header("Running")] 
        [SerializeField] private float maxAcceleration = 10;
        [SerializeField] private float runSpeed = 10;
        [SerializeField] private float sprintSpeedMultiplier = 1.2f;
        [SerializeField] private float crouchSpeedMultiplier = 0.5f;
        [SerializeField] private float slideDownForce = 5;

        [Header("Jumping")] 
        [SerializeField] private float jumpCooldown = 0.2f;
        [SerializeField] private float jumpForce = 2;

        [Header("Looking")]
        [SerializeField] private float sensitivity = 50f;

        //Crouch
        private readonly Vector3 _crouchScale = new(1, 0.5f, 1);

        //Jumping, used for waiting jumpCooldown before being able to jump again
        private bool _jumpCoolingDown;
        private bool _cancellingGrounded;

        //General
        private Rigidbody _rb;
        private bool _grounded;
        private float _rotationX;
        private float _rotationY;
        private float _targetSpeed;
        private Vector3 _targetVelocity;
        private Vector3 _position;
        private Vector3 _velocity;
        private Vector3 _normalVector = Vector3.up;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            //Don't do anything if not grounded
            CheckGrounded();
            if (!_grounded) return;
            
            Reset();
            CheckCrouch();
            CheckJump();
            CheckRun();
            ApplyForces();
        }

        private void Update()
        {
            Look();
        }

        private void ApplyForces()
        {
            _velocity.y = 0; //Dont care about vertical velocity
            Vector3 deltaVelocity = _targetVelocity - _velocity;
            
            //Clamp the velocity to within max acceleration
            deltaVelocity /= maxAcceleration / Mathf.Max(deltaVelocity.magnitude, maxAcceleration);
            
            _rb.AddForce(deltaVelocity, ForceMode.VelocityChange);
        }

        private void Reset()
        {
            _targetVelocity = Vector3.zero;
            _position = transform.position;
            _velocity = _rb.velocity;
            
            //Calculate target speed
            float inputMultiplier = Mathf.Min(input.move.y * input.move.y + input.move.x * input.move.x, 1); //Clamp at 1
            float crouchMultiplier = input.crouching ? 1 : crouchSpeedMultiplier;
            _targetSpeed = runSpeed * crouchMultiplier * inputMultiplier;
        }

        private void CheckCrouch()
        {
            if (input.crouching)
                _targetVelocity += slideDownForce * Vector3.down;
            
            transform.localScale = input.crouching ? _crouchScale : Vector3.one;
        }

        private void CheckRun()
        {
            //Apply forces to move player
            float crouchMultiplier = input.crouching ? crouchSpeedMultiplier : 1;
            float sprintMultiplier = input.sprinting ? sprintSpeedMultiplier : 1;
            
            //Direction if player is look forward
            Vector3 forwardDirection = new Vector3(input.move.x, 0, input.move.y);
            
            //This is the direction if the ground is flat
            Vector3 flatDirection = orientation.rotation * forwardDirection;
            
            //Make the direction align with the ground direction
            Debug.DrawRay(_position, Quaternion.LookRotation(Vector3.forward, _normalVector) * Vector3.forward);
            Vector3 normalDirection = Quaternion.LookRotation(Vector3.forward, _normalVector) * flatDirection;
            
            _targetVelocity += _targetSpeed * crouchMultiplier * sprintMultiplier * normalDirection;
        }
        
        private void CheckJump()
        {
            //Add jump forces (make the normal vector affect the jump direction)
            if (!_jumpCoolingDown && input.jumping)
                _targetVelocity += jumpForce * (0.5f * Vector3.up + 0.5f * _normalVector);
        }
        
        private IEnumerator JumpCooldown()
        {
            _jumpCoolingDown = true;
            yield return new WaitForSeconds(jumpCooldown);
            _jumpCoolingDown = false;
        }
        
        /// <summary>
        /// Rotates the body and camera according to look input
        /// </summary>
        private void Look()
        {
            //Calculate current look rotations
            float magnitude = sensitivity * Time.deltaTime;
            _rotationY += playerCam.transform.localRotation.eulerAngles.y + input.look.x * magnitude;
            _rotationX = Mathf.Clamp(_rotationX - input.look.y * magnitude, -90f, 90f);

            //Perform the rotations
            playerCam.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
        }

        /// <summary>
        /// Handle ground detection
        /// </summary>
        private void CheckGrounded()
        {
            bool wasGrounded = _grounded;
            
            //Run SphereCast underneath the player
            _grounded = Physics.SphereCast(floorRaycast.position, checkRadius, -floorRaycast.up, out RaycastHit hit, maxFloorDistance, whatIsGround);
            
            if (!_grounded)
                return;
            
            _normalVector = hit.normal; 
            _grounded = Vector3.Angle(Vector3.up, _normalVector) < maxSlopeAngle;
                
            //Check if the ground state has changed
            if (_grounded && !wasGrounded) 
                StartCoroutine(JumpCooldown());
        }
    }
}