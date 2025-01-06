using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HalconDotNet;
using static System.Net.WebRequestMethods;

/// <summary>
/// Namespace for device acquiring image
/// </summary>
namespace AnomalyDetection.Technique.Acquisitions.Cameras.D2D
{
    /// <summary>
    /// Class acquistion files directory
    /// </summary>
    public class AcqImage2DDirectory : IAcqImage2D
    {
        #region "Properties public"
        public List<string> ListeFile { get; private set; }

        public string Filter { get; set; } = ".png|.jpeg|.jpg";

        public event EventHandler EventUpdateFolder;

        public event EventHandler Progress;

        public event EventHandler End;

        public string FolderDirectory
        {
            get => _folderDirectory;
            set
            {
                _folderDirectory = value;
                UpdateListFile(_folderDirectory);

                watcher.Path = _folderDirectory;
                watcher.EnableRaisingEvents = true;
            }
        }

        public string CurrentFile { get; private set; }

        private int indexListFile = 0;

        private AcqIMage2DFile acqIMage2DFile;

        private string _folderDirectory = "";

        private FileSystemWatcher watcher;


        #endregion

        #region "Contructor"

        public AcqImage2DDirectory()
        {

            //Instance de la class AcquImage3DFile
            acqIMage2DFile = new AcqIMage2DFile();


            //Surveille le répertoire
            watcher = new FileSystemWatcher();

            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;
            watcher.Changed += OnDirectoryChanged;
            watcher.Created += OnDirectoryChanged;
            watcher.Deleted += OnDirectoryChanged;
            watcher.Renamed += OnDirectoryChanged;

            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true;


        }

        public AcqImage2DDirectory(string folderDirectory) : this()
        {
            FolderDirectory = folderDirectory;
        }

        #endregion


        #region "Methode public"

        //Return une image du répertoire
        public Image2D GetImage2D()
        {
            Progress?.Invoke(this, new EventArgs());

            if (ListeFile==null || ListeFile.Count() == 0) return null;

            //Recupère le chemin du fichier
            CurrentFile = acqIMage2DFile.FileDirectory = ListeFile[indexListFile++];
            if (indexListFile >= ListeFile.Count()) indexListFile = 0;

            End?.Invoke(this, new EventArgs());

            return acqIMage2DFile.GetImage2D();

        }
        #endregion

        public override string ToString() => CurrentFile;

        #region "Methode private"

        //Met à jour la liste des fichiers dans le répertoire
        private void UpdateListFile(string folder)
        {
            indexListFile = 0;

            var extensions = Filter.Split('|').ToHashSet(StringComparer.OrdinalIgnoreCase);
            ListeFile = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(s => extensions.Contains(Path.GetExtension(s)))
                .ToList();

            //ListeFile = Directory.EnumerateFiles(folder, "*.png", SearchOption.AllDirectories)
            //.Select(file => file)
            //.ToList();

            if (EventUpdateFolder != null)
                EventUpdateFolder(this, new EventArgs());
        }

        //Le répetoire à changer
        private void OnDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            UpdateListFile(((FileSystemWatcher)sender).Path);
        }

        #endregion

        /// <summary>
        /// Acquiring image folder asynchrone methode
        /// </summary>
        /// <param name="CallBackImage"></param>
        public async void GetImage3DAsync(Action<Image2D> CallBackImage)
        {
            await Task.Run(() =>
            {

                CallBackImage(GetImage2D());


            });
        }

        public void init()
        {
            indexListFile= 0;
        }
    }
}
