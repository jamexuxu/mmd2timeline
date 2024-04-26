using MVR.FileManagementSecure;
using System.Collections;
using UnityEngine;

namespace mmd2timeline
{
    internal partial class Player
    {
        JSONStorableFloat _currentFovJSON;
        JSONStorableFloat _startProgressJSON;
        JSONStorableFloat _maxProgressJSON;
        JSONStorableString _currentTitleJSON;
        JSONStorableBool _readyToRenderJSON;
        JSONStorableString _currentAudioPathJSON;

        /// <summary>
        /// 初始化Render时需要使用的变量值
        /// </summary>
        void InitForRenderValues()
        {
            _currentFovJSON = new JSONStorableFloat("Current FOV", 40f, 20f, 100f) { isStorable = false, isRestorable = false };
            _maxProgressJSON = new JSONStorableFloat("Max Progress Value", 0f, 0f, 0f, false) { isStorable = false, isRestorable = false };
            _currentTitleJSON = new JSONStorableString("Current Title", "") { isStorable = false, isRestorable = false };
            _startProgressJSON = new JSONStorableFloat("Start Progress Value", 0f, 0f, 0f, false) { isStorable = false, isRestorable = false };
            _readyToRenderJSON = new JSONStorableBool("Ready To Render", false) { isStorable = false, isRestorable = false };
            _currentAudioPathJSON = new JSONStorableString("Current Audio", "") { isStorable = false, isRestorable = false };

            var preparingForRenderingAction = new JSONStorableAction("Preparing For Rendering", PreparingForRendering);

            RegisterAction(new JSONStorableAction("Finish Rendering", () =>
            {
                this.StopPlaying();
                _readyToRenderJSON.val = false;
                config.IsRendering = false;

                _CameraHelper.RestoreEyesControl();
            }));

            RegisterAction(new JSONStorableAction("Refresh Audio Path", RefreshAudioPath));
            RegisterBool(_readyToRenderJSON);
            RegisterFloat(_currentFovJSON);
            RegisterFloat(_maxProgressJSON);
            RegisterFloat(_startProgressJSON);
            RegisterString(_currentTitleJSON);
            RegisterString(_currentAudioPathJSON);
            RegisterAction(preparingForRenderingAction);
        }

        /// <summary>
        /// 刷新音频地址
        /// </summary>
        void RefreshAudioPath()
        {
            _currentAudioPathJSON.val = FileManagerSecure.GetFullPath(_CurrentItem.AudioSetting.AudioPath);
        }

        /// <summary>
        /// 准备渲染
        /// </summary>
        void PreparingForRendering()
        {
            StartCoroutine(ReadyToRendering());
        }

        IEnumerator ReadyToRendering()
        {
            _ProgressHelper.Stop(10);

            // 开启视线同步
            _CameraHelper.SyncEyesControl();

            yield return _MotionHelperGroup.SetPersonOff();

            // 播放前设置一下人物关节参数
            _MotionHelperGroup.SetPersonAllJoints();
            yield return null;//new WaitForSeconds(1);
            _ProgressHelper.Forward(_startProgressJSON.val + 0.001f);
            yield return new WaitForSeconds(1);

            yield return _MotionHelperGroup.SetPersonOn();

            config.IsRendering = true;

            _ProgressHelper.SetProgress(_startProgressJSON.val, true);

            yield return new WaitForSeconds(3);

            _readyToRenderJSON.val = true;
        }

        void OnMaxLengthChanged(float length)
        {
            _maxProgressJSON.val = length;
        }

        void OnFOVChanged(CameraHelper cameraHelper, float fov)
        {
            _currentFovJSON.val = fov;
        }
    }
}
