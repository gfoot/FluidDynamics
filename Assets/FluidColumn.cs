using UnityEngine;
using System.Collections;

public class FluidColumn : MonoBehaviour
{
    public float height = 0.0f;
    public float deltaHeight = 0.0f;

    public float impedence = 1.0f;
    public bool isFixedHeight = false;

    public FluidManager Manager { get; set; }

    public FluidColumn[] _neighbours = new FluidColumn[2];

	void Update()
    {
        foreach (var neighbour in _neighbours)
        {
            if (neighbour != null)
            {
                float delta = neighbour.height - height;
                float amountToTransfer = delta * Manager.viscosity;
                deltaHeight += amountToTransfer/2;
                neighbour.deltaHeight -= amountToTransfer/2;
            }
        }
	}

    void LateUpdate()
    {
        if (isFixedHeight)
            deltaHeight = 0;

        height += deltaHeight;
        deltaHeight *= Manager.damping;

        deltaHeight *= impedence;

        transform.localScale = new Vector3(1, height*5, 1);
    }
}
