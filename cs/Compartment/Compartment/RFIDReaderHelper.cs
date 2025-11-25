using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compartment
{
    public class RFIDReaderHelper
    {
        // ID Code重複排除用にID Codeを保存
        public SyncObject<string> CurrentIDCode = new SyncObject<string>("");

        public Action<string> callbackReceivedDataSub = (str) => { };

        public Action<byte[]> GetDosetIDAction()
        {
            Action<byte[]> readIdAction = (datagram) =>
            {
                // チェックサム処理
                {
                    byte checksum = 0;
                    int checksumIndex = datagram.Count() - 1;
                    for (int i = 0; i <= checksumIndex - 1; i++)
                    {
                        checksum ^= datagram[i];
                    }

                    if (checksum != datagram[checksumIndex])
                    {
                        Debug.WriteLine("Checksum Error");
                        return;
                    }
                }

                string code = BitConverter.ToString(datagram);
                // データ部抽出処理
                {
                    // <DLE><STX><Unit from><Unit to><Command><Data> …… <DLE><ETX><Checksum> 
                    // 10-02-00-FF-08-39-20-14-50-00-56-72-78-10-03-F7 
                    //                39-20-14-50-00-56-72-78
                    // 3920145000567278

                    if (code.Length < 47)
                    {
                        return;
                    }
                    // <DLE><STX>を削除
                    code = code.Substring(6);

                    // <DLE><ETX><Checksum>を削除
                    code = code.Remove(code.Length - 9);

                    // 0x10を処理
                    code = code.Replace("10-10-", "10-");

                    // <Unit from><Unit to><Command>を削除
                    code = code.Substring(9);

                    // 『-』を削除
                    code = code.Replace("-", "");

                    if (CurrentIDCode.Value == code)
                    {
                        return;
                    }
                    CurrentIDCode.Value = code;
                }
                callbackReceivedDataSub(code);
            };
            return readIdAction;
        }


        public Action<byte[]> GetUnivrsalIDAction()
        {
            Action<byte[]> readIdAction = (datagram) =>
            {
                // チェックサム処理
                {
                    byte checksum = 0;
                    int checksumIndex = datagram.Count() - 2;
                    for (int i = 1; i <= checksumIndex - 2; i++)
                    {
                        checksum ^= datagram[i];
                    }
                    if (checksum != datagram[checksumIndex - 1])
                    {
                        Debug.WriteLine("Checksum Error");
                        return;
                    }
                }
                // データ部抽出処理
                {

                    /*
                     For example：on the tag shows:“900250000023921”
                    （Dec format 900 in the front，then card number 250000023921）
                    Module output:0231 37 31 41 39 32 35 33 41 33 34 38 33 30 30 31 30 30 30 30 30 30 30 30 30 3007 F8 03
                    Equal ASCII：171A9253A34830010000000000?
                    We can find card number is 171A9253A3, country number is 483 （LSB First）
                    Translate these number to Dec format，card numberequal: 250000023921,
                    Country number equal 900 And “31 37 31 41 39 32 35 33 41 33 34 38 33 30 30 31 30 30 30 30 30 30 30 30 30 30”
                    made all XOR caculate，we got the answer is 07 (check sum result) . 
                    F8 is 07’s bitwise invert result.
                     */
                    // <STX> <CARD NUM> <COUNTRY CODE> 
                    // 02 31 37 31 41 39 32 35 33 41 33 34 38 33 30 30 31 30 30 30 30 30 30 30 30 30 3007 F8 03

                    byte[] allData = new byte[datagram.Length - 2];

                    for (int i = 1; i < datagram.Length - 2; i++)
                    {
                        allData[i - 1] = datagram[i];
                    }
                    // ID
                    // 逆順積み
                    byte[] idByte = new byte[10];
                    for (int i = 10; i > 0; i--)
                    {
                        idByte[10 - i] = allData[i - 1];
                    }
                    // Country id 
                    // 逆順積み
                    byte[] countryByte = new byte[4];
                    for (int i = 4; i > 0; i--)
                    {
                        countryByte[4 - i] = allData[10 + i - 1];
                    }

                    // Encode ASCII
                    string idCode = Encoding.ASCII.GetString(idByte);
                    string CountryCode = Encoding.ASCII.GetString(countryByte);

                    string idString = long.Parse(idCode, System.Globalization.NumberStyles.HexNumber).ToString();
                    string countryString = long.Parse(CountryCode, System.Globalization.NumberStyles.HexNumber).ToString();

                    if (CurrentIDCode.Value == countryString + idString)
                    {
                        //連続読み取り
                        //return;
                    }
                    // Country + ID 合体
                    CurrentIDCode.Value = countryString + idString;
                }
                callbackReceivedDataSub(CurrentIDCode.Value);
            };
            return readIdAction;
        }
    }
}
