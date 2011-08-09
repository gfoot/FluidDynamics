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
    }

    private int _width;
    public int Width {
        get { return _width; }
    }
    public int Height {
        get { return _width; }
    }

    private float[,] _heights;
    private float[,] _velocities;
    private bool[,] _solid;

    private Settings _settings;

    public FluidSimulation(int width, Settings settings)
    {
        _width = width;
        _settings = settings;
        
        _heights = new float[width, width];
        _velocities = new float[width, width];
        _solid = new bool[width, width];
        
        for (int y = width/10; y < width; ++y)
        {
            for (int x = width/2; x < width*2/3; ++x)
            {
                _solid[x, y] = true;
            }
        }
    }

    public void AddWater(int x, int z, float amount)
    {
        _heights[x, z] += amount;
    }

    public float GetHeight(int x, int z)
    {
        return _heights[x, z];
    }

    public void Update()
    {
        UpdateVelocities();
        UpdatePositions();
    }

    void UpdateVelocities()
    {
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                if (_solid[x,y])
                    continue;

                for (int dx = -1; dx <= 1; dx += 2)
                {
                    int other_x = x + dx;
                    if (other_x < 0 || other_x >= _width)
                        continue;
                    if (_solid[other_x,y])
                        continue;

                    float delta = _heights[other_x, y] - _heights[x, y];
                    _velocities[x, y] += delta * _settings.viscosity;
                }
                for (int dy = -1; dy <= 1; dy += 2)
                {
                    int other_y = y + dy;
                    if (other_y < 0 || other_y >= _width)
                        continue;
                    if (_solid[x,other_y])
                        continue;

                    float delta = _heights[x, other_y] - _heights[x, y];
                    _velocities[x, y] += delta * _settings.viscosity;
                }
            }
        }
    }

    void UpdatePositions()
    {
        float damp_mult = 1 - Mathf.Pow(_settings.damping/100, 2);
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                _heights[x, y] += _velocities[x, y];
                _velocities[x, y] *= damp_mult;
            }
            
            _velocities[_width - 1, y] *= _settings.endpointImpedence;
        }
    }
}
