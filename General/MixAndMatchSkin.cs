using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkinSpine = Spine.Skin;
using Spine;

public class MixAndMatchSkin : MonoBehaviour
{
	public SkeletonAnimation skeletonAnimation;
	public SkeletonGraphic skeletonGraphic;
	public MeshRenderer mesh;
	SkinSpine characterSkin;
	[SerializeField] private string ID;

	public SkinSpineType Type;

    public void CallStart(string _ID)
    {
		if(mesh == null) mesh = GetComponent<MeshRenderer>();
		ID = _ID;
		UpdateCharacterSkin();
		UpdateCombinedSkin();
	}

	private void UpdateCharacterSkin()
	{
		Skeleton skeleton;
		if (skeletonAnimation != null) skeleton = skeletonAnimation.Skeleton;
		else skeleton = skeletonGraphic.Skeleton;
		SkeletonData skeletonData = skeleton.Data;

		switch (Type)
        {
			case SkinSpineType.Hero:
				characterSkin = new SkinSpine("hero-base");
				characterSkin.AddSkin(skeletonData.FindSkin(ID));
				break;
		}
	}

	private void UpdateCombinedSkin()
	{
		Skeleton skeleton;
		if (skeletonAnimation != null) skeleton = skeletonAnimation.Skeleton;
		else skeleton = skeletonGraphic.Skeleton;

		var resultCombinedSkin = new SkinSpine("hero-combined");
		switch (Type)
		{
			case SkinSpineType.Hero:
				resultCombinedSkin = new SkinSpine("hero-combined");
				break;
		}

		resultCombinedSkin.AddSkin(characterSkin);

		skeleton.SetSkin(resultCombinedSkin);
		skeleton.SetSlotsToSetupPose();
	}

}

public enum SkinSpineType
{
	Hero
}