//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First2OrderedFirst4CharWordsMeter : IAttributeMeter {


        //public First2OrderedFirst4CharWordsAttribute() {}

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First2OrderedFirst4CharWordsMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2) {
                StringBuilder sb=new StringBuilder(4);

                for(int i=0; sb.Length<4 && i<packetLength && packetStartIndex+i<frameData.Length; i++)
                    sb.Append(frameData[packetStartIndex+i]);
                yield return ConvertHelper.ToHashValue((sb.ToString()+packetDirection.ToString()).GetHashCode(), AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS);
            }
        }

        #endregion
    }
}
