using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ProtocolIdentification {

    //This class holds info about one tcp session
    public class SessionEvidence {
        private SortedList<string, ClassificationAttributeFingerprint> classificationAttributeFingerprints;
        private string protocolEvidenceName;
        private bool isOpen;
        //private SortedList<AttributeFingerprint.PacketDirection, DateTime> lastPacketTimestamp;
        private ulong packetCount;

        public bool IsOpen { get { return isOpen; } }
        public ulong PacketCount { get { return this.packetCount; } set { this.packetCount=value; } }


        public SessionEvidence(string protocolEvidenceName){
            this.protocolEvidenceName=protocolEvidenceName;
            this.isOpen=true;

            this.packetCount=0;

            this.classificationAttributeFingerprints=new SortedList<string,ClassificationAttributeFingerprint>();
            AddAttributeFingerprint(new ClassificationAttributes.ActionReactionFirst3ByteHashAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.ByteFrequencyAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.BytePairsReocurringCountIn32FirstBytes());
            AddAttributeFingerprint(new ClassificationAttributes.BytePairsReocurringIn32FirstBytes());
            AddAttributeFingerprint(new ClassificationAttributes.BytePairsReocurringOffsetsIn32FirstBytes());
            AddAttributeFingerprint(new ClassificationAttributes.ByteValueOffsetHashOfFirst32BytesInFirst4Packets());
            AddAttributeFingerprint(new ClassificationAttributes.DirectionByteFrequencyAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.DirectionPacketLengthDistributionAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.First2OrderedFirst4CharWordsAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.First2OrderedFirstBitPositionsAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.First2OrderedPacketsFirstNByteNibbles());
            AddAttributeFingerprint(new ClassificationAttributes.First2PacketsFirst16ByteHashCounts());
            AddAttributeFingerprint(new ClassificationAttributes.First2PacketsFirst3ByteHashAndPacketLength());
            AddAttributeFingerprint(new ClassificationAttributes.First2PacketsFirst8ByteHashDirectionCounts());
            AddAttributeFingerprint(new ClassificationAttributes.First2PacketsPerDirectionFirst5BytesDifferences());
            AddAttributeFingerprint(new ClassificationAttributes.First4DirectionFirstNByteNibbles());
            AddAttributeFingerprint(new ClassificationAttributes.First4OrderedDirectionFirstNByteNibbles());
            AddAttributeFingerprint(new ClassificationAttributes.First4OrderedDirectionInterPacketDelay());
            AddAttributeFingerprint(new ClassificationAttributes.First4OrderedDirectionPacketSize());
            AddAttributeFingerprint(new ClassificationAttributes.First4PacketsByteReoccurringDistanceWithByteHash());
            AddAttributeFingerprint(new ClassificationAttributes.First4PacketsFirst16BytePairsAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.First4PacketsFirst32BytesEquality());
            AddAttributeFingerprint(new ClassificationAttributes.FirstBitPositionsAttribute());
            AddAttributeFingerprint(new ClassificationAttributes.FirstPacketPerDirectionFirstNByteNibbles());
            AddAttributeFingerprint(new ClassificationAttributes.FirstServerPacketFirstBitPositionsAttribute());

        }
        public SessionEvidence(string protocolEvidenceName, SortedList<string, ClassificationAttributeFingerprint> classificationAttributeFingerprints)
            : this(protocolEvidenceName) {
            this.classificationAttributeFingerprints=classificationAttributeFingerprints;
        }



        private void AddAttributeFingerprint(ClassificationAttributes.IClassificationAttribute attribute){
            this.classificationAttributeFingerprints.Add(attribute.AttributeName, new ClassificationAttributeFingerprint(attribute));
        }

        public void Close() {
            this.isOpen=false;
        }


        public void AddObservation(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection) {
            if(packetLength>0) {//only analyze non-zero size packets since only they say something about the packet in the payload
                if(isOpen) {
                    
                    //TimeSpan interPacketDelay=packetTimestamp.Subtract(this.lastPacketTimestamp[packetDirection]);

                    foreach(ClassificationAttributeFingerprint f in classificationAttributeFingerprints.Values)
                        f.AddObservation(frameData, packetStartIndex, packetLength, packetTimestamp, packetDirection, (int)packetCount);

                    //this.lastPacketTimestamp[packetDirection]=packetTimestamp;
                    this.packetCount++;
                }
                else {
                    throw new Exception("Observations cannot be added to closed session evidences. Use MergeWith() function instead.");
                }
            }
        }

        public SessionEvidence MergeWith(SessionEvidence otherSessionEvidence) {

            SortedList<string, ClassificationAttributeFingerprint> mergedFingerprints=new SortedList<string, ClassificationAttributeFingerprint>(classificationAttributeFingerprints.Count);
            foreach(string fingerprintAttributeName in this.classificationAttributeFingerprints.Keys) {
                mergedFingerprints.Add(fingerprintAttributeName, this.classificationAttributeFingerprints[fingerprintAttributeName].MergeWith(otherSessionEvidence.classificationAttributeFingerprints[fingerprintAttributeName]));
            }
            SessionEvidence returnSessionEvidence=new SessionEvidence(this.protocolEvidenceName, mergedFingerprints);
            returnSessionEvidence.Close();

            returnSessionEvidence.PacketCount=this.PacketCount+otherSessionEvidence.PacketCount;

            return returnSessionEvidence;

        }

        public ClassificationAttributeFingerprint GetAttributeFingerprint(string protocolAttributeName) {
            if(this.classificationAttributeFingerprints.ContainsKey(protocolAttributeName))
                return this.classificationAttributeFingerprints[protocolAttributeName];
            else
                return ClassificationAttributeFingerprint.EmptySingletonInstance;
        }

        public double GetAverageKullbackLeiblerDivergenceFrom(SessionEvidence protocolModelEvidence) {
            double totalDivergence=0.0;
            foreach(ClassificationAttributeFingerprint f in classificationAttributeFingerprints.Values) {
                double attributeDivergence=f.GetKullbackLeiblerDivergenceFrom(protocolModelEvidence.GetAttributeFingerprint(f.ClassificationAttrubute.AttributeName));
                totalDivergence+=attributeDivergence;
                /*
                if(f.ProtocolAttrubute.AttributeName.Length>40)
                    Console.WriteLine("\t"+f.ProtocolAttrubute.AttributeName.Substring(0,40)+" : "+attributeDivergence);
                else
                    Console.WriteLine("\t"+f.ProtocolAttrubute.AttributeName+" : "+attributeDivergence);
                 * */
            }
            return totalDivergence/classificationAttributeFingerprints.Count;
        }

        public override string ToString() {
            return this.protocolEvidenceName;
        }

        public System.Xml.XmlElement GetXml(XmlDocument xmlDoc) {
            //XmlDocument xmlDoc=new XmlDocument();
            XmlElement evidenceElement=xmlDoc.CreateElement("sessionEvidence");
            evidenceElement.SetAttribute("name", this.protocolEvidenceName);
            evidenceElement.SetAttribute("packetCount", this.packetCount.ToString());
            foreach(ClassificationAttributeFingerprint f in this.classificationAttributeFingerprints.Values) {
                evidenceElement.AppendChild(f.GetXml(xmlDoc));
            }
            return evidenceElement;

        }
    }
}
