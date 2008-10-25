using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ImageSearch
{
    class Image
    {
        public Bitmap GetThumnail(int iHeight, string sBitmap)
        {
            Bitmap bmp = new Bitmap(sBitmap);

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

        }
    }
}
