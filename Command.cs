#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Forms = System.Windows.Forms;
using Excel = OfficeOpenXml;
using OfficeOpenXml;

#endregion

namespace RAA_WPF
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Challenge01Form form = new Challenge01Form();

            form.ShowDialog();
            string excelFilePath = form.GetFilePath();
            if (string.IsNullOrEmpty(excelFilePath))
            {
                return Result.Cancelled;
            }


            //Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //Get filteredelementcollector that get plan view types
            FilteredElementCollector collector = VFTCollector(doc);

            ViewFamilyType floorPlanVFT = FirstFloorPlanType(collector);
            ViewFamilyType ceilingPlanVFT = FirstCeilingPlanType(collector);

            //Open Excel file

            ExcelPackage excel = new ExcelPackage(excelFilePath);
            ExcelWorkbook workbook = excel.Workbook;
            ExcelWorksheet worksheet = workbook.Worksheets[0];

            //Get the number of rows and columns
            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            //Read excel data into lists
            List<string> levelNames = GetColumnDataString(worksheet, 1, true);
            List<double> levelElevationsFeet = GetColumnDataDouble(worksheet, 2, true);
            List<double> levelElevationsMeters = GetColumnDataDouble(worksheet, 3, true);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Levels");
                //Create levels
                if (form.GetUnitType() == "Metric")
                {
                    for (int i = 0; i < levelNames.Count; i++)
                    {
                        Level level = Level.Create(doc, ArchSmarterUtils.Convert.ConvertMtoFT(levelElevationsMeters[i]));
                        level.Name = levelNames[i];

                        if (form.IsFloorPlanChecked())
                        {
                            CreateFloorPlan(doc, floorPlanVFT, level);
                        }

                        if (form.IsCeilingPlanChecked())
                        {
                            CreateCeilingPlan(doc, ceilingPlanVFT, level);
                        }
                    }
                }
                else if (form.GetUnitType() == "Imperial")
                {
                    for (int i = 0; i < levelNames.Count; i++)
                    {
                        Level level = Level.Create(doc, levelElevationsFeet[i]);
                        level.Name = levelNames[i];

                        if (form.IsFloorPlanChecked())
                        {
                            CreateFloorPlan(doc, floorPlanVFT, level);
                        }

                        if (form.IsCeilingPlanChecked())
                        {
                            CreateCeilingPlan(doc, ceilingPlanVFT, level);
                        }
                    }


                }
                t.Commit();
            }

            if (form.IsFloorPlanChecked())
            {

            }

            if (form.IsCeilingPlanChecked())
            {

            }

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }

        public static FilteredElementCollector VFTCollector(Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
        }

        public static ViewFamilyType FirstFloorPlanType(FilteredElementCollector collector)
        {
            ViewFamilyType floorPlanVFT = null;
            foreach (ViewFamilyType curVFT in collector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    floorPlanVFT = curVFT;
                    break;
                }
            }

            return floorPlanVFT;
        }

        public static ViewFamilyType FirstCeilingPlanType(FilteredElementCollector collector)
        {
            ViewFamilyType ceilingPlanVFT = null;
            foreach (ViewFamilyType curVFT in collector)
            {
                if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    ceilingPlanVFT = curVFT;
                    break;
                }
            }

            return ceilingPlanVFT;
        }

        public static List<string> GetColumnDataString(ExcelWorksheet worksheet, int column, bool removeFirstRow)
        {
            List<string> cellsAsStrings = new List<string>();

            for (int i = column; i <= worksheet.Dimension.Rows; i++)
            {
                string cellContent = worksheet.Cells[i, column].Value.ToString();
                cellsAsStrings.Add(cellContent);
            }

            if (removeFirstRow)
            {
                cellsAsStrings.RemoveAt(0);
            }

            return cellsAsStrings;
        }

        public static List<double> GetColumnDataDouble(ExcelWorksheet worksheet, int column, bool removeFirstRow)
        {
            List<double> cellsAsDoubles = new List<double>();

            for (int i = 1; i <= worksheet.Dimension.Rows; i++)
            {
                string cellContent = worksheet.Cells[i, column].Value.ToString();
                double.TryParse(cellContent, out double cellContentdouble);
                cellsAsDoubles.Add(cellContentdouble);
            }

            if (removeFirstRow)
            {
                cellsAsDoubles.RemoveAt(0);
            }

            return cellsAsDoubles;
        }

        public static void CreateFloorPlan(Document doc, ViewFamilyType floorPlanVFT, Level level)
        {
            ViewPlan floorPlan = ViewPlan.Create(doc, floorPlanVFT.Id, level.Id);
            floorPlan.Name = level.Name + " Floor Plan";
        }

        public static void CreateCeilingPlan(Document doc, ViewFamilyType ceilingPlanVFT, Level level)
        {
            ViewPlan ceilingPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, level.Id);
            ceilingPlan.Name = level.Name + " Ceiling Plan";
        }
    }
}
