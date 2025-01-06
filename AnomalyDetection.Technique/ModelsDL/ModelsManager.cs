using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnomalyDetection.Technique.ModelsDL
{
    public class ModelsManager : BindingList<Model>
    {
        private List<Model> modelsOriginal;

        public ModelsManager()
        {
            
        }

        public XElement ToXElement()
        {
            XElement xElement = new XElement(nameof(ModelsManager));

            for (int i = 0; i < this.Count; i++)
            {
                XElement calibration = this[i].ToXElement();

                calibration.SetAttributeValue("Numero", i);
                calibration.SetAttributeValue("Name", this[i].Name);
                calibration.SetAttributeValue("Description", this[i].Description);

                xElement.Add(calibration);
            }

            return xElement;
        }

        public static ModelsManager FromXElement(XElement xElement)
        {
            ModelsManager calibrationManager = new ModelsManager();

            foreach (var item in xElement.Elements(nameof(Model)))
            {
                calibrationManager.Add(Model.FromXElement(item));
            }

            return calibrationManager;
        }

 
        public static ModelsManager LoadToXml(string path)
        {
            if (Path.GetExtension(path) != ".xml") return null;

            XDocument xDocument = XDocument.Load(path);

            return FromXElement(xDocument.Root);

        }

        public void SaveFromXml(string path)
        {
            XElement xmlRoot = ToXElement();
            XDocument xDocument = new XDocument(xmlRoot);

            xDocument.Save(path);

        }

        private ListSortDirection sortDirection= ListSortDirection.Ascending;
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            List<Model> models=null;

            if (sortDirection == ListSortDirection.Ascending) {
                models = this.OrderBy(x =>x.GetType().GetProperty(prop.Name).GetValue(x)).ToList();
                sortDirection = ListSortDirection.Descending;

            }else if (sortDirection == ListSortDirection.Descending) { 
                models = this.OrderByDescending(x =>   x.GetType().GetProperty(prop.Name).GetValue(x)).ToList();
                sortDirection = ListSortDirection.Ascending;
            }

            ResetItems(models);


        }
        private void ResetItems(List<Model> items)
        {

            base.ClearItems();

            for (int i = 0; i < items.Count; i++)
            {
                base.InsertItem(i, items[i]);
            }

        }

        protected override void RemoveSortCore()
        {

            ResetItems(modelsOriginal);
        }
        protected override bool SupportsSortingCore
        {
            get
            {
                // indeed we do
                return true;
            }
        }


        protected override void OnListChanged(ListChangedEventArgs e)
        {
            modelsOriginal = base.Items.ToList();

            base.OnListChanged(e);
        }
    }
}
