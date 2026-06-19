using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{

    #region Vector Get Functions

    /// <summary>
    /// Gets the direction of the raycast shot from origin that hits at hit
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="origin"></param>
    /// <returns> a normalised vector that is the direction of the raycast</returns>
    public static Vector3 GetDirectionFromRaycastHit(this RaycastHit hit, Vector3 origin)
    {
        return (hit.point - origin).normalized;
    }

    /// <summary>
    /// Gets a direction from two vectors where v1 is the destination and v2 is the start
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns> a normalised vector direction</returns>
    public static Vector3 GetDirectionFromVectors(this Vector3 v1, Vector3 v2)
    {
        return (v1 - v2).normalized;
    }

    /// <summary>
    /// Gets a vector perpendicular to the input vector in the relative positive axis
    /// </summary>
    /// <param name="direction"></param>
    /// <returns> a normalised vector</returns>
    public static Vector3 GetPerpendicularVector(this Vector3 direction)
    {
        Vector3 perpendicularVector;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) || Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            perpendicularVector = Vector3.Cross(direction, Vector3.up);
        }
        else
        {
            perpendicularVector = Vector3.Cross(direction, Vector3.right);
        }

        // Normalize the result if you only need the direction (a unit vector)
        return perpendicularVector.normalized;
    }

    /// <summary>
    /// Gets a vector between two other vectors, determined by a factor between 0 and 1. <br/>
    /// A factor of 0.5 will return a vector halfway between a and b .
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="factor"></param>
    /// <returns> a normalised vector between a and b</returns>
    public static Vector3 GetVectorBetweenVectorByFactor(this Vector3 a, Vector3 b, float factor)
    {
        return Vector3.Slerp(a, b, factor).normalized;
    }

    /// <summary>
    /// Gets a random vector on the plane between two other vectors a and b. <br/>
    /// The space of possible vectors is defined by a and b and clamped further within by a [0..1] range minFactor and maxFactor
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="minFactor"></param>
    /// <param name="maxFactor"></param>
    /// <returns> a random normalised vector between two others</returns>
    public static Vector3 GetRandomVectorBetweenVectors(this Vector3 a, Vector3 b, float minFactor = 0f, float maxFactor = 1f)
    {
        //return Vector3.Slerp(a, b, Random.Range(minFactor, maxFactor)).normalized;
        return Vector3.Lerp(a, b, Random.Range(minFactor, maxFactor)).normalized;

        //Unity.Mathematics.Random r = new Unity.Mathematics.Random(42); //42 is seed
        //return Vector3.Lerp(a, b, r.NextFloat(minFactor, maxFactor)).normalized;
    }


    /// <summary>
    /// Gets a random vector in a cone defined by a center axis and a half angle, obtained by rotating center about a reoriented up by halfAngle degrees 
    /// <br>
    /// Current random seed is 42
    /// </summary>
    /// <param name="center"></param>
    /// <param name="up"></param>
    /// <param name="halfAngle"></param>
    /// <returns> a normalised vector within the cone </returns>
    public static Vector3 GetRandomVectorInCone(this Vector3 center, Vector3 up, float halfAngle, ref Unity.Mathematics.Random seed)
    {
        center = center.normalized;
        up = up.normalized;

        up = Quaternion.AngleAxis(seed.NextFloat(0f, 360f), center) * up; //rotate up vector by a random amount between 0 and 360 about the center axis
        center = Quaternion.AngleAxis(seed.NextFloat(-halfAngle, halfAngle), up) * center; //rotate center vector around rotated up vector by a random value between -halfAngle and +halfAngle

        return center.normalized; //return vector in symmetrical cone

        #region LEGACY
        //LEGACY

        //Vector3 s = GetVectorAngle(center, up, halfAngle); //get right side of cone
        //Vector3 sref = GetVectorAngle(center, up, -halfAngle); //get left side

        //Vector3 r = GetRandomVectorBetweenVectors(s, sref); //get random vector
        //Vector3 p = GetPerpendicularVector(r);

        //float sa = Vector3.Angle(r, center);
        ////Debug.Log(sa);

        //float scaledHalf = MapRange(sa, 0, halfAngle, 0f, 1f); //invert in range to get more of an X shape
        //scaledHalf = ArcEaseInOut(scaledHalf) * halfAngle;

        //float randAngle = Random.Range(-scaledHalf, scaledHalf);

        //Quaternion rot = Quaternion.AngleAxis(randAngle,p);

        //return (rot * r).normalized;
        #endregion
    }

    /// <summary>
    /// does not use seeded random
    /// </summary>
    /// <param name="center"></param>
    /// <param name="up"></param>
    /// <param name="halfAngle"></param>
    /// <param name="axisScalar"></param>
    /// <returns></returns>
    public static Vector3 GetRandomVectorInIdk(this Vector3 center, Vector3 up, float halfAngle, float axisScalar)
    {
        //define cone as symmetrical
        //apply horizontal scalar based on how close to vertical the up rotation is, where no horizontal scaling is applied if rotation is horizontal (i.e center would be rotated upwards)

        center = center.normalized;
        up = up.normalized;
        Vector3 upOrig = up;

        float angle = Random.Range(-180f, 180f);

        up = Quaternion.AngleAxis(angle, center) * up;
        float mapped = MapRange(angle, 0f, 180f, 1f, 0f);

        axisScalar = axisScalar * ArcEaseInOut(mapped); //the percentage of the axis scalar to be applied
        halfAngle = halfAngle * axisScalar;

        center = Quaternion.AngleAxis(Random.Range(-halfAngle, halfAngle), up) * center;

        return center;
    }

    /// <summary>
    /// Gets a vector direction within a pyramid defined by two vectors and a max deviation outside of the plane defined by those two vectors
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="maxDeviationPositive"></param>
    /// <param name="maxDeviationNegative"></param>
    /// <returns> a normalised vector direction within the cone </returns>
    public static Vector3 GetRandomVectorInPyramid(this Vector3 a, Vector3 b, float maxDeviationPositive = 10f, float maxDeviationNegative = -10f)
    {
        a = a.normalized;
        b = b.normalized;

        //get random vector R between a and b
        Vector3 r = GetRandomVectorBetweenVectors(a, b);

        //get R's perpendicular vector P
        Vector3 p = GetPerpendicularVector(r);

        float randomAngle = Random.Range(maxDeviationNegative, maxDeviationPositive);

        //rotate around P by random angle no greater than angle between a and b
        Quaternion rot = Quaternion.AngleAxis(randomAngle, p);

        return (rot * r).normalized;
    }

    /// <summary>
    /// Gets the vector that is deg degrees to the right of the vector vec relative to the refAxis<br/>
    /// A negative float will get a vector to the left
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="refAxis"></param>
    /// <returns> a normalised vector </returns>
    public static Vector3 GetVectorAngle(Vector3 vec, Vector3 refAxis, float deg)
    {
        return (Quaternion.AngleAxis(deg, refAxis) * vec).normalized;
    }

    #endregion

    #region Vector Operations

    /// <summary>
    /// Makes all components of the vector vec 1 or 0 while keeping the sign
    /// </summary>
    /// <param name="vec"></param>
    /// <returns> a vector where all components are 1, -1, or 0</returns>
    public static Vector2 UnitiseVectorComponents(this Vector2 vec)
    {
        return new Vector2(CustomSign(vec.x), CustomSign(vec.y));
    }

    /// <summary>
    /// Makes all components of the vector vec 1 or 0 while keeping the sign
    /// </summary>
    /// <param name="vec"></param>
    /// <returns> a vector where all components are 1, -1, or 0</returns>
    public static Vector3 UnitiseVectorComponents(this Vector3 vec)
    {
        return new Vector3(CustomSign(vec.x), CustomSign(vec.y), CustomSign(vec.z));
    }

    /// <summary>
    /// Makes all components of the vector vec 1 or 0 while keeping the sign
    /// </summary>
    /// <param name="vec"></param>
    /// <returns> a vector where all components are 1, -1, or 0</returns>
    public static Vector4 UnitiseVectorComponents(this Vector4 vec)
    {
        return new Vector4(CustomSign(vec.x), CustomSign(vec.y), CustomSign(vec.z), CustomSign(vec.w));
    }

    /// <summary>
    /// Flattens a vector onto a plane. <br/> 
    /// The normal param should be the normal vector of the plane that vec is being flattened to.
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="normal"></param>
    /// <returns> a normalised vector</returns>
    public static Vector3 FlattenVector(this Vector3 vec, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(vec, normal).normalized;
    }

    #endregion

    #region Raycast Utilities
    /// <summary>
    /// Casts a series of raycasts where the start of raycast n is the hit point of raycast n-1. <br/>
    /// The function will cast maxBounces number of raycasts or until a raycast does not hit anything.<br/>
    /// <br/><br/>
    /// <paramref name="start"/>: the origin of the initial raycast<br/>
    /// <paramref name="dir"/>: the direction of the initial raycast<br/>
    /// <paramref name="bounceDistanceMultiplier"/>: a number that is multiplied to the maxDistance after each raycast hit, so a value of 0.5f will halve the max length of a raycast after a hit.<br/>
    /// <paramref name="isRandom"/>: are the vectors chosen after each hit chosen randomly or deterministically<br/>
    /// <paramref name="factor01"/>: a factor in the range [0..1]
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dir"></param>
    /// <param name="maxDistance"></param>
    /// <param name="maxBounces"></param>
    /// <param name="bounceDistanceMultiplier"> multiplied to the maxDistance after each raycast hit</param>
    /// <param name="isRandom"></param>
    /// <param name="factor01"> a factor in the range [0..1]</param>
    /// <param name="isDebug"></param>
    /// <returns> a list of RaycastHit types</returns>
    public static List<RaycastHit> SequentialRaycast(this Vector3 start, Vector3 dir, float maxDistance, int maxBounces, float bounceDistanceMultiplier = 0.5f, bool isRandom = true, float factor01 = 0.5f, bool isDebug = false)
    {
        RaycastHit hitInfo;
        Vector3 reflectVec;
        Vector3 newVecl;
        List<RaycastHit> raycastHitList = new List<RaycastHit>();

        for (int i = 0; i <= maxBounces; i++)
        {
            if (Physics.Raycast(start, dir, out hitInfo, maxDistance))
            {
                if (hitInfo.collider != null)
                {
                    raycastHitList.Add(hitInfo);

                    reflectVec = Vector3.Reflect((hitInfo.point - start).normalized, hitInfo.normal);

                    //note that the GetRandomVectorInPyramid sequence does not use a seeded random
                    newVecl = isRandom ?
                        reflectVec.GetRandomVectorInPyramid(Vector3.ProjectOnPlane(reflectVec, hitInfo.normal).normalized, 5f, -5f)
                        : reflectVec.GetVectorBetweenVectorByFactor(Vector3.ProjectOnPlane(reflectVec, hitInfo.normal).normalized, factor01);

                    if (isDebug) Debug.DrawLine(start, hitInfo.point, Color.red);

                    maxDistance *= bounceDistanceMultiplier;
                    start = hitInfo.point;
                    dir = newVecl;
                }
            }
        }

        return raycastHitList;
    }

    /// <summary>
    /// Casts numCasts number of raycasts in a circular cone, defined by a forward, up, and deviationHalfAngle. Seeded random with default seed 42 is used.
    /// <br/><br/>
    /// <paramref name="forward"/>: the center line going down the cone<br/>
    /// <paramref name="up"/>: a vector about which forward is rotated, ideally orthogonal but not required
    /// </summary>
    /// <param name="start"></param>
    /// <param name="forward"></param>
    /// <param name="up"></param>
    /// <param name="numCasts"></param>
    /// <param name="maxRange"></param>
    /// <param name="deviationHalfAngle"></param>
    /// <param name="isDebug"></param>
    /// <returns> a list of type RaycastHit</returns>
    public static List<RaycastHit> MultipleRaycastInCone(Vector3 start, Vector3 forward, Vector3 up, int numCasts, float maxRange, float deviationHalfAngle = 30f, uint seed = 42, bool isDebug = false)
    {
        RaycastHit hit;
        List<RaycastHit> hits = new List<RaycastHit>();
        Unity.Mathematics.Random r = new Unity.Mathematics.Random(seed); //seed is 42

        for (int i = 0; i < numCasts; ++i)
        {
            Vector3 dir = GetRandomVectorInCone(forward, up, deviationHalfAngle, ref r);
            Physics.Raycast(start, dir, out hit, maxRange);
            // hits.Add(hit);
            if (hit.collider != null)
            {
                hits.Add(hit);
            }

            if (isDebug) Debug.DrawLine(start, hit.point, Color.chartreuse);
        }

        return hits;
    }

    #endregion

    #region Easing Functions

    /// <summary>
    /// Uses cosine to get a Y value on the curve based on an x [0..1]
    /// </summary>
    /// <param name="x"></param>
    /// <returns> a float from 0 to 1</returns>
    public static float EaseInCos(this float x)
    {
        return Mathf.Cos((Mathf.PI * 0.5f) * x);
    }

    /// <summary>
    /// Gets a Y value on a curve from a given x (-inf, 1]
    /// </summary>
    /// <param name="x"></param>
    /// <returns> a positive float on the curve Sqrt(1-x) </returns>
    public static float DropOutSqrt(this float x)
    {
        return Mathf.Sqrt(1 - x);
    }

    /// <summary>
    /// Gets the Y value along the top half of a circle based on an input [-1..1]
    /// </summary>
    /// <param name="x"></param>
    /// <returns> a float from 0 to 1</returns>
    public static float ArcEaseInOut(this float x)
    {
        return Mathf.Sqrt(1f - (x * x));
    }

    public static float X2EaseOut(this float x)
    {
        return x * x;
    }
    #endregion

    #region Utilities

    /// <summary>
    /// Takes in a float value within the range [fromMin, fromMax] maps it to a range [toMin, toMax]
    /// </summary>
    /// <param name="input"></param>
    /// <param name="fromMin"></param>
    /// <param name="fromMax"></param>
    /// <param name="toMin"></param>
    /// <param name="toMax"></param>
    /// <returns> a float in the range [toMin toMax]</returns>
    public static float MapRange(this float input, float fromMin, float fromMax, float toMin, float toMax)
    {
        float t = Mathf.InverseLerp(fromMin, fromMax, input);
        float output = Mathf.Lerp(toMin, toMax, t);
        return output;
    }

    /// <summary>
    /// Gets whether the input float value is positive, negative, or 0
    /// </summary>
    /// <param name="val"></param>
    /// <returns> a float -1, 1, or 0</returns>
    public static float CustomSign(this float val)
    {
        if (val == 0f) return 0f;
        return (val >= 0f) ? 1f : (-1f);
    }

    #endregion

}
