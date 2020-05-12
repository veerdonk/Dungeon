using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public int damage;
    public bool isCrit;
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private TextMesh text;
    [SerializeField] private float destroyTime = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, .7f, 0);
    [SerializeField] private Vector3 randomizedPosition = new Vector3(1f, 0, 0);
    [SerializeField] private Color damageColor;
    [SerializeField] private Color criticalColor;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        gameObject.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-randomizedPosition.x, randomizedPosition.x), 0, 0);
        if (isCrit)
        {
            text.color = criticalColor;
        }
        else
        {
            text.color = damageColor;
        }
        text.text = "-" + damage;

        transform.localScale = new Vector3(transform.localScale.x * (1 + damage / 75), transform.localScale.y * (1 + damage / 75));

        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
