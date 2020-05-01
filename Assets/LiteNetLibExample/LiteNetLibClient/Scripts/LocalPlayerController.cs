using UnityEngine;

public class LocalPlayerController : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 180f;

    void Update()
    {
        Vector3 direction = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;		
        if(direction.sqrMagnitude > 0.01f)
        {
            Vector3 forward = Vector3.Slerp(
                transform.forward,
                direction,
                rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction)
            );
            transform.LookAt(transform.position + forward);
        }
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
}
