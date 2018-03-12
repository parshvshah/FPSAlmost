using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnviroWeatherPrefab : MonoBehaviour {

	public string Name;
	[Header("Season Settings")]
	public bool Spring = true;
	[Range(1,100)]
	public float possibiltyInSpring = 100f;
	public bool Summer = true;
	[Range(1,100)]
	public float possibiltyInSummer = 100f;
	public bool Autumn = true;
	[Range(1,100)]
	public float possibiltyInAutumn = 100f;
	public bool winter = true;
	[Range(1,100)]
	public float possibiltyInWinter = 100f;

	[Header("Cloud Settings")]
	public List<EnviroWeatherCloudConfig> cloudConfig = new List<EnviroWeatherCloudConfig>();

	[Header("Linear Fog")]
	public float fogStartDistance;
	public float fogDistance;
	[Header("Exp Fog")]
	public float fogDensity;

	[Header("Advanced Fog Settings:")]
	[Tooltip("Used to modify sky, direct, ambient light and fog color. The color alpha value defines the intensity")]
	public Gradient weatherSkyMod;
	public Gradient weatherLightMod;
	public Gradient weatherFogMod;
	[Range(0f,100f)][Tooltip("The density of height based fog for this weather.")]
	public float heightFogDensity = 1f;
	[Range(0,2)][Tooltip("Define the height of fog rendered in sky.")]
	public float SkyFogHeight = 0.5f;
	[Tooltip("Define the intensity of fog rendered in sky.")]
	[Range(0,1)]
	public float SkyFogIntensity = 0.5f;
	[Range(1,10)][Tooltip("Define the scattering intensity of fog.")]
	public float FogScatteringIntensity = 1f;
	[Range(0,1)][Tooltip("Block the sundisk with fog.")]
	public float fogSunBlocking = 0.25f;

	[Header("Weather Settings")]
	public List<ParticleSystem> effectParticleSystems = new List<ParticleSystem>();
	[Range(0,1)][Tooltip("Wind intensity that will applied to wind zone.")]
	public float WindStrenght = 0.5f;
	[Range(0,1)][Tooltip("The maximum wetness level that can be reached.")]
	public float wetnessLevel = 0f;
	[Range(0,1)][Tooltip("The maximum snow level that can be reached.")]
	public float snowLevel = 0f;
	[Tooltip("Activate this to enable thunder and lightning.")]
	public bool isLightningStorm;

	[Header("Audio Settings - SFX")]
	[Tooltip("Define an sound effect for this weather preset.")]
	public AudioClip weatherSFX;
	[Header("Audio Settings - Ambient")]
	[Tooltip("This sound wil be played in spring at day.(looped)")]
	public AudioClip SpringDayAmbient;
	[Tooltip("This sound wil be played in spring at night.(looped)")]
	public AudioClip SpringNightAmbient;
	[Tooltip("This sound wil be played in summer at day.(looped)")]
	public AudioClip SummerDayAmbient;
	[Tooltip("This sound wil be played in summer at night.(looped)")]
	public AudioClip SummerNightAmbient;
	[Tooltip("This sound wil be played in autumn at day.(looped)")]
	public AudioClip AutumnDayAmbient;
	[Tooltip("This sound wil be played in autumn at night.(looped)")]
	public AudioClip AutumnNightAmbient;
	[Tooltip("This sound wil be played in winter at day.(looped)")]
	public AudioClip WinterDayAmbient;
	[Tooltip("This sound wil be played in winter at night.(looped)")]
	public AudioClip WinterNightAmbient;

	[HideInInspector]
	public List<float> effectEmmisionRates = new List<float>();
}

