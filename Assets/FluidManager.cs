using UnityEngine;
using System.Collections;

public class FluidManager : MonoBehaviour
{
    public int width = 10;

    public FluidSimulation.Settings fluidSettings = FluidSimulation.Settings.Default;

    private float _peakVolume = 0.0f;
    private float _volume = 0.0f;

    private bool _fixColumn0Height;

    private FluidSimulation _simulation;

	void Start()
    {
        _simulation = new FluidSimulation(width, fluidSettings);

        GetComponent<FluidRenderer>().Init(_simulation);

        for (int i = 0; i < width; ++i)
        {
            _simulation.AddWater(i, 2.0f);
        }

        _fixColumn0Height = true;
    }

    void FixedUpdate()
    {
        CreateWave();

        _simulation.Update();
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        _volume = 0;
        for (int i = 0; i < width; ++i)
        {
            _volume += _simulation.GetHeight(i);
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
        _simulation.AddWater(0, height);
    }
}
