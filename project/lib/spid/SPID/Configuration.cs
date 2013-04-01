using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SPID {

    //[DefaultPropertyAttribute("DivergenceThreshold")]
    public class Configuration {

        public enum DateTimeFormat { CURRENT_UI_CULTURE, INVARIANT_CULTURE, UNIX_TIME }

        private static Configuration singletonInstance=null;

        /*
        public static void CreateInstance(string configXmlFile) {
            singletonInstance=new Configuration(configXmlFile);
        }*/

        public static IEnumerable<Configuration> GetInstances(string configXmlFile) {
            string[] options = GetOptionalAttributeMeterNames(configXmlFile);
            List<string> activatedOptions = new List<string>();
            if(options.Length==0) {
                singletonInstance = new Configuration(configXmlFile, activatedOptions);
                yield return singletonInstance;
            }
            else {
                foreach(string option in options) {
                    activatedOptions.Clear();
                    activatedOptions.Add(option);
                    singletonInstance = new Configuration(configXmlFile, activatedOptions);
                    yield return singletonInstance;
                }
            }
        }

        public static Configuration GetInstance(){
            return singletonInstance;
        }

        private static string[] GetOptionalAttributeMeterNames(string configXmlFile) {
            System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            xmlDoc.Load(configXmlFile);
            System.Xml.XmlNode attributeMetersNode=xmlDoc.DocumentElement.SelectSingleNode("/config/attributeMeters");
            List<AttributeMeterSetting> tmpAttributeMeterSettings=new List<AttributeMeterSetting>();
            System.Xml.XmlNodeList optionalAttributeMeterNodes = attributeMetersNode.SelectNodes("attributeMeter[@active='optional']");
            string[] returnArray=new string[optionalAttributeMeterNodes.Count];
            for(int i=0; i<returnArray.Length; i++)
                returnArray[i]=optionalAttributeMeterNodes[i].SelectSingleNode("@attributeName").Value;
            return returnArray;
        }

        private string configXmlFile;

        private double divergenceThreshold;
        private int maxSimultaneousSessions;
        private int maxFramesToInspectPerSession;
        private bool displayAllProtocolModelDivergences;
        private bool displayLogFile;
        private bool appendUnidirectionalSessions;//only works for TCP
        private DateTimeFormat timestampFormat;
        private ICollection<string> activeAttributeMeters;
        private System.Collections.ObjectModel.ReadOnlyCollection<AttributeMeterSetting> attributeMeterSettings;

        [DescriptionAttribute("A high value might generate false positives and a low value might generate false negatives. A value of 2.2 or 2.3 is appropriate when all attributeMeters are used. ")]
        public double DivergenceThreshold { get { return this.divergenceThreshold; } set { this.divergenceThreshold=value; } }
        [DescriptionAttribute("More than 1000 sessions can cause your RAM to fill up")]        
        public int MaxSimultaneousSessions { get { return this.maxSimultaneousSessions; } set { this.maxSimultaneousSessions=value; } }
        [DescriptionAttribute("There is no point in setting a value higher than 100 since the models have only been trained on the first 100 packets of various sessions")]
        public int MaxFramesToInspectPerSession { get { return this.maxFramesToInspectPerSession; } set { this.maxFramesToInspectPerSession=value; } }
        [DescriptionAttribute("Whether or not to display the session details with divergence measurements")]
        public bool DisplayAllProtocolModelDivergences { get { return this.displayAllProtocolModelDivergences; } set { this.displayAllProtocolModelDivergences=value; } }
        [DescriptionAttribute("The format to use for timestamps")]
        public DateTimeFormat TimestampFormat { get { return this.timestampFormat; } set { this.timestampFormat=value; } }
        [DescriptionAttribute("Whether or not to display the .txt log file after parsing a pcap file")]
        public bool DisplayLogFile { get { return this.displayLogFile; } set { this.displayLogFile=value; } }
        [DescriptionAttribute("Whether or not protocol models for unidirectional flows should be created when reading training data (only works for TCP sessions)")]
        public bool AppendUnidirectionalSessions { get { return this.appendUnidirectionalSessions; } set { this.appendUnidirectionalSessions=value; } }

        [Browsable(false)]
        public ICollection<string> ActiveAttributeMeters { get { return this.activeAttributeMeters; } }
        //[DisplayName("Attribute Meter Settings")]
        
        [DescriptionAttribute("Disabling attribute meters will improve speed and free more RAM")]
        public System.Collections.ObjectModel.ReadOnlyCollection<AttributeMeterSetting> AttributeMeterSettings { get { return this.attributeMeterSettings; } }


        private Configuration(string configXmlFile, ICollection<string> activatedOptionalAttributeMeterNames) {
            this.configXmlFile=configXmlFile;
            
            //read the config file
            this.Load(activatedOptionalAttributeMeterNames);
        }



        public void Load() {
            Load(new List<string>());
        }
        public void Load(ICollection<string> activatedOptionalAttributeMeterNames) {
            System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            xmlDoc.Load(configXmlFile);

            System.Xml.XmlNode divergenceNode=xmlDoc.DocumentElement.SelectSingleNode("/config/divergenceThreshold");
            this.divergenceThreshold=Double.Parse(divergenceNode.InnerText, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            System.Xml.XmlNode maxNode=xmlDoc.DocumentElement.SelectSingleNode("/config/maxSimultaneousSessions");
            this.maxSimultaneousSessions=Int32.Parse(maxNode.InnerText, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            System.Xml.XmlNode framesCountNode=xmlDoc.DocumentElement.SelectSingleNode("/config/maxFramesToInspectPerSession");
            this.maxFramesToInspectPerSession=Int32.Parse(framesCountNode.InnerText, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            System.Xml.XmlNode displayAllNode=xmlDoc.DocumentElement.SelectSingleNode("/config/displayAllProtocolModelDivergences");
            this.displayAllProtocolModelDivergences=displayAllNode.InnerText.Trim()=="true";

            System.Xml.XmlNode timestampFormatNode=xmlDoc.DocumentElement.SelectSingleNode("/config/timestampFormat");
            this.timestampFormat=(DateTimeFormat)Int32.Parse(timestampFormatNode.InnerText, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            System.Xml.XmlNode displayLogFileNode=xmlDoc.DocumentElement.SelectSingleNode("/config/displayLogFile");
            this.displayLogFile=displayLogFileNode.InnerText.Trim()=="true";

            System.Xml.XmlNode unidirNode=xmlDoc.DocumentElement.SelectSingleNode("/config/appendUnidirectionalSessions");
            this.appendUnidirectionalSessions=unidirNode.InnerText.Trim()=="true";


            System.Xml.XmlNode attributeMetersNode=xmlDoc.DocumentElement.SelectSingleNode("/config/attributeMeters");
            List<AttributeMeterSetting> tmpAttributeMeterSettings=new List<AttributeMeterSetting>();
            System.Xml.XmlNodeList attributeMeterNodes = attributeMetersNode.SelectNodes("attributeMeter");
            for(int i=0; i<attributeMeterNodes.Count; i++) {
                AttributeMeterSetting s=new AttributeMeterSetting(attributeMeterNodes[i].SelectSingleNode("@attributeName").Value);
                if(attributeMeterNodes[i].SelectSingleNode("@active").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    s.Active=true;
                else if(activatedOptionalAttributeMeterNames.Contains(s.Name) && attributeMeterNodes[i].SelectSingleNode("@active").Value.Equals("optional", StringComparison.InvariantCultureIgnoreCase))
                    s.Active=true;
                else
                    s.Active=false;
                tmpAttributeMeterSettings.Add(s);
            }
            /*
            foreach(System.Xml.XmlNode attributeMeterNode in attributeMetersNode.SelectNodes("attributeMeter")) {
                AttributeMeterSetting s=new AttributeMeterSetting(attributeMeterNode.SelectSingleNode("@attributeName").Value);
                if(attributeMeterNode.SelectSingleNode("@active").Value=="true")
                    s.Active=true;
                else
                    s.Active=false;
                tmpAttributeMeterSettings.Add(s);
            }
             * */
            this.attributeMeterSettings=new System.Collections.ObjectModel.ReadOnlyCollection<AttributeMeterSetting>(tmpAttributeMeterSettings);

            //make a short test to ensure that the config.xml holds all the protocols
            ProtocolIdentification.ProtocolModel pTest=new ProtocolIdentification.ProtocolModel("test");
            if(pTest.AttributeFingerprintHandlers.Count!=attributeMetersNode.SelectNodes("attributeMeter").Count)
                throw new Exception("The number of fingerprints in config.xml is not correct!\nXML value="+attributeMetersNode.SelectNodes("attributeName").Count+", should be "+pTest.AttributeFingerprintHandlers.Count);
            foreach(System.Xml.XmlNode n in attributeMetersNode.SelectNodes("attributeMeter"))
                if(!pTest.AttributeFingerprintHandlers.Keys.Contains(n.SelectSingleNode("@attributeName").Value))
                    throw new Exception("AttributeMeter "+n.SelectSingleNode("@attributeName").Value+" does not exist (only in config.xml)");

            this.activeAttributeMeters=new List<string>();
            foreach(AttributeMeterSetting s in this.attributeMeterSettings)
                if(s.Active)
                    this.activeAttributeMeters.Add(s.Name);
        }


        public void Save() {
            System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            xmlDoc.Load(configXmlFile);

            System.Xml.XmlNode divergenceNode=xmlDoc.DocumentElement.SelectSingleNode("/config/divergenceThreshold");
            divergenceNode.InnerText=this.divergenceThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            
            System.Xml.XmlNode maxNode=xmlDoc.DocumentElement.SelectSingleNode("/config/maxSimultaneousSessions");
            maxNode.InnerText=this.maxSimultaneousSessions.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            
            System.Xml.XmlNode framesCountNode=xmlDoc.DocumentElement.SelectSingleNode("/config/maxFramesToInspectPerSession");
            framesCountNode.InnerText=this.maxFramesToInspectPerSession.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            System.Xml.XmlNode displayAllNode=xmlDoc.DocumentElement.SelectSingleNode("/config/displayAllProtocolModelDivergences");
            if(this.displayAllProtocolModelDivergences)
                displayAllNode.InnerText="true";
            else
                displayAllNode.InnerText="false";

            System.Xml.XmlNode timestampFormatNode=xmlDoc.DocumentElement.SelectSingleNode("/config/timestampFormat");
            timestampFormatNode.InnerText=((int)this.timestampFormat).ToString();

            System.Xml.XmlNode displayLogFileNode=xmlDoc.DocumentElement.SelectSingleNode("/config/displayLogFile");
            if(this.displayLogFile)
                displayLogFileNode.InnerText="true";
            else
                displayLogFileNode.InnerText="false";

            System.Xml.XmlNode unidirNode=xmlDoc.DocumentElement.SelectSingleNode("/config/appendUnidirectionalSessions");
            if(this.appendUnidirectionalSessions)
                unidirNode.InnerText="true";
            else
                unidirNode.InnerText="false";

            //System.Xml.XmlNode attributeMetersNode=xmlDoc.DocumentElement.SelectSingleNode("/config/attributeMeters");
            foreach(AttributeMeterSetting s in this.attributeMeterSettings) {
                System.Xml.XmlNode attributeMeterNode=xmlDoc.DocumentElement.SelectSingleNode("/config/attributeMeters/attributeMeter[@attributeName='"+s.Name+"']");
                if(s.Active)
                    attributeMeterNode.SelectSingleNode("@active").Value="true";
                else
                    attributeMeterNode.SelectSingleNode("@active").Value="false";
            }

            xmlDoc.Save(configXmlFile);
        }

        public string FormatDateTime(DateTime timestamp) {
            if(this.TimestampFormat==DateTimeFormat.INVARIANT_CULTURE)
                return timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);
            else if(this.TimestampFormat==Configuration.DateTimeFormat.UNIX_TIME) {
                //from PcapFileHandler.PcapFileWriter.cs in NetworkMiner
                DateTime referenceTime=new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan delta=timestamp.ToUniversalTime().Subtract(referenceTime);
                //The smallest unit of time is the tick, which is equal to 100 nanoseconds.
                long totalMicroseconds=delta.Ticks/10;
                uint seconds=(uint)(totalMicroseconds/1000000);
                uint microseconds=(uint)(totalMicroseconds%1000000);
                return seconds.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)+"."+microseconds.ToString("D6", System.Globalization.CultureInfo.InvariantCulture.NumberFormat)+"000";
            }
            else//use local culture
                return timestamp.ToString(System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
        }

        public String ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("# config/divergenceThreshold: "+this.divergenceThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)+"\r\n");
            sb.Append("# config/maxSimultaneousSessions: "+this.maxSimultaneousSessions.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)+"\r\n");
            sb.Append("# config/maxFramesToInspectPerSession: "+this.maxFramesToInspectPerSession.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)+"\r\n");
            sb.Append("# config/displayAllProtocolModelDivergences: ");
            if(this.displayAllProtocolModelDivergences)
                sb.Append("true\r\n");
            else
                sb.Append("false\r\n");
            sb.Append("# config/timestampFormat: "+this.timestampFormat.ToString()+"\r\n");
            sb.Append("# config/displayLogFile: ");
            if(this.displayLogFile)
                sb.Append("true\r\n");
            else
                sb.Append("false\r\n");

            sb.Append("# config/appendUnidirectionalSessions: ");
            if(this.appendUnidirectionalSessions)
                sb.Append("true\r\n");
            else
                sb.Append("false\r\n");

            foreach(AttributeMeterSetting s in this.attributeMeterSettings) {
                sb.Append("# config/attributeMeters/attributeMeter/"+s.Name+": ");
                if(s.Active)
                    sb.Append("active\r\n");
                else
                    sb.Append("-\r\n");
            }
            return sb.ToString();
        }
    }


    public class AttributeMeterSetting {
        private string name;
        private bool active;

        public AttributeMeterSetting(string name) {
            this.name=name;
        }

        public string Name { get { return this.name; } }
        public bool Active { get { return this.active; } set { this.active=value; } }
    }

}
