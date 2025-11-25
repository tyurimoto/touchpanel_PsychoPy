using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Compartment
{
    public static class SettingSearcher
    {
        public static string SearchSetting()
        {
            if (!File.Exists(Path.Combine(Application.UserAppDataPath, "preference.xml")))
            {
                string pattern = ".xml";
                //バージョンNo順番が最新の番号とは限らないので末尾数値でソートしたほうが・・・

                var folderlist = Directory.GetDirectories(Directory.GetParent(Application.UserAppDataPath).ToString());

                List<string> filelist = new List<string>();
                // 全部列挙に変更
                foreach (string path in folderlist)
                {
                    filelist.AddRange(Directory.GetFiles(path.ToString())
                    .Where(list => pattern.Any(patterns => list.ToLower().EndsWith(pattern))).ToList());
                }
                if (filelist.Count == 0)
                {
                    return "";
                }
                return filelist?.Last();
            }

            return "";
        }
    }
}
