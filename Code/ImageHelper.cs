using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageHelper
{
    public class ImageUtil
    {
        /// <summary>
        /// Create a high quality thumbnail image
        /// </summary>
        public static Bitmap GetThumbnail(int iHeight, string sImage)
        {
            Image FullImage = new Bitmap(sImage);
            int iWidth = FullImage.Width / (FullImage.Height / iHeight);

            Bitmap bmp = new Bitmap(iWidth, iHeight);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(FullImage, 0, 0, iWidth, iHeight);
            FullImage.Dispose();
            return bmp;
        }

        /// <summary>
        /// Make sure the list of files are only images that the Bitmap object can load
        /// </summary>
        public static string[] GetValidFiles(IEnumerable<string> FileList)
        {
            List<string> ValidList = new List<string>();

            foreach (string sFile in FileList)
            {
                FileInfo info = new FileInfo(sFile);
                string sExt = info.Extension.ToLower();

                if (".bmp.png.gif.jpg.jpeg.tif".Contains(sExt))
                    ValidList.Add(sFile);
            }

            string[] ReturnList = new string[ValidList.Count];
            ValidList.CopyTo(ReturnList);
            return ReturnList;
        }
    }

    /// <summary>
    /// Uses the bitmap.LockBits method to enable high speed access to pixel data
    /// </summary>
    public class BitmapBytes
    {
        private Byte[] _Bytes;
        private int _iRowLength;

        public Byte[] Bytes
        { 
            get { return _Bytes; }
        }

        public BitmapBytes(Bitmap bmp)
        {
            LockBitmap(bmp);
        }

        /// <summary>
        /// Copy the contents of a bitmap to an array of bytes.
        /// This will greatly improve the speed of accessing the pixel data.
        /// </summary>
        public void LockBitmap(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            _iRowLength = bmpData.Stride;

            int iPixelCount = bmpData.Stride * bmpData.Height;
            _Bytes = new byte[iPixelCount];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, _Bytes, 0, iPixelCount);
        }

        /// <summary>
        /// Get pixel from byte array at specified coordinates
        /// </summary>
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
    }
}
