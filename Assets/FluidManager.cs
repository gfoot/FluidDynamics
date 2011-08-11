using UnityEngine;
using System.Collections;

public class FluidManager : MonoBehaviour
{
    public int width = 10;

    public FluidSimulation.Settings fluidSettings = FluidSimulation.Settings.Default;

    private float _peakVolume = 0.0f;
    private float _volume = 0.0f;

    private IFluidSimulation _simulation;
    private ISimulationDriver _driver;

    void Start()
    {
        _simulation = new FluidSimulationHvel(width, fluidSettings);
        
        GetComponent<FluidRenderer>().Init(_simulation);
        
        _driver = new MazeDriver(_simulation);
    }

    void FixedUpdate()
    {
        _simulation.Update();
        _driver.Update();
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        _volume = 0;
        for (int y = 0; y < width; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                _volume += _simulation.GetHeight(x, y);
            }
        }
        if (_volume > _peakVolume)
        {
            _peakVolume = _volume;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 20), string.Format("Volume: {0:0.0000}", _volume));
        //(_volume - _peakVolume) / _peakVolume));
    }
}
