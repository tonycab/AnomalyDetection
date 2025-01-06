using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnomalyDetection.IHM.Properties;
using AnomalyDetection.Technique.Acquisitions;
using AnomalyDetection.Technique.Acquisitions.Cameras.D2D;
using AnomalyDetection.Technique.Acquisitions.Discovery;
using AnomalyDetection.Technique.DeepLearning;
using AnomalyDetection.Technique.Logs;
using AnomalyDetection.Technique.Models;
using HalconDotNet;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace AnomalyDetection.IHM
{
    public partial class Form1 : Form
    {

        IAcqImage2D cam2D;

        AcqImage2DBasler basler;

        AcqImage2DDirectory dirCam;

        AcqIMage2DFile fileCam;

        Model modelCurrent;

        ModelsManager modelsManager = new ModelsManager();

        ResultDLManager resultDLmanager = new ResultDLManager();

        BindingSource bindingSource = new BindingSource();

        BindingSource bindingSource1 = new BindingSource();

        BindingSource bindingSource2 = new BindingSource();

        Task task;

        int stepContinus = 0;

        CancellationTokenSource cancellationTokenSource;

        //Mode de trigg
        enum enumMode
        {
            Step = 0,
            Continus = 1,
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //Affichage de la liste des modes de trigg dans le combobox
            comboBox1.DataSource = Enum.GetValues(typeof(enumMode));
            comboBox1.SelectedIndex = 0;

            //Chargement de la collection de models
            if (File.Exists("SaveModel.xml"))
                modelsManager = ModelsManager.LoadToXml("SaveModel.xml");

            //Databinding de la collection avec le datagridview Model
            bindingSource.DataSource = modelsManager;
            dataGridView1.DataSource = bindingSource;

            //Databinding de la collection avec le datagridview log
            bindingSource1.DataSource = LogsManager.GetLogs();
            dataGridView2.DataSource = bindingSource1;


            //Databinding de la collection avec le datagridview resultat
            bindingSource2.DataSource = resultDLmanager;
            dataGridView3.DataSource = bindingSource2;
            dataGridView3.CellFormatting += ResultCellFormatting;



            //Inscription évennement de détection de caméea
            DeviceDiscovery.EventNewDevice += NewDevices;
            DeviceDiscovery.EventRemoveDevice += RemoveDevices;

            //Instanciation d'une caméra Basler
            basler = new AcqImage2DBasler(textBox3.Text, true);

            //Inscription évennement de la caméra Basler
            basler.EventState += balserEventState;

        }



        //Pour afficher un logo dans la grille de résultat
        private void ResultCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].Name == "ResultState")
            {
                //Taille de l'image 5 pixel en dessous la hauteur ou largeur de la cellule
                int sizeImage = Math.Min(dataGridView3.RowTemplate.Height, dataGridView3.Columns[e.ColumnIndex].Width) - 10;

                if (e.Value != null)
                {

                    if (e.Value.ToString() == "PASS")
                    {
                        Bitmap image = new Bitmap(Resources.Image_pass, sizeImage, sizeImage);
                        e.Value = image;
                    }
                    else
                    {
                        Bitmap image = new Bitmap(Resources.Image_fail, sizeImage, sizeImage);
                        e.Value = image;
                    }
                    e.FormattingApplied = true;
                }
            }
        }


        #region Log camera
        //Evennement du status de la caméra Basler
        private void balserEventState(object sender, DeviceEventArg e) => LogsManager.Add(EnumCategory.Process, e.ToString());
        //Caméra GigeVision2 detecté
        private void NewDevices(object sender, DiscoveryEventArgs e) => LogsManager.Add(EnumCategory.Process, $"Camera detected: {e.Name}");
        //Cémara GigeVision 2 perdu
        private void RemoveDevices(object sender, DiscoveryEventArgs e) => LogsManager.Add(EnumCategory.Process, $"Camera lost: {e.Name}");

        #endregion


        #region Selection de la source d'image

        //Source d'imafe depuis un répertoire
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                radioButton3.Checked = false;

                //Deconnection de la caméra basler
                if (basler != null && basler.State == AcqDevice.DeviceState.Connected)
                    basler.Disconnect();

                //A supprimer
                if (textBox1.Text == "")
                    textBox1.Text = "C:\\Users\\alecabellec\\Desktop\\Images Test";

                // Instancie la classe d'acquisition d'image par répertoire
                if (dirCam == null)
                {
                    dirCam = new AcqImage2DDirectory(textBox1.Text);
                    stepContinus = dirCam.ListeFile.Count();
                }

                cam2D = dirCam;

            }

        }
        //Source d'image depuis un fichier
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;

                //Deconnection de la caméra basler
                if (basler != null && basler.State == AcqDevice.DeviceState.Connected)
                    basler.Disconnect();

                //Instancie la classe d'acquisition d'image par fichier
                if (fileCam == null)
                    fileCam = new AcqIMage2DFile(textBox2.Text);

                stepContinus = -1;

                cam2D = fileCam;
            }

        }
        //Source d'image depuis la caméra basler
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;

                basler.Name = textBox3.Text;

                basler.Connect();

                stepContinus = -1;

                cam2D = basler;
            }

        }
        //Selection répertoire
        private void button2_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult = folderBrowserDialog.ShowDialog();

            if (DialogResult == DialogResult.OK)
            {

                textBox1.Text = folderBrowserDialog.SelectedPath;
                if (dirCam == null)
                {
                    dirCam = new AcqImage2DDirectory(textBox1.Text);
                    stepContinus = dirCam.ListeFile.Count();
                }
            }

        }
        //Selection fichier
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";

            DialogResult = openFileDialog.ShowDialog();

            if (DialogResult == DialogResult.OK)
            {

                textBox2.Text = openFileDialog.FileName;
                if (fileCam != null)
                    fileCam.FileDirectory = textBox2.Text;
            }
        }

        #endregion


        #region Recherche de caméra
        //Recherche de la premiere caméra de détecté
        private void button4_Click(object sender, EventArgs e)
        {
            var camstr = DeviceDiscovery.GetListDevices(DeviceType.GigeEVision);

            textBox3.Text = camstr.Length > 0 ? camstr[0] : "";

            basler.Name = textBox3.Text;
        }
        #endregion


        #region gestion models
        //Ajout d'un model
        private void button5_Click(object sender, EventArgs e)
        {

            Model model = new Model();

            FormEditModel formEditModel = new FormEditModel(model);

            DialogResult = formEditModel.ShowDialog();

            if (DialogResult == DialogResult.OK)
                modelsManager.Add(model);

            LogsManager.Add(EnumCategory.Info, $"Modele {model.Name} ajouté");
        }
        //Suppression d'un model
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                modelsManager.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
            else
            {
                LogsManager.Add(EnumCategory.Info, "Aucun modele sélectionné");
            }

        }
        //Modification d'un model
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                var model = modelsManager[dataGridView1.SelectedRows[0].Index];

                FormEditModel formEditModel = new FormEditModel(model);

                DialogResult = formEditModel.ShowDialog();

                modelsManager.ResetBindings();
            }
            else
            {
                LogsManager.Add(EnumCategory.Info, "Aucun modele sélectionné");
            }

        }
        //Sauvegarde la collection de modesl
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                modelsManager.SaveFromXml("SaveModel.xml");

                LogsManager.Add(EnumCategory.Info, "Sauvegarde terminée");
            }
            catch
            {
                LogsManager.Add(EnumCategory.Error, "Erreur de sauvegarde");
            }
        }
        //Chargement du model
        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                try
                {
                    modelCurrent = modelsManager[dataGridView1.SelectedRows[0].Index].GetModelHDL();

                    textBox4.Text = modelCurrent;
                }
                catch
                {
                    LogsManager.Add(EnumCategory.Error, "Erreur de chargement du fichier HDL");
                }
            }
            else
            {
                LogsManager.Add(EnumCategory.Info, "Aucun modele sélectionné");
            }
        }

        //#endregion


        #region trigger
        //Acquisition d'image
        private void button1_Click(object sender, EventArgs e)
        {
            //(task != null && task.Status == TaskStatus.WaitingForActivation) ||
            //Lance le cycle
            if (task == null&& cam2D !=null)
            {
                cancellationTokenSource = new CancellationTokenSource();

                Execute(comboBox1.SelectedItem.ToString() == enumMode.Continus.ToString() ? stepContinus : 1, cancellationTokenSource);
            }
            //Annule le cycle
            else if (task != null && cam2D != null )
            {
                cancellationTokenSource.Cancel();
            }
            else
            {
                LogsManager.Add(EnumCategory.Info, "Aucune source d'image sélectionné");
            }

        }
        //Cycle d'acquisition d'image


        private async void Execute(int step, CancellationTokenSource cancellationTokenSource)
        {

            Random random = new Random();

            //En mode continus on initialise la caméra et les résultats (Réinitialise le compteur d'image lorsque la source d'image un répertoire)
            if (step != 1)
            {
                resultDLmanager.Clear();
                cam2D?.init();
            }

            int count = 0;

            task = Task.Run(async () =>
            {
                do
                {
                    count++;

                    //Annulation pour sortir de la boucle
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    //Acquisition de l'image
                    Image2D image = cam2D?.GetImage2D();


                    //Si image recu
                    if (image != null)
                    {

                        //Affichage du nom de l'image dans la barre d état
                        this.Invoke(
                            new Action(() =>
                            {
                                toolStripStatusLabel1.Text = image;
                            }));


                        //convertion Color ou Gray
                        if (checkBox1.Checked)
                        {
                            HOperatorSet.Rgb1ToGray(image, out HObject grayimage);
                            image.Image = new HImage(grayimage);
                        }


                        ///Insérer ici la méthode inference


                        //Affichage de l'image sur le HsmartWindowsControle
                        this.Invoke(
                            new Action(() =>
                            {
                                //Simulation du resultat
                                var score = Math.Round(random.NextDouble(), 2);

                                //Traitement du résultat (Random pour tester)
                                resultDLmanager.Add(new ResultDL(score > 0.5 ? EnumState.PASS : EnumState.FAIL, score, image.ToBitmap(45)));

                                //Affichage
                                hSmartWindowControl1.HalconWindow.AttachBackgroundToWindow(image);

                                //Set the display part to the full image size
                                ((HImage)image).GetImageSize(out HTuple width, out HTuple height);
                                hSmartWindowControl1.HalconWindow.SetPart(0, 0, height.I - 1, width.I - 1);

                            }));

                        await Task.Delay(400);
                    }
                } while (count < step || step == -1);

            }, cancellationTokenSource.Token);

            await task;

            task = null;

            //task.Dispose();
        }

        #endregion


        //Menu quitter l'application
        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Effacement des résultats
        private void button10_Click(object sender, EventArgs e)
        {
            resultDLmanager.Clear();
        }

        //Effacement des logs
        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu d = dataGridView2.ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("Effacer", new EventHandler((o, j) => { LogsManager.Clear(); })) });

                d.Show((Control)sender, new Point(e.X, e.Y),LeftRightAlignment.Right);
            }
        }
    }
}
