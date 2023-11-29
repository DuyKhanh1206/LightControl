using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LightControl.Control;
using LightControl.Models;

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
            bool _bRet = true;
            if (_bRet)
            {
                _bRet &= LightManager.Getinstance().LoadLightManager(AppData.Getinstance().sLightManagerSetting);
                if (_bRet)
                {
                    _bRet &= LightManager.Getinstance().LoadLightPowerNum(AppData.Getinstance().sLightManagerSetting);
                }                          
            }           
            if (_bRet)
            {
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("起動に失敗しました。", "ファイルを読み取れません", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
