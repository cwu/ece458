using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class PacketPairLengthPrimesMeter : IAttributeMeter{
        
        //I would like to make this one CONST, but I cannot!
        private readonly int[] PRIME_NUMBERS={ 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89 };

        private uint previousPacketPairLengthPrime;

        public PacketPairLengthPrimesMeter() {
            this.previousPacketPairLengthPrime=0;//not really needed...
        }

        #region IAttributeMeter Members

        string IAttributeMeter.AttributeName {
            get { return "PacketPairLengthPrimesMeter"; }
        }

        IEnumerable<int> IAttributeMeter.GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {

            //start with shifting down the previous values halfway down to the LSB positions
            previousPacketPairLengthPrime=previousPacketPairLengthPrime>>AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS/2;
            
            //now start building something to add
            uint newLengthPrime=0;
            for(int i=0; i<AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS/2; i++){
                if(packetLength%PRIME_NUMBERS[i]==0)//the prime is a factor (divisor)
                    newLengthPrime|=(uint)(1<<i);//add a flag for that prime
            }
            //add the newLengthPrime as the MSB to the previous thingy
            previousPacketPairLengthPrime|=(newLengthPrime<<AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS/2);
            yield return (int)previousPacketPairLengthPrime;
            
        }

        #endregion
    }
}
