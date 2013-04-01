using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {

    //Inspired by: "Automatic Handling of Protocol Dependencies
    //and Reaction to 0-Day Attacks with ScriptGen Based Honeypots"
    //
    //Corrado Leita, Marc Dacier, and Frederic Massicotte
    //http://www.eurecom.fr/util/publidownload.fr.htm?id=2023
    class BytePairsReocurringCountIn32FirstBytes : IClassificationAttribute {

        private System.Collections.Generic.SortedList<ushort, byte[]> bytePairsFromPreviousPacket;
        private ClassificationAttributeFingerprint.PacketDirection previousPacketDirection;
        //private System.Collections.Generic.SortedList<KeyValuePair<AttributeFingerprint.PacketDirection, AttributeFingerprint.PacketDirection>, int> directionPairOffset;
        private int directionPairLength;

        //private ushort[] bytePairsFromPreviousPacket;

        public BytePairsReocurringCountIn32FirstBytes() {
            this.bytePairsFromPreviousPacket=new SortedList<ushort, byte[]>();
            this.previousPacketDirection=ClassificationAttributeFingerprint.PacketDirection.Unknown;
            this.directionPairLength=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/9;
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "BytePairsReocurringCountIn32FirstBytes"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            System.Collections.Generic.SortedList<ushort, byte[]> bytePairsInCurrentPacket=new SortedList<ushort,byte[]>();
            byte[] bytePair=new byte[2];
            for(int i=0; packetStartIndex+2*i+1<frameData.Length && 2*i+1<packetLength && i<16; i++) {
                Array.Copy(frameData, packetStartIndex+i*2, bytePair, 0, 2);
                ushort key=ConvertToUshort(bytePair[0], bytePair[1]);
                if(!bytePairsInCurrentPacket.ContainsKey(key))
                    bytePairsInCurrentPacket.Add(key, (byte[])bytePair.Clone());
            }

            int reocurringCount=0;
            foreach(ushort key in bytePairsInCurrentPacket.Keys)
                if(this.bytePairsFromPreviousPacket.ContainsKey(key))
                    reocurringCount++;

            int returnValue=this.GetDirectionPairOffset(this.previousPacketDirection, packetDirection)+Math.Min(reocurringCount, directionPairLength-1);
            
            this.bytePairsFromPreviousPacket=bytePairsInCurrentPacket;
            this.previousPacketDirection=packetDirection;

            yield return returnValue;
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
