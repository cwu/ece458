using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {

    //Inspired by: "Automatic Handling of Protocol Dependencies
    //and Reaction to 0-Day Attacks with ScriptGen Based Honeypots"
    //
    //Corrado Leita, Marc Dacier, and Frederic Massicotte
    //http://www.eurecom.fr/util/publidownload.fr.htm?id=2023
    class BytePairsReocurringOffsetsIn32FirstBytes : IClassificationAttribute {

        private System.Collections.Generic.SortedList<ushort, int> bytePairOffsetsFromPreviousPacket;
        private ClassificationAttributeFingerprint.PacketDirection previousPacketDirection;
        //private System.Collections.Generic.SortedList<KeyValuePair<AttributeFingerprint.PacketDirection, AttributeFingerprint.PacketDirection>, int> directionPairOffset;
        private int directionPairLength;

        //private ushort[] bytePairsFromPreviousPacket;

        public BytePairsReocurringOffsetsIn32FirstBytes() {
            this.bytePairOffsetsFromPreviousPacket=new SortedList<ushort, int>();
            this.previousPacketDirection=ClassificationAttributeFingerprint.PacketDirection.Unknown;
            this.directionPairLength=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/9;
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "BytePairsReocurringOffsetsIn32FirstBytes"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            System.Collections.Generic.SortedList<ushort, int> bytePairOffsetsInCurrentPacket=new SortedList<ushort,int>();
            byte[] bytePair=new byte[2];
            for(int i=0; packetStartIndex+2*i+1<frameData.Length && 2*i+1<packetLength && i<16; i++) {
                Array.Copy(frameData, packetStartIndex+i*2, bytePair, 0, 2);
                ushort key=ConvertToUshort(bytePair[0], bytePair[1]);
                if(!bytePairOffsetsInCurrentPacket.ContainsKey(key))
                    bytePairOffsetsInCurrentPacket.Add(key, i);
                if(this.bytePairOffsetsFromPreviousPacket.ContainsKey(key))
                    yield return this.GetDirectionPairOffset(this.previousPacketDirection, packetDirection)+Math.Min(i, directionPairLength-1);
            }

            //int reocurringCount=0;
            /*
            foreach(ushort key in bytePairOffsetsInCurrentPacket.Keys)
                if(this.bytePairOffsetsFromPreviousPacket.ContainsKey(key))
                    yield return this.GetDirectionPairOffset(this.previousPacketDirection, packetDirection)+Math.Min(bytePairOffsetsInCurrentPacket[key], directionPairLength-1);
            */
            this.bytePairOffsetsFromPreviousPacket=bytePairOffsetsInCurrentPacket;
            this.previousPacketDirection=packetDirection;

            yield break;
        }

        #endregion

        private ushort ConvertToUshort(byte b1, byte b2) {
            ushort returnValue=b1;
            returnValue<<=8;
            returnValue+=b2;
            return returnValue;
        }

        private int GetDirectionPairOffset(ClassificationAttributeFingerprint.PacketDirection previousDirection, ClassificationAttributeFingerprint.PacketDirection currentDirection) {
            int returnValue=0*directionPairLength;
            if(previousPacketDirection== ClassificationAttributeFingerprint.PacketDirection.ClientToServer)
                returnValue+=3*directionPairLength;
            else if(previousPacketDirection== ClassificationAttributeFingerprint.PacketDirection.ServerToClient)
                returnValue+=6*directionPairLength;
            if(currentDirection== ClassificationAttributeFingerprint.PacketDirection.ClientToServer)
                returnValue+=1*directionPairLength;
            else if(currentDirection==ClassificationAttributeFingerprint.PacketDirection.ServerToClient)
                returnValue+=2*directionPairLength;
            return returnValue;
        }
    }
}
