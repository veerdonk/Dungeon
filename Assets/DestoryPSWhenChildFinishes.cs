using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryPSWhenChildFinishes : MonoBehaviour
{
    public float offset = 1;
    public ParticleSystem parent;
    public ParticleSystem[] children;

    private void Start()
    {
        CheckChildren();
    }


    void CheckChildren()
    {
        float longestChild = 0;
        foreach (ParticleSystem ps in children)
        {
            if(ps.main.duration > longestChild)
            {
                longestChild = ps.main.duration;
            }
        }
        if (parent.main.duration > longestChild)
        {
            longestChild = parent.main.duration;
        }
        longestChild += offset;
        Destroy(parent.gameObject, longestChild);
        foreach (ParticleSystem ps in children)
        {
            Destroy(ps, longestChild);
        }
    }

}
