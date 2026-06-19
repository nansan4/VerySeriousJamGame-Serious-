using UnityEngine;

public static class HelperExtensions
{
    /// <summary>
    /// Steps an angle towards a target angle using a spring-damper model
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="velocity"></param>
    /// <param name="stiffness"></param>
    /// <param name="damping"></param>
    /// <param name="maxSpeed"></param>
    public static void StepSpringAngle(ref float current, float target, ref float velocity, float stiffness, float damping, float maxSpeed)
    {
        //current is the current velocity
        float error = Mathf.DeltaAngle(current, target); //this is basically the "distance" between forward and the vector pointing to the lookAt position

        //(pull towards target, higher stiffness means more aggressive pull) - (resist current motion, higher damper means stronger resistance)
        float acceleration = error * stiffness - velocity * damping;

        velocity += acceleration * Time.deltaTime; //velocity is an instance variable, this is what allows for overshoot, velocity isn't 0 when then two rotations are equal
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);

        current += velocity * Time.deltaTime; //update and normalize the small step to set new local rotation to
        current = NormalizeAngle(current);
    }

    /// <summary>
    /// Normalises a given angle to be between -180 and +180 degrees
    /// </summary>
    /// <param name="angle"></param>
    /// <returns> an angle in degrees between + and -180</returns>
    public static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }
}
