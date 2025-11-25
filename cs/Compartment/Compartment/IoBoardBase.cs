
using NAudio.Dsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compartment.IoMicrochip;

namespace Compartment
{
    public class IoBoardBase
    {
        public string errorMsg { get; set; }

        public virtual bool AcquireDevice() { return false; }
        public virtual bool ReleaseDevice() { return false; }
        public virtual bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode) { return false; }
        public virtual bool DirectIn(IoBoardPortNo a_IoBoardPortNoObj, out ushort a_ushortInCode) { a_ushortInCode = 0x00; return false; }

        public virtual bool SaveDIn() { return false; }
        public virtual bool GetRecieveData(int index) { return true; }
        public virtual bool GetEdge(int index) { return false; }
        public virtual bool GetFEdge(int index) { return false; }
        public virtual void SetMotorSpeed(int speed) { }
        public virtual void SetOutBit(byte bitMask, bool data, int frameCount) { }
        public virtual byte[] GetSendData() { return new byte[64]; }
        public virtual ushort GetSendData(IoBoardPortNo a_IoBoardPortNoObj) { return 0x00; }
        public virtual string GetVersionData() { return "0000"; }
        public virtual bool GetData(IoBoardDInCode ioBoardDInCode) { return false; }
        public virtual bool GetData(IoBoardDInStatusCode ioBoardDInStatusCode, bool n) { return false; }
        public virtual bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState) { a_boolRawState = false; return false; }

        public virtual bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState) { a_boolLogicalState = false; return false; }
        public virtual bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj) { return false; }
    }
}
