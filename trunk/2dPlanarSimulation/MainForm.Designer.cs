namespace _2dPlanarSimulation
{
    partial class MainForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.spinningTriangleControl = new _2dPlanarSimulation.SpinningTriangleControl();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spinningTriangleControl);
            this.splitContainer1.Size = new System.Drawing.Size(792, 573);
            this.splitContainer1.SplitterDistance = 396;
            this.splitContainer1.TabIndex = 0;
            // 
            // spinningTriangleControl
            // 
            this.spinningTriangleControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spinningTriangleControl.Location = new System.Drawing.Point(0, 0);
            this.spinningTriangleControl.Name = "spinningTriangleControl";
            this.spinningTriangleControl.Size = new System.Drawing.Size(392, 573);
            this.spinningTriangleControl.TabIndex = 1;
            this.spinningTriangleControl.Text = "spinningTriangleControl";
            this.spinningTriangleControl.Click += new System.EventHandler(this.spinningTriangleControl_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "WinForms Graphics Device";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private SpinningTriangleControl spinningTriangleControl;
    }
}

