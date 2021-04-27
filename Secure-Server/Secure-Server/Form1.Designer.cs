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
            this.button_browse_serverPubKey = new System.Windows.Forms.Button();
            this.PublicLabel = new System.Windows.Forms.Label();
            this.button_browse_serverPrivKey = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mainRepository = new System.Windows.Forms.Label();
            this.button_browse_mainRepo = new System.Windows.Forms.Button();
            this.textBox_serverPub = new System.Windows.Forms.TextBox();
            this.textBox_serverPriv = new System.Windows.Forms.TextBox();
            this.textBox_mainRepo = new System.Windows.Forms.TextBox();
            this.textBox_onlineClients = new System.Windows.Forms.TextBox();
            this.label_onlineClients = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(445, 253);
            this.label_port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(38, 17);
            this.label_port.TabIndex = 0;
            this.label_port.Text = "Port:";
            // 
            // textBox_port_input
            // 
            this.textBox_port_input.Location = new System.Drawing.Point(492, 250);
            this.textBox_port_input.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_port_input.Name = "textBox_port_input";
            this.textBox_port_input.Size = new System.Drawing.Size(132, 22);
            this.textBox_port_input.TabIndex = 1;
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(492, 283);
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
            this.richTextBox_ConsoleOut.Size = new System.Drawing.Size(625, 216);
            this.richTextBox_ConsoleOut.TabIndex = 3;
            this.richTextBox_ConsoleOut.Text = "";
            // 
            // button_browse_serverPubKey
            // 
            this.button_browse_serverPubKey.Location = new System.Drawing.Point(291, 252);
            this.button_browse_serverPubKey.Margin = new System.Windows.Forms.Padding(4);
            this.button_browse_serverPubKey.Name = "button_browse_serverPubKey";
            this.button_browse_serverPubKey.Size = new System.Drawing.Size(133, 28);
            this.button_browse_serverPubKey.TabIndex = 4;
            this.button_browse_serverPubKey.Text = "Browse";
            this.button_browse_serverPubKey.UseVisualStyleBackColor = true;
            this.button_browse_serverPubKey.Click += new System.EventHandler(this.ServerPublicKey_Click);
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
            // button_browse_serverPrivKey
            // 
            this.button_browse_serverPrivKey.Location = new System.Drawing.Point(291, 300);
            this.button_browse_serverPrivKey.Margin = new System.Windows.Forms.Padding(4);
            this.button_browse_serverPrivKey.Name = "button_browse_serverPrivKey";
            this.button_browse_serverPrivKey.Size = new System.Drawing.Size(133, 28);
            this.button_browse_serverPrivKey.TabIndex = 6;
            this.button_browse_serverPrivKey.Text = "Browse";
            this.button_browse_serverPrivKey.UseVisualStyleBackColor = true;
            this.button_browse_serverPrivKey.Click += new System.EventHandler(this.ServerPrivateKey_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 303);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Server Private Key:";
            // 
            // mainRepository
            // 
            this.mainRepository.AutoSize = true;
            this.mainRepository.Location = new System.Drawing.Point(22, 353);
            this.mainRepository.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mainRepository.Name = "mainRepository";
            this.mainRepository.Size = new System.Drawing.Size(109, 17);
            this.mainRepository.TabIndex = 8;
            this.mainRepository.Text = "Main repository:";
            // 
            // button_browse_mainRepo
            // 
            this.button_browse_mainRepo.Location = new System.Drawing.Point(291, 347);
            this.button_browse_mainRepo.Margin = new System.Windows.Forms.Padding(4);
            this.button_browse_mainRepo.Name = "button_browse_mainRepo";
            this.button_browse_mainRepo.Size = new System.Drawing.Size(133, 28);
            this.button_browse_mainRepo.TabIndex = 9;
            this.button_browse_mainRepo.Text = "Browse";
            this.button_browse_mainRepo.UseVisualStyleBackColor = true;
            this.button_browse_mainRepo.Click += new System.EventHandler(this.mainRepo_Click);
            // 
            // textBox_serverPub
            // 
            this.textBox_serverPub.Location = new System.Drawing.Point(151, 255);
            this.textBox_serverPub.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_serverPub.Name = "textBox_serverPub";
            this.textBox_serverPub.ReadOnly = true;
            this.textBox_serverPub.Size = new System.Drawing.Size(132, 22);
            this.textBox_serverPub.TabIndex = 10;
            // 
            // textBox_serverPriv
            // 
            this.textBox_serverPriv.Location = new System.Drawing.Point(151, 303);
            this.textBox_serverPriv.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_serverPriv.Name = "textBox_serverPriv";
            this.textBox_serverPriv.ReadOnly = true;
            this.textBox_serverPriv.Size = new System.Drawing.Size(132, 22);
            this.textBox_serverPriv.TabIndex = 11;
            // 
            // textBox_mainRepo
            // 
            this.textBox_mainRepo.Location = new System.Drawing.Point(151, 353);
            this.textBox_mainRepo.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_mainRepo.Name = "textBox_mainRepo";
            this.textBox_mainRepo.ReadOnly = true;
            this.textBox_mainRepo.Size = new System.Drawing.Size(132, 22);
            this.textBox_mainRepo.TabIndex = 12;
            // 
            // textBox_onlineClients
            // 
            this.textBox_onlineClients.Location = new System.Drawing.Point(448, 356);
            this.textBox_onlineClients.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_onlineClients.Multiline = true;
            this.textBox_onlineClients.Name = "textBox_onlineClients";
            this.textBox_onlineClients.ReadOnly = true;
            this.textBox_onlineClients.Size = new System.Drawing.Size(177, 110);
            this.textBox_onlineClients.TabIndex = 13;
            // 
            // label_onlineClients
            // 
            this.label_onlineClients.AutoSize = true;
            this.label_onlineClients.Location = new System.Drawing.Point(445, 335);
            this.label_onlineClients.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_onlineClients.Name = "label_onlineClients";
            this.label_onlineClients.Size = new System.Drawing.Size(99, 17);
            this.label_onlineClients.TabIndex = 14;
            this.label_onlineClients.Text = "Online Clients:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 489);
            this.Controls.Add(this.label_onlineClients);
            this.Controls.Add(this.textBox_onlineClients);
            this.Controls.Add(this.textBox_mainRepo);
            this.Controls.Add(this.textBox_serverPriv);
            this.Controls.Add(this.textBox_serverPub);
            this.Controls.Add(this.button_browse_mainRepo);
            this.Controls.Add(this.mainRepository);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_browse_serverPrivKey);
            this.Controls.Add(this.PublicLabel);
            this.Controls.Add(this.button_browse_serverPubKey);
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
        private System.Windows.Forms.Button button_browse_serverPubKey;
        private System.Windows.Forms.Label PublicLabel;
        private System.Windows.Forms.Button button_browse_serverPrivKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mainRepository;
        private System.Windows.Forms.Button button_browse_mainRepo;
        private System.Windows.Forms.TextBox textBox_serverPub;
        private System.Windows.Forms.TextBox textBox_serverPriv;
        private System.Windows.Forms.TextBox textBox_mainRepo;
        private System.Windows.Forms.TextBox textBox_onlineClients;
        private System.Windows.Forms.Label label_onlineClients;
    }
}

