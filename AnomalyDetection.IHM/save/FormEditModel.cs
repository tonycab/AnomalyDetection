using AnomalyDetection.Technique.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnomalyDetection.IHM
{
    public partial class FormEditModel : Form
    {
        public FormEditModel()
        {
            InitializeComponent();

            _model =  new Model();

        }
        private Model _model;
        public FormEditModel(Model model)
        {
            InitializeComponent();

            _model = model;

            textBox1.Text = model.Name;
            textBox2.Text = model.Description;
            textBox3.Text = model.ModelHDLDirectory;
            numericUpDown1.Value = (decimal)model.ClassificationThreshold;
            numericUpDown2.Value = (decimal)model.SegmentationThreshold;
        }

        private void FormEditModel_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _model.Name= textBox1.Text;
            _model.Description= textBox2.Text;
            _model.ModelHDLDirectory= textBox3.Text;
            _model.ClassificationThreshold = (double)numericUpDown1.Value;
            _model.SegmentationThreshold = (double)numericUpDown2.Value;

            this.DialogResult = DialogResult.OK;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
           

            openFileDialog.Filter = "hdl files (*.hdl)|*.hdl";

            if (DialogResult.OK==openFileDialog.ShowDialog())
            {
               textBox3.Text = openFileDialog.FileName;
            }


        }
    }
}
