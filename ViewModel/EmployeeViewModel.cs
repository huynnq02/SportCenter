using Microsoft.Win32;
using SportCenter.Helper;
using SportCenter.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SportCenter.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        private ObservableCollection<account> _Listemployee;
        public ObservableCollection<account> Listemployee { get => _Listemployee; set { _Listemployee = value; OnPropertyChanged(); } }

        //Storage VM

        private string imageFileName;
        private byte?[] _imageemployee;
        public byte?[] imageemployee { get => _imageemployee; set { _imageemployee = value; OnPropertyChanged(); } }
        public Func<double, string> YFormatter { get; set; }
        private int _idemployee;
        public int idemployee { get => _idemployee; set { _idemployee = value; OnPropertyChanged(); } }

        private string _nameemployee;
        public string nameemployee { get => _nameemployee; set { _nameemployee = value; OnPropertyChanged(); } }

        private decimal? _salaryemployee;
        public decimal? salaryemployee { get => _salaryemployee; set { _salaryemployee = value; OnPropertyChanged(); } }

        private string _roleemployee;
        public string roleemployee { get => _roleemployee; set { _roleemployee = value; OnPropertyChanged(); } }

        private string _phoneemployee;
        public string phoneemployee { get => _phoneemployee; set { _phoneemployee = value; OnPropertyChanged(); } }
        private string _username;
        public string username { get => _username; set { _username = value; OnPropertyChanged(); } }
        private string _password;
        public string password { get => _password; set { _password = value; OnPropertyChanged(); } }
        private string _confirmPassword;
        public string confirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        private DateTime _dateOfBirthemployee = new DateTime(2000, 1, 1);
        public DateTime dateOfBirthemployee { get => _dateOfBirthemployee; set { _dateOfBirthemployee = value; OnPropertyChanged(); } }

        public ICommand addCommand { get; set; }
        public ICommand editCommand { get; set; }
        public ICommand deleteCommand { get; set; }
        public ICommand ReloadCommand { get; set; }

        public ICommand OpenAddEmployeeWindow { get; set; }

        public ICommand SelectImageCommand { get; set; }

        private account _SelectedItem;
        public account SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    idemployee = SelectedItem.id;
                    nameemployee = SelectedItem.name;
                    salaryemployee = SelectedItem.salary;
                    roleemployee = SelectedItem.role;
                    phoneemployee = SelectedItem.phoneNumber;
                    dateOfBirthemployee = (DateTime)SelectedItem.dateOfBirth;
                    if (SelectedItem.imageFile != null)
                    {
                        byte[] imageBytes = SelectedItem.imageFile;
                        int length = imageBytes.Length;
                        byte?[] imageNullables = new byte?[length];
                        for (int i = 0; i < length; i++)
                        {
                            imageNullables[i] = imageBytes[i];
                        }
                        imageemployee = imageNullables;
                    }
                    else
                    {
                        imageemployee = null;
                    }
                }
            }
        }
        public EmployeeViewModel()
        {
            Listemployee = new ObservableCollection<account>(DataProvider.Ins.DB.accounts);
            /*            LoadEmployeeData();
            */
            OpenAddEmployeeWindow = new RelayCommand<object>((parameter) => true, (parameter) => f_Open_Add_employee());
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => ChooseImage(parameter));
            ReloadCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => ReloadEmployee(parameter));
            // Add goods
            addCommand = new RelayCommand<Grid>((parameter) =>
            {
                if (nameemployee == null || (nameemployee.Trim().Equals("")) || salaryemployee == null ||
     roleemployee == null || (roleemployee.Trim().Equals("")) ||
     phoneemployee == null || (phoneemployee.Trim().Equals("")) ||
     username == null || (username.Trim().Equals("")) ||
     password == null || (password.Trim().Equals("")) ||
     confirmPassword == null || (confirmPassword.Trim().Equals("")) ||
     dateOfBirthemployee == null)
                    return false;
                return true;

            },
            (parameter) => AddEmployee(parameter));


            //Edit goods
            editCommand = new RelayCommand<Grid>((parameter) =>
            {

                if (string.IsNullOrEmpty(nameemployee) || SelectedItem == null)
                    return false;
                var nameList = DataProvider.Ins.DB.accounts.Where(p => p.id == idemployee);
                if (nameList != null && nameList.Count() != 0)
                    return true;
                return false;
            }, (parameter) =>
            {
                MessageBoxResult result = MessageBox.Show("Confirm edit?", "Note", MessageBoxButton.YesNo);
                Listemployee = new ObservableCollection<account>(DataProvider.Ins.DB.accounts);

                if (result == MessageBoxResult.Yes)
                {
                    if (imageFileName != null)
                    {
                        byte[] imgByteArr;
                        imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                        var employee = DataProvider.Ins.DB.accounts.Where(x => x.id == SelectedItem.id).SingleOrDefault();
                        employee.name = nameemployee;
                        employee.role = roleemployee;
                        employee.phoneNumber = phoneemployee;
                        employee.imageFile = imgByteArr;
                        employee.dateOfBirth = dateOfBirthemployee;
                        employee.salary = salaryemployee;

                        DataProvider.Ins.DB.SaveChanges();
                        parameter.Background = null;

                        ImageBrush image = new ImageBrush();
                        image.ImageSource = ByteToImage(employee.imageFile);
                        parameter.Background = image;
                        parameter.Children[0].Visibility = Visibility.Visible;
                        //parameter.Children[2].Visibility = Visibility.Visible;
                        MessageBox.Show("Edit employee successfully!!");
                    }
                    else
                    {
                        var employee = DataProvider.Ins.DB.accounts.Where(x => x.id == SelectedItem.id).SingleOrDefault();
                        employee.name = nameemployee;
                        employee.role = roleemployee;
                        employee.phoneNumber = phoneemployee;
                        employee.dateOfBirth = dateOfBirthemployee;
                        employee.salary = salaryemployee;


                        DataProvider.Ins.DB.SaveChanges();
                        ClearEmployeeData();
                        LoadEmployeeData();
                        MessageBox.Show("Edit employee successfully!!");
                    }
                }

            });

            //Delete goods
            deleteCommand = new RelayCommand<Grid>((parameter) => true,
            (parameter) =>
            {
                MessageBoxResult result = MessageBox.Show("Confirm delete?", "Note", MessageBoxButton.YesNo);

                Listemployee = new ObservableCollection<account>(DataProvider.Ins.DB.accounts);
                if (result == MessageBoxResult.Yes)
                {
                    var employee = DataProvider.Ins.DB.accounts.Where(x => x.id == SelectedItem.id).SingleOrDefault();
                    DataProvider.Ins.DB.accounts.Remove(employee);
                    DataProvider.Ins.DB.SaveChanges();
                    Listemployee.Remove(employee);
                    idemployee = 0;
                    nameemployee = null;
                    salaryemployee = null;
                    phoneemployee = null;
                    roleemployee = null;
                    dateOfBirthemployee = DateTime.Now;
                    MessageBox.Show("Delete employee successfully!!");
                }
            });
        }

        public void ReloadEmployee(Grid parameter)
        {

            foreach (var item in Listemployee)
            {
                if (item == SelectedItem)
                {
                    ImageBrush image = new ImageBrush
                    {
                        ImageSource = ByteToImage(item.imageFile)
                    };
                    parameter.Background = image;
                }
            }
        }
        public void LoadDataAfterLogin()
        {
            ClearEmployeeData();
            LoadEmployeeData();
        }
        public static ImageSource ByteToImage(byte[] imageData)
        {
            if (imageData == null)
            {
                return null;
            }
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        private void f_Open_Add_employee()
        {
            Add_employee rp = new Add_employee();
            rp.Show();
        }
        private void ChooseImage(Grid parameter)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Choose picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                using (var stream = new FileStream(imageFileName, FileMode.Open))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        int length = imageBytes.Length;
                        byte?[] imageNullables = new byte?[length];
                        for (int i = 0; i < length; i++)
                        {
                            imageNullables[i] = imageBytes[i];
                        }
                        imageemployee = imageNullables;
                    }
                }
                imageBrush.ImageSource = bitmap;
                parameter.Background = imageBrush;
                //if (parameter.Children.Count > 1)
                //{
                //    parameter.Children[0].Visibility = Visibility.Hidden;
                //    parameter.Children[2].Visibility = Visibility.Hidden;

                //}
            }
        }

        internal void LoadEmployeeData()
        {

            Listemployee = new ObservableCollection<account>();
            var listemployee = DataProvider.Ins.DB.accounts;
            foreach (var item in listemployee)
            {
                _ = new employee();

                account Storage = item;
                if (Storage.username.Equals(GlobalData.username))
                {
                    Storage.name += " (You)";
                }
                Listemployee.Add(Storage);
            }

        }
        internal void ClearEmployeeData()
        {

            Listemployee.Clear();

        }
        public void AddEmployee(Grid parameter)
        {
            {



                Listemployee = new ObservableCollection<account>(DataProvider.Ins.DB.accounts);
                var accCount = DataProvider.Ins.DB.accounts.Where(x => x.username == username).Count();
                if (nameemployee == null || (nameemployee.Trim().Equals("")) || salaryemployee == null ||
    roleemployee == null || (roleemployee.Trim().Equals("")) ||
    phoneemployee == null || (phoneemployee.Trim().Equals("")) ||
    username == null || (username.Trim().Equals("")) ||
    password == null || (password.Trim().Equals("")) ||
    confirmPassword == null || (confirmPassword.Trim().Equals("")) ||
    dateOfBirthemployee == null)
                {
                    MessageBox.Show("Please fill all the blanks!!");
                }

                else
                {
                    if (accCount > 0)
                    {
                        MessageBox.Show("Username already exists!!");
                    }
                    else
                    {
                        if (password != confirmPassword)
                        {
                            MessageBox.Show("Password does not match!!");

                        }
                        else
                        {
                            byte[] imgByteArr;
                            imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                            var employee = new account()
                            {
                                name = nameemployee,
                                id = idemployee,
                                salary = salaryemployee,
                                phoneNumber = phoneemployee,
                                dateOfBirth = dateOfBirthemployee,
                                role = roleemployee,
                                imageFile = imgByteArr,
                                username = username,
                                password = CreateMD5(Base64Encode(password))

                            };
                            DataProvider.Ins.DB.accounts.Add(employee);
                            DataProvider.Ins.DB.SaveChanges();
                            Listemployee.Add(employee);
                            idemployee = 0;
                            nameemployee = null;
                            phoneemployee = null;
                            salaryemployee = null;
                            roleemployee = null;
                            dateOfBirthemployee = DateTime.Now;
                            parameter.Background = null;
                            /*   parameter.Children[0].Visibility = Visibility.Visible;


                               parameter.Children[2].Visibility = Visibility.Visible;*/
                            MessageBox.Show("Add employee successfully!!");
                        }
                    }
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
