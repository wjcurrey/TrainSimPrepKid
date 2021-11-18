﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;

namespace Orts.Graphics.Window.Controls
{
    public abstract class TextControl : WindowTextureControl
    {
        private protected Font font;
        private protected static Brush whiteBrush = new SolidBrush(Color.White);

        protected TextControl(int x, int y, int width, int height) : 
            base(x, y, width, height)
        {
        }

        public override void Initialize(WindowManager windowManager)
        {
            base.Initialize(windowManager);
        }

        protected virtual void InitializeSize(string text, WindowManager windowManager)
        {
            using (Bitmap measureBitmap = new Bitmap(1, 1))
            {
                using (System.Drawing.Graphics measureGraphics = System.Drawing.Graphics.FromImage(measureBitmap))
                {
                    Resize(measureGraphics.MeasureString(text, font).ToSize(), windowManager);
                }
            }
        }

        protected virtual void Resize(Size size, WindowManager windowManager)
        {
            if (null == windowManager)
                throw new ArgumentNullException(nameof(windowManager));

            Texture2D current = texture;
            texture = new Texture2D(windowManager.Game.GraphicsDevice, size.Width, size.Height, false, SurfaceFormat.Bgra32);
            current?.Dispose();
        }

        protected virtual void DrawString(string text)
        {
            // Create the final bitmap
            using (Bitmap bmpSurface = new Bitmap(texture.Width, texture.Height))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpSurface))
                {
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                    // Draw the text to the clean bitmap
                    g.Clear(System.Drawing.Color.Transparent);
                    g.DrawString(text, font, whiteBrush, PointF.Empty);

                    BitmapData bmd = bmpSurface.LockBits(new Rectangle(0, 0, bmpSurface.Width, bmpSurface.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    int bufferSize = bmd.Height * bmd.Stride;
                    //create data buffer 
                    byte[] bytes = new byte[bufferSize];
                    // copy bitmap data into buffer
                    Marshal.Copy(bmd.Scan0, bytes, 0, bytes.Length);

                    // copy our buffer to the texture
                    texture.SetData(bytes);
                    // unlock the bitmap data
                    bmpSurface.UnlockBits(bmd);
                }
            }
        }

    }
}