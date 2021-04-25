using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Secure_Server.Models;
using System.IO;

namespace Secure_Server
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool listening = false;
        //bool remoteConnected = false;
        
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> socketList = new List<Socket>();
        List<string> usernames = new List<string>();

        string serverPublicKey = "";
        string serverPrivateKey = "";
        string mainRepositoryPath = "";

        Dictionary<string, string> userPubKeys = new Dictionary<string, string>();
        Dictionary<string, string> userHMACKeys = new Dictionary<string, string>();

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }

        private bool ChallengeResponsePhase1(Socket client,string username)
        {
            string randomNumber = randomNumberGenerator(16);
            try
            {
                string randomNumberMessage = createCommunicationMessage(MessageCodes.Request, "RN", randomNumber);
                sendMessage(client, randomNumberMessage);
                
                Byte[] signedRandomNumberBuffer = new Byte[1088];
                client.Receive(signedRandomNumberBuffer);
                string inMessage = Encoding.Default.GetString(signedRandomNumberBuffer).Trim('\0');
                richTextBox_ConsoleOut.AppendText(inMessage+"\n");
                CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(inMessage);

                if (msg.msgCode == MessageCodes.Request)
                {
                    string signedRandom = msg.message;
                             
                    try
                    {
                        string clientPubKey = userPubKeys[username];
                        bool isVerified = verifyWithRSA(randomNumber, 4096, clientPubKey, hexStringToByteArray(signedRandom));
                        if (!isVerified)
                        {
                            string negativeAckJSON = createCommunicationMessage(MessageCodes.ErrorResponse, "Session Key", "Negative Acknowledgement");
                            byte[] signedNegativeAck = signWithRSA(negativeAckJSON, 4096, serverPrivateKey);
                            string hexSignedNegativeAck = generateHexStringFromByteArray(signedNegativeAck);
                            richTextBox_ConsoleOut.AppendText("Length of Signed Negative Ack: " + hexSignedNegativeAck.Length.ToString() + "\n");

                            string sessionKeyProblem = negativeAckJSON + hexSignedNegativeAck;
                            sendMessage(client, sessionKeyProblem);

                            // Close the connection
                            return false;
                        }
                        else
                        {
                            // Positive acknowledgement
                            string hmacKey = randomNumberGenerator(32);

                            // Encrypt the HMAC Key and serialize it
                            byte[] encryptedHMAC = encryptWithRSA(hmacKey, 4096, clientPubKey);
                            string hmacMessageJSON = createCommunicationMessage(MessageCodes.SuccessfulResponse, "Session Key", generateHexStringFromByteArray(encryptedHMAC));
                            richTextBox_ConsoleOut.AppendText("Length of HMAC Message: " + hmacMessageJSON.Length.ToString() + "\n");

                            // Sign the message
                            byte[] signedMessage = signWithRSA(hmacMessageJSON, 4096, serverPrivateKey);

                            // Merge
                            string sessionKeyAgreement = hmacMessageJSON + generateHexStringFromByteArray(signedMessage);
                            richTextBox_ConsoleOut.AppendText("Length of Session Key Agreement: " + sessionKeyAgreement.Length.ToString() + "\n");
                            
                            // Send the Session Key Agreement to corresponding client
                            sendMessage(client, sessionKeyAgreement);

                            // Add to dictionary
                            userHMACKeys.Add(username, hmacKey);
                        }
                    }
                    catch (Exception e)
                    {
                        richTextBox_ConsoleOut.AppendText("In challenge response: " + e.Message + "\n");
                        return false;
                    }
                }
                else
                {
                    richTextBox_ConsoleOut.AppendText("Wrong Message Code while retrieving signed random number from the client:\n");
                    return false;
                }
            }
            catch (Exception e)   // If an error is encountered
            {
                richTextBox_ConsoleOut.AppendText("Something occurs " + e.Message + "\n");
                return false;
            }
            return true;
        }

        private void Receive(Socket s, string username) //username is send from accept thread, to be used to find files in public key repo. 
        {
            bool connected = true;
            /*SERVER PHASE 1 CHALLENGE-RESPONSE*/
            try
            {
                if (!ChallengeResponsePhase1(s, username))
                {
                    connected = false;
                    if (!terminating)
                    {
                        richTextBox_ConsoleOut.AppendText("A client has disconnected...\n");
                    }
                    usernames.Remove(username);
                    userPubKeys.Remove(username);
                    s.Close();
                    socketList.Remove(s);
                }
            }
            catch(Exception ex)//challenge-response failed
            {
                richTextBox_ConsoleOut.AppendText("Challenge-Response Exception: " + ex.Message);
                if (!terminating)
                {   
                    richTextBox_ConsoleOut.AppendText(String.Format("Challenge-response failed for {0}\n",username));
                }
                usernames.Remove(username);
                userPubKeys.Remove(username);
                s.Close();
                socketList.Remove(s);
                connected = false;
            }
           

            /*Part below is for post-authentication protocols*/
            while (connected && !terminating)
            {
                try
                {
                    // TODO: server sends a 128-bit random number to the client to initiate the challenge-response protocol.

                    Byte[] buffer = new Byte[64];
                    s.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));

                    // TODO: verify signature comes from "username" 
                    /*(If the server cannot verify the signature, it sends a negative acknowledgment message(signed) 
                     * to the client and closes the communication.)*/

                    // TODO: if client is authenticated, server generates a random 256-bit value which will be used as HMAC key
                    /*(The server encrypts this HMAC key with the RSA public key of the client. Then, the server signs this 
                     * encrypted HMAC key together with a positive acknowledgment message using server's own private RSA key. 
                     * After these operations, the server sends the encrypted HMAC key, the positive acknowledgment message and 
                     * the signature to the client.)
                     * The server should keep track of the session authentication keys for each client after a successful authentication phase*/

                    /* If authentication protocol fails, the connection must be closed,
                     * connection/authentication can be initiated again through the GUI.*/
                }
                catch
                {
                    if (!terminating)
                    {
                        richTextBox_ConsoleOut.AppendText("A client has disconnected\n");
                    }
                    usernames.Remove(username);
                    userPubKeys.Remove(username);
                    s.Close();
                    socketList.Remove(s);
                    connected = false;
                }
            }
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {
                    Socket newClient = serverSocket.Accept();
                    string username = "";
                    Byte[] buffer = new Byte[64];

                    // Receive username
                    newClient.Receive(buffer);  

                    string inMessage = Encoding.Default.GetString(buffer).Trim('\0');
                    CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(inMessage);
                    if (msg.msgCode == MessageCodes.Request)
                    {
                        username = msg.message;
                    }

                    int i = 0;
                    bool con = true;
                    while (i < usernames.Count() && con == true)
                    {
                        if (usernames[i] == username)   //if username is already exists in usernames list
                        {
                            richTextBox_ConsoleOut.AppendText("This client already exists!\n");
                            string message = createCommunicationMessage(MessageCodes.ErrorResponse, "User name", "You are already connected!\n");
                            sendMessage(newClient, message);    //sends message to client
                            newClient.Close();  // and closes the socket
                            con = false;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (con == true) //if username could not be found in the list
                    {
                        socketList.Add(newClient);
                        richTextBox_ConsoleOut.AppendText("A client is connected.\n");
                        string message = createCommunicationMessage(MessageCodes.SuccessfulResponse, "User name", "You connected succesfully!\n");
                        sendMessage(newClient, message);
                        usernames.Add(username);
                        addClientPubKey(username);
                        richTextBox_ConsoleOut.AppendText("Client username: " + username + "\n");
                        Thread receiveThread = new Thread(() => Receive(newClient, username));
                        receiveThread.Start();  //Login protocol initiates
                    }
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        richTextBox_ConsoleOut.AppendText("The socket stopped working \n");
                    }
                }
            }
        }


        // Helper Functions
        public void addClientPubKey (string userName)
        {
            string clientPublicKeyPath = mainRepositoryPath + "\\" + userName + "_pub.txt";
            try
            {
                string clientPubKey = File.ReadAllText(clientPublicKeyPath);
                userPubKeys.Add(userName, clientPubKey);
            }
            catch
            {
                richTextBox_ConsoleOut.AppendText("Error while reading " + userName + " public key");
            }
        }

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

        public void sendStringMessage(Socket receiver, String message)
        {
            // Not used anymore!
            Byte[] buffer = Encoding.Default.GetBytes(message);
            receiver.Send(buffer);
        }

        public string createCommunicationMessage(MessageCodes msgCode, string topic, string message)
        {
            CommunicationMessage msg = new CommunicationMessage
            {
                msgCode = msgCode,
                topic = topic,
                message = message
            };
            string msgJSON = JsonConvert.SerializeObject(msg);
            return msgJSON;
        }

        public void sendMessage(Socket s, string m)
        {
            Byte[] buffer = Encoding.Default.GetBytes(m);
            s.Send(buffer);
        }

        public void receiveMessage(Socket s, string incomingMessage)
        {
            // TODO: Implement the functionality
        }

        public string randomNumberGenerator(int length)
        {
            Byte[] bytesRandom = new Byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytesRandom);
            }
            string randomNumber = Encoding.Default.GetString(bytesRandom).Trim('\0');
            richTextBox_ConsoleOut.AppendText((length*8).ToString() + "-bit Random Number:\n" + generateHexStringFromByteArray(bytesRandom) + "\n"); // For debugging purposes
            return randomNumber;
        }


        /***** GUI Elements *****/
        private void button_listen_Click(object sender, EventArgs e)
        {
            int serverPort;

            if (serverPrivateKey == "" || serverPublicKey == "" || mainRepositoryPath == "")
            {
                richTextBox_ConsoleOut.AppendText("Please browse all files and folders first!\n");
            }
            else
            {
                if (Int32.TryParse(textBox_port_input.Text, out serverPort))
                {
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                    serverSocket.Listen(3);

                    listening = true;
                    button_listen.Enabled = false;
                    Thread acceptThread = new Thread(Accept);
                    acceptThread.Start();

                    richTextBox_ConsoleOut.AppendText("Started listening on port: " + serverPort + "\n");
                }
                else
                {
                    richTextBox_ConsoleOut.AppendText("Please check port number.\n");
                }
            }
        }

        private void ServerPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                string onlyFileName = fileName.Substring(fileName.LastIndexOf('\\')+1);
                textBox_serverPub.Text = onlyFileName;

                try
                {
                    serverPublicKey = File.ReadAllText(fileName);
                    byte[] byteServerPubKey = Encoding.Default.GetBytes(serverPublicKey);
                    string hexaServerPubKey = generateHexStringFromByteArray(byteServerPubKey);
                    richTextBox_ConsoleOut.AppendText("Server Public Key: " + hexaServerPubKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox_ConsoleOut.AppendText("Error while getting client public key " + ex.Message + "\n");
                }
            }
        }

        private void ServerPrivateKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                string onlyFileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                textBox_serverPriv.Text = onlyFileName;

                try
                {
                    serverPrivateKey = File.ReadAllText(fileName);
                    byte[] byteServerPrvKey = Encoding.Default.GetBytes(serverPrivateKey);
                    string hexaServerPrvKey = generateHexStringFromByteArray(byteServerPrvKey);
                    richTextBox_ConsoleOut.AppendText("Server Private Key: " + hexaServerPrvKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox_ConsoleOut.AppendText("Error while getting client public key " + ex.Message + "\n");
                }
            }
        }

        private void mainRepo_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                mainRepositoryPath = fbd.SelectedPath;
                string onlyFolderName = mainRepositoryPath.Substring(mainRepositoryPath.LastIndexOf('\\') + 1);
                textBox_mainRepo.Text = onlyFolderName;
            }
        }


        /***** Cryptographic Helper Functions *****/
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
        // encryption with AES-128
        static byte[] encryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
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

        // encryption with AES-192
        static byte[] encryptWithAES192(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-192
            aesObject.KeySize = 192;
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

        // encryption with AES-128
        static byte[] decryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            // aesObject.FeedbackSize = 128;
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

        // decryption with AES-192
        static byte[] decryptWithAES192(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-192
            aesObject.KeySize = 192;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            // aesObject.FeedbackSize = 128;
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
                result = rsaObject.SignData(byteInput, "SHA256");
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
                result = rsaObject.VerifyData(byteInput, "SHA512", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

    }
}
