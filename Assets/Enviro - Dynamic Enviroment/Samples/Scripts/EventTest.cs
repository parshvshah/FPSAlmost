using UnityEngine;
using System.Collections;

public class EventTest : MonoBehaviour {

	void Start ()
	{
		EnviroSky.instance.OnWeatherChanged += (EnviroWeatherPrefab type) =>
		{
			Debug.Log("Weather changed to: " + type.Name);
		};

		EnviroSky.instance.OnSeasonChanged += (SeasonVariables.Seasons season) =>
		{
			Debug.Log("Season changed");
		};

		EnviroSky.instance.OnHourPassed += () =>
		{
			Debug.Log("Hour Passed!");
		};

		EnviroSky.instance.OnDayPassed += () =>
		{
			Debug.Log("New Day!");
		};
		EnviroSky.instance.OnYearPassed += () =>
		{
			Debug.Log("New Year!");
		};

	}

	public void TestEventsWWeather ()
	{
		print("Weather Changed though interface!");
	}

	public void TestEventsNight ()
	{
		print("Night now!!");
	}

	public void TestEventsDay ()
	{
		print("Day now!!");
	}
}
