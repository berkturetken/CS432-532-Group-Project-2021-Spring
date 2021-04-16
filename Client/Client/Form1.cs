using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {

        bool terminating = false;
        bool connected = false;
        Socket clientSocket;


        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_IP_input.Text;
            int port;

            if (Int32.TryParse(textBox_Port_input.Text, out port))
            {
                try
                {
                    clientSocket.Connect(IP, port); // IP:port --> socket,  127.0.0.1:8080
                    button_connect.Enabled = false;
                    connected = true;
                    richTextBox1.AppendText("Connected to the server.\n");

                    Thread receiveThread = new Thread(Receive);
                    receiveThread.Start();

                }
                catch
                {
                    richTextBox1.AppendText("Could not connect to the server.\n");
                }

            }
            else
            {
                richTextBox1.AppendText("Check the port number.\n");
            }

        }

        private void Receive()
        {
            while (connected)
            {

                try
                {

                    Byte[] buffer = new Byte[64]; // word\0\0\0\0...... until we have the size 64
                    clientSocket.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf('\0'));

                    richTextBox1.AppendText(incomingMessage + "\n");

                }
                catch
                {

                    if (!terminating)
                    {
                        richTextBox1.AppendText("The server has disconnected.\n");
                    }

                    clientSocket.Close();
                    connected = false;

                }

            }

        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            String message = textBox_message.Text;

            if (message != "" && message.Length < 63)
            {
                Byte[] buffer = new Byte[64];
                buffer = Encoding.Default.GetBytes(message);
                clientSocket.Send(buffer);
            }
        }
    }
}
