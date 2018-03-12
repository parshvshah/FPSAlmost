/////////////////////////////////////////////////////////////////////////////////////////////////////////
//////    EnviroMeshSeasons - Switches Materials on MEshRenderer for seasons                      ///////
/////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[AddComponentMenu("Enviro/Seasons for Meshes")]
public class EnviroMeshSeasons : MonoBehaviour {

	public Material SpringMaterial;
	public Material SummerMaterial;
	public Material AutumnMaterial;
	public Material WinterMaterial;

	private MeshRenderer myRenderer;

	void Start () 
	{
		myRenderer = GetComponent<MeshRenderer>();
		if (myRenderer == null)
		{
			Debug.LogError("Please correct script placement! We need a MeshRenderer to work with!");
			this.enabled = false;
		}

		UpdateSeasonMaterial ();

		EnviroSky.instance.OnSeasonChanged += (SeasonVariables.Seasons season) =>
		{
			UpdateSeasonMaterial ();
		};
	}
	
	// Check for correct Setup
	void OnEnable ()
	{

		if(SpringMaterial == null)
			{
			Debug.LogError("Please assign a spring material in Inspector!");
			this.enabled = false;
			}
		if(SummerMaterial == null)
			{
			Debug.LogError("Please assign a summer material in Inspector!");
			this.enabled = false;
			}
		if(AutumnMaterial == null)
			{
			Debug.LogError("Please assign a autumn material in Inspector!");
			this.enabled = false;
			}
		if(WinterMaterial == null)
			{
			Debug.LogError("Please assign a winter material in Inspector!");
			this.enabled = false;
			}
	}


	void UpdateSeasonMaterial ()
	{
		switch (EnviroSky.instance.Seasons.currentSeasons)
		{
		case SeasonVariables.Seasons.Spring:
			myRenderer.sharedMaterial = SpringMaterial;
			break;
			
		case SeasonVariables.Seasons.Summer:
			myRenderer.sharedMaterial = SummerMaterial;
			break;
			
		case SeasonVariables.Seasons.Autumn:
			myRenderer.sharedMaterial = AutumnMaterial;
			break;
			
		case SeasonVariables.Seasons.Winter:
			myRenderer.sharedMaterial = WinterMaterial;
			break;
		}
	}
}
