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
        bool _bBreak;
        int _iDupesSelectedIndex;

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
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            grdDupes.Visible = false;
            List<ImageSearch.Result> ResultList = _IR.Search(openFileDialog.FileName, 10, 500);
            GridLoadResults(ResultList, grdResult);
        }

        private void grdResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            string sFile = grdResult.Rows[e.RowIndex].Cells["FileName"].Value.ToString();

            if (grdResult.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show(Properties.Resources.DialogMessageImageDelete,
                                    "Remove Image", 
                                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    WinFormsUIHelper.SendFileToRecycleBin(sFile);
                    grdResult.Rows.RemoveAt(e.RowIndex);
                    
                    // If this was a duplicates search remove from the dupes collection as well
                    if(grdDupes.Visible)
                        _Results[_iDupesSelectedIndex].DupesList.RemoveAt(e.RowIndex);
                }
            }
            else
            {
                RunProgram(sFile);
            }
        }

        private void grdDupes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            _iDupesSelectedIndex = e.RowIndex;
            GridLoadResults(_Results[e.RowIndex].DupesList, grdResult);
        }

        private void btnDupes_Click(object sender, EventArgs e)
        {
            grdDupes.Visible = true;
            _Results = _IR.FindDuplicates();
            GridLoadDupes(_Results, grdDupes);
        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ctrl + Break key will interupt image loading
            if (e.KeyChar == 3)
                _bBreak = true;
        }

        # endregion

        #region SupportMethods

        private bool CheckForLoadedImages()
        {
            if (_IR.Count > 0)
                if (MessageBox.Show(Properties.Resources.DialogMessageResetLibrary,
                    "Exiting Images", MessageBoxButtons.YesNo) == DialogResult.No)
                    return true;
                else
                {
                    _IR.Clear();
                    grdDupes.Visible = false;
                    grdResult.Rows.Clear();
                    return false;
                }

            return false;
        }

        private void LoadImages(bool bSubDirectories)
        {
            if (CheckForLoadedImages())
                return;

            string[] sFileList = WinFormsUIHelper.GetFilesFromFolder(bSubDirectories);
            if (sFileList.Length == 0) return;
            sFileList = ImageHelper.ImageUtil.GetValidFiles(sFileList);
            if (sFileList.Length == 0) return;

            ProgressBar.Visible = true;
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = sFileList.Length - 1;

            _bBreak = false;

            for (int iCount = 0; iCount < sFileList.Length; iCount++)
            {
                if (_bBreak)
                {
                    // Load has been interupted
                    ImagesLoaded(iCount + 1);
                    return;
                }

                string sFile = sFileList[iCount];
                StatusBarLabel.Text = "Loading - " + sFile + " (Ctrl + Break to cancel)";
                ProgressBar.Value = iCount;
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

            Bitmap icon = Properties.Resources.delete_16;

            foreach (Result Result in ResultList)
            {
                Bitmap bmp = ImageHelper.ImageUtil.GetThumbnail(100, Result.File);

                grd.Rows.Add(new object[] { Result.Score, bmp, icon, Result.File });
            }
        }

        #endregion

        #region Menus

        private void loadLibraryMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForLoadedImages())
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.FileName = "";
            openFileDialog.Filter = "Library Files(*.xml)|*.xml";

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
            SaveFileDialog saveFileDialog = new SaveFileDialog();

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