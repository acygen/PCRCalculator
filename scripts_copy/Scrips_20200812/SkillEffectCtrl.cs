using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCRCaculator.Battle;
using PCRCaculator;

namespace Elements
{
    public class SkillEffectCtrl : MonoBehaviour
    {
        //public static readonly eSE[] BattleCommonSeArray;
        public ParticleSystem particle;
        //public TweenPosition[] TweenerList;
        //public TweenRotation[] TweenRotList;
        public Animator[] AnimatorList;
        public bool IsAura;
        //protected CriAtomSource skillSeSource;
        public bool isRepeat;
        public UnitCtrl source;
        public bool isCommon;
        public eBattleSkillSeType seType;
        //private List<RarityUpTextureChangeDataSet> textureChangeParticles;
        //private List<DefenceModeTextureChange> defenceModeTextureSetting;
        public Action<SkillEffectCtrl> OnEffectEnd;
        public UnitCtrl SortTarget;
        public UnitCtrl Target;
        public bool IsPlaying;
        public bool timeToDie;
        public bool isPause;
        //private SoundManager soundManager;
        protected Dictionary<Renderer, int> particleRendererDictionary;
        private bool isFront;
        private ParticleSystem[] particles;
        private Dictionary<ParticleSystem, float> particleStartDelayDictionary;
        public float resumeTime;
        private static readonly string LAYER_NAME;
        private const int SORT_ORDER_BOUNDARY = 0x3e8;
        //private static Yggdrasil<SkillEffectCtrl> staticSingletonTree;
        private static BattleManager staticBattleManager;

        public virtual bool _Update() =>
            new bool();

        protected void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit)
        {
        }

        private void Awake()
        {
        }

        public virtual void ExecAppendCoroutine(UnitCtrl _unit, bool _isAbnormal = false)
        {
        }
        //UnitActionController__createNormalEffectPrefab调用
        public virtual void InitializeSort()
        {
        }

        public void InitializeSortForWithOutBattle()
        {
        }

        //protected virtual void initTweener(UITweener _tweener)
        //{
        //}
        //UnitActionController__createNormalEffectPrefab调用
        public virtual void OnAwakeWhenSkipCutIn()
        {
        }

        protected virtual void onDestroy()
        {
        }

        private void OnDestroy()
        {
        }

        private void particleRendererDictionaryInitialize()
        {
        }

        public virtual void Pause()
        {
        }

        private void pauseSe(bool isPause)
        {
        }

        //public void PlaySe(eSE se, bool isEnemySide)
        //{
        //}

        public void PlaySe(int soundUnitId, bool isEnemySide)
        {
        }

        public virtual void ResetParameter(GameObject _prefab, int _skinID = 0, bool _isShadow = false)
        {
        }

        protected virtual void resetStartDelay()
        {
        }

        public void RestartTween()
        {
        }

        public virtual void Resume()
        {
        }

        private void setLayerName(string value)
        {
        }

        public void SetLayerUI()
        {
        }
        //UnitActionController__createNormalEffectPrefab调用
        public virtual void SetPossitionAppearanceType(NormalSkillEffect skillEffect, BasePartsData target, UnitCtrl _owner, Skill skill)
        {
        }

        public virtual void SetSortOrderBack()
        {
        }

        public void SetSortOrderForSummon(int _sortOrder)
        {
        }

        public virtual void SetSortOrderFront()
        {
        }

        public void SetSortOrderHit(int _offset)
        {
        }

        public void SetSortOrderStatus(int offset)
        {
        }
        //UnitActionController__createNormalEffectPrefab调用
        public virtual void SetStartTime(float _starttime)
        {
        }

        public void SetTimeToDie(bool value)
        {
        }
        //UnitActionController__createNormalEffectPrefab调用

        public virtual void SetTimeToDieByStartHp(float _hpPercent)
        {
        }

        public static void StaticInitSingletonTree()
        {
        }

        public static void StaticRelease()
        {
        }

        //public IEnumerator TrackTarget(BasePartsData trans, Vector3 absolutePosition, bool followX = true, bool followY = true, Bone bone, bool _trackRotation = false) =>
        //    null;

       // public IEnumerator TrackTarget(UnitSpineController _spine, Vector3 _absolutePosition, bool _followX = true, bool _followY = true, Bone _bone, bool _trackRotation = false, Transform _traskScale, float _coefficient = 1f) =>
       //     null;

        public IEnumerator TrackTargetSort(UnitCtrl unit) =>
            null;

        public IEnumerator TrackTargetSortForSummon(UnitCtrl _unitCtrl) =>
            null;

        private IEnumerator UpdateTimer() =>
            null;

        private IEnumerator UpdateTimerRepeat() =>
            null;
    }
}