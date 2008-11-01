using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageSearch
{
    public partial class frmMain : Form
    {
        IR _IR;
        List<Dupes> _Results;

        public frmMain()
        {
            InitializeComponent();
            _IR = new IR();
        }

        private void RunProgram(string sParam)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = sParam;
            proc.Start();
        }

        #region ControlEvents

        private void frmMain_Load(object sender, EventArgs e)
        { }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            imgTest.Load(openFileDialog.FileName);
            List<ImageSearch.Result>  ResultList = _IR.Search(openFileDialog.FileName, 10, 0);
            GridLoadResults(ResultList, grdResult);
        }

        private void grdResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sFile = grdResult.Rows[e.RowIndex].Cells["FileName"].Value.ToString();
            RunProgram(sFile);
        }

        private void grdDupes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GridLoadResults(_Results[e.RowIndex].DupesList, grdResult);
        }

        private void btnDupes_Click(object sender, EventArgs e)
        {
            
            _Results = _IR.FindDuplicates();
            GridLoadDupes(_Results, grdDupes);
        }

        # endregion

        #region SupportMethods

        private bool CheckForLoadedImages()
        {
            if (_IR.Count > 0)
                if (MessageBox.Show("You have images loaded are you sure you wish to continue and overwrite these?",
                    "Exiting Images", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;

            return true;
        }

        private void LoadImages(bool bSubDirectories)
        {
            if (CheckForLoadedImages())
                _IR.Clear();
            else
                return;

            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
                return;

            string sDir = folderBrowserDialog.SelectedPath;
            string[] sFileList;

            if (bSubDirectories)
                sFileList = Directory.GetFiles(sDir, "*.jpg", SearchOption.AllDirectories);
            else
                sFileList = Directory.GetFiles(sDir, "*.jpg");

            if (sFileList.Length == 0) return;

            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = sFileList.Length - 1;

            for (int iCount = 0; iCount < sFileList.Length; iCount++)
            {
                string sFile = sFileList[iCount];
                StatusBarLabel.Text = "Loading - " + sFile;
                ProgressBar.Value = iCount;
                imgTest.Load(sFile);
                Application.DoEvents();

                _IR.LoadImage(sFile);
            }

            ImagesLoaded(_IR.Count);
        }

        private void ImagesLoaded(int iNumLoaded)
        {
            ProgressBar.Visible = false;
            StatusBarLabel.Text = iNumLoaded + " Images Loaded";
            saveLibraryMenuItem.Enabled = true;
            btnDupes.Enabled = true;
            btnSearch.Enabled = true;
        }

        private void GridLoadDupes(IEnumerable<Dupes> DupesList, DataGridView grd)
        {
            grd.Rows.Clear();

            foreach (Dupes Dupe in DupesList)
            {
                Bitmap bmp = ImageHelper.ImageUtil.GetThumbnail(100, Dupe.File);

                grd.Rows.Add(new object[] { bmp, Dupe.File });
            }
        }

        private void GridLoadResults(IEnumerable<Result> ResultList, DataGridView grd)
        {
            grd.Rows.Clear();

            foreach (Result Result in ResultList)
            {
                Bitmap bmp = ImageHelper.ImageUtil.GetThumbnail(100, Result.File);

                grd.Rows.Add(new object[] { Result.Score, bmp, Result.File });
            }
        }

        #endregion

        #region Menus

        private void loadLibraryMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForLoadedImages())
                _IR.Clear();
            else
                return;

            openFileDialog.FileName = "";

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            _IR.Load(openFileDialog.FileName);

            ImagesLoaded(_IR.Count);
        }

        private void loadImagesMenuItem_Click(object sender, EventArgs e)
        {
            LoadImages(false);
        }

        private void saveLibraryMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            _IR.Save(saveFileDialog.FileName);
        }

        private void loadImagesIncSubdirectoriesMenuItem_Click(object sender, EventArgs e)
        {
            LoadImages(true);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void abToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAboutBox frmAbout = new frmAboutBox();
            frmAbout.Show();
        }

#endregion

    }
}