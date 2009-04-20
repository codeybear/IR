using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ImageSearch.Properties;

namespace ImageHelper
{
    public class ImageUtil
    {
        /// <summary> Create a high quality thumbnail image</summary>
        public static Bitmap GetThumbnail(int iHeight, int iWidth, Bitmap bmpImage, PixelFormat PixelFormat)
        {
            Bitmap bmp = new Bitmap(iWidth, iHeight, PixelFormat);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(bmpImage, 0, 0, iWidth, iHeight);
            bmpImage.Dispose();
            return bmp;
        }

        /// <summary> Create a high quality thumbnail image</summary>
        public static Bitmap GetThumbnail(int iHeight, int iWidth, string sImage, PixelFormat PixelFormat)
        {
            Bitmap bmpImage = new Bitmap(sImage);

            return GetThumbnail(iHeight, iWidth, bmpImage, PixelFormat);
        }

        /// <summary> Create a high quality thumbnail image</summary>
        public static Bitmap GetThumbnail(BitmapData BitmapData, Bitmap bmpImage)
        {
            return GetThumbnail(BitmapData.Height, BitmapData.Width, bmpImage, PixelFormat.Format16bppRgb555);
        }

        /// <summary> Create a high quality thumbnail image</summary>
        public static Bitmap GetThumbnail(int iHeight, string sImage)
        {
            if (System.IO.File.Exists(sImage))
            {
                Bitmap bmpImage = new Bitmap(sImage);
                int iWidth = bmpImage.Width / (bmpImage.Height / iHeight);
                return GetThumbnail(iHeight, iWidth, bmpImage, PixelFormat.Format32bppRgb);
            }
            else
                return ImageSearch.Properties.Resources.bmpMissingImage;
        }

         ///<summary> Create a bitmap from byte array </summary>
        public static Bitmap CreateBitmapFromArray(byte[] Bytes, BitmapData SmallBitmapData)
        {
            Bitmap bmp = new Bitmap(SmallBitmapData.Width, SmallBitmapData.Height, SmallBitmapData.PixelFormat);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(Bytes, 0, bmpData.Scan0, Bytes.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }

    /// <summary> Uses the bitmap.LockBits method to enable high speed access to pixel data </summary>
    public class BitmapBytes
    {
        private Byte[] _Bytes;
        private int _iRowLength;
        private BitmapData _bmpData;
        private Bitmap _bmp;

        public Byte[] Bytes
        { 
            get { return _Bytes; }
        }

        public BitmapBytes(Bitmap bmp)
        {
            LockBitmap(bmp);
        }

        /// <summary> Copy the contents of a bitmap to an array of bytes
        /// This will greatly improve the speed of accessing the pixel data. </summary>
        public void LockBitmap(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            _bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            _iRowLength = _bmpData.Stride;

            int iPixelCount = _bmpData.Stride * _bmpData.Height;
            _Bytes = new byte[iPixelCount];
            System.Runtime.InteropServices.Marshal.Copy(_bmpData.Scan0, _Bytes, 0, iPixelCount);
            _bmp = bmp;
        }

        /// <summary> Copy the data back into the bitmap.</summary>
        public void UnlockBitmap(bool bCopyToBitmap)
        {
            if (bCopyToBitmap)
            {
                int iPixelCount = _bmpData.Stride * _bmpData.Height;
                System.Runtime.InteropServices.Marshal.Copy(_Bytes, 0, _bmpData.Scan0, iPixelCount);
            }

            // Unlock the bitmap.
            _bmp.UnlockBits(_bmpData);

            // Release resources
            _Bytes = null;
            _bmpData = null;
        }

        /// <summary> Get pixel hsi value from byte array at specified coordinates </summary>
        public Color GetHSIFromPoint(int x ,int y)
        {
            byte r, g, b;
            int iPixel = (y * _iRowLength) + (x * 3);

            r = _Bytes[iPixel + 2];
            g = _Bytes[iPixel + 1];
            b = _Bytes[iPixel];

            Color Color = new Color();
            Color = Color.FromArgb(r, g, b);
            return Color;
        }

        public int GetPixelPart(int x, int y, ImageSearch.RGB iRGB)
        {
            return _Bytes[(y * _iRowLength) + (x * 3) + (int)iRGB];
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            _Bytes[(y * _iRowLength) + (x * 3) + 2] = r;
            _Bytes[(y * _iRowLength) + (x * 3) + 1] = g;
            _Bytes[(y * _iRowLength) + (x * 3)] = b;
        }
    }
}
