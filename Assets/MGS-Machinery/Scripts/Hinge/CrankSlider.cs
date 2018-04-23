﻿/*************************************************************************
 *  Copyright © 2015-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  CrankSlider.cs
 *  Description  :  Define CrankSlider component.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/17/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using Mogoson.Mathematics;
using UnityEngine;

namespace Mogoson.Machinery
{
    [AddComponentMenu("Mogoson/Machinery/CrankSlider")]
    [ExecuteInEditMode]
    public class CrankSlider : CrankLinkMechanism
    {
        #region Field and Property
        /// <summary>
        /// Joint of link bar and slider.
        /// </summary>
        public Transform joint;

        /// <summary>
        /// All mechanism is set Intact.
        /// </summary>
        public bool IsIntact { get { return crank && link && joint; } }

        /// <summary>
        /// lsJoint start local position.
        /// </summary>
        public Vector3 JointPosition { protected set; get; }

        /// <summary>
        /// link bar start local position.
        /// </summary>
        protected Vector3 linkPosition;

        /// <summary>
        /// lsJoint start local euler angles.
        /// </summary>
        protected Vector3 JointAngles;

        /// <summary>
        /// Line from link bar to slider.
        /// </summary>
		protected Line linkLine;

        /// <summary>
        /// Circle base link bar.
        /// </summary>
		protected Circle linkCircle;

        /// <summary>
        /// Radius of the circle that base link bar.
        /// </summary>
		protected double linkRadius;

        /// <summary>
        /// Link bar and slider joint is on the right of link bar on start.
        /// </summary>
		protected bool isRight;

#if UNITY_EDITOR
        /// <summary>
        /// This mechanism is initialized?
        /// </summary>
        private bool isInitialized = false;
#endif
        #endregion

        #region Protected Method
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                Initialize();
        }

#if UNITY_EDITOR
        protected virtual void Update()
        {
            if (Application.isPlaying)
                return;

            if (IsIntact)
            {
                if (!isInitialized)
                    Initialize();
                DriveLinkJoints();
            }
            else
                isInitialized = false;
        }
#endif
        /// <summary>
        /// Drive link bar and slider.
        /// </summary>
		protected override void DriveLinkJoints()
        {
            //Rivet joints.
            joint.localEulerAngles = JointAngles;
            crank.transform.localPosition = Vector3.zero;
            link.transform.localPosition = linkPosition;

            var linkPoint = CorrectPoint(GetLinkPosition());
            linkCircle = new Circle(linkPoint, linkRadius);
            var points = Planimetry.GetIntersections(linkCircle, linkLine);
            if (points == null)
            {
                IsLock = true;
                return;
            }

            IsLock = false;
            var point = Point.Zero;
            if (points.Count == 1)
                point = points[0];
            else
                point = isRight ? points[0] : points[1];
            joint.localPosition = new Vector3((float)point.x, (float)point.y);

            //Drive linkBar.
            link.Drive();
        }

        /// <summary>
        /// Clear angles z and set y to 90.
        /// </summary>
        /// <param name="angles">Local euler angles.</param>
        /// <returns>Correct lsJoint angles.</returns>
        protected Vector3 CorrectLSJointAngles(Vector3 angles)
        {
            return new Vector3(angles.x, 90);
        }

        protected void Initialize()
        {
            //Correct crank.
            crank.transform.localEulerAngles = CorrectAngles(crank.transform.localEulerAngles);
            crank.Awake();

            //Correct lsJoint.
            JointAngles = CorrectLSJointAngles(joint.localEulerAngles);
            joint.localEulerAngles = JointAngles;

            //Save start local position.
            linkPosition = CorrectPosition(link.transform.localPosition);
            JointPosition = CorrectPosition(joint.localPosition);

            //Initialize CrankSlider mathematical model.
            var lsJointPoint = CorrectPoint(joint.localPosition);
            var linkPoint = CorrectPoint(GetLinkPosition());
            var direction = transform.InverseTransformDirection(ProjectDirection(joint.forward));
            var directionPoint = CorrectPoint(joint.localPosition + direction);
            linkRadius = Point.Distance(linkPoint, lsJointPoint);
            linkLine = Line.FromPoints(lsJointPoint, directionPoint);
            isRight = lsJointPoint.x - linkPoint.x >= 0;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Project direction vector on plane(Normal is transform.forward).
        /// </summary>
        /// <param name="direction">World space direction.</param>
        /// <returns>Project direction.</returns>
        public Vector3 ProjectDirection(Vector3 direction)
        {
            direction = Vector3.ProjectOnPlane(direction, transform.forward);
            if (direction == Vector3.zero)
                direction = transform.right;
            return direction;
        }
        #endregion
    }
}