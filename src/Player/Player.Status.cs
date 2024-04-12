namespace mmd2timeline
{
    internal partial class Player
    {
        /// <summary>
        /// 镜头状态
        /// </summary>
        JSONStorableBool _cameraActiveJSON;

        /// <summary>
        /// 播放状态
        /// </summary>
        JSONStorableBool _playStatusJSON;

        /// <summary>
        /// 脚本加载完毕
        /// </summary>
        JSONStorableBool _scriptLoadedJSON;

        void InitStatusParams()
        {
            _cameraActiveJSON = new JSONStorableBool($"Camera Active Status", false);
            _cameraActiveJSON.setCallbackFunction = v =>
            {
                if (v)
                {
                    _triggerHelper.Trigger(TriggerEventHelper.TRIGGER_CAMERA_ACTIVATED);
                }
                else
                {
                    _triggerHelper.Trigger(TriggerEventHelper.TRIGGER_CAMERA_DEACTIVATED);
                }
            };

            _playStatusJSON = new JSONStorableBool($"Playing Status", false);

            _playStatusJSON.setCallbackFunction = ChangePlayingStatus;

            RegisterBool(_playStatusJSON);

            _scriptLoadedJSON = new JSONStorableBool($"Script Loaded", false);
            _scriptLoadedJSON.setCallbackFunction = (v) =>
            {
                if (v)
                {
                    _triggerHelper.Trigger(TriggerEventHelper.TRIGGER_SCRIPT_LOADED, v);
                }
            };
        }

        void ChangePlayingStatus(bool playing)
        {
            // 返回是否在播放状态
            SetPlayButton(playing);

            if (playing)
            {
                // 有镜头数据时，才会隐藏主UI
                if (_CameraHelper.HasMotion)
                {
                    // 隐藏主HUD
                    SuperController.singleton.HideMainHUD();
                }

                _ProgressHelper.Play();
            }
            else
            {
                _AudioPlayHelper.Stop(1);
                _ProgressHelper.Stop(1);
            }

            _triggerHelper.Trigger(TriggerEventHelper.TRIGGER_PLAYING, playing);
        }
    }
}
