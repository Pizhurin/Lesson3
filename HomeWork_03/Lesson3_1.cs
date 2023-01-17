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
    public class Lesson3_1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDocument = uiApp.ActiveUIDocument;
            Document document = uiDocument.Document;

            FilteredElementCollector fecWall = new FilteredElementCollector(document);
            IList<Wall> walls = fecWall.OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            double volume = 0.0;

            foreach(Wall w in walls)
            {
                Parameter pw = document.GetElement(w.Id).get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (pw.StorageType == StorageType.Double)
                {
                    volume += UnitUtils.ConvertFromInternalUnits(document.GetElement(w.Id).get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble(), UnitTypeId.CubicMeters);
                }
            }

            TaskDialog.Show("Info", $"Общий объем стен: {Math.Round(volume, 3)} м3");


            return Result.Succeeded;
        }
    }
}
