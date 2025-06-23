using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimeBasedOrbitCamera : MonoBehaviour
{
    [System.Serializable]
    public class CameraKeyframe
    {
        public static int s_numOfKeyframe;

        public float time;
        public bool isFirstPerson = false;
        public float height = 2f;
        public float distance = 5f;
        public Vector3 rotationOffset = Vector3.zero;
        public bool orbitMode = false;
        public float orbitSpeed = 30f; // degrees per second

        public CameraKeyframe()
        {
            time = s_numOfKeyframe * 5f;
            switch (s_numOfKeyframe % 3)
            {
                //1인칭
                case 0:
                    isFirstPerson = true;
                    orbitMode = false;
                    break;
                //3인칭
                case 1:
                    isFirstPerson = false;
                    orbitMode = false;
                    break;
                //orbit
                case 2:
                    isFirstPerson = false;
                    orbitMode = true;
                    break;
                default:
                    break;
            }
            s_numOfKeyframe++;
        }
    }

    [SerializeField] Text _currentModeText;
    public Transform target;
    public List<CameraKeyframe> keyframes = new List<CameraKeyframe>();

    [Header("Lerp Speeds")]
    public float positionLerpSpeed = 3f;
    public float rotationLerpSpeed = 3f;

    private float elapsedTime = 0f;
    private float orbitAngle = 0f;

    private void Start()
    {
        //test
        for (int i = 0; i < 1000; i++)
        {
            CameraKeyframe keyframe = new CameraKeyframe();
            //keyframe.isFirstPerson = true;
            //keyframe.orbitMode = true;
            keyframes.Add(keyframe);
        }
    }

    void Update()
    {
        if (!target || keyframes.Count == 0) return;

        elapsedTime += Time.deltaTime;

        // 현재 키프레임 찾기
        CameraKeyframe before = keyframes[0];
        CameraKeyframe after = keyframes[keyframes.Count - 1];
        for (int i = 0; i < keyframes.Count - 1; i++)
        {
            if (elapsedTime >= keyframes[i].time && elapsedTime < keyframes[i + 1].time)
            {
                before = keyframes[i];
                after = keyframes[i + 1];
                break;
            }
        }

        float segmentDuration = after.time - before.time;
        float t = segmentDuration > 0f ? Mathf.InverseLerp(before.time, after.time, elapsedTime) : 0f;

        // 보간된 파라미터 계산
        float height = Mathf.Lerp(before.height, after.height, t);
        float distance = Mathf.Lerp(before.distance, after.distance, t);
        Vector3 rotOffset = Vector3.Lerp(before.rotationOffset, after.rotationOffset, t);
        bool orbitMode = t < 0.5f ? before.orbitMode : after.orbitMode;
        float orbitSpeed = Mathf.Lerp(before.orbitSpeed, after.orbitSpeed, t);
        float isFirstPersonVal = Mathf.Lerp(before.isFirstPerson ? 1 : 0, after.isFirstPerson ? 1 : 0, t);

        Vector3 targetPos = target.position + Vector3.up * height;
        //Vector3 targetPos = target.position + target.transform.TransformDirection(Vector3.up) * height;

        // Orbit 모드 처리
        Vector3 desiredPos;
        if (isFirstPersonVal >= 0.5f)
        {
            desiredPos = targetPos;
            _currentModeText.text = "1인칭";
        }
        else if (orbitMode)
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
            float rad = orbitAngle * Mathf.Deg2Rad;
            //Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * distance;
            Vector3 offset = new Vector3(Mathf.Sin(orbitAngle), 1, Mathf.Cos(orbitAngle)).normalized * distance;
            desiredPos = targetPos + offset;
            _currentModeText.text = "궤도";
        }
        else
        {
            //Vector3 backOffset = -target.forward * distance;
            Vector3 backOffset = target.TransformDirection(-target.forward + target.up) * distance;
            desiredPos = targetPos + backOffset;
            _currentModeText.text = "3인칭";
        }

        // 위치 이동
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * positionLerpSpeed);

        // 회전 처리
        Quaternion lookRot = Quaternion.LookRotation(target.position - transform.position);
        Quaternion desiredRot = lookRot * Quaternion.Euler(rotOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * rotationLerpSpeed);
    }


}
