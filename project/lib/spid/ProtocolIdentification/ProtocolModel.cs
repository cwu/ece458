//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ProtocolIdentification {

    //This class holds info about one tcp session
    public class ProtocolModel {
        private SortedList<string, AttributeFingerprintHandler> attributeFingerprintHandlers;
        private string protocolName;
        private int trainingSessionCount;
        private bool sessionIsOpen;
        //private SortedList<AttributeFingerprint.PacketDirection, DateTime> lastPacketTimestamp;
        private ulong observationCount;
        private List<ushort> defaultPorts;

        public string ProtocolName { get { return this.protocolName; } set { this.protocolName=value; } }
        public SortedList<string, AttributeFingerprintHandler> AttributeFingerprintHandlers { get { return this.attributeFingerprintHandlers; } }
        public bool SessionIsOpen { get { return sessionIsOpen; } }
        public int TrainingSessionCount { get { return this.trainingSessionCount; } }
        public ulong ObservationCount { get { return this.observationCount; } set { this.observationCount=value; } }
        public List<ushort> DefaultPorts { get { return this.defaultPorts; } set { this.defaultPorts=value; } }

        public ProtocolModel(string protocolName)
            : this(protocolName, null) {
            //TrimAttributeFingerprintHandlers(activeAttributeMeters);
        }
        public ProtocolModel(string protocolName, ICollection<string> activeAttributeMeterNames) {
            this.protocolName=protocolName;
            this.trainingSessionCount=0;//we are starting a new model from scratch
            this.sessionIsOpen=true;

            this.observationCount=0;
            this.defaultPorts=new List<ushort>();

            List<AttributeMeters.IAttributeMeter> attributeMeters=new List<ProtocolIdentification.AttributeMeters.IAttributeMeter>();
            attributeMeters.Add(new AttributeMeters.AccumulatedDirectionBytesMeter());//new attributeMeter from v0.2
            attributeMeters.Add(new AttributeMeters.ActionReactionFirst3ByteHashMeter());
            attributeMeters.Add(new AttributeMeters.ByteBitValueMeter());//new attributeMeter from v0.2
            attributeMeters.Add(new AttributeMeters.ByteFrequencyMeter());
            attributeMeters.Add(new AttributeMeters.ByteFrequencyOfFirstPacketBytesMeter());
            attributeMeters.Add(new AttributeMeters.BytePairsReocurringCountIn32FirstBytesMeter());
            attributeMeters.Add(new AttributeMeters.BytePairsReocurringIn32FirstBytesMeter());
            attributeMeters.Add(new AttributeMeters.BytePairsReocurringOffsetsIn32FirstBytesMeter());
            attributeMeters.Add(new AttributeMeters.ByteValueOffsetHashOfFirst32BytesInFirst4PacketsMeter());
            attributeMeters.Add(new AttributeMeters.DirectionByteFrequencyMeter());
            attributeMeters.Add(new AttributeMeters.DirectionPacketLengthDistributionMeter());
            attributeMeters.Add(new AttributeMeters.DirectionPacketSizeChange());
            attributeMeters.Add(new AttributeMeters.First2OrderedFirst4CharWordsMeter());
            attributeMeters.Add(new AttributeMeters.First2OrderedFirstBitPositionsMeter());
            attributeMeters.Add(new AttributeMeters.First2OrderedPacketsFirstNByteNibblesMeter());
            attributeMeters.Add(new AttributeMeters.First2PacketsFirst16ByteHashCountsMeter());
            attributeMeters.Add(new AttributeMeters.First2PacketsFirst3ByteHashAndPacketLengthMeter());
            attributeMeters.Add(new AttributeMeters.First2PacketsFirst8ByteHashDirectionCountsMeter());
            attributeMeters.Add(new AttributeMeters.First2PacketsPerDirectionFirst5BytesDifferencesMeter());
            attributeMeters.Add(new AttributeMeters.First4DirectionFirstNByteNibblesMeter());
            attributeMeters.Add(new AttributeMeters.First4OrderedDirectionFirstNByteNibblesMeter());
            attributeMeters.Add(new AttributeMeters.First4OrderedDirectionInterPacketDelayMeter());
            attributeMeters.Add(new AttributeMeters.First4OrderedDirectionPacketSizeMeter());
            attributeMeters.Add(new AttributeMeters.First4PacketsByteFrequencyMeter());
            attributeMeters.Add(new AttributeMeters.First4PacketsByteReoccurringDistanceWithByteHashMeter());
            attributeMeters.Add(new AttributeMeters.First4PacketsFirst16BytePairsMeter());
            attributeMeters.Add(new AttributeMeters.First4PacketsFirst32BytesEqualityMeter());
            attributeMeters.Add(new AttributeMeters.FirstBitPositionsMeter());
            attributeMeters.Add(new AttributeMeters.FirstPacketPerDirectionFirstNByteNibblesMeter());
            attributeMeters.Add(new AttributeMeters.FirstServerPacketFirstBitPositionsMeter());
            attributeMeters.Add(new AttributeMeters.NibblePositionFrequencyMeter());
            attributeMeters.Add(new AttributeMeters.NibblePositionPopularityMeter());
            attributeMeters.Add(new AttributeMeters.PacketLengthDistributionMeter());
            attributeMeters.Add(new AttributeMeters.PacketPairLengthPrimesMeter());

            this.attributeFingerprintHandlers=new SortedList<string, AttributeFingerprintHandler>();
            foreach(AttributeMeters.IAttributeMeter am in attributeMeters){
                if(activeAttributeMeterNames==null || activeAttributeMeterNames.Contains(am.AttributeName)) {
                    this.attributeFingerprintHandlers.Add(am.AttributeName, new AttributeFingerprintHandler(am));
                }
            }

            //run the trim function just to be extra sure ;)
            if(activeAttributeMeterNames!=null)
                TrimAttributeFingerprintHandlers(activeAttributeMeterNames);

            /*
            TryAddAttributeMeter(new AttributeMeters.AccumulatedDirectionBytesMeter());//new attributeMeter from v0.2
            TryAddAttributeMeter(new AttributeMeters.ActionReactionFirst3ByteHashMeter());
            TryAddAttributeMeter(new AttributeMeters.ByteBitValueMeter());//new attributeMeter from v0.2
            TryAddAttributeMeter(new AttributeMeters.ByteFrequencyMeter());
            TryAddAttributeMeter(new AttributeMeters.ByteFrequencyOfFirstPacketBytesMeter());
            TryAddAttributeMeter(new AttributeMeters.BytePairsReocurringCountIn32FirstBytesMeter());
            TryAddAttributeMeter(new AttributeMeters.BytePairsReocurringIn32FirstBytesMeter());
            TryAddAttributeMeter(new AttributeMeters.BytePairsReocurringOffsetsIn32FirstBytesMeter());
            TryAddAttributeMeter(new AttributeMeters.ByteValueOffsetHashOfFirst32BytesInFirst4PacketsMeter());
            TryAddAttributeMeter(new AttributeMeters.DirectionByteFrequencyMeter());
            TryAddAttributeMeter(new AttributeMeters.DirectionPacketLengthDistributionMeter());
            TryAddAttributeMeter(new AttributeMeters.First2OrderedFirst4CharWordsMeter());
            TryAddAttributeMeter(new AttributeMeters.First2OrderedFirstBitPositionsMeter());
            TryAddAttributeMeter(new AttributeMeters.First2OrderedPacketsFirstNByteNibblesMeter());
            TryAddAttributeMeter(new AttributeMeters.First2PacketsFirst16ByteHashCountsMeter());
            TryAddAttributeMeter(new AttributeMeters.First2PacketsFirst3ByteHashAndPacketLengthMeter());
            TryAddAttributeMeter(new AttributeMeters.First2PacketsFirst8ByteHashDirectionCountsMeter());
            TryAddAttributeMeter(new AttributeMeters.First2PacketsPerDirectionFirst5BytesDifferencesMeter());
            TryAddAttributeMeter(new AttributeMeters.First4DirectionFirstNByteNibblesMeter());
            TryAddAttributeMeter(new AttributeMeters.First4OrderedDirectionFirstNByteNibblesMeter());
            TryAddAttributeMeter(new AttributeMeters.First4OrderedDirectionInterPacketDelayMeter());
            TryAddAttributeMeter(new AttributeMeters.First4OrderedDirectionPacketSizeMeter());
            TryAddAttributeMeter(new AttributeMeters.First4PacketsByteFrequencyMeter());
            TryAddAttributeMeter(new AttributeMeters.First4PacketsByteReoccurringDistanceWithByteHashMeter());
            TryAddAttributeMeter(new AttributeMeters.First4PacketsFirst16BytePairsMeter());
            TryAddAttributeMeter(new AttributeMeters.First4PacketsFirst32BytesEqualityMeter());
            TryAddAttributeMeter(new AttributeMeters.FirstBitPositionsMeter());
            TryAddAttributeMeter(new AttributeMeters.FirstPacketPerDirectionFirstNByteNibblesMeter());
            TryAddAttributeMeter(new AttributeMeters.FirstServerPacketFirstBitPositionsMeter());
            TryAddAttributeMeter(new AttributeMeters.PacketLengthDistributionMeter());
            TryAddAttributeMeter(new AttributeMeters.PacketPairLengthPrimesMeter());
            */
        }
        public ProtocolModel(string protocolName, SortedList<string, AttributeFingerprintHandler> attributeFingerprintHandlers, int modelTrainingSessionCount, ulong observationCount, List<ushort> defaultPorts)
            : this(protocolName) {
            this.attributeFingerprintHandlers=attributeFingerprintHandlers;
            this.trainingSessionCount=modelTrainingSessionCount;
            this.observationCount=observationCount;
           
            this.defaultPorts=defaultPorts;
        }
        public ProtocolModel(string protocolName, SortedList<string, AttributeFingerprintHandler> attributeFingerprintHandlers, int modelTrainingSessionCount, ulong observationCount, List<ushort> defaultPorts, ICollection<string> activeAttributeMeters)
            : this(protocolName, attributeFingerprintHandlers, modelTrainingSessionCount, observationCount, defaultPorts) {
            TrimAttributeFingerprintHandlers(activeAttributeMeters);
        }

        
        private void TrimAttributeFingerprintHandlers(ICollection<string> activeAttributeMeterNames) {
            //This is a bit backwards, but I will now remove the protocol models that are not in the
            //supplied activeAttributeMeters list
            List<string> keysToRemove=new List<string>();
            foreach(string name in this.attributeFingerprintHandlers.Keys)
                if(!activeAttributeMeterNames.Contains(name))
                    keysToRemove.Add(name);
            foreach(string key in keysToRemove)
                this.attributeFingerprintHandlers.Remove(key); 
        }
        

        /*
        private bool TryAddAttributeMeter(AttributeMeters.IAttributeMeter attributeMeter, ICollection<string> activeAttributeMeterNames) {
            if(activeAttributeMeterNames.Contains(attributeMeter.AttributeName)) {
                this.attributeFingerprintHandlers.Add(attributeMeter.AttributeName, new AttributeFingerprintHandler(attributeMeter));
                return true;
            }
            throw new Exception("Just testing...");
            return false;
        }*/

        public void Clear() {
            this.sessionIsOpen=true;
            this.trainingSessionCount=0;
            this.observationCount=0;
            foreach(AttributeFingerprintHandler h in this.attributeFingerprintHandlers.Values)
                h.AttributeFingerprint.Clear();
        }

        public void Close() {
            this.sessionIsOpen=false;
        }


        public void AddObservation(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection) {
            if(packetLength>0) {//only analyze non-zero size packets since only they say something about the packet in the payload
                if(sessionIsOpen) {
                    
                    //TimeSpan interPacketDelay=packetTimestamp.Subtract(this.lastPacketTimestamp[packetDirection]);

                    foreach(AttributeFingerprintHandler f in attributeFingerprintHandlers.Values)
                        f.AddObservation(frameData, packetStartIndex, packetLength, packetTimestamp, packetDirection, (int)observationCount);

                    //this.lastPacketTimestamp[packetDirection]=packetTimestamp;
                    this.observationCount++;
                    if(trainingSessionCount==0)
                        trainingSessionCount++;
                }
                else {
                    throw new Exception("Observations cannot be added to closed session models. Use MergeWith() function instead.");
                }
            }
        }

        public ProtocolModel MergeWith(ProtocolModel otherSessionModel) {

            SortedList<string, AttributeFingerprintHandler> mergedFingerprints=new SortedList<string, AttributeFingerprintHandler>(attributeFingerprintHandlers.Count);
            foreach(string fingerprintAttributeName in this.attributeFingerprintHandlers.Keys) {
                mergedFingerprints.Add(fingerprintAttributeName, this.attributeFingerprintHandlers[fingerprintAttributeName].MergeWith(otherSessionModel.attributeFingerprintHandlers[fingerprintAttributeName]));
            }
            List<ushort> mergedDefaultPorts=new List<ushort>(this.defaultPorts);
            mergedDefaultPorts.AddRange(otherSessionModel.defaultPorts);

            ProtocolModel returnSessionModel=new ProtocolModel(this.protocolName, mergedFingerprints, this.trainingSessionCount+otherSessionModel.TrainingSessionCount, this.observationCount+otherSessionModel.ObservationCount, mergedDefaultPorts);
            returnSessionModel.Close();

            //returnSessionModel.ObservationCount=this.ObservationCount+otherSessionModel.ObservationCount;

            return returnSessionModel;

        }

        public AttributeFingerprintHandler GetAttributeFingerprint(string protocolAttributeName) {
            if(this.attributeFingerprintHandlers.ContainsKey(protocolAttributeName))
                return this.attributeFingerprintHandlers[protocolAttributeName];
            else
                return AttributeFingerprintHandler.EmptySingletonInstance;
        }

        public SortedList<string, double> GetKullbackLeiblerDivergencesFrom(ProtocolModel protocolModel) {
            SortedList<string, double> divergences=new SortedList<string, double>(attributeFingerprintHandlers.Count);
            foreach(AttributeFingerprintHandler f in attributeFingerprintHandlers.Values) {
                divergences.Add(f.AttributeMeterName, f.GetKullbackLeiblerDivergenceFrom(protocolModel.GetAttributeFingerprint(f.AttributeMeterName)));
            }
            return divergences;
        }
        public double GetAverageKullbackLeiblerDivergenceFrom(ProtocolModel protocolModel) {
            double totalDivergence=0.0;
            /*
            foreach(AttributeFingerprintHandler f in attributeFingerprintHandlers.Values) {
                double attributeDivergence=f.GetKullbackLeiblerDivergenceFrom(protocolModel.GetAttributeFingerprint(f.AttributeMeterName));
                totalDivergence+=attributeDivergence;
            }
             * */
            foreach(double d in GetKullbackLeiblerDivergencesFrom(protocolModel).Values)
                totalDivergence+=d;
            return totalDivergence/attributeFingerprintHandlers.Count;
        }

        public override string ToString() {
            return this.protocolName;
        }

        public System.Xml.XmlElement GetXml(XmlDocument xmlDoc) {
            //XmlDocument xmlDoc=new XmlDocument();
            XmlElement modelElement=xmlDoc.CreateElement("protocolModel");
            modelElement.SetAttribute("name", this.protocolName);
            modelElement.SetAttribute("sessionCount", this.trainingSessionCount.ToString());
            modelElement.SetAttribute("observationCount", this.observationCount.ToString());
            XmlElement defaultPortsElement=xmlDoc.CreateElement("defaultPorts");
            foreach(uint p in this.defaultPorts){
                XmlElement portElement=xmlDoc.CreateElement("port");
                portElement.InnerText=p.ToString();
                defaultPortsElement.AppendChild(portElement);
            }
            modelElement.AppendChild(defaultPortsElement);

            foreach(AttributeFingerprintHandler f in this.attributeFingerprintHandlers.Values) {
                modelElement.AppendChild(f.GetXml(xmlDoc));
            }
            return modelElement;

        }

    }
}
