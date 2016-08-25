using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using MJPEGDecoderWinRTLib;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Openhab.UWP.Controls
{
    public sealed partial class ImageWidget : WidgetBase
    {
        private BitmapImage _cameraBitmapImage;
        private MJPEGDecoder _mjpegDecoder;

        public BitmapImage CameraBitmapImage
        {
            get { return _cameraBitmapImage; }
            set
            {
                if (_cameraBitmapImage == value) return;
                _cameraBitmapImage = value;
                RaisePropertyChanged();
            }
        }

        public ImageWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _mjpegDecoder = new MJPEGDecoder();
            CameraBitmapImage = new BitmapImage();

            // Register listener methods
            _mjpegDecoder.FrameReady += mjpegDecoder_FrameReady;
            _mjpegDecoder.Error += mjpegDecoder_Error;

            // Construct Http Uri
            string requestUri = "http://jarvis:8888";
            
            // Tell MJPEGDecoder to connect to the IP camera, parse the mjpeg stream, and 
            // report the received image frames.
            await _mjpegDecoder.ParseStreamAsync(requestUri, "", "");
        }

        private async void mjpegDecoder_FrameReady(object sender, FrameReadyEventArgs e)
        {
            // Copy the received FrameBuffer to an InMemoryRandomAccessStream.
            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {

                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(e.FrameBuffer);
                    await writer.StoreAsync();
                }
                // Update source of CameraBitmap with the memory stream
                CameraBitmapImage.SetSource(ms);
            }
        }

        private void mjpegDecoder_Error(object sender, ErrorEventArgs e)
        {
            //ErrorMsg = e.Message;
        }
    }
}
