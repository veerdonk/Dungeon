using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmorSwitcher : MonoBehaviour
{
    public static ArmorSwitcher instance;

    [SerializeField] SpriteRenderer headRenderer;
    [SerializeField] SpriteRenderer torsoRenderer;
    //[SerializeField] SpriteRenderer baseRenderer;

    [SerializeField] Animator animator;

    public ArmorPiece headArmor;
    public ArmorPiece torsoArmor;
    public float totalArmor = 0;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("ArmorSwitcher is not a singleton");
        }
        instance = this;
    }

    // Use lateupdate to set sprite after animator does work
    void LateUpdate()
    {
        totalArmor = 0;
        if (headArmor != null)
        {
            //Replace with other frame
            if (headArmor.equipped)
            {
                totalArmor += headArmor.defense;
                headRenderer.sprite = headArmor.sprites.FirstOrDefault(s => s.name == headRenderer.sprite.name);
                headRenderer.color = headArmor.color;
            }
        }

        if (torsoArmor != null)
        {
            if (torsoArmor.equipped)
            {
                totalArmor += torsoArmor.defense;
                torsoRenderer.sprite = torsoArmor.sprites.FirstOrDefault(s => s.name == torsoRenderer.sprite.name);
                torsoRenderer.color = torsoArmor.color;
            }
        }
    }
}
