using UnityEngine;
using System.Collections;

public class BasicFluidRenderer : FluidRenderer
{
    private IFluidSimulation _simulation;

    private Transform[] _transforms;

    public override void Init(IFluidSimulation simulation)
    {
        _simulation = simulation;

        _transforms = new Transform[_simulation.Width];

        for (int i = 0; i < _simulation.Width; ++i)
        {
            GameObject obj0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj0.transform.localPosition = new Vector3(0, 0.5f, 0);

            GameObject obj = new GameObject();
            obj0.transform.parent = obj.transform;

            obj.transform.position = new Vector3(i, 0, 0);

            _transforms[i] = obj.transform;
        }
    }
    
    void LateUpdate()
    {
        for (int i = 0; i < _transforms.Length; ++i)
        {
            _transforms[i].localScale = new Vector3(1, _simulation.GetHeight(i, 0) * 2, 1);
        }
    }
}
