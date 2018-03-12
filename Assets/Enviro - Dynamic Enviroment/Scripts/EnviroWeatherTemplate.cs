using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnviroWeatherTemplate {

	public string Name;
	public bool Spring = true;
	public float possibiltyInSpring = 100f;
	public bool Summer = true;
	public float possibiltyInSummer = 100f;
	public bool Autumn = true;
	public float possibiltyInAutumn = 100f;
	public bool winter = true;
	public float possibiltyInWinter = 100f;

	public List<ParticleSystem> effectParticleSystems = new List<ParticleSystem>();
	public List<float> effectEmmisionRates = new List<float>();
	public bool isLightningStorm;
	public EnviroWeatherCloudConfig cloudConfig;
	public float fogDistance;
	public float sunLightMod = 0.0f;
	public float WindStrenght = 0.5f;
	public float wetnessLevel = 0f;
	public float snowLevel = 0f;
	public AudioClip Sfx;
}

[System.Serializable]
public class EnviroWeatherCloudConfig {
	[Tooltip("Base color of clouds.")]
	public Color FirstColor = Color.white;
	[Tooltip("Light influence from direct lighting.")]
	public float DirectLightInfluence = 1f;
	[Tooltip("Density of clouds generated.")]
	[Range(-0.5f,0.5f)]
	public float Density = -0.2f;
	[Tooltip("Coverage rate of clouds generated.")]
	[Range(-1f,1f)]
	public float Coverage  = 1.0f; // Dense of clouds
	[Tooltip("Clouds alpha modificator.")]
	[Range(0f,10f)]
	public float Alpha = 0.5f;
}


