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
        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(LightManager.Getinstance().LstLightPowerBaseName.ToArray()); 
            LightManager.Getinstance().lpbs.TerminateCompletedAction += Lpbs_TerminateCompletedAction;

            foreach (var lightPowerBase in LightManager.Getinstance()._lstLightPowerBase)
            {
                lightPowerBase.ResultNotificationAction += LightPowerBase_ResultNotificationAction;                
            }  
        }

        private void Lpbs_TerminateCompletedAction(object sender, EventArgs arg)
        {            
            Action act = () =>
            {
                LightManager.Getinstance().lpbs.StopedThread();
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
            if (textBox1.Text != "")
            {
                if (comboBox1.SelectedItem == null )
                {
                    LightManager.Getinstance().AllLightChangeIsCommand(textBox1.Text);
                }
                else
                {
                    LightManager.Getinstance().SettingIsCommand(comboBox1.Text, textBox1.Text);
                }
            }                
            else
            {
                MessageBox.Show("データがまだ入力されていません");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox2.Text != "")
            {
                if (0 > int.Parse(textBox2.Text) || int.Parse(textBox2.Text) > 999)
                {
                    return;
                }
                if (comboBox1.SelectedItem == null)
                {
                    LightManager.Getinstance().AllLightChangeIsValue(Int32.Parse(textBox2.Text));
                }
                else /*(comboBox1.SelectedItem != null)*/
                {
                    LightManager.Getinstance().ChangeLightValue(comboBox1.Text, Int32.Parse(textBox2.Text));
                }
            }           
          
            else
            {
                MessageBox.Show("データがまだ入力されていません");
            }
            
        }

        private void btnTerminate_Click(object sender, EventArgs e)
        {
            LightManager.Getinstance().AllLightTerminateNew();

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
    }
}
