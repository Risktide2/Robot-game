using UnityEngine.Events;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Settings")]
    public float FireCooldown;

    public UnityEvent OnGunShoot;

    // By defult gun is semi
    public bool Automatic;

    private float CurrentCooldown;

    void Start()
    {
        CurrentCooldown = FireCooldown;
    }

    void Update()
    {
        if (Automatic)
        {
            if (Input.GetMouseButton(0))
            {
                if (CurrentCooldown <= 0f)
                {
                    OnGunShoot?.Invoke();
                    CurrentCooldown = FireCooldown;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnGunShoot?.Invoke();
                CurrentCooldown = FireCooldown;
            }

        }

        CurrentCooldown -= Time.deltaTime;

    }
}
