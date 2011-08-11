using UnityEngine;
using System.Collections;

public abstract class FluidRenderer : MonoBehaviour
{
    public abstract void Init(IFluidSimulation simulation);
}

