using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

public class SpineMeshRenderer : MonoBehaviour
{
	protected CommandBuffer _commandBuffer;
	protected RenderTexture _renderTexture;

	protected virtual SkeletonAnimation SpineAnimation => GetComponentInChildren<SkeletonAnimation>();
	protected virtual MeshFilter SpineMeshFilter => SpineAnimation.GetComponentInChildren<MeshFilter>();
	protected virtual MeshRenderer SpineRenderer => SpineAnimation.GetComponentInChildren<MeshRenderer>();
	

	protected virtual MeshFilter MeshFilter => GetComponent<MeshFilter>();
	protected virtual MeshRenderer MeshRenderer => GetComponent<MeshRenderer>();

	protected virtual void Awake()
	{
		_commandBuffer = new CommandBuffer { name = "SpineMeshRenderer" };
	}

	protected virtual void OnDestroy()
	{
		// 釋放 CommandBuffer
		if (_commandBuffer != null)
		{
			_commandBuffer.Release();
			_commandBuffer = null;
		}

		// 釋放 RenderTexture
		if (_renderTexture)
		{
			RenderTexture.ReleaseTemporary(_renderTexture);
			MeshRenderer.material.mainTexture = null;
			_renderTexture = null;
		}
	}

	protected virtual void Update()
	{
		Refresh();
	}

	public virtual void Refresh()
	{
		if (SpineMeshFilter == null || SpineRenderer == null)
		{
			Debug.LogError("請確保 Mesh 和 Material 已設定");
			return;
		}

		PrepareRenderTexture();
		PrepareMesh();
		PrepareCommandBuffer();
		DrawCommandBuffer();

		// **將 Spine 的 Mesh 複製到 MeshFilter**
		// MeshFilter.mesh = SpineMeshFilter.mesh;
		// MeshRenderer.material = SpineRenderer.material;
		// MeshRenderer.material.mainTexture = _renderTexture;
	}

	protected virtual void PrepareRenderTexture()
	{
		var meshBounds = SpineMeshFilter.mesh.bounds;

		// 計算 RenderTexture 的大小（避免過小或過大）
		int textureWidth = Mathf.Max(128, Mathf.CeilToInt(meshBounds.size.x * 100));
		int textureHeight = Mathf.Max(128, Mathf.CeilToInt(meshBounds.size.y * 100));

		if (CurrentTextureValid(textureWidth, textureHeight))
			return;

		if (_renderTexture)
			RenderTexture.ReleaseTemporary(_renderTexture);

		_renderTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
		MeshRenderer.material.mainTexture = _renderTexture;
	}

	protected virtual bool CurrentTextureValid(int width, int height)
	{
		return _renderTexture && _renderTexture.width == width && _renderTexture.height == height;
	}

	protected virtual void PrepareMesh()
	{
		var mesh = SpineMeshFilter.mesh;
		var clonedMesh = new Mesh
		{
			vertices = mesh.vertices,
			triangles = mesh.triangles,
			uv = mesh.uv,
			normals = mesh.normals,
			tangents = mesh.tangents,
			colors = mesh.colors
		};
		
		MeshFilter.mesh = clonedMesh;
	}

	protected virtual void PrepareCommandBuffer()
	{
		_commandBuffer.Clear();
		_commandBuffer.SetRenderTarget(_renderTexture);
		_commandBuffer.ClearRenderTarget(true, true, Color.clear);
	}

	protected virtual void DrawCommandBuffer()
	{
		_commandBuffer.DrawMesh(SpineMeshFilter.mesh, Matrix4x4.identity, SpineRenderer.material);
		Graphics.ExecuteCommandBuffer(_commandBuffer);
	}
}