using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {

    //Inspired by: "Automatic Handling of Protocol Dependencies
    //and Reaction to 0-Day Attacks with ScriptGen Based Honeypots"
    //
    //Corrado Leita, Marc Dacier, and Frederic Massicotte
    //http://www.eurecom.fr/util/publidownload.fr.htm?id=2023
    class BytePairsReocurringIn32FirstBytes : IClassificationAttribute {

        private System.Collections.Generic.SortedList<ushort, byte[]> bytePairsFromPreviousPacket;

        //private ushort[] bytePairsFromPreviousPacket;

        public BytePairsReocurringIn32FirstBytes() {
            this.bytePairsFromPreviousPacket=new SortedList<ushort, byte[]>();
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "BytePairsReocurringIn32FirstBytes"; }
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

            foreach(ushort key in bytePairsInCurrentPacket.Keys)
                if(this.bytePairsFromPreviousPacket.ContainsKey(key))
                    yield return ConvertHelper.ToHashValue((int)key, ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS);

            this.bytePairsFromPreviousPacket=bytePairsInCurrentPacket;
            yield break;
        }

        #endregion

        private ushort ConvertToUshort(byte b1, byte b2) {
            ushort returnValue=b1;
            returnValue<<=8;
            returnValue+=b2;
            return returnValue;
        }

    }
}
