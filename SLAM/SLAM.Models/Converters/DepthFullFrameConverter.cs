﻿using System.Windows.Media;


namespace SLAM.Models.Converters {

    internal sealed class DepthFullFrameConverter : DataConverter, IDataConverter {

        private Color nearColor;
        private Color farColor;
        private Color[] intensity;

        public DepthFullFrameConverter(DepthFrameSequenceInfo frameInfo) : base(frameInfo) {
            InitializeColorBuffers();
        }

        private void InitializeColorBuffers() {

            nearColor = Color.FromArgb(255, 0, 128, 192);
            farColor = Color.FromArgb(255, 0, 0, 30);

            int fullDepth = FrameInfo.MaxDepth - FrameInfo.MinDepth;
            double intencityStep = 192.0 / fullDepth;

            intensity = new Color[fullDepth];
            for (int i = 0; i < intensity.Length; ++i) {
                byte colorComponent = (byte)(byte.MaxValue - (i * intencityStep));
                intensity[i] = Color.FromArgb(255, colorComponent, colorComponent, colorComponent);
            }
        }

        public override void ConvertRawDataToViewportFrame(byte[] rawInput, byte[] viewportOutput) {

            int fullDepth = FrameInfo.MaxDepth - FrameInfo.MinDepth;
            double intencityStep = 192.0 / fullDepth;

            int colorPixelIndex = 0;
            for (int i = 0; i < FrameInfo.Length; ++i) {

                short depth = GetDepthFromRawFrameAt(rawInput, i * sizeof(short));

                if (depth < FrameInfo.MinDepth) {
                    SetColorToViewportByteArray(viewportOutput, colorPixelIndex, nearColor);
                }
                else if (depth > FrameInfo.MaxDepth) {
                    SetColorToViewportByteArray(viewportOutput, colorPixelIndex, farColor);
                }
                else {
                    SetColorToViewportByteArray(viewportOutput, colorPixelIndex, intensity[depth - FrameInfo.MinDepth]);
                }
                colorPixelIndex += sizeof(int);
            }
        }
    }
}