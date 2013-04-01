using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {

    /// <summary>
    /// Counts the total bytes sent in each direction for the 4 first flow direction changes
    /// 2 bits used to denote the number of flow direction changes, the other bits show the
    /// total number of bytes using a linear representation (many other attributeMeters use
    /// logarithmical representations).
    /// </summary>
    class AccumulatedDirectionBytesMeter : IAttributeMeter{

        private const int BYTE_CHUNK_UNIT_SIZE=64;//64 bytes per "byteChunk"
        
        private AttributeFingerprintHandler.PacketDirection lastDirection;
        private int accumulatedBytesCount;
        private int directionChanges;
        

        public AccumulatedDirectionBytesMeter() {
            this.lastDirection= AttributeFingerprintHandler.PacketDirection.Unknown;
            this.accumulatedBytesCount=0;
            this.directionChanges=0;
        }

        #region IAttributeMeter Members

        public string AttributeName {
            get { return "AccumulatedDirectionBytesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(directionChanges<4 && (packetDirection!=lastDirection || accumulatedBytesCount/BYTE_CHUNK_UNIT_SIZE<AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/4)){
                //see if the direction has changed
                if(packetDirection!=lastDirection){
                    if(lastDirection!= AttributeFingerprintHandler.PacketDirection.Unknown) {
                        directionChanges++;
                        if(directionChanges>=4)
                            yield break;
                    }
                    lastDirection=packetDirection;
                    accumulatedBytesCount=0;
                }

                accumulatedBytesCount+=packetLength;
                //encode the direction change count and accumulated bytes count to something expressed in BYTE_CHUNK_UNIT_SIZEs and return the value
                yield return (directionChanges<<(AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS-2))+Math.Min(accumulatedBytesCount/BYTE_CHUNK_UNIT_SIZE, (AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/4)-1);
            }
        }

        #endregion
    }
}
