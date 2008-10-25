using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ImageSearch
{
    public struct ImageInfo
    {
        public string File { get; set; }
        public SectionInfo[] Section { get; set;}
    }

    public struct SectionInfo
    {
        public int[] Hues { get; set; }
    }

    public struct Result
    {
        public string File { get; set; }
        public int Score { get; set; }
    }

    public struct Dupes
    {
        public string File { get; set; }
        public List<Result> DupesList { get; set; }
    }

    public struct Coords
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    public class IR
    {
        public int _iNumBars = 12;
        const int HUE_MAX = 360;
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

        public int NumBars
        {
            get { return _iNumBars; }
            set { _iNumBars = value; }
        }

        public void LoadImage(string sFile)
        {
            _ImageList.Add(sFile, GetSignature(sFile));
        }

        public void Clear()
        {
            _ImageList = new Dictionary<String,ImageInfo>();
        }

        public int Count
        {
            get { return _ImageList.Count; }
        }

        /// <summary>
        /// Find duplicates within the currectly loaded images
        /// </summary>
        public List<Dupes> FindDuplicates()
        {
            List<Dupes> Dupes = new List<Dupes>();

            foreach (string sKey in _ImageList.Keys)
            {
                ImageInfo ThisImage = _ImageList[sKey];

                List<Result> Results = Search(ThisImage, 0, 100);

                // Search results will always contain 1 result as the search image will be found in the results
                if (Results.Count > 1)
                {
                    // Remove the search image
                    int iIndex = Results.FindIndex(Item => Item.File == ThisImage.File);
                    Results.RemoveAt(iIndex);
                    
                    Dupes.Add(new Dupes { File = ThisImage.File, DupesList = Results });
                }
            }

            return Dupes;
        }

        /// <summary>
        /// Note: OverLoaded Method
        /// 
        /// Search for an image within the loaded list of images
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

                int iScore = 0;

                for (int iSection = 0; iSection < _iGridSize - 1; iSection++)
                {
                    iScore += Compare(SearchImage.Section[iSection], ThisImage.Section[iSection]);
                }

                if (iScore <= iTopScore || iTopScore == 0)
                    ResultList.Add(new Result { File = ThisImage.File, Score = iScore });
            }

            ResultList.Sort(delegate(Result r1, Result r2)
                { return r1.Score.CompareTo(r2.Score); });

            if(iTop > 0 && iTop < ResultList.Count)
                ResultList = ResultList.GetRange(0, iTop);

            return ResultList;
        }

        public List<Result> Search(string sFile, int iTop, int iTopScore)
        {
            ImageInfo SearchImageInfo = GetSignature(sFile);

            return Search(SearchImageInfo, iTop, iTopScore);
        }

        private int Compare(SectionInfo Section1, SectionInfo Section2)
        {
            int iCount;
            int iScore = 0;

            for (iCount = 0; iCount < _iNumBars; iCount++)
            {
                iScore += Math.Abs(Section1.Hues[iCount] - Section2.Hues[iCount]);
            }

            return iScore;
        }

        public ImageInfo GetSignature(string sFile)
        {
            ImageInfo ImageInfo = new ImageInfo { File = sFile, Section = new SectionInfo[_iGridSize] };
            Bitmap bmp = new Bitmap(sFile);
            ImageHelper.BitmapBytes BitmapBytes = new ImageHelper.BitmapBytes(bmp);

            DateTime dt = DateTime.Now;

            for (int iRow = 0; iRow < _iRows; iRow++)
                for (int iCol = 0; iCol < _iCols; iCol++)
                {
                    Coords Coords = GetSectionCoords(iRow, iCol, _iRows, _iCols, bmp.Width, bmp.Height);
                    int iSection = (iRow * _iCols) + iCol;
                    ImageInfo.Section[iSection] = GetHisto(Coords, BitmapBytes);
                }

            TimeSpan duration = DateTime.Now - dt;
            Console.WriteLine("GetSignature Duration: " + duration.ToString());
                
            return ImageInfo;
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

        /// <summary>
        /// Calculate the colour histogram for this section of the image
        /// </summary>
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
                    if (iSat > 13)
                        if (iIll > 15 && iIll < 91)
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

            for (iCount = 0; iCount < _iNumBars; iCount++)
            {
                float fPercent = (float)Hue[iCount] / iPixelCount * 100;
                Hue[iCount] = (int)fPercent;
            }

            SectionInfo.Hues = Hue;
            return SectionInfo;
        }

        /// <summary>
        /// Save Hue data to file - class is serialized as xml.
        /// </summary>
        /// <param name="sFileName"></param>
        public void Save(string sFileName)
        {
            ImageInfo[] SaveList = new ImageInfo[_ImageList.Values.Count];
            _ImageList.Values.CopyTo(SaveList, 0);

            XmlSerializer serializer = new XmlSerializer(typeof(ImageInfo[]));
            TextWriter Writer = new StreamWriter(sFileName);
            serializer.Serialize(Writer, SaveList);
            Writer.Close();
        }

        /// <summary>
        /// Load Hue data from file - class is serialized as xml.
        /// </summary>
        /// <param name="sFileName"></param>
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
