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
            this.SuspendLayout();
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(9, 253);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(29, 13);
            this.label_port.TabIndex = 0;
            this.label_port.Text = "Port:";
            // 
            // textBox_port_input
            // 
            this.textBox_port_input.Location = new System.Drawing.Point(44, 250);
            this.textBox_port_input.Name = "textBox_port_input";
            this.textBox_port_input.Size = new System.Drawing.Size(100, 20);
            this.textBox_port_input.TabIndex = 1;
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(44, 277);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(100, 23);
            this.button_listen.TabIndex = 2;
            this.button_listen.Text = "Listen";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // richTextBox_ConsoleOut
            // 
            this.richTextBox_ConsoleOut.Location = new System.Drawing.Point(197, 13);
            this.richTextBox_ConsoleOut.Name = "richTextBox_ConsoleOut";
            this.richTextBox_ConsoleOut.Size = new System.Drawing.Size(146, 287);
            this.richTextBox_ConsoleOut.TabIndex = 3;
            this.richTextBox_ConsoleOut.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 312);
            this.Controls.Add(this.richTextBox_ConsoleOut);
            this.Controls.Add(this.button_listen);
            this.Controls.Add(this.textBox_port_input);
            this.Controls.Add(this.label_port);
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
    }
}

