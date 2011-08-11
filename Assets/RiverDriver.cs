using UnityEngine;
using System.Collections;


public class RiverDriver : ISimulationDriver
{
    private IFluidSimulation _simulation;

    public RiverDriver(IFluidSimulation simulation)
    {
        _simulation = simulation;

        for (int y = 0; y < _simulation.Height; ++y)
        {
            for (int x = 0; x < _simulation.Width; ++x)
            {
                if ((y < _simulation.Height / 3 && x < _simulation.Width * 5 / 8)
                    || (y > _simulation.Height * 2 / 3) || (x > _simulation.Width * 7 / 8)
                    || ((x > _simulation.Width * 5 / 8 && y > _simulation.Height / 3
                         && Mathf.Pow(((float)x / _simulation.Width - 5.0f / 8) * 4, 2)
                         + Mathf.Pow(((float)y / _simulation.Height - 1.0f / 3) * 3, 2) > 1))
                    || (x > _simulation.Width * 24 / 32 && x < _simulation.Width * 25 / 32
                        && y > _simulation.Height * 2 / 16 && y < _simulation.Height * 4 / 16))
                {
                    _simulation.AddWater(x, y, 2.0f);
                    _simulation.SetSolid(x, y, true);
                }
            }
        }

    }

    public void Update()
    {
        for (int y = _simulation.Height * 3 / 8; y < _simulation.Height * 5 / 8; ++y)
        {
            _simulation.AddWater(0, y, 2.0f);
        }
        for (int x = _simulation.Width * 5 / 8; x < _simulation.Width * 7 / 8; ++x)
        {
            _simulation.SetHeight(x, 0, 0.0f);
            _simulation.SetVelocity(x, 0, 0.0f);
        }
    }
}
