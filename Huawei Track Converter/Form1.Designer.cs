namespace Huawei_Track_Converter
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.directoryEntry1 = new System.DirectoryServices.DirectoryEntry();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.lblDistance = new System.Windows.Forms.Label();
            this.lblAscent = new System.Windows.Forms.Label();
            this.lblDescent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.listBoxExportFormat = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.listBoxActivity = new System.Windows.Forms.ListBox();
            this.lblActivityType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "file.png");
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(2, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 32);
            this.button1.TabIndex = 4;
            this.button1.Text = "Choose Folder";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(421, 68);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(614, 351);
            this.gMapControl1.TabIndex = 5;
            this.gMapControl1.Zoom = 0D;
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.Location = new System.Drawing.Point(2, 42);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(413, 446);
            this.listBoxFiles.TabIndex = 6;
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(421, 42);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(54, 13);
            this.lblDistance.TabIndex = 7;
            this.lblDistance.Text = "Distance:";
            // 
            // lblAscent
            // 
            this.lblAscent.AutoSize = true;
            this.lblAscent.Location = new System.Drawing.Point(574, 42);
            this.lblAscent.Name = "lblAscent";
            this.lblAscent.Size = new System.Drawing.Size(44, 13);
            this.lblAscent.TabIndex = 8;
            this.lblAscent.Text = "Ascent:";
            // 
            // lblDescent
            // 
            this.lblDescent.AutoSize = true;
            this.lblDescent.Location = new System.Drawing.Point(725, 42);
            this.lblDescent.Name = "lblDescent";
            this.lblDescent.Size = new System.Drawing.Size(51, 13);
            this.lblDescent.TabIndex = 9;
            this.lblDescent.Text = "Descent:";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(876, 42);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(56, 13);
            this.lblDuration.TabIndex = 10;
            this.lblDuration.Text = "Duration:";
            // 
            // listBoxExportFormat
            // 
            this.listBoxExportFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxExportFormat.FormattingEnabled = true;
            this.listBoxExportFormat.Items.AddRange(new object[] {
            "GPX",
            "TCX"});
            this.listBoxExportFormat.Location = new System.Drawing.Point(427, 458);
            this.listBoxExportFormat.Name = "listBoxExportFormat";
            this.listBoxExportFormat.Size = new System.Drawing.Size(135, 30);
            this.listBoxExportFormat.TabIndex = 11;
            this.listBoxExportFormat.SelectedIndexChanged += new System.EventHandler(this.listBoxExportFormat_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(424, 442);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Export Format:";
            // 
            // btnExport
            // 
            this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(427, 491);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(135, 32);
            this.btnExport.TabIndex = 13;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // listBoxActivity
            // 
            this.listBoxActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxActivity.FormattingEnabled = true;
            this.listBoxActivity.Items.AddRange(new object[] {
            "Running",
            "Biking",
            "Other ",
            "MultiSport"});
            this.listBoxActivity.Location = new System.Drawing.Point(577, 458);
            this.listBoxActivity.Name = "listBoxActivity";
            this.listBoxActivity.Size = new System.Drawing.Size(135, 56);
            this.listBoxActivity.TabIndex = 14;
            this.listBoxActivity.Visible = false;
            // 
            // lblActivityType
            // 
            this.lblActivityType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblActivityType.AutoSize = true;
            this.lblActivityType.Location = new System.Drawing.Point(574, 442);
            this.lblActivityType.Name = "lblActivityType";
            this.lblActivityType.Size = new System.Drawing.Size(71, 13);
            this.lblActivityType.TabIndex = 15;
            this.lblActivityType.Text = "Activity Type:";
            this.lblActivityType.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 535);
            this.Controls.Add(this.lblActivityType);
            this.Controls.Add(this.listBoxActivity);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxExportFormat);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblDescent);
            this.Controls.Add(this.lblAscent);
            this.Controls.Add(this.lblDistance);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Huawei Track Converter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.DirectoryServices.DirectoryEntry directoryEntry1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button button1;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label lblAscent;
        private System.Windows.Forms.Label lblDescent;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.ListBox listBoxExportFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ListBox listBoxActivity;
        private System.Windows.Forms.Label lblActivityType;
    }
}

