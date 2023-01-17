using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace HomeWork_03
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]   
    internal class Lesson3_3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uIDocument = uiApp.ActiveUIDocument;
            Document document = uIDocument.Document;

            CategorySet categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(document, BuiltInCategory.OST_PipeCurves)); 

           
            // Добавление параметра проекта из общих параметров
            SharedParameters sharedParameters = new SharedParameters(uiApp, document);
            string parameterName = "Длина с запасом";
            sharedParameters.CreateSharedParameter(parameterName, categorySet, BuiltInParameterGroup.PG_DATA, true);

            if (sharedParameters.GetAccess())
            {
                TaskDialog.Show("Info", $"Параметр \"{parameterName}\" успешно создан");
            }



            try
            {
                using (Transaction tr = new Transaction(document, "fill new parameter"))
                {
                    tr.Start();

                    Reference refer = uIDocument.Selection.PickObject(ObjectType.Element, "Выберите трубу для заполения параметра");
                    Pipe selectPipe = document.GetElement(refer) as Pipe;

                    double value = selectPipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                    selectPipe.LookupParameter(parameterName).Set(value * 1.1); 
               
                    tr.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
            }


            return Result.Succeeded;
        }
    }
}
