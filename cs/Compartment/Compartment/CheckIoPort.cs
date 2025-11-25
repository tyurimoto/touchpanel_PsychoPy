using System.Windows.Forms;

namespace Compartment
{

    public class CheckIoOutPort
    {
        // コンストラクタ
        public CheckIoOutPort(IoBoardPortNo a_IoBoardPortNoPort, ushort a_ushortBitCode, CheckBox a_CheckBoxPort)
        {
            IoBoardPortNoPort = a_IoBoardPortNoPort;
            ushortBitCode = a_ushortBitCode;
            CheckBoxPort = a_CheckBoxPort;
        }
        public IoBoardPortNo IoBoardPortNoPort { get; set; }
        public ushort ushortBitCode { get; set; }
        public CheckBox CheckBoxPort { get; set; }
    }
    public class CheckIoInPort
    {
        // コンストラクタ
        public CheckIoInPort(IoBoardPortNo a_IoBoardPortNoPort, ushort a_ushortBitCode, TextBox a_TextBoxPort)
        {
            IoBoardPortNoPort = a_IoBoardPortNoPort;
            ushortBitCode = a_ushortBitCode;
            TextBoxPort = a_TextBoxPort;
        }
        public IoBoardPortNo IoBoardPortNoPort { get; set; }
        public ushort ushortBitCode { get; set; }
        public TextBox TextBoxPort { get; set; }
    }
}
