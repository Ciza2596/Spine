using Spine.Unity;
using UnityEngine;

public class SetTime : MonoBehaviour
{
	[SerializeField]
	protected AnimationReferenceAsset _clip;

	[SerializeField]
	protected bool _isLoop = false;

	[SerializeField]
	protected float _time;

	[SerializeField]
	protected float _runtime;

	[SerializeField]
	protected bool _isSkeleton;

	[SerializeField]
	[Range(0, 1)]
	protected float _alpha = 1f;

	protected MaterialPropertyBlock _propertyBlock;


	protected virtual SkeletonAnimation SkeletonAnimation => GetComponentInChildren<SkeletonAnimation>();
	protected virtual MeshRenderer MeshRenderer => GetComponentInChildren<MeshRenderer>();

	protected virtual void OnEnable()
	{
		SkeletonAnimation.Play(_clip, _isLoop);
		_propertyBlock = new MaterialPropertyBlock();
	}

	protected virtual void Update()
	{
		SkeletonAnimation.RefreshByTime(_time);
		_runtime = SkeletonAnimation.GetTime();

		// if (_isSkeleton)
		// {
		// 	var color = SkeletonAnimation.Skeleton.GetColor();
		// 	SkeletonAnimation.Skeleton.SetColor(new Color(color.r, color.g, color.b, _alpha));
		// }
		// else
		// {
		// 	var color = MeshRenderer.sharedMaterial.color;
		// 	color.a = _alpha;
		// 	MeshRenderer.sharedMaterial.color = color;


		// MeshRenderer.GetPropertyBlock(_propertyBlock);
		// Color currentColor = _propertyBlock.GetColor("_TintColor"); // 取得當前的顏色
		// currentColor.a = _alpha; // 設定新的透明度
		// _propertyBlock.SetColor("_Color", currentColor); 
		// MeshRenderer.SetPropertyBlock(_propertyBlock);
		// }
	}
}