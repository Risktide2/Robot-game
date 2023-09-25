// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private const float Threshold = 0.01f;

    [Header("References")]
    public Transform playerCam;
    public Transform orientation;
    
    [Header("Settings")]
    public LayerMask whatIsGround;
    public float maxSlopeAngle = 35f;

    [Header("Running")]
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public float counterMovement = 0.175f;
    
    [Header("Crouching")]
    [SerializeField] private float slideForce = 400;
    [SerializeField] private  float slideCounterMovement = 0.2f;
    
    [Header("Jumping")]
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float jumpForce = 550f;
    
    [Header("Looking")]
    [SerializeField] private float sensitivity = 50f;
    [SerializeField] private float sensMultiplier = 1f;
    
    //General movement
    private bool _grounded;

    //Crouch & Slide
    private Vector3 _crouchScale = new(1, 0.5f, 1);
    private Vector3 _playerScale;

    //Jumping
    private bool _readyToJump = true;

    //Input
    private float _x, _y;
    private bool _jumping, _sprinting, _crouching;

    //Sliding
    private Vector3 _normalVector = Vector3.up;
    private Vector3 _wallNormalVector;
    
    private Rigidbody _rb;
    private float _xRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _playerScale = transform.localScale;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        _x = Input.GetAxisRaw("Horizontal");
        _y = Input.GetAxisRaw("Vertical");
        _jumping = Input.GetButton("Jump");
        _crouching = Input.GetKey(KeyCode.LeftControl);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = _crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (_rb.velocity.magnitude > 0.5f)
            if (_grounded)
                _rb.AddForce(orientation.transform.forward * slideForce);
    }

    private void StopCrouch()
    {
        transform.localScale = _playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        //Extra gravity
        _rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        var mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(_x, _y, mag);

        //If holding jump && ready to jump, then jump
        if (_readyToJump && _jumping) Jump();

        //Set max speed
        var maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (_crouching && _grounded && _readyToJump)
        {
            _rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (_x > 0 && xMag > maxSpeed) _x = 0;
        if (_x < 0 && xMag < -maxSpeed) _x = 0;
        if (_y > 0 && yMag > maxSpeed) _y = 0;
        if (_y < 0 && yMag < -maxSpeed) _y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!_grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (_grounded && _crouching) multiplierV = 0f;

        //Apply forces to move player
        _rb.AddForce(orientation.transform.forward * _y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        _rb.AddForce(orientation.transform.right * _x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (_grounded && _readyToJump)
        {
            _readyToJump = false;

            //Add jump forces
            _rb.AddForce(Vector2.up * jumpForce * 1.5f);
            _rb.AddForce(_normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            var vel = _rb.velocity;
            if (_rb.velocity.y < 0.5f)
                _rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (_rb.velocity.y > 0)
                _rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    private float _desiredX;

    private void Look()
    {
        var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        var rot = playerCam.transform.localRotation.eulerAngles;
        _desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(_xRotation, _desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, _desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!_grounded || _jumping) return;

        //Slow down sliding
        if (_crouching)
        {
            _rb.AddForce(moveSpeed * Time.deltaTime * -_rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if ((Math.Abs(mag.x) > Threshold && Math.Abs(x) < 0.05f) || (mag.x < -Threshold && x > 0) ||
            (mag.x > Threshold && x < 0))
            _rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        if ((Math.Abs(mag.y) > Threshold && Math.Abs(y) < 0.05f) || (mag.y < -Threshold && y > 0) ||
            (mag.y > Threshold && y < 0))
            _rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt(Mathf.Pow(_rb.velocity.x, 2) + Mathf.Pow(_rb.velocity.z, 2)) > maxSpeed)
        {
            var fallspeed = _rb.velocity.y;
            var n = _rb.velocity.normalized * maxSpeed;
            _rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        var lookAngle = orientation.transform.eulerAngles.y;
        var moveAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;

        var u = Mathf.DeltaAngle(lookAngle, moveAngle);
        var v = 90 - u;

        var magnitue = _rb.velocity.magnitude;
        var yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        var xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        var angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool _cancellingGrounded;
    internal bool Freeze;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        var layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (var i = 0; i < other.contactCount; i++)
        {
            var normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                _grounded = true;
                _cancellingGrounded = false;
                _normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        var delay = 3f;
        if (!_cancellingGrounded)
        {
            _cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        _grounded = false;
    }
}