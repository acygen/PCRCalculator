using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCRCaculator.Battle
{
    public enum eBattleSkillSeType
    {
        NAME = 0,
        ARROW = 1,
        INVALID_VALUE = -1
    }
    public enum ePauseType
    {
        VISUAL = 0,
        SYSTEM = 1,
        IGNORE_BLACK_OUT = 2,
        NO_DIALOG = 3
    }
    public enum eMoveTypes
    {
        LINEAR = 0,
        NONE = 1,
        PARABORIC = 2,
        PARABORIC_ROTATE = 3,
        HORIZONTAL = 4,
        INVALID_VALUE = -1
    }
}