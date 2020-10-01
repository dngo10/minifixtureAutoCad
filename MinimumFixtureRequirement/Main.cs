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

namespace MinimumFixtureRequirement
{
    public class Main
    {
        [CommandMethod("P_GetTable")]
        public void CreateTable()
        {
            ac.Document doc = ac.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //GET THE ID HERE..... ?
            string url = string.Format("{0}/#/minimumfixturerequirement", Goodie.hostName);
            Process process = Process.Start(url);
            process.WaitForExit();
            DialogResult result = MessageBox.Show("Press [OK] Button To Paste", "Gouvis Plumbing", MessageBoxButtons.OK);
            //getClipboard web = new getClipboard();
            if (result != DialogResult.OK) return;
            string json = Clipboard.GetText();
            if (string.IsNullOrEmpty(json)) return;
            JsonData data = GetInformation(json);
            if(data != null)
            {
            }
            return;
        }

        [CommandMethod("P_EditTable")]
        public void EditTable()
        {
            ac.Document doc = ac.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;



            Table tb = new Table();

        }

        private void EditOrCreateTable(JsonData data)
        {
            TotalFacilitiesRequired total = data.minimumfixture;
        }

        private JsonData GetInformation(string json) {
            Dictionary<string,string> map = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
            JsonData jData = new JsonData();
            if(map != null)
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
            FixtureUnit funit = new FixtureUnit();
            funit.unit = unit;
            funit.outputUnits = outputUnits;
            funit.otherOptions = otherOptions;
            funit.inputUnits = inputUnits;
            funit.choiceOption = choiceOption;
            return funit;
        }
    }

    public class JsonData
    {
        public int id { get; set; }

        public TotalFacilitiesRequired minimumfixture {get; set;}
    }

    public class TotalFacilitiesRequired
    {
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
    }

    public class PairEntry
    {
        public string t1 { get; set; }
        public string t2 {get; set;}
    }

    public class PairEntry2
    {
    }
}
