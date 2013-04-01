using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    //Inspired by "KISS: Stochastic Packet Inspection" by Finamore, Mellia, Meo and Rossi
    //http://www.tlc-networks.polito.it/mellia/papers/KISS_tma.pdf
    class NibblePositionPopularityMeter : IAttributeMeter{
        private const int PACKETS_TO_INSPECT=8;
        //this class holds a great deal of state
        private int nibblesToInspect;
        private int[,] nibbleCounters;



        public NibblePositionPopularityMeter() {
            this.nibblesToInspect=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/PACKETS_TO_INSPECT;
            this.nibbleCounters=new int[16, this.nibblesToInspect];//16 values for a nibble, 32 nibbles to inspect (24 in the original paper)
        }



        #region IAttributeMeter Members

        public string AttributeName {
            get { return "NibblePositionPopularityMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<PACKETS_TO_INSPECT) {
                for(int nibbleIndex=0; packetStartIndex+nibbleIndex/2<frameData.Length && nibbleIndex/2<packetLength && nibbleIndex<this.nibblesToInspect; nibbleIndex++) {
                    int byteIndex=packetStartIndex+nibbleIndex/2;
                    bool firstNibble=(nibbleIndex%2==0);
                    byte nibble=ConvertHelper.GetNibble(frameData[byteIndex], firstNibble);
                    
                    //count how many nibble values that have occured more often than the received one
                    int nibblePopularityRank=0;//we start with the best popularity (0)
                    for(int i=0; i<16; i++)
                        if(nibbleCounters[i, nibbleIndex]>nibbleCounters[nibble, nibbleIndex])
                            nibblePopularityRank++;//one step down on the popularity ranking

                    nibbleCounters[nibble, nibbleIndex]++;
                    //skip the first return values
                    if(packetOrderNumberInSession>0) {
                        //nibblePopularityRank<PACKETS_TO_INSPECT
                        //now we need to combine the nibbleIndex and populatiryRank and return it
                        yield return nibbleIndex*PACKETS_TO_INSPECT+nibblePopularityRank;
                    }
                    
                }
            }
        }

        #endregion
    }
}
