using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]private float speed;
    private float currentPosition;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosition, transform.position.y, transform.position.z), ref velocity, speed * Time.deltaTime);
    }

    public void MoveToNewChunk(Transform NewChunk)
    {
        currentPosition = NewChunk.position.x;
    }
}