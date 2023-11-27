using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private string[] tagsToCheck;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in tagsToCheck)
        {
            if(collision.CompareTag(tag))
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}
