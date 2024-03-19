using System;
using System.Linq;
using UnityEngine;

namespace mmd2timeline
{
    internal partial class Settings : BaseScript
    {
        /// <summary>
        /// 忽略这个基类
        /// </summary>
        /// <returns></returns>
        public override bool ShouldIgnore()
        {
            return false;
        }

        /// <summary>
        /// 首次调用Update之前调用的方法
        /// </summary>
        public virtual void Start()
        {
            try
            {
                InitScript();

                InitSettingUI();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ex, "GeneralSettings::Start:");
            }

            RefreshCameraUI();
        }

        private Atom _WindowCameraAtom;

        /// <summary>
        /// 获取WindowCamera原子
        /// </summary>
        protected Atom WindowCameraAtom
        {
            get
            {
                if (_WindowCameraAtom == null)
                {
                    _WindowCameraAtom = SuperController.singleton.GetAtoms().FirstOrDefault(a => a.type == "WindowCamera");
                }
                return _WindowCameraAtom;
            }
        }

        #region 休眠处理相关的值
        /// <summary>
        /// 休眠前等待的秒数
        /// </summary>
        const float waitSleepSeconds = 10f;
        /// <summary>
        /// 等待休眠的秒数
        /// </summary>
        float waitingSleepSeconds = 0f;
        /// <summary>
        /// 休眠前的音频播放状态
        /// </summary>
        bool audioPlaying = false;
        #endregion

        public void Update()
        {
            #region 处理允许休眠的情况
            if (config.EnableSleepWhenRunInBackground)
            {
                if (Application.isFocused)
                {
                    if (!Application.runInBackground)
                    {
                        Application.runInBackground = true;

                        if (audioPlaying)
                        {
                            AudioPlayHelper.GetInstance().Play();
                        }
                    }
                    waitingSleepSeconds = 0f;
                }
                else
                {
                    if (Application.runInBackground)
                    {
                        waitingSleepSeconds += Time.deltaTime;

                        if (waitingSleepSeconds > waitSleepSeconds)
                        {
                            audioPlaying = AudioPlayHelper.GetInstance().IsPlaying;

                            if (audioPlaying)
                            {
                                AudioPlayHelper.GetInstance().Stop();
                            }

                            Application.runInBackground = false;
                        }
                    }
                }
            }
            #endregion
        }

        public override void OnDestroy()
        {
            SuperController.singleton.onAtomUIDRenameHandlers -= OnAtomUIDChanged;
            SuperController.singleton.onAtomAddedHandlers -= OnAtomChanged;
            SuperController.singleton.onAtomRemovedHandlers -= OnAtomChanged;

            base.OnDestroy();
        }

        public override void OnEnable()
        {
            SuperController.singleton.onAtomUIDRenameHandlers += OnAtomUIDChanged;
            SuperController.singleton.onAtomAddedHandlers += OnAtomChanged;
            SuperController.singleton.onAtomRemovedHandlers += OnAtomChanged;

            base.OnEnable();
        }

        public override void OnDisable()
        {
            SuperController.singleton.onAtomUIDRenameHandlers -= OnAtomUIDChanged;
            SuperController.singleton.onAtomAddedHandlers -= OnAtomChanged;
            SuperController.singleton.onAtomRemovedHandlers -= OnAtomChanged;

            base.OnDisable();
        }
    }
}
