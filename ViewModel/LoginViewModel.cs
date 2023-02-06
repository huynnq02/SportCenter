using SportCenter.Helper;
using SportCenter.Model;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SportCenter.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        public bool IsLogin { get; set; }
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Role;
        public string Role
        {
            get => _Role; set
            {
                _Role = value; OnPropertyChanged();
            }
        }
        private string _NewPassword;
        public string NewPassword { get => _NewPassword; set { _NewPassword = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _ConfirmPassword; set { _ConfirmPassword = value; OnPropertyChanged(); } }
        private string _ConfirmPassword;
        public string CurrentPassword { get => _CurrentPassword; set { _CurrentPassword = value; OnPropertyChanged(); } }
        private string _CurrentPassword;
        public ICommand LoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand ChangepasswordCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public LoginViewModel()
        {

            IsLogin = false;
            Password = "";
            UserName = "";
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });
            ChangepasswordCommand = new RelayCommand<object>((parameter) => true, (parameter) => ChangePW());
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            RegisterCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Register(); });
        }

        private void ChangePW()
        {
            string currentpassEncode = CreateMD5(Base64Encode(CurrentPassword));
            var accCount1 = DataProvider.Ins.DB.accounts.Where(x => x.username == UserName && x.password == currentpassEncode).SingleOrDefault();


            if (accCount1 != null)
            {
                if (NewPassword == ConfirmPassword)
                {
                    accCount1.password = CreateMD5(Base64Encode(NewPassword));
                    MessageBox.Show("Password changed");
                    DataProvider.Ins.DB.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Confirm Password not match");
                }

            }
            else
            {
                MessageBox.Show("Wrong Password");
            }
        }
        private void Register()
        {
            RegisterScreen registerScreen = new RegisterScreen();
            registerScreen.Show();
        }
        void Login(Window p)
        {
            if (p == null)
                return;
            string passEncode = CreateMD5(Base64Encode(Password));
            var accCount = DataProvider.Ins.DB.accounts.Where(x => x.username == UserName && x.password == passEncode).Count();


            if (accCount > 0)
            {
                GlobalData.ClearData();
                account account = DataProvider.Ins.DB.accounts.Where(x => x.username == UserName).SingleOrDefault();
                IsLogin = true;
                GlobalData.isLoggedIn = true;
                GlobalData.username = UserName;
                GlobalData.id = account.id;
                GlobalData.role = account.role;
                GlobalData.name = account.name;
                EmployeeViewModel evm = new EmployeeViewModel();
                evm.LoadDataAfterLogin();
                /*  MainViewModel mainViewModel = MainViewModel.Instance;
                  if (GlobalData.role.Equals("Staff"))
                  {
                      mainViewModel.EmployeeTabVisibility = Visibility.Collapsed;
                  }*/
                MessageBox.Show(GlobalData.isLoggedIn + " " + GlobalData.username + " " + GlobalData.id + " " + GlobalData.role + " " + GlobalData.role.Equals("Staff"));

                p.Close();
            }
            else
            {
                IsLogin = false;
                GlobalData.isLoggedIn = false;
                MessageBox.Show("Incorrect Username or Password !");
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
