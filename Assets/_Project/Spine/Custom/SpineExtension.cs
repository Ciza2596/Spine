using UnityEngine;
using UnityEngine.Profiling;

namespace Spine.Unity
{
	public static class SpineExtension
	{
		public static bool CheckIsLoop(this SkeletonAnimation skeletonAnimation) =>
			skeletonAnimation.AnimationState.GetCurrent(0)?.loop ?? false;

		public static float GetDuration(this SkeletonAnimation skeletonAnimation) =>
			skeletonAnimation.AnimationState.GetCurrent(0)?.AnimationEnd ?? -1;

		public static float GetTime(this SkeletonAnimation skeletonAnimation) =>
			skeletonAnimation.AnimationState.GetCurrent(0)?.trackTime ?? 0;

		public static float GetNormalized(this SkeletonAnimation skeletonAnimation) =>
			Mathf.Clamp(skeletonAnimation.GetTime() / skeletonAnimation.GetDuration(), 0, 0.98f);

		public static void Play(this SkeletonAnimation skeletonAnimation, AnimationReferenceAsset clip, bool isLoop = false, float normalized = 0, int trackIndex = 0)
		{
			skeletonAnimation.AnimationState.SetAnimation(trackIndex, clip.Animation, isLoop);
			skeletonAnimation.RefreshByTime(normalized);
		}

		public static void Play(this SkeletonAnimation skeletonAnimation, string animState, bool isLoop = false, float normalized = 0, int trackIndex = 0)
		{
			skeletonAnimation.AnimationState.SetAnimation(trackIndex, animState, isLoop);
			skeletonAnimation.RefreshByTime(normalized);
		}

		public static void Stop(this SkeletonAnimation skeletonAnimation, int trackIndex = 0)
		{
			skeletonAnimation.AnimationState.ClearTrack(trackIndex);
			skeletonAnimation.Refresh();
		}

		public static void SetNormalized(this SkeletonAnimation skeletonAnimation, float normalized) =>
			skeletonAnimation.SetTime(normalized * skeletonAnimation.GetDuration());

		public static void SetTime(this SkeletonAnimation skeletonAnimation, float time)
		{
			var trackEntry = skeletonAnimation.AnimationState.GetCurrent(0);
			if (trackEntry != null)
				trackEntry.TrackTime = Mathf.Clamp(skeletonAnimation.CheckIsLoop() ? time % skeletonAnimation.GetDuration() : time, 0, float.MaxValue);
		}

		public static void RefreshByTime(this SkeletonAnimation skeletonAnimation, float time)
		{
			skeletonAnimation.SetTime(time);
			skeletonAnimation.Refresh();
		}

		public static void RefreshByNormalized(this SkeletonAnimation skeletonAnimation, float normalized)
		{
			skeletonAnimation.SetNormalized(normalized);
			skeletonAnimation.Refresh();
		}

		public static void Refresh(this SkeletonAnimation skeletonAnimation)
		{
			Profiler.BeginSample("Spine.Refresh");
			skeletonAnimation.Refresh(0);
			skeletonAnimation.Render(0);
			Profiler.EndSample();
		}

		public static void Tick(this SkeletonAnimation skeletonAnimation, float deltaTime)
		{
			Profiler.BeginSample("Spine.Tick");
			skeletonAnimation.SetTime(skeletonAnimation.GetTime() + deltaTime);
			skeletonAnimation.Refresh(0);
			skeletonAnimation.Render(0);
			Profiler.EndSample();
		}
	}
}