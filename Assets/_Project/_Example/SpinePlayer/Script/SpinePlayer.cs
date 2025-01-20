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
		Debug.Log(SkeletonAnimation.GetDataId());
	}

	protected virtual void Update()
	{
		if (_isAutoUpdate)
		{
			SkeletonAnimation.Tick(Time.deltaTime);
			Debug.Log(SkeletonAnimation.GetTime());
		}
		else
			SkeletonAnimation.RefreshByNormalized(_normalized);
	}
}