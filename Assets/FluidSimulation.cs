using UnityEngine;
using System;
using System.Collections;

public class FluidSimulation
{
    [Serializable]
    public class Settings
    {
        public float viscosity = 0.05f;
        public float shallowViscosity = 0.45f;
        public float shallownessScale = 1.0f;
        public float damping = 4.0f;
        public float endpointImpedence = 1.0f;

        public static Settings Default = new Settings();
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
    }

    public void AddWater(int x, int z, float amount)
    {
        _heights[x, z] += amount;
    }

    public float GetHeight(int x, int z)
    {
        return _heights[x, z];
    }

    public void SetHeight(int x, int z, float v)
    {
        _heights[x, z] = v;
    }

    public void SetVelocity(int x, int z, float v)
    {
        _velocities[x, z] = v;
    }

    public void SetSolid(int x, int z, bool s)
    {
        _solid[x, z] = s;
    }

    public void Update()
    {
        UpdateVelocities();
        UpdatePositions();
    }

    void UpdateNeighbour(int x, int y, int dx, int dy)
    {
        int other_x = x + dx;
        int other_y = y + dy;
        if (other_x < 0 || other_x >= _width || other_y < 0 || other_y >= _width)
            return;
        if (_solid[other_x, other_y])
            return;
        
        float mean = (_heights[other_x, other_y] + _heights[x, y]) / 2;
        float viscosity = _settings.viscosity + (_settings.shallowViscosity - _settings.viscosity) * Mathf.Exp(-mean * _settings.shallownessScale);
        float delta = _heights[other_x, other_y] - _heights[x, y];
        _velocities[x, y] += delta * viscosity;
    }

    void UpdateVelocities()
    {
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                if (_solid[x, y])
                    continue;

                UpdateNeighbour(x, y, 1, 0);
                UpdateNeighbour(x, y, -1, 0);
                UpdateNeighbour(x, y, 0, 1);
                UpdateNeighbour(x, y, 0, -1);
            }
        }
    }

    void UpdatePositions()
    {
        float damp_mult = 1 - Mathf.Pow(_settings.damping / 100, 2);
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                _heights[x, y] += _velocities[x, y];
                _velocities[x, y] *= damp_mult;
                
                if (_heights[x, y] < 0)
                {
                    _heights[x, y] = 0;
                    _velocities[x, y] = 0;
                }
            }
            
            _velocities[_width - 1, y] *= _settings.endpointImpedence;
        }
    }
}
