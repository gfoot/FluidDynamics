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
        
        for (int y = 0; y < width; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if ((y < width / 3 && x < width * 5 / 8) || (y > width * 2 / 3) || (x > width * 7 / 8)
                    || ((x > width *  5/8 && y > width / 3 &&
                         Mathf.Pow(((float)x/width - 5.0f/8) * 4, 2) +
                         Mathf.Pow(((float)y / width - 1.0f/3) * 3, 2) > 1))
                    || (x > width * 24/32 && x < width * 25/32 && y > width * 2/16 && y < width * 4/16)

                    )
                {
                    _simulation.AddWater(x, y, 2.0f);
                    _simulation.SetSolid(x, y, true);
                }
            }
        }
        
        _fixColumn0Height = true;
    }

    void FixedUpdate()
    {
        //CreateWave();
        DriveRiver();
        
        _simulation.Update();
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
            height = 5 - lambda;
        }
        else
        {
            height = 0;
            _fixColumn0Height = false;
        }
        
        _simulation.AddWater(0, 0, height * 20);
    }

    void DriveRiver()
    {
        for (int y = width * 3 / 8; y < width * 5 / 8; ++y)
        {
            _simulation.AddWater(0, y, 2.0f);
            //_simulation.SetHeight(0, y, 3.0f);
            //_simulation.SetVelocity(0, y, 0.0f);
        }
        for (int x = width * 5 / 8; x < width * 7 / 8; ++x)
        {
            _simulation.SetHeight(x, 0, 0.0f);
            _simulation.SetVelocity(x, 0, 0.0f);
        }
    }
}
