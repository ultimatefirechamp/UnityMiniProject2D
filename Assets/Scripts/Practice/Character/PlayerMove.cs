using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        Move();   
    }
    void Move()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y+1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position = new Vector2(transform.position.x - 1 , transform.position.y);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position = new Vector2(transform.position.x + 1, transform.position.y);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.position = new Vector2(transform.position.x - 1, transform.position.y + 1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.position = new Vector2(transform.position.x + 1, transform.position.y + 1);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            transform.position = new Vector2(transform.position.x - 1, transform.position.y - 1);
        }

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            transform.position = new Vector2(transform.position.x + 1, transform.position.y - 1);
        }
    }
}
