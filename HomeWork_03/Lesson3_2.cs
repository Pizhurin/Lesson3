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
    internal class Lesson3_2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDocument = uiApp.ActiveUIDocument;
            Document document = uiDocument.Document;

            FilteredElementCollector fec = new FilteredElementCollector(document);
            IList<Pipe> pipes = fec.OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .Cast<Pipe>()
                .ToList();

            double totalLegth = 0.00;
            IList<ElementId> elementId = new List<ElementId>();
            foreach (Pipe pipe in pipes)
            {
                totalLegth += pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                elementId.Add(pipe.Id);
            }

            TaskDialog.Show("Info", $"{Math.Round(UnitUtils.ConvertFromInternalUnits(totalLegth, UnitTypeId.Meters), 2).ToString()} м.");
            
            //uiDocument.Selection.SetElementIds(elementId); // Выделить элементы
            View view = uiDocument.ActiveView;  // Получить текущий вид
            using (Transaction tr = new Transaction(document, "Isolate selected elements"))
            {
                tr.Start();
                view.IsolateElementsTemporary(elementId);   // Изолировать на виде выбранные элементы
                tr.Commit();
            }


            return Result.Succeeded;
        }
    }
}
