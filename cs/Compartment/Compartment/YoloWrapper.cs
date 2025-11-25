using Compartment;
using System;
using System.Runtime.InteropServices;

namespace Darknet
{
    public class YoloWrapper : IDisposable
    {
        private const string YoloLibraryName = "darknet.dll";

        private const int MaxObjects = 1000;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batchSize);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_rawimage")]
        private static extern int DetectRawImage(image_t rawimage, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int nSize, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_raw_image")]
        private static extern int DetectImage(IntPtr pArray, int w, int h, int c, ref BboxContainer container, double thresh);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [StructLayout(LayoutKind.Sequential)]
        public struct image
        {
            public Int32 w, h, c;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct bbox_t
        {
            public UInt32 x, y, w, h;    // (x,y) - top-left corner, (w, h) - width & height of bounded box
            public float prob;           // confidence - probability that the object was found correctly
            public UInt32 obj_id;        // class of object - from range [0, classes-1]
            public UInt32 track_id;      // tracking id for video (0 - untracked, 1 - inf - tracked object)
            public UInt32 frames_counter;
            public float x_3d, y_3d, z_3d;  // 3-D coordinates, if there is used 3D-stereo camera
        };
        public struct image_t
        {
            public int h;                        // height
            public int w;                        // width
            public int c;                        // number of chanels (3 - for RGB)
            public IntPtr data;                  // pointer to the image data
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct BboxContainer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxObjects)]
            public bbox_t[] candidates;
        }

        public YoloWrapper(string configurationFilename, string weightsFilename, int gpu)
        {
            InitializeYolo(configurationFilename, weightsFilename, gpu, 1);
        }

        public void Dispose()
        {
            DisposeYolo();
        }

        public int DetectCount { get => _detectCount.Value; private set => _detectCount.Value = value; }

        private SyncObject<int> _detectCount = new SyncObject<int>(-1);

        // byte列から検出を行い、検出数を返す
        public int DetectedCount(byte[] imageData, int width, int height, double threshold)
        {
            const int colorChannels = 3;
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, size);
                DetectCount = DetectImage(pnt, width, height, colorChannels, ref container, threshold);
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return DetectCount;
        }

        public bbox_t[] Detect(byte[] imageData, int width, int height, double threashold)
        {
            const int colorChannels = 3;
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, size);
                DetectCount = DetectImage(pnt, width, height, colorChannels, ref container, threashold);
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }

        public bbox_t[] Detect(string filename)
        {
            var container = new BboxContainer();
            container.candidates = new bbox_t[MaxObjects];
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }
        public bbox_t[] Detect(image_t image_T)
        {
            var container = new BboxContainer();
            var count = DetectRawImage(image_T, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();
            container.candidates = new bbox_t[MaxObjects];

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, size);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }
}
