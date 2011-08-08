using UnityEngine;
using System.Collections;

public class FluidManager : MonoBehaviour
{
    public int width = 10;

    public float viscosity = 0.001f;
    public float damping = 0.9f;

    private float _peakVolume = 0.0f;
    private float _volume = 0.0f;

    private FluidColumn[] _fluidColumns;

	// Use this for initialization
	void Start()
    {
        _fluidColumns = new FluidColumn[width];

        for (int i = 0; i < width; ++i)
        {
            GameObject obj0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj0.transform.localPosition = new Vector3(0, 0.5f, 0);

            GameObject obj = new GameObject();
            obj0.transform.parent = obj.transform;

            obj.transform.position = new Vector3(i, 0, 0);

            _fluidColumns[i] = obj.AddComponent<FluidColumn>();
            _fluidColumns[i].Manager = this;

            if (i > 0)
            {
                _fluidColumns[i]._neighbours[0] = _fluidColumns[i-1];
                _fluidColumns[i-1]._neighbours[1] = _fluidColumns[i];
            }
        }

        for (int i = 0; i < width; ++i)
        {
            _fluidColumns[i].height = 2.0f;
        }

        _fluidColumns[0].isFixedHeight = true;
        //_fluidColumns[width-1].impedence = 0.9f;
    }

    void Update()
    {
        CreateWave();

        UpdateVolume();
    }

    void UpdateVolume()
    {
        _volume = 0;
        foreach (var fluidColumn in _fluidColumns)
        {
            _volume += fluidColumn.height;
        }
        if (_volume > _peakVolume)
        {
            _peakVolume = _volume;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 20), string.Format("Volume: {0:0.0000}", _volume));//(_volume - _peakVolume) / _peakVolume));
    }

    void CreateWave()
    {
        if (!_fluidColumns[0].isFixedHeight)
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
            _fluidColumns[0].isFixedHeight = false;
        }
        _fluidColumns[0].height = 2.0f + 3.0f * height;
    }
}
