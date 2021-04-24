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
            this.ServerPublicKey = new System.Windows.Forms.Button();
            this.PublicLabel = new System.Windows.Forms.Label();
            this.ServerPrivateKey = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(348, 253);
            this.label_port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(38, 17);
            this.label_port.TabIndex = 0;
            this.label_port.Text = "Port:";
            // 
            // textBox_port_input
            // 
            this.textBox_port_input.Location = new System.Drawing.Point(395, 250);
            this.textBox_port_input.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_port_input.Name = "textBox_port_input";
            this.textBox_port_input.Size = new System.Drawing.Size(132, 22);
            this.textBox_port_input.TabIndex = 1;
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(395, 283);
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
            this.richTextBox_ConsoleOut.Location = new System.Drawing.Point(15, 4);
            this.richTextBox_ConsoleOut.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox_ConsoleOut.Name = "richTextBox_ConsoleOut";
            this.richTextBox_ConsoleOut.ReadOnly = true;
            this.richTextBox_ConsoleOut.Size = new System.Drawing.Size(588, 216);
            this.richTextBox_ConsoleOut.TabIndex = 3;
            this.richTextBox_ConsoleOut.Text = "";
            // 
            // ServerPublicKey
            // 
            this.ServerPublicKey.Location = new System.Drawing.Point(154, 249);
            this.ServerPublicKey.Margin = new System.Windows.Forms.Padding(4);
            this.ServerPublicKey.Name = "ServerPublicKey";
            this.ServerPublicKey.Size = new System.Drawing.Size(133, 28);
            this.ServerPublicKey.TabIndex = 4;
            this.ServerPublicKey.Text = "Browse";
            this.ServerPublicKey.UseVisualStyleBackColor = true;
            this.ServerPublicKey.Click += new System.EventHandler(this.ServerPublicKey_Click);
            // 
            // PublicLabel
            // 
            this.PublicLabel.AutoSize = true;
            this.PublicLabel.Location = new System.Drawing.Point(22, 255);
            this.PublicLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PublicLabel.Name = "PublicLabel";
            this.PublicLabel.Size = new System.Drawing.Size(124, 17);
            this.PublicLabel.TabIndex = 5;
            this.PublicLabel.Text = "Server Public Key:";
            // 
            // ServerPrivateKey
            // 
            this.ServerPrivateKey.Location = new System.Drawing.Point(154, 300);
            this.ServerPrivateKey.Margin = new System.Windows.Forms.Padding(4);
            this.ServerPrivateKey.Name = "ServerPrivateKey";
            this.ServerPrivateKey.Size = new System.Drawing.Size(133, 28);
            this.ServerPrivateKey.TabIndex = 6;
            this.ServerPrivateKey.Text = "Browse";
            this.ServerPrivateKey.UseVisualStyleBackColor = true;
            this.ServerPrivateKey.Click += new System.EventHandler(this.ServerPrivateKey_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 306);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Server Private Key:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 361);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ServerPrivateKey);
            this.Controls.Add(this.PublicLabel);
            this.Controls.Add(this.ServerPublicKey);
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
        private System.Windows.Forms.Button ServerPublicKey;
        private System.Windows.Forms.Label PublicLabel;
        private System.Windows.Forms.Button ServerPrivateKey;
        private System.Windows.Forms.Label label1;
    }
}

