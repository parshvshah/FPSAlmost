using UnityEngine;
using System.Collections;

public class EnviroEvents : MonoBehaviour {


	[System.Serializable]
	public class EnviroActionEvent : UnityEngine.Events.UnityEvent
	{

	}
	//[Header("Time Events")]
	public EnviroActionEvent onHourPassedActions = new EnviroActionEvent();
	public EnviroActionEvent onDayPassedActions = new EnviroActionEvent();
	public EnviroActionEvent onYearPassedActions = new EnviroActionEvent();
	public EnviroActionEvent onWeatherChangedActions = new EnviroActionEvent();
	public EnviroActionEvent onSeasonChangedActions = new EnviroActionEvent();
	public EnviroActionEvent onNightActions = new EnviroActionEvent();
	public EnviroActionEvent onDayActions = new EnviroActionEvent();

	void Start ()
	{
		EnviroSky.instance.OnHourPassed += () => HourPassed ();
		EnviroSky.instance.OnDayPassed += () => DayPassed ();
		EnviroSky.instance.OnYearPassed += () => YearPassed ();
		EnviroSky.instance.OnWeatherChanged += (EnviroWeatherPrefab type) =>  WeatherChanged ();
		EnviroSky.instance.OnSeasonChanged += (SeasonVariables.Seasons season) => SeasonsChanged ();
		EnviroSky.instance.OnNightTime += () => NightTime ();
		EnviroSky.instance.OnDayTime += () => DayTime ();
	}
		
	private void HourPassed()
	{
		onHourPassedActions.Invoke();
	}

	private void DayPassed()
	{
		onDayPassedActions.Invoke();
	}
		
	private void YearPassed()
	{
		onYearPassedActions.Invoke();
	}

	private void WeatherChanged()
	{
		onWeatherChangedActions.Invoke();
	}

	private void SeasonsChanged()
	{
		onSeasonChangedActions.Invoke();
	}

	private void NightTime()
	{
		onNightActions.Invoke ();
	}

	private void DayTime()
	{
		onDayActions.Invoke ();
	}

}
