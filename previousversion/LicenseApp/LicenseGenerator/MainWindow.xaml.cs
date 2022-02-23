using Managment;
using System;
using System.IO;
using System.Linq;
using System.Windows;


namespace LicenseGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string LicenseFN = "app.dll";
        private const string AppSettingKey = "server";
        AppManager appManager = new AppManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var path = dialog.SelectedPath;
                    txtLocation.Text = path;
                }

            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlLicKey.Text = GetLicense(10);
        }

        private static Random random = new Random();

        public static string GetLicense(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtBlLicKey.Text = GetLicense(10);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtBlLicKey.Text = GetLicense(10);
            txtLocation.Text = string.Empty;
            txtPhysicalAdd.Text = string.Empty;
            txtSrlNumber.Text = string.Empty;
            txtClientName.Text = string.Empty;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var location = txtLocation.Text;
            var licenseKey = txtBlLicKey.Text;
            var physicalAdd = txtPhysicalAdd.Text;
            var licenseName = txtClientName.Text;
            var srlNumber = txtSrlNumber.Text;
            var validUpto = dtpValidUpto.Text;


            if ((!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(licenseKey) && !string.IsNullOrEmpty(physicalAdd) && !string.IsNullOrEmpty(srlNumber)))
            {
                System.Windows.Forms.DialogResult messageBoxResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Are you sure you are ready to generate License with input details?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == System.Windows.Forms.DialogResult.Yes)
                {
                    if (physicalAdd.Contains("-"))
                    {
                        physicalAdd = physicalAdd.Replace("-", string.Empty);
                    }

                    UpdateConfigFile(location, AppSettingKey, licenseKey);
                    CreateFile(location, licenseKey, physicalAdd, srlNumber, validUpto);
                    CreateLicenseRecord(licenseName, licenseKey, physicalAdd, srlNumber, validUpto);
                    MessageBox.Show("Licesense Generated successfully please proceed with deployment.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetForm();
                }
            }
            else
            {
                MessageBox.Show("Please provide required fields", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CreateLicenseRecord(string licenseName, string licenseKey, string physicalAdd, string srlNumber, string validUpto)
        {
            string newFileName = string.Concat(System.AppDomain.CurrentDomain.BaseDirectory, "\\client_License_details.csv");

            string clientDetails = string.Concat(licenseName, ",", licenseKey, ",", physicalAdd, ",", srlNumber, ",", DateTime.UtcNow.ToShortDateString(), validUpto, Environment.NewLine);


            if (!File.Exists(newFileName))
            {
                string clientHeader = string.Concat("Client_Name", ",", "License_Key", ",", "Machine_Physcal_Address", ",", "Serial_Number", ",", "Generated_Date","Valid_Upto",Environment.NewLine);

                File.WriteAllText(newFileName, clientHeader);
            }

            File.AppendAllText(newFileName, clientDetails);
        }

        private void CreateFile(string location, string licenseKey, string physicalAdd, string srlNumber,string validUpto)
        {
            appManager.AppPath = string.Concat(location, "\\bin\\", LicenseFN);
            appManager.CreateLicenseFile(srlNumber, physicalAdd, licenseKey, validUpto);
        }

        private void UpdateConfigFile(string appConfigPath, string key, string value)
        {
            var webConfigPath = string.Concat(appConfigPath, "\\Web.config");
            if (File.Exists(webConfigPath))
            {
                var appConfigContent = File.ReadAllText(webConfigPath);
                var searchedString = $"<add key=\"{key}\" value=\"";
                var index = appConfigContent.IndexOf(searchedString) + searchedString.Length;
                var currentValue = appConfigContent.Substring(index, appConfigContent.IndexOf("\"", index) - index);
                var newContent = appConfigContent.Replace($"{searchedString}{currentValue}\"", $"{searchedString}{value}\"");
                File.WriteAllText(webConfigPath, newContent);
            }
        }



    }
}
