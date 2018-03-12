/// <summary>
/// This component can be used to synchronize time and weather in games where server is a player too.
/// </summary>

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
[AddComponentMenu("Enviro/Integration/UNet Player")]
[RequireComponent(typeof (NetworkIdentity))]
public class EnviroUNetPlayer : NetworkBehaviour {

	public bool AssignOnStart = true;
	public GameObject Player;
	public Camera PlayerCamera;

	public void Start()
	{
		// Deactivate if it isn't ours!
		if (!isLocalPlayer && !isServer) {
			this.enabled = false;
			return;
		}

		if (isLocalPlayer) 
		{
			if (AssignOnStart && Player != null && PlayerCamera != null)
				EnviroSky.instance.AssignAndStart (Player, PlayerCamera);

			Cmd_RequestSeason ();
			Cmd_RequestCurrentWeather ();
		}
	}
		
	[Command]
	void Cmd_RequestSeason ()
	{
		RpcRequestSeason((int)EnviroSky.instance.Seasons.currentSeasons);
	}

	[ClientRpc]
	void RpcRequestSeason (int season)
	{
		EnviroSky.instance.Seasons.currentSeasons = (SeasonVariables.Seasons)season;
	}

	[Command]
	void Cmd_RequestCurrentWeather ()
	{
		for (int i = 0; i < EnviroSky.instance.Weather.zones.Count; i++) 
		{
			for (int w = 0; w < EnviroSky.instance.Weather.WeatherTemplates.Count; w++)
			{
				if(EnviroSky.instance.Weather.WeatherTemplates[w] == EnviroSky.instance.Weather.zones[i].currentActiveZoneWeatherID)
					RpcRequestCurrentWeather(w,i);
			}
		}
	}

	[ClientRpc]
	void RpcRequestCurrentWeather (int weather, int zone)
	{
		EnviroSky.instance.Weather.zones[zone].currentActiveZoneWeatherID = EnviroSky.instance.Weather.WeatherTemplates [weather];
	}
}
