﻿using MacGruber;
using System.Collections.Generic;

namespace mmd2timeline
{
    internal partial class CameraHelper
    {
        /// <summary>
        /// 默认空字符串
        /// </summary>
        private const string noneString = "None";

        /// <summary>
        /// 空字符串列表
        /// </summary>
        internal readonly List<string> noneStrings = new List<string> { noneString };

        /// <summary>
        /// 镜头设置UI
        /// </summary>
        GroupUI _SettingsUI;

        /// <summary>
        /// 镜头选择器UI
        /// </summary>
        GroupUI _ChooserUI;

        /// <summary>
        /// 进度
        /// </summary>
        JSONStorableFloat _progressJSON;

        /// <summary>
        /// 延迟
        /// </summary>
        JSONStorableFloat _timeDelayJSON;

        /// <summary>
        /// 选择器
        /// </summary>
        JSONStorableStringChooser _chooser;

        JSONStorableFloat _PositionOffsetXJSON;

        JSONStorableFloat _PositionOffsetYJSON;

        JSONStorableFloat _PositionOffsetZJSON;

        JSONStorableFloat _RotationOffsetXJSON;

        JSONStorableFloat _RotationOffsetYJSON;

        JSONStorableFloat _RotationOffsetZJSON;

        //JSONStorableFloat _CameraScaleJSON;

        //JSONStorableFloat _CameraYScaleJSON;

        /// <summary>
        /// 初始化CameraUI
        /// </summary>
        internal void CreateSettingsUI(BaseScript self, bool rightSide = false)
        {
            if (_SettingsUI != null)
                return;

            _SettingsUI = new GroupUI(self);

            var toggleJSON = self.SetupToggle("Show Camera Settings", Lang.Get("Show Camera Settings"), false, v =>
            {
                _SettingsUI?.RefreshView(v);
            }, rightSide);
            _SettingsUI.ToggleBool = toggleJSON;

            // 镜头动作播放进度
            _progressJSON = self.SetupSliderFloat("Camera Progress", Lang.Get("Camera Progress"), 0f, 0f, 0f, v =>
            {
                // 更新镜头进度
                //_MmdCamera.SetPlayPos((double)v);
                Refresh();
            }, rightSide);
            _SettingsUI.Elements.Add(_progressJSON);

            _timeDelayJSON = Utils.SetupSliderFloatWithRange(self, Lang.Get("Camera Delay"), 0f, 0f, 0f, rightSide);
            _timeDelayJSON.setCallbackFunction = v =>
            {
                // 更新镜头延迟
                _CameraSetting.TimeDelay = v;
                Refresh();
            };
            _SettingsUI.Elements.Add(_timeDelayJSON);

            // 位置偏移X
            _PositionOffsetXJSON = self.SetupSliderFloat("Position Offset X", 0f, -2f, 2f, v =>
            {
                //  更新位置偏移
                _CameraSetting.PositionOffsetX = v;
                UpdatePositionOffset();
                Refresh();
            }, rightSide, "F4");
            _SettingsUI.Elements.Add(_PositionOffsetXJSON);

            // 位置偏移Y
            _PositionOffsetYJSON = self.SetupSliderFloat("Position Offset Y", 0f, -2f, 2f, v =>
            {
                // 更新位置偏移
                _CameraSetting.PositionOffsetY = v;
                UpdatePositionOffset();
                Refresh();
            }, rightSide, "F4");
            _SettingsUI.Elements.Add(_PositionOffsetYJSON);

            // 位置偏移Z
            _PositionOffsetZJSON = self.SetupSliderFloat("Position Offset Z", 0f, -2f, 2f, v =>
            {
                // 更新位置偏移
                _CameraSetting.PositionOffsetZ = v;
                UpdatePositionOffset();
                Refresh();
            }, rightSide, "F4");
            _SettingsUI.Elements.Add(_PositionOffsetZJSON);

            // 方向偏移X
            _RotationOffsetXJSON = self.SetupSliderFloat("Rotation Offset X", 0f, -180f, 180f, v =>
            {
                // 更新方向偏移
                _CameraSetting.RotationOffsetX = v;
                UpdateRotationOffset();
                Refresh();
            }, rightSide);
            _SettingsUI.Elements.Add(_RotationOffsetXJSON);

            // 方向偏移Y
            _RotationOffsetYJSON = self.SetupSliderFloat("Rotation Offset Y", 0f, -180f, 180f, v =>
            {
                // 更新方向偏移
                _CameraSetting.RotationOffsetY = v;
                UpdateRotationOffset();
                Refresh();
            }, rightSide);
            _SettingsUI.Elements.Add(_RotationOffsetYJSON);

            // 方向偏移Z
            _RotationOffsetZJSON = self.SetupSliderFloat("Rotation Offset Z", 0f, -180f, 180f, v =>
            {
                // 更新方向偏移
                _CameraSetting.RotationOffsetZ = v;
                UpdateRotationOffset();
                Refresh();
            }, rightSide);
            _SettingsUI.Elements.Add(_RotationOffsetZJSON);

            //// 镜头缩放
            //_CameraScaleJSON = self.SetupSliderFloat("Camera Scale", 0f, -0.5f, 0.5f, v =>
            //{
            //    // 更新镜头缩放
            //    _CameraSetting.CameraScale = v;
            //    Refresh();
            //}, rightSide, "F4");
            //_SettingsUI.Elements.Add(_CameraScaleJSON);

            //// 镜头Y轴缩放
            //_CameraYScaleJSON = self.SetupSliderFloat("Y Axis Scale", 1f, 0.1f, 1.5f, v =>
            //{
            //    // 更新镜头缩放
            //    _CameraSetting.CameraYScale = v;
            //    Refresh();
            //}, rightSide, "F4");
            //_SettingsUI.Elements.Add(_CameraYScaleJSON);

            InitSettingValues();
        }

        /// <summary>
        /// 初始化镜头设定值
        /// </summary>
        /// <param name="settings"></param>
        void InitSettingValues()
        {
            if (_SettingsUI == null || _CameraSetting == null)
                return;

            _PositionOffsetXJSON.valNoCallback = _CameraSetting.PositionOffsetX;
            _PositionOffsetYJSON.valNoCallback = _CameraSetting.PositionOffsetY;
            _PositionOffsetZJSON.valNoCallback = _CameraSetting.PositionOffsetZ;
            _RotationOffsetXJSON.valNoCallback = _CameraSetting.RotationOffsetX;
            _RotationOffsetYJSON.valNoCallback = _CameraSetting.RotationOffsetY;
            _RotationOffsetZJSON.valNoCallback = _CameraSetting.RotationOffsetZ;
            //_CameraScaleJSON.valNoCallback = _CameraSetting.CameraScale;
            //_CameraYScaleJSON.valNoCallback = _CameraSetting.CameraYScale;

            UpdatePositionOffset();
            UpdateRotationOffset();
        }

        /// <summary>
        /// 设置延迟数值的范围
        /// </summary>
        /// <param name="length"></param>
        void SetDelayRange(float length)
        {
            if (_timeDelayJSON != null)
            {
                _timeDelayJSON.min = 0 - length;
                _timeDelayJSON.max = length;
            }
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="show"></param>
        internal void ShowSettingUI(bool show)
        {
            _SettingsUI?.Show(show);
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="show"></param>
        internal void ShowChooserUI(bool show)
        {
            _ChooserUI?.Show(show);
        }

        /// <summary>
        /// 创建镜头选择器UI
        /// </summary>
        /// <param name="self"></param>
        /// <param name="rightSide"></param>
        internal void CreateChooserUI(BaseScript self, bool rightSide = false)
        {
            if (_ChooserUI != null)
                return;

            _ChooserUI = new GroupUI(self);

            // 镜头选择
            _chooser = self.SetupStringChooser("Camera Motion", noneStrings, 600f, rightSide);
            _chooser.setCallbackFunction = v =>
            {
                ImportVmd(v);
                SetCameraPath(v);
            };
            self.RegisterStringChooser(_chooser);

            _ChooserUI.Elements.Add(_chooser);
        }

        /// <summary>
        /// 设置镜头路径
        /// </summary>
        /// <param name="path"></param>
        void SetCameraPath(string path)
        {
            if (_CameraSetting == null)
            {
                if (path != noneString)
                {
                    LogUtil.LogWarning($"_CameraSetting:::CameraHelper Is Not Inited.{path}");
                }
                return;
            }

            if (path == noneString)
            {
                path = null;
            }

            //镜头路径选中的处理
            _CameraSetting.CameraPath = path;
        }

        /// <summary>
        /// 重置音频选择器
        /// </summary>
        void ResetChooser()
        {
            SetChooser(noneStrings, noneStrings, noneString);
        }

        /// <summary>
        /// 设置音频选择器
        /// </summary>
        /// <param name="displayChoices"></param>
        /// <param name="choices"></param>
        /// <param name="choice"></param>
        void SetChooser(List<string> displayChoices, List<string> choices, string choice)
        {
            if (_ChooserUI != null)
            {
                _chooser.choices = choices.ToArray().ToList();
                _chooser.displayChoices = displayChoices.ToArray().ToList();

                if (string.IsNullOrEmpty(choice))
                {
                    choice = _chooser.defaultVal;

                    this.IsActive = false;
                }

                _chooser.valNoCallback = choice;
            }

            ImportVmd(choice);
        }
    }
}
