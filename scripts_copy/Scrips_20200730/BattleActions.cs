using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCRCaculator.Battle
{
    public class MoveAction : ActionParameter
    {
        private enum eMoveType
        {
            TARGET_POS_RETURN = 1,
            ABSOLUTE_POS_RETURN = 2,
            TARGET_POS = 3,
            ABSOLUTE_POS = 4,
            TARGET_MOVE_BY_VELOSITY = 5,
            ABSOLUTE_MOVE_BY_VELOCITY = 6,
            ABSOLUTE_MOVE_DONOT_USE_DIRECTION = 7
        }

        private const int MOVE_BY_VELOCITY_LOOP_MOTION_SUFFIX = 1;
        private const int MOVE_BY_VELOCITY_LOOP_END_MOTION_SUFFIX = 2;
        private const int MOVE_BY_VELOCITY_RETURN_LOOP_MOTION_SUFFIX = 3;
        private const int MOVE_BY_VELOCITY_RETURN_LOOP_MOTION_END_SUFFIX = 4;
        private const float MOVE_POSITION_Y = 1f;
        private ActionParameter endAction;

        public ActionParameter EndAction { get => endAction; set => endAction = value; }

        private IEnumerator absoluteMoveByVerocity(float _distance, float _speed, UnitActionController _sourceUnitActionController, UnitCtrl _source, Skill _skill) =>
            null;

        private static Vector3 calculatePosotion(UnitCtrl _source, BasePartsData _target, UnitActionController _sourceActionController, Dictionary<int, float> _valueDictionary) =>
            new Vector3();

        public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<int, float> _valueDictionary)
        {
        }

        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
        }

        private IEnumerator moveByVerocityEnd(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill, float _velocity) =>
            null;

        private IEnumerator moveByVerocityReturn(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill, float _velocity) =>
            null;

        private IEnumerator moveType4ReturnEnd(UnitCtrl _source, Skill _skill) =>
            null;

        private IEnumerator resetPositionY(Skill _skill, UnitCtrl _source) =>
            null;

        private IEnumerator targetMoveByVerocity(float _main, float _sub, BasePartsData _target, UnitActionController _sourceUnitActionController, UnitCtrl _source, Skill _skill) =>
            null;

    } 

}
