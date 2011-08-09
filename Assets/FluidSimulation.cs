using UnityEngine;
using System;
using System.Collections;

public class FluidSimulation
{
    [Serializable]
    public class Settings
    {
        public float viscosity;
        public float damping;
        public float endpointImpedence;

        public static Settings Default = new Settings(0.1f, 0.999f, 1.0f);

        public Settings(float viscosity, float damping, float endpointImpedence)
        {
            this.viscosity = viscosity;
            this.damping = damping;
            this.endpointImpedence = endpointImpedence;
        }
    };

    private int _width;
    public int Width { get { return _width; } }

    private float[] _heights;
    private float[] _velocities;

    private Settings _settings;

    public FluidSimulation(int width, Settings settings)
    {
        _width = width;
        _settings = settings;

        _heights = new float[width];
        _velocities = new float[width];
    }

    public void AddWater(int i, float amount)
    {
        _heights[i] += amount;
    }

    public float GetHeight(int i)
    {
        return _heights[i];
    }

    public void Update()
    {
        UpdateVelocities();
        UpdatePositions();
    }

    void UpdateVelocities()
    {
        for (int i = 0; i < _width; ++i)
        {
            for (int di = -1; di <= 1; di += 2)
            {
                int other_i = i + di;
                if (other_i < 0 || other_i >= _width)
                    continue;

                float delta = _heights[other_i] - _heights[i];
                _velocities[i] += delta * _settings.viscosity;
            }
        }
    }

    void UpdatePositions()
    {
        for (int i = 0; i < _width; ++i)
        {
            _heights[i] += _velocities[i];
            _velocities[i] *= _settings.damping;
        }

        _velocities[_width-1] *= _settings.endpointImpedence;
    }
}
