﻿using Leap.Unity.Infix;
using MeshVR;
using System;
using UnityEngine;

namespace mmd2timeline
{
    /// <summary>
    /// 镜头动作控制，跟随。
    /// 参考了Passenger的部分实现
    /// </summary>
    internal partial class CameraHelper
    {
        #region 初始参数设定
        /// <summary>
        /// 启用
        /// </summary>
        private bool _isActive = false;
        private bool _rotationLock = true;
        private bool _positionLock = true;

        //Quaternion _RotationOffset = Quaternion.Euler(0f, 0f, 0f);
        //Vector3 _PositionOffset = Vector3.zero;

        Atom _customCameraAtom;

        internal void SetCustomCameraAtom(Atom atom) { _customCameraAtom = atom; }

        Atom _positionReferenceAtom;

        internal void SetPositionReferenceAtom(Atom atom) { _positionReferenceAtom = atom; }

        /// <summary>
        /// 更新旋转偏移
        /// </summary>
        Quaternion _RotationOffset
        {
            get
            {
                return Quaternion.Euler(_CameraSetting.RotationOffsetX, _CameraSetting.RotationOffsetY, _CameraSetting.RotationOffsetZ);
            }
        }

        /// <summary>
        /// 更新位置偏移
        /// </summary>
        Vector3 _PositionOffset
        {
            get
            {
                return new Vector3(_CameraSetting.PositionOffsetX, _CameraSetting.PositionOffsetY, _CameraSetting.PositionOffsetZ); ;
            }
        }

        /// <summary>
        /// 是否启用全局缩放
        /// </summary>
        private bool _worldScaleEnabled = false;
        /// <summary>
        /// 全局缩放比例
        /// </summary>
        private float _worldScale = 1f;
        private float _eyesToHeadDistance = 0.0f;

        /// <summary>
        /// 之前的世界尺度
        /// </summary>
        private float _previousWorldScale;

        /// <summary>
        /// 占有者
        /// </summary>
        private Possessor _possessor;
        /// <summary>
        /// 开始方向偏移
        /// </summary>
        private Quaternion _startRotationOffset;
        /// <summary>
        /// 当前方向速度
        /// </summary>
        private Quaternion _currentRotationVelocity;
        /// <summary>
        /// 当前位置速度
        /// </summary>
        private Vector3 _currentPositionVelocity;

        #endregion

        /// <summary>
        /// 导航器快照，保存当前镜头状态
        /// </summary>
        private NavigationRigSnapshot _NavigationRigSnapshot;

        /// <summary>
        /// 获取导航装置
        /// </summary>
        Transform NavigationRig
        {
            get
            {
                if (config.IsInVR)
                {
                    return SuperController.singleton.navigationRig;
                }
                else
                {
                    return SuperController.singleton.MonitorRig;
                }
            }
        }

        /// <summary>
        /// 是否已经启用
        /// </summary>
        private bool _isActived;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                //// 触发事件
                //OnCameraActivateStatusChanged?.Invoke(this, _isActive);

                if (_isActive != value)
                {
                    _isActive = value;

                    if (value)
                        Activate();
                    else
                        Deactivate();
                }
            }
        }

        /// <summary>
        /// 设置WindowCamera的开关状态
        /// </summary>
        /// <param name="on"></param>
        private void SetCameraOn(bool on = true)
        {
            if (WindowCamera == null)
            {
                return;
            }

            // 如果在监控模式，切换窗口
            if (on == SuperController.singleton.IsMonitorRigActive)
            {
                SuperController.singleton.ToggleMainMonitor();
            }

            var cameraOn = _CameraControl.GetBoolParamValue("cameraOn");

            if (cameraOn == on)
            {
                return;
            }

            WindowCamera.GetBoolJSONParam("on").val = on;
            _CameraControl.SetBoolParamValue("cameraOn", on);

            var displayControl = WindowCamera.GetStorableByID("DisplayControl") as FreeControllerV3;
            displayControl.on = !on;
        }

        /// <summary>
        /// 更新镜头
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="fov"></param>
        /// <param name="orthographic"></param>
        public void UpdateCamera(Vector3 position, Quaternion rotation, float fov, bool orthographic)
        {
            try
            {
                // 如果不启用，直接返回
                if (!_isActive && !isSetting)
                {
                    Deactivate();
                    return;
                }

                Transform positionRef = null;
                //JSONStorableFloat scaleJSON = null;
                //var personScale = 1f;
                var distanceScale = config.CameraZAxisScale;

                var heightScale = config.CameraYAxisScale;

                if (_positionReferenceAtom != null)
                {
                    positionRef = _positionReferenceAtom.mainController.control;

                    //var rescaleObject = _positionReferenceAtom.GetStorableByID("rescaleObject");

                    //if (rescaleObject != null)
                    //{
                    //    scaleJSON = rescaleObject.GetFloatJSONParam("scale");
                    //    if (scaleJSON != null)
                    //    {
                    //        personScale = scaleJSON.val;
                    //    }
                    //}
                }

                if (!isSetting && config.UseWindowCamera && WindowCamera != null)
                {
                    if (!(config.UseOriginalCamera || (config.CameraPositionSmoothing <= 0f && config.CameraRotationSmoothing <= 0f && !_FocusOnAtom)))
                    {
                        // 位置平滑大于0
                        if (config.CameraPositionSmoothing > 0)
                            position = Vector3.SmoothDamp(_CameraTransform.position, position, ref _currentPositionVelocity, config.CameraPositionSmoothing);
                        if (config.CameraRotationSmoothing > 0)
                            rotation = SmoothDamp(_CameraTransform.rotation, rotation, ref _currentRotationVelocity, config.CameraRotationSmoothing);
                    }
                    //_CameraTransform.SetPositionAndRotation(position, rotation);
                    _CameraTransform.position = position;

                    var focusRotation = FocusOn(position, rotation.GetUp());
                    if (focusRotation != Quaternion.identity)
                    {
                        _CameraTransform.rotation = focusRotation;
                    }
                    else
                    {
                        _CameraTransform.rotation = rotation;
                    }

                    _CameraTransform.position += _PositionOffset;
                    _CameraTransform.rotation *= _RotationOffset;

                    UpdatePosition(_CameraTransform, positionRef, distanceScale, heightScale);

                    if (config.CameraFOVEnabled)
                    {
                        _CameraControl.cameraFOV = fov;
                    }
                    _CameraControl.cameraToControl.orthographic = orthographic;
                }
                else
                {
                    // 如果使用自定义相机原子模式并且自定义相机原子不为空
                    if (!isSetting && config.UseCustomCameraAtom && _customCameraAtom != null)
                    {
                        //_cameraAtom.mainController.control.SetPositionAndRotation(position, rotation);
                        _customCameraAtom.mainController.control.position = position;

                        var focusRotation = FocusOn(position, rotation.GetUp());
                        if (focusRotation != Quaternion.identity)
                        {
                            _customCameraAtom.mainController.control.rotation = focusRotation;
                        }
                        else
                        {
                            _customCameraAtom.mainController.control.rotation = rotation;
                        }
                        _customCameraAtom.mainController.control.position += _PositionOffset;
                        _customCameraAtom.mainController.control.rotation *= _RotationOffset;

                        UpdatePosition(_customCameraAtom.mainController.control, positionRef, distanceScale, heightScale);
                    }
                    else
                    {
                        var navigationRigPosition = GetPosition(position, rotation, NavigationRig);

                        if (_positionLock)
                        {
                            NavigationRig.position = navigationRigPosition;
                        }

                        var focusRotation = FocusOn(position, rotation.GetUp());
                        if (focusRotation != Quaternion.identity)
                        {
                            NavigationRig.rotation = focusRotation;
                        }
                        else
                        {
                            var navigationRigRotation = GetRotation(position, rotation, NavigationRig);
                            NavigationRig.rotation = navigationRigRotation;
                        }

                        UpdatePosition(NavigationRig, positionRef, distanceScale, heightScale);
                    }

                    if (config.CameraFOVEnabled)
                    {
                        SuperController.singleton.monitorCameraFOV = fov;
                    }
                    SuperController.singleton.MonitorCenterCamera.orthographic = orthographic;
                }
                isSetting = false;
            }
            catch (Exception e)
            {
                LogUtil.Debug(e, "Failed to update: ");
            }
        }

        private void UpdatePosition(Transform target, Transform positionRef, float distanceScale, float heightScale)
        {
            if (positionRef != null)
            {
                target.position += positionRef.position;
            }

            var heightCorrection = target.position;
            heightCorrection.y += heightCorrection.y * heightScale;
            heightCorrection.z += heightCorrection.z * distanceScale;
            target.position = heightCorrection;
        }

        /// <summary>
        /// 获取旋转数据
        /// </summary>
        /// <param name="navigationRig">导航装置</param>
        /// <param name="rotationSmoothing">旋转平滑</param>
        private Quaternion GetRotation(Vector3 position, Quaternion rotation, Transform navigationRig)
        {
            // 获取导航装置的旋转
            var targetRotation = rotation;

            if (rotation == Quaternion.identity)
            {
                targetRotation = navigationRig.rotation;
            }

            if (_startRotationOffset == Quaternion.identity)
                targetRotation *= _startRotationOffset;

            targetRotation *= _RotationOffset;

            if (config.CameraRotationSmoothing > 0)
            {
                targetRotation = SmoothDamp(navigationRig.rotation, targetRotation, ref _currentRotationVelocity, config.CameraRotationSmoothing);
            }

            return targetRotation;
        }

        /// <summary>
        /// 获取位置数据
        /// </summary>
        /// <param name="navigationRig">导航装置</param>
        /// <param name="positionSmoothing">位置平滑</param>
        private Vector3 GetPosition(Vector3 position, Quaternion rotation, Transform navigationRig, bool isCamera = false)
        {
            // 结果位置
            Vector3 targetPosition;
            if (!isCamera)
            {
                // 镜头三角
                var cameraDelta = CameraTarget.centerTarget.transform.position - navigationRig.transform.position - CameraTarget.centerTarget.transform.rotation * new Vector3(0, 0, _eyesToHeadDistance);
                targetPosition = position - cameraDelta + rotation * _PositionOffset;
            }
            else
            {
                targetPosition = position + rotation * _PositionOffset;
            }

            // 位置平滑大于0
            if (config.CameraPositionSmoothing > 0)
                targetPosition = Vector3.SmoothDamp(navigationRig.position, targetPosition, ref _currentPositionVelocity, config.CameraPositionSmoothing);

            // 更新导航仪的位置
            return targetPosition;
        }

        /// <summary>
        /// 禁用导航
        /// </summary>
        /// <param name="enable"></param>
        internal void EnableNavigation(bool enable = true)
        {
            // 未启用镜头时，如果要禁用镜头，跳过处理
            if (!enable && (!config.EnableCamera || !HasMotion))
                return;

            if (config.UseWindowCamera)
            {
                SetCameraOn(!enable);
            }
            else
            {
                // 如果状态一致跳过处理
                if (SuperController.singleton.navigationDisabled != enable)
                    return;

                // 如果是要禁用导航，则重置导航装置的位置和角度
                if (!enable)
                {
                    SuperController.singleton.ResetNavigationRigPositionRotation();
                }

                // 只有镜头处于启用状态才能禁用导航
                if (!enable && !_isActive) { return; }

                if (GlobalSceneOptions.singleton != null)
                {
                    GlobalSceneOptions.singleton.disableNavigation = !enable;
                }
                SuperController.singleton.disableNavigation = !enable;

                OnCameraActivateStatusChanged?.Invoke(this, SuperController.singleton.navigationDisabled);

                // 如果使用镜头原子，则检查镜头原子是否有Embody插件，如果有，则自动进行镜头启用和禁用的处理
                if (config.UseCustomCameraAtom)
                {
                    if (_customCameraAtom != null)
                    {
                        // 如果自定义相机原子为关闭状态，则打开它
                        var cameraAtomOnJSON = _customCameraAtom.GetBoolJSONParam("on");
                        if (cameraAtomOnJSON != null && !cameraAtomOnJSON.val)
                        {
                            cameraAtomOnJSON.val = true;
                        }

                        foreach (string receiverChoice in _customCameraAtom.GetStorableIDs())
                        {
                            if (receiverChoice.StartsWith("plugin#") && receiverChoice.IndexOf("Embody") > 7)
                            {
                                var receiver = _customCameraAtom.GetStorableByID(receiverChoice);

                                if (receiver != null)
                                {
                                    var activeJSON = receiver.GetBoolJSONParam("Active");

                                    if (activeJSON != null)
                                    {
                                        activeJSON.val = SuperController.singleton.navigationDisabled;
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 激活
        /// </summary>
        private void Activate()
        {
            // 如果已经时激活状态，返回
            if (_isActived) return;

            if (_NavigationRigSnapshot == null)
            {
                _NavigationRigSnapshot = NavigationRigSnapshot.Snap();
            }

            // 禁用导航
            EnableNavigation(false);

            // 监控设备没有被激活
            var offsetStartRotation = !SuperController.singleton.MonitorRig.gameObject.activeSelf;
            // 设定开始旋转的偏移量
            if (offsetStartRotation)
                _startRotationOffset = Quaternion.Euler(0, NavigationRig.eulerAngles.y - _possessor.transform.eulerAngles.y, 0f);

            // 应用全局缩放
            ApplyWorldScale();

            // 设定为激活状态
            _isActived = true;
        }

        /// <summary>
        /// 应用全局缩放
        /// </summary>
        private void ApplyWorldScale()
        {
            // 如果没有开启全局缩放，则直接返回
            if (!_worldScaleEnabled) return;

            // 前一个全局缩放尺寸
            _previousWorldScale = SuperController.singleton.worldScale;
            // 设定全局缩放尺寸
            SuperController.singleton.worldScale = _worldScale;
        }

        /// <summary>
        /// 取消激活
        /// </summary>
        private void Deactivate(bool force = false)
        {
            if (!_isActived && !force) return;

            _NavigationRigSnapshot?.Restore();

            EnableNavigation(true);

            if (_previousWorldScale > 0f)
            {
                SuperController.singleton.worldScale = _previousWorldScale;
                _previousWorldScale = 0f;
            }

            _currentPositionVelocity = Vector3.zero;
            _currentRotationVelocity = Quaternion.identity;
            _startRotationOffset = Quaternion.identity;

            _isActived = false;

            if (!force)
            {
                Refresh();
            }
        }

        /// <summary>
        /// 销毁跟随对象
        /// </summary>
        private void DisposeFollow()
        {
            this.Deactivate();
        }

        // Source: https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
        /// <summary>
        /// 方向平滑
        /// </summary>
        /// <param name="current">当前</param>
        /// <param name="target">目标</param>
        /// <param name="currentVelocity">当前速度</param>
        /// <param name="smoothTime">平滑时间</param>
        /// <returns></returns>
        public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Quaternion currentVelocity, float smoothTime)
        {
            // account for double-cover
            var Dot = Quaternion.Dot(current, target);
            var Multi = Dot > 0f ? 1f : -1f;
            target.x *= Multi;
            target.y *= Multi;
            target.z *= Multi;
            target.w *= Multi;
            // smooth damp (nlerp approx)
            var Result = new Vector4(
                Mathf.SmoothDamp(current.x, target.x, ref currentVelocity.x, smoothTime),
                Mathf.SmoothDamp(current.y, target.y, ref currentVelocity.y, smoothTime),
                Mathf.SmoothDamp(current.z, target.z, ref currentVelocity.z, smoothTime),
                Mathf.SmoothDamp(current.w, target.w, ref currentVelocity.w, smoothTime)
            ).normalized;
            // compute deriv
            var dtInv = 1f / Time.smoothDeltaTime;
            currentVelocity.x = (Result.x - current.x) * dtInv;
            currentVelocity.y = (Result.y - current.y) * dtInv;
            currentVelocity.z = (Result.z - current.z) * dtInv;
            currentVelocity.w = (Result.w - current.w) * dtInv;
            return new Quaternion(Result.x, Result.y, Result.z, Result.w);
        }
    }
}
