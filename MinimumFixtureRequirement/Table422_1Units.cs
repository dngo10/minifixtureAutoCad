using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimumFixtureRequirement
{
    class Table422_1Units
    {
        public static string person { get; private set; } = "person";
        public static string male { get; private set; } = "male";
        public static string female { get; private set; } = "female";
        public static string apartment { get; private set; } = "apartment";
        public static string floorOrcellBlock { get; private set; } = "cellblock/floor";
        public static string floor { get; private set; } = "floor";
        public static string room { get; private set; } = "room";
        public static string familydwelling { get; private set; } = "familydwelling";
        public static string cell { get; private set; } = "cell";
        public static string sleepingroom { get; private set; } = "sleepingroom";
        public static string patient { get; private set; } = "patient";
        public static string none { get; private set; } = "none";

        public static HashSet<string> InputSet = new HashSet<string>{
        person,
        male,
        female,
        apartment,
        floorOrcellBlock,
        floor,
        room,
        familydwelling,
        cell,
        sleepingroom,
        patient,
        none // No unit specified.
        };
    }

    class Table422_1Categories
    {
        public static string waterClosets { get; private set; } = "WATER CLOSETS";
        public static string urinals { get; private set; } = "URINALS";
        public static string lavatories { get; private set; } = "LAVATORIES";
        public static string bathtubsOrShowers { get; private set; } = "BATHTUBS OR SHOWERS";
        public static string drinkingFountains { get; private set; } = "DRINKING FOUNTAINS/ FACILITIES";
        public static string servicesink { get; private set; } = "SERVICE SINK";
        public static string laundrytray { get; private set; } = "LAUNDRY TRAY";
        public static string kitchensink { get; private set; } = "KITCHEN SINK";
        public static string automaticclotheswasherconnection { get; private set; } = "AUTOMATIC CLOTHES WASHER CONNECTION";
        public static string big_landrytray { get; private set; } = "LAUNDRY ROOM";
        public static string big_automaticclotheswasherconnection { get; private set; } = "PUBLIC AUTOMATIC LAUNDRY";

        public static HashSet<string> OtherSet { get; private set; } = new HashSet<string>{
          servicesink,
          laundrytray,
          kitchensink,
          automaticclotheswasherconnection,
          big_landrytray,
          big_automaticclotheswasherconnection,
        };

        public static HashSet<string> OutputSet = new HashSet<string>{
            waterClosets,
            urinals,
            lavatories,
            bathtubsOrShowers,
            drinkingFountains,
            servicesink,
            laundrytray,
            kitchensink,
            automaticclotheswasherconnection,
            big_landrytray,
            big_automaticclotheswasherconnection,
        };
    }

    class Table422_1Ids
    {
        public static string A1 { get; private set; } = "A1";
        public static string A2 { get; private set; } = "A2";
        public static string A3 { get; private set; } = "A3";
        public static string A4 { get; private set; } = "A4";
        public static string A5 { get; private set; } = "A5";
        public static string B { get; private set; } = "B";
        public static string E { get; private set; } = "E";
        public static string F1 { get; private set; } = "F1";
        public static string F2 { get; private set; } = "F2";
        public static string I1 { get; private set; } = "I1";
        public static string I2_1 { get; private set; } = "I2_1";
        public static string I2_2 { get; private set; } = "I2_2";
        public static string I2_3 { get; private set; } = "I2_3";
        public static string I3_1 { get; private set; } = "I3_1";
        public static string I3_2 { get; private set; } = "I3_2";
        public static string I3_3 { get; private set; } = "I3_3";
        public static string I4 { get; private set; } = "I4";
        public static string M { get; private set; } = "M";
        public static string R1 { get; private set; } = "R1";
        public static string R2_1 { get; private set; } = "R2_1";
        public static string R2_2 { get; private set; } = "R2_2";
        public static string R2_3 { get; private set; } = "R2_3";
        public static string R3_1 { get; private set; } = "R3_1";
        public static string R3_2 { get; private set; } = "R3_2";
        public static string R4 { get; private set; } = "R4";
        public static string S1 { get; private set; } = "S2";
        public static string S2 { get; private set; } = "S1";
    }

    public class PairEntry
    {
        public string t1;
        public string t2;

        public PairEntry(string t1, string t2)
        {
            this.t1 = t1;
            this.t2 = t2;
        }

        public string toString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
