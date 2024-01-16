using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using PsychoLab.Model;

namespace PsychoLab.Views.Windows.AdminWindows
{
    /// <summary>
    /// Interaction logic for ProfileUserView.xaml
    /// </summary>
    public partial class ProfileUserView : Window
    {
        public User User { get; set; }
        public ProfileUserView(User user)
        {
            InitializeComponent();
            User = user;
            lblFirstname.Content = User.FirstName;
            lblLastname.Content = User.LastName;
            lblMiddlename.Content = User.MiddleName;
            lblEmail.Content = User.Email;
            lblPassword.Content = User.Password;
            lblCreateAt.Content = User.CreatedAt;
            lblUpdateAt.Content = User.UpdatedAt;
            LoadImageFromDatabase(User);
        }
        private void LoadImageFromDatabase(User user)
        {
            if (user.PicUser != null && user.PicUser.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(user.PicUser))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();

                    Dispatcher.Invoke(() => {
                        picUser.Source = image;
                    });
                }
            }
        }
        private void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
