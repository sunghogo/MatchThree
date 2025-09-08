using UnityEngine;

public class Center : MonoBehaviour
{
    [SerializeField] public Transform Parent { get; protected set; }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile") && collision.transform != Parent)
        {
            Debug.Log(collision.name);
            collision.gameObject.GetComponent<Tile>().DestroyNoPoints();
            GameManager.Instance.Matched();
        }
    }
}
