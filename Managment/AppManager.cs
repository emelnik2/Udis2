namespace Managment
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Net.NetworkInformation;
    using System.Security.AccessControl;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    public class AppManager
    {

        private string _lic;


        const string SECURITY = "code hash";
        public string Server { get; set; }
        public string AppPath { get; set; }


        private static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = SECURITY;
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = SECURITY;

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        string GetAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            var adapter = nics.FirstOrDefault();
            IPInterfaceProperties properties = adapter.GetIPProperties();
            sMacAddress = adapter.GetPhysicalAddress().ToString();
            return sMacAddress;
        }

        /// <summary>
        /// Get concatination of Processor and MotherboardSerial Key
        /// </summary>
        /// <returns></returns>
        private static string GetUniqueSysID()
        {

            //string sProcessorID = string.Empty;
            string motherBoardSerialNumber = string.Empty;
            string physicalIP = string.Empty;

            //string sQuery = "SELECT ProcessorId FROM Win32_Processor";

            //ManagementObjectSearcher oManagementObjectSearcher = new ManagementObjectSearcher(sQuery);

            //ManagementObjectCollection oCollection = oManagementObjectSearcher.Get();

            //foreach (ManagementObject oManagementObject in oCollection)

            //{

            //    sProcessorID = (string)oManagementObject["ProcessorId"];

            //}

            ManagementObjectSearcher MOS = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");


            foreach (ManagementObject getserial in MOS.Get())
            {
                if (getserial["SerialNumber"] != null)
                    motherBoardSerialNumber = getserial["SerialNumber"].ToString().Contains('.') ? getserial["SerialNumber"].ToString().Split('.')[1] : getserial["SerialNumber"].ToString();
               
            }

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                //  if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.Name.Equals("Ethernet"))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        physicalIP = nic.GetPhysicalAddress().ToString();
                    }
                }
            }

            return (string.Concat(physicalIP, motherBoardSerialNumber));

        }

        public bool IsValid()
        {
            _lic = ReadCurrentLicense();
            //var mac = GetAddress();
            var uniqueId = GetUniqueSysID();
            if (!string.IsNullOrEmpty(_lic))
            {

                var decryptedCurrentLicense = Decrypt(_lic, true);
                if (decryptedCurrentLicense.IndexOf('_') > 0)
                {
                    var splitLic = decryptedCurrentLicense.Split('_');
                    if (splitLic[0].Equals(uniqueId) && splitLic[1].Equals(Server) && Convert.ToDateTime(splitLic[2]) >= System.DateTime.Now)
                    {
                        return true;
                    }
                }
                //else if (decryptedCurrentLicense.IndexOf('_') == -1)
                //{
                //    var newLic = string.Concat(uniqueId, "_", Server);
                //    WriteCurrentLicense(Encrypt(newLic, true));
                //    return true;
                //}
            }
            //else
            //{
            //    var newLic = string.Concat(uniqueId, "_", Server);
            //    WriteCurrentLicense(Encrypt(newLic, true));
            //    return true;
            //}

            return false;
        }

        public void CreateLicenseFile(string motherBoardSerialNumber, string physicalIP, string licKey, string validupto)
        {
            var newLic = string.Concat(physicalIP, motherBoardSerialNumber, "_", licKey, "_", validupto);
            //WriteCurrentLicense(Encrypt(newLic, true));
            string lic = Encrypt(newLic, true);
            var path = AppPath;
            File.WriteAllText(path, lic);
        }
        
        private string ReadCurrentLicense()
        {
            var path = AppPath;
            if (File.Exists(path))
                return File.ReadAllText(path);

            return null;
        }

        private void WriteCurrentLicense(string lic)
        {
            var path = AppPath;
            if (File.Exists(path))
                File.WriteAllText(path, lic);
        }
        
        static public byte[] Encryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey); encryptedData = RSA.Encrypt(Data, DoOAEPPadding);
                    var val = RSA.ToXmlString(false);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public byte[] Decryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    decryptedData = RSA.Decrypt(Data, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private static void AddFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {


            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);

        }

        public static void RestrictEveryoneFileSecurity(string fileName)
        {


            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(everyone,
                FileSystemRights.Read, AccessControlType.Deny));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);

        }
    }
}
