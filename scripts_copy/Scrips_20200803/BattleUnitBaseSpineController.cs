using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
namespace PCRCaculator.Battle
{

    public enum eAnimationType 
    { 
        run_start=0,idle=1,attack=2,attack_skipQuest=3,damage=4,die=5,run=6,walk=7,standBy = 8,multi_standBy = 9,
        joy_long = 10,joy_long_return = 11,joy_short = 12,joy_short_return = 13,
        
        skill0 = 20, skill0_1 = 21, skill0_2 = 22, skill0_3 = 23, skill0_4 = 24,
        skill1 = 30, skill1_1 = 31, skill1_2 = 32, skill1_3 = 33, skill1_4 = 34,
        skill2 = 40, skill2_1 = 41, skill2_2 = 42, skill2_3 = 43, skill2_4 = 44

    }
    public class BattleUnitBaseSpineController : MonoBehaviour
    {
        public SkeletonAnimation sa;
        public int unitid;
        public List<Spine.Animation> spineBasicAnimations = new List<Spine.Animation>();
        public List<Spine.Animation> spineSkillAnimations = new List<Spine.Animation>();

        private float timeScale = 1;
        private float playSpeed = 1;
        private bool isPause = false;
        private eAnimationType currentAnimation;
        private UnitCtrl owner;

        private Dictionary<string, eAnimationType> animationNameDic = new Dictionary<string, eAnimationType>();

        private static string[] basicAnimationName = new string[14]
        {
        "_run_gamestart","_idle","_attack","_attack_skipQuest",
        "_damage","_die","_run","_walk","_standBy","_multi_standBy",
        "_joy_long","_joy_long_return","_joy_short","_joy_short_return"
        };
        private static string[] skillAnimationName = new string[15]
        {
        "_skill0","_skill0_1","_skill0_2","_skill0_3","_skill0_4",
        "_skill1","_skill1_1","_skill1_2","_skill1_3","_skill1_4",
        "_skill2","_skill2_1","_skill2_2","_skill2_3","_skill2_4"
        };

        public bool IsPlayingAnimeBattleComplete { get => CheckIsPlayingAnimeBattle();}

        /// <summary>
        /// 初始化动画控制器
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="dataAsset"></param>
        /// <param name="unitid"></param>
        public void SetOnAwake(SkeletonDataAsset dataAsset, int unitid,UnitCtrl unitCtrl)
        {
            this.sa = gameObject.GetComponent<SkeletonAnimation>();
            this.unitid = unitid;
            owner = unitCtrl;
            int motiontype = MainManager.Instance.UnitRarityDic[unitid].detailData.motionType;
            string motiontype_str = motiontype < 10 ? "0" + motiontype : "" + motiontype;
            for (int i = 0; i < basicAnimationName.Length; i++)
            {
                string animationName = motiontype_str + basicAnimationName[i];
                var spineAnimation = dataAsset.GetSkeletonData(false).FindAnimation(animationName);
                spineBasicAnimations.Add(spineAnimation);
                animationNameDic.Add(animationName, (eAnimationType)i);
            }
            for (int i = 0; i < skillAnimationName.Length; i++)
            {
                string animationName = unitid + skillAnimationName[i];
                var spineAnimation = dataAsset.GetSkeletonData(false).FindAnimation(animationName);
                spineSkillAnimations.Add(spineAnimation);
                animationNameDic.Add(animationName, (eAnimationType)(i + 21));
            }

        }
        /// <summary>
        /// 设置动画状态，由UnitCtrl调用
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isloop"></param>
        public void SetAnimaton(eAnimationType type,bool isloop,float speed = 1)
        {
            //Resume();
            Spine.Animation animation = null;
            if ((int)type <= 19)
            {
                animation = spineBasicAnimations[(int)type];
            }
            else if((int)type<=29)
            {
                animation =  spineSkillAnimations[(int)type-20];
            }
            else if ((int)type <= 39)
            {
                animation =  spineSkillAnimations[(int)type-25];
            }
            else if ((int)type <= 49)
            {
                animation = spineSkillAnimations[(int)type - 30];
            }
            if(animation != null)
            {
                sa.AnimationState.SetAnimation(0, animation, isloop);
                playSpeed = speed;
                sa.timeScale = timeScale * playSpeed;
                currentAnimation = type;
            }
            else
            {
                Debug.LogError("动画不存在！");
            }
        }
        public void SetTimeScale(float scale)
        {
            timeScale = scale;
            sa.timeScale = isPause ? 0 : timeScale * playSpeed;
        }
        public void Pause(bool isPause = true)
        {
            this.isPause = isPause;
            sa.timeScale = isPause ? 0 : timeScale * playSpeed;
        }
        public void SetCurColor(Color color)
        {
            //mpb.SetColor("_OverlayColor", color);
            //meshRenderer.SetPropertyBlock(mpb);
            sa.skeleton.SetColor(color);
        }
        public void Resume()
        {
            sa.ClearState();
            sa.timeScale = owner.TimeScale;
        }
        public eAnimationType GetCurrentAnimationName()
        {
            string name =  sa.AnimationState.GetCurrent(0).animation.name;
            return animationNameDic[name];
        }
        public bool HasAnimation(eAnimationType eAnimation)
        {
            if ((int)eAnimation <= 30)
            {
                return true;
            }
            return spineSkillAnimations[(int)eAnimation - 27] != null;
        }
        public bool HasNextAnimation()
        {
            eAnimationType eAnimation = currentAnimation;
            if((int)eAnimation >= 31 && (int)eAnimation <=33)
            {
                return spineSkillAnimations[(int)eAnimation - 25] == null;
            }
            if ((int)eAnimation >= 41 && (int)eAnimation <= 43)
            {
                return spineSkillAnimations[(int)eAnimation - 30] == null;
            }
            return false;
        }
        private bool CheckIsPlayingAnimeBattle()
        {
            return sa.AnimationState.GetCurrent(0).IsComplete;
        }
    }
}