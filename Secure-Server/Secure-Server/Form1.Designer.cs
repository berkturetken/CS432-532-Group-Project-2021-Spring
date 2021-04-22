namespace Secure_Server
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
            this.label_port = new System.Windows.Forms.Label();
            this.textBox_port_input = new System.Windows.Forms.TextBox();
            this.button_listen = new System.Windows.Forms.Button();
            this.richTextBox_ConsoleOut = new System.Windows.Forms.RichTextBox();
            this.textBox_KeyFilePath = new System.Windows.Forms.TextBox();
            this.textBox_RepoPath = new System.Windows.Forms.TextBox();
            this.button_browse_keyFile = new System.Windows.Forms.Button();
            this.button_browse_repoPath = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(12, 311);
            this.label_port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(38, 17);
            this.label_port.TabIndex = 0;
            this.label_port.Text = "Port:";
            // 
            // textBox_port_input
            // 
            this.textBox_port_input.Location = new System.Drawing.Point(59, 308);
            this.textBox_port_input.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_port_input.Name = "textBox_port_input";
            this.textBox_port_input.Size = new System.Drawing.Size(132, 22);
            this.textBox_port_input.TabIndex = 1;
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(59, 341);
            this.button_listen.Margin = new System.Windows.Forms.Padding(4);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(133, 28);
            this.button_listen.TabIndex = 2;
            this.button_listen.Text = "Listen";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // richTextBox_ConsoleOut
            // 
            this.richTextBox_ConsoleOut.Location = new System.Drawing.Point(263, 16);
            this.richTextBox_ConsoleOut.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox_ConsoleOut.Name = "richTextBox_ConsoleOut";
            this.richTextBox_ConsoleOut.Size = new System.Drawing.Size(193, 352);
            this.richTextBox_ConsoleOut.TabIndex = 3;
            this.richTextBox_ConsoleOut.Text = "";
            // 
            // textBox_KeyFilePath
            // 
            this.textBox_KeyFilePath.Location = new System.Drawing.Point(42, 39);
            this.textBox_KeyFilePath.Name = "textBox_KeyFilePath";
            this.textBox_KeyFilePath.Size = new System.Drawing.Size(100, 22);
            this.textBox_KeyFilePath.TabIndex = 4;
            // 
            // textBox_RepoPath
            // 
            this.textBox_RepoPath.Location = new System.Drawing.Point(42, 98);
            this.textBox_RepoPath.Name = "textBox_RepoPath";
            this.textBox_RepoPath.Size = new System.Drawing.Size(100, 22);
            this.textBox_RepoPath.TabIndex = 5;
            // 
            // button_browse_keyFile
            // 
            this.button_browse_keyFile.Location = new System.Drawing.Point(160, 38);
            this.button_browse_keyFile.Name = "button_browse_keyFile";
            this.button_browse_keyFile.Size = new System.Drawing.Size(75, 23);
            this.button_browse_keyFile.TabIndex = 6;
            this.button_browse_keyFile.Text = "Browse1";
            this.button_browse_keyFile.UseVisualStyleBackColor = true;
            this.button_browse_keyFile.Click += new System.EventHandler(this.button_browse_keyFile_Click);
            // 
            // button_browse_repoPath
            // 
            this.button_browse_repoPath.Location = new System.Drawing.Point(160, 97);
            this.button_browse_repoPath.Name = "button_browse_repoPath";
            this.button_browse_repoPath.Size = new System.Drawing.Size(75, 23);
            this.button_browse_repoPath.TabIndex = 7;
            this.button_browse_repoPath.Text = "Browse2";
            this.button_browse_repoPath.UseVisualStyleBackColor = true;
            this.button_browse_repoPath.Click += new System.EventHandler(this.button_browse_repoPath_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Key File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Main Repo";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 384);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_browse_repoPath);
            this.Controls.Add(this.button_browse_keyFile);
            this.Controls.Add(this.textBox_RepoPath);
            this.Controls.Add(this.textBox_KeyFilePath);
            this.Controls.Add(this.richTextBox_ConsoleOut);
            this.Controls.Add(this.button_listen);
            this.Controls.Add(this.textBox_port_input);
            this.Controls.Add(this.label_port);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_port;
        private System.Windows.Forms.TextBox textBox_port_input;
        private System.Windows.Forms.Button button_listen;
        private System.Windows.Forms.RichTextBox richTextBox_ConsoleOut;
        private System.Windows.Forms.TextBox textBox_KeyFilePath;
        private System.Windows.Forms.TextBox textBox_RepoPath;
        private System.Windows.Forms.Button button_browse_keyFile;
        private System.Windows.Forms.Button button_browse_repoPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

