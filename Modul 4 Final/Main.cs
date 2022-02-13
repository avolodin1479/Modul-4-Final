using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modul_4_Final
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Room> rooms = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .OfType<Room>()
                .ToList();

            List<FamilySymbol> familySymbols = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .ToList();

            Transaction tr = new Transaction(doc, "Нумерация помещений");
            tr.Start();
            int roomNumber = 1;
            foreach (Room room in rooms)
            {
                Parameter roomNumberReference = room.get_Parameter(BuiltInParameter.ROOM_NUMBER);
                roomNumberReference.Set(Convert.ToString(roomNumber));
                roomNumber++;   
                
                FamilySymbol roomNumberTag = familySymbols[0];
                XYZ roomCenter = GetElementCenter(room);

                IndependentTag.Create(doc, roomNumberTag.Id, doc.ActiveView.Id, new Reference(room), false, TagOrientation.Horizontal, roomCenter);
            }
            tr.Commit();
            return Result.Succeeded;
        }
        public XYZ GetElementCenter(Element element)
        {
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            return (bounding.Max + bounding.Min) / 2;
        }

    }
}
