﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConvergenceEngine.Views.Converters {

    [ValueConversion(typeof(IEnumerable<Point>), typeof(ImageSource))]
    public class PointSequenceToImageSourceConverter : IValueConverter {

        private Point max;
        private int width, height;
        private double offsetX, offsetY;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            IEnumerable<Point> newPoints = value as IEnumerable<Point>;
            Color newColor = (Color)parameter;

            if (newPoints == null || newPoints.Count() < 1) { return null; }

            UpdateLimits(newPoints);
            return CreateBitmap(newPoints, newColor);
        }

        private void UpdateLimits(IEnumerable<Point> points) {

            max = new Point(points.Max(p => Math.Abs(p.X)), points.Max(p => Math.Abs(p.Y)));

            offsetX = max.X;
            offsetY = max.Y;

            width = (int)(max.X * 2) + 1;
            height = (int)(max.Y * 2) + 1;
        }

        private ImageSource CreateBitmap(IEnumerable<Point> points, Color color) {

            if (max.X < 1 || max.Y < 1) { return null; }

            WriteableBitmap result = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgr32, null);
            byte[] fullFrameBuffer = new byte[width * height * sizeof(int)];

            foreach (var point in points) {
                point.Offset(offsetX, offsetY);
                int index = GetLinearIndex((int)point.X, (height - 1) - ((int)point.Y), width);
                SetColorToViewportByteArray(fullFrameBuffer, index * sizeof(int), color);
            }

            result.WritePixels(new Int32Rect(0, 0, width, height), fullFrameBuffer, result.PixelWidth * sizeof(int), 0);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetColorToViewportByteArray(byte[] viewportByteArray, int startIndex, Color color) {
            viewportByteArray[startIndex] = color.B;
            viewportByteArray[++startIndex] = color.G;
            viewportByteArray[++startIndex] = color.R;
            viewportByteArray[++startIndex] = color.A;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}