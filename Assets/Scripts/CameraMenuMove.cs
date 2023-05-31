using UnityEngine;

public class CameraMenuMove : MonoBehaviour
{
    [SerializeField] GameObject newPosition;
    [SerializeField] GameObject oldPosition;

    public void Move()
    {
        transform.position = newPosition.transform.position;
        transform.rotation = newPosition.transform.rotation;
    }
    public void MoveBack()
    {
        transform.position = oldPosition.transform.position;
        transform.rotation = oldPosition.transform.rotation;
    }
}
