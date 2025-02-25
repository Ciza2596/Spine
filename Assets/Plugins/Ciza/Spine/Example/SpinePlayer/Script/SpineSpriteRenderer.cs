using Spine.Unity;
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

	protected SkeletonAnimation SpineAnimation => GetComponentInChildren<SkeletonAnimation>();
	protected MeshFilter SpineMeshFilter => SpineAnimation.GetComponentInChildren<MeshFilter>();
	protected MeshRenderer SpineRenderer => SpineAnimation.GetComponentInChildren<MeshRenderer>();
	
	protected SpriteRenderer SpriteRenderer => GetComponentInChildren<SpriteRenderer>();

	protected virtual void Awake()
	{
		_outputTexture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.ARGB32, false);

		_sprite = Sprite.Create(_outputTexture, new Rect(0, 0, _textureSize.x, _textureSize.y), new Vector2(0.5f, 0.5f));
		SpriteRenderer.sprite = _sprite;

		_commandBuffer = new CommandBuffer { name = "SpineSpriteRenderer" };
	}

	protected virtual void OnDestroy()
	{
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
		if (SpineMeshFilter == null || SpineRenderer == null)
		{
			Debug.LogError("請確保 Mesh 和 Material 已設定");
			return;
		}

		PrepareRenderTexture();
		PrepareCommandBuffer();
		DrawCommandBuffer();
		DrawOutputTexture();
	}

	protected virtual void PrepareRenderTexture()
	{
		var requiredSize = new Vector2Int(Mathf.RoundToInt(_textureSize.x), Mathf.RoundToInt(_textureSize.y));
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
		_commandBuffer.DrawMesh(SpineMeshFilter.mesh, transform.localToWorldMatrix, SpineRenderer.material, 0, -1);
		// _commandBuffer.DrawRenderer(SpineRenderer, SpineRenderer.material);
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