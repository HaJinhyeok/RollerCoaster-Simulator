using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineSpeedController : MonoBehaviour
{
    [System.Serializable]
    public class SpeedSegment
    {
        public float startTime;
        public float endTime;
        public float speed;
    }

    public enum Axis
    {
        XAxis,
        YAxis,
        ZAxis,
        XAxisNeg,
        YAxisNeg,
        ZAxisNeg
    }

    public SplineContainer splineContainer;
    public SpeedSegment[] speedSegments;
    public Axis forwardAxis = Axis.ZAxis;
    public Axis upAxis = Axis.YAxis;

    private float elapsedTime = 0f;
    private float totalDistance;
    private float currentT = 0f;

    void Start()
    {
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();

        totalDistance = splineContainer.Splines[0].GetLength();
        Debug.Log(Quaternion.identity);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float currentSpeed = GetCurrentSpeed(elapsedTime);

        float deltaDistance = currentSpeed * Time.deltaTime;
        float deltaT = deltaDistance / totalDistance;
        currentT += deltaT;

        currentT = Mathf.Clamp01(currentT);

        float3 pos = splineContainer.EvaluatePosition(currentT);
        float3 tangent = splineContainer.EvaluateTangent(currentT);
        float3 up = splineContainer.EvaluateUpVector(currentT);

        Quaternion axisRemap = Quaternion.Inverse(Quaternion.LookRotation(GetAxisVector(forwardAxis), GetAxisVector(upAxis)));
        Debug.Log(axisRemap.eulerAngles);
        Debug.Log(axisRemap);
        //Quaternion finalRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)up) * axisRemap;
        Quaternion finalRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)up);

        transform.position = (Vector3)pos;
        transform.rotation = finalRotation;
        if (currentT >= 1f)
        {
            elapsedTime = 0f;
            currentT = 0f;
        }
    }

    float GetCurrentSpeed(float time)
    {
        foreach (var segment in speedSegments)
        {
            if (time >= segment.startTime && time < segment.endTime)
                return segment.speed;
        }
        return 0f;
    }

    Vector3 GetAxisVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.XAxis: return Vector3.right;
            case Axis.YAxis: return Vector3.up;
            case Axis.ZAxis: return Vector3.forward;
            case Axis.XAxisNeg: return -Vector3.right;
            case Axis.YAxisNeg: return -Vector3.up;
            case Axis.ZAxisNeg: return -Vector3.forward;
            default: return Vector3.forward;
        }
    }
}

