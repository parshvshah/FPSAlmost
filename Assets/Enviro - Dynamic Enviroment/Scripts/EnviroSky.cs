////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroSky- Renders a SkyDome with sun,moon,clouds and weather.          ////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class EnviroQualitySettings
{

	[Range(0,1)][Tooltip("Modifies the amount of particles used in weather effects.")]
	public float GlobalParticleEmissionRates = 1f;
	[Tooltip("How often Enviro Growth Instances should be updated. Lower value = smoother growth and more frequent updates but more perfomance hungry!")]
	public float UpdateInterval = 0.5f; //Attention: lower value = smoother growth and more frequent updates but more perfomance hungry!
}
[Serializable]
public class SeasonVariables
{
	public enum Seasons
	{
		Spring,
		Summer,
		Autumn,
		Winter,
	}
	[Tooltip("When enabled the system will change seasons automaticly when enough days passed.")]
	public bool calcSeasons; // if unticked you can manually overwrite current seas. Ticked = automaticly updates seasons
	[Tooltip("The current season.")]
	public Seasons currentSeasons;
	[HideInInspector]
	public Seasons lastSeason;
	[Tooltip("How many days in spring?")]
	public float SpringInDays = 90f;
	[Tooltip("How many days in summer?")]
	public float SummerInDays = 93f;
	[Tooltip("How many days in autumn?")]
	public float AutumnInDays = 92f;
	[Tooltip("How many days in winter?")]
	public float WinterInDays = 90f;
}

[Serializable]
public class AudioVariables // AudioSetup variables
{
	[Tooltip("The prefab with AudioSources used by Enviro. Will be instantiated at runtime.")]
	public GameObject SFXHolderPrefab;

	[Header("Volume Settings:")]
	[Range(0f,1f)][Tooltip("The volume of ambient sounds played by enviro.")]
	public float ambientSFXVolume = 0.5f;
	[Range(0f,1f)][Tooltip("The volume of weather sounds played by enviro.")]
	public float weatherSFXVolume = 1.0f;

	[HideInInspector]public EnviroAudioSource currentAmbientSource;
	[HideInInspector]public float ambientSFXVolumeMod = 0f;
	[HideInInspector]public float weatherSFXVolumeMod = 0f;
}


[Serializable]
public class ObjectVariables // References - setup these in inspector! Or use the provided prefab.
{
	[Tooltip("The Enviro sun object.")]
	public GameObject Sun = null;
	[Tooltip("The Enviro moon object.")]
	public GameObject Moon = null;
	[Tooltip("The Enviro Clouds Holder object.")]
	public GameObject Clouds = null;
	[Tooltip("The directional light for direct sun and moon lighting.")]
	public Transform DirectLight;
	[Tooltip("The Enviro global reflection probe for dynamic reflections.")]
	public ReflectionProbe GlobalReflectionProbe;
}

[Serializable]
public class Satellite 
{
	[Tooltip("Name of this satellite")]
	public string name;
	[Tooltip("Prefab with model that get instantiated.")]
	public GameObject prefab = null;
	[Tooltip("This value will influence the satellite orbitpositions.")]
	public float orbit_X;
	[Tooltip("This value will influence the satellite orbitpositions.")]
	public float orbit_Y;
	[Tooltip("The speed of the satellites orbit.")]
	public float speed;
	[HideInInspector]
	public GameObject satObject;
}

[Serializable]
public class EnviroWeather 
{
	[Header("Weather Setup:")]
	[Tooltip("If disabled the weather will never change.")]
	public bool updateWeather = true;
	[Tooltip("Your WindZone that reflect our weather wind settings.")]
	public WindZone windZone;
	[Tooltip("The Enviro Lighting Flash Component.")]
	public EnviroLightning LightningGenerator; // Creates lightning Flashes
	[Tooltip("A list of all possible thunder audio effects.")]
	public List<AudioClip> ThunderSFX = new List<AudioClip> ();

	[HideInInspector]public List<EnviroWeatherPrefab> WeatherTemplates = new List<EnviroWeatherPrefab>();
	[HideInInspector]public List<GameObject> WeatherPrefabs = new List<GameObject>();

	[Header("Zones:")]
	[Tooltip("List of additional zones. Will be updated on startup!")]
	public List<EnviroZone> zones = new List<EnviroZone>();
	[Tooltip("Tag for zone triggers. Create and assign a tag to this gameObject")]
	public bool useTag;

	[Header("Weather Settings:")]
	[Tooltip("Defines the speed of wetness will raise when it is raining.")]
	public float wetnessAccumulationSpeed = 0.05f;
	[Tooltip("Defines the speed of snow will raise when it is snowing.")]
	public float snowAccumulationSpeed = 0.05f;
	[Tooltip("Defines the speed of clouds will change when weather conditions changed.")]
	public float cloudTransitionSpeed = 1f;
	[Tooltip("Defines the speed of fog will change when weather conditions changed.")]
	public float fogTransitionSpeed = 1f;
	[Tooltip("Defines the speed of particle effects will change when weather conditions changed.")]
	public float effectTransitionSpeed = 1f;

	[Header("Start Weather ID:")]
	public int startWithWeather = 0;

	[Header("Current active zone:")]
	[Tooltip("The current active zone.")]
	public EnviroZone currentActiveZone;

	[Header("Current active weather:")]
	[Tooltip("The current active weather conditions.")]
	public EnviroWeatherPrefab currentActiveWeatherID;

	[HideInInspector]public EnviroWeatherPrefab lastActiveWeatherID;
	[HideInInspector]public GameObject VFXHolder;
	[HideInInspector]public float wetness;
	[HideInInspector]public float curWetness;
	[HideInInspector]public float SnowStrenght;
	[HideInInspector]public float curSnowStrenght;
	[HideInInspector]public int thundersfx;
	[HideInInspector]public EnviroAudioSource currentAudioSource;
}

[Serializable]
public class Sky 
{
	public enum SunAndMoonCalc
	{
		Simple,
		Realistic
	}

	public enum MoonPhases
	{
		Custom,
		Realistic
	}

	public enum SkyboxModi
	{
		Default,
		CustomSkybox,
		CustomColor
	}
	[Header("Sky Mode:")]
	[Tooltip("Select if you want to use enviro skybox your custom material.")]
	public SkyboxModi SkyboxMode;
	[Tooltip("If SkyboxMode == CustomSkybox : Assign your skybox material here!")]
	public Material customSkyboxMaterial;
	[Tooltip("If SkyboxMode == CustomColor : Select your sky color here!")]
	public Color customSkyboxColor;

	[Header("Scattering")]
	[Tooltip("Light Wavelenght used for atmospheric scattering. Keep it near defaults for earthlike atmospheres, or change for alien or fantasy atmospheres for example.")]
	public Vector3 waveLenght;
	[Tooltip("Influence atmospheric scattering.")]
	public float rayleigh = 1f;
	[Tooltip("Sky turbidity. Particle in air. Influence atmospheric scattering.")]
	public float turbidity = 1f;
	[Tooltip("Influence scattering near sun.")]
	public float mie = 1f;
	[Tooltip("Influence scattering near sun.")]
	public float g = 0.75f;
	[Tooltip("Intensity gradient for atmospheric scattering. Influence atmospheric scattering based on current time. 0h - 24h")]
	public AnimationCurve scatteringCurve;
	[Tooltip("Color gradient for atmospheric scattering. Influence atmospheric scattering based on current time. 0h - 24h")]
	public Gradient scatteringColor;

	[Header("Sun")]
	public SunAndMoonCalc SunAndMoonPosition;
	[Tooltip("Intensity of Sun Influence Scale and Dropoff of sundisk.")]
	public float SunIntensity = 50f;
	[Tooltip("Scale of rendered sundisk.")]
	public float SunDiskScale = 50f;
	[Tooltip("Intenisty of rendered sundisk.")]
	public float SunDiskIntensity = 5f;
	[Tooltip("Color gradient for sundisk. Influence sundisk color based on current time. 0h - 24h")]
	public Gradient SunDiskColor;


	[Header("Moon")]
	public MoonPhases MoonPhaseMode;
	[Tooltip("The Moon texture.")]
	public Texture MoonTexture;
	[Tooltip("Brightness of the moon.")]
	public float MoonBrightness = 1.5f;
	[Tooltip("Current Moon phase.-1f - 1f")]
	public float MoonPhase = 0.0f;

	[Header("Tonemapping")]
	[Tooltip("Higher values = brighter sky.")]
	public float SkyExposure = 1.5f;
	[Tooltip("Higher values = brighter sky.")]
	public float SkyLuminence = 1f;
	[Tooltip("Higher values = stronger colors applied BEFORE clouds rendered!")]
	public float SkyColorPower = 2f;

	[HideInInspector]public Color currentWeatherSkyMod;
	[HideInInspector]public Color currentWeatherLightMod;
	[HideInInspector]public Color currentWeatherFogMod;

	[Header("Stars")]
	[Tooltip("A cubemap for night sky.")]
	public Cubemap StarsCubeMap;
	[Tooltip("Intensity of stars based on time of day.")]
	public AnimationCurve StarsIntensity;
}

[Serializable]
public class SatellitesVariables 
{
	[Tooltip("List of satellites.")]
	public List<Satellite> additionalSatellites;
	[Tooltip("Link to the object that hold all additional satellites as childs.")]
	public Transform satelliteHolder;

	[HideInInspector]
	public RenderTexture satRenderTarget;
}

[Serializable]
public class TimeVariables // GameTime variables
{
	public enum TimeProgressMode
	{
		None,
		Simulated,
		SystemTime
	}

	[Header("Date and Time")]
	[Tooltip("None = No time auto time progressing, Simulated = Time calculated with DayLenghtInMinutes, SystemTime = uses your systemTime.")]
	public TimeProgressMode ProgressTime = TimeProgressMode.Simulated;
	[Space(20)]
	[Tooltip("Current Time: minutes")][Range(0,60)]
	public int Seconds  = 0; 
	[Tooltip("Current Time: minutes")][Range(0,60)]
	public int Minutes  = 0; 
	[Tooltip("Current Time: hours")][Range(0,24)]
	public int Hours  = 12; 
	[Tooltip("Current Time: Days")]
	public int Days = 1; 
	[Tooltip("Current Time: Years")]
	public int Years = 1;
	[Space(20)]
	[Tooltip("How long a day needs in real minutes.")]
	public float DayLengthInMinutes = 30; // DayLenght in realtime minutes
	[Header("Location")]
	[Range(-90,90)] [Tooltip("-90,  90   Horizontal earth lines")]
	public float Latitude   = 0f; 
	[Range(-180,180)] [Tooltip("-180, 180  Vertical earth line")]
	public float Longitude  = 0f; 
	[HideInInspector]public float solarTime; 
	[HideInInspector]public float lunarTime;
}

[Serializable] 
public class LightVariables // All Lightning Variables
{
	[Header("Direct")]
	[Tooltip("Color gradient for sun and moon light based on current time. 0h - 24h")]
	public Gradient LightColor;
	[Tooltip("Direct light (sun/moon) intensity based on time of day. 0h - 24h")]
	public AnimationCurve directLightIntensity;
	[Tooltip("Direct light shadowstrenght.")][Range(0f,1f)]
	public float shadowStrenght = 1f;
	[Header("Ambient")]
	[Tooltip("Ambient Rendering Mode.")]
	public UnityEngine.Rendering.AmbientMode ambientMode;
	[Tooltip("Ambientlight ntensity based on current time. 0h - 24h")]
	public AnimationCurve ambientIntensity;
	[Tooltip("Ambientlight sky color.")]
	public Gradient ambientSkyColor;
	[Tooltip("Ambientlight Equator color.")]
	public Gradient ambientEquatorColor;
	[Tooltip("Ambientlight Ground color.")]
	public Gradient ambientGroundColor;
	[HideInInspector]public float SunWeatherMod = 0.0f;

	[Header("Global Reflections")]
	public bool globalReflections;
	public float globalReflectionsIntensity = 1f;
	public float globalReflectionsUpdate = 0.1f;
}


[Serializable]
public class FogSettings 
{
	[Header("Fog is influenced from weather! Check out weather prefabs for more control over fog!")]
	[Tooltip("Unity's fog mode.")]
	public FogMode Fogmode;
	[Tooltip("Use the enviro fog image effect?")]
	public bool AdvancedFog = true;

	[Header("Distance Fog")]
	[Tooltip("Use distance fog?")]
	public bool distanceFog = true;
	[Tooltip("Use radial distance fog?")]
	public bool useRadialDistance = false;
	[Tooltip("The distance where fog starts.")]
	public float startDistance = 0.0f;
	[Range(0f,10f)][Tooltip("The intensity of distance fog.")]
	public float distanceFogIntensity = 0.0f;
	[Range(0f,1f)][Tooltip("The maximum density of fog.")]
	public float maximumFogDensity = 0.1f;

	[Header("Height Fog")]
	[Tooltip("Use heightbased fog?")]
	public bool heightFog = true;
	[Tooltip("The height of heightbased fog.")]
	public float height = 90.0f;
	[Range(0f,10f)][Tooltip("The intensity of heightbased fog.")]
	public float heightFogIntensity = 1f;
	[HideInInspector]
	public float heightDensity = 0.15f;
	[Range(0f,1f)][Tooltip("The noise intensity of heightbased fog.")]
	public float noiseIntensity = 0.01f;
	[Range(0f,0.1f)][Tooltip("The noise scale of heightbased fog.")]
	public float noiseScale = 0.1f;
	[Tooltip("The speed and direction of heightbased fog.")]
	public Vector2 heightFogVelocity;

	[Header("Sky Fog")]
	[Range(0f,1f)][Tooltip("The Intensity of fog in sky.")]
	public float skyFogIntensity = 1f;

	[HideInInspector]
	public float skyFogHeight = 1f;
	[HideInInspector]
	public float skyFogStrength = 0.1f;
	[HideInInspector]
	public float scatteringStrenght = 0.5f;
	[HideInInspector]
	public float sunBlocking = 0.5f;
}

[Serializable]
public class LightShaftsSettings 
{
	[Tooltip("Use light shafts?")]
	public bool sunLightShafts = true;
	public bool moonLightShafts = true;

	[Header("Quality Settings")]
	[Tooltip("Lightshafts resolution quality setting.")]
	public EnviroLightShafts.SunShaftsResolution resolution = EnviroLightShafts.SunShaftsResolution.Normal;
	[Tooltip("Lightshafts blur mode.")]
	public EnviroLightShafts.ShaftsScreenBlendMode screenBlendMode = EnviroLightShafts.ShaftsScreenBlendMode.Screen;
	[Tooltip("Use cameras depth to hide lightshafts?")]
	public bool useDepthTexture = true;

	[Header("Intensity Settings")]
	[Tooltip("Color gradient for lightshafts based on sun position.")]
	public Gradient lightShaftsColorSun;
	[Tooltip("Color gradient for lightshafts based on moon position.")]
	public Gradient lightShaftsColorMoon;
	[Tooltip("Treshhold gradient for lightshafts based on sun position. This will influence lightshafts intensity!")]
	public Gradient thresholdColorSun;
	[Tooltip("Treshhold gradient for lightshafts based on moon position. This will influence lightshafts intensity!")]
	public Gradient thresholdColorMoon;
	[Tooltip("Radius of blurring applied.")]
	public float blurRadius = 2.5f;
	[Tooltip("Global Lightshafts intensity.")]
	public float intensity = 1.15f;
	[Tooltip("Lightshafts maximum radius.")]
	public float maxRadius = 0.75f;
}
[Serializable]
public class EnviroCloudsLayer 
{
	public string Name;
	[Header("Mesh Setup")]
	[Range(1,100)][Tooltip("Clouds Quality. High Performance Impact! Call InitClouds() to apply change in runtime.")]
	public int Quality = 25;
	[Tooltip("Segments of generated clouds mesh. Good for curvate meshes. If curved disabled keep it low.")]
	public int segmentCount = 3;
	[Tooltip("Thickness of generated clouds mesh.")]
	public float thickness = 0.4f;
	[Tooltip("Clouds mesh curved at horizon?")]
	public bool curved;
	[Tooltip("Clouds mesh curve intensity.")]
	public float curvedIntensity = 0.1f;

	[Header("Material Setup")]
	[Tooltip("The texture used for this cloud layer.")]
	public Texture myCloudsTexture;
	[Range(0.5f,2f)][Tooltip("Clouds tiling/scale modificator.")]
	public float Scaling = 1f; 
	[Tooltip("Enable to let cat this cloudlayer shadows.")]
	public bool canCastShadows = false;
	[Tooltip("Altitude of this cloud layer")]
	public float layerAltitude = 0.1f;
	[Tooltip("Offset used for multi layer clouds rendering.")]
	public float LayerOffset = 0.5f;

	[HideInInspector]
	public GameObject myObj;
	[HideInInspector]
	public Material myMaterial;
	[HideInInspector]
	public Material myShadowMaterial;

	[HideInInspector]
	public float DirectLightIntensity = 1f;
	[HideInInspector][Tooltip("Base color of clouds.")]
	public Color FirstColor;
	[HideInInspector][Tooltip("Coverage rate of clouds generated.")]
	public float Coverage = 1.0f; // 
	[HideInInspector][Tooltip("Density of clouds generated.")]
	public float Density = 1.0f; 
	[HideInInspector][Tooltip("Clouds alpha modificator.")]
	public float Alpha = 0.5f;
}


[Serializable]
public class CloudVariables // Default cloud settings, will be changed on runtime if Weather is enabled!
{
	public enum CloudRenderResolution
	{
		Legacy,
		Low,
		Medium,
		High,
		Ultra
	}

	[Header("Clouds layers")]
	public List<EnviroCloudsLayer> cloudsLayers = new List<EnviroCloudsLayer> ();

	[Range(1f,3f)][Header("Global Clouds Position")]
	public float worldScale = 1f;
	[Tooltip("When enabled, clouds will stay at this height. When disabled clouds height will be calculated by player position.")]
	public bool FixedAltitude = false;
	[Tooltip("The altitude of the clouds when 'FixedAltitude' is enabled.")]
	public float cloudsAltitude = 0.2f;
	[Header("Clouds Wind Animation")]
	[Range(-1f,1f)][Tooltip("Time scale / wind animation speed of clouds.")]
	public float cloudsTimeScale = 1f;
	[Range(0f,0.1f)][Tooltip("Global clouds wind speed modificator.")]
	public float cloudsWindStrenghtModificator = 0.025f;

	public bool useWindZoneDirection;
	[Range(-1f,1f)][Tooltip("Global clouds wind direction X axes.")]
	public float cloudsWindDirectionX = 1f;
	[Range(-1f,1f)][Tooltip("Global clouds wind direction Y axes.")]
	public float cloudsWindDirectionY = 1f;

	[Header("Cloud Rendering")]
	public CloudRenderResolution cloudsRenderQuality = CloudRenderResolution.Low;

	[Header("Cloud Lighting")]
	[Tooltip("Global Color for clouds based sun positon.")]
	public Gradient skyColor;
	[Tooltip("Global Color for clouds based sun positon.")]
	public Gradient sunHighlightColor;
	[Tooltip("Global Color for clouds based sun positon.")]
	public Gradient moonHighlightColor;
	[Tooltip("Direct Light intensity for clouds based on time of day.")]
	public AnimationCurve LightIntensity;

	[HideInInspector]
	public RenderTexture cloudsRenderTarget;
}

[ExecuteInEditMode]
[AddComponentMenu("Enviro/Sky System")]
public class EnviroSky : MonoBehaviour
{
	private static EnviroSky _instance; // Creat a static instance for easy access!

	public static EnviroSky instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<EnviroSky>();
			return _instance;
		}
	}

	[Header("Player and Camera")]
	[Tooltip("Assign your player gameObject here. Required Field! or enable AssignInRuntime!")]
	public GameObject Player;
	[Tooltip("Assign your main camera here. Required Field! or enable AssignInRuntime!")]
	public Camera PlayerCamera;
	[Tooltip("If enabled Enviro will search for your Player and Camera by Tag!")]
	public bool AssignInRuntime;
	[Tooltip("Your Player Tag")]
	public string PlayerTag = "";
	[Tooltip("Your CameraTag")]
	public string CameraTag = "MainCamera";

	[Header("Rendering Setup")]
	[Tooltip("This is the layer id for your clouds and satellites to get projected directly into skybox.!")]
	public int skyRenderingLayer = 30;
	[Tooltip("This is the layer id for all satellites like moons, planets.")]
	public int satelliteRenderingLayer = 31;

	[Header("Settings")]
	// Parameters
	public ObjectVariables Components = null;
	public AudioVariables Audio = null;
	public TimeVariables GameTime = null;
	public LightVariables Lighting = null;
	public Sky  Sky = null;
	public EnviroWeather Weather = null;
	public SeasonVariables Seasons = null;
	public CloudVariables Clouds = null;
	public FogSettings Fog = null;
	public LightShaftsSettings LightShafts = null;
	public SatellitesVariables Satellites = null;
	public EnviroQualitySettings Quality = null;

	[HideInInspector]public bool started;
	[HideInInspector]public bool isNight = true;
	[HideInInspector]public EnviroFog atmosphericFog;
	[HideInInspector]public EnviroLightShafts lightShaftsScriptSun;
	[HideInInspector]public EnviroLightShafts lightShaftsScriptMoon;
	[HideInInspector]public EnviroSkyRendering EnviroCloudsRender;
	[HideInInspector]public GameObject EffectsHolder;
	[HideInInspector]public EnviroAudioSource AudioSourceWeather;
	[HideInInspector]public EnviroAudioSource AudioSourceWeather2;
	[HideInInspector]public EnviroAudioSource AudioSourceAmbient;
	[HideInInspector]public EnviroAudioSource AudioSourceAmbient2;
	[HideInInspector]public AudioSource AudioSourceThunder;

	[HideInInspector]
	public List<EnviroVegetationInstance> EnviroVegetationInstances = new List<EnviroVegetationInstance>(); // All EnviroInstance that getting updated at the moment.

	// Used from other Enviro componets
	[HideInInspector]
	public float internalHour;
	[HideInInspector]
	public float currentHour;
	[HideInInspector]
	public float currentDay;
	[HideInInspector]
	public float currentYear;
	[HideInInspector]
	public float currentTimeInHours;

	//Some Pointers
	private Transform DomeTransform;

	private Material  VolumeCloudShader1;
	private Material  VolumeCloudShadowShader1;    
	private Material  VolumeCloudShader2;
	private Material  VolumeCloudShadowShader2;

	private Transform SunTransform;
	private Light     MainLight;
	private Transform MoonTransform;
	private Renderer  MoonRenderer;
	private Material  MoonShader;

	private float lastHourUpdate;
	private float starsRot;
	private float lastHour;
	private float lastRelfectionUpdate;

	private float OrbitRadius
	{
		get { return DomeTransform.localScale.x; }
	}
	private bool serverMode = false;

	// Scattering constants
	const float pi = Mathf.PI;
	private Vector3 K =  new Vector3(686.0f, 678.0f, 666.0f);
	private const float n =  1.0003f;   
	private const float N =  2.545E25f;
	private const float pn =  0.035f;    

	private Camera cloudsCamera;
	private Camera satCamera;
	private GameObject cloudsCamObj;
	private Vector2 cloudAnim;
	private float hourTime;
	private float E0 = 0f;
	private float E1 = 0f;
	private float LST;

	// Events
	public delegate void HourPassed();
	public delegate void DayPassed();
	public delegate void YearPassed();
	public delegate void WeatherChanged(EnviroWeatherPrefab weatherType);
	public delegate void ZoneWeatherChanged(EnviroWeatherPrefab weatherType,EnviroZone zone);
	public delegate void SeasonChanged(SeasonVariables.Seasons season);
	public delegate void isNightE();
	public delegate void isDay();
	public event HourPassed OnHourPassed;
	public event DayPassed OnDayPassed;
	public event YearPassed OnYearPassed;
	public event WeatherChanged OnWeatherChanged;
	public event ZoneWeatherChanged OnZoneWeatherChanged;
	public event SeasonChanged OnSeasonChanged;
	public event isNightE OnNightTime;
	public event isDay OnDayTime;
	///

	// Events:
	public virtual void NotifyHourPassed()
	{
		if(OnHourPassed != null)
			OnHourPassed();
	}
	public virtual void NotifyDayPassed()
	{
		if(OnDayPassed != null)
			OnDayPassed();
	}
	public virtual void NotifyYearPassed()
	{
		if(OnYearPassed != null)
			OnYearPassed();
	}
	public virtual void NotifyWeatherChanged(EnviroWeatherPrefab type)
	{
		if(OnWeatherChanged != null)
			OnWeatherChanged (type);
	}
	public virtual void NotifyZoneWeatherChanged(EnviroWeatherPrefab type, EnviroZone zone)
	{
		if(OnZoneWeatherChanged != null)
			OnZoneWeatherChanged (type,zone);
	}
	public virtual void NotifySeasonChanged(SeasonVariables.Seasons season)
	{
		if(OnSeasonChanged != null)
			OnSeasonChanged (season);
	}
	public virtual void NotifyIsNight()
	{
		if(OnNightTime != null)
			OnNightTime ();
	}
	public virtual void NotifyIsDay()
	{
		if(OnDayTime != null)
			OnDayTime ();
	}
		
	void Start()
	{
		started = false;
		SetTime (GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
		lastHourUpdate = Mathf.RoundToInt(internalHour);
		currentTimeInHours = GetInHours (internalHour, GameTime.Days, GameTime.Years);
		//lastHourUpdate = currentTimeInHours;

		//Weather Setup
		Weather.currentActiveWeatherID = Weather.zones[0].zoneWeatherPrefabs[0].GetComponent<EnviroWeatherPrefab>();
		Weather.lastActiveWeatherID = Weather.currentActiveWeatherID;

		InvokeRepeating("UpdateEnviroment", 0, Quality.UpdateInterval);

		CreateEffects ();  //Create Weather Effects Holder

		if (PlayerCamera != null && Player != null && AssignInRuntime == false)
		{
			Init ();
			started = true;
		}
	}

	void OnEnable()
	{
		DomeTransform = transform;

		if (AssignInRuntime) 
		{
			started = false;	//Wait for assignment
		} 
		else if (PlayerCamera != null && Player != null)
		{
			Init ();
			started = true;
		}
	}

	void Init ()
	{
		// Check time
		if (GameTime.solarTime < 0.5f)
			isNight = true;
		else
			isNight = false;

		//return when in server mode!
		if (serverMode)
			return;

		InitImageEffects ();
		CheckSatellites ();

		// Setup Fog Mode
		RenderSettings.fogMode = Fog.Fogmode;

		// Setup Skybox Material
		if (Sky.SkyboxMode == Sky.SkyboxModi.Default) {
			Material sky = new Material (Shader.Find ("Enviro/Skybox"));
			sky.SetTexture ("_Stars", Sky.StarsCubeMap);
			RenderSettings.skybox = sky;
		} else if (Sky.SkyboxMode == Sky.SkyboxModi.CustomSkybox) {
			if(Sky.customSkyboxMaterial != null)
				RenderSettings.skybox = Sky.customSkyboxMaterial;
		}

		RenderSettings.ambientMode = Lighting.ambientMode;
		RenderSettings.fogDensity = 0f;

		Components.GlobalReflectionProbe.size = transform.localScale;
		Components.GlobalReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;

		// Setup Camera
		if (PlayerCamera != null) 
		{
			PlayerCamera.clearFlags = CameraClearFlags.SolidColor;
			PlayerCamera.backgroundColor = Color.black;
			PlayerCamera.hdr = true;
			Components.GlobalReflectionProbe.farClipPlane = PlayerCamera.farClipPlane;
		}

		CreateCameraHolder ();
		InitSatCamera ();
		InitClouds ();


		if (Components.Sun) { SunTransform = Components.Sun.transform; } else { Debug.LogError("Please set Sun object in inspector!"); }

		if (Components.Moon){
			MoonTransform = Components.Moon.transform;
			MoonRenderer = Components.Moon.GetComponent<Renderer>();

			if (MoonRenderer == null)
				MoonRenderer = Components.Moon.AddComponent<MeshRenderer> ();

			MoonRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			MoonRenderer.receiveShadows = false;

			if (MoonRenderer.sharedMaterial != null)
				DestroyImmediate (MoonRenderer.sharedMaterial);

			if(Sky.MoonPhaseMode == Sky.MoonPhases.Realistic)
			MoonShader = new Material (Shader.Find ("Enviro/MoonShader"));
			else
			MoonShader = new Material (Shader.Find ("Enviro/MoonShaderPhased"));
			
			MoonShader.SetTexture ("_MainTex", Sky.MoonTexture);

			MoonRenderer.sharedMaterial = MoonShader;
		}
		else { Debug.LogError("Please set Moon object in inspector!"); }

		if (Components.DirectLight) { MainLight = Components.DirectLight.GetComponent<Light>(); 
		} else { Debug.LogError ("Please set Direct Light object in inspector!"); }
	}

	void InitImageEffects ()
	{
		atmosphericFog = PlayerCamera.gameObject.GetComponent<EnviroFog> ();

		if (atmosphericFog != null) 
		{
			DestroyImmediate (atmosphericFog.fogMaterial);
			atmosphericFog.fogMaterial = new Material (Shader.Find ("Enviro/Fog"));
			atmosphericFog.fogShader = atmosphericFog.fogMaterial.shader;
		}
		else
		{
			atmosphericFog = PlayerCamera.gameObject.AddComponent<EnviroFog> ();
			atmosphericFog.fogMaterial = new Material (Shader.Find ("Enviro/Fog"));
			atmosphericFog.fogShader = atmosphericFog.fogMaterial.shader;
		}

		EnviroLightShafts[] shaftScripts = PlayerCamera.gameObject.GetComponents<EnviroLightShafts>();

		if(shaftScripts.Length > 0)
			lightShaftsScriptSun = shaftScripts [0];

		if (lightShaftsScriptSun != null) 
		{
			DestroyImmediate (lightShaftsScriptSun.sunShaftsMaterial);
			DestroyImmediate (lightShaftsScriptSun.simpleClearMaterial);
			lightShaftsScriptSun.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptSun.sunShaftsShader = lightShaftsScriptSun.sunShaftsMaterial.shader;
			lightShaftsScriptSun.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptSun.simpleClearShader = lightShaftsScriptSun.simpleClearMaterial.shader;
		}
		else
		{
			lightShaftsScriptSun = PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
			lightShaftsScriptSun.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptSun.sunShaftsShader = lightShaftsScriptSun.sunShaftsMaterial.shader;
			lightShaftsScriptSun.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptSun.simpleClearShader = lightShaftsScriptSun.simpleClearMaterial.shader;
		}

		if(shaftScripts.Length > 1)
			lightShaftsScriptMoon = shaftScripts [1];

		if (lightShaftsScriptMoon != null) 
		{
			DestroyImmediate (lightShaftsScriptMoon.sunShaftsMaterial);
			DestroyImmediate (lightShaftsScriptMoon.simpleClearMaterial);
			lightShaftsScriptMoon.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptMoon.sunShaftsShader = lightShaftsScriptMoon.sunShaftsMaterial.shader;
			lightShaftsScriptMoon.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptMoon.simpleClearShader = lightShaftsScriptMoon.simpleClearMaterial.shader;
		}
		else
		{
			lightShaftsScriptMoon = PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
			lightShaftsScriptMoon.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptMoon.sunShaftsShader = lightShaftsScriptMoon.sunShaftsMaterial.shader;
			lightShaftsScriptMoon.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptMoon.simpleClearShader = lightShaftsScriptMoon.simpleClearMaterial.shader;
		}
	}
		

	void CreateCameraHolder ()
	{
		// Destroy Rendering Objects
		if (cloudsCamObj != null)
			DestroyImmediate(cloudsCamObj);

		DestroyImmediate(GameObject.Find ("Enviro Rendering"));
		// Create new Holder
		GameObject cam = new GameObject ();
		cam.name = "Enviro Rendering";
		cam.transform.SetParent (PlayerCamera.transform.parent);
		cam.transform.localPosition = PlayerCamera.transform.localPosition;
		cam.transform.localRotation = PlayerCamera.transform.localRotation;
		cloudsCamObj = cam;
	}


	/// <summary>
	/// Recreates clouds and applies all settings.////
	/// </summary>
	public void InitClouds ()
	{
		int childs = Components.Clouds.transform.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(Components.Clouds.transform.GetChild(i).gameObject);
		}


		for (int i = 0; i < Clouds.cloudsLayers.Count; i++) 
		{
			// Create Cloud layer Object, Material, Renderer and Mesh
			GameObject layer = new GameObject();
			layer.name = "Clouds Layer: " + i.ToString ();
			layer.transform.SetParent (Components.Clouds.transform);
			layer.transform.localEulerAngles = new Vector3(0f,0f,-180f);
			layer.transform.localScale = new Vector3(1f * Clouds.worldScale, 1f,1f * Clouds.worldScale);
			layer.transform.localPosition = new Vector3 (0f, Clouds.cloudsLayers [i].layerAltitude, 0f);
			layer.layer = 30;
			Clouds.cloudsLayers [i].myObj = layer;
			MeshFilter layerMeshFilter = layer.AddComponent<MeshFilter> ();
			MeshRenderer layerMeshRenderer = layer.AddComponent<MeshRenderer> ();
			Clouds.cloudsLayers[i].myMaterial = new Material (Shader.Find ("Enviro/Clouds"));
			layerMeshRenderer.sharedMaterial = Clouds.cloudsLayers [i].myMaterial;
			layerMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			layerMeshFilter.sharedMesh = CreateCloudsLayer (i, false);
			Clouds.cloudsLayers [i].myMaterial.SetTexture ("_CloudsMap", Clouds.cloudsLayers [i].myCloudsTexture);

			// Create CloudShadows layer Object, Material, Renderer and Mesh
			if (Clouds.cloudsLayers[i].canCastShadows) {
				GameObject layerShadows = new GameObject ();
				layerShadows.name = "CloudsShadows Layer: " + i.ToString ();
				layerShadows.transform.SetParent (Components.Clouds.transform);
				layerShadows.transform.localEulerAngles = new Vector3 (0f, 0f, -180f);
				layerShadows.transform.localScale = new Vector3(1f * Clouds.worldScale, 1f,1f * Clouds.worldScale);
				layerShadows.transform.localPosition = new Vector3 (0f, Clouds.cloudsLayers [i].layerAltitude, 0f);
				MeshFilter layerShadowsMeshFilter = layerShadows.AddComponent<MeshFilter> ();
				MeshRenderer layerShadowsMeshRenderer = layerShadows.AddComponent<MeshRenderer> ();
				layerShadowsMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
				Clouds.cloudsLayers [i].myShadowMaterial = new Material (Shader.Find ("Enviro/CloudsShadows"));
				layerShadowsMeshRenderer.sharedMaterial = Clouds.cloudsLayers [i].myShadowMaterial;
				layerShadowsMeshFilter.sharedMesh = CreateCloudsLayer (i, true);
				Clouds.cloudsLayers [i].myShadowMaterial.SetTexture ("_CloudsMap", Clouds.cloudsLayers [i].myCloudsTexture);
			}
		}

		if (cloudsCamera == null && Clouds.cloudsRenderQuality != CloudVariables.CloudRenderResolution.Legacy) {

			// Search all cams and remove sky layer from cullingmask!
			Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
			for (int i = 0; i < cams.Length; i++) 
			{
				cams[i].cullingMask &= ~(1 << skyRenderingLayer);
			}

			DestroyImmediate(GameObject.Find ("Enviro Sky Camera"));

			GameObject camObj = new GameObject ();	

			camObj.name = "Enviro Sky Camera";
			camObj.transform.SetParent (cloudsCamObj.transform);
			camObj.transform.localPosition = Vector3.zero;
			camObj.transform.localRotation = Quaternion.identity;
			cloudsCamera = camObj.AddComponent<Camera> ();
			camObj.AddComponent <EnviroCamera>();
			cloudsCamera.farClipPlane = PlayerCamera.farClipPlane * Clouds.worldScale;
			cloudsCamera.nearClipPlane = PlayerCamera.nearClipPlane;
			cloudsCamera.aspect = PlayerCamera.aspect;
			cloudsCamera.hdr = true;
			cloudsCamera.renderingPath = RenderingPath.Forward;
			cloudsCamera.fieldOfView = PlayerCamera.fieldOfView;
			if (Sky.SkyboxMode != Sky.SkyboxModi.CustomColor)
				cloudsCamera.clearFlags = CameraClearFlags.Skybox;
			else {
				cloudsCamera.clearFlags = CameraClearFlags.SolidColor;
				cloudsCamera.backgroundColor = Sky.customSkyboxColor;
			}
			cloudsCamera.cullingMask = (1 << skyRenderingLayer);
			cloudsCamera.stereoTargetEye = StereoTargetEyeMask.Both;

		} 
		else if (Clouds.cloudsRenderQuality == CloudVariables.CloudRenderResolution.Legacy) 
		{
			for (int i = 0; i < Clouds.cloudsLayers.Count; i++) 
			{
				Clouds.cloudsLayers [i].myObj.layer = 1;
			}
			Components.Moon.layer = 1;
		}

		switch (Clouds.cloudsRenderQuality) 
		{
		case CloudVariables.CloudRenderResolution.Low:
			Clouds.cloudsRenderTarget = new RenderTexture (Screen.currentResolution.width / 4, Screen.currentResolution.height / 4, 16);
			break;
		case CloudVariables.CloudRenderResolution.Medium:
			Clouds.cloudsRenderTarget = new RenderTexture (Screen.currentResolution.width / 3, Screen.currentResolution.height / 3, 16);
			break;
		case CloudVariables.CloudRenderResolution.High:
			Clouds.cloudsRenderTarget = new RenderTexture (Screen.currentResolution.width / 2, Screen.currentResolution.height / 2,16);
			break;
		case CloudVariables.CloudRenderResolution.Ultra:
			Clouds.cloudsRenderTarget = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height,16);
			break;
		}

		// Update Cloud Rendering Camera
		if (cloudsCamera != null) 
			cloudsCamera.targetTexture = Clouds.cloudsRenderTarget;

		if (PlayerCamera != null)
			AddEnviroCloudsRendering ();
		
			
	}

	private void AddEnviroCloudsRendering ()
	{
		EnviroCloudsRender = PlayerCamera.gameObject.GetComponent<EnviroSkyRendering> ();

		if(EnviroCloudsRender == null)
			EnviroCloudsRender = PlayerCamera.gameObject.AddComponent<EnviroSkyRendering> ();

		Material mat = new Material (Shader.Find ("Enviro/SkyRendering"));
		mat.SetTexture("_Clouds",cloudsCamera.targetTexture);
		mat.SetTexture("_Satellites",satCamera.targetTexture);
		EnviroCloudsRender.material = mat;
		EnviroCloudsRender.skyRenderTexture = cloudsCamera.targetTexture;
		EnviroCloudsRender.Apply ();
	}

	public void InitSatCamera ()
	{
		Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
		for (int i = 0; i < cams.Length; i++) 
		{
			cams[i].cullingMask &= ~(1 << satelliteRenderingLayer);
		}

		DestroyImmediate(GameObject.Find ("Enviro Effect Camera"));

		GameObject camObj = new GameObject ();	

		camObj.name = "Enviro Effect Camera";
		camObj.transform.SetParent (cloudsCamObj.transform);
		camObj.transform.localPosition = Vector3.zero;
		camObj.transform.localRotation = Quaternion.identity;

		satCamera = camObj.AddComponent<Camera> ();
		satCamera.farClipPlane = PlayerCamera.farClipPlane;
		satCamera.nearClipPlane = PlayerCamera.nearClipPlane;
		satCamera.aspect = PlayerCamera.aspect;
		satCamera.hdr = true;
		satCamera.renderingPath = RenderingPath.Forward;
		satCamera.fieldOfView = PlayerCamera.fieldOfView;
		satCamera.clearFlags = CameraClearFlags.SolidColor;
		satCamera.backgroundColor = Color.black;
		satCamera.cullingMask = (1 << satelliteRenderingLayer);
		satCamera.depth = PlayerCamera.depth + 1;

		satCamera.enabled = true;
		PlayerCamera.cullingMask &= ~(1 << satelliteRenderingLayer);

		Components.Moon.layer = satelliteRenderingLayer;

		Satellites.satRenderTarget = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height,16);
		satCamera.targetTexture = Satellites.satRenderTarget;
	}


	public void CreateEffects ()
	{
			GameObject old = GameObject.Find ("Enviro Effects");

			if (old != null)
				DestroyImmediate (old);

			EffectsHolder = new GameObject ();
			EffectsHolder.name = "Enviro Effects";
			if(Player != null)
				EffectsHolder.transform.position = Player.transform.position;
			else
				EffectsHolder.transform.position = EnviroSky.instance.transform.position;


			CreateWeatherEffectHolder ();

			GameObject SFX = (GameObject)Instantiate (Audio.SFXHolderPrefab, Vector3.zero, Quaternion.identity);

			SFX.transform.parent = EffectsHolder.transform;

			EnviroAudioSource[] srcs = SFX.GetComponentsInChildren<EnviroAudioSource> ();

			for (int i = 0; i < srcs.Length; i++) 
			{
				switch (srcs [i].myFunction) {
				case EnviroAudioSource.AudioSourceFunction.Weather1:
					AudioSourceWeather = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Weather2:
					AudioSourceWeather2 = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Ambient:
					AudioSourceAmbient = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Ambient2:
					AudioSourceAmbient2 = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Thunder:
					AudioSourceThunder = srcs [i].audiosrc;
					break;
				}
			}
			
		Weather.currentAudioSource = AudioSourceWeather; 
		Audio.currentAmbientSource = AudioSourceAmbient;
		TryPlayAmbientSFX ();
	}

	// EnviroGrowInstancesSeason Component will register on start!
	public int RegisterMe (EnviroVegetationInstance me) 
	{
		EnviroVegetationInstances.Add (me);
		return EnviroVegetationInstances.Count - 1;
	}

	// Manuell change season.
	public void ChangeSeason (SeasonVariables.Seasons season)
	{
		Seasons.currentSeasons = season;
		NotifySeasonChanged (season);
	}

	// Update the Season according gameDays
	void UpdateSeason ()
	{
		
		if (currentDay >= 0 && currentDay < Seasons.SpringInDays)
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Spring;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Spring);

			Seasons.lastSeason = Seasons.currentSeasons;
		} 
		else if (currentDay >= Seasons.SpringInDays && currentDay < (Seasons.SpringInDays + Seasons.SummerInDays))
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Summer;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Summer);

			Seasons.lastSeason = Seasons.currentSeasons;
		} 
		else if (currentDay >= (Seasons.SpringInDays + Seasons.SummerInDays) && currentDay < (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays)) 
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Autumn;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Autumn);

			Seasons.lastSeason = Seasons.currentSeasons;
		}
		else if(currentDay >= (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays) && currentDay <= (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays + Seasons.WinterInDays))
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Winter;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Winter);

			Seasons.lastSeason = Seasons.currentSeasons;
		}
	}

	void PlayAmbient (AudioClip sfx)
	{
		if (sfx == Audio.currentAmbientSource.audiosrc.clip) {
			Audio.currentAmbientSource.FadeIn (sfx);
			return;
		}
			if (Audio.currentAmbientSource == AudioSourceAmbient){
				AudioSourceAmbient.FadeOut();
				AudioSourceAmbient2.FadeIn(sfx);
				Audio.currentAmbientSource = AudioSourceAmbient2;
			}
			else if (Audio.currentAmbientSource == AudioSourceAmbient2){
				AudioSourceAmbient2.FadeOut();
				AudioSourceAmbient.FadeIn(sfx);
				Audio.currentAmbientSource = AudioSourceAmbient;
			}
	}


	void TryPlayAmbientSFX ()
	{
		if (Weather.currentActiveWeatherID == null)
			return;
		
		if (isNight) 
		{
			switch (Seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				if (Weather.currentActiveWeatherID.SpringNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.SpringNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
			break;

			case SeasonVariables.Seasons.Summer:
				if (Weather.currentActiveWeatherID.SummerNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.SummerNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case SeasonVariables.Seasons.Autumn:
				if (Weather.currentActiveWeatherID.AutumnNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.AutumnNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case SeasonVariables.Seasons.Winter:
				if (Weather.currentActiveWeatherID.WinterNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.WinterNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			}
		} 
		else 
		{
			switch (Seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				if (Weather.currentActiveWeatherID.SpringDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.SpringDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case SeasonVariables.Seasons.Summer:
				if (Weather.currentActiveWeatherID.SummerDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.SummerDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case SeasonVariables.Seasons.Autumn:
				if (Weather.currentActiveWeatherID.AutumnDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.AutumnDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case SeasonVariables.Seasons.Winter:
				if (Weather.currentActiveWeatherID.WinterDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherID.WinterDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			}
		}
	}

	void UpdateEnviroment () // Update the all GrowthInstances
	{
		// Set correct Season.
		if(Seasons.calcSeasons)
			UpdateSeason ();

		// Update all EnviroGrowInstancesSeason in scene!
		if (EnviroVegetationInstances.Count > 0) 
		{
			for (int i = 0; i < EnviroVegetationInstances.Count; i++) {
				if (EnviroVegetationInstances [i] != null)
					EnviroVegetationInstances [i].UpdateInstance ();

			}
		}
	}

	void CreateSatellite (int id)
	{
		GameObject sat = (GameObject)Instantiate (Satellites.additionalSatellites [id].prefab, transform.position + new Vector3 (0f, 500f, 0f), Quaternion.identity);
		sat.layer = satelliteRenderingLayer;
		sat.transform.parent = Satellites.satelliteHolder;
		Satellites.additionalSatellites [id].satObject = sat;
	}

	void CheckSatellites ()
	{
		int childs = Satellites.satelliteHolder.childCount;
		for (int i = childs-1; i >= 0; i--) 
		{
			DestroyImmediate (Satellites.satelliteHolder.GetChild (i).gameObject);
		}

		for (int i = 0; i < Satellites.additionalSatellites.Count; i++) 
		{
			CreateSatellite (i);
		}
	}


	void UpdateQuality()
	{
		//Update Fog
		if (atmosphericFog != null) 
		{
			atmosphericFog.distanceFog = Fog.distanceFog;
			atmosphericFog.heightFog = Fog.heightFog;
			atmosphericFog.height = Fog.height;
			atmosphericFog.heightDensity = Fog.heightDensity;
			atmosphericFog.useRadialDistance = Fog.useRadialDistance;
			atmosphericFog.startDistance = Fog.startDistance;

			if (Fog.AdvancedFog)
				atmosphericFog.enabled = true;
			else
				atmosphericFog.enabled = false;
		}

		//Update LightShafts
		if (lightShaftsScriptSun != null) 
		{
			lightShaftsScriptSun.resolution = LightShafts.resolution;
			lightShaftsScriptSun.screenBlendMode = LightShafts.screenBlendMode;
			lightShaftsScriptSun.useDepthTexture = LightShafts.useDepthTexture;
			lightShaftsScriptSun.sunThreshold = LightShafts.thresholdColorSun.Evaluate (GameTime.solarTime);

			lightShaftsScriptSun.sunShaftBlurRadius = LightShafts.blurRadius;
			lightShaftsScriptSun.sunShaftIntensity = LightShafts.intensity;
			lightShaftsScriptSun.maxRadius = LightShafts.maxRadius;
			lightShaftsScriptSun.sunColor = LightShafts.lightShaftsColorSun.Evaluate (GameTime.solarTime);
			lightShaftsScriptSun.sunTransform = Components.Sun.transform;
			lightShaftsScriptSun.moonTransform = Components.Moon.transform;

			if (LightShafts.sunLightShafts) {
				lightShaftsScriptSun.enabled = true;
			} else {
				lightShaftsScriptSun.enabled = false;
			}
		}

		if (lightShaftsScriptMoon != null) 
		{
			lightShaftsScriptMoon.resolution = LightShafts.resolution;
			lightShaftsScriptMoon.screenBlendMode = LightShafts.screenBlendMode;
			lightShaftsScriptMoon.useDepthTexture = LightShafts.useDepthTexture;
			lightShaftsScriptMoon.sunThreshold = LightShafts.thresholdColorMoon.Evaluate (GameTime.lunarTime);
   

			lightShaftsScriptMoon.sunShaftBlurRadius = LightShafts.blurRadius;
			lightShaftsScriptMoon.sunShaftIntensity = Mathf.Clamp ((LightShafts.intensity - GameTime.solarTime),0,100);
			lightShaftsScriptMoon.maxRadius = LightShafts.maxRadius;
			lightShaftsScriptMoon.sunColor = LightShafts.lightShaftsColorMoon.Evaluate (GameTime.lunarTime);
			lightShaftsScriptMoon.sunTransform = Components.Moon.transform;

			if (LightShafts.moonLightShafts) {
				lightShaftsScriptMoon.enabled = true;
			} else {
				lightShaftsScriptMoon.enabled = false;
			}
		}
	}

	Vector3 CalculatePosition ()
	{
		Vector3 newPosition;
		newPosition.x = Player.transform.position.x;
		newPosition.z = Player.transform.position.z;
		newPosition.y = Player.transform.position.y;

		return newPosition;
	}

	void UpdateSatellites ()
	{
		for (int i = 0; i < Satellites.additionalSatellites.Count; i++)
		{
			Satellites.additionalSatellites[i].satObject.transform.localPosition = UpdateSatellitePosition (Satellites.additionalSatellites[i].orbit_X,Satellites.additionalSatellites[i].orbit_Y,Satellites.additionalSatellites[i].speed);
		}
	}

	void Update()
	{
		if (!started) 
		{
			if (AssignInRuntime && PlayerTag != "" && CameraTag != "" && Application.isPlaying) {
				Player = GameObject.FindGameObjectWithTag (PlayerTag);
				PlayerCamera = GameObject.FindGameObjectWithTag (CameraTag).GetComponent<Camera>();

				if (Player != null && PlayerCamera != null) {
					Init ();
					started = true;
				}
				else  {started = false; return;}
			} else {started = false; return;}
		}

		UpdateTime ();
		ValidateParameters();

		if (!serverMode) {
			UpdateQuality ();
			UpdateAmbientLight ();
			UpdateReflections ();
			UpdateWeather ();
			UpdateSatellites ();

			if (EffectsHolder != null)
				EffectsHolder.transform.position = Player.transform.position;

			if (Fog.AdvancedFog)
				UpdateAdvancedFog ();

			// Update sun and fog color according to the new position of the sun
			if (Sky.SunAndMoonPosition == Sky.SunAndMoonCalc.Realistic)
				UpdateSunAndMoonPosition ();
			else
				UpdateSimpleSunAndMoonPosition ();
			
			CalculateDirectLight ();
	
			if (PlayerCamera != null) {
				// Set Clouds layers altitude
				if (Clouds.FixedAltitude)
					Components.Clouds.transform.position = new Vector3(Components.Clouds.transform.position.x,Clouds.cloudsAltitude,Components.Clouds.transform.position.z);
				else
					Components.Clouds.transform.localPosition = new Vector3(0f,0.2f,0f);

				//Move with player
			//	transform.position = CalculatePosition ();
				transform.position = Player.transform.position;
				transform.localScale = new Vector3 (PlayerCamera.farClipPlane, PlayerCamera.farClipPlane, PlayerCamera.farClipPlane);
			}

			if (!isNight && GameTime.solarTime < 0.5f) {
				isNight = true;
				if (AudioSourceAmbient != null)
					TryPlayAmbientSFX ();
				NotifyIsNight ();
			} else if (isNight && GameTime.solarTime >= 0.5f) {
				isNight = false;
				if (AudioSourceAmbient != null)
					TryPlayAmbientSFX ();
				NotifyIsDay ();
			}
		} 
		else 
		{


		}
	
	}

	private Vector3 BetaRay() {
		Vector3 Br;

		Vector3 realWavelenght = Sky.waveLenght * 1.0e-9f;

		Br.x = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelenght.x, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;
		Br.y = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelenght.y, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;
		Br.z = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelenght.z, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;

		return Br;
	}


	private Vector3 BetaMie() {
		Vector3 Bm;

		float c = (0.2f * Sky.turbidity ) * 10.0f;

		Bm.x = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / Sky.waveLenght.x, 2.0f) * K.x);
		Bm.y = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / Sky.waveLenght.y, 2.0f) * K.y);
		Bm.z = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / Sky.waveLenght.z, 2.0f) * K.z);

		Bm.x=Mathf.Pow(Bm.x,-1.0f);
		Bm.y=Mathf.Pow(Bm.y,-1.0f);
		Bm.z=Mathf.Pow(Bm.z,-1.0f);

		return Bm;
	}

	private Vector3 GetMieG() {
		return new Vector3(1.0f - Sky.g * Sky.g, 1.0f + Sky.g * Sky.g, 2.0f * Sky.g);
	}

	// Setup the Shaders with correct information
	private void SetupShader(float setup)
	{
		RenderSettings.skybox.SetVector ("_SunDir", -SunTransform.transform.forward);
		RenderSettings.skybox.SetVector ("_MoonDir", -Components.Moon.transform.forward);
		RenderSettings.skybox.SetMatrix ("_Sun",  SunTransform.worldToLocalMatrix);
		RenderSettings.skybox.SetColor("_scatteringColor",Sky.scatteringColor.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetColor("_sunDiskColor", Sky.SunDiskColor.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetColor("_weatherSkyMod",Sky.currentWeatherSkyMod);
		RenderSettings.skybox.SetColor("_weatherFogMod",Sky.currentWeatherFogMod);
		RenderSettings.skybox.SetVector ("_waveLenght", Sky.waveLenght);
		RenderSettings.skybox.SetVector ("_Bm", BetaMie () * (Sky.mie * Fog.scatteringStrenght));
		RenderSettings.skybox.SetVector ("_Br", BetaRay() * Sky.rayleigh);
		RenderSettings.skybox.SetVector ("_mieG",GetMieG ());
		RenderSettings.skybox.SetFloat ("_SunIntensity",Sky.SunIntensity);
		RenderSettings.skybox.SetFloat ("_SunDiskSize", Sky.SunDiskScale);
		RenderSettings.skybox.SetFloat ("_SunDiskIntensity", Sky.SunDiskIntensity);
		RenderSettings.skybox.SetFloat ("_SunDiskSize",Sky.SunDiskScale);
		RenderSettings.skybox.SetFloat ("_Exposure", Sky.SkyExposure);
		RenderSettings.skybox.SetFloat ("_SkyLuminance", Sky.SkyLuminence);
		RenderSettings.skybox.SetFloat ("_scatteringPower", Sky.scatteringCurve.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetFloat ("_SkyColorPower", Sky.SkyColorPower);
		//RenderSettings.skybox.SetFloat ("_CloudsColorBlending", Clouds.CloudsColorBlending);
		RenderSettings.skybox.SetFloat ("_StarsIntensity", Sky.StarsIntensity.Evaluate(GameTime.solarTime));
		// Update SkyFog settingss
		if(PlayerCamera != null)
		RenderSettings.skybox.SetVector ("_CameraWS", PlayerCamera.transform.position);


		//if (Sky.StarsBlinking > 0.0f)
		//{
		//	starsRot += Sky.StarsBlinking * Time.deltaTime;
		//	Quaternion rot = Quaternion.Euler (starsRot, starsRot, starsRot);
	//		Matrix4x4 NoiseRot = Matrix4x4.TRS (Vector3.zero, rot, new Vector3 (1, 1, 1));
	//		RenderSettings.skybox.SetMatrix ("_NoiseMatrix", NoiseRot);
		//}

		float windStrenght = 0;

		if (Weather.currentActiveWeatherID != null)
			windStrenght = Weather.currentActiveWeatherID.WindStrenght;

		if (Clouds.useWindZoneDirection) {
			Clouds.cloudsWindDirectionX = Weather.windZone.transform.forward.x;
			Clouds.cloudsWindDirectionY = Weather.windZone.transform.forward.z;
		}
			
		cloudAnim += new Vector2(((Clouds.cloudsTimeScale * (windStrenght * Clouds.cloudsWindDirectionX)) * Clouds.cloudsWindStrenghtModificator) * Time.deltaTime,((Clouds.cloudsTimeScale * (windStrenght * Clouds.cloudsWindDirectionY)) * Clouds.cloudsWindStrenghtModificator) * Time.deltaTime);
			
		if (cloudAnim.x > 1f)
			cloudAnim.x = -1f;
		else if (cloudAnim.x < -1f)
			cloudAnim.x = 1f;

		if (cloudAnim.y > 1f)
			cloudAnim.y = -1f;
		else if (cloudAnim.y < -1f)
			cloudAnim.y = 1f;

		if (cloudAnim.x == 0)
			cloudAnim.x = 0.1f;
		if (cloudAnim.y == 0)
			cloudAnim.y = 0.1f;

		for (int i = 0; i < Clouds.cloudsLayers.Count; i++) 
		{
			Clouds.cloudsLayers[i].myMaterial.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.cloudsLayers[i].Scaling);
			Clouds.cloudsLayers[i].myMaterial.SetColor("_BaseColor", Clouds.cloudsLayers[i].FirstColor);
			Clouds.cloudsLayers[i].myMaterial.SetColor("_SkyColor", Clouds.skyColor.Evaluate(GameTime.solarTime));
			Clouds.cloudsLayers[i].myMaterial.SetColor("_MoonColor", Clouds.moonHighlightColor.Evaluate(GameTime.lunarTime));
			Clouds.cloudsLayers[i].myMaterial.SetColor("_SunColor", Clouds.sunHighlightColor.Evaluate(GameTime.solarTime));
			Clouds.cloudsLayers[i].myMaterial.SetFloat("_CloudCover", Clouds.cloudsLayers[i].Coverage);
			Clouds.cloudsLayers[i].myMaterial.SetFloat("_Density", Clouds.cloudsLayers[i].Density);
			Clouds.cloudsLayers[i].myMaterial.SetFloat("_CloudAlpha", Clouds.cloudsLayers[i].Alpha);
			Clouds.cloudsLayers[i].myMaterial.SetVector("_timeScale", cloudAnim);
			Clouds.cloudsLayers[i].myMaterial.SetFloat ("_lightIntensity", Clouds.LightIntensity.Evaluate(GameTime.solarTime));
			Clouds.cloudsLayers[i].myMaterial.SetFloat ("_direct", Clouds.cloudsLayers[i].DirectLightIntensity);

			Clouds.cloudsLayers[i].myMaterial.SetVector ("_SunDirection", -Components.Sun.transform.forward);
			Clouds.cloudsLayers[i].myMaterial.SetVector ("_MoonDirection", -Components.Moon.transform.forward);

			if (Clouds.cloudsLayers [i].canCastShadows) {
				Clouds.cloudsLayers [i].myShadowMaterial.SetFloat ("_Scale", PlayerCamera.farClipPlane * Clouds.cloudsLayers [i].Scaling);
				Clouds.cloudsLayers [i].myShadowMaterial.SetFloat ("_CloudCover", Clouds.cloudsLayers[i].Coverage);
				Clouds.cloudsLayers [i].myShadowMaterial.SetFloat ("_CloudAlpha", Clouds.cloudsLayers[i].Alpha);
				Clouds.cloudsLayers [i].myShadowMaterial.SetVector("_timeScale", cloudAnim);
			}
		}

		if (MoonShader != null)
		{
			MoonShader.SetFloat("_Phase", Sky.MoonPhase);
			MoonShader.SetFloat("_Brightness", Sky.MoonBrightness * (1-GameTime.solarTime));
		}
	}

	void UpdateAdvancedFog ()
	{
		if (atmosphericFog == null)
			return;

		atmosphericFog.fogMaterial.SetTexture ("_Clouds", cloudsCamera.targetTexture);

		atmosphericFog.fogMaterial.SetVector ("_SunDir", -SunTransform.transform.forward);
		//atmosphericFog.fogMaterial.SetMatrix ("_Sun",  SunTransform.worldToLocalMatrix);
		atmosphericFog.fogMaterial.SetColor("_scatteringColor", Sky.scatteringColor.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetColor("_sunDiskColor", Sky.SunDiskColor.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetColor("_weatherSkyMod", Sky.currentWeatherSkyMod);
		atmosphericFog.fogMaterial.SetColor("_weatherFogMod", Sky.currentWeatherFogMod);

		atmosphericFog.fogMaterial.SetFloat ("_SkyFogHeight", Fog.skyFogHeight);
		atmosphericFog.fogMaterial.SetFloat ("_scatteringStrenght", Fog.scatteringStrenght);
		atmosphericFog.fogMaterial.SetFloat ("_skyFogIntensity", Fog.skyFogIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_SkyFogStrenght", Fog.skyFogStrength);
		atmosphericFog.fogMaterial.SetFloat ("_SunBlocking", Fog.sunBlocking);

		atmosphericFog.fogMaterial.SetVector ("_waveLenght", Sky.waveLenght);
		atmosphericFog.fogMaterial.SetVector ("_Bm", BetaMie () * (Sky.mie * (Fog.scatteringStrenght * GameTime.solarTime)));
		atmosphericFog.fogMaterial.SetVector ("_Br", BetaRay() * Sky.rayleigh);
		atmosphericFog.fogMaterial.SetVector ("_mieG", GetMieG ());
		atmosphericFog.fogMaterial.SetFloat ("_SunIntensity",  Sky.SunIntensity);

		atmosphericFog.fogMaterial.SetFloat ("_SunDiskSize",  Sky.SunDiskScale);
		atmosphericFog.fogMaterial.SetFloat ("_SunDiskIntensity",  Sky.SunDiskIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_SunDiskSize",  Sky.SunDiskScale);

		atmosphericFog.fogMaterial.SetFloat ("_Exposure", Sky.SkyExposure);
		atmosphericFog.fogMaterial.SetFloat ("_SkyLuminance", Sky.SkyLuminence);
		atmosphericFog.fogMaterial.SetFloat ("_scatteringPower", Sky.scatteringCurve.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetFloat ("_SkyColorPower", Sky.SkyColorPower);

		atmosphericFog.fogMaterial.SetFloat ("_heightFogIntensity", Fog.heightFogIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_scatteringStrenght", Fog.scatteringStrenght);
		atmosphericFog.fogMaterial.SetFloat ("_distanceFogIntensity", Fog.distanceFogIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_maximumFogDensity", 1 - Fog.maximumFogDensity);

		atmosphericFog.fogMaterial.SetVector ("_NoiseData", new Vector4 (Fog.noiseScale, Fog.heightDensity, Fog.noiseIntensity, 0f));
		atmosphericFog.fogMaterial.SetVector ("_NoiseVelocity", new Vector4 (Fog.heightFogVelocity.x, Fog.heightFogVelocity.y,0f, 0f));
	}
		
	DateTime CreateSystemDate ()
	{
		DateTime date = new DateTime ();

		date = date.AddYears (GameTime.Years - 1);
		date = date.AddDays (GameTime.Days - 1);

		return date;
	}

	void UpdateSunAndMoonPosition ()
	{

		DateTime date = CreateSystemDate ();
		float d = 367 * date.Year - 7 * ( date.Year + (date.Month / 12 + 9) / 12 ) / 4 + 275 * date.Month/9 + date.Day - 730530;
		d += (GetInternalTimeOfDay() / 24f);

		float ecl = 23.4393f - 3.563E-7f * d;

		CalculateSunPosition (d, ecl);
		CalculateMoonPosition (d, ecl);
	}


	 private float Remap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	void CalculateSunPosition (float d, float ecl)
	{
		/////http://www.stjarnhimlen.se/comp/ppcomp.html#5////
		///////////////////////// SUN ////////////////////////
		float w = 282.9404f + 4.70935E-5f * d;
		float e = 0.016709f - 1.151E-9f * d;
		float M = 356.0470f + 0.9856002585f * d;

		float E = M + e * Mathf.Rad2Deg * Mathf.Sin(Mathf.Deg2Rad * M) * (1 + e * Mathf.Cos(Mathf.Deg2Rad * M));

		float xv = Mathf.Cos(Mathf.Deg2Rad * E) - e;
		float yv = Mathf.Sin(Mathf.Deg2Rad * E) * Mathf.Sqrt(1 - e*e);

		float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
		float r = Mathf.Sqrt(xv*xv + yv*yv);

		float l = v + w;

		float xs = r * Mathf.Cos(Mathf.Deg2Rad * l);
		float ys = r * Mathf.Sin(Mathf.Deg2Rad * l);

		float xe = xs;
		float ye = ys * Mathf.Cos(Mathf.Deg2Rad * ecl);
		float ze = ys * Mathf.Sin(Mathf.Deg2Rad * ecl);

		float decl_rad = Mathf.Atan2(ze, Mathf.Sqrt(xe*xe + ye*ye));
		float decl_sin = Mathf.Sin(decl_rad);
		float decl_cos = Mathf.Cos(decl_rad);

		float GMST0 = v + w + 180;
		float GMST  = GMST0 + internalHour * 15;
		LST   = GMST + GameTime.Longitude;

		if (LST > 24)LST -= 24;  
		else if (LST < 0)LST += 24;

		float HA_deg = LST - Mathf.Rad2Deg * Mathf.Atan2(ye, xe);
		float HA_rad = Mathf.Deg2Rad * HA_deg;
		float HA_sin = Mathf.Sin(HA_rad);
		float HA_cos = Mathf.Cos(HA_rad);

		float x = HA_cos * decl_cos;
		float y = HA_sin * decl_cos;
		float z = decl_sin;

		float sin_Lat = Mathf.Sin(Mathf.Deg2Rad * GameTime.Latitude);
		float cos_Lat = Mathf.Cos(Mathf.Deg2Rad * GameTime.Latitude);

		float xhor = x * sin_Lat - z * cos_Lat;
		float yhor = y;
		float zhor = x * cos_Lat + z * sin_Lat;

		float azimuth  = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
		float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor*xhor + yhor*yhor));

		float sunTheta = (90 * Mathf.Deg2Rad) - altitude;
		float sunPhi   = azimuth;

		//Set SolarTime: 1 = mid-day (sun directly above you), 0.5 = sunset/dawn, 0 = midnight;
		GameTime.solarTime = Mathf.Clamp01(Remap (sunTheta, -1.5f, 0f, 1.5f, 1f));

		SunTransform.localPosition = OrbitalToLocal(sunTheta, sunPhi);

		// Always Face dome or better face the playerCamera!
		if(PlayerCamera != null)
			SunTransform.LookAt(PlayerCamera.transform.position);
		else
			SunTransform.transform.LookAt(DomeTransform.position);


		SetupShader(sunTheta);
	}

	void CalculateMoonPosition (float d, float ecl)
	{
		float N = 125.1228f - 0.0529538083f * d;
		float i = 5.1454f;
		float w = 318.0634f + 0.1643573223f * d;
		float a = 60.2666f;
		float e = 0.054900f;
		float M = 115.3654f + 13.0649929509f * d;

		float sun_w = 282.9404f + 4.70935E-5f * d;
		float sun_M = 356.0470f + 0.9856002585f * d;

		float sin_M = Mathf.Sin(Mathf.Deg2Rad * M);
		float cos_M = Mathf.Cos(Mathf.Deg2Rad * M);

		float E = M + e * Mathf.Rad2Deg * sin_M * (1 + e * cos_M);
	
		E0 = E;

		for (int eL = 0; eL < 1000; eL++){
			E1 = E0 - (E0 - (180.0f/pi) * e * Mathf.Sin(E0 * Mathf.Deg2Rad) - M) / ( 1.0f - e * Mathf.Cos(Mathf.Deg2Rad * E0));
			if (Mathf.Abs(E1)-Mathf.Abs(E0) < 0.005f){
				break;
			} else {
				E0 = E1;
			}
		}
		E = E1;

		float xv = a * (Mathf.Cos(Mathf.Deg2Rad * E) - e);
		float yv = a * (Mathf.Sin(Mathf.Deg2Rad * E) * Mathf.Sqrt(1 - e*e));

		float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
		float r = Mathf.Sqrt(xv*xv + yv*yv);

		float l = v + w;

		float sin_l = Mathf.Sin(Mathf.Deg2Rad * l);
		float cos_l = Mathf.Cos(Mathf.Deg2Rad * l);
		float cos_i = Mathf.Cos(Mathf.Sin(Mathf.Deg2Rad * i));
		float sin_N = Mathf.Sin(Mathf.Deg2Rad * N);
		float cos_N = Mathf.Cos(Mathf.Deg2Rad * N);

		float xh = r * (cos_N * cos_l - sin_N * sin_l * cos_i);
		float yh = r * (sin_N * cos_l + cos_N * sin_l * cos_i);
		float zh = r * (sin_l * Mathf.Sin(Mathf.Deg2Rad * i));

		float moonLongitude = Mathf.Atan2(yh,xh)*Mathf.Rad2Deg;
		float moonLatitude = Mathf.Atan2(zh,Mathf.Sqrt(xh*xh+yh*yh))*Mathf.Rad2Deg;

		float Ms = sun_M;	// Mean Anomaly of the Sun
		float Mm = M;	// Mean Anomaly of the Moon
		float Nm = N;	// Longitude of the Moon's node
		float ws = sun_w;	// Argument of perihelion for the Sun
		float wm = w;	// Argument of perihelion for the Moon

		float Ls = sun_w + sun_M;										// Mean Longitude of the Sun  (Ns=0)
		float Lm = Mm + wm + Nm;								// Mean longitude of the Moon
		float Dm = Lm - Ls;									// Mean elongation of the Moon
		float F = Lm - Nm;									// Argument of latitude for the Moon

		//Add these terms to the Moon's longitude (degrees):
		moonLongitude -= 1.274f * Mathf.Sin((Mm - (2.0f*Dm))* Mathf.Deg2Rad );          		// (the Evection)
		moonLongitude += 0.658f * Mathf.Sin((2.0f*Dm) * Mathf.Deg2Rad);               		// (the Variation)
		moonLongitude -= 0.186f * Mathf.Sin(Ms* Mathf.Deg2Rad);                 		// (the Yearly Equation)
		moonLongitude -= 0.059f * Mathf.Sin(((2.0f*Mm) - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLongitude -= 0.057f * Mathf.Sin((Mm - (2.0f*Dm) + Ms) * Mathf.Deg2Rad);
		moonLongitude += 0.053f * Mathf.Sin((Mm + (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLongitude += 0.046f * Mathf.Sin(((2.0f*Dm) - Ms) * Mathf.Deg2Rad);
		moonLongitude += 0.041f * Mathf.Sin((Mm - Ms) * Mathf.Deg2Rad);
		moonLongitude -= 0.035f * Mathf.Sin(Dm * Mathf.Deg2Rad);                 		// (the Parallactic Equation)
		moonLongitude -= 0.031f * Mathf.Sin((Mm + Ms) * Mathf.Deg2Rad);
		moonLongitude -= 0.015f * Mathf.Sin(((2.0f*F) - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLongitude += 0.011f * Mathf.Sin((Mm - (4.0f*Dm)) * Mathf.Deg2Rad);

		//Add these terms to the Moon's latitude (degrees):
		moonLatitude -= 0.173f * Mathf.Sin((F - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude -= 0.055f * Mathf.Sin(((Mm) - F - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude -= 0.046f * Mathf.Sin(((Mm) + F - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude += 0.033f * Mathf.Sin((F + (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude += 0.017f * Mathf.Sin(((2.0f*Mm) + F) * Mathf.Deg2Rad);

		xh = 1f * Mathf.Cos(moonLongitude * Mathf.Deg2Rad) * Mathf.Cos(moonLatitude * Mathf.Deg2Rad);
		yh = 1f * Mathf.Sin(moonLongitude* Mathf.Deg2Rad) * Mathf.Cos(moonLatitude* Mathf.Deg2Rad);
		zh = 1f * Mathf.Sin(moonLatitude* Mathf.Deg2Rad);

		float xe = xh;
		float ye = yh * Mathf.Cos(Mathf.Deg2Rad * ecl) - zh * Mathf.Sin(Mathf.Deg2Rad * ecl);
		float ze = zh * Mathf.Sin(Mathf.Deg2Rad * ecl) + zh * Mathf.Cos(Mathf.Deg2Rad * ecl);

		//float GMST0 = v + w + 180;
		//float GMST = GMST0 + (internalHour * 15);
		//float LST = GMST + GameTime.Longitude;

		float HA = Mathf.Deg2Rad * ( LST - Mathf.Rad2Deg * Mathf.Atan2(ye , xe));
		float cos_decl = Mathf.Cos(Mathf.Atan2( ze, Mathf.Sqrt(xe * xe + ye * ye)));

		float x = Mathf.Cos(HA) * cos_decl;
		float y = Mathf.Sin(HA) * cos_decl;
		float z = Mathf.Sin(Mathf.Atan2(ze, Mathf.Sqrt(xe*xe + ye*ye)));

		float sin_Lat = Mathf.Sin(Mathf.Deg2Rad * GameTime.Latitude);
		float cos_Lat = Mathf.Cos(Mathf.Deg2Rad * GameTime.Latitude);

		float xhor = x * sin_Lat - z * cos_Lat;
		float yhor = y;
		float zhor = x * cos_Lat + z * sin_Lat;

		float azimuth = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
		float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor*xhor + yhor*yhor));

		float MoonTheta = (90 * Mathf.Deg2Rad) - altitude;
		float MoonPhi = azimuth;

		MoonTransform.localPosition = OrbitalToLocal(MoonTheta, MoonPhi);
		GameTime.lunarTime = Mathf.Clamp01(Remap (MoonTheta, -1.5f, 0f, 1.5f, 1f));

		// Always Face dome or better face the playerCamera!
		if(PlayerCamera != null)
			MoonTransform.LookAt(PlayerCamera.transform.position);
		else
			MoonTransform.transform.LookAt(DomeTransform.position);
	}

	void UpdateSimpleSunAndMoonPosition ()
	{
		// Calculates the Solar latitude
		float latitudeRadians = Mathf.Deg2Rad * GameTime.Latitude;
		float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
		float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

		// Calculates the Solar longitude
		float longitudeRadians = Mathf.Deg2Rad * GameTime.Longitude;

		// Solar declination - constant for the whole globe at any given day
		float solarDeclination = 0.4093f * Mathf.Sin(2f * pi / 368f * (GameTime.Days - 81f));
		float solarDeclinationSin = Mathf.Sin(solarDeclination);
		float solarDeclinationCos = Mathf.Cos(solarDeclination);

		// Calculate Solar time
		float timeZone = (int)(GameTime.Longitude / 15f);
		float meridian = Mathf.Deg2Rad * 15f * timeZone;
		float solarTime = internalHour + 0.170f * Mathf.Sin(4f * pi / 373f * (GameTime.Days - 80f)) - 0.129f * Mathf.Sin(2f * pi / 355f * (GameTime.Days - 8f))  + 12f / pi * (meridian - longitudeRadians);
		float solarTimeRadians = pi / 12f * solarTime;
		float solarTimeSin = Mathf.Sin(solarTimeRadians);
		float solarTimeCos = Mathf.Cos(solarTimeRadians);

		// Solar altitude angle between the sun and the horizon
		float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
		float solarAltitude = Mathf.Asin(solarAltitudeSin);

		// Solar azimuth angle of the sun around the horizon
		float solarAzimuthY = -solarDeclinationCos * solarTimeSin;
		float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
		float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

		// Convert to spherical coords
		float theta = pi / 2 - solarAltitude;
		float phi = solarAzimuth;

		GameTime.solarTime = Mathf.Clamp01(Remap (theta, -1.5f, 0f, 1.5f, 1f));
		GameTime.lunarTime = Mathf.Clamp01(Remap (theta - pi, -1.5f, 0f, 1.5f, 1f));

		// Update sun position
		SunTransform.localPosition = OrbitalToLocal(theta, phi);
		SunTransform.LookAt(DomeTransform.position);
		// Update moon position
		MoonTransform.localPosition = OrbitalToLocal(theta - pi, phi);
		MoonTransform.LookAt(DomeTransform.position);

		SetupShader(theta);
	}

	Vector3 UpdateSatellitePosition (float orbit,float orbit2,float speed)
	{
		// Calculates the Solar latitude
		float latitudeRadians = Mathf.Deg2Rad * GameTime.Latitude;
		float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
		float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

		// Calculates the Solar longitude
		float longitudeRadians = Mathf.Deg2Rad * GameTime.Longitude;

		// Solar declination - constant for the whole globe at any given day
		float solarDeclination = orbit2 * Mathf.Sin(2f * pi / 368f * (GameTime.Days - 81f));
		float solarDeclinationSin = Mathf.Sin(solarDeclination);
		float solarDeclinationCos = Mathf.Cos(solarDeclination);

		// Calculate Solar time
		float timeZone = (int)(GameTime.Longitude / 15f);
		float meridian = Mathf.Deg2Rad * 15f * timeZone;

		float solarTime = internalHour + orbit * Mathf.Sin(4f * pi / 377f * (GameTime.Days - 80f)) - speed * Mathf.Sin(1f * pi / 355f * (GameTime.Days - 8f))  + 12f / pi * (meridian - longitudeRadians);

		float solarTimeRadians = pi / 12f * solarTime;
		float solarTimeSin = Mathf.Sin(solarTimeRadians);
		float solarTimeCos = Mathf.Cos(solarTimeRadians);

		// Solar altitude angle between the sun and the horizon
		float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
		float solarAltitude = Mathf.Asin(solarAltitudeSin);

		// Solar azimuth angle of the sun around the horizon
		float solarAzimuthY = -solarDeclinationCos * solarTimeSin;
		float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
		float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

		// Convert to spherical coords
		float theta = pi / 2 - solarAltitude;
		float phi = solarAzimuth;

		// Send local position
		return OrbitalToLocal(theta, phi);
	}


	Vector3 OrbitalToLocal(float theta, float phi)
	{
		Vector3 res;

		float sinTheta = Mathf.Sin(theta);
		float cosTheta = Mathf.Cos(theta);
		float sinPhi   = Mathf.Sin(phi);
		float cosPhi   = Mathf.Cos(phi);

		res.z = sinTheta * cosPhi;
		res.y = cosTheta;
		res.x = sinTheta * sinPhi;

		return res;
	}



	void UpdateReflections ()
	{
		Components.GlobalReflectionProbe.intensity = Lighting.globalReflectionsIntensity;

		if ((currentTimeInHours > lastRelfectionUpdate + Lighting.globalReflectionsUpdate || currentTimeInHours < lastRelfectionUpdate - Lighting.globalReflectionsUpdate) && Lighting.globalReflections) {
			Components.GlobalReflectionProbe.enabled = true;
			lastRelfectionUpdate = currentTimeInHours;
			Components.GlobalReflectionProbe.RenderProbe ();
		} else if (!Lighting.globalReflections) {
			Components.GlobalReflectionProbe.enabled = false;
		}
	}

	// Update the GameTime
	void UpdateTime()
	{
		if (Application.isPlaying) {

			float t = (24.0f / 60.0f) / GameTime.DayLengthInMinutes;
			hourTime = t * Time.deltaTime;

			switch (GameTime.ProgressTime) {
			case TimeVariables.TimeProgressMode.None:
				//Set Time over editor or other scripts.
				SetTime (GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
				break;

			case TimeVariables.TimeProgressMode.Simulated:
				internalHour += hourTime;
				SetInternalTimeOfDay (internalHour);
				Sky.MoonPhase += Time.deltaTime / (30f * (GameTime.DayLengthInMinutes * 60f)) * 2f;
				break;

			case TimeVariables.TimeProgressMode.SystemTime:
				SetTime (System.DateTime.Now);
				Sky.MoonPhase += Time.deltaTime / (30f * (1440 * 60f)) * 2f;
				break;
			}
		} else {
			SetTime (GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
		}

		if (Sky.MoonPhase < -1) Sky.MoonPhase += 2;
		else if (Sky.MoonPhase > 1) Sky.MoonPhase -= 2;

		if (internalHour > (lastHourUpdate + 1f)) 
		{
			lastHourUpdate = internalHour;
			NotifyHourPassed ();
		}

		if (internalHour >= 24)
		{
			internalHour = 0;
			NotifyHourPassed ();
			lastHourUpdate = 0f;
			GameTime.Days = GameTime.Days + 1;
			NotifyDayPassed ();
		}

		if(GameTime.Days >= (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays + Seasons.WinterInDays))
		{
			GameTime.Years = GameTime.Years + 1;
			GameTime.Days = 0;
			NotifyYearPassed ();
		}

		currentHour = internalHour;
		currentDay = GameTime.Days;
		currentYear = GameTime.Years;

		currentTimeInHours = GetInHours (internalHour, currentDay, currentYear);
	}

	void UpdateAmbientLight ()
	{
		switch (Lighting.ambientMode) {
		case UnityEngine.Rendering.AmbientMode.Flat:
			RenderSettings.ambientSkyColor = Color.Lerp(Lighting.ambientSkyColor.Evaluate (GameTime.solarTime),Sky.currentWeatherLightMod,Sky.currentWeatherLightMod.a) * Lighting.ambientIntensity.Evaluate(GameTime.solarTime);
			break;

		case UnityEngine.Rendering.AmbientMode.Trilight:
			RenderSettings.ambientSkyColor = Color.Lerp(Lighting.ambientSkyColor.Evaluate (GameTime.solarTime),Sky.currentWeatherLightMod,Sky.currentWeatherLightMod.a) * Lighting.ambientIntensity.Evaluate(GameTime.solarTime);
			RenderSettings.ambientEquatorColor = Color.Lerp(Lighting.ambientEquatorColor.Evaluate (GameTime.solarTime),Sky.currentWeatherLightMod,Sky.currentWeatherLightMod.a) * Lighting.ambientIntensity.Evaluate(GameTime.solarTime);
			RenderSettings.ambientGroundColor = Color.Lerp(Lighting.ambientGroundColor.Evaluate (GameTime.solarTime),Sky.currentWeatherLightMod,Sky.currentWeatherLightMod.a) * Lighting.ambientIntensity.Evaluate(GameTime.solarTime);
			break;

		case UnityEngine.Rendering.AmbientMode.Skybox:
			DynamicGI.UpdateEnvironment ();
			break;

		}
	}

	// Calculate sun and moon light intensity and color
	private void CalculateDirectLight()
	{ 
		MainLight.color = Color.Lerp(Lighting.LightColor.Evaluate (GameTime.solarTime),Sky.currentWeatherLightMod,Sky.currentWeatherLightMod.a);

		Shader.SetGlobalColor ("_EnviroLighting", Lighting.LightColor.Evaluate (GameTime.solarTime));
		Shader.SetGlobalVector ("_SunDirection", -Components.Sun.transform.forward);

		Shader.SetGlobalVector ("_SunPosition", Components.Sun.transform.localPosition + (-Components.Sun.transform.forward * 10000f));
		Shader.SetGlobalVector ("_MoonPosition", Components.Moon.transform.localPosition);
			
		float lightIntensity;

		// Set sun and moon intensity
		if (!isNight)
		{
			lightIntensity = Lighting.directLightIntensity.Evaluate (GameTime.solarTime);
			Components.DirectLight.position = Components.Sun.transform.position;
			Components.DirectLight.rotation = Components.Sun.transform.rotation;
		}
		else
		{
			lightIntensity = Lighting.directLightIntensity.Evaluate (GameTime.solarTime) * Mathf.Clamp01(2f - Mathf.Abs(Sky.MoonPhase)) + Lighting.SunWeatherMod;
			Components.DirectLight.position = Components.Moon.transform.position;
			Components.DirectLight.rotation = Components.Moon.transform.rotation;
		}
			
		// Set the light and shadow intensity
		MainLight.intensity = Mathf.Lerp (MainLight.intensity, lightIntensity, 10f * Time.deltaTime);
		MainLight.shadowStrength = Lighting.shadowStrenght;
	}

	// Make the parameters stay in reasonable range
	private void ValidateParameters()
	{
		// Keep GameTime Parameters right!
		internalHour = Mathf.Repeat(internalHour, 24);
		GameTime.Longitude = Mathf.Clamp(GameTime.Longitude, -180, 180);
		GameTime.Latitude = Mathf.Clamp(GameTime.Latitude, -90, 90);

		#if UNITY_EDITOR
		if (GameTime.DayLengthInMinutes == 0)
		{
			internalHour = 12f;
			Sky.MoonPhase = 0f;
		}
		#endif

		// Moon
		#if UNITY_EDITOR
		Sky.MoonPhase = Mathf.Clamp(Sky.MoonPhase, -1, +1);
		#endif
	}
		
	///////////////////////////////////////////////////////////////////cloud meshes/////////////////////////////////////////////////////////////////////////
	Mesh CreateCloudsLayer (int layerID, bool isShadowMesh)
	{
		int sliceQuality = 1;

		if (!isShadowMesh)
			sliceQuality = Clouds.cloudsLayers [layerID].Quality;

		//Setting arrays up
		Vector3[] vertices = new Vector3[(Clouds.cloudsLayers[layerID].segmentCount * Clouds.cloudsLayers[layerID].segmentCount) * sliceQuality];
		Vector2[] uvMap = new Vector2[vertices.Length];
		int[] triangleConstructor = new int[(Clouds.cloudsLayers[layerID].segmentCount-1) * (Clouds.cloudsLayers[layerID].segmentCount-1) * sliceQuality * 2 * 3];
		Color[] vertexColor = new Color[vertices.Length];
		float tempRatio = 1.0f / ((float)Clouds.cloudsLayers[layerID].segmentCount - 1);
		Vector3 posGainPerVertices = new Vector3(tempRatio * 2f, 1.0f/(Mathf.Clamp(sliceQuality - 1, 1, 999999)) * Clouds.cloudsLayers[layerID].thickness, tempRatio * 2f); 
		float posGainPerUV = tempRatio;

		// Lets Create our mesh yea!
		int iteration = 0; 
		int vIncrement = 0;
		int increment = 0;
		float curvature = 0.0f;

		float depthColor = -1.0f;
		float mirrorColor = 0.0f;
		//computes slices by vertices row, each time the row ends, do the next one.
		for(int s = 0; s < sliceQuality; s++){
			depthColor = -1 + (s*(2/(float)sliceQuality));

			if(s < sliceQuality * 0.5f)
				mirrorColor = 0 + (1.0f / ((float)sliceQuality * 0.5f)) * s;
			else 				 
				mirrorColor = 2 - (1.0f / ((float)sliceQuality * 0.5f)) * (s + 1);

			if(sliceQuality == 1 || isShadowMesh)
				mirrorColor = 1;
			//horizontal vertices
			for(int h = 0; h < Clouds.cloudsLayers[layerID].segmentCount; h++){
				int incrementV = Clouds.cloudsLayers[layerID].segmentCount * iteration;
				//vertical vertices
				for(int v = 0; v < Clouds.cloudsLayers[layerID].segmentCount; v++){

					if(Clouds.cloudsLayers[layerID].curved)
						curvature = Vector3.Distance(new Vector3(posGainPerVertices.x*v - 1f, 0.0f, posGainPerVertices.z * h - 1f), Vector3.zero);

					if(sliceQuality == 1 || isShadowMesh)					
						vertices[v+incrementV] = new Vector3(posGainPerVertices.x*v- 1f, 0f + (Mathf.Pow(curvature, 2f) * Clouds.cloudsLayers[layerID].curvedIntensity), posGainPerVertices.z*h-1f);
					else 
						vertices[v+incrementV] = new Vector3(posGainPerVertices.x*v- 1f, posGainPerVertices.y*s-(Clouds.cloudsLayers[layerID].thickness / 2f)+(Mathf.Pow(curvature, 2f) * Clouds.cloudsLayers[layerID].curvedIntensity), posGainPerVertices.z * h - 1f);

					uvMap[v+incrementV] = new Vector2(posGainPerUV*v, posGainPerUV*h);
					vertexColor[v+incrementV] = new Vector4(depthColor, depthColor, depthColor, mirrorColor);
				}
				iteration += 1;

				//Triangle construction
				if(h >= 1){
					for(int tri = 0; tri < Clouds.cloudsLayers[layerID].segmentCount-1; tri++){
						triangleConstructor[0+increment] = (0+tri)+vIncrement+(s*Clouds.cloudsLayers[layerID].segmentCount);//
						triangleConstructor[1+increment] = (Clouds.cloudsLayers[layerID].segmentCount+tri)+vIncrement+(s*Clouds.cloudsLayers[layerID].segmentCount);
						triangleConstructor[2+increment] = (1+tri)+vIncrement+(s*Clouds.cloudsLayers[layerID].segmentCount);//
						triangleConstructor[3+increment] = ((Clouds.cloudsLayers[layerID].segmentCount+1)+tri)+vIncrement+(s*Clouds.cloudsLayers[layerID].segmentCount);
						triangleConstructor[4+increment] = (1+tri)+vIncrement+(s*Clouds.cloudsLayers[layerID].segmentCount);
						triangleConstructor[5+increment] = (Clouds.cloudsLayers[layerID].segmentCount+tri)+vIncrement+(s*Clouds.cloudsLayers[layerID].segmentCount);
						increment +=6;
					}
					vIncrement += Clouds.cloudsLayers[layerID].segmentCount;
				}
			}
		}
		if (!isShadowMesh) 
		{
			Mesh slicedCloudMesh = new Mesh ();
			slicedCloudMesh.Clear ();
			slicedCloudMesh.name = "Clouds";
			slicedCloudMesh.vertices = vertices;
			slicedCloudMesh.triangles = triangleConstructor;
			slicedCloudMesh.uv = uvMap;
			slicedCloudMesh.colors = vertexColor;
			slicedCloudMesh.RecalculateNormals ();
			slicedCloudMesh.RecalculateBounds ();
			CalcMeshTangents (slicedCloudMesh);

			return slicedCloudMesh;
		} 
		else
		{
			Mesh shadowMesh = new Mesh ();
			shadowMesh.Clear ();
			shadowMesh.name = "CloudsShadows";
			shadowMesh.vertices = vertices;
			shadowMesh.triangles = triangleConstructor;
			shadowMesh.uv = uvMap;
			shadowMesh.colors = vertexColor;
			shadowMesh.RecalculateNormals ();
			shadowMesh.RecalculateBounds ();
			CalcMeshTangents (shadowMesh);

			return shadowMesh;
		}
	}

	public static void CalcMeshTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;

		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];

		Vector4[] tangents = new Vector4[vertexCount];

		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];

			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];

			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float r = 1.0f / (s1 * t2 - s2 * t1);

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}


		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			Vector3.OrthoNormalize(ref n, ref t);

			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		mesh.tangents = tangents;
	}

	///////////////////////////////////////////////////////////////////WEATHER SYSTEM /////////////////////////////////////////////////////////////////////////
	public void RegisterZone (EnviroZone zoneToAdd)
	{
		Weather.zones.Add (zoneToAdd);
	}


	public void EnterZone (EnviroZone zone)
	{
		Weather.currentActiveZone = zone;
	}

	public void ExitZone ()
	{

	}

	public void CreateWeatherEffectHolder()
	{
		if (Weather.VFXHolder == null) {
			GameObject VFX = new GameObject ();
			VFX.name = "VFX";
			VFX.transform.parent = EffectsHolder.transform;
			VFX.transform.localPosition = Vector3.zero;
			Weather.VFXHolder = VFX;
		}
	}

	void UpdateAudioSource (EnviroWeatherPrefab i)
	{
		if (i != null && i.weatherSFX != null)
		{
			if (i.weatherSFX == Weather.currentAudioSource.audiosrc.clip)
			{
				Weather.currentAudioSource.FadeIn(i.weatherSFX);
				return;
			}

			if (Weather.currentAudioSource == AudioSourceWeather)
			{
				AudioSourceWeather.FadeOut();
				AudioSourceWeather2.FadeIn(i.weatherSFX);
				Weather.currentAudioSource = AudioSourceWeather2;
			}
			else if (Weather.currentAudioSource == AudioSourceWeather2)
			{
				AudioSourceWeather2.FadeOut();
				AudioSourceWeather.FadeIn(i.weatherSFX);
				Weather.currentAudioSource = AudioSourceWeather;
			}
		} 
		else
		{
			AudioSourceWeather.FadeOut();
			AudioSourceWeather2.FadeOut();
		}
	}

	void UpdateClouds (EnviroWeatherPrefab i, bool withTransition)
	{
		if (i == null)
			return;

		float speed = 500f * Time.deltaTime;

		if (withTransition)
			speed = Weather.cloudTransitionSpeed * Time.deltaTime;

		for(int q = 0; q < Clouds.cloudsLayers.Count; q++)
		{
			if (i.cloudConfig.Count > q) {
				Clouds.cloudsLayers [q].FirstColor = Color.Lerp (Clouds.cloudsLayers [q].FirstColor, i.cloudConfig [q].FirstColor, speed);
				Clouds.cloudsLayers [q].DirectLightIntensity = Mathf.Lerp (Clouds.cloudsLayers [q].DirectLightIntensity, i.cloudConfig [q].DirectLightInfluence, speed);
				Clouds.cloudsLayers [q].Coverage = Mathf.Lerp (Clouds.cloudsLayers [q].Coverage, i.cloudConfig [q].Coverage, speed);
				Clouds.cloudsLayers [q].Density = Mathf.Lerp (Clouds.cloudsLayers [q].Density, i.cloudConfig [q].Density, speed);
				Clouds.cloudsLayers [q].Alpha = Mathf.Lerp (Clouds.cloudsLayers [q].Alpha, i.cloudConfig [q].Alpha, speed);
			} 
			else 
			{
				Clouds.cloudsLayers [q].Density = Mathf.Lerp (Clouds.cloudsLayers [q].Density, 0f, speed);
				Clouds.cloudsLayers [q].Coverage = Mathf.Lerp (Clouds.cloudsLayers [q].Coverage, -1f, speed);
				Clouds.cloudsLayers [q].Alpha = Mathf.Lerp (Clouds.cloudsLayers [q].Alpha, 0.5f, speed);
			}
		}
		Sky.currentWeatherSkyMod = Color.Lerp (Sky.currentWeatherSkyMod, i.weatherSkyMod.Evaluate(GameTime.solarTime), speed);
		Sky.currentWeatherFogMod = Color.Lerp (Sky.currentWeatherFogMod, i.weatherFogMod.Evaluate(GameTime.solarTime), speed * 10);
		Sky.currentWeatherLightMod = Color.Lerp (Sky.currentWeatherLightMod, i.weatherLightMod.Evaluate(GameTime.solarTime), speed);
	}


	void UpdateFog (EnviroWeatherPrefab i, bool withTransition)
	{
		if (i != null) {

			float speed = 500f * Time.deltaTime;

			if (withTransition)
				speed = Weather.fogTransitionSpeed * Time.deltaTime;


			if (Fog.Fogmode == FogMode.Linear) {
				RenderSettings.fogEndDistance = Mathf.Lerp (RenderSettings.fogEndDistance, i.fogDistance, speed);
				RenderSettings.fogStartDistance = Mathf.Lerp (RenderSettings.fogStartDistance, i.fogStartDistance, speed);
			} else
				RenderSettings.fogDensity = Mathf.Lerp (RenderSettings.fogDensity, i.fogDensity, speed);


			// Set the Fog color to light color to match Day-Night cycle and weather
			RenderSettings.fogColor = Color.Lerp(Lighting.ambientSkyColor.Evaluate(GameTime.solarTime),Sky.currentWeatherFogMod,Sky.currentWeatherFogMod.a);
			Fog.heightDensity = Mathf.Lerp (Fog.heightDensity, i.heightFogDensity, speed);
			Fog.skyFogHeight = Mathf.Lerp (Fog.skyFogHeight, i.SkyFogHeight, speed);
			Fog.skyFogStrength = Mathf.Lerp (Fog.skyFogStrength, i.SkyFogIntensity, speed);
			Fog.scatteringStrenght = Mathf.Lerp (Fog.scatteringStrenght, i.FogScatteringIntensity, speed);
			Fog.sunBlocking = Mathf.Lerp (Fog.sunBlocking, i.fogSunBlocking, speed);
		}
	}

	void UpdateEffectSystems (EnviroWeatherPrefab id, bool withTransition)
	{
		if (id != null) {

			float speed = 500f * Time.deltaTime;

			if (withTransition)
				speed = Weather.effectTransitionSpeed * Time.deltaTime;

			for (int i = 0; i < id.effectParticleSystems.Count; i++) {
				// Set EmissionRate
				float val = Mathf.Lerp (GetEmissionRate (id.effectParticleSystems [i]), id.effectEmmisionRates [i] * Quality.GlobalParticleEmissionRates, speed );
				SetEmissionRate (id.effectParticleSystems [i], val);
			}

			for (int i = 0; i < Weather.WeatherTemplates.Count; i++) {
				if (Weather.WeatherTemplates [i].gameObject != id.gameObject) {
					for (int i2 = 0; i2 < Weather.WeatherTemplates [i].effectParticleSystems.Count; i2++) {
						float val2 = Mathf.Lerp (GetEmissionRate (Weather.WeatherTemplates [i].effectParticleSystems [i2]), 0f, speed);

						if (val2 < 1f)
							val2 = 0f;

						SetEmissionRate (Weather.WeatherTemplates [i].effectParticleSystems [i2], val2);
					}
				}
			}

			Weather.windZone.windMain = id.WindStrenght; // Set Wind Strenght

			Weather.curWetness = Weather.wetness;
			Weather.wetness = Mathf.Lerp (Weather.curWetness, id.wetnessLevel, Weather.wetnessAccumulationSpeed * Time.deltaTime);
			Weather.wetness = Mathf.Clamp (Weather.wetness, 0f, 1f);

			Weather.curSnowStrenght = Weather.SnowStrenght;
			Weather.SnowStrenght = Mathf.Lerp (Weather.curSnowStrenght, id.snowLevel, Weather.snowAccumulationSpeed * Time.deltaTime);
			Weather.SnowStrenght = Mathf.Clamp (Weather.SnowStrenght, 0f, 1f);
			Shader.SetGlobalFloat ("_EnviroGrassSnow", Weather.curSnowStrenght);
		}
	}

	public static float GetEmissionRate (ParticleSystem system)
	{
		return system.emission.rate.constantMax;
	}


	public static void SetEmissionRate (ParticleSystem sys, float emissionRate)
	{
		var emission = sys.emission;
		var rate = emission.rate;
		rate.constantMax = emissionRate;
		emission.rate = rate;
	}

	IEnumerator PlayThunderRandom()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(10,20));
		int i = UnityEngine.Random.Range(0,Weather.ThunderSFX.Count);
		AudioSourceThunder.clip = Weather.ThunderSFX[i];
		AudioSourceThunder.loop = false;
		AudioSourceThunder.Play ();
		Weather.LightningGenerator.Lightning ();
		Weather.thundersfx = 0;
	}

	void UpdateWeather ()
	{	
		//Current active weather not matching current zones weather:
		if(Weather.currentActiveWeatherID != Weather.currentActiveZone.currentActiveZoneWeatherID)
		{
			Weather.lastActiveWeatherID = Weather.currentActiveWeatherID;
			Weather.currentActiveWeatherID = Weather.currentActiveZone.currentActiveZoneWeatherID;
			if (Weather.currentActiveWeatherID != null) {
				NotifyWeatherChanged (Weather.currentActiveWeatherID);
				TryPlayAmbientSFX ();
				UpdateAudioSource (Weather.currentActiveWeatherID);
			}
		}

		if (Weather.currentActiveWeatherID != null) 
		{
			UpdateClouds (Weather.currentActiveWeatherID, true);
			UpdateFog (Weather.currentActiveWeatherID, true);
			UpdateEffectSystems (Weather.currentActiveWeatherID, true);

			//Play ThunderSFX
			if (Weather.thundersfx == 0 && Weather.currentActiveWeatherID && Weather.currentActiveWeatherID.isLightningStorm) {
				Weather.thundersfx = 1;
				StartCoroutine (PlayThunderRandom ());
			} else {
				StopCoroutine (PlayThunderRandom ());
			}
		}
	}

	/// <summary>
	/// Set weather directly with list id of Weather.WeatherTemplates. No transtions!
	/// </summary>
	public void SetWeatherOverwrite (int weatherId)
	{
		if (weatherId < 0 || weatherId > Weather.WeatherTemplates.Count)
			return;

		if (Weather.WeatherTemplates[weatherId] != Weather.currentActiveWeatherID)
		{
			Weather.currentActiveZone.currentActiveZoneWeatherID = Weather.WeatherTemplates[weatherId];
			EnviroSky.instance.NotifyZoneWeatherChanged (Weather.WeatherTemplates[weatherId], Weather.currentActiveZone);
		}

		UpdateClouds (Weather.currentActiveZone.currentActiveZoneWeatherID, false);
		UpdateFog (Weather.currentActiveZone.currentActiveZoneWeatherID, false);
		UpdateEffectSystems (Weather.currentActiveZone.currentActiveZoneWeatherID, false);
	}

	/// <summary>
	/// Set weather over id with smooth transtion.
	/// </summary>
	public void ChangeWeather (int weatherId)
	{
		if (weatherId < 0 || weatherId > Weather.WeatherTemplates.Count)
			return;

		if (Weather.WeatherTemplates[weatherId] != Weather.currentActiveWeatherID)
		{
			Weather.currentActiveZone.currentActiveZoneWeatherID = Weather.WeatherTemplates[weatherId];
			EnviroSky.instance.NotifyZoneWeatherChanged (Weather.WeatherTemplates[weatherId], Weather.currentActiveZone);
		}
	}

	/// <summary>
	/// Set weather over name.
	/// </summary>
	public void ChangeWeather (string weatherName)
	{
		for (int i = 0; i < Weather.WeatherTemplates.Count; i++) {
			if (Weather.WeatherTemplates [i].Name == weatherName && Weather.WeatherTemplates [i] != Weather.currentActiveWeatherID) {
				ChangeWeather (i);
				EnviroSky.instance.NotifyZoneWeatherChanged (Weather.WeatherTemplates [i], Weather.currentActiveZone);
			}
		}

	}
		
	/// <summary>
	/// Saves the current time and weather in Playerprefs.
	/// </summary>
	public void Save ()
	{
		PlayerPrefs.SetFloat("Time_Hours",internalHour);
		PlayerPrefs.SetInt("Time_Days",GameTime.Days);
		PlayerPrefs.SetInt("Time_Years",GameTime.Years);
		for (int i = 0; i < Weather.WeatherTemplates.Count; i++) {
			if(Weather.WeatherTemplates[i] == Weather.currentActiveWeatherID)
				PlayerPrefs.SetInt("currentWeather",i);
		}
	}

	/// <summary>
	/// Loads the saved time and weather from Playerprefs.
	/// </summary>
	public void Load ()
	{
		if (PlayerPrefs.HasKey ("Time_Hours"))
			internalHour = PlayerPrefs.GetFloat ("Time_Hours");
		if (PlayerPrefs.HasKey ("Time_Days"))
			GameTime.Days = PlayerPrefs.GetInt ("Time_Days");
		if (PlayerPrefs.HasKey ("Time_Years"))
			GameTime.Years = PlayerPrefs.GetInt ("Time_Years");
		if (PlayerPrefs.HasKey ("currentWeather"))
			SetWeatherOverwrite(PlayerPrefs.GetInt("currentWeather"));
	}

	/// <summary>
	/// Set the exact date. by DateTime
	/// </summary>
	public void SetTime(DateTime date)
	{
		GameTime.Years = date.Year;
		GameTime.Days = date.DayOfYear;
		GameTime.Minutes = date.Minute;
		GameTime.Seconds = date.Second;
		GameTime.Hours = date.Hour;
		internalHour = date.Hour + (date.Minute * 0.0166667f) + (date.Second * 0.000277778f);
	}

	/// <summary>
	/// Set the exact date.
	/// </summary>
	public void SetTime(int year, int dayOfYear, int hour, int minute, int seconds)
	{
		GameTime.Years = year;
		GameTime.Days = dayOfYear;
		GameTime.Minutes = minute;
		GameTime.Hours = hour;
		internalHour = hour + (minute * 0.0166667f) + (seconds * 0.000277778f);
	}

	/// <summary>
	/// Set the time of day in hours. (12.5 = 12:30)
	/// </summary>
	public void SetInternalTimeOfDay(float inHours)
	{
		internalHour = inHours;
		GameTime.Hours = (int)inHours;
		inHours -= GameTime.Hours;
		GameTime.Minutes = (int)(inHours * 60);
		inHours -= GameTime.Minutes * 0.0166667f;
		GameTime.Seconds = (int)(inHours * 3600f);
	}

	/// <summary>
	/// Get current time in a nicely formatted string with seconds!
	/// </summary>
	/// <returns>The time string.</returns>
	public string GetTimeStringWithSeconds ()
	{
		return string.Format ("{0:00}:{1:00}:{2:00}", GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
	}

	/// <summary>
	/// Get current time in a nicely formatted string!
	/// </summary>
	/// <returns>The time string.</returns>
	public string GetTimeString ()
	{
		return string.Format ("{0:00}:{1:00}", GameTime.Hours, GameTime.Minutes);
	}

	/// <summary>
	/// Get current time in hours. (12.5 = 12:30)
	/// </summary>
	/// <returns>The the current time of day in hours.</returns>
	public float GetInternalTimeOfDay()
	{
		return internalHour;
	}

	/// <summary>
	/// Calculate total time in hours.
	/// </summary>
	/// <returns>The the current date in hours.</returns>
	public float GetInHours (float hours,float days, float years)
	{
		float inHours  = hours + (days*24f) + ((years * (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays + Seasons.WinterInDays)) * 24f);
		return inHours;
	}
		
	/// <summary>
	/// Assign your Player and Camera and Initilize.////
	/// </summary>
	public void AssignAndStart (GameObject player, Camera Camera)
	{
		this.Player = player;
		PlayerCamera = Camera;
		Init ();
		started = true;
	}

	/// <summary>
	/// Assign your Player and Camera and Initilize.////
	/// </summary>
	public void StartAsServer ()
	{
		Player = gameObject;
		serverMode = true;
		Init ();
		started = true;
	}

	/// <summary>
	/// Changes focus on other Player or Camera on runtime.////
	/// </summary>
	/// <param name="Player">Player.</param>
	/// <param name="Camera">Camera.</param>
	public void ChangeFocus (GameObject player, Camera Camera)
	{
		this.Player = player;
		PlayerCamera = Camera;
		InitImageEffects ();
	}
}


