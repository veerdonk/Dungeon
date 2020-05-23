using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class ShadowTrigger : MonoBehaviour
{

    [SerializeField] new SpriteRenderer renderer;
    [SerializeField] new Animator animator;
    [SerializeField] new GameObject collider;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            RoomSpawner.instance.EnableShadowWalls();
        }   
    }

    public void EnableWall()
    {
        renderer.enabled = true;
        animator.enabled = true;
        collider.SetActive(true);
        gameObject.tag = "Wall";
    }

    public void DisableWall()
    {
        renderer.enabled = false;
        animator.enabled = false;
        collider.SetActive(false);
        gameObject.tag = "Untagged";
    }
}
