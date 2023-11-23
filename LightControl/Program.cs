using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightControl
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // có thể load data tại đây. nếu lỗi sẽ chỉ thông báo và không load form lên
            // hoặc vẫn load lên nhưng báo cảnh báo ở đèn tín hiệu.
            Application.Run(new Form1());
        }
    }
}
