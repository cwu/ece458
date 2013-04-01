using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class PacketLengthDistributionMeter : IAttributeMeter{
        #region IAttributeMeter Members

        string IAttributeMeter.AttributeName {
            get { return "PacketLengthDistributionMeter"; }
        }

        IEnumerable<int> IAttributeMeter.GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //this is a VERY simple metering function!
            yield return packetLength%AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH;
        }

        #endregion
    }
}
