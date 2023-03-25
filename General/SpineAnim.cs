using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnim : MonoBehaviour
{
    public SkeletonAnimation bone;
    public SkeletonGraphic boneUI;

    void OnEnable()
    {
        if (!bone && boneUI == null)
            bone = GetComponentInChildren<SkeletonAnimation>();
    }

    public string currentAnimation;
    public void SetAnim(int track = 0, string name = "", bool loop = false)
    {
        if (name == currentAnimation)
            return;
        currentAnimation = name;
        bone.state.SetAnimation(track, name, loop);
    }

    public void SetSkin(string skin)
    {
        bone.initialSkinName = skin;
    }

    public void SetSpeed()
    {
        bone.timeScale = Time.timeScale;
    }
    private bool IsSetEvent = false;
    public float Play(string Name, bool loop = true, float speed = 1f, System.Action action = null)
    {
        if (bone != null) if (!bone.gameObject.activeInHierarchy) return 0;
        if (Name == currentAnimation)
            return 0;
        if (boneUI == null)
        {
            var anim = bone.state.SetAnimation(0, Name, loop);
            anim.TimeScale = speed;
            currentAnimation = Name;
            if (!IsSetEvent)
            {
                bone.AnimationState.Event += HandleEvent;
                IsSetEvent = true;
            }
            float time = anim.Animation.Duration / speed;
            if (action != null)
            {
                if (coroutineWaitEndAnimation != null) StopCoroutine(coroutineWaitEndAnimation);
                coroutineWaitEndAnimation = StartCoroutine(DelayAction(time, action));
            }
            return time;
        }
        else
        {
            var anim = boneUI.AnimationState.SetAnimation(0, Name, loop);
            anim.TimeScale = speed;
            currentAnimation = Name;
            if (!IsSetEvent)
            {
                boneUI.AnimationState.Event += HandleEvent;
                IsSetEvent = true;
            }
            float time = anim.Animation.Duration / speed;
            if (action != null)
            {
                if (coroutineWaitEndAnimation != null) StopCoroutine(coroutineWaitEndAnimation);
                coroutineWaitEndAnimation = StartCoroutine(DelayAction(time, action));
            }
            return time;
        }
    }

    Coroutine coroutineWaitEndAnimation;
    IEnumerator DelayAction(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        if (action != null) action();
    }

    private void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "events")
        {

        }
    }
}