using UnityEngine;
using System.Collections;

public class FluidManager : MonoBehaviour
{
    public int width = 10;

    public float viscosity = 0.001f;
    public float damping = 0.9f;
    public float endpointImpedence = 1.0f;

    private float _peakVolume = 0.0f;
    private float _volume = 0.0f;

    private float[] _heights;
    private float[] _velocities;

    private Transform[] _transforms;

    private bool _fixColumn0Height;

	// Use this for initialization
	void Start()
    {
        _heights = new float[width];
        _velocities = new float[width];
        _transforms = new Transform[width];

        for (int i = 0; i < width; ++i)
        {
            GameObject obj0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj0.transform.localPosition = new Vector3(0, 0.5f, 0);

            GameObject obj = new GameObject();
            obj0.transform.parent = obj.transform;

            obj.transform.position = new Vector3(i, 0, 0);

            _transforms[i] = obj.transform;

            _heights[i] = 2.0f;
            _velocities[i] = 0.0f;
        }

        _fixColumn0Height = true;
    }

    void FixedUpdate()
    {
        CreateWave();

        UpdateVelocities();
        UpdatePositions();
    }

    void Update()
    {
        UpdateVolume();

        UpdateVisuals();
    }

    void UpdateVelocities()
    {
        for (int i = 0; i < width; ++i)
        {
            for (int di = -1; di <= 1; di += 2)
            {
                int other_i = i + di;
                if (other_i < 0 || other_i >= width)
                    continue;

                float delta = _heights[other_i] - _heights[i];
                _velocities[i] += delta * viscosity;
            }
        }
    }

    void UpdatePositions()
    {
        for (int i = 0; i < width; ++i)
        {
            _heights[i] += _velocities[i];
            _velocities[i] *= damping;
        }

        _velocities[width-1] *= endpointImpedence;
    }

    void UpdateVolume()
    {
        _volume = 0;
        foreach (var height in _heights)
        {
            _volume += height;
        }
        if (_volume > _peakVolume)
        {
            _peakVolume = _volume;
        }
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < width; ++i)
        {
            _transforms[i].localScale = new Vector3(1, _heights[i] * 2, 1);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 20), string.Format("Volume: {0:0.0000}", _volume));//(_volume - _peakVolume) / _peakVolume));
    }

    void CreateWave()
    {
        if (!_fixColumn0Height)
            return;

        float lambda = (UnityEngine.Time.realtimeSinceStartup - 1) / 0.2f;
        float height;
        if (lambda < 0)
        {
            height = 0;
        }
        else if (lambda < 1)
        {
            height = lambda;
        }
        else if (lambda < 4)
        {
            height = 1;
        }
        else if (lambda < 5)
        {
            height = 5-lambda;
        }
        else
        {
            height = 0;
            _fixColumn0Height = false;
        }
        _heights[0] = 2.0f + 3.0f * height;
    }
}
