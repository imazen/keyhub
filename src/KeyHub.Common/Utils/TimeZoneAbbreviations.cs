using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Common.Utils
{
    public class TimeZoneAbbreviations
    {
        /// <summary>
        /// Attempts to replace a trailing time zone abbreviation (Like " MST") with the correct time zone offset in 'zzz' format "-07:00". 
        /// If the abbreviation is not recognized, the string is left alone.
        /// </summary>
        /// <param name="timeDateString"></param>
        /// <returns></returns>
        public static string ReplaceTimeZoneAbbreviation(string timeDateString)
        {
            Load();
            var lastSpace = timeDateString.LastIndexOf(' ');
            var timeZone = timeDateString.Substring(lastSpace + 1);
            var s = timeDateString.Substring(0, lastSpace + 1);
            string replacement;
            if (abbreviations.TryGetValue(timeZone, out replacement)) return s + replacement;
            else return timeDateString;
        }

        private static void Load()
        {
            lock (syncLock)
            {
                if (!loaded)
                {
                    Populate();
                    loaded = true;
                }
            }
        }
        private static object syncLock = new object();
        private static bool loaded = false;
        private static Dictionary<string, string> abbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


        private static void AddZone(string name, string offset)
        {
            if (offset.StartsWith("UTC")) offset = offset.Substring(3);
            if (offset.Length == 0) offset = "+00";
            if (!offset.Contains(":")) offset += ":00";
            abbreviations.Add(name, offset.Replace('−','-')); 
        }
        private static void Populate()
        {
            //From http://en.wikipedia.org/wiki/List_of_time_zone_abbreviations
            AddZone("ACDT","UTC+10:30");//	Australian Central Daylight Time
            AddZone("ACST","UTC+09:30");//	Australian Central Standard Time
            AddZone("ACT","UTC+08");//	ASEAN Common Time
            AddZone("ADT","UTC−03");//	Atlantic Daylight Time
            AddZone("AEDT","UTC+11");//	Australian Eastern Daylight Time
            AddZone("AEST","UTC+10");//	Australian Eastern Standard Time
            AddZone("AFT","UTC+04:30");//	Afghanistan Time
            AddZone("AKDT","UTC−08");//	Alaska Daylight Time
            AddZone("AKST","UTC−09");//	Alaska Standard Time
            AddZone("AMST","UTC+05");//	Armenia Summer Time
            AddZone("AMT","UTC+04");//	Armenia Time
            AddZone("ART","UTC−03");//	Argentina Time
            //AddZone("AST","UTC+03");//	Arab Standard Time (Kuwait, Riyadh)
            //AddZone("AST","UTC+04");//	Arabian Standard Time (Abu Dhabi, Muscat)
            //AddZone("AST","UTC+03");//	Arabic Standard Time (Baghdad)
            AddZone("AST","UTC−04");//	Atlantic Standard Time
            AddZone("AWDT","UTC+09");//	Australian Western Daylight Time
            AddZone("AWST","UTC+08");//	Australian Western Standard Time
            AddZone("AZOST","UTC−01");//	Azores Standard Time
            AddZone("AZT","UTC+04");//	Azerbaijan Time
            AddZone("BDT","UTC+08");//	Brunei Time
            AddZone("BIOT","UTC+06");//	British Indian Ocean Time
            AddZone("BIT","UTC−12");//	Baker Island Time
            AddZone("BOT","UTC−04");//	Bolivia Time
            AddZone("BRT","UTC−03");//	Brasilia Time
            AddZone("BST","UTC+06");//	Bangladesh Standard Time
            //AddZone("BST","UTC+01");//	British Summer Time (British Standard Time from Feb 1968 to Oct 1971)
            AddZone("BTT","UTC+06");//	Bhutan Time
            AddZone("CAT","UTC+02");//	Central Africa Time
            AddZone("CCT","UTC+06:30");//	Cocos Islands Time
            AddZone("CDT","UTC−05");//	Central Daylight Time (North America)
            //AddZone("CDT","UTC−04");//	Cuba Daylight Time[1]
            AddZone("CEDT","UTC+02");//	Central European Daylight Time
            AddZone("CEST","UTC+02");//	Central European Summer Time (Cf. HAEC)
            AddZone("CET","UTC+01");//	Central European Time
            AddZone("CHADT","UTC+13:45");//	Chatham Daylight Time
            AddZone("CHAST","UTC+12:45");//	Chatham Standard Time
            AddZone("CHOT","UTC-08");//	Choibalsan
            AddZone("ChST","UTC+10");//	Chamorro Standard Time
            AddZone("CHUT","UTC+10");//	Chuuk Time
            AddZone("CIST","UTC-08");//	Clipperton Island Standard Time
            AddZone("CIT","UTC+08");//	Central Indonesia Time
            AddZone("CKT","UTC-10");//	Cook Island Time
            AddZone("CLST","UTC−03");//	Chile Summer Time
            AddZone("CLT","UTC−04");//	Chile Standard Time
            AddZone("COST","UTC−04");//	Colombia Summer Time
            AddZone("COT","UTC−05");//	Colombia Time
            AddZone("CST","UTC−06");//	Central Standard Time (North America)
            //AddZone("CST","UTC+08");//	China Standard Time
            //AddZone("CST","UTC+09:30");//	Central Standard Time (Australia)
            //AddZone("CST","UTC+10:30");//	Central Summer Time (Australia)
            //AddZone("CST","UTC-05");//	Cuba Standard Time
            AddZone("CT","UTC+08");//	China time
            AddZone("CVT","UTC−01");//	Cape Verde Time
            AddZone("CWST","UTC+08:45");//	Central Western Standard Time (Australia)
            AddZone("CXT","UTC+07");//	Christmas Island Time
            AddZone("DAVT","UTC+07");//	Davis Time
            AddZone("DDUT","UTC+10");//	Dumont d'Urville Time
            AddZone("DFT","UTC+01");//	AIX specific equivalent of Central European Time[2]
            AddZone("EASST","UTC−05");//	Easter Island Standard Summer Time
            AddZone("EAST","UTC−06");//	Easter Island Standard Time
            AddZone("EAT","UTC+03");//	East Africa Time
            AddZone("ECT","UTC−04");//	Eastern Caribbean Time (does not recognise DST)
            //AddZone("ECT","UTC−05");//	Ecuador Time
            AddZone("EDT","UTC−04");//	Eastern Daylight Time (North America)
            AddZone("EEDT","UTC+03");//	Eastern European Daylight Time
            AddZone("EEST","UTC+03");//	Eastern European Summer Time
            AddZone("EET","UTC+02");//	Eastern European Time
            AddZone("EGST","UTC+00");//	Eastern Greenland Summer Time
            AddZone("EGT","UTC-01");//	Eastern Greenland Time
            AddZone("EIT","UTC+09");//	Eastern Indonesian Time
            AddZone("EST","UTC−05");//	Eastern Standard Time (North America)
            //AddZone("EST","UTC+10");//	Eastern Standard Time (Australia)
            AddZone("FET","UTC+03");//	Further-eastern_European_Time
            AddZone("FJT","UTC+12");//	Fiji Time
            AddZone("FKST","UTC−03");//	Falkland Islands Summer Time
            AddZone("FKT","UTC−04");//	Falkland Islands Time
            AddZone("FNT","UTC-02");//	Fernando de Noronha Time
            AddZone("GALT","UTC−06");//	Galapagos Time
            AddZone("GAMT","UTC−09");//	Gambier Islands
            AddZone("GET","UTC+04");//	Georgia Standard Time
            AddZone("GFT","UTC−03");//	French Guiana Time
            AddZone("GILT","UTC+12");//	Gilbert Island Time
            AddZone("GIT","UTC−09");//	Gambier Island Time
            AddZone("GMT","UTC");//	Greenwich Mean Time
            //AddZone("GST","UTC−02");//	South Georgia and the South Sandwich Islands
            AddZone("GST","UTC+04");//	Gulf Standard Time
            AddZone("GYT","UTC−04");//	Guyana Time
            AddZone("HADT","UTC−09");//	Hawaii-Aleutian Daylight Time
            AddZone("HAEC","UTC+02");//	Heure Avancée d'Europe Centrale francised name for CEST
            AddZone("HAST","UTC−10");//	Hawaii-Aleutian Standard Time
            AddZone("HKT","UTC+08");//	Hong Kong Time
            AddZone("HMT","UTC+05");//	Heard and McDonald Islands Time
            AddZone("HOVT","UTC+07");//	Khovd Time
            AddZone("HST","UTC−10");//	Hawaii Standard Time
            AddZone("ICT","UTC+07");//	Indochina Time
            AddZone("IDT","UTC+03");//	Israel Daylight Time
            AddZone("IOT","UTC+03");//	Indian Ocean Time
            AddZone("IRDT","UTC+08");//	Iran Daylight Time
            AddZone("IRKT","UTC+08");//	Irkutsk Time
            AddZone("IRST","UTC+03:30");//	Iran Standard Time
            AddZone("IST","UTC+05:30");//	Indian Standard Time
            //AddZone("IST","UTC+01");//	Irish Summer Time
            //AddZone("IST","UTC+02");//	Israel Standard Time
            AddZone("JST","UTC+09");//	Japan Standard Time
            AddZone("KGT","UTC+06");//	Kyrgyzstan time
            AddZone("KOST","UTC+11");//	Kosrae Time
            AddZone("KRAT","UTC+07");//	Krasnoyarsk Time
            AddZone("KST","UTC+09");//	Korea Standard Time
            AddZone("LHST","UTC+10:30");//	Lord Howe Standard Time
            //AddZone("LHST","UTC+11");//	Lord Howe Summer Time
            AddZone("LINT","UTC+14");//	Line Islands Time
            AddZone("MAGT","UTC+12");//	Magadan Time
            AddZone("MART","UTC-09:30");//	Marquesas Islands Time
            AddZone("MAWT","UTC+05");//	Mawson Station Time
            AddZone("MDT","UTC−06");//	Mountain Daylight Time (North America)
            AddZone("MET","UTC+01");//	Middle European Time Same zone as CET
            AddZone("MEST","UTC+02");//	Middle European Saving Time Same zone as CEST
            AddZone("MHT","UTC+12");//	Marshall Islands
            AddZone("MIST","UTC+11");//	Macquarie Island Station Time
            AddZone("MIT","UTC−09:30");//	Marquesas Islands Time
            AddZone("MMT","UTC+06:30");//	Myanmar Time
            AddZone("MSK","UTC+04");//	Moscow Time
            //AddZone("MST","UTC+08");//	Malaysia Standard Time
            AddZone("MST","UTC−07");//	Mountain Standard Time (North America)
           // AddZone("MST","UTC+06:30");//	Myanmar Standard Time
            AddZone("MUT","UTC+04");//	Mauritius Time
            //AddZone("MVT","UTC+05");//	Maldives Time
            AddZone("MYT","UTC+08");//	Malaysia Time
            AddZone("NCT","UTC+11");//	New Caledonia Time
            AddZone("NDT","UTC−02:30");//	Newfoundland Daylight Time
            AddZone("NFT","UTC+11:30");//	Norfolk Time
            AddZone("NPT","UTC+05:45");//	Nepal Time
            AddZone("NST","UTC−03:30");//	Newfoundland Standard Time
            AddZone("NT","UTC−03:30");//	Newfoundland Time
            AddZone("NUT","UTC−11:30");//	Niue Time
            AddZone("NZDT","UTC+13");//	New Zealand Daylight Time
            AddZone("NZST","UTC+12");//	New Zealand Standard Time
            AddZone("OMST","UTC+06");//	Omsk Time
            AddZone("ORAT","UTC-05");//	Oral Time
            AddZone("PDT","UTC−07");//	Pacific Daylight Time (North America)
            AddZone("PET","UTC-05");//	Peru Time
            AddZone("PETT","UTC+12");//	Kamchatka Time
            AddZone("PGT","UTC+10");//	Papua New Guinea Time
            AddZone("PHOT","UTC+13");//	Phoenix Island Time
            AddZone("PHT","UTC+08");//	Philippine Time
            AddZone("PKT","UTC+05");//	Pakistan Standard Time
            AddZone("PMDT","UTC+08");//	Saint Pierre and Miquelon Daylight time
            AddZone("PMST","UTC+08");//	Saint Pierre and Miquelon Standard Time
            AddZone("PONT","UTC+11");//	Pohnpei Standard Time
            AddZone("PST","UTC−08");//	Pacific Standard Time (North America)
            //AddZone("PST","UTC+08");//	Philippine Standard Time
            AddZone("RET","UTC+04");//	Réunion Time
            AddZone("ROTT","UTC-03");//	Rothera Research Station Time
            AddZone("SAKT","UTC+11");//	Sakhalin Island time
            AddZone("SAMT","UTC+04");//	Samara Time
            AddZone("SAST","UTC+02");//	South African Standard Time
            AddZone("SBT","UTC+11");//	Solomon Islands Time
            AddZone("SCT","UTC+04");//	Seychelles Time
            AddZone("SGT","UTC+08");//	Singapore Time
            AddZone("SLT","UTC+05:30");//	Sri Lanka Time
            AddZone("SRT","UTC−03");//	Suriname Time
            //AddZone("SST","UTC−11");//	Samoa Standard Time
            AddZone("SST","UTC+08");//	Singapore Standard Time
            AddZone("SYOT","UTC+03");//	Showa Station Time
            AddZone("TAHT","UTC−10");//	Tahiti Time
            AddZone("THA","UTC+07");//	Thailand Standard Time
            AddZone("TFT","UTC+05");//	Indian/Kerguelen
            AddZone("TJT","UTC+05");//	Tajikistan Time
            AddZone("TKT","UTC+14");//	Tokelau Time
            AddZone("TLT","UTC+09");//	Timor Leste Time
            AddZone("TMT","UTC+05");//	Turkmenistan Time
            AddZone("TOT","UTC+13");//	Tonga Time
            AddZone("TVT","UTC+12");//	Tuvalu Time
            AddZone("UCT","UTC");//	Coordinated Universal Time
            AddZone("ULAT","UTC+08");//	Ulaanbaatar Time
            AddZone("UTC","UTC");//	Coordinated Universal Time
            AddZone("UYST","UTC−02");//	Uruguay Summer Time
            AddZone("UYT","UTC−03");//	Uruguay Standard Time
            AddZone("UZT","UTC+05");//	Uzbekistan Time
            AddZone("VET","UTC−04:30");//	Venezuelan Standard Time
            AddZone("VLAT","UTC+10");//	Vladivostok Time
            AddZone("VOLT","UTC+04");//	Volgograd Time
            AddZone("VOST","UTC+06");//	Vostok Station Time
            AddZone("VUT","UTC+11");//	Vanuatu Time
            AddZone("WAKT","UTC+12");//	Wake Island Time
            AddZone("WAST","UTC+02");//	West Africa Summer Time
            AddZone("WAT","UTC+01");//	West Africa Time
            AddZone("WEDT","UTC+01");//	Western European Daylight Time
            AddZone("WEST","UTC+01");//	Western European Summer Time
            AddZone("WET","UTC");//	Western European Time
            AddZone("WST","UTC+08");//	Western Standard Time
            AddZone("YAKT","UTC+09");//	Yakutsk Time
            AddZone("YEKT","UTC+05");//	Yekaterinburg Time
        }

    }
}
