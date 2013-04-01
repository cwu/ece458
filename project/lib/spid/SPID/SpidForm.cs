//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SPID {

    public partial class SpidForm : Form {

        private SortedList<string, ProtocolIdentification.ProtocolModel> protocolModels;
        private ProtocolIdentification.ProtocolModel sessionModel;


        public SpidForm() {
            InitializeComponent();
            this.protocolModels=new SortedList<string, ProtocolIdentification.ProtocolModel>();
            this.sessionModel=null;
        }

        private void existingProtocolComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.existingProtocolRadioButton.Select();
        }

        private void newProtocolTextBox_TextChanged(object sender, EventArgs e) {
            this.newProtocolRadioButton.Select();
            this.newProtocolTextBox.Focus();
        }

        private void newProtocolTextBox_Enter(object sender, EventArgs e) {
            if(this.newProtocolTextBox.Text=="[new protocol name]")
                this.newProtocolTextBox.Text="";
            this.newProtocolTextBox.Enabled=true;
        }

        private void existingProtocolRadioButton_CheckedChanged(object sender, EventArgs e) {
            if(this.sessionModel!=null)
                this.addSessionToProtocolModelsButton.Enabled=true;
        }

        private void newProtocolRadioButton_CheckedChanged(object sender, EventArgs e) {
            if(this.sessionModel!=null)
                this.addSessionToProtocolModelsButton.Enabled=true;
        }

        private void openPcapFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this.openPcapFileDialog.ShowDialog()==DialogResult.OK) {
                //this.LoadPcapFile(this.openFileDialog1.FileName);
                this.existingProtocolComboBox.SelectedItem=null;

                try {
                    this.sessionModel=ReadProtocolModelFromSessionFile(this.openPcapFileDialog.FileName, 100);
                    if(sessionModel!=null && sessionModel.ObservationCount>0 && sessionModel.TrainingSessionCount>0) {
                        this.addToModelsGroupBox.Enabled=true;
                        this.newProtocolTextBox.Text="[new protocol name]";
                    }

                    this.protocolIdentificationListView.Items.Clear();
                    //protocolModel=protocolModel.MergeWith(sessionModel);
                    string bestProtocolMatch=null;
                    double bestProtocolMatchDivergence=2.4;
                    foreach(ProtocolIdentification.ProtocolModel protocolModel in this.protocolModels.Values) {
                        ListViewItem item=this.protocolIdentificationListView.Items.Add(protocolModel.ProtocolName);
                        double divergence=sessionModel.GetAverageKullbackLeiblerDivergenceFrom(protocolModel);
                        if(divergence<bestProtocolMatchDivergence) {
                            bestProtocolMatch=protocolModel.ProtocolName;
                            bestProtocolMatchDivergence=divergence;
                        }

                        item.SubItems.Add(divergence.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));

                        StringBuilder matchStrength=new StringBuilder();
                        for(int i=0; i<Math.Pow(2, -divergence)*20; i++)
                            matchStrength.Append('*');
                        item.SubItems.Add(matchStrength.ToString());
                        

                    }
                    if(bestProtocolMatch!=null && this.existingProtocolComboBox.Items.Contains(bestProtocolMatch))
                        this.existingProtocolComboBox.SelectedItem=bestProtocolMatch;
                        
                }
                catch(Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private IEnumerable<ProtocolIdentification.ProtocolModel> GetProtocolModelsFromDatabaseFile(string filename) {
            System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            xmlDoc.Load(filename);
            if(xmlDoc.DocumentElement.SelectSingleNode("/protocolModels").Attributes["fingerprintLength"].Value!=ProtocolIdentification.AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH.ToString())
                throw new Exception("Fingerprint length is not correct!");
            foreach(System.Xml.XmlNode protocolModelNode in xmlDoc.DocumentElement.SelectNodes("/protocolModels/protocolModel")){
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

                yield return new ProtocolIdentification.ProtocolModel(protocolName, attributeFingerprintHandlers, sessionCount, observationCount, defaultPorts);

            }
        }

        /// <summary>
        /// This function ONLY works when the pcap file only contains one TCP sessions
        /// i.e. "Follow TCP stream" has been used in Wireshark
        /// </summary>
        /// <param name="filename">The pcap file to open</param>
        /// <param name="maxFramesToParse">100 is a good value here</param>
        /// <returns></returns>
        private ProtocolIdentification.ProtocolModel ReadProtocolModelFromSessionFile(string filename, int maxFramesToParse) {

            PcapFileHandler.PcapFileReader observationReader=new PcapFileHandler.PcapFileReader(filename);
            ProtocolIdentification.ProtocolModel observationModel=new ProtocolIdentification.ProtocolModel(filename);
            //Console.WriteLine("Checking observation "+observationFilename);

            string serverToClientDirectionID=null;//sIP+dIP+sPort+dPort
            string clientToServerDirectionID=null;//sIP+dIP+sPort+dPort
            //byte tcpSynFlag=0x02;
            //byte tcpSynAckFlag=0x12;
            //int tcpFlagOffset=0x2f;

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
            foreach(PcapFileHandler.PcapPacket packet in observationReader.PacketEnumerator()) {
                PacketParser.Frame frame=new PacketParser.Frame(packet.Timestamp, packet.Data, packetType, ++nFramesReceived, false);
                framesCount++;
                if(framesCount>maxFramesToParse)
                    break;

                int ipStartIndex=-1;
                //ushort totalLengthFromIp=0;
                ushort ipPayloadLength=0;
                System.Net.IPAddress sourceIp=null;
                System.Net.IPAddress destinationIp=null;
                PacketParser.Packets.TcpPacket tcpPacket=null;

                foreach(PacketParser.Packets.AbstractPacket p in frame.PacketList) {

                    if(p.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                        ipStartIndex=p.PacketStartIndex;
                        PacketParser.Packets.IPv4Packet ipPacket=(PacketParser.Packets.IPv4Packet)p;
                        //totalLengthFromIp=ipPacket.TotalLength;
                        ipPayloadLength=(ushort)(ipPacket.TotalLength-ipPacket.HeaderLength);
                        sourceIp=ipPacket.SourceIPAddress;
                        destinationIp=ipPacket.DestinationIPAddress;
                    }
                    else if(p.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                        ipStartIndex=p.PacketStartIndex;
                        PacketParser.Packets.IPv6Packet ipPacket=(PacketParser.Packets.IPv6Packet)p;
                        //totalLengthFromIp=ipPacket.TotalLength;
                        ipPayloadLength=ipPacket.PayloadLength;
                        sourceIp=ipPacket.SourceIP;
                        destinationIp=ipPacket.DestinationIP;
                    }
                    else if(p.GetType()==typeof(PacketParser.Packets.TcpPacket)) {
                        tcpPacket=(PacketParser.Packets.TcpPacket)p;
                        //there is no point in enumarating further than the TCP packet
                        break;//quit the foreach loop
                    }
                }

                if(ipStartIndex>=0 && tcpPacket!=null && sourceIp!=null && destinationIp!=null) {

                    //set the direction upon the SYN+ACK packet
                    if(tcpPacket.FlagBits.Synchronize && tcpPacket.FlagBits.Acknowledgement && serverToClientDirectionID==null && clientToServerDirectionID==null) {
                        //set the session server and client ID
                        serverToClientDirectionID=GetDirectionID(sourceIp, tcpPacket.SourcePort, destinationIp, tcpPacket.DestinationPort);
                        clientToServerDirectionID=GetDirectionID(destinationIp, tcpPacket.DestinationPort, sourceIp, tcpPacket.SourcePort);
                    }

                    ProtocolIdentification.AttributeFingerprintHandler.PacketDirection direction= ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown;
                    string directionID=GetDirectionID(sourceIp, tcpPacket.SourcePort, destinationIp, tcpPacket.DestinationPort);
                    if(directionID==serverToClientDirectionID)
                        direction= ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient;
                    else if(directionID==clientToServerDirectionID)
                        direction= ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer;

                    int protocolPacketStartIndex=tcpPacket.PacketStartIndex+tcpPacket.DataOffsetByteCount;
                    int protocolPacketLength=ipPayloadLength-tcpPacket.DataOffsetByteCount;
                    observationModel.AddObservation(frame.Data, protocolPacketStartIndex, protocolPacketLength, frame.Timestamp, direction);
                }

                if(framesCount>5 && (serverToClientDirectionID==null || clientToServerDirectionID==null))
                    throw new Exception("The capture file must contain the TCP handshake (or at least a SYN+ACK packet) within the first 5 frames!");



            }

            return observationModel;
        }

        private string GetDirectionID(System.Net.IPAddress sourceIP, ushort sourcePort, System.Net.IPAddress destinationIP, ushort destinationPort) {
            return sourceIP.ToString()+":"+sourcePort.ToString()+"->"+destinationIP.ToString()+":"+destinationPort.ToString();
        }

        private void importProtocolModelToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this.openProtocolModelDatabase.ShowDialog()==DialogResult.OK) {
                foreach(ProtocolIdentification.ProtocolModel model in this.GetProtocolModelsFromDatabaseFile(this.openProtocolModelDatabase.FileName)){
                    if(this.protocolModels.ContainsKey(model.ProtocolName))
                        this.protocolModels[model.ProtocolName]=this.protocolModels[model.ProtocolName].MergeWith(model);
                    else
                        this.protocolModels.Add(model.ProtocolName, model);
                }
                this.UpdateProtocols();

            }
        }

        private void UpdateProtocols() {
            this.protocolModelsListView.Items.Clear();
            this.existingProtocolComboBox.Items.Clear();
            foreach(ProtocolIdentification.ProtocolModel model in this.protocolModels.Values) {
                //this.protocolModelTreeView.Nodes.Add(model.ProtocolName);
                ListViewItem item=this.protocolModelsListView.Items.Add(model.ProtocolName);
                item.SubItems.Add(model.TrainingSessionCount.ToString());
                item.SubItems.Add(model.ObservationCount.ToString());

                this.existingProtocolComboBox.Items.Add(model.ProtocolName);
            }
        }

        private void addSessionToProtocolModelsButton_Click(object sender, EventArgs e) {
            if(this.sessionModel==null)
                return;

            string sessionProtocolName;
            //detect the name!
            
            if(this.existingProtocolRadioButton.Checked)
                sessionProtocolName=this.existingProtocolComboBox.SelectedItem.ToString();
            else if(this.newProtocolRadioButton.Checked && this.newProtocolTextBox.Text.Length>0)
                sessionProtocolName=this.newProtocolTextBox.Text;
            else
                return;
            this.sessionModel.ProtocolName=sessionProtocolName;

            if(this.protocolModels.ContainsKey(sessionProtocolName))
                this.protocolModels[sessionProtocolName]=this.protocolModels[sessionProtocolName].MergeWith(this.sessionModel);
            else
                this.protocolModels.Add(sessionProtocolName, this.sessionModel);
            this.newProtocolTextBox.Text="";
            this.sessionModel=null;
            this.protocolIdentificationListView.Items.Clear();
            this.addSessionToProtocolModelsButton.Enabled=false;
            this.existingProtocolRadioButton.Checked=false;
            this.newProtocolRadioButton.Checked=false;
            this.addToModelsGroupBox.Enabled=false;


            this.UpdateProtocols();
        }

        private void button2_Click(object sender, EventArgs e) {

        }

        private void addNewPortButton_Click(object sender, EventArgs e) {
            if(!this.defaultPortsListBox.Items.Contains((int)this.newPortNumericUpDown.Value))
                this.defaultPortsListBox.Items.Add((int)this.newPortNumericUpDown.Value);
        }

        private void removeSelectedPortToolStripMenuItem_Click(object sender, EventArgs e) {
            this.defaultPortsListBox.Items.Remove(this.defaultPortsListBox.SelectedItem);
                
        }

        private void protocolModelsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            if(protocolModelsListView.SelectedItems.Count>0) {
                if(!this.protocolModelPropertiesGroupBox.Enabled)
                    this.protocolModelPropertiesGroupBox.Enabled=true;
                //load the data to the textboxes
                string protocolName=(string)this.protocolModelsListView.SelectedItems[0].Text;
                ProtocolIdentification.ProtocolModel protocolModel=this.protocolModels[protocolName];
                this.protocolNameTextBox.Text=protocolModel.ProtocolName;
                foreach(ushort port in protocolModel.DefaultPorts)
                    this.defaultPortsListBox.Items.Add(port);

            }
            else {
                
                //reset all data in the textboxes
                this.protocolNameTextBox.Text="";
                this.defaultPortsListBox.Items.Clear();
                this.protocolModelPropertiesGroupBox.Enabled=false;
                
            }
        }

        private void updateProtocolModelButton_Click(object sender, EventArgs e) {
            ProtocolIdentification.ProtocolModel protocolModel=this.protocolModels[(string)this.protocolModelsListView.SelectedItems[0].Text];
            //see if name is changed
            if(this.protocolNameTextBox.Text!=null && this.protocolNameTextBox.Text.Length>0 && this.protocolNameTextBox.Text!=protocolModel.ProtocolName) {
                this.protocolModels.Remove(protocolModel.ProtocolName);
                protocolModel.ProtocolName=this.protocolNameTextBox.Text;
                this.protocolModels.Add(protocolModel.ProtocolName, protocolModel);
            }
            List<ushort> ports=new List<ushort>();
            foreach(object oPort in this.defaultPortsListBox.Items)
                ports.Add(UInt16.Parse(oPort.ToString()));
            protocolModel.DefaultPorts=ports;

            this.protocolModelsListView.SelectedItems.Clear();

            this.UpdateProtocols();
        }

        private void resetAllProtocolModelFingerprintsButton_Click(object sender, EventArgs e) {
            foreach(ProtocolIdentification.ProtocolModel model in this.protocolModels.Values)
                model.Clear();

            this.UpdateProtocols();
        }

        private void removeProtocolModelToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach(ListViewItem item in this.protocolModelsListView.SelectedItems)
                this.protocolModels.Remove(item.Text);
            this.UpdateProtocols();
        }

        private void saveProtocolModelDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this.saveProtocolModelDatabaseDialog.ShowDialog()==DialogResult.OK) {
                System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
                xmlDoc.LoadXml("<protocolModels/>");
                xmlDoc.DocumentElement.SetAttribute("fingerprintLength", ProtocolIdentification.AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH.ToString());
                foreach(ProtocolIdentification.ProtocolModel model in protocolModels.Values) {
                    xmlDoc.DocumentElement.AppendChild(model.GetXml(xmlDoc));
                }
                xmlDoc.Save(this.saveProtocolModelDatabaseDialog.FileName);
            }
        }


    }
}