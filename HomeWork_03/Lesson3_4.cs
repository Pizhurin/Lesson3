using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;


namespace HomeWork_03
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class Lesson3_4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uIDocument = uiApp.ActiveUIDocument;
            Document document = uIDocument.Document;

            CategorySet categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(document, BuiltInCategory.OST_PipeCurves));

            string ParameterName = "Наименование";
            SharedParameters sharedParameters = new SharedParameters(uiApp, document);
            sharedParameters.CreateSharedParameter(ParameterName, categorySet, BuiltInParameterGroup.PG_DATA, true);

            if (sharedParameters.GetAccess())
            {
                TaskDialog.Show("Info", $"Параметр \"{ParameterName}\" успешно создан");
            }

            FilteredElementCollector fecPipe = new FilteredElementCollector(document);
            IList<Pipe> pipeList = fecPipe.OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .Cast<Pipe>()
                .ToList();
            double outsideDiameter = 0.00, innerDiameter = 0.00;
            string dataPipe = string.Empty;

            using (Transaction tr = new Transaction (document, "Set new parameters"))
            {
                tr.Start();
                foreach (Pipe pipe in pipeList)
                {
                    outsideDiameter = UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble(), UnitTypeId.Millimeters);
                    innerDiameter = UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble(), UnitTypeId.Millimeters);

                    dataPipe = $"Труба {Math.Round(outsideDiameter, 1)}/{Math.Round(innerDiameter, 1)} мм.";

                    pipe.LookupParameter(ParameterName).Set(dataPipe);
                }
                tr.Commit();
            }


            return Result.Succeeded;
        }
    }
}
