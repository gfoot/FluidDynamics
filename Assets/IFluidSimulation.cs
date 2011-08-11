using UnityEngine;
using System.Collections;

public interface IFluidSimulation
{
    int Width { get; }
    int Height { get; }

    float GetHeight(int x, int y);

    void SetHeight(int x, int y, float h);
    void SetVelocity(int x, int y, float v);
    void SetSolid(int x, int y, bool s);

    void AddWater(int x, int y, float amount);

    void Update();
}
