using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSaA_2425_LINQDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        DataClasses1DataContext _loginDB = 
            new DataClasses1DataContext(Properties.Settings.Default.DSaA_LoginSampleConnectionString);
        int failCount = 0;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            int uID = 0;
            //string pass = "";
            bool fail = false;

            if(txtbUserName.Text.Length >0 && txtbPassword.Text.Length >0 
                && int.TryParse(txtbUserName.Text, out uID))
            {
                
                List<uspCheckResult> things = _loginDB.uspCheck(uID).ToList();
                if (things.Count() == 1)
                {
                    //MessageBox.Show("Username exists!");
                    foreach (uspCheckResult th in things)
                    {
                        if (th.password == txtbPassword.Text)
                        {
                            List<uspLoginSuccessResult> login = _loginDB.uspLoginSuccess(uID).ToList();
                            foreach (uspLoginSuccessResult lr in login)
                            {
                                lblMessage.Content = $"Welcome {lr.AccessTypeName} {lr.Name}";
                                _loginDB.uspUpdateLoginTime(uID);
                                _loginDB.uspLog(uID, "Login Success!");
                            }
                        }
                        else
                        {
                            _loginDB.uspLog(uID, "Login Attempt made!");
                            failCount++;
                            fail = true;
                        }
                    }
                }
                else
                {
                    lblMessage.Content = "Username does not exist!";
                    //MessageBox.Show("Username does not exist!");
                    failCount++;
                    fail = true;
                }

                if(fail)
                    lblMessage.Content = $"Login Attempt: {failCount}";

                if(failCount == 5)
                {
                    lblMessage.Content = $"Login Disabled";
                    btnLogin.IsEnabled = false;
                }
            }
        }
    }
}
