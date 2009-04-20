using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ImageSearch
{
    public class Utils
    {
        /// <summary>
        /// Get a list of files from a folder based on the result of a FolderBrowserDialog
        /// </summary>
        /// <param name="bSubDirs">Include sub directories</param>
        public static List<string> GetFilesFromFolder(bool bSubDirs)
        {
            FolderBrowserDialog dlgFolder = new FolderBrowserDialog();
            dlgFolder.SelectedPath = "";

            if (dlgFolder.ShowDialog() == DialogResult.Cancel)
                return new List<string>();

            string sDir = dlgFolder.SelectedPath;

            string[] sFileList;

            if (bSubDirs)
                sFileList = Directory.GetFiles(sDir, "*.*", SearchOption.AllDirectories);
            else
                sFileList = Directory.GetFiles(sDir);

            return new List<string>(sFileList);
        }

        /// <summary> Restrict a list of files to valid extensions only </summary>
        /// <param name="sValidExt">Delimited list of file extensions</param>
        public static List<string> GetValidFiles(List<string> FileList, string sValidExt)
        {
            return FileList.FindAll(sFileName => sValidExt.Contains(new FileInfo(sFileName).Extension.ToLower()));
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

        /// <summary>
        /// Deal with unhandled exceptions
        /// </summary>
        public static void HandleException(System.Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }
}
