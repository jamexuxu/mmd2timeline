using System.Collections.Generic;
using System.Linq;

namespace mmd2timeline
{
    /// <summary>
    /// 处理眼睛看向Player时的特殊情况
    /// </summary>
    internal partial class CameraHelper
    {
        Dictionary<string, EyesControl.LookMode> _lookModes = new Dictionary<string, EyesControl.LookMode>();

        /// <summary>
        /// 镜头启用时同步眼睛控制
        /// </summary>
        void SyncEyesControl()
        {
            #region 暂存当前的视线模式
            _lookModes.Clear();
            var personAtoms = SuperController.singleton.GetAtoms().Where(a => a.type == "Person").ToArray();
            foreach (var atom in personAtoms)
            {
                var eyeBehavior = (EyesControl)atom.GetStorableByID("Eyes");

                if (eyeBehavior != null)
                {
                    // 只对视线为Player模式的进行处理
                    if (eyeBehavior.currentLookMode == EyesControl.LookMode.Player)
                    {
                        _lookModes.Add(atom.uid, eyeBehavior.currentLookMode);

                        if (!isSetting && config.UseWindowCamera && WindowCamera != null)
                        {
                            eyeBehavior.currentLookMode = EyesControl.LookMode.Target;
                            eyeBehavior.lookAt = WindowCamera.mainController.transform;
                        }
                        else if (!isSetting && IsUsingCustomCameraAtom)
                        {
                            eyeBehavior.currentLookMode = EyesControl.LookMode.Target;
                            eyeBehavior.lookAt = _customCameraAtom.mainController.transform;
                        }
                    }
                }
            }
            #endregion

        }

        /// <summary>
        /// 镜头停用时恢复眼睛控制
        /// </summary>
        void RestoreEyesControl()
        {
            foreach (var item in _lookModes)
            {
                var atom = SuperController.singleton.GetAtomByUid(item.Key);

                if (atom != null)
                {
                    var eyeBehavior = (EyesControl)atom.GetStorableByID("Eyes");

                    if (eyeBehavior != null)
                    {
                        eyeBehavior.currentLookMode = item.Value;
                    }
                }
            }

            _lookModes.Clear();
        }
    }
}
