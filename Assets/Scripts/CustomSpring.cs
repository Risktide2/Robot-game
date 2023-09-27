using UnityEngine;

/// <summary>
/// Alternative to the unity spring joint
/// </summary>
public class CustomSpring
{
    //Settings
    public float Stiffness;
    public float DampAmount;
    public float RestLength;
    
    //State variables
    public float Length;
    public float Velocity;
    
    //Recalculates the spring force
    public void Update(float deltaTime)
    {
        float direction = Mathf.Sign(RestLength - Length);
        float restoringForce = Mathf.Abs(RestLength - Length) * Stiffness * direction;
        float dampingForce = -Velocity * DampAmount;
        Velocity += (restoringForce + dampingForce) * deltaTime;
        Length += Velocity * deltaTime;
    }

    public void Reset()
    {
        Velocity = 0f;
        Length = 0f;
    }
}