using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GOST_1477_93
{
    public partial class ScrewOptionsForm : Form
    {
        private readonly Program _program;

        public ScrewOptionsForm(Program program)
        {
            _program = program;
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            double length;
            bool result = Double.TryParse(LengthTextBox.Text, out length);
            if (NameTextBox.Text == "")
            {
                MessageBox.Show("Не введено имя винта!");
                return;
            }
            if (!result)
            {
                MessageBox.Show("Некорректно введена длина винта");
            }
            if (ScrewNX.Screw.Table_GOST_1477_93[TypeComboBox.SelectedItem as string].SlotWidth +
                (ScrewNX.Screw.Table_GOST_1477_93[TypeComboBox.SelectedItem as string].OuterDiameter - 
                ScrewNX.Screw.Table_GOST_1477_93[TypeComboBox.SelectedItem as string].InnerDiameter) * 2 > length)
            {
                MessageBox.Show("Длина винта слишком мала!");
                return;
            }
            _program.CreateScrew(ScrewNX.Screw.Table_GOST_1477_93[TypeComboBox.SelectedItem as string], length, NameTextBox.Text);
            this.Close();
        }
    }
}
