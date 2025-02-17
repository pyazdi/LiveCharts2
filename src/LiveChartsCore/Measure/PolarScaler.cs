﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Measure
{
    /// <summary>
    /// Defines the polar scaler class, this class helps to scale from the data scale to the user interface scale and vise versa.
    /// </summary>
    public class PolarScaler
    {
        private const double ToRadians = Math.PI / 180d;
        private readonly double _deltaRadius, _innerRadius, _scalableRadius;
        private readonly double _deltaAngleVal;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarScaler"/> class.
        /// </summary>
        /// <param name="drawMagrinLocation">The draw margin location.</param>
        /// <param name="drawMarginSize">Size of the draw margin.</param>
        /// <param name="radiusAxis">The radius axis.</param>
        /// <param name="angleAxis">The angle axis.</param>
        /// <param name="innerRadius">The inner radius.</param>
        /// <param name="usePreviousScale">Indicates if the scaler should be built based on the previous known data.</param>
        /// <exception cref="Exception">The axis is not ready to be scaled.</exception>
        public PolarScaler(
            LvcPoint drawMagrinLocation,
            LvcSize drawMarginSize,
            IPolarAxis angleAxis,
            IPolarAxis radiusAxis,
            float innerRadius,
            bool usePreviousScale = false)
        {
            var actualAngleBounds = usePreviousScale ? angleAxis.PreviousDataBounds : angleAxis.DataBounds;
            var actualAngleVisibleBounds = usePreviousScale ? angleAxis.PreviousVisibleDataBounds : angleAxis.VisibleDataBounds;

            var actualRadiusBounds = usePreviousScale ? radiusAxis.PreviousDataBounds : radiusAxis.DataBounds;
            var actualRadiusVisibleBounds = usePreviousScale ? radiusAxis.PreviousVisibleDataBounds : radiusAxis.VisibleDataBounds;

            if (actualAngleBounds is null || actualAngleVisibleBounds is null) throw new Exception("angle bounds not found");
            if (actualRadiusBounds is null || actualRadiusVisibleBounds is null) throw new Exception("radius bounds not found");

            CenterX = drawMagrinLocation.X + drawMarginSize.Width * 0.5f;
            CenterY = drawMagrinLocation.Y + drawMarginSize.Height * 0.5f;

            MinRadius = actualRadiusVisibleBounds.Min;
            MaxRadius = actualRadiusVisibleBounds.Max;
            _deltaRadius = MaxRadius - MinRadius;

            var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;
            _innerRadius = 50; //innerRadius;
            _scalableRadius = minDimension * 0.5 - _innerRadius;

            MinAngle = actualAngleBounds.Min;
            MaxAngle = actualAngleBounds.Max;
            _deltaAngleVal = MaxAngle - MinAngle;
        }

        /// <summary>
        /// Gets the center in the X axis.
        /// </summary>
        public float CenterX { get; private set; }

        /// <summary>
        /// Gets the center in the Y axis.
        /// </summary>
        public float CenterY { get; private set; }

        /// <summary>
        /// Gets the max radius.
        /// </summary>
        public double MaxRadius { get; private set; }

        /// <summary>
        /// Gets the min radius.
        /// </summary>
        public double MinRadius { get; private set; }

        /// <summary>
        /// Gets the max angle.
        /// </summary>
        public double MinAngle { get; private set; }

        /// <summary>
        /// Gets the min angle.
        /// </summary>
        public double MaxAngle { get; private set; }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="polarPoint">The polar point.</param>
        /// <returns></returns>
        public LvcPoint ToPixels(ChartPoint polarPoint)
        {
            return ToPixels(polarPoint.SecondaryValue, polarPoint.PrimaryValue);
        }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="angle">The angle in chart values scale.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public LvcPoint ToPixels(double angle, double radius)
        {
            var p = (radius - MinRadius) / _deltaRadius;
            var r = _innerRadius + _scalableRadius * p;
            var a = 360 * angle / _deltaAngleVal;
            a *= ToRadians;

            unchecked
            {
                return new LvcPoint(
                    CenterX + (float)(Math.Cos(a) * r),
                    CenterY + (float)(Math.Sin(a) * r));
            }
        }
    }
}

