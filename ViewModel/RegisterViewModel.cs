using SportCenter.Model;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SportCenter.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _ConfirmPassword; set { _ConfirmPassword = value; OnPropertyChanged(); } }
        private string _ConfirmPassword;

        public ICommand PasswordChangedCommand { get; set; }
        public ICommand ConfirmPasswordChangedCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public RegisterViewModel()
        {
            Password = "";
            UserName = "";
            ConfirmPassword = "";
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            ConfirmPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { ConfirmPassword = p.Password; });

            RegisterCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Register(); });
        }
        private void Register()
        {
            var accCount = DataProvider.Ins.DB.accounts.Where(x => x.username == UserName).SingleOrDefault();

            if (UserName.Trim() == "" || Password.Trim() == "" || ConfirmPassword.Trim() == "")
            {
                MessageBox.Show("Please fill all the blank!!");
            }

            else if (accCount != null)
            {
                MessageBox.Show("This username already exists!!");
            }
            else if (accCount == null)
            {
                if (Password != ConfirmPassword)
                {
                    MessageBox.Show("Password does not match!!");
                }
                else
                {
                    account newAccount = new account();
                    newAccount.username = UserName;
                    newAccount.password = CreateMD5(Base64Encode(Password));
                    newAccount.role = "owner";
                    DataProvider.Ins.DB.accounts.Add(newAccount);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Create success.");
                }

            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
