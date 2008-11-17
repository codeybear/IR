using System.IO;
using System.Windows.Forms;

namespace ImageSearch
{
    public class WinFormsUIHelper
    {
        public static string[] GetFilesFromFolder(bool bSubDirs)
        {
            FolderBrowserDialog dlgFolder = new FolderBrowserDialog();
            dlgFolder.SelectedPath = "";

            if (dlgFolder.ShowDialog() == DialogResult.Cancel)
                return new string[0];

            string sDir = dlgFolder.SelectedPath;

            string[] sFileList;

            if (bSubDirs)
                sFileList = Directory.GetFiles(sDir, "*.*", SearchOption.AllDirectories);
            else
                sFileList = Directory.GetFiles(sDir);

            return sFileList;
        }

        /// <summary>
        /// Send a file to the recycle bin
        /// c# has no call for this so VB method is used (rather than API call).
        /// </summary>
        public static void SendFileToRecycleBin(string sFile)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                sFile,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }


        public static void DeleteRowFromGrid(DataGridView grd, string sColumn, string sValue)
        {
            for (int iRow = 0; iRow < grd.Rows.Count; iRow++)
                if (grd.Rows[iRow].Cells[sColumn].Value.ToString() == sValue)
                    grd.Rows.RemoveAt(iRow);
        }

    }
}
