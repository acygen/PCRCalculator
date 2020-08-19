using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
namespace PCRCaculator.Battle
{

    public enum eAnimationType 
    { 
        run_start=0,idle=1,attack=2,attack_skipQuest=3,damage=4,die=5,run=6,walk=7,standBy = 8,multi_standBy = 9,
        
        skill0=21,skill1=22,skill2=23 
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

        private Dictionary<string, eAnimationType> animationNameDic = new Dictionary<string, eAnimationType>();

        private static string[] basicAnimationName = new string[10]
        {
        "_run_gamestart","_idle","_attack","_attack_skipQuest",
        "_damage","_die","_run","_walk","_standBy","_multi_standBy"
        };
        private static string[] skillAnimationName = new string[3]
        {
        "_skill0","_skill1","_skill2"
        };

        public bool IsPlayingAnimeBattleComplete { get => CheckIsPlayingAnimeBattle();}

        /// <summary>
        /// 初始化动画控制器
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="dataAsset"></param>
        /// <param name="unitid"></param>
        public void SetOnAwake(SkeletonDataAsset dataAsset, int unitid)
        {
            this.sa = gameObject.GetComponent<SkeletonAnimation>();
            this.unitid = unitid;
            int motiontype = MainManager.Instance.UnitRarityDic[unitid].detailData.motionType;
            string motiontype_str = motiontype < 10 ? "0" + motiontype : "" + motiontype;
            for (int i = 0; i < 9; i++)
            {
                string animationName = motiontype_str + basicAnimationName[i];
                var spineAnimation = dataAsset.GetSkeletonData(false).FindAnimation(animationName);
                spineBasicAnimations.Add(spineAnimation);
                animationNameDic.Add(animationName, (eAnimationType)i);
            }
            for (int i = 0; i < 3; i++)
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
            if ((int)type <= 19)
            {
                sa.AnimationState.SetAnimation(0, spineBasicAnimations[(int)type], isloop);
            }
            else
            {
                sa.AnimationState.SetAnimation(0, spineSkillAnimations[(int)type-21], isloop);
            }
            playSpeed = speed;
            sa.timeScale = timeScale * playSpeed;
            //Debug.Log("ChangeAnimation:" + type.ToString());
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

        public void Resume()
        {
            sa.ClearState();
            sa.timeScale = timeScale;
        }
        public eAnimationType GetCurrentAnimationName()
        {
            string name =  sa.AnimationState.GetCurrent(0).animation.name;
            return animationNameDic[name];
        }
        private bool CheckIsPlayingAnimeBattle()
        {
            return sa.AnimationState.GetCurrent(0).IsComplete;
        }
    }
}