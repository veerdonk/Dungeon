using System.Collections;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class particleAttractorLinear : MonoBehaviour {
	ParticleSystem ps;
	ParticleSystem.Particle[] m_Particles;
	public Transform target;
	public float speed = 5f;
	public float delay = 0;
	int numParticlesAlive;
	void Start () {
		target = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform;

		ps = GetComponent<ParticleSystem>();
		if (!GetComponent<Transform>()){
			GetComponent<Transform>();
		}
	}
	void Update () {
		if (delay <= 0)
		{
			m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
			numParticlesAlive = ps.GetParticles(m_Particles);
			float step = speed * Time.deltaTime;
			for (int i = 0; i < numParticlesAlive; i++)
			{
				m_Particles[i].position = Vector3.LerpUnclamped(m_Particles[i].position, target.position, step);
			}
			ps.SetParticles(m_Particles, numParticlesAlive);
		}
		else
		{
			delay -= Time.deltaTime;
		}
	}
}
