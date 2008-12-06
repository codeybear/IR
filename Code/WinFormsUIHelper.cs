using System.IO;
using System.Windows.Forms;

namespace ImageSearch
{
    public class WinFormsUIHelper
    {
        /// <summary>
        /// Get a list of files from a folder based on the result of a FolderBrowserDialog
        /// </summary>
        /// <param name="bSubDirs">Include sub directories</param>
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
    }
}
