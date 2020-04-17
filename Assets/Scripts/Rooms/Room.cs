using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomX;
    public int roomY;

    public void setCoordinates(int x, int y)
    {
        roomX = x;
        roomY = y;
    }
}
