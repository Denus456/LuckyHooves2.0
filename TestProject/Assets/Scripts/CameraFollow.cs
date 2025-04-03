using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float _smoothSpeed = 0.125f;

    private Transform target;

    public void SetTarget(Transform newTarget) => target = newTarget;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}