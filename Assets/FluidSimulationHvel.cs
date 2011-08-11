using UnityEngine;
using System;
using System.Collections;

public class FluidSimulationHvel : IFluidSimulation
{
    private int _width;
    public int Width {
        get { return _width; }
    }
    public int Height {
        get { return _width; }
    }

    private float[,] _heights;
    private float[,] _xvelocities;
    private float[,] _yvelocities;
    private float[,] _velocities;
    private bool[,] _solid;

    private FluidSimulation.Settings _settings;

    public FluidSimulationHvel(int width, FluidSimulation.Settings settings)
    {
        _width = width;
        _settings = settings;
        
        _heights = new float[width, width];
        _xvelocities = new float[width + 1, width];
        _yvelocities = new float[width, width + 1];
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
        //_velocities[x, z] = v;
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

    void UpdateVelocities()
    {
        float damp_mult = 1 - Mathf.Pow(_settings.damping / 100, 2);

        // Calculate horizontal velocities - note the loop endpoints, especially that
        // the horizontal velocity arrays have _width+1 elements.  So we're not filling
        // in the extra cells yet.
        //
        // _xvelocities[x,y] is the rate of flow per unit height into [x,y] from [x-1,y], etc.
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                if (_solid[x, y])
                {
                    _xvelocities[x, y] = 0;
                    _yvelocities[x, y] = 0;
                    continue;
                }

                // Dampen old values
                _xvelocities[x, y] *= damp_mult;
                _yvelocities[x, y] *= damp_mult;
                
                if (x == 0 || _solid[x - 1, y])
                {
                    _xvelocities[x, y] = 0;
                }
                else
                {
                    _xvelocities[x, y] += _settings.viscosity * (_heights[x - 1, y] - _heights[x, y]);
                }
                
                if (y == 0 || _solid[x, y - 1])
                {
                    _yvelocities[x, y] = 0;
                }
                else
                {
                    _yvelocities[x, y] += _settings.viscosity * (_heights[x, y - 1] - _heights[x, y]);
                }
            }
        }

        // Fill in the extra cells in the horizontal velocity arrays
        for (int i = 0; i < _width; ++i)
        {
            _xvelocities[_width, i] = 0;
            _yvelocities[i, _width] = 0;
        }

        // Fill in the derived vertical velocity array, ready for application to the height array
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                _velocities[x, y] = XVelTimesHeight(x, y) - XVelTimesHeight(x+1, y)
                    + YVelTimesHeight(x, y) - YVelTimesHeight(x, y+1);
            }
        }
    }

    float XVelTimesHeight(int x, int y)
    {
        float v = _xvelocities[x, y];
        return v;
        if (x == 0)
            return v * _heights[x, y];
        else if (x == _width)
            return v * _heights[x-1, y];
        else
            return v * (_heights[x-1, y] + _heights[x, y]) / 2;
        //int sx = x;
        //if (v > 0) sx = x - 1;
        //if (sx < 0) sx = 0;
        //if (sx >= _width) sx = _width - 1;
        //return v * _heights[sx, y];
    }

    float YVelTimesHeight(int x, int y)
    {
        float v = _yvelocities[x, y];
        return v;
        if (y == 0)
            return v * _heights[x, y];
        else if (y == _width)
            return v * _heights[x, y-1];
        else
            return v * (_heights[x, y-1] + _heights[x, y]) / 2;
        //int sy = y;
        //if (v > 0) sy = y - 1;
        //if (sy < 0) sy = 0;
        //if (sy >= _width) sy = _width - 1;
        //return v * _heights[x, sy];
    }

    void UpdatePositions()
    {
        for (int y = 0; y < _width; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                _heights[x, y] += _velocities[x, y];
                
                if (_heights[x, y] < 0)
                {
                    _heights[x, y] = 0;
                }
            }
        }
    }
}
