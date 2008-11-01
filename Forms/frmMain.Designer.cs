namespace ImageSearch
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.imgTest = new System.Windows.Forms.PictureBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grdResult = new System.Windows.Forms.DataGridView();
            this.Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Image = new System.Windows.Forms.DataGridViewImageColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadImagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadImagesIncSubdirectoriesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLibraryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLibraryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnDupes = new System.Windows.Forms.Button();
            this.grdDupes = new System.Windows.Forms.DataGridView();
            this.DupeImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.File = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.imgTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdResult)).BeginInit();
            this.StatusBar.SuspendLayout();
            this.menuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDupes)).BeginInit();
            this.SuspendLayout();
            // 
            // imgTest
            // 
            this.imgTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.imgTest.Location = new System.Drawing.Point(12, 65);
            this.imgTest.Name = "imgTest";
            this.imgTest.Size = new System.Drawing.Size(128, 208);
            this.imgTest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgTest.TabIndex = 0;
            this.imgTest.TabStop = false;
            // 
            // btnSearch
            // 
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(12, 36);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grdResult
            // 
            this.grdResult.AllowUserToAddRows = false;
            this.grdResult.AllowUserToDeleteRows = false;
            this.grdResult.AllowUserToResizeColumns = false;
            this.grdResult.AllowUserToResizeRows = false;
            this.grdResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdResult.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.grdResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Score,
            this.Image,
            this.FileName});
            this.grdResult.Location = new System.Drawing.Point(371, 36);
            this.grdResult.MultiSelect = false;
            this.grdResult.Name = "grdResult";
            this.grdResult.ReadOnly = true;
            this.grdResult.Size = new System.Drawing.Size(259, 587);
            this.grdResult.TabIndex = 5;
            this.grdResult.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdResult_CellClick);
            // 
            // Score
            // 
            this.Score.HeaderText = "Score";
            this.Score.Name = "Score";
            this.Score.ReadOnly = true;
            // 
            // Image
            // 
            this.Image.HeaderText = "Image";
            this.Image.Name = "Image";
            this.Image.ReadOnly = true;
            // 
            // FileName
            // 
            this.FileName.HeaderText = "Hidden";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Visible = false;
            // 
            // StatusBar
            // 
            this.StatusBar.AutoSize = false;
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar,
            this.StatusBarLabel});
            this.StatusBar.Location = new System.Drawing.Point(0, 638);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(642, 23);
            this.StatusBar.Stretch = false;
            this.StatusBar.TabIndex = 6;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(100, 17);
            this.ProgressBar.Visible = false;
            // 
            // StatusBarLabel
            // 
            this.StatusBarLabel.Name = "StatusBarLabel";
            this.StatusBarLabel.Size = new System.Drawing.Size(0, 18);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.libraryToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(642, 24);
            this.menuMain.TabIndex = 7;
            this.menuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(114, 22);
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.abToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(111, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(114, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // libraryToolStripMenuItem
            // 
            this.libraryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadImagesMenuItem,
            this.loadImagesIncSubdirectoriesMenuItem,
            this.loadLibraryMenuItem,
            this.saveLibraryMenuItem});
            this.libraryToolStripMenuItem.Name = "libraryToolStripMenuItem";
            this.libraryToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.libraryToolStripMenuItem.Text = "Library";
            // 
            // loadImagesMenuItem
            // 
            this.loadImagesMenuItem.Name = "loadImagesMenuItem";
            this.loadImagesMenuItem.Size = new System.Drawing.Size(233, 22);
            this.loadImagesMenuItem.Text = "Load Images";
            this.loadImagesMenuItem.Click += new System.EventHandler(this.loadImagesMenuItem_Click);
            // 
            // loadImagesIncSubdirectoriesMenuItem
            // 
            this.loadImagesIncSubdirectoriesMenuItem.Name = "loadImagesIncSubdirectoriesMenuItem";
            this.loadImagesIncSubdirectoriesMenuItem.Size = new System.Drawing.Size(233, 22);
            this.loadImagesIncSubdirectoriesMenuItem.Text = "Load Images inc Subdirectories";
            this.loadImagesIncSubdirectoriesMenuItem.Click += new System.EventHandler(this.loadImagesIncSubdirectoriesMenuItem_Click);
            // 
            // loadLibraryMenuItem
            // 
            this.loadLibraryMenuItem.Name = "loadLibraryMenuItem";
            this.loadLibraryMenuItem.Size = new System.Drawing.Size(233, 22);
            this.loadLibraryMenuItem.Text = "Load Library";
            this.loadLibraryMenuItem.Click += new System.EventHandler(this.loadLibraryMenuItem_Click);
            // 
            // saveLibraryMenuItem
            // 
            this.saveLibraryMenuItem.Enabled = false;
            this.saveLibraryMenuItem.Name = "saveLibraryMenuItem";
            this.saveLibraryMenuItem.Size = new System.Drawing.Size(233, 22);
            this.saveLibraryMenuItem.Text = "Save Library";
            this.saveLibraryMenuItem.Click += new System.EventHandler(this.saveLibraryMenuItem_Click);
            // 
            // btnDupes
            // 
            this.btnDupes.Enabled = false;
            this.btnDupes.Location = new System.Drawing.Point(94, 35);
            this.btnDupes.Name = "btnDupes";
            this.btnDupes.Size = new System.Drawing.Size(97, 23);
            this.btnDupes.TabIndex = 8;
            this.btnDupes.Text = "Find Duplicates";
            this.btnDupes.UseVisualStyleBackColor = true;
            this.btnDupes.Click += new System.EventHandler(this.btnDupes_Click);
            // 
            // grdDupes
            // 
            this.grdDupes.AllowUserToAddRows = false;
            this.grdDupes.AllowUserToDeleteRows = false;
            this.grdDupes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grdDupes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.grdDupes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDupes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DupeImage,
            this.File});
            this.grdDupes.Location = new System.Drawing.Point(12, 65);
            this.grdDupes.Name = "grdDupes";
            this.grdDupes.ReadOnly = true;
            this.grdDupes.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdDupes.Size = new System.Drawing.Size(301, 558);
            this.grdDupes.TabIndex = 9;
            this.grdDupes.Visible = false;
            this.grdDupes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDupes_CellClick);
            // 
            // DupeImage
            // 
            this.DupeImage.HeaderText = "Image";
            this.DupeImage.Name = "DupeImage";
            this.DupeImage.ReadOnly = true;
            // 
            // File
            // 
            this.File.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.File.DefaultCellStyle = dataGridViewCellStyle1;
            this.File.HeaderText = "File";
            this.File.Name = "File";
            this.File.ReadOnly = true;
            this.File.Width = 48;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 661);
            this.Controls.Add(this.grdDupes);
            this.Controls.Add(this.btnDupes);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.grdResult);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.imgTest);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuMain;
            this.Name = "frmMain";
            this.Text = "Image Search";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmMain_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.imgTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdResult)).EndInit();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDupes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgTest;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView grdResult;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Score;
        private System.Windows.Forms.DataGridViewImageColumn Image;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadImagesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLibraryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLibraryMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarLabel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem loadImagesIncSubdirectoriesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnDupes;
        private System.Windows.Forms.DataGridView grdDupes;
        private System.Windows.Forms.DataGridViewImageColumn DupeImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn File;
    }
}
