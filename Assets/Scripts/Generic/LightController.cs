using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightController : MonoBehaviour
{

    [SerializeField] private Light2D light;

    [SerializeField] private Color dungeonColor;

    private void Start()
    {
        RoomSpawner.instance.OnBiomeSwitch += SwitchBiome;
    }

    void SwitchBiome(Biome biome)
    {
        switch (biome)
        {
            case Biome.GRASS:
                light.intensity = 0.9f;
                light.color = Color.white;
                break;
            case Biome.DUNGEON:
                light.intensity = 0.4f;
                light.color = dungeonColor;
                break;
            default:
                Debug.LogWarning("No light data for biome: " + biome);
                break;
        }
    }

}
