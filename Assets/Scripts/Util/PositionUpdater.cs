using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdater : MonoBehaviour
{

    public Transform follow;
    public float Yoffset;

    private void LateUpdate()
    {

        
        GetComponent<RectTransform>().position = new Vector3(follow.position.x, follow.position.y + Yoffset, 0f);

    }
}
