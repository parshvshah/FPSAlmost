/// <summary>
/// This component can be used to synchronize time and weather in games where server is a player too.
/// </summary>

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
[AddComponentMenu("Enviro/Integration/UNet Server Component")]
[RequireComponent(typeof (NetworkIdentity))]
public class EnviroUNetServer : NetworkBehaviour {

	public float updateSmoothing = 15f;

	[SyncVar] private float networkHours;
	[SyncVar] private int networkDays;
	[SyncVar] private int networkYears;

	public bool isHeadless = true;
	//public bool disableCamera;

	public override void OnStartServer()
	{
		if (isHeadless) {
			EnviroSky.instance.StartAsServer();
		}
			
		EnviroSky.instance.Weather.updateWeather = true;
			
		EnviroSky.instance.OnSeasonChanged += (SeasonVariables.Seasons season) => {
			SendSeasonToClient (season);
		};
		EnviroSky.instance.OnZoneWeatherChanged += (EnviroWeatherPrefab type, EnviroZone zone) => {
			SendWeatherToClient (type, zone);
		};
	}

	public void Start ()
	{
		if (!isServer) {
			EnviroSky.instance.GameTime.ProgressTime = TimeVariables.TimeProgressMode.None;
			EnviroSky.instance.Weather.updateWeather = false;
		}
	}

	void SendWeatherToClient (EnviroWeatherPrefab w, EnviroZone z)
	{
		int zoneID = 0;

		for (int i = 0; i < EnviroSky.instance.Weather.zones.Count; i++) 
		{
			if (EnviroSky.instance.Weather.zones [i] == z)
				zoneID = i;

		}

		for (int i = 0; i < EnviroSky.instance.Weather.WeatherTemplates.Count; i++) {

			if (EnviroSky.instance.Weather.WeatherTemplates [i] == w)
				RpcWeatherUpdate (i,zoneID);
		}
	}

	void SendSeasonToClient (SeasonVariables.Seasons s)
	{
		RpcSeasonUpdate((int)s);
	}

	[ClientRpc]
	void RpcSeasonUpdate (int season)
	{
		EnviroSky.instance.Seasons.currentSeasons = (SeasonVariables.Seasons)season;
	}

	[ClientRpc]
	void RpcWeatherUpdate (int weather, int zone)
	{
		EnviroSky.instance.Weather.zones[zone].currentActiveZoneWeatherID = EnviroSky.instance.Weather.WeatherTemplates [weather];
	}


	void Update ()
	{
		if (!isServer) 
		{
			if (networkHours < 1f && EnviroSky.instance.internalHour > 23f)
				EnviroSky.instance.SetInternalTimeOfDay(networkHours);

			EnviroSky.instance.SetInternalTimeOfDay(Mathf.Lerp (EnviroSky.instance.GetInternalTimeOfDay(), (float)networkHours, Time.deltaTime * updateSmoothing));
			EnviroSky.instance.GameTime.Days = networkDays;
			EnviroSky.instance.GameTime.Years = networkYears;

		} else {
			networkHours = EnviroSky.instance.GetInternalTimeOfDay();
			networkDays = EnviroSky.instance.GameTime.Days;
			networkYears = EnviroSky.instance.GameTime.Years;
		}

	}
}

