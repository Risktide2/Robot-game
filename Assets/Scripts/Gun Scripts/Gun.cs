using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// Its a gun, uses raycasting to check hits.
/// </summary>
public class Gun : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private float damage = 20;
    [SerializeField] private float bulletRange = 1000;
    [SerializeField] private bool automatic = true; // By default gun is semi
    
    [Header("References")]
    [SerializeField]  Transform playerCamera;
    
    public UnityEvent onGunShoot;
    
    private float _currentCooldown;

    private void Update()
    {
        //Wait until cooldown <= 0
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
            return;
        }
        
        //Check if firing
        bool firePressed = automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (firePressed)
        {
            Shoot();
            onGunShoot?.Invoke();
            _currentCooldown = fireCooldown;
        }
    }
    
    public void Shoot()
    {
        Ray gunRay = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, bulletRange))
            if (hitInfo.collider.gameObject.TryGetComponent(out Health enemy))
                enemy.TakeDamage(damage);
    }
}
