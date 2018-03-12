using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoUI : MonoBehaviour {


	public UnityEngine.UI.Slider sliderTime;
	public UnityEngine.UI.Slider sliderQuality;
	public UnityEngine.UI.Text timeText;
	public UnityEngine.UI.Dropdown weatherDropdown;
	public UnityEngine.UI.Dropdown seasonDropdown;

 	bool seasonmode = true;
	bool fastdays = false;

	private bool started = false;

	void Start () 
	{
		EnviroSky.instance.OnWeatherChanged += (EnviroWeatherPrefab type) =>
		{
			UpdateWeatherSlider ();	
		};

		EnviroSky.instance.OnSeasonChanged += (SeasonVariables.Seasons season) =>
		{
			UpdateSeasonSlider(season);
		};
	}
		
	IEnumerator setupDrodown ()
	{
		started = true;
		yield return new WaitForSeconds (0.1f);

		for (int i = 0; i < EnviroSky.instance.Weather.WeatherTemplates.Count; i++) {
			UnityEngine.UI.Dropdown.OptionData o = new UnityEngine.UI.Dropdown.OptionData();
			o.text = EnviroSky.instance.Weather.WeatherTemplates [i].Name;
			weatherDropdown.options.Add (o);
		}

		yield return new WaitForSeconds (0.1f);
		UpdateWeatherSlider ();
	}

	public void ChangeTimeSlider () 
	{
		if (sliderTime.value < 0.01f)
			sliderTime.value = 0.01f;
		EnviroSky.instance.SetInternalTimeOfDay (sliderTime.value * 24f);
	}


	public void ChangeTimeLenghtSlider (float value) 
	{
		EnviroSky.instance.GameTime.DayLengthInMinutes = value;
	}

	public void ChangeQualitySlider () 
	{
		EnviroSky.instance.Quality.GlobalParticleEmissionRates = sliderQuality.value;
	}

	public void ChangeCloudsSlider (float value) 
	{
		//EnviroSky.instance.Clouds.Quality = Mathf.RoundToInt(value);
		EnviroSky.instance.InitClouds ();
	}

	public void SetWeatherID (int id) 
	{
		EnviroSky.instance.ChangeWeather (id);
	}

	public void OverwriteSeason ()
	{
		if (!seasonmode) {
			seasonmode = true;
			EnviroSky.instance.Seasons.calcSeasons = true;
		}
		else {
			seasonmode = false;
			EnviroSky.instance.Seasons.calcSeasons = false;
		}
		
	}

	public void FastDays ()
	{
		if (!fastdays) {
			fastdays = true;
			EnviroSky.instance.GameTime.DayLengthInMinutes = 0.2f;
		}
		else {
			fastdays = false;
			EnviroSky.instance.GameTime.DayLengthInMinutes = 5f;
		}

	}

	public void SetSeason (int id)
	{
		switch (id) 
		{
		case 0:
			EnviroSky.instance.ChangeSeason (SeasonVariables.Seasons.Spring);
		break;
		case 1:
			EnviroSky.instance.ChangeSeason (SeasonVariables.Seasons.Summer);
			break;
		case 2:
			EnviroSky.instance.ChangeSeason (SeasonVariables.Seasons.Autumn);
			break;
		case 3:
			EnviroSky.instance.ChangeSeason (SeasonVariables.Seasons.Winter);
			break;
		}
	}


	public void SetTimeProgress (int id)
	{
		switch (id) 
		{
		case 0:
			EnviroSky.instance.GameTime.ProgressTime = TimeVariables.TimeProgressMode.None;
			break;
		case 1:
			EnviroSky.instance.GameTime.ProgressTime = TimeVariables.TimeProgressMode.Simulated;
			break;
		case 2:
			EnviroSky.instance.GameTime.ProgressTime = TimeVariables.TimeProgressMode.SystemTime;
			break;
		}
	}
		
	public void SetFog (bool i)
	{
		EnviroSky.instance.Fog.AdvancedFog = i;
	}


	void UpdateWeatherSlider ()
	{
		if (EnviroSky.instance.Weather.currentActiveWeatherID != null) {
			for (int i = 0; i < weatherDropdown.options.Count; i++) {
				if (weatherDropdown.options [i].text == EnviroSky.instance.Weather.currentActiveWeatherID.Name)
					weatherDropdown.value = i;
			}
		}
	}

	void UpdateSeasonSlider (SeasonVariables.Seasons s)
	{
		switch (s) {
		case SeasonVariables.Seasons.Spring:
			seasonDropdown.value = 0;
			break;
		case SeasonVariables.Seasons.Summer:
			seasonDropdown.value = 1;
			break;
		case SeasonVariables.Seasons.Autumn:
			seasonDropdown.value = 2;
			break;
		case SeasonVariables.Seasons.Winter:
			seasonDropdown.value = 3;
			break;
		}
	}

	void Update ()
	{
		if (!EnviroSky.instance.started)
			return;
		else {
			if(!started)
				StartCoroutine(setupDrodown ());
		}

		timeText.text = EnviroSky.instance.GetTimeString ();
		ChangeQualitySlider ();
	}

	void LateUpdate ()
	{
		sliderTime.value = EnviroSky.instance.GetInternalTimeOfDay() / 24f;
	}
}
