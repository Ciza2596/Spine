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

	protected virtual SkeletonAnimation SkeletonAnimation => GetComponentInChildren<SkeletonAnimation>();

	protected virtual void OnEnable()
	{
		SkeletonAnimation.Play(_clip, _isLoop);
	}

	protected virtual void Update()
	{
		SkeletonAnimation.RefreshByTime(_time);
		_runtime = SkeletonAnimation.GetTime();
	}
}