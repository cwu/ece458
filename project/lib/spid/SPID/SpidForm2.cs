using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SPID {
    public partial class SpidForm2 : Form {

        private Configuration config;

        private SortedList<string, ProtocolIdentification.ProtocolModel> protocolModels;


        public SpidForm2(IEnumerable<string> configFilenames, string protocolDatabaseFilename, IEnumerable<string> pcapFilenames, bool automaticExit, System.IO.TextWriter statusLogger, bool ouputDirectoryIsPcapDirectory, string traningDataDirectory) {
            InitializeComponent();

            

            foreach(string configFilename in configFilenames) {
                if(statusLogger!=null)
                    statusLogger.WriteLine("Loading config file : "+configFilename);
                foreach(Configuration c in Configuration.GetInstances(configFilename)) {

                    this.config=c;

                    //10 000 sessions á 30kB => 300 MB
                    //this.sessionHandler=new SessionHandler(10000);
                    this.protocolModels=new SortedList<string, ProtocolIdentification.ProtocolModel>();
                    try {
                        openProtocolModelDatabaseFile(protocolDatabaseFilename, false);
                    }
                    catch(Exception e) {
                        MessageBox.Show("Unable to load protocol database "+protocolDatabaseFilename+".\n"+e.Message, "Database Not Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.IO.FileInfo outputPath = new System.IO.FileInfo(Application.ExecutablePath);
                    foreach(string pcapFilename in pcapFilenames) {
                        if(statusLogger!=null)
                            statusLogger.WriteLine("Opeining pcap file : "+pcapFilename);
                        if(ouputDirectoryIsPcapDirectory)
                            outputPath = new System.IO.FileInfo(pcapFilename);
                        this.OpenSessionPcapFile(pcapFilename, outputPath);
                    }
                }
            }
            if(traningDataDirectory!=null && traningDataDirectory.Length>0) {
                if(statusLogger!=null)
                    statusLogger.WriteLine("Loading training data : "+traningDataDirectory);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(traningDataDirectory);
                this.LoadTrainingDataFolder(di, false);
                if(statusLogger!=null)
                    statusLogger.WriteLine("Saving protocol database : "+di.Name+".xml");
                this.SaveProtocolModelDatabase(di.Name+".xml");
            }
            if(automaticExit) {
                Environment.Exit(0);
                //base.Close();
            }
        }

        private void trainingDataFolderToolStripMenuItem_Click(object sender, EventArgs e) {

            if(this.trainingDataFolderBrowserDialog.ShowDialog()==DialogResult.OK) {
                //open new training data folder (replace old database)
                System.IO.DirectoryInfo trainingDataFolderInfo=new System.IO.DirectoryInfo(this.trainingDataFolderBrowserDialog.SelectedPath);
                this.LoadTrainingDataFolder(trainingDataFolderInfo, false);
            }
        }

        private void trainingDataFolderToolStripMenuItem1_Click(object sender, EventArgs e) {
            if(this.trainingDataFolderBrowserDialog.ShowDialog()==DialogResult.OK) {
                //append training data folder to existing database
                System.IO.DirectoryInfo trainingDataFolderInfo=new System.IO.DirectoryInfo(this.trainingDataFolderBrowserDialog.SelectedPath);
                this.LoadTrainingDataFolder(trainingDataFolderInfo, true);
            }
        }

        private void LoadTrainingDataFolder(System.IO.DirectoryInfo trainingDataFolderInfo, bool appendToExistingDatabase) {
            if(!appendToExistingDatabase)
                this.protocolModels.Clear();
            this.protocolModelsListView.Items.Clear();
            //Display loading form
            LoadingSimple loadingForm=new LoadingSimple();
            loadingForm.Show();
            if(!loadingForm.InvokeRequired)
                loadingForm.AppendLine("Loading training data from: "+trainingDataFolderInfo.ToString());
            foreach(ProtocolIdentification.ProtocolModel protocolModel in GetProtocolModels(trainingDataFolderInfo)) {
                //Display text about the loaded protocol model
                if(!loadingForm.InvokeRequired)
                    loadingForm.AppendLine(protocolModel.ProtocolName+" "+protocolModel.ObservationCount.ToString()+" observations");

                if(this.protocolModels.ContainsKey(protocolModel.ProtocolName))
                    this.protocolModels[protocolModel.ProtocolName]=this.protocolModels[protocolModel.ProtocolName].MergeWith(protocolModel);
                else
                    this.protocolModels.Add(protocolModel.ProtocolName, protocolModel);
            }

            //add the models to the GUI list
            foreach(ProtocolIdentification.ProtocolModel protocolModel in this.protocolModels.Values) {
                this.protocolModelsListView.Items.Add(GetProtocolModelListViewItem(protocolModel));
            }

            //Remove/close loading form
            loadingForm.Close();
            
        }

        private IEnumerable<ProtocolIdentification.ProtocolModel> GetProtocolModels(System.IO.DirectoryInfo di) {
            //a bit of recursion here
            foreach(System.IO.DirectoryInfo subDirectoryInfo in di.GetDirectories()) {
                foreach(ProtocolIdentification.ProtocolModel protocolModel in GetProtocolModels(subDirectoryInfo))
                    yield return protocolModel;
            }
            foreach(System.IO.FileInfo pcapFileInfo in di.GetFiles("*.*cap")) {
                foreach(ProtocolIdentification.ProtocolModel model in ReadProtocolModelsFromSessionFile(pcapFileInfo.FullName, di.Name, config.MaxFramesToInspectPerSession, config.AppendUnidirectionalSessions))
                    yield return model;
            }
        }

        /// <summary>
        /// This function ONLY works when the pcap file only contains one TCP/UDP session
        /// i.e. "Follow TCP stream" has been used in Wireshark
        /// </summary>
        /// <param name="filename">The pcap file to open</param>
        /// <param name="maxFramesToParse">100 is a good value here</param>
        /// <returns></returns>
        private IEnumerable<ProtocolIdentification.ProtocolModel> ReadProtocolModelsFromSessionFile(string filename, string protocolName, int maxFramesToParse, bool appendUnidirectionalSessions) {

            PcapFileHandler.PcapFileReader observationReader=new PcapFileHandler.PcapFileReader(filename, 4000, null);
            //PcapFileHandler.PcapFileReader observationReader=(PcapFileHandler.PcapFileReader)e.Argument;

            TcpSession bidirectionalTcpSession=new TcpSession(config, new ProtocolIdentification.ProtocolModel(protocolName, config.ActiveAttributeMeters));//bi-directional
            UdpSession bidirectionalUdpSession=new UdpSession(config, new ProtocolIdentification.ProtocolModel(protocolName, config.ActiveAttributeMeters));//bi-directional

            TcpSession unidirectionalClientToServerTcpSession=null;//uni-directional client-to-server
            TcpSession unidirectionalServerToClientTcpSession=null;//uni-directional server-to-client
            if(appendUnidirectionalSessions) {
                unidirectionalClientToServerTcpSession=new TcpSession(config, new ProtocolIdentification.ProtocolModel(protocolName+" (client->server)", config.ActiveAttributeMeters));
                unidirectionalServerToClientTcpSession=new TcpSession(config, new ProtocolIdentification.ProtocolModel(protocolName+" (server->client)", config.ActiveAttributeMeters));
            }

            //System.Diagnostics.Debug.Assert(bidirectionalTcpSession.ClientIP != null && bidirectionalTcpSession.ServerIP != null);
            //System.Diagnostics.Debug.Assert(bidirectionalUdpSession.ClientIP != null && bidirectionalUdpSession.ServerIP != null);


            int nFramesReceived=0;

            Type packetType=null;
            if(observationReader.FileDataLinkType==PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_ETHERNET)
                packetType=typeof(PacketParser.Packets.Ethernet2Packet);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_RAW_IP || observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_RAW_IP_2 || observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_RAW_IP_3)
                packetType=typeof(PacketParser.Packets.IPv4Packet);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_IEEE_802_11)
                packetType=typeof(PacketParser.Packets.IEEE_802_11Packet);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_IEEE_802_11_WLAN_RADIOTAP)
                packetType=typeof(PacketParser.Packets.IEEE_802_11RadiotapPacket);
            else
                throw new Exception("No packet type found for "+observationReader.FileDataLinkType.ToString());

            int framesCount=0;
            foreach(PcapFileHandler.PcapPacket packet in observationReader.PacketEnumerator(new PcapFileHandler.PcapFileReader.EmptyDelegate(Application.DoEvents), null)) {
                framesCount++;
                if(framesCount>maxFramesToParse) {
                    observationReader.AbortFileRead();
                    break;
                }
                else {
                    PacketParser.Frame frame=new PacketParser.Frame(packet.Timestamp, packet.Data, packetType, ++nFramesReceived, false);
                    try {
                        ProtocolIdentification.AttributeFingerprintHandler.PacketDirection tcpDirection=bidirectionalTcpSession.AddFrame(frame);
                        if(tcpDirection==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown) {
                            //No TCP data found, try UDP
                            bidirectionalUdpSession.AddFrame(frame);
                        }
                        else if(appendUnidirectionalSessions) {
                            if(unidirectionalClientToServerTcpSession.ClientIP==null || unidirectionalClientToServerTcpSession.ServerIP==null) {
                                unidirectionalClientToServerTcpSession.ClientIP=bidirectionalTcpSession.ClientIP;
                                unidirectionalClientToServerTcpSession.ClientPort=bidirectionalTcpSession.ClientPort;
                                unidirectionalClientToServerTcpSession.ServerIP=bidirectionalTcpSession.ServerIP;
                                unidirectionalClientToServerTcpSession.ServerPort=bidirectionalTcpSession.ServerPort;
                            }
                            if(unidirectionalServerToClientTcpSession.ClientIP==null || unidirectionalServerToClientTcpSession.ServerIP==null) {
                                unidirectionalServerToClientTcpSession.ClientIP=bidirectionalTcpSession.ClientIP;
                                unidirectionalServerToClientTcpSession.ClientPort=bidirectionalTcpSession.ClientPort;
                                unidirectionalServerToClientTcpSession.ServerIP=bidirectionalTcpSession.ServerIP;
                                unidirectionalServerToClientTcpSession.ServerPort=bidirectionalTcpSession.ServerPort;
                            }
                            if(tcpDirection==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer)
                                unidirectionalClientToServerTcpSession.AddFrame(frame);
                            else if(tcpDirection==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient)
                                unidirectionalServerToClientTcpSession.AddFrame(frame);
                        }
                    }
                    catch(Exception e) {
                        MessageBox.Show("Unable to parse training data file: "+filename+" (protocol "+protocolName+")\n\nDetails: "+e.Message);
                        //rethrow the exception
                        throw e;
                    }
                    //Application.DoEvents();//needed in order to allow the background worker to complete
                }
            }

            if(bidirectionalTcpSession.ApplicationProtocolModel.ObservationCount>0)
                yield return bidirectionalTcpSession.ApplicationProtocolModel;
            if(bidirectionalUdpSession.ApplicationProtocolModel.ObservationCount>0)
                yield return bidirectionalUdpSession.ApplicationProtocolModel;
            if(appendUnidirectionalSessions) {
                if(unidirectionalClientToServerTcpSession.ApplicationProtocolModel.ObservationCount>0)
                    yield return unidirectionalClientToServerTcpSession.ApplicationProtocolModel;
                if(unidirectionalServerToClientTcpSession.ApplicationProtocolModel.ObservationCount>0)
                    yield return unidirectionalServerToClientTcpSession.ApplicationProtocolModel;
            }
        }

        private ListViewItem GetProtocolModelListViewItem(ProtocolIdentification.ProtocolModel protocolModel) {
            ListViewItem item=new ListViewItem(protocolModel.ProtocolName);
            item.SubItems.Add(protocolModel.TrainingSessionCount.ToString());
            item.SubItems.Add(protocolModel.ObservationCount.ToString());
            return item;
        }

        //opens a pcap with (several) TCP sessions to be classified
        private void pcapFileToolStripMenuItem_Click(object sender, EventArgs e) {
            
            if(this.openPcapFileDialog.ShowDialog()==DialogResult.OK) {
                OpenSessionPcapFile(this.openPcapFileDialog.FileName, new System.IO.FileInfo(Application.ExecutablePath));
                
            }
        }

        //opens a pcap with (several) TCP/UDP sessions to be classified
        private void OpenSessionPcapFile(string filePathAndName, System.IO.FileInfo outputDirectory){
            this.sessionsListView.Items.Clear();
            this.sessionProtocolDetailsListView.Items.Clear();

            //create a file to write to
            //DateTime format according to http://www.geekzilla.co.uk/View00FF7904-B510-468C-A2C8-F859AA20581F.htm
            string timestampString = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'_'mm'_'ss_fffffff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            string outputFileName=outputDirectory.DirectoryName+"\\SPID_"+timestampString+".txt";
            if(System.IO.File.Exists(outputFileName)) {
                int n=1;
                while(System.IO.File.Exists(outputFileName+"."+n+".txt"))
                    n++;
                outputFileName = outputFileName+"."+n+".txt";
            }
            System.IO.StreamWriter writer=new System.IO.StreamWriter(outputFileName, false);

            StringBuilder fHeader = new StringBuilder();

            fHeader.Append("# "+this.Text+"\r\n");//to get the version number
            fHeader.Append("# Created by Erik Hjelmvik - erik.hjelmvik at gmail.com\r\n");
            fHeader.Append("# http://sourceforge.net/projects/spid \r\n");
            fHeader.Append("# \r\n");
            fHeader.Append("# Parsing file: "+filePathAndName+"\r\n");
            DateTime startTime=System.DateTime.Now;
            fHeader.Append("# SPID start: "+config.FormatDateTime(startTime)+"\r\n");

            StringBuilder fSessions = new StringBuilder();
            fSessions.Append("# [clientIp]\t[clientPort]\t[serverIp]\t[serverPort]\t[sessionStartTime]\t[inspectedFramesWithPayload]\t[identifiedProtocol]\r\n");
            

            SessionAndProtocolModelExtractor extractor=new SessionAndProtocolModelExtractor(filePathAndName, this.protocolModels, this.config);
            //just display the 200 first sessions
            int maxSessions=200;
            int sessionsCount=0;

            SortedDictionary<string, int> protocolCounters=new SortedDictionary<string, int>();

            foreach(ListViewItem item in extractor.GetSessionListViewItems()) {
                if(sessionsCount<maxSessions)
                    this.sessionsListView.Items.Add(item);
                //now this is a bit ugly, but I'm using the sessionListView as
                //a container for the data I want to output
                StringBuilder sb=new StringBuilder();

                foreach(ListViewItem.ListViewSubItem subItem in item.SubItems)
                    if(subItem!=null && subItem.Text!=null) {
                        if(sb.Length>0)
                            sb.Append("\t");
                        sb.Append(subItem.Text);
                    }
                fSessions.Append(sb.ToString()+"\r\n");

                sessionsCount++;

                //find the identified protocol
                if(item.SubItems.ContainsKey("protocol")) {
                    string protocol=item.SubItems["protocol"].Text;
                    if(protocolCounters.ContainsKey(protocol))
                        protocolCounters[protocol]++;
                    else
                        protocolCounters.Add(protocol, 1);
                }
            }
            DateTime endTime=System.DateTime.Now;
            fHeader.Append("# SPID stop: "+config.FormatDateTime(endTime)+"\r\n");
            fHeader.Append("# Total time: "+endTime.Subtract(startTime).ToString()+"\r\n");
            fHeader.Append("# \r\n");
            fHeader.Append("# Total number of sessions: "+sessionsCount.ToString(System.Globalization.CultureInfo.InvariantCulture)+"\r\n");
            fHeader.Append("# \r\n");
            fHeader.Append("# Identified protocols summary:\r\n");
            fHeader.Append("# [protocolName]\t[numberOfSessions]\r\n");
            int identifiedSessions = 0;
            foreach(string protocol in protocolCounters.Keys) {
                fHeader.Append("# "+protocol+"\t"+protocolCounters[protocol].ToString()+"\r\n");
                identifiedSessions+=protocolCounters[protocol];
            }
            if(identifiedSessions<sessionsCount)
                fHeader.Append("# UNKNOWN\t"+(sessionsCount-identifiedSessions)+"\r\n");

            StringBuilder fProtocolModels = new StringBuilder();
            fProtocolModels.Append("# Protocol models:\r\n");
            fProtocolModels.Append("# [protocolName]\t[trainingSessionsCount]\t[observationCount]\r\n");
            foreach(ProtocolIdentification.ProtocolModel model in this.protocolModels.Values) {
                fProtocolModels.Append("# "+model.ProtocolName+"\t"+model.TrainingSessionCount+"\t"+model.ObservationCount+"\r\n");
            }

            writer.Write(fHeader.ToString());
            writer.Write("# \r\n");
            writer.Write("# \r\n");
            writer.Write("# Configuration:\r\n");
            writer.Write(this.config.ToString());
            writer.Write("# \r\n");
            writer.Write("# \r\n");
            writer.Write(fProtocolModels.ToString());
            writer.Write("# \r\n");
            writer.Write("# \r\n");
            writer.Write(fSessions.ToString());
            writer.Close();

            /**
             * Ändra ordningen så att listan med sessioner ligger sist - OK
             * Visa vilken protokolldatabas som används - TODO
             * Visa konfigurationen (inklusive vilka attribute meters som används) - OK
             * 
             * */

            if(this.config.DisplayLogFile)
                System.Diagnostics.Process.Start(outputFileName);
        }

        

        

        private void ShowProtocolModelDivergences(ProtocolIdentification.ProtocolModel observationModel, ProtocolIdentification.ProtocolModel bestProtocolMatch) {
            //här kommer lite debug-kod:
            //jag måste dela på både protokoll och attributeMeter

            double[,] matrix=new double[this.protocolModels.Count, observationModel.AttributeFingerprintHandlers.Count];
            //double[][] jaggedArray=new double[this.protocolModels.Count][];

            for(int i=0; i<this.protocolModels.Values.Count; i++) {
                //jaggedArray[i]=new double[observationModel.AttributeFingerprintHandlers.Count];
                for(int j=0; j<this.protocolModels.Values[i].AttributeFingerprintHandlers.Values.Count; j++) {
                    ProtocolIdentification.AttributeFingerprintHandler observationAttributeModel=observationModel.AttributeFingerprintHandlers[this.protocolModels.Values[i].AttributeFingerprintHandlers.Values[j].AttributeMeterName];
                    matrix[i, j]=observationAttributeModel.GetKullbackLeiblerDivergenceFrom(this.protocolModels.Values[i].AttributeFingerprintHandlers.Values[j]);
                }
            }

            System.Data.DataTable dt=new DataTable("test");
            foreach(ProtocolIdentification.ProtocolModel m in this.protocolModels.Values)
                dt.Columns.Add(m.ToString());
            for(int j=0; j<observationModel.AttributeFingerprintHandlers.Values.Count; j++) {
                System.Data.DataRow dr=dt.NewRow();
                for(int i=0; i<dt.Columns.Count; i++)
                    dr[i]=matrix[i, j];
                dt.Rows.Add(dr);
            }
            new AttributeMeterTable(dt).Show();
            
            //now compute the best 4-attributeMeter-combination
            //the column "i" with the mest protocol match is....
            int bestProtocolMatchIndex=this.protocolModels.IndexOfValue(bestProtocolMatch);
            if(bestProtocolMatchIndex>=0) {//if there is a best match
                StringBuilder sb=new StringBuilder();
                foreach(int modelIndex in GetBestAttributeMeterIndexes(matrix, observationModel, bestProtocolMatchIndex, 3)) {
                    sb.Append("["+modelIndex+"]");
                    sb.Append(observationModel.AttributeFingerprintHandlers.Values[modelIndex].AttributeMeterName+"\n");
                    //sb.Append(" Percentage: "+bestAttributeMeterPercentage.ToString("00.000")+"%");
                }
                MessageBox.Show("Best attribute meters:\n"+sb.ToString());
            }
        }

        private int[] GetBestAttributeMeterIndexes(double[,] matrix, ProtocolIdentification.ProtocolModel observationModel, int bestProtocolMatchIndex, int nAttributeMeters) {
            int totalAttributeMetersCount=observationModel.AttributeFingerprintHandlers.Values.Count;
            int[] bestAttributeMeterIndexes=new int[nAttributeMeters];
            //int bestAttributeMeterIndex=-1;//not a valid number
            double bestAttributeMeterCombinationPercentage=0.0;

            
            //I will use modulus to cover all alternatives
            //this method will have exponential complexity
            //List<int> usedAttributerMeterIndices=new List<int>(4);
            for(int j=0; j<Math.Pow(totalAttributeMetersCount, nAttributeMeters); j++) {

                //double combinationProtocolTotalStrength=0.0;
                //double combinationCorrectProtocolStrength=0.0;
                
                double[] combinationAttributeModelDivergences=new double[this.protocolModels.Values.Count];
                
                //a quick loop to avoid extra work
                bool attributeComboIsNew=true;
                int largestAttributeMeterIndex=-1;
                for(int m=0; m<nAttributeMeters; m++) {
                    int attributeMeterIndex=(int)(j/Math.Pow(totalAttributeMetersCount, m))%totalAttributeMetersCount;
                    if(attributeMeterIndex<=largestAttributeMeterIndex) {
                        attributeComboIsNew=false;
                        break;
                    }
                    else
                        largestAttributeMeterIndex=attributeMeterIndex;
                    
                    //int divisor=Math.Pow(totalAttributeMetersCount, m);
                    for(int i=0; i<this.protocolModels.Values.Count; i++) {
                        combinationAttributeModelDivergences[i]+=matrix[i, attributeMeterIndex];
                        //protocolTotalStrength+=Math.Pow(2.0, -matrix[i, j]);//the strength of this protocol
                    }
                }
                if(attributeComboIsNew) {
                    //see if the value is better
                    double totalStrength=0.0;
                    for(int i=0; i<this.protocolModels.Values.Count; i++) {
                        totalStrength+=Math.Pow(2.0, -combinationAttributeModelDivergences[i]);
                    }
                    double strengthPercentage=100.0*Math.Pow(2.0, -combinationAttributeModelDivergences[bestProtocolMatchIndex])/totalStrength;
                    if(strengthPercentage>bestAttributeMeterCombinationPercentage) {
                        bestAttributeMeterCombinationPercentage=strengthPercentage;
                        for(int m=0; m<nAttributeMeters; m++) {
                            int attributeMeterIndex=(int)(j/Math.Pow(totalAttributeMetersCount, m))%totalAttributeMetersCount;
                            bestAttributeMeterIndexes[m]=attributeMeterIndex;
                        }
                    }
                }

            }
            return bestAttributeMeterIndexes;
        }

        private void SaveProtocolModelDatabase(string fileName) {
            System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            xmlDoc.LoadXml("<protocolModels/>");
            xmlDoc.DocumentElement.SetAttribute("fingerprintLength", ProtocolIdentification.AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH.ToString());
            foreach(ProtocolIdentification.ProtocolModel model in this.protocolModels.Values) {
                xmlDoc.DocumentElement.AppendChild(model.GetXml(xmlDoc));
            }
            xmlDoc.Save(fileName);
        }

        private void sessionsListView_SelectedIndexChanged(object sender, EventArgs e) {
            if(this.sessionsListView.SelectedItems.Count>0 && this.sessionsListView.SelectedItems[0].Tag!=null) {
                
                SortedList<string, double> protocolModelDivergences = (SortedList<string, double>)this.sessionsListView.SelectedItems[0].Tag;
                double totalWeight=0.0;
                //I need to know how many attribute meters there are
                //in order to convert from average values to total divergences
                //there is nothing scientific about doing SQRT here, I just need
                //to compensate for the fact that many attributeMeters aren't
                //indipendent. They would be orthogonal in the idea world though
                int nAttributeMeters=(int)Math.Pow(this.protocolModels.Values[0].AttributeFingerprintHandlers.Count, 0.5);
                foreach(double d in protocolModelDivergences.Values)
                    totalWeight+=Math.Pow(2, -nAttributeMeters*d);
                this.sessionProtocolDetailsListView.Items.Clear();
                foreach(KeyValuePair<string, double> nameDivergence in protocolModelDivergences) {
                    ListViewItem item=this.sessionProtocolDetailsListView.Items.Add(nameDivergence.Key);
                    item.SubItems.Add(nameDivergence.Value.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));
                    //calculate the percentage for this protocol compared to the others
                    double percent=100.0*Math.Pow(2, -nAttributeMeters*nameDivergence.Value)/totalWeight;
                    StringBuilder starBuilder=new StringBuilder();
                    for(int i=0; i<percent; i+=2)
                        starBuilder.Append('*');
                    item.SubItems.Add(percent.ToString("00.00", System.Globalization.CultureInfo.InvariantCulture)+"% "+starBuilder.ToString());
                }


            }
        }

        private void saveProtocolModelDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this.saveProtocolModelDatabaseFileDialog.ShowDialog()==DialogResult.OK) {
                this.SaveProtocolModelDatabase(this.saveProtocolModelDatabaseFileDialog.FileName);
            }
        }



        private void protocolModelDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            //open new protocol model database
            if(this.openProtocolModelDatabaseFileDialog.ShowDialog()==DialogResult.OK) {
                openProtocolModelDatabaseFile(this.openProtocolModelDatabaseFileDialog.FileName, false);
            }
        }


        private void protocolModelDatabaseToolStripMenuItem1_Click(object sender, EventArgs e) {
            //appends a protocol model database to the existing database
            if(this.openProtocolModelDatabaseFileDialog.ShowDialog()==DialogResult.OK) {
                openProtocolModelDatabaseFile(this.openProtocolModelDatabaseFileDialog.FileName, true);
            }
        }

        private void openProtocolModelDatabaseFile(string filename, bool appendToExistingDatabase) {
            if(!appendToExistingDatabase)
                this.protocolModels.Clear();
            this.protocolModelsListView.Items.Clear();
            foreach(ProtocolIdentification.ProtocolModel model in this.GetProtocolModelsFromDatabaseFile(filename)) {
                if(this.protocolModels.ContainsKey(model.ProtocolName))
                    this.protocolModels[model.ProtocolName]=this.protocolModels[model.ProtocolName].MergeWith(model);
                else
                    this.protocolModels.Add(model.ProtocolName, model);
            }
            foreach(ProtocolIdentification.ProtocolModel model in this.protocolModels.Values) {
                this.protocolModelsListView.Items.Add(GetProtocolModelListViewItem(model));
            }
        }

        private IEnumerable<ProtocolIdentification.ProtocolModel> GetProtocolModelsFromDatabaseFile(string filename) {
            System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            xmlDoc.Load(filename);
            if(xmlDoc.DocumentElement.SelectSingleNode("/protocolModels").Attributes["fingerprintLength"].Value!=ProtocolIdentification.AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH.ToString())
                throw new Exception("Fingerprint length is not correct!");
            foreach(System.Xml.XmlNode protocolModelNode in xmlDoc.DocumentElement.SelectNodes("/protocolModels/protocolModel")) {
                string protocolName=protocolModelNode.SelectSingleNode("@name").Value;
                int sessionCount=Int32.Parse(protocolModelNode.SelectSingleNode("@sessionCount").Value);
                ulong observationCount=UInt64.Parse(protocolModelNode.SelectSingleNode("@observationCount").Value);
                //ProtocolIdentification.ProtocolModel model=new ProtocolIdentification.ProtocolModel(name);
                SortedList<string, ProtocolIdentification.AttributeFingerprintHandler> attributeFingerprintHandlers=new SortedList<string, ProtocolIdentification.AttributeFingerprintHandler>();
                List<ushort> defaultPorts=new List<ushort>();
                foreach(System.Xml.XmlNode defaultPortNode in protocolModelNode.SelectNodes("defaultPorts/port")) {
                    defaultPorts.Add(UInt16.Parse(defaultPortNode.InnerText));
                }

                foreach(System.Xml.XmlNode attributeFingerprintNode in protocolModelNode.SelectNodes("attributeFingerprint")) {
                    string attributeMeterName=attributeFingerprintNode.SelectSingleNode("@attributeMeterName").Value;
                    ulong measurementCount=UInt64.Parse(attributeFingerprintNode.SelectSingleNode("@measurementCount").Value);

                    System.Xml.XmlNodeList probabilityNodeList=attributeFingerprintNode.SelectNodes("bin");
                    double[] probabilityDistributionVector=new double[probabilityNodeList.Count];

                    for(int i=0; i<probabilityDistributionVector.Length; i++)
                        probabilityDistributionVector[i]=Double.Parse(probabilityNodeList[i].InnerText, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    //.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                    attributeFingerprintHandlers.Add(attributeMeterName, new ProtocolIdentification.AttributeFingerprintHandler(attributeMeterName, probabilityDistributionVector, measurementCount));

                }

                yield return new ProtocolIdentification.ProtocolModel(protocolName, attributeFingerprintHandlers, sessionCount, observationCount, defaultPorts, config.ActiveAttributeMeters);

            }
        }

        private void aboutSPIDAlgorithmProofofConceptToolStripMenuItem_Click(object sender, EventArgs e) {
            new AboutBox1().ShowDialog();
        }

        private void classifiedSessionsGroupBox_DragEnter(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect=DragDropEffects.Copy;
            else
                e.Effect=DragDropEffects.None;
        }

        private void classifiedSessionsGroupBox_DragDrop(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] filenames=(string[])e.Data.GetData(DataFormats.FileDrop);
                foreach(string fileNameAndLocation in filenames) {
                    OpenSessionPcapFile(fileNameAndLocation, new System.IO.FileInfo(Application.ExecutablePath));
                }
            }
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e) {
            new ConfigurationForm(config).ShowDialog();
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("LICENSE.txt");
        }




        
    }
}