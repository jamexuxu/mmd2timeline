﻿using MacGruber;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mmd2timeline
{
    /// <summary>
    /// 镜头动作管理器 - 聚焦部分功能
    /// </summary>
    internal partial class CameraHelper
    {
        #region 变量
        /// <summary>
        /// 是否聚焦在原子上
        /// </summary>
        bool _FocusOnAtom = false;

        /// <summary>
        /// 聚焦的原子
        /// </summary>
        Atom _FocusAtom;

        /// <summary>
        /// 聚焦设置UI
        /// </summary>
        GroupUI _FocusUI;

        /// <summary>
        /// 指示是否聚焦到原子
        /// </summary>
        JSONStorableBool _FocusOnAtomJSON;
        /// <summary>
        /// 聚焦原子选择器
        /// </summary>
        JSONStorableStringChooser _FocusAtomJSON;
        /// <summary>
        /// 聚焦目标选择器
        /// </summary>
        JSONStorableStringChooser _FocusReceiverJSON;
        #endregion

        /// <summary>
        /// 初始化CameraUI
        /// </summary>
        internal void CreateFocusUI(BaseScript self, bool rightSide = false)
        {
            if (_FocusUI != null)
            {
                LogUtil.LogWarning($"Camera Focus UI is Created Already.");
                return;
            }

            _FocusUI = new GroupUI(self);

            _FocusOnAtomJSON = self.SetupToggle("Focus On ...", _FocusOnAtom, v =>
            {
                _FocusOnAtom = v;
                _FocusUI.RefreshView(v);
            }, rightSide);
            _FocusUI.ToggleBool = _FocusOnAtomJSON;

            _FocusAtomJSON = self.SetupStringChooser("Target Atom", noneStrings, rightSide: rightSide);
            _FocusAtomJSON.setCallbackFunction = SyncAtomChoices;
            _FocusUI.Elements.Add(_FocusAtomJSON);

            _FocusReceiverJSON = self.SetupStringChooser("Target Receiver", noneStrings, rightSide: rightSide);
            _FocusUI.Elements.Add(_FocusReceiverJSON);

            // 生成聚焦原子列表
            MakeFocusAtomList();

            // 刷新UI元素
            _FocusUI.RefreshView();
        }

        /// <summary>
        /// 显示/隐藏聚焦UI
        /// </summary>
        /// <param name="show"></param>
        internal void ShowFocusUI(bool show)
        {
            _FocusUI?.Show(show);
        }

        /// <summary>
        /// 刷新聚焦原子列表
        /// </summary>
        internal void RefreshFocusAtomList()
        {
            MakeFocusAtomList();
        }

        /// <summary>
        /// 设置对焦目标
        /// </summary>
        /// <param name="atomUID"></param>
        /// <param name="target"></param>
        internal void SetFocusTarget(string atomUID, string target)
        {
            _FocusAtomJSON.val = atomUID;
            _FocusReceiverJSON.val = target;
        }

        /// <summary>
        /// 生成聚焦原子列表
        /// </summary>
        void MakeFocusAtomList()
        {
            if (_FocusUI == null)
            {
                LogUtil.LogWarning($"Camera Focus UI is not Created.");
                return;
            }

            List<string> targetChoices = new List<string>() { noneString };
            foreach (string atomUID in SuperController.singleton.GetAtomUIDs())
            {
                var targetAtom = SuperController.singleton.GetAtomByUid(atomUID);
                if (targetAtom.hidden == false && targetAtom.GetBoolJSONParam("on").val == true && targetAtom.name != "CoreControl")
                {
                    targetChoices.Add(atomUID);
                }
            }

            _FocusAtomJSON.choices = targetChoices;
            _FocusAtomJSON.displayChoices = targetChoices;
            // 选中默认的元素
            //FocusAtomJSON.val = "";
        }

        /// <summary>
        /// 同步原子选项
        /// </summary>
        /// <param name="atomUID"></param>
        void SyncAtomChoices(string atomUID)
        {
            List<string> receiverChoices = new List<string>() { noneString };
            if (atomUID != null && atomUID != noneString)
            {
                _FocusAtom = SuperController.singleton.GetAtomByUid(atomUID);
                if (_FocusAtom != null)
                {
                    FreeControllerV3[] controls = _FocusAtom.freeControllers;
                    foreach (var control in controls)
                    {
                        receiverChoices.Add(control.name);
                    }
                }
            }
            else
            {
                _FocusAtom = null;
            }
            _FocusReceiverJSON.choices = receiverChoices;
            _FocusReceiverJSON.displayChoices = receiverChoices;
            _FocusReceiverJSON.val = noneString;
        }

        /// <summary>
        /// 聚焦
        /// </summary>
        Quaternion FocusOn(Vector3 position, Vector3 up, string tagetId = null)
        {
            if (!_FocusOnAtom)
            {
                return Quaternion.identity;
            }

            if (_FocusAtom == null)
            {
                _FocusAtom = SuperController.singleton.GetAtomByUid(_FocusAtomJSON.val);
            }

            if (_FocusAtom != null)
            {
                if (string.IsNullOrEmpty(tagetId))
                {
                    tagetId = _FocusReceiverJSON.val;
                }

                var target = _FocusAtom.freeControllers.FirstOrDefault(c => c.name == tagetId);//.GetStorableByID(FocusReceiverJSON.val).transform.position;

                //// 如果是VR模式，则主动计算镜头方向（因为FocusOnController方法在VR模式有向右偏移35度）
                //if (config.IsInVR)
                //{
                var focusRotation = GetFocusOnRotation(position, up, target);
                var rotation = GetRotation(position, focusRotation, NavigationRig);
                //    NavigationRig.rotation = rotation;
                //}
                //else
                //{
                //    if (config.UseWindowCamera)
                //    {
                //        _CameraTransform.LookAt(target.transform);
                //    }
                //    else
                //    {
                //        SuperController.singleton.FocusOnController(target);
                //    }
                //}

                return rotation;
            }

            return Quaternion.identity;
        }

        Vector3 targetPastPosition = Vector3.zero;
        /// <summary>
        /// 获取聚焦角度
        /// </summary>
        /// <returns></returns>
        Quaternion GetFocusOnRotation(Vector3 position, Vector3 up, FreeControllerV3 target)
        {
            if (_FocusAtom != null)
            {
                var targetAtomPosition = target.transform.position;

                var currentAtomPosition = position;
                Vector3 targetPosition = Vector3.zero;

                Vector3 targetDiff = targetAtomPosition - targetPastPosition;
                if (Mathf.Abs(targetDiff.x) >= 0f)
                {
                    targetPosition.x = targetPastPosition.x + targetDiff.x;
                }
                if (Mathf.Abs(targetDiff.y) >= 0f)
                {
                    targetPosition.y = targetPastPosition.y + targetDiff.y;
                }
                if (Mathf.Abs(targetDiff.z) >= 0f)
                {
                    targetPosition.z = targetPastPosition.z + targetDiff.z;
                }

                targetPastPosition = targetPosition;

                var aim = (targetPosition) - (currentAtomPosition);

                return Quaternion.LookRotation(aim, up);
            }
            else
            {
                return Quaternion.identity;
            }
        }
    }
}
