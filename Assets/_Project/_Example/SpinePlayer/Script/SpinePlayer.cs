using Spine.Unity;
using UnityEngine;

public class SpinePlayer : MonoBehaviour
{
	[SerializeField]
	protected AnimationReferenceAsset _clip;

	[SerializeField]
	protected bool _isLoop = true;

	[Space]
	[SerializeField]
	protected bool _isAutoUpdate;

	[Range(0, 1)]
	[SerializeField]
	protected float _normalized;

	protected virtual SkeletonAnimation SkeletonAnimation => GetComponentInChildren<SkeletonAnimation>();

	protected virtual void OnEnable()
	{
		SkeletonAnimation.Play(_clip, _isLoop);
	}

	protected virtual void Update()
	{
		if (_isAutoUpdate)
			SkeletonAnimation.Tick(Time.deltaTime);
		else
			SkeletonAnimation.RefreshByNormalized(_normalized);
	}
}