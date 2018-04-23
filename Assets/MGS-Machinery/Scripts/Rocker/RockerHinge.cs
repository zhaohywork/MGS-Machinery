﻿/*************************************************************************
 *  Copyright © 2016-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  RockerHinge.cs
 *  Description  :  Define RockerHinge component.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/17/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using Mogoson.Extention;
using UnityEngine;

namespace Mogoson.Machinery
{
    /// <summary>
    /// Hinge rotate around the axis base on rocker joint.
    /// </summary>
    [AddComponentMenu("Mogoson/Machinery/RockerHinge")]
    [ExecuteInEditMode]
    public class RockerHinge : RockerMechanism
    {
        #region Field and Property
        /// <summary>
        /// Rotate Axis.
        /// </summary>
        public Vector3 Axis { get { return transform.forward; } }

        /// <summary>
        /// Zero Axis.
        /// </summary>
        public Vector3 ZeroAxis
        {
            get
            {
                if (transform.parent)
                    return transform.parent.up;
                else
                    return Vector3.up;
            }
        }
        #endregion

        #region Protected Method
#if UNITY_EDITOR
        protected virtual void Update()
        {
            if (!Application.isPlaying && joint)
                Drive();
        }
#endif
        #endregion

        #region Public Method
        /// <summary>
        /// Drive rocker.
        /// </summary>
        public override void Drive()
        {
            joint.position = transform.position;
            var angle = -EVector3.ProjectAngle(joint.forward, ZeroAxis, Axis);
            var euler = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
            transform.localRotation = Quaternion.Euler(euler);
        }
        #endregion
    }
}