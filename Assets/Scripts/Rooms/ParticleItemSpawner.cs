using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class ParticleItemSpawner : MonoBehaviour
{
    public ParticleSystem ps;
    public List<Item> items;
    public GameObject pickupPrefab;
    private ParticleSystem.Particle[] particles;

    [SerializeField] private Transform pointLightPrefab;
    private List<Transform> pointLights = new List<Transform>();

    public float delay = 0;

    public Color common;
    public Color rare;
    public Color epic;
    public Color legendary;

    bool positionAdded;
    int listSize;
    
    List<Vector3> particlePositions = new List<Vector3>();

    private void Start()
    {
        ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, items.Count) });
        ParticleSystem.MainModule main = ps.main;



        main.startDelay = delay;

        Color colorToSet = common;
        switch (items[0].rarity)
        {
            case Rarity.COMMON:
                colorToSet = common;
                break;
            case Rarity.RARE:
                colorToSet = rare;
                break;
            case Rarity.EPIC:
                colorToSet = epic;
                break;
            case Rarity.LEGENDARY:
                colorToSet = legendary;
                break;
            default:
                break;
        }

        main.startColor = colorToSet;

        //Create enough lights
        for (int i = 0; i < items.Count; i++)
        {
            Transform light = Instantiate(pointLightPrefab, transform);
            light.GetComponent<Light2D>().color = colorToSet;
            light.GetComponent<Light2D>().intensity = 0;
            pointLights.Add(light);
        }

        Invoke("EnableLights", delay);
    }

    void EnableLights()
    {
        foreach (Transform light in pointLights)
        {
            switch (items[0].rarity)
            {
                case Rarity.COMMON:
                    light.GetComponent<Light2D>().intensity = .6f;
                    break;
                case Rarity.RARE:
                    light.GetComponent<Light2D>().intensity = 1f;
                    break;
                case Rarity.EPIC:
                    light.GetComponent<Light2D>().intensity = 1.3f;
                    break;
                case Rarity.LEGENDARY:
                    light.GetComponent<Light2D>().intensity = 1.8f;
                    break;
                default:
                    break;
            }
            
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        InitializeIfNeeded();

        if (!positionAdded)
        {
            for (int i = 0; i < ps.GetParticles(particles); i++)
            {
                pointLights[i].position = particles[i].position;

                if (particles[i].remainingLifetime <= 0.1)
                {
                    particlePositions.Add(particles[i].position);
                    positionAdded = true;
                }

            }
            if(particlePositions.Count == listSize)
            {
                DestroyLights();
                SpawnItems();
                
            }
        }

    }

    void InitializeIfNeeded()
    {
        if(particles == null || particles.Length < ps.main.maxParticles)
        {
            listSize = ps.emission.GetBurst(0).maxCount;
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }
    }

    void SpawnItems()
    {
        if(items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                AudioManager.instance.PlayRandomOfType(SoundType.ITEM_DROP);
                GameObject newItem = Instantiate(pickupPrefab, RoomSpawner.instance.getCurrentRoom().transform);
                newItem.GetComponent<Pickup>().item = items[i];
                newItem.transform.position = particlePositions[i];

            }
        }
        else
        {
            Debug.LogWarning("No Items found in ParticleSpawner list.");
        }

        Destroy(gameObject, 2);
    }

    void DestroyLights()
    {
        foreach (Transform light in pointLights)
        {
            Destroy(light.gameObject);
        }
    }

}
