using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using IpCam.Device;
using IpCam.Media;
using IpCam.Media2;
using System.IO;
using System.Collections.ObjectModel;

namespace IpCam
{
    public partial class Form1 : Form
    {
        static DiscoveryEndpoint endpoint;
        DiscoveryClient discoveryClient;
        FindCriteria findCriteria;

        

        ObservableCollection<UriBuilder> deviceUris = new ObservableCollection<UriBuilder>();
        ObservableCollection<Profile> profiles = new ObservableCollection<Profile>();
        MediaClient media;
        Media2Client media2;

        // vlcコントロール用
        DirectoryInfo vlcPath = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));


        public Form1()
        {
            InitializeComponent();

            // WS-Discoveryの機能を利用してOnvif対応カメラを探す
            endpoint = new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryApril2005);
            discoveryClient = new DiscoveryClient(endpoint);
            discoveryClient.FindProgressChanged += (object sender, FindProgressChangedEventArgs e) => {
                Console.WriteLine(e.EndpointDiscoveryMetadata.ListenUris.FirstOrDefault());
                deviceUris.Add(new UriBuilder(e.EndpointDiscoveryMetadata.ListenUris.FirstOrDefault()));
            };

            findCriteria = new FindCriteria();
            findCriteria.Duration = TimeSpan.MaxValue;
            findCriteria.MaxResults = 20;
            findCriteria.ContractTypeNames.Add(new System.Xml.XmlQualifiedName("NetworkVideoTransmitter", "http://www.onvif.org/ver10/network/wsdl"));

            discoveryClient.FindAsync(findCriteria);


            // デバイスの追加・削除・変更時
            deviceUris.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                comboBox2.Items.Clear();
                foreach(var uri in deviceUris)
                {
                    comboBox2.Items.Add("CAM" + comboBox2.Items.Count);
                }
            };

            // プロファイルの追加・削除・変更時
            profiles.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                comboBox1.Items.Clear();
                foreach (var profile in profiles)
                {
                    comboBox1.Items.Add(profile.Name);
                }
                if(comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0;
                }
            };

            vlcControl1.VlcMediaPlayer.Log += VlcMediaPlayer_Log;
            vlcControl1.VlcMediaPlayer.Stopped += VlcMediaPlayer_Stopped;
            vlcControl1.EndInit();
        }

        private void VlcMediaPlayer_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
        }

        private void VlcMediaPlayer_Log(object sender, Vlc.DotNet.Core.VlcMediaPlayerLogEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("libVlc : {0} {1} @ {2}", e.Level, e.Message, e.Module));
        }


        // Onvif device_serviceよりストリーミングのプロファイルを取得する
        private void ConnectCam(UriBuilder deviceUri)
        {
            System.ServiceModel.Channels.Binding binding;
            HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
            httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
            binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), httpTransport);

            try
            {
                Device.DeviceClient device = new Device.DeviceClient(binding, new EndpointAddress(deviceUri.ToString()));
                Device.Service[] services = device.GetServices(false);

                if (services.Any(s => s.Namespace == "http://www.onvif.org/ver10/media/wsdl"))
                {
                    media2 = null;
                    media = new MediaClient(binding, new EndpointAddress(deviceUri.ToString()));
                    profiles.Clear();

                    foreach (Media.Profile profile in media.GetProfiles())
                    {
                        profiles.Add(profile);
                    }
                }
                else if (services.Any(s => s.Namespace == "http://www.onvif.org/ver10/media/wsdl"))
                {
                    media = null;
                    media2 = new Media2Client(binding, new EndpointAddress(deviceUri.ToString()));
                    profiles.Clear();

                    foreach (Media2.MediaProfile profile in media2.GetProfiles(null, null))
                    {
                        Console.WriteLine(profile.Name);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        // 選択されているプロファイルでストリーミング
        private void StreamVideoOnVLC(String[] recordParams)
        {
            if (media != null)
            {
                var setup = new Media.StreamSetup();
                setup.Stream = Media.StreamType.RTPMulticast;
                setup.Transport = new Media.Transport();
                setup.Transport.Protocol = Media.TransportProtocol.RTSP;
                UriBuilder uri = new UriBuilder(media.GetStreamUri(setup, profiles.ElementAt(comboBox1.SelectedIndex).token).Uri);

                vlcControl1.VlcMediaPlayer.Play(uri.Uri, recordParams);
            }
            else if (media2 != null)
            {
                UriBuilder uri = new UriBuilder(media2.GetStreamUri("RtspOverHttp", profiles.ElementAt(comboBox1.SelectedIndex).token));

                vlcControl1.VlcMediaPlayer.Play(uri.Uri, recordParams);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var option = new List<String>();
            option.Add("--demux h264");
            StreamVideoOnVLC(option.ToArray());
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            e.VlcLibDirectory = vlcPath;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectCam(deviceUris.ElementAt(comboBox2.SelectedIndex));
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
