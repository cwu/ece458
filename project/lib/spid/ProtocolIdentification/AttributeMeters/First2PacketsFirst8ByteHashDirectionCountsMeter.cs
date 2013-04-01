//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First2PacketsFirst8ByteHashDirectionCountsMeter : IAttributeMeter{
        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First2PacketsFirst8ByteHashDirectionCountsMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //The first FINGERPRINT_BITS-4 bits define the byteHash value
            //Next bit defines the packet direction
            //The last 3 bits defines the count (number of bytes) with the specified byteHash


            if(packetOrderNumberInSession<2) {
                int packetDirectionSignature;
                if(packetDirection==AttributeFingerprintHandler.PacketDirection.ClientToServer)
                    packetDirectionSignature=8;//00001000
                else if(packetDirection==AttributeFingerprintHandler.PacketDirection.ServerToClient)
                    packetDirectionSignature=0;//00000000
                else
                    yield break;


                int byteHashBinCount=1<<(AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS-4);
                byte[] byteHashBinCounters=new byte[byteHashBinCount];
                for(int i=0; i<8 && i+packetStartIndex<frameData.Length && i<packetLength; i++) {
                    int byteHash=ConvertHelper.ToHashValue((int)frameData[packetStartIndex+i], AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS-4);
                    byteHashBinCounters[byteHash]++;
                }

                //now return the values
                for(int i=0; i<byteHashBinCounters.Length; i++) {
                    yield return (i<<4)^packetDirectionSignature^byteHashBinCounters[i];
                }
            }
        }

        #endregion
    }
}
