namespace RevitPythonShell
{
    partial class ScriptOutput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptOutput));
            this.cmdCopyToClipboard = new System.Windows.Forms.Button();
            this.txtStdOut = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmdCopyToClipboard
            // 
            this.cmdCopyToClipboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cmdCopyToClipboard.Location = new System.Drawing.Point(0, 357);
            this.cmdCopyToClipboard.Name = "cmdCopyToClipboard";
            this.cmdCopyToClipboard.Size = new System.Drawing.Size(774, 23);
            this.cmdCopyToClipboard.TabIndex = 5;
            this.cmdCopyToClipboard.Text = "Copy text to clipboard";
            this.cmdCopyToClipboard.UseVisualStyleBackColor = true;
            this.cmdCopyToClipboard.Click += new System.EventHandler(this.cmdCopyToClipboard_Click);
            // 
            // txtStdOut
            // 
            this.txtStdOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStdOut.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStdOut.Location = new System.Drawing.Point(0, 0);
            this.txtStdOut.Multiline = true;
            this.txtStdOut.Name = "txtStdOut";
            this.txtStdOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStdOut.Size = new System.Drawing.Size(774, 357);
            this.txtStdOut.TabIndex = 6;
            // 
            // ScriptOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 380);
            this.Controls.Add(this.txtStdOut);
            this.Controls.Add(this.cmdCopyToClipboard);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptOutput";
            this.Text = "RevitPythonShell - Output";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCopyToClipboard;
        public System.Windows.Forms.TextBox txtStdOut;

    }
}