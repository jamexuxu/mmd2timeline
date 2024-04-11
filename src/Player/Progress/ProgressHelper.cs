﻿using mmd2timeline.Store;
using System;
using UnityEngine;

namespace mmd2timeline
{
    /// <summary>
    /// 播放进度管理器
    /// </summary>
    internal partial class ProgressHelper
    {
        //public int SyncMode = ProgressSyncMode.SyncWithAudio;

        /// <summary>
        /// 播放状态改变的回调委托
        /// </summary>
        /// <param name="progress">返回音频长度</param>
        /// <param name="playing">是否在播放中</param>
        public delegate void PlayStatusCallback(float progress, bool playing);

        /// <summary>
        /// 播放状态变化的事件
        /// </summary>
        public event PlayStatusCallback OnPlayStatusChanged;

        /// <summary>
        /// 进度更改的回调委托
        /// </summary>
        /// <param name="length">返回长度</param>
        public delegate void ProgressChangedCallback(float length, bool hardUpdate);

        /// <summary>
        /// 进度更改的事件
        /// </summary>
        public event ProgressChangedCallback OnProgressChanged;

        /// <summary>
        /// 最大长度更改的回调函数
        /// </summary>
        /// <param name="length"></param>
        public delegate void MaxLengthChangedCallback(float length);
        /// <summary>
        /// 最大长度更改事件
        /// </summary>
        public event MaxLengthChangedCallback OnMaxLengthChanged;

        /// <summary>
        /// 进度结束的回调委托
        /// </summary>
        /// <param name="isEnd"></param>
        public delegate void ProgressEndedCallback(bool isEnd);

        /// <summary>
        /// 进度条结束的事件
        /// </summary>
        public event ProgressEndedCallback OnProgressEnded;

        /// <summary>
        /// 当前进度
        /// </summary>
        float _progress = 0f;

        /// <summary>
        /// 获取当前进度
        /// </summary>
        internal float Progress
        {
            get { return _progress; }
        }

        /// <summary>
        /// 此变量存储当前MMD配置的播放长度，如果为0则为未设置
        /// </summary>
        /// <remarks>此数值只用于判断当更新最大时间时是否更新播放长度，如果此数值大于0则只会遵从设定值。</remarks>
        float _length = 0f;

        /// <summary>
        /// 最大时间
        /// </summary>
        float _maxTime = 0f;

        /// <summary>
        /// 获取或设置最大时间
        /// </summary>
        internal float MaxTime
        {
            get
            {
                return _maxTime;
            }
            set
            {
                // 只有当设定值比当前值大时才会更新最大值数据
                if (_maxTime < value)
                {
                    _maxTime = value;

                    // 更新最大长度设置条的最大值为设定值
                    _maxLengthJSON.max = value;
                    _maxLengthJSON.min = 0f;

                    // 更新进度条的最大值
                    if (_progressJSON.max <= 0f)
                    {
                        _progressJSON.max = value;
                    }

                    // 如果 如果原设定值为0，则修改设定值
                    if (_length <= 0f)
                    {
                        SetMaxLength(value);
                    }
                }
            }
        }

        /// <summary>
        /// MMD设置数据
        /// </summary>
        MMDSetting _mmdSetting;
        ///// <summary>
        ///// 速度值
        ///// </summary>
        //float _speed = 1f;
        ///// <summary>
        ///// 获取当前播放速度
        ///// </summary>
        //public float Speed { get { return _speed; } }

        /// <summary>
        /// 获取是否已经播放到末尾
        /// </summary>
        internal bool IsEnd
        {
            get
            {
                return CheckEnd(_progress);
            }
        }

        /// <summary>
        /// 检查指定进度是否为结束
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        internal bool CheckEnd(float progress)
        {
            if (_mmdSetting != null && _mmdSetting.MaxLength > 0f)
            {
                return progress > _mmdSetting.MaxLength;
            }
            else
            {
                return progress > _maxTime;
            }
        }

        bool _isPlaying = false;

        /// <summary>
        /// 获取是否在播放状态
        /// </summary>
        internal bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        private ProgressHelper() { }

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            _isPlaying = true;

            OnPlayStatusChanged?.Invoke(_progress, _isPlaying);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop(int test)
        {
            _isPlaying = false;

            OnPlayStatusChanged?.Invoke(_progress, _isPlaying);
        }

        /// <summary>
        /// 是否播放被冻结
        /// </summary>
        bool _PlayingFreezed = false;

        /// <summary>
        /// 获取播放是否被冻结
        /// </summary>
        internal bool IsFreezed
        {
            get
            {
                return _PlayingFreezed;
            }
        }

        /// <summary>
        /// 播放冻结
        /// </summary>
        public void Freeze()
        {
            // 只有在播放状态才会冻结
            if (_isPlaying)
            {
                _PlayingFreezed = true;
                Stop(1);
            }
        }

        /// <summary>
        /// 冻结恢复
        /// </summary>
        public void FreezeRestore()
        {
            if (_PlayingFreezed)
            {
                _PlayingFreezed = false;
                Play();
            }
        }

        ///// <summary>
        ///// 设置播放速度
        ///// </summary>
        ///// <param name="speed"></param>
        //public void SetPlaySpeed(float speed)
        //{
        //    _speed = speed;

        //    //Time.timeScale = speed;

        //    //_AudioSource.pitch = speed;

        //    //if (speed == 1f)
        //    //{
        //    //    _AudioSource.velocityUpdateMode = AudioVelocityUpdateMode.Auto;
        //    //}
        //    //else
        //    //{
        //    //    _AudioSource.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
        //    //}
        //}

        /// <summary>
        /// 更新进度数据
        /// </summary>
        internal void Update(float speed)
        {
            var deltaTime = Time.deltaTime * speed;

            var newProgress = _progress + deltaTime;

            SetProgress(newProgress, false);
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        /// <param name="setting"></param>
        internal void InitSettings(MMDSetting setting)
        {
            _mmdSetting = setting;

            // 重置进度条
            ResetProgress();

            // 记录设定的长度数值
            _length = setting.MaxLength;

            if (_length > 0f)
            {
                // 将设定长度赋予最大时间
                MaxTime = _length;

                // 设置播放长度
                SetMaxLength(_length);
            }
        }

        /// <summary>
        /// 清理播放数据
        /// </summary>
        internal void Clear()
        {
            _mmdSetting = null;
            _length = 0f;

            ResetProgress();
        }

        /// <summary>
        /// 重置进度条
        /// </summary>
        private void ResetProgress()
        {
            // 重置进度条数据
            _progressJSON.valNoCallback = 0f;
            _progressJSON.max = 0f;
            _progressJSON.min = 0f;

            // 重置最大长度设定数据
            _maxLengthJSON.valNoCallback = 0f;
            _maxLengthJSON.max = 0f;
            _maxLengthJSON.min = 0f;

            // 重置进度和最大时间参数值
            _progress = 0f;
            _maxTime = 0f;

            // 重置速度
            //_speed = 1f;

            OnMaxLengthChanged?.Invoke(0);
        }

        /// <summary>
        /// 设置播放长度
        /// </summary>
        /// <param name="length"></param>
        void SetMaxLength(float length)
        {
            _mmdSetting.MaxLength = length;
            _maxLengthJSON.valNoCallback = length;
            _maxLengthJSON.SetDefaultFromCurrent();
            _progressJSON.max = length;

            OnMaxLengthChanged?.Invoke(length);
        }

        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="progress"></param>
        internal void SetProgress(float progress, bool hardUpdate = true)
        {
            try
            {
                // 如果已经到结尾结束播放
                if (CheckEnd(progress))
                {
                    //_isPlaying = false;
                    OnProgressEnded?.Invoke(true);
                }
                else
                {
                    // 强制更新时不进行精度调整和帧数限制
                    if (!hardUpdate)
                    {
                        // 调整精度
                        //progress = (float)Math.Ceiling(progress * 10000f) / 10000f;
                        // 限制最大帧数，刷新太快可能造成镜头抖动
                        //if (Mathf.Abs(_progress - progress) < (1f / 120f))
                        //    return;
                    }
                    // 更新进度变量
                    _progress = progress;

                    // 更新进度条进度（不触发UI控件的值更改事件）
                    _progressJSON.valNoCallback = _progress;

                    // 触发器
                    //_progressFloatTrigger.Trigger($"{_progress}");

                    // 抛出进度更改事件
                    OnProgressChanged?.Invoke(_progress, hardUpdate);
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError(e, $"SetProgress::");
            }
        }

        /// <summary>
        /// 前进指定的秒数
        /// </summary>
        /// <param name="s"></param>
        internal void Forward(float s = 1f)
        {
            _progress += s;

            SetProgress(_progress);
        }

        /// <summary>
        /// 后退指定的秒数
        /// </summary>
        /// <param name="s"></param>
        internal void Back(float s = 1f)
        {
            _progress -= s;

            SetProgress(_progress);
        }

        #region 单例
        private static ProgressHelper _instance;
        private static object _lock = new object();

        /// <summary>
        /// 镜头控制器的单例
        /// </summary>
        public static ProgressHelper GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ProgressHelper();
                }

                return _instance;
            }
        }
        #endregion


        /// <summary>
        /// 销毁时执行的函数
        /// </summary>
        public void OnDestroy()
        {
            Stop(0);

            Clear();

            _instance = null;
        }

        /// <summary>
        /// 禁用时执行的函数
        /// </summary>
        public void OnDisable()
        {
            Freeze();
        }

        /// <summary>
        /// 启用时执行的函数
        /// </summary>
        public void OnEnable()
        {
            FreezeRestore();
        }
    }
}
