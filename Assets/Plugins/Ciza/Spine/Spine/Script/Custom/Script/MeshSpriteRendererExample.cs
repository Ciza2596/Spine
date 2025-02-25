using UnityEngine;

public class MeshSpriteRendererExample : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	private void Start()
	{
		GenerateSpriteFromMesh();
	}

	public void GenerateSpriteFromMesh()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
		MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

		if (meshFilter == null || meshRenderer == null || meshFilter.sharedMesh == null)
		{
			Debug.LogError("缺少 MeshFilter 或 MeshRenderer 或 Mesh");
			return;
		}

		// 取得 Mesh 資訊
		Mesh mesh = meshFilter.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Bounds bounds = mesh.bounds;

		// 取得 MeshRenderer 的材質與貼圖
		Material material = meshRenderer.sharedMaterial;
		Texture2D mainTexture = material.mainTexture as Texture2D;

		if (mainTexture == null)
		{
			Debug.LogError("材質沒有 MainTexture，無法生成 Sprite");
			return;
		}

		// 計算 UV 範圍
		float minU = float.MaxValue, maxU = float.MinValue;
		float minV = float.MaxValue, maxV = float.MinValue;

		foreach (Vector2 uvCoord in uv)
		{
			if (uvCoord.x < minU) minU = uvCoord.x;
			if (uvCoord.x > maxU) maxU = uvCoord.x;
			if (uvCoord.y < minV) minV = uvCoord.y;
			if (uvCoord.y > maxV) maxV = uvCoord.y;
		}

		// 轉換 UV 到貼圖像素座標
		int x = Mathf.RoundToInt(minU * mainTexture.width);
		int y = Mathf.RoundToInt(minV * mainTexture.height);
		int width = Mathf.RoundToInt((maxU - minU) * mainTexture.width);
		int height = Mathf.RoundToInt((maxV - minV) * mainTexture.height);

		// 從 Texture 擷取對應範圍
		Texture2D croppedTexture = new Texture2D(width, height);
		Color[] pixels = mainTexture.GetPixels(x, y, width, height);
		croppedTexture.SetPixels(pixels);
		croppedTexture.Apply();

		// 轉換為 Sprite
		Sprite sprite = Sprite.Create(croppedTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
		spriteRenderer.sprite = sprite;
	}
}