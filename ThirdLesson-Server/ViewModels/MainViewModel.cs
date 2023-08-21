using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ThirdLesson_Server.Commands;
using System.IO;

namespace ThirdLesson_Server.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public BitmapImage ToImage(byte[] array)
        {
            try
            {
                using (var ms = new System.IO.MemoryStream(array))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private BitmapImage currentImage;

        public BitmapImage CurrentImage
        {
            get { return currentImage; }
            set { currentImage = value; OnPropertyChanged(); }
        }
        public RelayCommand ConnectClickCommand { get; set; }
        private BitmapImage CreateBitmapImageFromBytes(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }
        public MainViewModel()
        {
            ConnectClickCommand = new RelayCommand(async (obj) =>
            {
                var ip = IPAddress.Parse("192.168.0.106");
                var port = 27001;

                EndPoint ep = new IPEndPoint(ip, port);

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(ep);

                MessageBox.Show("Listening . . .");

                await Task.Run(() =>
                {
                    while (true)
                    {
                        var bytes = new byte[socket.ReceiveBufferSize];
                        var length = socket.ReceiveFrom(bytes, ref ep);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BitmapImage msg = CreateBitmapImageFromBytes(bytes);
                            CurrentImage = msg;
                        });
                    }
                });

            });


        }
    }
}
