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
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;
using Client.Models;
using System.Net;

namespace Client
{
    public partial class Form1 : Form
    {
        string name;
        bool terminating = false;
        bool connected = false;
        bool authenticated = false;
        bool loginStart = false;
        Socket serverSocket;

        private string ServerKey = ""; 
        private string UserPublicKey = "";
        private string UserEncryptedPrivateKey = "";
        private string UserPrivateKey = "";
        private string SessionKey = "";
        private string randomNumber = "";
        private byte[] AES256Key = new byte[32];
        private byte[] AES256IV = new byte[16];

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            //Initial button and textbox situations
            button_send.Enabled = false;
            textBox_message.Enabled = false;
            button_Login.Enabled = false;
            textBox_Password.Enabled = false;
            button_disconnect.Enabled = false;
        }

        private void Receive()
        {
            while (!authenticated && connected)
            {
                try
                {
                    Byte[] buffer = new Byte[2176];
                    serverSocket.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer).Trim('\0');

                    CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(incomingMessage);

                    if (msg.msgCode == MessageCodes.DisconnectResponse)
                    {
                        serverSocket.Close();
                        richTextBox1.AppendText("The server has disconnected.\n");
                        connectionClosedButtons();
                    }
                    else if (msg.msgCode == MessageCodes.Request)
                    {
                        try
                        {
                            //Receive Verification and Session Key from server
                            string verificationString = msg.message;
                            //Split the sent message into signature and message                     
                            string hmacMessage = verificationString.Substring(0, verificationString.Length - 1024);
                            string signatureHexa = verificationString.Substring(verificationString.Length - 1024);
                            byte[] signatureBytes = hexStringToByteArray(signatureHexa);
                            string signature = Encoding.Default.GetString(signatureBytes);

                            CommunicationMessage hmacCommMessage = JsonConvert.DeserializeObject<CommunicationMessage>(hmacMessage);
                            MessageCodes verificationResult = hmacCommMessage.msgCode;

                            //If it is a positive acknowledgement
                            if (verificationResult == MessageCodes.SuccessfulResponse)
                            {
                                bool isSignatureVerified = verifyWithRSA(hmacMessage, 4096, ServerKey, signatureBytes);
                                if (isSignatureVerified) // If signature is verified
                                {
                                    byte[] encryptedHmacBytes = hexStringToByteArray(hmacCommMessage.message);
                                    string encryptedHmac = Encoding.Default.GetString(encryptedHmacBytes);
                                    byte[] decryptedHmacBytes = decryptWithRSA(encryptedHmac, 4096, UserPrivateKey);
                                    SessionKey = Encoding.Default.GetString(decryptedHmacBytes);
                                    richTextBox1.AppendText("Session key: " + generateHexStringFromByteArray(decryptedHmacBytes) + "\n");

                                    authenticated = true;
                                    // Manage GUI elements
                                    button_Login.Enabled = false;
                                    button_send.Enabled = true;
                                    textBox_message.Enabled = true;
                                }
                                else // If not verified
                                {
                                    connectionClosedButtons();
                                    serverSocket.Close();
                                    richTextBox1.AppendText("Signature can not be verified! Connection closed\n");
                                }
                            }
                            else // If it is a negative acknowledgement
                            {
                                bool isSignatureVerified = verifyWithRSA(hmacMessage, 4096, ServerKey, signatureBytes);
                                if (isSignatureVerified) // If signature is verified
                                {
                                    richTextBox1.AppendText("Negative Acknowledgment from the server! Connection closed\n");
                                }
                                else //If not verified
                                {
                                    richTextBox1.AppendText("Server can not be verified! Connection closed\n");
                                }
                                connectionClosedButtons();
                                serverSocket.Close();
                            }
                        }
                        catch
                        {
                            connectionClosedButtons();
                            serverSocket.Close();
                            richTextBox1.AppendText("Error during session key receiving!\n");
                        }
                    }
                }
                catch
                {
                   
                }
            }

            while (connected && authenticated)
            {
                try
                {
                    Byte[] buffer = new Byte[64]; // word\0\0\0\0...... until we have the size 64
                    serverSocket.Receive(buffer);

                }
                catch
                {
                    if (!terminating)
                    {
                        richTextBox1.AppendText("Server disconnected!\n");
                    }
                    connectionClosedButtons();
                    serverSocket.Close();
                    connected = false;
                }
            }
        }
    
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            serverSocket.Close();
            Environment.Exit(0);
        }


        /***** HELPER FUNCTIONS *****/
        private void connectionClosedButtons()
        {
            authenticated = false;
            connected = false;
            loginStart = false;
            button_send.Enabled = false;
            textBox_message.Enabled = false;
            button_Login.Enabled = false;
            textBox_Password.Enabled = false;
            button_disconnect.Enabled = false;
            button_connect.Enabled = true;
            textBox_Password.Text = "";
            UserPrivateKey = "";
            SessionKey = "";
        } // Button defaults when the connection is closed

        // Sends any kind of message to the server
        private void send_message(string message, string topic, MessageCodes code) 
        {
            CommunicationMessage msg = new CommunicationMessage();
            msg.topic = topic;
            msg.message = message;
            msg.msgCode = code;
            string jsonObject = JsonConvert.SerializeObject(msg);
            byte[] buffer = Encoding.Default.GetBytes(jsonObject);
            serverSocket.Send(buffer);
        }

        //Receive one-time messages and returning a message object
        private CommunicationMessage receiveOneMessage()
        {
            Byte[] buffer = new Byte[128];
            serverSocket.Receive(buffer);
            string incomingMessage = Encoding.Default.GetString(buffer).Trim('\0');
            CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(incomingMessage);
            return msg;
        }


        /***** GUI ELEMENTS *****/
        private void serverPubKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;     
                serverPubText.Text = fileName.Substring(fileName.LastIndexOf('\\')+1);

                try
                {
                    ServerKey = File.ReadAllText(fileName);
                    byte[] byteServerKey = Encoding.Default.GetBytes(ServerKey);
                    string hexaServerKey = generateHexStringFromByteArray(byteServerKey);
                    richTextBox1.AppendText("Server Public Key: " + hexaServerKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox1.AppendText("Error while getting server public key " + ex.Message);
                }
            }
        }

        private void clientPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                clientPubText.Text = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                try
                {
                    UserPublicKey = File.ReadAllText(fileName);
                    byte[] byteUserPubKey = Encoding.Default.GetBytes(UserPublicKey);
                    string hexaUserPubKey = generateHexStringFromByteArray(byteUserPubKey);
                    richTextBox1.AppendText("User Public Key: " + hexaUserPubKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox1.AppendText("Error while getting client public key " + ex.Message);
                }
            }
        }

        private void clientPrivateKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                clientPrivText.Text = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                try
                {
                    UserEncryptedPrivateKey = File.ReadAllText(fileName);
                    richTextBox1.AppendText("User Encrypted Private Key: " + UserEncryptedPrivateKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox1.AppendText("Error while getting client encrypted private key " + ex.Message);
                }
            }
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_IP_input.Text;
            int port;
            name = textBox_Username.Text;

            // Input checks for username
            if(name.IndexOf('_') != -1)
            {
                richTextBox1.AppendText("Username cannot contain '_' symbol!\n");
                return;
            }
            else if(name.Length == 0 || name.IndexOf(' ') != -1)
            {
                richTextBox1.AppendText("Username cannot be empty or contain whitespaces!\n");
                return;
            }


            //Get the local IP Address
            string currIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    currIP = ip.ToString();                         //Since type of "ip" is System.Net.IPAddress, there must be conversions
            }




            if (Int32.TryParse(textBox_Port_input.Text, out port))
            {
                try
                {

                    if (IP == currIP)
                    {

                        serverSocket.Connect(IP, port);
                        send_message(name, "User name", MessageCodes.Request); // send username to server and wait for uniqeness check
                        CommunicationMessage serverResponse = receiveOneMessage(); // Receive Uniqueness check
                        if (serverResponse.msgCode != MessageCodes.ErrorResponse) // if unique
                        {
                            // Change button settings
                            button_connect.Enabled = false;
                            button_disconnect.Enabled = true;
                            button_Login.Enabled = true;
                            textBox_Password.Enabled = true;
                            connected = true;
                            richTextBox1.AppendText(serverResponse.message);

                            CommunicationMessage randomNumberMessage = receiveOneMessage();
                            randomNumber = randomNumberMessage.message;


                            Thread recieveThread = new Thread(Receive);
                            recieveThread.Start();

                        }
                        else // if username is already used 
                        {
                            richTextBox1.AppendText(serverResponse.message);
                            serverSocket.Close();
                            connected = false;
                        }
                    }
                    else
                    {
                        richTextBox1.AppendText("Could not connect to the server. Check IP!\n");
                    }
                }
                catch
                {
                    //Change button settings to initial
                    connectionClosedButtons();
                    richTextBox1.AppendText("Could not connect to the server.\n");
                }
            }
            else
            {
                richTextBox1.AppendText("Check the port number.\n");
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText("You disconnected.\n");
            connected = false;
            authenticated = false;
            terminating = true;
            connectionClosedButtons();
            CommunicationMessage disconnectMsg = new CommunicationMessage();
            disconnectMsg.msgCode = MessageCodes.DisconnectResponse;
            disconnectMsg.message = name + " disconnected\n";
            disconnectMsg.topic = "Disconnect";
            string disconnectMsg_str = JsonConvert.SerializeObject(disconnectMsg);
            byte[] msg = Encoding.Default.GetBytes(disconnectMsg_str);
            serverSocket.Send(msg);
            serverSocket.Close();
        }

        private void button_Login_Click(object sender, EventArgs e)// Login protocol 
        {
            if (UserPublicKey == "" || UserEncryptedPrivateKey == "" || ServerKey == "")
            {
                richTextBox1.AppendText("You need to choose all the keys first!\n");
            }
            else
            {
                loginStart = true;
                if (connected)
                {
                    byte[] randomNumberBytes = Encoding.Default.GetBytes(randomNumber);
                    richTextBox1.AppendText("Random Number:" + generateHexStringFromByteArray(randomNumberBytes) + "\n");
                    string pass = textBox_Password.Text;

                    //Password hashing and creating AES-256 Key and IV
                    byte[] hashedPassword = hashWithSHA384(pass);
                    Array.Copy(hashedPassword, 0, AES256Key, 0, 32);
                    Array.Copy(hashedPassword, 32, AES256IV, 0, 16);
                    string hexaDecimalAES256Key = generateHexStringFromByteArray(AES256Key);
                    string hexaDecimalAES256IV = generateHexStringFromByteArray(AES256IV);                    
                    try
                    {
                        //Decrypt the Private Key using AES-256 
                        byte[] decryptedPasswordBytes = decryptWithAES256HexVersion(UserEncryptedPrivateKey, AES256Key, AES256IV);
                        UserPrivateKey = Encoding.Default.GetString(decryptedPasswordBytes);
                        string hexaPrivateKey = generateHexStringFromByteArray(decryptedPasswordBytes);
                        richTextBox1.AppendText("AES 256 Key: " + hexaDecimalAES256Key + "\n");
                        richTextBox1.AppendText("AES 256 IV: " + hexaDecimalAES256IV + "\n");
                        richTextBox1.AppendText("User Private Key: " + UserPrivateKey + "\n");
                            try
                            {
                                //Sign the Random Number and Send it to the server
                                byte[] signedNonce = signWithRSA(randomNumber, 4096, UserPrivateKey);
                                string strNonce = Encoding.Default.GetString(signedNonce);
                                string hexaDecimalSignedNonce = generateHexStringFromByteArray(signedNonce);
                                send_message(hexaDecimalSignedNonce, "signedRN", MessageCodes.Request);
                                richTextBox1.AppendText("Signed Nonce: " + hexaDecimalSignedNonce + "\n");

                            }
                            catch
                            {
                                connectionClosedButtons();
                                serverSocket.Close();
                                richTextBox1.AppendText("Error during signing the nonce and sending to the server!\n");
                            }                                        
                    }
                    catch
                    {
                        AES256Key = new byte[32];
                        AES256IV = new byte[16];
                        richTextBox1.AppendText("Wrong password. Please try again.\n");
                    }
                }
            }
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            String message = textBox_message.Text;

            if (message != "" && message.Length < 63)
            {
                Byte[] buffer = new Byte[64];
                buffer = Encoding.Default.GetBytes(message);
                serverSocket.Send(buffer);
            }
        }
    

        /****** CRYPTOGRAPHIC HELPER FUNCTIONS *******/
        static string generateHexStringFromByteArray(byte[] input)
        {
            string hexString = BitConverter.ToString(input);
            return hexString.Replace("-", "");
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /*    HASH    */
        // hash function: SHA-256
        static byte[] hashWithSHA256(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha256Hasher.ComputeHash(byteInput);

            return result;
        }

        // hash function: SHA-384
        static byte[] hashWithSHA384(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA384CryptoServiceProvider sha384Hasher = new SHA384CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha384Hasher.ComputeHash(byteInput);

            return result;
        }

        // hash function: SHA-512
        static byte[] hashWithSHA512(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA512CryptoServiceProvider sha512Hasher = new SHA512CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha512Hasher.ComputeHash(byteInput);

            return result;
        }

        // HMAC with SHA-256
        static byte[] applyHMACwithSHA256(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA256 hmacSHA256 = new HMACSHA256(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA256.ComputeHash(byteInput);

            return result;
        }

        // HMAC with SHA-384
        static byte[] applyHMACwithSHA384(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA384 hmacSHA384 = new HMACSHA384(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA384.ComputeHash(byteInput);

            return result;
        }

        // HMAC with SHA-512
        static byte[] applyHMACwithSHA512(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA512 hmacSHA512 = new HMACSHA512(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA512.ComputeHash(byteInput);

            return result;
        }

        /*    SYMMETRIC CIPHERS     */
        // encryption with AES-256
        static byte[] encryptWithAES256(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-256
            aesObject.KeySize = 256;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            // RijndaelManaged Mode property doesn't support CFB and OFB modes. 
            //If you want to use one of those modes, you should use RijndaelManaged library instead of RijndaelManaged.
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform encryptor = aesObject.CreateEncryptor();
            byte[] result = null;

            try
            {
                result = encryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        // decryption with AES-256
        static byte[] decryptWithAES256(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-256
            aesObject.KeySize = 256;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try
            {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {

                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        static byte[] decryptWithAES256HexVersion(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = hexStringToByteArray(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-256
            aesObject.KeySize = 256;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try
            {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {

                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }
       
        /*    PUBLIC KEY CRYPTOGRAPHY    */
        // RSA encryption with varying bit length
        static byte[] encryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                //true flag is set to perform direct RSA encryption using OAEP padding
                result = rsaObject.Encrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // RSA decryption with varying bit length
        static byte[] decryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                result = rsaObject.Decrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // signing with RSA
        static byte[] signWithRSA(string input, int algoLength, string xmlString)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            byte[] result = null;

            try
            {
                result = rsaObject.SignData(byteInput, "SHA512");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // verifying with RSA
        static bool verifyWithRSA(string input, int algoLength, string xmlString, byte[] signature)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try
            {
                result = rsaObject.VerifyData(byteInput, "SHA256", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

    }
}
