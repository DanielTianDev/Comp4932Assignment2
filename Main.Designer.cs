namespace Comp4932_Assignment2
{
    partial class Main
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
            this.btnInitialize = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imgDisplay = new System.Windows.Forms.PictureBox();
            this.imgReverse = new System.Windows.Forms.PictureBox();
            this.imgIntermediate = new System.Windows.Forms.PictureBox();
            this.btnCompressImage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgReverse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgIntermediate)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInitialize
            // 
            this.btnInitialize.Location = new System.Drawing.Point(103, 72);
            this.btnInitialize.Name = "btnInitialize";
            this.btnInitialize.Size = new System.Drawing.Size(145, 55);
            this.btnInitialize.TabIndex = 0;
            this.btnInitialize.Text = "Find MV";
            this.btnInitialize.UseVisualStyleBackColor = true;
            this.btnInitialize.Click += new System.EventHandler(this.btnFileSelect_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(1022, 107);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(37, 20);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "Info";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imgDisplay
            // 
            this.imgDisplay.Location = new System.Drawing.Point(40, 241);
            this.imgDisplay.Name = "imgDisplay";
            this.imgDisplay.Size = new System.Drawing.Size(708, 693);
            this.imgDisplay.TabIndex = 2;
            this.imgDisplay.TabStop = false;
            // 
            // imgReverse
            // 
            this.imgReverse.Location = new System.Drawing.Point(1475, 241);
            this.imgReverse.Name = "imgReverse";
            this.imgReverse.Size = new System.Drawing.Size(665, 693);
            this.imgReverse.TabIndex = 3;
            this.imgReverse.TabStop = false;
            // 
            // imgIntermediate
            // 
            this.imgIntermediate.Location = new System.Drawing.Point(780, 241);
            this.imgIntermediate.Name = "imgIntermediate";
            this.imgIntermediate.Size = new System.Drawing.Size(671, 693);
            this.imgIntermediate.TabIndex = 4;
            this.imgIntermediate.TabStop = false;
            // 
            // btnCompressImage
            // 
            this.btnCompressImage.Location = new System.Drawing.Point(281, 72);
            this.btnCompressImage.Name = "btnCompressImage";
            this.btnCompressImage.Size = new System.Drawing.Size(145, 55);
            this.btnCompressImage.TabIndex = 5;
            this.btnCompressImage.Text = "Compress Image";
            this.btnCompressImage.UseVisualStyleBackColor = true;
            this.btnCompressImage.Click += new System.EventHandler(this.btnCompressImage_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2178, 1289);
            this.Controls.Add(this.btnCompressImage);
            this.Controls.Add(this.imgIntermediate);
            this.Controls.Add(this.imgReverse);
            this.Controls.Add(this.imgDisplay);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.btnInitialize);
            this.Name = "Main";
            this.Text = "Image Manipulator";
            ((System.ComponentModel.ISupportInitialize)(this.imgDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgReverse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgIntermediate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox imgDisplay;
        private System.Windows.Forms.PictureBox imgReverse;
        private System.Windows.Forms.PictureBox imgIntermediate;
        private System.Windows.Forms.Button btnCompressImage;
    }
}

