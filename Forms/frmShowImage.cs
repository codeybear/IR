using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageSearch
{
    public partial class frmShowImage : Form
    {
        string _sFile;

        /// <summary>
        /// Display the image
        /// </summary>
        /// <param name="sFile">Image file and path</param>
        public frmShowImage(string sFile)
        {
            InitializeComponent();

            _sFile = sFile;
            picImage.Image = new Bitmap(_sFile);
            this.Text = _sFile;
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            IR ir = new IR();

            picImage.Image = ir.GetTestImage(_sFile);
        }
    }
}
