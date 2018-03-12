using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class EnviroSkyRendering : MonoBehaviour
{

	public Material material;
	public RenderTexture skyRenderTexture;
	private Camera myCam;


	public void Apply()
	{
		myCam = GetComponent<Camera> ();

		RenderTargetIdentifier lowResRenderTarget = new RenderTargetIdentifier(skyRenderTexture);
		CommandBuffer cb = new CommandBuffer();
		cb.Blit(lowResRenderTarget, BuiltinRenderTextureType.CameraTarget,material);
		cb.name = "Enviro Sky Rendering";

		if (myCam.actualRenderingPath == RenderingPath.DeferredShading) {
			CommandBuffer[] cbs;
			cbs = myCam.GetCommandBuffers (CameraEvent.BeforeGBuffer);

			for (int i = 0; i < cbs.Length; i++) {

				if (cbs [i].name == "Enviro Sky Rendering")
					myCam.RemoveCommandBuffer (CameraEvent.BeforeGBuffer, cbs [i]);
			}

			myCam.AddCommandBuffer (CameraEvent.BeforeGBuffer, cb);

		} else {

			CommandBuffer[] cbs;
			cbs = myCam.GetCommandBuffers (CameraEvent.BeforeForwardOpaque);

			for (int i = 0; i < cbs.Length; i++) {

				if (cbs [i].name == "Enviro Sky Rendering")
					myCam.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, cbs [i]);
			}

			myCam.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);

		}

	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) 
	{
		var format= GetComponent<Camera>().hdr ? RenderTextureFormat.DefaultHDR: RenderTextureFormat.Default;
		RenderTexture tmpBuffer = RenderTexture.GetTemporary (source.width, source.height, 0, format);
		RenderTexture.active = tmpBuffer;
		GL.ClearWithSkybox (false, GetComponent<Camera>());

		material.SetTexture ("_Skybox", tmpBuffer);
		Graphics.Blit (source, destination);
		RenderTexture.ReleaseTemporary (tmpBuffer);
	}
}