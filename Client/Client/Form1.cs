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

using Client.Models;

namespace Client
{


    public partial class Form1 : Form
    {
        string name;
        bool terminating = false;
        bool connected = false;
        bool authenticated = false;
        Socket clientSocket;

        private string ServerKey = ""; // path to server's public key file (taken when browse1 clicked)
        private string UserPublicKey = "";// path to user's pub/prv key file (taken when browse2 clicked)
        private string UserEncryptedPrivateKey = "";
        private string UserPrivateKey = "";
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

        private void button_connect_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_IP_input.Text;
            int port;
            name = textBox_Username.Text;

            

            if (Int32.TryParse(textBox_Port_input.Text, out port))
            {
                try
                {
                    clientSocket.Connect(IP, port);
                    send_message(name,"User name",MessageCodes.Request); // send username to server and wait for uniqeness check
                    CommunicationMessage serverResponse = receiveOneMessage(); // Receive Uniqueness check
                    if (serverResponse.msgCode != MessageCodes.ErrorResponse) // if unique
                    {
                        button_connect.Enabled = false;
                        button_disconnect.Enabled = true;
                        button_Login.Enabled = true;
                        textBox_Password.Enabled = true;
                        connected = true;
                        richTextBox1.AppendText(serverResponse.message);
                        try
                        {
                            loadKeys(name);
                        }
                        catch(Exception ex)
                        {
                            richTextBox1.AppendText("Error while loading keys: " + ex.Message +"\n");
                        }
                    }
                    else // if username is already used 
                    {
                        richTextBox1.AppendText(serverResponse.message);
                        clientSocket.Close();
                        connected = false;
                    }


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

        private void loadKeys(string name)
        {
            using (System.IO.StreamReader fileReader = new System.IO.StreamReader("server_pub.txt"))
            {
                ServerKey = fileReader.ReadLine();
                byte[] byteServerKey = Encoding.Default.GetBytes(ServerKey);
                string hexaServerKey = generateHexStringFromByteArray(byteServerKey);
                richTextBox1.AppendText("Server Public Key: "+hexaServerKey + "\n");
            }

            string userPublicKeyFile = name + "_pub.txt";

            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(userPublicKeyFile))
            {
                UserPublicKey = fileReader.ReadLine();
                byte[] byteUserPubKey = Encoding.Default.GetBytes(UserPublicKey);
                string hexaUserPubKey = generateHexStringFromByteArray(byteUserPubKey);
                richTextBox1.AppendText("User Public Key: " + hexaUserPubKey + "\n");
            }

            string userEncryptedFileName = "enc_" + name + "_pub_prv.txt";

            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(userEncryptedFileName))
            {
                UserEncryptedPrivateKey = fileReader.ReadLine();
                richTextBox1.AppendText("User Encrypted Private Key: " + UserEncryptedPrivateKey + "\n");
            }


        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            clientSocket.Close();
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



        private void button_Login_Click(object sender, EventArgs e)// Login protocol 
        {

            if (connected)
            {
                string pass = textBox_Password.Text;

                //Password hashing and creating AES-256 Key and IV
                byte[] hashedPassword = hashWithSHA384(pass);
                Array.Copy(hashedPassword, 0, AES256Key, 0, 32);
                Array.Copy(hashedPassword, 32, AES256IV, 0, 16);
                string hexaDecimalAES256Key = generateHexStringFromByteArray(AES256Key);
                string hexaDecimalAES256IV = generateHexStringFromByteArray(AES256IV);
                richTextBox1.AppendText("AES 256 Key: " +hexaDecimalAES256Key+ "\n");
                richTextBox1.AppendText("AES 256 IV: " + hexaDecimalAES256IV + "\n");


                try
                {
                    //Decrypt the Private Key using AES-256 
                    byte[] decryptedPasswordBytes = decryptWithAES256HexVersion(UserEncryptedPrivateKey, AES256Key, AES256IV);
                    UserPrivateKey = Encoding.Default.GetString(decryptedPasswordBytes);
                    string hexaPrivateKey = generateHexStringFromByteArray(decryptedPasswordBytes);
                    richTextBox1.AppendText("User Private Key: " + UserPrivateKey + "\n");

                    //Get Random Number from Server
                    CommunicationMessage randomNumberMessage = receiveOneMessage();
                    string randomNumber = randomNumberMessage.message;
                    byte[] randomNumberBytes = Encoding.Default.GetBytes(randomNumber);
                    richTextBox1.AppendText("Random Number:" + generateHexStringFromByteArray(randomNumberBytes));

                    //Sign the Random Number
                    byte[] signedNonce = signWithRSA(randomNumber, 4096, UserPrivateKey);
                    richTextBox1.AppendText("Nonce Length"+signedNonce.Length.ToString());
                    //clientSocket.Send(signedNonce);//Challenge-response phase 1 initiated here


                   

                }
                catch(Exception ex)
                {
                    AES256Key = new byte[32];
                    AES256IV = new byte[16];
                    richTextBox1.AppendText("Wrong password. Please try again.\n");
                }
           

                // TODO: login protocol

                /*The encrypted 4096-bit RSA private key, is decrypted using the hash of the entered password 
                 * 
                 * (The RSA-4096 private key of each user is given in encrypted form. 
                 * The encryption is done using AES-256 in CFB mode. For the key and IV of this encryption, 
                 * SHA-384 hash of the password is used, first 32 bytes of the hash output, i.e. the byte array indices [0…31], 
                 * being the key and last 16 bytes of the hash output, i.e. the byte array indices [32…47], being the IV.)
                 * 
                 * if the password is entered correctly, we obtain the decrypted RSA private key. 
                 * !! should not store this decrypted private key in any file; just keep it in memory during a session.
                 * If the password is entered wrong, the decryption operation would fail (probably by throwing an exception) 
                 * and user understands that he/she entered the password wrong. 
                 * (inform the user about the wrong password and ask for it again)*/

                // TODO: handle receiver thread 
                // in receiver thread;
                // TODO: receive 128 bit random number 
                /* the client signs this random number using his/her private RSA key and sends this signature to the server. 
                 * The hash algorithm used in signature is SHA-512.*/

                // TODO: verify the signature comes from server
                /*If verified, the client will decrypt the HMAC key using his/her own private RSA key and store it in the memory*/

                /* If authentication protocol fails, the connection must be closed,
                 * connection/authentication can be initiated again through the GUI.*/
            }



        }

        private void send_message(string message, string topic, MessageCodes code) //sends username
        {
            CommunicationMessage msg = new CommunicationMessage();
            msg.topic = topic;
            msg.message = message;
            msg.msgCode = code;
            string jsonObject = JsonConvert.SerializeObject(msg);
            byte[] buffer = Encoding.Default.GetBytes(jsonObject);
            clientSocket.Send(buffer);
        }

        private CommunicationMessage receiveOneMessage()
        {
            Byte[] buffer = new Byte[128];
            clientSocket.Receive(buffer);
            string incomingMessage = Encoding.Default.GetString(buffer).Trim('\0');
            CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(incomingMessage);
            return msg;
        }
        private void button_disconnect_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText("You disconnected\n");
            connected = false;
            button_connect.Enabled = true;
            button_disconnect.Enabled = false;
            button_Login.Enabled = false;
            textBox_Password.Enabled = false;
            clientSocket.Close();

        }


        // helper functions
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

        private void textBox_message_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
