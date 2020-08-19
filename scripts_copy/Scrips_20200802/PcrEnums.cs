using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
namespace PCRCaculator.Battle
{
    public enum BuffParamKind
    {
        [Description("攻击")]
        ATK = 1,
        [Description("防御")]
        DEF = 2,
        [Description("魔法攻击")]
        MAGIC_STR = 3,
        [Description("魔法防御")]
        MAGIC_DEF = 4,
        [Description("闪避")]
        DODGE = 5,
        [Description("暴击")]
        PHYSICAL_CRITICAL = 6,
        [Description("魔法暴击")]
        MAGIC_CRITICAL = 7,
        [Description("TP回复率")]
        ENERGY_RECOVER_RATE = 8,
        [Description("生命偷取")]
        LIFE_STEAL = 9,
        [Description("移动速度")]
        MOVE_SPEED = 10,
        [Description("暴击伤害")]
        PHYSICAL_CRITICAL_DAMAGE_RATE = 11,
        [Description("魔法暴击伤害")]
        MAGIC_CRITICAL_DAMAGE_RATE = 12,
        [Description("ERROR!")]
        NUM = 13,
        NONE = 13
    }

    public enum DirectionType { FRONT = 1, FRONT_AND_BACK = 2, ALL = 3, UNKNOWN = 0 }
    public enum eAbnormalState
    {
        NONE = 0,
        GUARD_ATK = 1,
        GUARG_MGC = 2,
        DRAIN_ATK = 3,
        DRAIN_MGC = 4,
        GUANG_BOTH = 5,
        DRAIN_BOTH = 6,
        HASTE = 7,
        POISON = 8,
        BURN = 9,
        CURSE = 10,
        SLOW = 11,
        PARALYSIS = 12,
        FREEZE = 13,
        CONVERT = 14,
        DARK = 15,
        SLIENCE = 0x10,
        CHAINED = 0x11,
        SLEEP = 0x12,
        STUN = 0x13,
        DETAIN = 20,
        NO_EFFECT_SLIP_DAMAGE = 21,
        NO_DAMAGE_MOTION = 22,
        NO_ABNORMAL = 23,
        CONTINUOUS_ATTACK_NEARBY = 24,
        ACCUMULATIVE_DAMAGE = 0x19,
        DECOY = 0x1a,
        MIFUYU = 0x1b,
        STONE = 0x1c,
        REGENERATION = 0x1d,
        PHYSICS_DODGE = 30,
        CONFUSION = 0x1f,//31
        VENOM = 0x20,//32
        COUNT_BLIND = 0x21,//33
        INHIBIT_HEAL = 0x22,//34
        FEAR = 0x23,//35
        TP_REGENERATION = 0x24,//36
        HEX = 0x25,//37
        FAINT = 0x26,//38
        PARTS_NO_DAMAGE = 40,
        COMPENSATION = 0x29,
        CUT_ATK_DAMAGE = 0x2a,//52
        CUT_MGC_DAMAGE = 0x2b,//53
        CUT_ALL_DAMAGE = 0x2c,//54
        LOG_ATK_BARRIR = 0x2d,//55
        LOG_MGC_BARRIR = 0x2e,//56
        LOG_ALL_BARRIR = 0x2f,//57
        NUM = 0x30,
        TOP = 1,
        END = 0x2f
    }
    public enum eAbnormalStateCategory
    {

        NONE = 0,

        DAMAGE_RESISTANCE_MGK = 1,

        DAMAGE_RESISTANCE_ATK = 2,

        DAMAGE_RESISTANCE_BOTH = 3,

        POISON = 4,

        BURN = 5,

        CURSE = 6,

        SPEED = 7,

        FREEZE = 8,

        PARALYSIS = 9,

        STUN = 10,

        DETAIN = 11,

        CONVERT = 12,

        SILENCE = 13,

        DARK = 14,

        MIFUYU = 15,

        DECOY = 0x10,

        NO_DAMAGE = 0x11,

        NO_ABNORMAL = 0x12,

        NO_DEBUF = 0x13,

        SLEEP = 20,

        CHAINED = 0x15,

        CONTINUOUS_ATTACK_NEARBY = 0x16,

        ACCUMULATIVE_DAMAGE = 0x17,

        NO_EFFECT_SLIP_DAMAGE = 24,

        STONE = 0x19,

        REGENERATION = 0x1a,

        PHYSICS_DODGE = 0x1b,

        CONFUSION = 0x1c,

        VENOM = 0x1d,

        COUNT_BLIND = 30,

        INHIBIT_HEAL = 0x1f,//31

        FEAR = 0x20,

        TP_REGENERATION = 0x21,

        HEX = 0x22,

        FAINT = 0x23,

        PARTS_NO_DAMAGE = 0x24,

        COMPENSATION = 0x25,

        CUT_ATK_DAMAGE = 0x26,

        CUT_MGC_DAMAGE = 0x27,

        CUT_ALL_DAMAGE = 40,

        LOG_ATK_BARRIR = 0x29,

        LOG_MGC_BARRIR = 0x2a,

        LOG_ALL_BARRIR = 0x2b,

        NUM = 0x2c,

        TOP = 0,

        END = 0x2b
    }
    public enum eAccumulativeDamageType { FIXED = 1, PERCENTAGE = 2 }
    public enum eActionState
    {
        IDLE,
        ATK,
        SKILL1,
        SKILL,
        WALK,
        DAMAGE,
        DIE,
        GAMESTART
    }
    public enum eActionType
    {
        [Description("攻击")]
        ATTACK = 1, 
        [Description("移动")]
        MOVE = 2, 
        KNOCK = 3, 
        HEAL = 4, 
        CURE = 5,//忽略 
        BARRIER = 6, 
        REFLEXIVE = 7, 
        CHANGE_SPEED = 8, 
        SLIP_DAMAGE = 9,//10
        [Description("BUFF/DEBUFF")]
        BUFF_DEBUFF = 10, 
        CHARM = 11, 
        BLIND = 12, 
        SILENCE = 13, //忽略
        MODE_CHANGE = 14, //我方只有muimi有，暂时忽略
        SUMMON = 15, 
        CHARGE_ENERGY = 16, 
        TRIGER = 17, //只有中二有
        DAMAGE_CHANGE = 18, //怜和宫子有
        CHARGE = 19, //只有一个怪有，忽略
        DECOY = 20,//20
        NO_DAMAGE = 21, 
        CHANGE_PATTERN = 22, //忽略，只有怪物有
        IF_FOR_CHILDREN = 23, 
        REVIVAL = 24, //忽略，只有怪有
        CONTINUOUS_ATTACK = 25,//忽略，没有角色有这个技能
        GIVE_VALUE_AS_ADDITIVE = 26, 
        GIVE_VALUE_AS_MULTIPLE = 27,//27
        IF_FOR_AL = 28, 
        SEARCH_AREA_CHANGE = 29, //忽略，没有角色有这个技能
        DESTROY = 30, //只有中二有的即死
        CONTINUOUS_ATTACK_NEARBY = 31, //忽略，没有角色有这个技能
        ENCHANT_LIFE_STEAL = 32, //只有妹法大招有这个技能
        ENCHANT_STRIKE_BACK = 33,//33
        ACCUMULATIVE_DAMAGE = 34, //只有狗拳有
        SEAL = 35, 
        ATTACK_FIELD = 36, 
        HEAL_FIELD = 37, 
        CHANGE_PARAMETER_FIELD = 38, 
        ABNORMAL_STATE_FIELD = 39,//忽略，只有两个怪有这个 
        KETSUBAN = 40,//忽略，没有角色有这个技能
        UB_CHANGE_TIME = 41, //忽略，没有角色有这个技能
        LOOP_TRIGGER = 42, 
        IF_HAS_TARGET = 43, //忽略，没有角色有这个技能
        WAVE_START_IDLE = 44, //只有羊驼有
        SKILL_EXEC_COUNT = 45, 
        RATIO_DAMAGE = 46, //只有圣诞伊利亚有
        UPPER_LIMIT_ATTACK = 47,//忽略
        REGENERATION = 48, 
        BUFF_DEBUFF_CLEAR = 49, //忽略
        LOOP_MOTION_BUFF_DEBUFF = 50, //只有クルミ（クリスマス）有
        DIVISION = 51, //忽略，没有角色有这个技能
        CHANGE_BODY_WIDTH = 52, //忽略
        IF_EXISTS_FIELD_FOR_ALL = 53,//シズル（バレンタイン）专属
        STEALT = 54, //忽略
        MOVE_PARTS = 55, //忽略
        COUNT_BLIND = 56, //ラム专属
        COUNT_DOWN = 57, //ミソギ（ハロウィン）和一个怪有
        STOP_FIELD = 58, //忽略
        INHIBIT_HEAL = 59, //忽略
        ATTACK_SEAL = 60,
        FEAR = 61,//ミヤコ（ハロウィン）和黑猫有
        AWE = 62, //忽略
        LOOP_MOTION_REPEAT = 63, //忽略
        TOAD = 69, //忽略，这个属性就一个怪有，判定代码却到处都有，贼烦
        FORCE_HP_CHANGE = 70,//忽略，没有角色有这个技能
        KNGHT_GUARD = 71,//ペコリーヌ（プリンセス）专属 
        DAMAGE_CUT = 72, //忽略，没有角色有这个技能
        LOG_BARRIER = 73, //忽略，工会战boss专属的对数盾
        GIVE_VALUE_AS_DIVIDE = 74,//忽略，没有角色有这个技能
        [Description("被动技能")]
        PASSIVE = 90,//被动
        PASSIVE_INTERMITTENT = 91,//忽略，没有角色有这个技能
        UNKNOWN = 92//台服代码还没有的未知技能，有两个工会战boss有
    }
    public enum eAttacktype { PHYSICAL = 1, MAGIC = 2, INEVITABLE_PHYSICAL = 3 }
    public enum eDamageEffectType { NORMAL = 0, COMBO = 1, LARGE = 2 }
    public enum eDamageType { ATK = 1, MGC = 2, NONE = 3 }
    public enum eEffectType { COMMON = 0, NONE = 1 }
    public enum eInhibitHealType
    {
        PHYSICS = 0,
        MAGIC = 1,
        NO_EFFECT = 2
    }

    public enum eMissLogType
    {
        DODGE_ATTACK = 0,
        DODGE_ATTACK_DARK = 1,
        DODGE_CHANGE_SPEED = 2,
        DODGE_CHARM = 3,
        DODGE_DARK = 4,
        DODGE_SLIP_DAMAGE = 5,
        DODGE_SILENCE = 6,
        DODGE_BY_NO_DAMAGE_MOTION = 7,
        MISS_HP0_RECOVER = 8,
        DODGE_KNOCK = 9,
        INVALID_VALUE = -1
    }

    public enum eSetEnergyType
    {
        [Description("剧情添加")]
        BY_STORY_TIME_LINE,
        [Description("行动前恢复")]
        BY_ATK,
        [Description("技能")]
        BY_CHARGE_SKILL,
        [Description("战斗模式改变")]
        BY_MODE_CHANGE,
        [Description("初始化")]
        INITIALIZE,
        [Description("受伤")]
        BY_SET_DAMAGE,
        [Description("使用UB")]
        BY_USE_SKILL,
        [Description("战斗结束回复")]
        BATTLE_RECOVERY,
        [Description("击杀单位")]
        KILL_BONUS,
        [Description("调用改变能量函数")]
        BY_CHANGE_ENERGY
    }
    public enum eSkillMotionType { DEFAULT = 0, AWAKE = 1, ATTACK = 2, NONE = 3, EVOLUTION = 4 }
    public enum eStateIconType
    {
        NONE = 0, BUFF_PHYSICAL_ATK = 1, BUFF_PHYSICAL_DEF = 2, BUFF_MAGIC_ATK = 3, BUFF_MAGIC_DEF = 4,
        BUFF_DODGE = 5, BUFF_CRITICAL = 6, BUFF_ENERGY_RECOVERY = 7, BUFF_HP_RECOVERY = 8,
        HASTE = 9, NO_DAMAGE = 10, BUFF_LIFE_STEAL = 11, BUFF_ADD_LIFE_STEAL = 12,
        DEBUFF_PHYSICAL_ATK = 13, DEBUFF_PHYSICAL_DEF = 14, DEBUFF_MAGIC_ATK = 15, DEBUFF_MAGIC_DEF = 16,
        DEBUFF_DODGE = 17, DEBUFF_CRITICAL = 18, DEBUFF_ENERGY_RECOVERY = 19, DEBUFF_HP_RECOVERY = 20,
        DEBUFF_LIFE_STEAL = 21, SLOW = 22, UB_DISABLE = 23, PHYSICS_BARRIAR = 24, MAGIC_BARRIAR = 25,
        PHYSICAS_DRAIN_BARRIAR = 26, MAGIC_DRAIN_BARRIAR = 27, BOTH_BARRIAR = 28, BOTH_DRAIN_BARRIAR = 29,
        DEBUF_BARRIAR = 30, STRIKE_BACK = 31, PARALISYS = 32, SLIP_DAMAGE = 33, DARK = 34, SILENCE = 35,
        CONVER = 36, DECOY = 37, BURN = 38, CURSE = 39, FREEZE = 40, CHAINED = 41, SLEEP = 42, STUN = 43,
        STONE = 44, DETAIN = 45, REGENERATION = 46, DEBUFF_MOVE_SPEED = 47, PHYSICS_DODGE = 48,
        CONFUSION = 49, HEROIC_SPIRIT_SEAL = 50, VENOM = 51, COUNT_BLIND = 52, INHIBIT_HEAL = 53,
        FEAR = 54, ERROR = 55, SOUL_EAT = 56, KUROE = 57, FIRE_NUTS = 58, AWE = 59, LUNA = 60,
        CHRISTINA = 61, TP_REGENERATION = 62, SAGITTARIUS = 63, TONAKAI = 64, HEX = 65, FAINT = 66,
        BUFF_PHYSICAL_CRITICAL_DAMAGE = 67, DEBUFF_PHYSICAL_CRITICAL_DAMAGE = 68,
        BUFF_MAGIC_CRITICAL_DAMAGE = 69, DEBUFF_MAGIC_CRITICAL_DAMAGE = 70,
        COMPENSATION = 71, KNIGHT_GUARD = 72, CUT_ATK_DAMAGE = 73, CUT_MGC_DAMAGE = 74,
        CUT_ALL_DAMAGE = 75, CHIERU = 76, REI = 77, LOG_ATK_BARRIER = 78, LOG_MGC_BARRIER = 79,
        LOG_ALL_BARRIER = 80, NUM = 81, INVALID_VALUE = -1
    }
    public enum eSummonType { NONE = 0, SUMMON = 1, PHANTOM = 2, DIVISION = 1001 }
    public enum eTargetAssignment { OTHER_SIDE = 1, OWNER_SIDE = 2, ALL = 3 }
    public enum eWeaponSeType
    {
        DEAULT = 0, KNUCKLE = 1, SHORTSWORD = 2, AX = 3, SWORD = 4, LONGSWORD = 5, SPEAR = 6, WAND = 7, ARROW = 8, DAGGER = 9,
        LONGSWORD_2 = 10, SCRATCH_1 = 11, SCRATCH_2 = 12, HAMMER = 13, BITE = 14, COMMON_SMALL = 15,
        COMMON_LARGE = 16, EXPLOSION = 17, WAND_KIMONO = 18, SWORD_KIMONO = 19, NO_WAND_WITCH = 20,
        WAND_2 = 21, NO_WANO_WITCH_2 = 22
    }
    public enum eWeaponMotionType
    {
        DEFAULT = 0, KNUCKLE = 1, SHORTSWORD = 2, AX = 3, SWORD = 4, LONGSWORD = 5, SPEAR = 6,
        WAND = 7, ARROW = 8, DAGGER = 9, LONGSWORD_2 = 10, WAND_KIMONO = 21, SWOAD_KIMONO = 22,
        NO_WAND_WITCH = 23, WAND_2 = 25, NO_WAND_WITCH_2 = 26
    }
    public enum PirorityPattern
    {
        NONE = 1, RANDOM = 2, NEAR = 3, FAR = 4, HP_ASC = 5, HP_DEC = 6, OWNER = 7, RANDOM_ONCE = 8, FORWARD = 9, BACK = 10,
        ABSOLUTE_POSITION = 11, ENERGY_DEC = 12, ENERGY_ASC = 13, ATK_DEC = 14, ATK_ASC = 15, MAGIC_STR_DEC = 16,
        MAGIC_STR_ASC = 17, SUMMON = 18, ENERGY_REDUCING = 19, ATK_PHYSICS = 20, ATK_MAGIC = 21,
        ALL_SUMMON_RANDOM = 22, OWN_SUMMON_RANDOM = 23, BOSS = 24, HP_ASC_NEAR = 25, HP_DEC_NEAR = 26,
        ENERGY_DEC_NEAR = 27, ENERGY_ASC_NEAR = 28, ATK_DEC_NEAR = 29, ATK_ASC_NEAR = 30,
        MAGIC_STR_DEC_NEAR = 31, MAGIC_STR_ASC_NEAR = 32, SHADOW = 33
    }
    public static class BattleDefine
    {
        public static float MAX_ENERGY = 1000;
        public static Dictionary<eWeaponMotionType, float> WEAPON_HIT_DELAY_DIC;
        static BattleDefine()
        {
            WEAPON_HIT_DELAY_DIC = new Dictionary<eWeaponMotionType, float>
            {
                { (eWeaponMotionType)8, 0 },
                { (eWeaponMotionType)3, 0.45f },
                { (eWeaponMotionType)1, 0.4f },
                { (eWeaponMotionType)5, 0.47f },
                { (eWeaponMotionType)2, 0.4f },
                { (eWeaponMotionType)6, 0.35f },
                { (eWeaponMotionType)4, 0.35f },
                { (eWeaponMotionType)7, 0 },
                { (eWeaponMotionType)9, 0 },
                { (eWeaponMotionType)10, 0.6f },
                { (eWeaponMotionType)22, 0.4f },
                { (eWeaponMotionType)21, 0 },
                { (eWeaponMotionType)23, 0 },
                { (eWeaponMotionType)25, 0 },
                { (eWeaponMotionType)26, 0 }
            };


        }
        public static ActionParameter GetActionParameterByActionType(eActionType actionType,out bool isSuccess)
        {
            ActionParameter action = new ActionParameter();
            isSuccess = true;
            switch (actionType)
            {
                case eActionType.ATTACK:
                    action = new AttackAction();
                    break;
                case eActionType.MOVE:
                    action = new MoveAction();
                    break;
                case eActionType.WAVE_START_IDLE:
                    action = new WaveStartIdleAction();
                    break;
                case eActionType.BUFF_DEBUFF:
                    action = new BuffDebuffAction();
                    break;
                default:
                    isSuccess = false;
                    break;
            }
            return action;
        }
    }
    public static class BattleUtil
    {
        public static int FloatToIntReverseTruncate(float num)
        {
            if(Mathf.Abs((int)num - num) <= 0.001)
            {
                return (int)num;
            }
            if (num < 0)
            {
                return Mathf.FloorToInt(num);
            }
            else
            {
                return Mathf.CeilToInt(num);
            }
        }
    }
}
