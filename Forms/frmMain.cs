using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ImageSearch.Properties;

namespace ImageSearch
{
    public partial class frmMain : Form
    {
        IR _IR;
        DupesList _Results;
        bool _bBreak;
        int _iDupesSelectedIndex;

        public frmMain()
        {
            InitializeComponent();
            _IR = new IR();
        }

        private void RunProgram(string sParam)
        {
            // TODO possibly have an option to enable the default viewer:
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //proc.EnableRaisingEvents = false;
            //proc.StartInfo.FileName = sParam;
            //proc.Start();

            frmShowImage frmShowImage = new frmShowImage(sParam);
            frmShowImage.Show();
            
        }

        #region ControlEvents

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                    return;

                splitContainer.Panel1Collapsed = true;
                splitContainer.Visible = true;

                grdDupes.Visible = false;
                List<ImageSearch.Result> ResultList = _IR.Search(openFileDialog.FileName, 10, 500);
                GridLoadResults(ResultList, grdResult);
            }
            catch(Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void grdResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;

                string sFile = grdResult.Rows[e.RowIndex].Cells["FileName"].Value.ToString();

                if (grdResult.Columns[e.ColumnIndex].Name == "Delete")
                {
                    if (MessageBox.Show(Properties.Resources.DialogMessageImageDelete,
                                        "Remove Image",
                                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Utils.SendFileToRecycleBin(sFile);
                        grdResult.Rows.RemoveAt(e.RowIndex);
                        _IR.DeleteImage(sFile);
                        // If this was a duplicates search remove from the dupes collection as well
                        if (grdDupes.Visible)
                            _Results[_iDupesSelectedIndex].ResultList.RemoveAt(e.RowIndex);
                    }
                }
                else
                {
                    RunProgram(sFile);
                }
            }
            catch(Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void grdDupes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;
                _iDupesSelectedIndex = e.RowIndex;
                GridLoadResults(_Results[e.RowIndex].ResultList, grdResult);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }

        }

        private void btnDupes_Click(object sender, EventArgs e)
        {
            try
            {
                splitContainer.Panel1Collapsed = false;
                grdDupes.Visible = true;
                _Results = _IR.FindDuplicates();
                GridLoadDupes(_Results, grdDupes);
                grdResult.Rows.Clear();
                splitContainer.Visible = true;
            }
            catch(Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
            // Ctrl + Break key will interupt image loading
            if (e.KeyChar == 3)
                _bBreak = true;
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ImageSearch.WindowSettings.Record(this, splitContainer);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                ImageSearch.WindowSettings.Restore(this, splitContainer);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        # endregion

        #region SupportMethods

        /// <summary> Check to see if any images have already been loaded
        /// User can choose to cancel or continue and overight </summary>
        /// <returns>true = continue and clear loaded images</returns>
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

        /// <summary> Load in images from folder selected by the user
        /// <param name="bSubDirectories">If true load from sub directories</param>
        private void LoadImages(bool bSubDirectories)
        {
            if (CheckForLoadedImages())
                return;

            List<string> FileList = Utils.GetFilesFromFolder(bSubDirectories);
            if (FileList.Count == 0) return;
            FileList = Utils.GetValidFiles(FileList, Settings.Default.ValidImageExtensions);
            if (FileList.Count == 0) return;
            
            ProgressBar.Visible = true;
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = FileList.Count - 1;

            _bBreak = false;

            for (int iCount = 0; iCount < FileList.Count; iCount++)
            {
                if (_bBreak)
                {
                    // Load has been interrupted
                    ImagesLoaded(iCount + 1);
                    return;
                }

                string sFile = FileList[iCount];
                StatusBarLabel.Text = "Loading - " + sFile + " (Ctrl + Break to cancel)";
                ProgressBar.Value = iCount;
                Application.DoEvents();

                try {
                    _IR.LoadImage(sFile);
                }
                catch (Exception ex) {
                    Utils.HandleException(ex);
                }
                
            }

            ImagesLoaded(_IR.Count);
        }

        /// <summary> Sets up the UI for when images have been loaded
        /// <param name="iNumLoaded">Total images that have been loaded</param>
        private void ImagesLoaded(int iNumLoaded)
        {
            ProgressBar.Visible = false;
            StatusBarLabel.Text = iNumLoaded + " Images Loaded";
            saveLibraryMenuItem.Enabled = true;
            btnDupes.Enabled = true;
            btnSearch.Enabled = true;
        }

        /// <summary> Load images into DataGridView </summary>
        private void GridLoadDupes(IEnumerable<Dupes> DupesList, DataGridView grd)
        {
            grd.Rows.Clear();
            Bitmap bmpThumb;

            foreach (Dupes Dupe in DupesList)
            {
                bmpThumb = ImageHelper.ImageUtil.GetThumbnail(Properties.Settings.Default.ThumbNailHeight, Dupe.File);

                grd.Rows.Add(new object[] { bmpThumb, Dupe.File });
            }
        }

        /// <summary> Load search results into DataGridView </summary>
        private void GridLoadResults(IEnumerable<Result> ResultList, DataGridView grd)
        {
            grd.Rows.Clear();

            Bitmap icon = Properties.Resources.bmpDelete;
            Bitmap bmpThumb;

            foreach (Result Result in ResultList)
            {
                bmpThumb = ImageHelper.ImageUtil.GetThumbnail(Properties.Settings.Default.ThumbNailHeight, Result.File);

                string sScore = Result.Info.Substring(0,Result.Info.IndexOf("/"));

                grd.Rows.Add(new object[] { sScore, bmpThumb, icon, Result.File });
            }
        }

        #endregion

        #region Menus

        private void loadLibraryMenuItem_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void loadImagesMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                LoadImages(false);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void saveLibraryMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.DefaultExt = "xml";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                    return;

                _IR.Save(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
        }

        private void loadImagesIncSubdirectoriesMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                LoadImages(true);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
            }
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