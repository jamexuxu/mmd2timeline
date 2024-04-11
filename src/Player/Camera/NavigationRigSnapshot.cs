using UnityEngine;

namespace mmd2timeline
{
    /// <summary>
    /// 导航器快照
    /// </summary>
    internal class NavigationRigSnapshot
    {
        private float _playerHeightAdjust;
        private Quaternion _rotation;
        private Vector3 _position;

        //private Vector3 _monitorPosition;
        //private Quaternion _monitorRotation;
        //private Vector3 _monitorLocalScale;
        //private float _monitorFOV;
        //private bool _monitorOrthographic;

        //private Vector3 _monitorRigPosition;
        //private Quaternion _monitorRigRotation;

        /// <summary>
        /// 执行快照。保存当前镜头状态
        /// </summary>
        /// <returns></returns>
        public static NavigationRigSnapshot Snap()
        {
            //SuperController.singleton.ResetNavigationRigPositionRotation();
            //SuperController.singleton.ResetMonitorCenterCamera();
            var navigationRig = SuperController.singleton.navigationRig;
            //var monitorCenterCamera = SuperController.singleton.MonitorCenterCamera;
            //var monitorTransform = monitorCenterCamera.transform;
            //var monitorRig = SuperController.singleton.MonitorRig;
            return new NavigationRigSnapshot
            {
                _playerHeightAdjust = SuperController.singleton.playerHeightAdjust,
                _position = navigationRig.position,
                _rotation = navigationRig.rotation,
                //_monitorRigPosition = Vector3.zero,
                //_monitorRigRotation = Quaternion.identity,
                //_monitorPosition = monitorTransform.position,
                //_monitorRotation = monitorTransform.rotation,
                //_monitorLocalScale = monitorTransform.localScale,
                //_monitorFOV = 40f,
                //_monitorOrthographic = monitorCenterCamera.orthographic,
            };
        }

        /// <summary>
        /// 恢复镜头状态
        /// </summary>
        public void Restore()
        {
            var navigationRig = SuperController.singleton.navigationRig;
            SuperController.singleton.playerHeightAdjust = _playerHeightAdjust;
            navigationRig.position = _position;
            navigationRig.rotation = _rotation;
            //var monitorRig = SuperController.singleton.MonitorRig;
            //monitorRig.localPosition = _monitorRigPosition;
            //monitorRig.localRotation = _monitorRigRotation;

            //var monitorCenterCamera = SuperController.singleton.MonitorCenterCamera;
            //var monitorTransform = monitorCenterCamera.transform;
            //monitorTransform.position = _monitorPosition;
            //monitorTransform.rotation = _monitorRotation;
            //monitorTransform.localScale = _monitorLocalScale;
            //SuperController.singleton.monitorCameraFOV = _monitorFOV;
            //monitorCenterCamera.orthographic = _monitorOrthographic;

            //SuperController.singleton.MoveToSceneLoadPosition();
            SuperController.singleton.MonitorCenterCamera.transform.rotation = navigationRig.rotation;
            var monitorPosition = navigationRig.position;
            monitorPosition.y = 1.6f;
            SuperController.singleton.MonitorCenterCamera.transform.position = monitorPosition;
        }
    }
}
