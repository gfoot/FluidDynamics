using UnityEngine;
using System.Collections;

public class CameraNavigation : MonoBehaviour
{
    public float moveSpeed = 100;
    public float rotateScale = 1;
    public float maxScale = 1;

	void Update()
    {
        float sideways = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float forwards = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        Vector3 flatFront = transform.forward;
        flatFront.y = 0;
        flatFront.Normalize();

        transform.position =
            transform.position
                + sideways * transform.right
                + forwards * flatFront;

        if (Input.GetMouseButton(1))
        {
            float rotateSideways = Input.GetAxis("Mouse X");
            float scale = Mathf.Abs(rotateSideways);
            if (scale > maxScale) scale = maxScale;
            transform.RotateAround(Vector3.up, rotateSideways * scale * rotateScale);
        }
	}
}
