/////////////////////////////////////////////////////////////////////////////////////////////////////////
//////    EnviroMeshSeasons - Switches Materials on MeshRenderer for seasons                      ///////
/////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[AddComponentMenu("Enviro/Seasons for GameObjects")]
public class EnviroSeasonObjectSwitcher : MonoBehaviour {

	public GameObject SpringObject;
	public GameObject SummerObject;
	public GameObject AutumnObject;
	public GameObject WinterObject;

	void Start () 
	{
		SwitchSeasonObject ();

		EnviroSky.instance.OnSeasonChanged += (SeasonVariables.Seasons season) =>
		{
			SwitchSeasonObject ();
		};
	}
	
	// Check for correct Setup
	void OnEnable ()
	{

		if(SpringObject == null)
			{
			Debug.LogError("Please assign a spring Object in Inspector!");
			this.enabled = false;
			}
		if(SummerObject == null)
			{
			Debug.LogError("Please assign a summer Object in Inspector!");
			this.enabled = false;
			}
		if(AutumnObject == null)
			{
			Debug.LogError("Please assign a autumn Object in Inspector!");
			this.enabled = false;
			}
		if(WinterObject == null)
			{
			Debug.LogError("Please assign a winter Object in Inspector!");
			this.enabled = false;
			}
	}

	void SwitchSeasonObject ()
	{
		switch (EnviroSky.instance.Seasons.currentSeasons)
		{
		case SeasonVariables.Seasons.Spring:
			SummerObject.SetActive(false);
			AutumnObject.SetActive(false);
			WinterObject.SetActive(false);
			SpringObject.SetActive(true);
			break;
			
		case SeasonVariables.Seasons.Summer:
			SpringObject.SetActive(false);
			AutumnObject.SetActive(false);
			WinterObject.SetActive(false);
			SummerObject.SetActive(true);
			break;
			
		case SeasonVariables.Seasons.Autumn:
			SpringObject.SetActive(false);
			SummerObject.SetActive(false);
			WinterObject.SetActive(false);
			AutumnObject.SetActive(true);
			break;
			
		case SeasonVariables.Seasons.Winter:
			SpringObject.SetActive(false);
			SummerObject.SetActive(false);
			AutumnObject.SetActive(false);
			WinterObject.SetActive(true);
			break;
		}
	}
}
