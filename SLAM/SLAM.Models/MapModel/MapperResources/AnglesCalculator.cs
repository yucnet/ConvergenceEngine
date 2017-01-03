﻿using System;
using System.Runtime.CompilerServices;

namespace SLAM.Models.MapModel.MapperResources {

    internal sealed class AnglesCalculator {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AlphaAngle(double a, double b, double c) {
            return TrigonometricHelper.DegreesFromRadians(RadiansAngle(c, b, a));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double BetaAngle(double a, double b, double c) {
            return TrigonometricHelper.DegreesFromRadians(RadiansAngle(a, c, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GammaAngle(double a, double b, double c) {  
            return TrigonometricHelper.DegreesFromRadians(RadiansAngle(a, b, c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RadiansAngle(double a, double b, double c) {
            return // radians γ = arccos((a² + b² - c²) / (2ab))
                Math.Acos(
                    (Math.Pow(a, 2.0) + Math.Pow(b, 2.0) - Math.Pow(c, 2.0))
                    / (2.0 * a * b));
        }
    }
}