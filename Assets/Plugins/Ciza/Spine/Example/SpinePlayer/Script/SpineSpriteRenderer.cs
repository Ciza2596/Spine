using UnityEngine;
using UnityEngine.Rendering;

public class SpineSpriteRenderer : MonoBehaviour
{
	[SerializeField]
	protected Vector2Int _textureSize = new Vector2Int(512, 512);

	protected Texture2D _outputTexture;
	protected Sprite _sprite;
	protected CommandBuffer _commandBuffer;

	protected RenderTexture _renderTexture;

	protected Mesh Mesh => GetComponentInChildren<MeshFilter>().mesh;
	protected Material Material => GetComponentInChildren<MeshRenderer>().sharedMaterial;
	protected SpriteRenderer SpriteRenderer => GetComponentInChildren<SpriteRenderer>();

	protected virtual void Awake()
	{
		// 創建 RenderTexture & Texture2D
		_outputTexture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.ARGB32, false);

		// 創建 Sprite 並賦值給 SpriteRenderer
		_sprite = Sprite.Create(_outputTexture, new Rect(0, 0, _textureSize.x, _textureSize.y), new Vector2(0.5f, 0.5f));
		SpriteRenderer.sprite = _sprite;

		// 初始化 CommandBuffer
		_commandBuffer = new CommandBuffer { name = "Render SkeletonMesh to Texture" };
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
			_renderTexture = null;
		}
		
		if (_outputTexture)
		{
			Destroy(_outputTexture);
			_outputTexture = null;
		}
	}

	protected virtual void Update()
	{
		Refresh();
	}

	public virtual void Refresh()
	{
		if (Mesh == null || Material == null)
		{
			Debug.LogError("請確保 Mesh 和 Material 已設定");
			return;
		}

		PrepareRenderTexture(_textureSize);
		PrepareCommandBuffer();
		DrawCommandBuffer();
		DrawOutputTexture();
	}

	protected virtual void PrepareRenderTexture(Vector2 drawDimensions)
	{
		var requiredSize = new Vector2Int(Mathf.RoundToInt(drawDimensions.x), Mathf.RoundToInt(drawDimensions.y));
		if (CurrentTextureValid())
			return;

		if (_renderTexture)
			RenderTexture.ReleaseTemporary(_renderTexture);

		_renderTexture = RenderTexture.GetTemporary(requiredSize.x, requiredSize.y);
		bool CurrentTextureValid() => _renderTexture && _renderTexture.width == requiredSize.x && _renderTexture.height == requiredSize.y;
	}

	protected virtual void PrepareCommandBuffer()
	{
		_commandBuffer.Clear();
		_commandBuffer.SetRenderTarget(_renderTexture);
		_commandBuffer.ClearRenderTarget(true, true, Color.clear);
	}

	protected virtual void DrawCommandBuffer()
	{
		_commandBuffer.DrawMesh(Mesh, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one), Material);
		Graphics.ExecuteCommandBuffer(_commandBuffer);
	}

	protected virtual void DrawOutputTexture()
	{
		RenderTexture.active = _renderTexture;
		_outputTexture.ReadPixels(new Rect(0, 0, _textureSize.x, _textureSize.y), 0, 0);
		_outputTexture.Apply();
		RenderTexture.active = null;
	}
}