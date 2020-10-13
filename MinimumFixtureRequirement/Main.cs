using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;
using Newtonsoft.Json;
using ac = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Diagnostics;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.GraphicsInterface;

namespace MinimumFixtureRequirement
{
    public class Main
    {
        private const string uriInit = "minimumfixturerequirement";

        [CommandMethod("P_GETMINIMUMFIXTURE")]
        public void CreateTable()
        {
            LayerHelper.CreateAndAssignALayer("TABLE");
            ac.Document doc = ac.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //GET THE ID HERE..... ?
            string url = string.Format("{0}/#/{1}", Goodie.hostName, uriInit);
            JsonData data = OpeningBrowser(url);
            if(data != null) EditOrCreateTable(data, new Point3d(-1000, -1000, -1000));
            return;
        }

        [CommandMethod("P_EDITMINIMUMFIXTURE")]
        public void EditTable()
        {
            TypedValue[] acTypValAr = new TypedValue[2];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "ACAD_TABLE"), 0);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.LayerName, "TABLE"), 1);

            SelectionFilter filter = new SelectionFilter(acTypValAr);

            ac.Document doc = ac.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            PromptSelectionOptions promptSelectionOptions = new PromptSelectionOptions();
            Editor ed = doc.Editor;
            PromptSelectionResult selRel = ed.GetSelection(promptSelectionOptions, filter);
            if(selRel.Status == PromptStatus.OK)
            {
                SelectionSet selSet = selRel.Value;
                if(selSet.Count >= 1)
                {
                    using(Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Table gotTable = (Table)tr.GetObject(selSet[0].ObjectId, OpenMode.ForRead);
                        ResultBuffer rb = gotTable.XData;
                        if(rb != null)
                        {
                            TypedValue[] vArr = rb.AsArray();
                            if(vArr.Length == 2 && vArr[0].Value.ToString() == uriInit)
                            {
                               string url = string.Format("{0}/#/{1}?id={2}", Goodie.hostName, uriInit, vArr[1].Value);
                               JsonData data = OpeningBrowser(url);
                                if (data != null)
                                {
                                    EditOrCreateTable(data, gotTable.Position);
                                    gotTable.UpgradeOpen();
                                    gotTable.Erase();
                                }
                                tr.Commit();
                            }
                        }
                    }
                }
            }


            Table tb = new Table();

        }

        private bool EditOrCreateTable(JsonData data, Point3d point)
        {
            //TotalFacilitiesRequired total = data.minimumfixture;
            ac.Document doc = ac.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Table tb = new Table();
            tb.TableStyle = db.Tablestyle;
            tb.Layer = "TABLE";
            

            //Set Size First...
            tb.SetSize(data.minimumfixture.fixtureUnitArray.Count + 4, 9);
            tb.Cells.ContentColor = Color.FromColorIndex(ColorMethod.ByAci, 2);
            tb.Cells.TextHeight = 0.09375;
            tb.Columns[0].Width = 1.06;
            tb.Columns[1].Width = 1.34 / 2;
            tb.Columns[2].Width = 1.34 / 2;
            tb.Columns[3].Width = 0.85;
            tb.Columns[4].Width = 1.3 / 2;
            tb.Columns[5].Width = 1.3 / 2;
            tb.Columns[6].Width = 1.27;
            tb.Columns[7].Width = 1.53;
            tb.Columns[8].Width = 2.15;

            //Table Title
            CellRange titleRange = CellRange.Create(tb, 0, 0, 0, 8);
            tb.MergeCells(titleRange);
            tb.Cells[0, 0].Value = "Minimum Number of Required Fixtures";
            tb.Cells[0, 0].TextHeight = 3.0 / 16;

            //Table Items

            tb.Cells[1, 0].Value = "TYPE OF OCCUPANCY";

            CellRange wcTitleRange = CellRange.Create(tb, 1, 1, 1, 2);
            tb.MergeCells(wcTitleRange);
            tb.Cells[1, 1].Value = "WATER CLOSETS";

            tb.Cells[1, 3].Value = "URINALS";

            CellRange lavatoriesTitle = CellRange.Create(tb, 1, 4, 1, 5);
            tb.MergeCells(lavatoriesTitle);
            tb.Cells[1, 4].Value = "LAVATORIES";

            tb.Cells[1, 6].Value = "BATHTUBS OR SHOWERS";

            tb.Cells[1, 7].Value = "DRINKINGFOUNTAINS/FACILITIES";
            

            tb.Cells[1, 8].Value = "OTHER";

            //Table Bottom:

            if (!data.minimumfixture.totalRequiredFixture.ContainsKey(Table422_1Categories.urinals) &&
                data.minimumfixture.sliderValue > 0)
            {
                data.minimumfixture.totalRequiredFixture.Add(Table422_1Categories.urinals, 0);
            }

            string bottomValue = "";

            if (data.minimumfixture.totalFixtureBasedOnGender.ContainsKey(data.minimumfixture.totalFemaleCloset) &&
                data.minimumfixture.femaleWaterClosetAddIn > 0)
            {
                if(data.minimumfixture.femaleWaterClosetAddIn > 0)
                {
                    bottomValue += string.Format("Female Water Closet: {0} + {1} (note 3)\n", data.minimumfixture.totalFixtureBasedOnGender[data.minimumfixture.totalFemaleCloset], data.minimumfixture.femaleWaterClosetAddIn);
                }
                else
                {
                    bottomValue += string.Format("Female Water Closet: {0}\n", data.minimumfixture.totalFixtureBasedOnGender[data.minimumfixture.totalFemaleCloset]);
                }
                
            }

            if (data.minimumfixture.totalFixtureBasedOnGender.ContainsKey(data.minimumfixture.totalMaleCloset) &&
                data.minimumfixture.totalRequiredFixture.ContainsKey(Table422_1Categories.urinals)
                )
            {
                if (data.minimumfixture.sliderValue > 0)
                    bottomValue += string.Format("Male Water Closet: {0} - {1} (note 4)\n", data.minimumfixture.totalFixtureBasedOnGender[data.minimumfixture.totalMaleCloset], data.minimumfixture.sliderValue);
                else
                    bottomValue += string.Format("Male Water Closet: {0}\n", data.minimumfixture.totalFixtureBasedOnGender[data.minimumfixture.totalMaleCloset]);
            }

            if (data.minimumfixture.totalFixtureBasedOnGender.ContainsKey(data.minimumfixture.totalMaleUrinals) &&
                data.minimumfixture.totalRequiredFixture.ContainsKey(Table422_1Categories.urinals)
                )
            {
                if (data.minimumfixture.sliderValue > 0)
                    bottomValue += string.Format("** Male Urinal(s): {0} + {1} (note 4)\n", data.minimumfixture.totalFixtureBasedOnGender[data.minimumfixture.totalMaleUrinals], data.minimumfixture.sliderValue);
            }

            if (data.minimumfixture.femaleWaterClosetAddIn > 0)
            { 
                bottomValue += string.Format("* added {0} female lavatories -- satisfies note 3 requirement.\n", data.minimumfixture.femaleWaterClosetAddIn);
            }
            if(data.minimumfixture.sliderValue > 0)
            {
                bottomValue += string.Format("** based on Note 4, it's okay to added {0} male urinals and remove {0} male water closet\n", data.minimumfixture.sliderValue);
            }

            bottomValue += "\nCalifornia Plumbing Code 2019 - Table 422.1 - Note:\n";
            bottomValue += "(3) The total number of required water closets for females shall be not less than the total number of required water closets and urinals for males.\n";
            bottomValue += "(4) For each urinal added in excess of the minimum required, one water closet shall be permitted to be deducted. The number of water closets shall not be reduced to less than two-thirds of the minimum requirement.\n";
            bottomValue += "\n Created based on Table 422.1 California Plumbing Code 2019";
            int bottomRowIndex = data.minimumfixture.fixtureUnitArray.Count + 4 - 1;
            CellRange bottomRange = CellRange.Create(tb, bottomRowIndex , 0, bottomRowIndex, 8);
            tb.Cells[bottomRowIndex, 0].Value = bottomValue;
            tb.Cells[bottomRowIndex, 0].Alignment = CellAlignment.BottomLeft;
            tb.MergeCells(bottomRange);
            tb.Cells[0, 0].TextHeight = 3.0 / 16;

            //Fill Data...
            int rIndex = 2;

            foreach(FixtureUnit unit in data.minimumfixture.fixtureUnitArray)
            {
                createRow(unit.outputUnits, tb, ref rIndex, data, unit.occupancy.type,  false);
            }

            createRow(data.minimumfixture.totalRequiredFixture , tb, ref rIndex, data, "TOTAL", true );


            tb.Cells.DataFormat = "%lu2%pr0%";

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                TableJig tJig = new TableJig(tb);
                Editor ed = ac.Application.DocumentManager.MdiActiveDocument.Editor;

                PromptResult result;
                bool cont = false;

                if (point == new Point3d(-1000, -1000, -1000))
                {
                    result = ed.Drag(tJig);
                    if (result.Status == PromptStatus.OK)
                    {
                        point = tJig.insertionPoint;
                        cont = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    cont = true;
                }
                if(cont)
                {
                    tb.Position = point;
                    btr.AppendEntity(tb);
                    tr.AddNewlyCreatedDBObject(tb, true);
                    AddTableRecord(data.id.ToString(), db, tr, tb);
                    ResultBuffer rb = new ResultBuffer(
                            new TypedValue(1001, uriInit),
                            new TypedValue(1000, data.id.ToString())
                        );
                    tb.XData = rb;
                    rb.Dispose();
                    tr.Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private JsonData OpeningBrowser(string url)
        {
            Process process = Process.Start(url);
            //process.WaitForExit();
            DialogResult result = MessageBox.Show("Press [OK] Button To Paste", "Gouvis Plumbing", MessageBoxButtons.OK);
            getClipboard web = new getClipboard();
            string json;
            try
            {
                object obj = Clipboard.GetText();
                Clipboard.Clear();
                if(obj is string)
                {
                    json = obj as string;
                }
                else
                {
                    return null;
                }
                
            }
            catch
            {
                return null;
            }
            
            if (string.IsNullOrEmpty(json)) return null;
            JsonData data = GetInformation(json);
            return data;
        }


        private void createRow(Dictionary<string,double> itemsCount, Table tb, ref int rIndex, JsonData data, string name, bool isTotal)
        {
            tb.Cells[rIndex, 0].Value = name;
            CellRange wcTitleRange1 = CellRange.Create(tb, rIndex, 1, rIndex, 2);
            tb.MergeCells(wcTitleRange1);

            CellRange lavatoriesTitle1 = CellRange.Create(tb, rIndex, 4, rIndex, 5);
            tb.MergeCells(lavatoriesTitle1);

            foreach (KeyValuePair<string, double> kv in itemsCount)
            {
                if (kv.Key == Table422_1Categories.waterClosets)
                {
                    if (isTotal)
                    {
                        tb.Cells[rIndex, 1].Value = (kv.Value - data.minimumfixture.sliderValue + data.minimumfixture.femaleWaterClosetAddIn).ToString();
                        if(data.minimumfixture.sliderValue > 0 || data.minimumfixture.femaleWaterClosetAddIn > 0)
                        {
                            tb.Cells[rIndex, 1].Value += "*";
                        }
                    }
                    else
                    {
                        tb.Cells[rIndex, 1].Value = kv.Value;
                    }
                    tb.Cells[rIndex, 1].Alignment = CellAlignment.MiddleRight;
                }
                else if (kv.Key == Table422_1Categories.urinals)
                {
                    if (isTotal)
                    {
                        tb.Cells[rIndex, 3].Value = (kv.Value + data.minimumfixture.sliderValue).ToString();
                        if (data.minimumfixture.sliderValue > 0)
                        {
                            tb.Cells[rIndex, 3].Value += "**";
                        }
                    }
                    else
                        tb.Cells[rIndex, 3].Value = kv.Value;
                    tb.Cells[rIndex, 3].Alignment = CellAlignment.MiddleRight;
                }
                else if (kv.Key == Table422_1Categories.lavatories)
                {
                    tb.Cells[rIndex, 4].Value = kv.Value;
                    tb.Cells[rIndex, 4].Alignment = CellAlignment.MiddleRight;
                }
                else if (kv.Key == Table422_1Categories.bathtubsOrShowers)
                {
                    tb.Cells[rIndex, 6].Value = kv.Value;
                    tb.Cells[rIndex, 6].Alignment = CellAlignment.MiddleRight;
                }
                else if (kv.Key == Table422_1Categories.drinkingFountains)
                {
                    tb.Cells[rIndex, 7].Value = kv.Value;
                    tb.Cells[rIndex, 7].Alignment = CellAlignment.MiddleRight;
                }
            }
            tb.Cells[rIndex, 8].Value = getOtherFixtures(itemsCount);
            tb.Cells[rIndex, 8].Alignment = CellAlignment.MiddleRight;
            rIndex++;
        }

        /// <summary>
        /// This function is the get all other fixtures and its value > 0 to put in on string.
        /// </summary>
        /// <param name="data"></param>
        private string getOtherFixtures(Dictionary<string, double> itemsCounts)
        {
            string ans = "";
            string oneLine = "{0} {1}(s)\n";
            foreach (KeyValuePair<string, double> kv in itemsCounts)
            {
                if(Table422_1Categories.OtherSet.Contains(kv.Key) && kv.Value > 0)
                {
                    ans += string.Format(oneLine, kv.Value, kv.Key);
                }
            }

            return ans;
        }

        private JsonData GetInformation(string json) {
            try
            {
                Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                JsonData jData = new JsonData();
                if (map != null)
                {
                    if (map.ContainsKey("id") && map.ContainsKey("data"))
                    {
                        int id = -1;
                        dynamic data = JsonConvert.DeserializeObject(map["data"]);
                        if (int.TryParse(map["id"], out id))
                        {
                            jData.id = id;
                        }

                        TotalFacilitiesRequired total = GetTotalFacilitiesRequired(map["data"]);
                        jData.minimumfixture = total;

                        return jData;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }

        }

        private TotalFacilitiesRequired GetTotalFacilitiesRequired(string dataStr)
        {

            TotalFacilitiesRequired total = new TotalFacilitiesRequired();
            if (!string.IsNullOrEmpty(dataStr))
            {
                dynamic data = JsonConvert.DeserializeObject(dataStr);
                if(data != null)
                {
                    if (data.fixtureUnitArray != null) total.fixtureUnitArray = GetFixtureUnitArray(data);
                    if (data.outItem != null) total.outItem = JsonConvert.DeserializeObject<string>((string)data.outItem);
                    if (data.isEditing != null) total.isEditing = (bool)data.isEditing;

                    if (data.totalRequiredFixture != null)
                    {
                        total.totalRequiredFixture = new Dictionary<string, double>();
                        var temp = data.totalRequiredFixture;
                        foreach (var i in temp)
                        {
                            string key = i.Name;
                            double value = i.Value;
                            total.totalRequiredFixture.Add(key, value);
                        }
                    }

                    if (data.totalFixtureBasedOnGender != null) {
                        //This is the worst way to decode json.
                        total.totalFixtureBasedOnGender = new Dictionary<string, double>();
                        var temp = data.totalFixtureBasedOnGender;
                        foreach(var i in temp)
                        {
                            string key = i.Name;
                            double value = i.Value;
                            total.totalFixtureBasedOnGender.Add(key, value);
                        }
                    }
                    if (data.femaleWaterClosetAddIn != null) total.femaleWaterClosetAddIn = JsonConvert.DeserializeObject<double>((string)data.femaleWaterClosetAddIn);
                    if (data.maleUrinalsAllowedToBeAdded != null) total.maleUrinalsAllowedToBeAdded = JsonConvert.DeserializeObject<double>((string)data.maleUrinalsAllowedToBeAdded);
                    if (data.userUrinalsAdded != null) total.userUrinalsAdded = JsonConvert.DeserializeObject<double>((string)data.userUrinalsAdded);
                    if (data.sliderValue != null) total.sliderValue = JsonConvert.DeserializeObject<int>((string)data.sliderValue);
                }
                return total;
            }
            return null;
        }


        private List<FixtureUnit> GetFixtureUnitArray(dynamic data)
        {
            List<FixtureUnit> fixArr = new List<FixtureUnit>();
            if (data != null && data.fixtureUnitArray != null)
            {
                foreach(string item in data.fixtureUnitArray)
                {
                    fixArr.Add(GetFixtureUnit(item));
                }
            }
            return fixArr;
        }

        private FixtureUnit GetFixtureUnit(string json)
        {
            dynamic temp = JsonConvert.DeserializeObject(json);
            Dictionary<string, double> unitStr = new Dictionary<string, double>();
            Dictionary<PairEntry, double> unit = new Dictionary<PairEntry, double>();
            List<List<string>> otherOptions = new List<List<string>>();
            Dictionary<string, double> outputUnits = new Dictionary<string, double>();
            Dictionary<string, double> inputUnits = new Dictionary<string, double>();
            TypeOfOccupancy occupancy = new TypeOfOccupancy();
            int choiceOption = 0;
            if (temp.unit != null)
            {
                unitStr = JsonConvert.DeserializeObject<Dictionary<string, double>>((string)temp.unit);
                if(unitStr != null)
                {
                    unit = new Dictionary<PairEntry, double>();
                    foreach (KeyValuePair<string, double> kv in unitStr)
                    {
                        PairEntry pe = JsonConvert.DeserializeObject<PairEntry>(kv.Key);
                        unit.Add(pe, kv.Value);
                    }
                }

            }
            if (temp.otherOptions != null) otherOptions = JsonConvert.DeserializeObject<List<List<string>>>((string)temp.otherOptions);
            if (temp.outputUnits != null) outputUnits = JsonConvert.DeserializeObject<Dictionary<string, double>>((string)temp.outputUnits);
            if (temp.inputUnits != null) inputUnits = JsonConvert.DeserializeObject<Dictionary<string, double>>((string)temp.inputUnits);
            if (temp.choiceOption != null) choiceOption = JsonConvert.DeserializeObject<int>((string)temp.choiceOption);
            if (temp.occupancy != null) occupancy = JsonConvert.DeserializeObject<TypeOfOccupancy>((string)temp.occupancy);
            FixtureUnit funit = new FixtureUnit();
            funit.unit = unit;
            funit.outputUnits = outputUnits;
            funit.otherOptions = otherOptions;
            funit.inputUnits = inputUnits;
            funit.choiceOption = choiceOption;
            funit.occupancy = occupancy;
            return funit;
        }

        private void AddTableRecord(string data, Database db, Transaction tr, Table t)
        {
            string regName = uriInit;
            RegAppTable rat = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForRead);
            if (!rat.Has(regName))
            {
                rat.UpgradeOpen();
                RegAppTableRecord ratr = new RegAppTableRecord();
                ratr.Name = regName;
                rat.Add(ratr);
                tr.AddNewlyCreatedDBObject(ratr, true);
            }
        }
    }

    public class JsonData
    {
        public int id { get; set; }

        public TotalFacilitiesRequired minimumfixture {get; set;}
    }

    public class TotalFacilitiesRequired
    {
        public string totalFemaleCloset = "totalFemaleCloset";
        public string totalMaleCloset = "totalMaleCloset";
        public string totalMaleUrinals = "totalMaleUrinals";

        public string fixtureUnitForEdit { get; set; }
        public string outItem { get; set; }
        public bool isEditing { get; set; }
        public List<FixtureUnit> fixtureUnitArray {get; set;}
        public Dictionary<string,double> totalRequiredFixture { get; set; }
        public Dictionary<string, double> totalFixtureBasedOnGender { get; set;}
        public double femaleWaterClosetAddIn { get; set; }
        public double maleUrinalsAllowedToBeAdded { get; set; }
        public double userUrinalsAdded { get; set; }
        public int sliderValue { get; set; }
    }

    public class FixtureUnit
    {
        public Dictionary<string, double> inputUnits { get; set; }
        public Dictionary<string, double> outputUnits { get; set; }
        public List<List<string>> otherOptions { get; set;}
        public Dictionary<PairEntry, double> unit { get; set;} // the String here is PairEntry Json
        public int choiceOption { get; set; }
        public TypeOfOccupancy occupancy;
    }

    public class TypeOfOccupancy
    {
        public string id;
        public string type;
        public string sub_type;
        public string description;
    }

    public class LayerHelper
    {
        public static void CreateAndAssignALayer(string layerName)
        {
            // Get the current document and database
            ac.Document acDoc = ac.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;

                string sLayerName = layerName;

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord();

                    // Assign the layer the ACI color 1 and a name
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 13); // is like red.. but paler.
                    acLyrTblRec.Name = sLayerName;

                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }
                acTrans.Commit();
            }
        }
    }

    public class TableJig : DrawJig
    {
        Table table;
        public Point3d insertionPoint;
        
        public TableJig(Table table)
        {
            this.table = table;
            insertionPoint = new Point3d();
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            PromptPointResult pResult = prompts.AcquirePoint("Choose a point: ");
            if(pResult.Status == PromptStatus.OK)
            {
                insertionPoint = pResult.Value;
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.Cancel;
            }
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            WorldGeometry geo = draw.Geometry;
            if(geo != null)
            {
                table.Position = insertionPoint;
                geo.Draw(table);
                return true;
            }
            return false;
        }
    }
}
