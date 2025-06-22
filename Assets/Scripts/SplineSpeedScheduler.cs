using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSpeedScheduler : MonoBehaviour
{
    [System.Serializable]
    public struct TimeSpeedPair
    {
        public float time;     // 경과 시간 기준
        public float speed;    // 해당 시점에 설정할 속도
    }

    public SplineAnimate splineAnimate;
    public List<TimeSpeedPair> speedSchedule = new List<TimeSpeedPair>();

    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        if (splineAnimate == null || speedSchedule.Count == 0)
        {
            Debug.LogWarning("SplineAnimate or speed schedule not set.");
            enabled = false;
            return;
        }

        // 시작 속도 설정
        splineAnimate.MaxSpeed = speedSchedule[0].speed;        
    }

    void Update()
    {
        if (splineAnimate == null || splineAnimate.AnimationMethod != SplineAnimate.Method.Speed)
            return;

        timer += Time.deltaTime;
        Debug.Log($"Current index: {currentIndex}, Current timer: {timer}");
        // 다음 타임스탬프가 도달하면 속도 변경
        if (currentIndex + 1 < speedSchedule.Count &&
            timer >= speedSchedule[currentIndex + 1].time)
        {
            currentIndex++;
            splineAnimate.MaxSpeed = speedSchedule[currentIndex].speed;
            Debug.Log($"Current MaxSpeed: {splineAnimate.MaxSpeed}");
        }
    }
}
