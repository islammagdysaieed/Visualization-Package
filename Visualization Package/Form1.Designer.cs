namespace VisualizationPackage
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
            this.loadFileButton = new System.Windows.Forms.Button();
            this.simpleOpenGlControl1 = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mappingMode_combo = new System.Windows.Forms.ComboBox();
            this.colorsPanel = new System.Windows.Forms.Panel();
            this.coloringModeGroupBox = new System.Windows.Forms.GroupBox();
            this.edgeColoringRadio = new System.Windows.Forms.RadioButton();
            this.faceColoringRadio = new System.Windows.Forms.RadioButton();
            this.defaultColoringRadio = new System.Windows.Forms.RadioButton();
            this.dataName_ListBox = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.zAxisRadio = new System.Windows.Forms.RadioButton();
            this.yAxisRadio = new System.Windows.Forms.RadioButton();
            this.xAxisRadio = new System.Windows.Forms.RadioButton();
            this.minTextBox = new System.Windows.Forms.TextBox();
            this.maxTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.contouringGroupBox = new System.Windows.Forms.GroupBox();
            this.isosurfaceButton = new System.Windows.Forms.RadioButton();
            this.floodedContourRadioBtn = new System.Windows.Forms.RadioButton();
            this.lineContourRadioBtn = new System.Windows.Forms.RadioButton();
            this.noContourRadioBtn = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.num_Contours = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.twoDRadioBtn = new System.Windows.Forms.RadioButton();
            this.threeDRadioBtn = new System.Windows.Forms.RadioButton();
            this.coloringModeGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contouringGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadFileButton
            // 
            this.loadFileButton.Location = new System.Drawing.Point(1706, 35);
            this.loadFileButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.loadFileButton.Name = "loadFileButton";
            this.loadFileButton.Size = new System.Drawing.Size(184, 44);
            this.loadFileButton.TabIndex = 0;
            this.loadFileButton.Text = "Load File";
            this.loadFileButton.UseVisualStyleBackColor = true;
            this.loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // simpleOpenGlControl1
            // 
            this.simpleOpenGlControl1.AccumBits = ((byte)(0));
            this.simpleOpenGlControl1.AutoCheckErrors = false;
            this.simpleOpenGlControl1.AutoFinish = false;
            this.simpleOpenGlControl1.AutoMakeCurrent = true;
            this.simpleOpenGlControl1.AutoSwapBuffers = true;
            this.simpleOpenGlControl1.BackColor = System.Drawing.Color.Black;
            this.simpleOpenGlControl1.ColorBits = ((byte)(32));
            this.simpleOpenGlControl1.DepthBits = ((byte)(16));
            this.simpleOpenGlControl1.Location = new System.Drawing.Point(22, 21);
            this.simpleOpenGlControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
            this.simpleOpenGlControl1.Size = new System.Drawing.Size(1642, 1159);
            this.simpleOpenGlControl1.StencilBits = ((byte)(0));
            this.simpleOpenGlControl1.TabIndex = 2;
            this.simpleOpenGlControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.simpleOpenGlControl1_Paint);
            this.simpleOpenGlControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.simpleOpenGlControl1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1700, 286);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Data Name :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1700, 616);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 25);
            this.label3.TabIndex = 8;
            this.label3.Text = "Mapping Method :";
            // 
            // mappingMode_combo
            // 
            this.mappingMode_combo.AllowDrop = true;
            this.mappingMode_combo.FormattingEnabled = true;
            this.mappingMode_combo.Location = new System.Drawing.Point(1706, 645);
            this.mappingMode_combo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mappingMode_combo.Name = "mappingMode_combo";
            this.mappingMode_combo.Size = new System.Drawing.Size(222, 33);
            this.mappingMode_combo.TabIndex = 7;
            this.mappingMode_combo.SelectedIndexChanged += new System.EventHandler(this.mappingMode_combo_SelectedIndexChanged);
            // 
            // colorsPanel
            // 
            this.colorsPanel.Location = new System.Drawing.Point(1706, 447);
            this.colorsPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.colorsPanel.Name = "colorsPanel";
            this.colorsPanel.Size = new System.Drawing.Size(469, 96);
            this.colorsPanel.TabIndex = 9;
            this.colorsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.colorsPanel_Paint);
            // 
            // coloringModeGroupBox
            // 
            this.coloringModeGroupBox.Controls.Add(this.edgeColoringRadio);
            this.coloringModeGroupBox.Controls.Add(this.faceColoringRadio);
            this.coloringModeGroupBox.Controls.Add(this.defaultColoringRadio);
            this.coloringModeGroupBox.Location = new System.Drawing.Point(1705, 715);
            this.coloringModeGroupBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.coloringModeGroupBox.Name = "coloringModeGroupBox";
            this.coloringModeGroupBox.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.coloringModeGroupBox.Size = new System.Drawing.Size(400, 179);
            this.coloringModeGroupBox.TabIndex = 10;
            this.coloringModeGroupBox.TabStop = false;
            this.coloringModeGroupBox.Text = "Coloring Mode";
            // 
            // edgeColoringRadio
            // 
            this.edgeColoringRadio.AutoSize = true;
            this.edgeColoringRadio.Location = new System.Drawing.Point(42, 125);
            this.edgeColoringRadio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.edgeColoringRadio.Name = "edgeColoringRadio";
            this.edgeColoringRadio.Size = new System.Drawing.Size(179, 29);
            this.edgeColoringRadio.TabIndex = 2;
            this.edgeColoringRadio.Text = "Edge Coloring";
            this.edgeColoringRadio.UseVisualStyleBackColor = true;
            this.edgeColoringRadio.CheckedChanged += new System.EventHandler(this.edgeColoringRadio_CheckedChanged);
            // 
            // faceColoringRadio
            // 
            this.faceColoringRadio.AutoSize = true;
            this.faceColoringRadio.Location = new System.Drawing.Point(42, 81);
            this.faceColoringRadio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.faceColoringRadio.Name = "faceColoringRadio";
            this.faceColoringRadio.Size = new System.Drawing.Size(177, 29);
            this.faceColoringRadio.TabIndex = 1;
            this.faceColoringRadio.Text = "Face Coloring";
            this.faceColoringRadio.UseVisualStyleBackColor = true;
            this.faceColoringRadio.CheckedChanged += new System.EventHandler(this.faceColoringRadio_CheckedChanged);
            // 
            // defaultColoringRadio
            // 
            this.defaultColoringRadio.AutoSize = true;
            this.defaultColoringRadio.Checked = true;
            this.defaultColoringRadio.Location = new System.Drawing.Point(42, 37);
            this.defaultColoringRadio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.defaultColoringRadio.Name = "defaultColoringRadio";
            this.defaultColoringRadio.Size = new System.Drawing.Size(111, 29);
            this.defaultColoringRadio.TabIndex = 0;
            this.defaultColoringRadio.TabStop = true;
            this.defaultColoringRadio.Text = "Default";
            this.defaultColoringRadio.UseVisualStyleBackColor = true;
            this.defaultColoringRadio.CheckedChanged += new System.EventHandler(this.defaultColoringRadio_CheckedChanged);
            // 
            // dataName_ListBox
            // 
            this.dataName_ListBox.FormattingEnabled = true;
            this.dataName_ListBox.ItemHeight = 25;
            this.dataName_ListBox.Location = new System.Drawing.Point(1706, 317);
            this.dataName_ListBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dataName_ListBox.Name = "dataName_ListBox";
            this.dataName_ListBox.Size = new System.Drawing.Size(238, 104);
            this.dataName_ListBox.TabIndex = 11;
            this.dataName_ListBox.SelectedIndexChanged += new System.EventHandler(this.dataName_ListBox_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.zAxisRadio);
            this.groupBox2.Controls.Add(this.yAxisRadio);
            this.groupBox2.Controls.Add(this.xAxisRadio);
            this.groupBox2.Location = new System.Drawing.Point(1705, 163);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Size = new System.Drawing.Size(401, 92);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rotation Axis";
            // 
            // zAxisRadio
            // 
            this.zAxisRadio.AutoSize = true;
            this.zAxisRadio.Location = new System.Drawing.Point(268, 37);
            this.zAxisRadio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.zAxisRadio.Name = "zAxisRadio";
            this.zAxisRadio.Size = new System.Drawing.Size(56, 29);
            this.zAxisRadio.TabIndex = 2;
            this.zAxisRadio.TabStop = true;
            this.zAxisRadio.Text = "Z";
            this.zAxisRadio.UseVisualStyleBackColor = true;
            // 
            // yAxisRadio
            // 
            this.yAxisRadio.AutoSize = true;
            this.yAxisRadio.Location = new System.Drawing.Point(158, 37);
            this.yAxisRadio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.yAxisRadio.Name = "yAxisRadio";
            this.yAxisRadio.Size = new System.Drawing.Size(58, 29);
            this.yAxisRadio.TabIndex = 1;
            this.yAxisRadio.TabStop = true;
            this.yAxisRadio.Text = "Y";
            this.yAxisRadio.UseVisualStyleBackColor = true;
            // 
            // xAxisRadio
            // 
            this.xAxisRadio.AutoSize = true;
            this.xAxisRadio.Location = new System.Drawing.Point(42, 37);
            this.xAxisRadio.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.xAxisRadio.Name = "xAxisRadio";
            this.xAxisRadio.Size = new System.Drawing.Size(57, 29);
            this.xAxisRadio.TabIndex = 0;
            this.xAxisRadio.TabStop = true;
            this.xAxisRadio.Text = "X";
            this.xAxisRadio.UseVisualStyleBackColor = true;
            // 
            // minTextBox
            // 
            this.minTextBox.Location = new System.Drawing.Point(1762, 554);
            this.minTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.minTextBox.Name = "minTextBox";
            this.minTextBox.Size = new System.Drawing.Size(108, 31);
            this.minTextBox.TabIndex = 13;
            // 
            // maxTextBox
            // 
            this.maxTextBox.Location = new System.Drawing.Point(1948, 554);
            this.maxTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.maxTextBox.Name = "maxTextBox";
            this.maxTextBox.Size = new System.Drawing.Size(108, 31);
            this.maxTextBox.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1700, 560);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "Min";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1884, 560);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 25);
            this.label4.TabIndex = 16;
            this.label4.Text = "Max";
            // 
            // contouringGroupBox
            // 
            this.contouringGroupBox.Controls.Add(this.isosurfaceButton);
            this.contouringGroupBox.Controls.Add(this.floodedContourRadioBtn);
            this.contouringGroupBox.Controls.Add(this.lineContourRadioBtn);
            this.contouringGroupBox.Controls.Add(this.noContourRadioBtn);
            this.contouringGroupBox.Controls.Add(this.label5);
            this.contouringGroupBox.Controls.Add(this.num_Contours);
            this.contouringGroupBox.Location = new System.Drawing.Point(1705, 909);
            this.contouringGroupBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.contouringGroupBox.Name = "contouringGroupBox";
            this.contouringGroupBox.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.contouringGroupBox.Size = new System.Drawing.Size(400, 271);
            this.contouringGroupBox.TabIndex = 11;
            this.contouringGroupBox.TabStop = false;
            this.contouringGroupBox.Text = "Contouring";
            // 
            // isosurfaceButton
            // 
            this.isosurfaceButton.AutoSize = true;
            this.isosurfaceButton.Location = new System.Drawing.Point(42, 212);
            this.isosurfaceButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.isosurfaceButton.Name = "isosurfaceButton";
            this.isosurfaceButton.Size = new System.Drawing.Size(151, 29);
            this.isosurfaceButton.TabIndex = 22;
            this.isosurfaceButton.TabStop = true;
            this.isosurfaceButton.Text = "Iso Surface";
            this.isosurfaceButton.UseVisualStyleBackColor = true;
            this.isosurfaceButton.CheckedChanged += new System.EventHandler(this.isosurfaceButton_CheckedChanged);
            // 
            // floodedContourRadioBtn
            // 
            this.floodedContourRadioBtn.AutoSize = true;
            this.floodedContourRadioBtn.Location = new System.Drawing.Point(42, 171);
            this.floodedContourRadioBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.floodedContourRadioBtn.Name = "floodedContourRadioBtn";
            this.floodedContourRadioBtn.Size = new System.Drawing.Size(203, 29);
            this.floodedContourRadioBtn.TabIndex = 21;
            this.floodedContourRadioBtn.TabStop = true;
            this.floodedContourRadioBtn.Text = "Flooded Contour";
            this.floodedContourRadioBtn.UseVisualStyleBackColor = true;
            this.floodedContourRadioBtn.CheckedChanged += new System.EventHandler(this.floodedContourRadioBtn_CheckedChanged);
            // 
            // lineContourRadioBtn
            // 
            this.lineContourRadioBtn.AutoSize = true;
            this.lineContourRadioBtn.Location = new System.Drawing.Point(42, 125);
            this.lineContourRadioBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lineContourRadioBtn.Name = "lineContourRadioBtn";
            this.lineContourRadioBtn.Size = new System.Drawing.Size(166, 29);
            this.lineContourRadioBtn.TabIndex = 20;
            this.lineContourRadioBtn.TabStop = true;
            this.lineContourRadioBtn.Text = "Line Contour";
            this.lineContourRadioBtn.UseVisualStyleBackColor = true;
            this.lineContourRadioBtn.CheckedChanged += new System.EventHandler(this.lineContourRadioBtn_CheckedChanged);
            // 
            // noContourRadioBtn
            // 
            this.noContourRadioBtn.AutoSize = true;
            this.noContourRadioBtn.Location = new System.Drawing.Point(42, 79);
            this.noContourRadioBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.noContourRadioBtn.Name = "noContourRadioBtn";
            this.noContourRadioBtn.Size = new System.Drawing.Size(152, 29);
            this.noContourRadioBtn.TabIndex = 19;
            this.noContourRadioBtn.TabStop = true;
            this.noContourRadioBtn.Text = "No Contour";
            this.noContourRadioBtn.UseVisualStyleBackColor = true;
            this.noContourRadioBtn.CheckedChanged += new System.EventHandler(this.noContourRadioBtn_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 38);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(204, 25);
            this.label5.TabIndex = 18;
            this.label5.Text = "Number of Contours";
            // 
            // num_Contours
            // 
            this.num_Contours.Location = new System.Drawing.Point(248, 37);
            this.num_Contours.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.num_Contours.Name = "num_Contours";
            this.num_Contours.Size = new System.Drawing.Size(100, 31);
            this.num_Contours.TabIndex = 17;
            this.num_Contours.TextChanged += new System.EventHandler(this.num_Contours_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.threeDRadioBtn);
            this.groupBox1.Controls.Add(this.twoDRadioBtn);
            this.groupBox1.Location = new System.Drawing.Point(1926, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 124);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drawing Mode";
            // 
            // twoDRadioBtn
            // 
            this.twoDRadioBtn.AutoSize = true;
            this.twoDRadioBtn.Checked = true;
            this.twoDRadioBtn.Location = new System.Drawing.Point(9, 33);
            this.twoDRadioBtn.Margin = new System.Windows.Forms.Padding(6);
            this.twoDRadioBtn.Name = "twoDRadioBtn";
            this.twoDRadioBtn.Size = new System.Drawing.Size(76, 29);
            this.twoDRadioBtn.TabIndex = 3;
            this.twoDRadioBtn.Text = "2 D";
            this.twoDRadioBtn.UseVisualStyleBackColor = true;
            // 
            // threeDRadioBtn
            // 
            this.threeDRadioBtn.AutoSize = true;
            this.threeDRadioBtn.Location = new System.Drawing.Point(9, 74);
            this.threeDRadioBtn.Margin = new System.Windows.Forms.Padding(6);
            this.threeDRadioBtn.Name = "threeDRadioBtn";
            this.threeDRadioBtn.Size = new System.Drawing.Size(76, 29);
            this.threeDRadioBtn.TabIndex = 4;
            this.threeDRadioBtn.Text = "3 D";
            this.threeDRadioBtn.UseVisualStyleBackColor = true;
            this.threeDRadioBtn.CheckedChanged += new System.EventHandler(this.threeDRadioBtn_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2190, 1223);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.contouringGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maxTextBox);
            this.Controls.Add(this.minTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.dataName_ListBox);
            this.Controls.Add(this.coloringModeGroupBox);
            this.Controls.Add(this.colorsPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mappingMode_combo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.simpleOpenGlControl1);
            this.Controls.Add(this.loadFileButton);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Mesh Load";
            this.coloringModeGroupBox.ResumeLayout(false);
            this.coloringModeGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.contouringGroupBox.ResumeLayout(false);
            this.contouringGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadFileButton;
        private Tao.Platform.Windows.SimpleOpenGlControl simpleOpenGlControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mappingMode_combo;
        private System.Windows.Forms.Panel colorsPanel;
        private System.Windows.Forms.GroupBox coloringModeGroupBox;
        private System.Windows.Forms.RadioButton edgeColoringRadio;
        private System.Windows.Forms.RadioButton faceColoringRadio;
        private System.Windows.Forms.RadioButton defaultColoringRadio;
        private System.Windows.Forms.ListBox dataName_ListBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton zAxisRadio;
        private System.Windows.Forms.RadioButton yAxisRadio;
        private System.Windows.Forms.RadioButton xAxisRadio;
        private System.Windows.Forms.TextBox minTextBox;
        private System.Windows.Forms.TextBox maxTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox contouringGroupBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox num_Contours;
        private System.Windows.Forms.RadioButton floodedContourRadioBtn;
        private System.Windows.Forms.RadioButton lineContourRadioBtn;
        private System.Windows.Forms.RadioButton noContourRadioBtn;
        private System.Windows.Forms.RadioButton isosurfaceButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton threeDRadioBtn;
        private System.Windows.Forms.RadioButton twoDRadioBtn;
    }
}