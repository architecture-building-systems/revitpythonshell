namespace RevitPythonShell
{
    partial class ScriptInput
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
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtStdOut = new System.Windows.Forms.TextBox();
            this.cmdExecute = new System.Windows.Forms.Button();
            this.cmdClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtSource
            // 
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSource.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(0, 0);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSource.Size = new System.Drawing.Size(792, 226);
            this.txtSource.TabIndex = 0;
            this.txtSource.Text = "# assumes the following file exists: C:\\RevitPythonShell\\current.py\r\ntry:\r\n    im" +
                "port current\r\n    reload(current)\r\n\r\n    current.main(revit)\r\nexcept:\r\n    impor" +
                "t traceback\r\n    traceback.print_exc()";
            this.txtSource.WordWrap = false;
            // 
            // txtStdOut
            // 
            this.txtStdOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStdOut.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStdOut.Location = new System.Drawing.Point(0, 224);
            this.txtStdOut.Multiline = true;
            this.txtStdOut.Name = "txtStdOut";
            this.txtStdOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStdOut.Size = new System.Drawing.Size(792, 302);
            this.txtStdOut.TabIndex = 3;
            // 
            // cmdExecute
            // 
            this.cmdExecute.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cmdExecute.Location = new System.Drawing.Point(0, 546);
            this.cmdExecute.Name = "cmdExecute";
            this.cmdExecute.Size = new System.Drawing.Size(792, 26);
            this.cmdExecute.TabIndex = 2;
            this.cmdExecute.Text = "Execute";
            this.cmdExecute.UseVisualStyleBackColor = true;
            this.cmdExecute.Click += new System.EventHandler(this.cmdExecute_Click);
            // 
            // cmdClear
            // 
            this.cmdClear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cmdClear.Location = new System.Drawing.Point(0, 523);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(792, 23);
            this.cmdClear.TabIndex = 4;
            this.cmdClear.Text = "Clear";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // ScriptInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 572);
            this.Controls.Add(this.cmdClear);
            this.Controls.Add(this.cmdExecute);
            this.Controls.Add(this.txtStdOut);
            this.Controls.Add(this.txtSource);
            this.KeyPreview = true;
            this.Name = "ScriptInput";
            this.Text = "ScriptInput";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScriptInput_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSource;
        public System.Windows.Forms.TextBox txtStdOut;
        private System.Windows.Forms.Button cmdExecute;
        private System.Windows.Forms.Button cmdClear;
    }
}