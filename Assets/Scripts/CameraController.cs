using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _cameraHolder;
    [SerializeField] Text _currentModeText;

    readonly Vector3 _posOffset = new Vector3(0, 10, -10);
    readonly Vector3 _angleOffset = new Vector3(20, 0, 0);
    const float _orbitDistance = 15f;
    const float _orbitSpeed = 3f;
    const float _posLerpSpeed = 3f;

    Vector3 _prevPosition;
    bool _isOrbit = false;
    bool _isFirstPerson = false;
    float _timer = 0f;

    public bool IsOrbit
    {
        get { return _isOrbit; }
        set
        {
            _isOrbit = value;
            if (_isOrbit)
            {
                // 궤도모드 전환 시 타이머 초기화
                _timer = 0f;
                if (!_isFirstPerson)
                {
                    transform.SetParent(null);
                    SetOrbitPerspective(0f);
                }
                else
                {
                    SetFirstPersonPerspective();
                }
            }
            else
            {
                if (_isFirstPerson)
                {
                    SetFirstPersonPerspective();
                }
                else
                {
                    SetThirdPersonPerspective();
                }
            }
        }
    }

    public bool IsFirstPerson
    {
        get { return _isFirstPerson; }
        set
        {
            _isFirstPerson = value;
            if (!_isFirstPerson)
            {
                if (_isOrbit)
                {
                    transform.SetParent(null);
                    SetOrbitPerspective(0f);
                }
                else
                {
                    SetThirdPersonPerspective();
                }
            }
            else
            {
                SetFirstPersonPerspective();
            }
        }
    }

    void Start()
    {
        SetThirdPersonPerspective();
        _prevPosition = transform.position;
    }

    void Update()
    {
        // 인칭 전환
        if (Input.GetKeyDown(KeyCode.V))
        {
            IsFirstPerson = !IsFirstPerson;
        }

        // 궤도 모드 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsOrbit = !IsOrbit;
        }

        if (_isOrbit && !_isFirstPerson)
        {
            _timer += Time.deltaTime;
            // 0~360도 사이로 조절
            if (_timer >= 360 / _orbitSpeed)
            {
                _timer -= 360 / _orbitSpeed;
            }
            SetOrbitPerspective(_timer * _orbitSpeed);
        }
    }

    Vector3 EvaluateOrbitPosition(float angle)
    {
        Vector3 orbitPos = new Vector3(Mathf.Sin(angle), 1, -Mathf.Cos(angle));
        orbitPos = _target.TransformDirection(orbitPos) * _orbitDistance;
        orbitPos += _target.position - _target.TransformDirection(_target.forward) * 5f;

        return orbitPos;
    }

    void SetThirdPersonPerspective()
    {
        transform.SetParent(_target);
        transform.localPosition = _posOffset;
        transform.localEulerAngles = _angleOffset;
        _currentModeText.text = "3인칭";
    }

    void SetFirstPersonPerspective()
    {
        transform.SetParent(_cameraHolder);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        _currentModeText.text = "1인칭";
    }

    void SetOrbitPerspective(float angle)
    {
        transform.position = EvaluateOrbitPosition(angle);
        transform.LookAt(_target.position + _target.TransformDirection(Vector3.forward) * 5f);
        _currentModeText.text = "궤도";
    }
}
