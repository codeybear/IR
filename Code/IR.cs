using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace ImageSearch
{
    public struct ImageInfo
    {
        public string File { get; set; }
        public SectionInfo[] Section { get; set;}
        public byte[] SmallImage { get; set; }
        public int AspectRatio { get; set; }
    }

    public struct SectionInfo
    {
        public int[] Hues { get; set; }
        // Conserve memory with smallest variables possible
        public UInt16 ChangeCount { get; set; }
        public UInt16 ChangeDistance { get; set; }
    }

    public struct Result
    {
        public string File { get; set; }
        public int Score { get; set; }
        public Bitmap SmallImage;
        public string Info { get; set; }
    }

    public struct Dupes
    {
        public string File { get; set; }
        public List<Result> ResultList { get; set; }
        public Bitmap smallImage;
    }

    public class DupesList : List<Dupes>
    {
        public void SaveToCSV(string sFile)
        {
            string sContent = "";

            foreach (Dupes dupes in this)
            {
                foreach (Result result in dupes.ResultList)
                {
                    string[] sLine = new string[] { result.File, result.Info.Replace("/", ",") };
                    sContent += string.Join(",", sLine) + Environment.NewLine;
                }
            }

            TextWriter tw = new StreamWriter(sFile);
            tw.Write(sContent);
            tw.Close();
        }
    }

    public struct Coords
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    public enum RGB
    {
        R = 2,
        G = 1,
        B = 0
    }

    public class IR
    {
        const int HUE_MAX = 360;
        const int PIXEL_DIFF_TOLERENCE = 100;

        // Properties of the small version of the image
        public BitmapData SmallBitmapData = new BitmapData { Height = 40, Width = 40, Stride=80, PixelFormat = PixelFormat.Format16bppRgb555 };
        public int _iNumBars = 12;

        private int _iCols;
        private int _iRows;
        private int _iGridSize;
        private Dictionary<String,ImageInfo> _ImageList = new Dictionary<String,ImageInfo>();

        public IR()
        {
            // Default grid size is 3x3
            _iCols = 3;
            _iRows = 3;
            _iGridSize = _iCols * _iRows;
        }

        /// <summary> Number of bars the histogram is split into </summary>
        public int NumBars
        {
            get { return _iNumBars; }
            set { _iNumBars = value; }
        }

        /// <summary> Create a signature for this image and load it into the library </summary>
        public void LoadImage(string sFile)
        {
            _ImageList.Add(sFile, GetSignature(sFile));
        }

        /// <summary> Clear all images from the library </summary>
        public void Clear()
        {
            _ImageList = new Dictionary<String,ImageInfo>();
        }

        /// <summary> Delete an image from the libary based on its filename including path </summary>
        public void DeleteImage(string sFile)
        {
            _ImageList.Remove(sFile);
        }

        /// <summary> Number of images that have been loaded </summary>
        public int Count
        {
            get { return _ImageList.Count; }
        }

        /// <summary> Find duplicates within the currectly loaded images </summary>
        public DupesList FindDuplicates()
        {
            DupesList Dupes = new DupesList();

            Dictionary<string, string> ResultCheck = new Dictionary<string, string>();

            DateTime dt = DateTime.Now;

            foreach (string sKey in _ImageList.Keys)
            {
                ImageInfo ThisImage = _ImageList[sKey];

                // If this image has already been found as a match then don't search for it
                // This prevents unnecessary duplicates, i.e. where image A matches B, also B matches A
                if(ResultCheck.ContainsKey(ThisImage.File))
                    continue;

                List<Result> Results = Search(ThisImage, 0, 130);

                // Search results will always contain 1 result as the search image will be found in the results
                if (Results.Count > 1)
                {
                    //Bitmap bmp = ImageHelper.ImageUtil.CreateBitmapFromArray(ThisImage.SmallImage, SmallBitmapData);
                    Dupes.Add(new Dupes { File = ThisImage.File, ResultList = Results, smallImage = null });

                    foreach (Result result in Results)
                        if (!ResultCheck.ContainsKey(result.File))
                            ResultCheck.Add(result.File, "");
                }
            }

            TimeSpan duration = DateTime.Now - dt;
            Console.WriteLine("ImageSearch.IR.FindDuplicates Duration: " + duration.ToString());

            return Dupes;
        }

        /// <summary> Search for an image within the loaded list of images
        /// The likelyness of a match is given by a score
        /// Returns a list of results sorted by the score
        /// </summary>
        /// <param name="SearchImage">ImageInfo structure of the image to be searched for</param>
        /// <param name="iTop">The number of results returned, set to 0 to return all</param>
        /// <param name="iTopScore">Only return images with a match score below this, set to 0 return all</param>
        public List<Result> Search(ImageInfo SearchImage, int iTop, int iTopScore)
        {
            List<Result> ResultList = new List<Result>();

            foreach (string sKey in _ImageList.Keys)
            {
                ImageInfo ThisImage = _ImageList[sKey];

                bool bMatch = true;
                int iScore = 0;
                int iChangeScore = 0;
                int iDistanceScore = 0;
                int iDistanceDiff = 0;

                // TODO assign this to iScore and run the tests
                int iAspectScore = AspectCompare(SearchImage.AspectRatio, ThisImage.AspectRatio);

                for (int iSection = 0; iSection < _iGridSize - 1; iSection++)
                {
                    iScore += Compare(SearchImage.Section[iSection].Hues, ThisImage.Section[iSection].Hues);
                    // TODO not sure if this is usefull
                    iChangeScore += Math.Abs(SearchImage.Section[iSection].ChangeCount - ThisImage.Section[iSection].ChangeCount);

                    iDistanceDiff += Math.Abs(SearchImage.Section[iSection].ChangeDistance - ThisImage.Section[iSection].ChangeDistance);
                    iDistanceScore = DistanceScore(iDistanceDiff);

                    // Stop as soon as the tolerences are reached
                    if ((iScore + iDistanceScore) > iTopScore ||  iDistanceScore > 100)
                    {
                        bMatch = false;
                        break;
                    }
                }

                if (bMatch)
                {
                    //Bitmap bmp = ImageHelper.ImageUtil.CreateBitmapFromArray(ThisImage.SmallImage, SmallBitmapData);
                    string sInfo = iScore.ToString() + "/" + iChangeScore.ToString() + "/" + iDistanceScore.ToString();
                    ResultList.Add(new Result { File = ThisImage.File, Score = iScore, SmallImage = null, Info = sInfo });
                }
            }

            if(ResultList.Count > 1)
                ResultList.Sort((r1, r2) => r1.Score.CompareTo(r2.Score));

            if(iTop > 0 && iTop < ResultList.Count)
                ResultList = ResultList.GetRange(0, iTop);

            return ResultList;
        }

        private int DistanceScore(int iDistanceDiff)
        {
            if (iDistanceDiff > 80) return 40;
            if (iDistanceDiff > 45) return 20;
            return 0;
        }

        /// <summary> Compare two aspect ratios, producing a score based on the difference </summary>
        private int AspectCompare(int iRatio, int iRatio2)
        {
            const float RATIOTOLERANCE = (float)0.2;
            const int RATIOWEIGHT = 100;

            int iDiff = Math.Abs(iRatio - iRatio2) / (int)(RATIOTOLERANCE * (float)iRatio);

            return iDiff * RATIOWEIGHT;
        }

        /// <summary> Search for an image within the loaded list of images
        /// Method is overloaded see <seealso cref="Search(ImageInfo SearchImage, int iTop, int iTopScore)"/>
        /// </summary>
        public List<Result> Search(string sFile, int iTop, int iTopScore)
        {
            ImageInfo SearchImageInfo = GetSignature(sFile);

            return Search(SearchImageInfo, iTop, iTopScore);
        }

        private int CompareSmallImage(byte[] Image1, byte[] Image2)
        {
            int iScore = 0;

            for (int iCount = 0; iCount < Image1.Length; iCount++)
            {
                iScore += Math.Abs(Image1[iCount]- Image2[iCount]);
            }

            return iScore;
        }

        /// <summary> Compare to sections of the image signature </summary>
        private int Compare(int[] Hues1, int[] Hues2)
        {
            int iScore = 0;

            for (int iCount = 0; iCount < _iNumBars; iCount++)
            {
                iScore += Math.Abs(Hues1[iCount] - Hues2[iCount]);
            }

            return iScore;
        }

        /// <summary> Get the image signature for a specific file </summary>
        public ImageInfo GetSignature(string sFile)
        {
            ImageInfo ImageInfo = new ImageInfo { File = sFile, Section = new SectionInfo[_iGridSize] };
            Bitmap bmp = new Bitmap(sFile);
            ImageHelper.BitmapBytes BitmapBytes = new ImageHelper.BitmapBytes(bmp);

            ImageInfo.AspectRatio = (int)((float)bmp.Height / (float)bmp.Width * 100);

            DateTime dt = DateTime.Now;

            for (int iRow = 0; iRow < _iRows; iRow++)
                for (int iCol = 0; iCol < _iCols; iCol++)
                {
                    Coords Coords = GetSectionCoords(iRow, iCol, _iRows, _iCols, bmp.Width, bmp.Height);
                    int iSection = (iRow * _iCols) + iCol;
                    ImageInfo.Section[iSection] = GetHisto(Coords, BitmapBytes);

                    SectionInfo EdgeVars = GetEdge(Coords, BitmapBytes, false);
                    ImageInfo.Section[iSection].ChangeCount = EdgeVars.ChangeCount;
                    ImageInfo.Section[iSection].ChangeDistance = EdgeVars.ChangeDistance;
                }

            TimeSpan duration = DateTime.Now - dt;
            Console.WriteLine("GetSignature Duration: " + duration.ToString());

            return ImageInfo;
        }

        public Bitmap GetTestImage(string sFile)
        {
            Bitmap bmp = new Bitmap(sFile);

            ImageHelper.BitmapBytes BitmapBytes = new ImageHelper.BitmapBytes(bmp);
            GetEdge(new Coords { Left = 0, Top = 0, Bottom = bmp.Height - 1, Right = bmp.Width - 1 }, BitmapBytes, true);
            BitmapBytes.UnlockBitmap(true);

            return bmp;
        }

        /// <summary> Get the edge detection values for a section of an image </summary>
        /// <param name="Coords">Coords specify section of the image to analyse</param>
        /// <param name="bModifyImage">if true change the image to show pattern, for debug purposes</param>
        private SectionInfo GetEdge(Coords Coords, ImageHelper.BitmapBytes BitmapBytes, bool bModifyImage)
        {
            int x, y;
            int iPixelCount = 0;
            int iPixelDist = 0;
            SectionInfo EdgeValues = new SectionInfo();

            for (x = Coords.Left; x < Coords.Right - 1; x++)
            {
                for (y = Coords.Top; y < Coords.Bottom - 1; y++)
                {
                    if (GetPixelDiff(BitmapBytes, x, y) > PIXEL_DIFF_TOLERENCE)
                    {
                        iPixelCount++;
                        iPixelDist += GetDistance(x - Coords.Left, y - Coords.Top);

                        if (bModifyImage)
                            BitmapBytes.SetPixel(x, y, 255, 255, 255);
                    }
                    else
                        if (bModifyImage)
                            BitmapBytes.SetPixel(x, y, 0, 0, 0);
                }
            }

            if (iPixelCount != 0 && iPixelDist != 0)
                iPixelDist /= iPixelCount;

            EdgeValues.ChangeCount = Convert.ToUInt16((float)iPixelCount / (float)GetPixelCount(Coords) * 100);
            int iMaxChange = GetDistance(Coords.Right - Coords.Left, Coords.Bottom - Coords.Top);
            EdgeValues.ChangeDistance = Convert.ToUInt16((float)iPixelDist / iMaxChange * 100);

            return EdgeValues;
        }

        /// <summary> Get the pythagorean distance between a point and the top left of a section (basically point 0,0) </summary>
        private int GetDistance(int x, int y)
        {
            return (int)Math.Sqrt((x * x) + (y * y));
        }

        private Coords GetSectionCoords(int iRow, int iCol, int iRows, int iCols, int iWidth, int iHeight)
        {
            Coords Coords;

            Coords.Left = iWidth / iCols * iCol;
            Coords.Right = iWidth / iCols * (iCol + 1) - 1;
            Coords.Top = iHeight / iRows * iRow;
            Coords.Bottom = iHeight / iRows * (iRow + 1) - 1;

            return Coords;
        }

        /// <summary> Calculate the colour histogram for this section of the image </summary>
        private SectionInfo GetHisto(Coords Coords, ImageHelper.BitmapBytes BitmapBytes)
        {
            int[] Hue = new int[_iNumBars];
            int x, y;

            SectionInfo SectionInfo = new SectionInfo();

            for (x = Coords.Left; x < Coords.Right - 1; x++)
                for (y = Coords.Top; y < Coords.Bottom - 1; y++)
                {
                    Color pixel = BitmapBytes.GetHSIFromPoint(x, y);
                    float fHue = pixel.GetHue();
                    int iIll = (int)(pixel.GetBrightness() * 100);
                    int iSat = (int)(pixel.GetSaturation() * 100);
                    bool bColour = false;

                    // Check for colour or black and white
                    // based on various illumination and saturation tolerances
                    if (iSat > 10)
                        if (iIll > 15 && iIll < 91)
                            if (((100 - iIll) + iSat) > 35)
                                if (iSat + iIll > 25)
                                {
                                    int iSegment = Convert.ToInt32(Math.Floor(fHue / HUE_MAX * (_iNumBars - 2)));
                                    Hue[iSegment]++;
                                    bColour = true;
                                }

                    // If this was b/w store either b or w based on the illumination
                    if (!bColour)
                        if (iIll > 50)
                            Hue[_iNumBars - 2] += 1;
                        else
                            Hue[_iNumBars - 1] += 1;
                }

            int iCount;
            int iPixelCount = (Coords.Bottom - Coords.Top + 1) * (Coords.Right - Coords.Left + 1);
            iPixelCount = GetPixelCount(Coords);

            for (iCount = 0; iCount < _iNumBars; iCount++)
            {
                float fPercent = (float)Hue[iCount] / iPixelCount * 100;
                Hue[iCount] = (int)fPercent;
            }

            SectionInfo.Hues = Hue;
            return SectionInfo;
        }

        /// <summary> Calculate a number representing the difference between it and the pixels around it
        /// basically a comparison of the r, g and b of 3 pixels to the bottom right
        /// This depends on the calling method cycling through all pixels from left to right and downwards,
        /// this prevents having to compare all eight surrounding pixels which would duplicate effort </summary>
        public int GetPixelDiff(ImageHelper.BitmapBytes BitmapBytes, int x, int y)
        {
            int iRGB;
            int iDiff = 0;

                for(iRGB = 0; iRGB < 3; iRGB++)
                {
                    // Middle Right
                    iDiff += System.Math.Abs(BitmapBytes.GetPixelPart(x, y, (RGB)iRGB) - BitmapBytes.GetPixelPart(x + 1, y, (RGB)iRGB));
                    // Bottom Right
                    iDiff += System.Math.Abs(BitmapBytes.GetPixelPart(x, y, (RGB)iRGB) - BitmapBytes.GetPixelPart(x + 1, y + 1, (RGB)iRGB));
                    // Bottom Middle
                    iDiff += System.Math.Abs(BitmapBytes.GetPixelPart(x, y, (RGB)iRGB) - BitmapBytes.GetPixelPart(x, y + 1, (RGB)iRGB));
                }

                return iDiff;
        }

        private int GetPixelCount(Coords Coords)
        {
            return (Coords.Bottom - Coords.Top + 1) * (Coords.Right - Coords.Left + 1);
        }

        /// <summary> Save Hue data to file - class is serialized as xml
        /// Dictionary object cannot be serialized so its converted to an array </summary>
        public void Save(string sFileName)
        {
            ImageInfo[] SaveList = new ImageInfo[_ImageList.Values.Count];
            _ImageList.Values.CopyTo(SaveList, 0);

            XmlSerializer serializer = new XmlSerializer(typeof(ImageInfo[]));
            TextWriter Writer = new StreamWriter(sFileName);
            serializer.Serialize(Writer, SaveList);
            Writer.Close();
        }

        /// <summary> Load Hue data from file - class is serialized as xml </summary>
        public void Load(string sFileName)
        {
            ImageInfo[] LoadList;

            XmlSerializer serializer = new XmlSerializer(typeof(ImageInfo[]));
            FileStream fs = new FileStream(sFileName, FileMode.Open);
            LoadList = (ImageInfo[])serializer.Deserialize(fs);
            fs.Close();

            foreach (ImageInfo ImageInfo in LoadList)
            {
                _ImageList.Add(ImageInfo.File, ImageInfo);
            }
        }
    }
}
