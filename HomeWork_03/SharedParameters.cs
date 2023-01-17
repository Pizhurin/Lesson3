using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HomeWork_03
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class SharedParameters
    {
        UIApplication _uiApp = null;
        Document _document = null;
        bool _accessful = false; 

        public SharedParameters(UIApplication uIApplication, Document doc)
        {
            _uiApp = uIApplication;
            _document = doc;
        }

        public void CreateSharedParameter(string nameParam, CategorySet cs, BuiltInParameterGroup builtInParameterGroup, bool isInstance)
        {

            DefinitionFile definitionFile = _uiApp.Application.OpenSharedParameterFile();
            if(definitionFile == null)
            {
                TaskDialog.Show("Error", "Не найден ФОП");
                return;
            }

            Definition definition = definitionFile.Groups.SelectMany(it => it.Definitions).FirstOrDefault(it => it.Name.Equals(nameParam));

            if(definition == null)
            {
                TaskDialog.Show("Error", "Параметр не найден в ФОП");
                return;
            }

            using (Transaction tr = new Transaction(_document, "Create shared parameter"))
            {
                tr.Start();

                Binding binding = _uiApp.Application.Create.NewTypeBinding(cs);
                if (isInstance)
                {
                    binding = _uiApp.Application.Create.NewInstanceBinding(cs);
                }

                BindingMap bindingMap = _document.ParameterBindings;
                bindingMap.Insert(definition, binding, builtInParameterGroup);

                tr.Commit();
            }

            _accessful = true;

        }

        public bool GetAccess()
        {
            return _accessful;
        }

    }
}
