using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LightControl.Control;

namespace LightControl
{
    public partial class Form1 : Form
    {
        private LightManager _lightmanager;
        public Form1()
        {
            bool _bRet = true;
            _lightmanager = LightManager.Getinstance();
            _bRet &= _lightmanager.LoadLightInfo(@"C:\Users\k-nguyen\Documents\Visual Studio 2015\Projects\yatteimasu\CodeDesign\LightControl\LightControl\bin\Debug\Setting\");
            if (!_bRet)
            {
                MessageBox.Show("ファイルを読み取れません", "Reading File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //DialogResult result = MessageBox.Show("đọc không thành công", "Reading File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //if(result == DialogResult.OK){}else if(result == DialogResult.None){}else{}
            }

            InitializeComponent();
            comboBox1.Items.AddRange(_lightmanager.LstLightPowerBaseName.ToArray());
            _lightmanager.lpbs.TerminateCompletedAction += Lpbs_TerminateCompletedAction;

            foreach (var lightPowerBase in _lightmanager._lstLightPowerBase)
            {
                lightPowerBase.ResultNotificationAction += LightPowerBase_ResultNotificationAction;                
            }  
        }

        private void Lpbs_TerminateCompletedAction(object sender, EventArgs arg)
        {
            
            Action act = () =>
            {
                _lightmanager.lpbs.StopedThread();
            };
            if (InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }

        /// <summary> Event Result Notification </summary> 
        private void LightPowerBase_ResultNotificationAction(object sender, LightPowerBase.ResultNotificationEvented arg)
        {
            Action act = () =>
            {
            listView1.Items.Insert(0, arg.Message + " is : "+ arg.Confirm);
            };
            if (InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null && textBox1.Text != "")
            {
                _lightmanager.AllLightChangeIsCommand(textBox1.Text);
            }
            else if (comboBox1.Text != null && textBox1.Text != "")
            {
                _lightmanager.SettingIsCommand(comboBox1.Text, textBox1.Text);
            }
            else
            {
                MessageBox.Show("データがまだ入力されていません");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem == null && textBox2.Text != "")
            {
                _lightmanager.AllLightChangeIsValue(Int32.Parse(textBox2.Text));
            }
            else if(comboBox1.SelectedItem != null && textBox2.Text != "")
            {
                _lightmanager.ChangeLightValue(comboBox1.Text, Int32.Parse(textBox2.Text));
            }
            else
            {
                MessageBox.Show("データがまだ入力されていません");
            }
            
        }

        private void btnTerminate_Click(object sender, EventArgs e)
        {
            _lightmanager.AllLightTerminateNew();

        }
    }
}
