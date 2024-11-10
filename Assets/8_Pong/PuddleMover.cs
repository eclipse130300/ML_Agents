using UnityEngine;

public class PuddleMover : MonoBehaviour
{
    [SerializeField]
    private float paddleMaxSpeed = 15;
    [SerializeField]
    private float paddleMinY = 8.8f;
    [SerializeField]
    private float paddleMaxY = 17.4f;
    
        public void Move(float vY)
        {
            float posy = Mathf.Clamp(transform.position.y + (vY * Time.deltaTime * paddleMaxSpeed),
                paddleMinY, paddleMaxY);
        
            transform.position = new Vector3(transform.position.x, posy, transform.position.z);
        }
}