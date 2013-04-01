using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification {

    public class ClassificationAttributeFingerprint{
        public enum PacketDirection { Unknown, ClientToServer, ServerToClient }

        private static ClassificationAttributeFingerprint emptySingletonInstance=null;
        public static ClassificationAttributeFingerprint EmptySingletonInstance {
            get {
                if(emptySingletonInstance==null)
                    emptySingletonInstance=new ClassificationAttributeFingerprint(null);
                return emptySingletonInstance;
            }
        }

        private ClassificationAttributes.IClassificationAttribute classificationAttribute;
        private Fingerprint fingerprint;

        public double[] FingerprintProbabilityData { get { return this.fingerprint.FingerprintProbabilityData; } }
        public ClassificationAttributes.IClassificationAttribute ClassificationAttrubute { get { return this.classificationAttribute; } }

        public double ModelMultiplicator { get { return fingerprint.SampleCount/(Fingerprint.FINGERPRINT_LENGTH+(double)fingerprint.SampleCount); } }
        public double ModelIncrement { get { return 1.0/(Fingerprint.FINGERPRINT_LENGTH+(double)fingerprint.SampleCount); } }
        public double ObservationMultiplicator { get { return fingerprint.SampleCount/(1.0+(double)fingerprint.SampleCount); } }
        public double ObservationIncrement { get { return (1.0/Fingerprint.FINGERPRINT_LENGTH)/(1.0+(double)fingerprint.SampleCount); } }

        public ClassificationAttributeFingerprint(ClassificationAttributes.IClassificationAttribute attribute) {
            this.classificationAttribute=attribute;
            this.fingerprint=new Fingerprint();
        }
        public ClassificationAttributeFingerprint(ClassificationAttributes.IClassificationAttribute attribute, double[] fingerprintData, ulong fingerprintCount)
            : this(attribute) {
            if(fingerprintCount>ulong.MaxValue/4) {
                fingerprintCount/=4;
            }
            this.fingerprint=new Fingerprint(fingerprintData, fingerprintCount);
        }

        public ClassificationAttributeFingerprint MergeWith(ClassificationAttributeFingerprint otherFingerprint) {
            if(otherFingerprint.classificationAttribute.AttributeName==this.classificationAttribute.AttributeName) {
                return new ClassificationAttributeFingerprint(this.classificationAttribute, this.fingerprint.MergeWith(otherFingerprint.fingerprint).FingerprintProbabilityData, this.fingerprint.SampleCount+otherFingerprint.fingerprint.SampleCount);
            }
            else
                throw new Exception("Fingerprints must be of the same attribute in order to be merged!");
        }

        public double GetKullbackLeiblerDivergenceFrom(ClassificationAttributeFingerprint protocolAttributeModel) {
            //We now assume that "this" is the observation (P)
            //and the "protocolAttributeModel" is the model/theory (Q)

            //KL-divergence = SUM_over_all_i( P(i)*log( P(i)/Q(i) ) )

            //The method of using KL-divergence is very similar to using "cross entropy" as in "Evaluation of Language Identification Methods" by Simon Kranig
            //The main difference is that KL-divergence reduces the cross entropy with the entropy of the observation it self

            double observationFixMultiplicator=this.ObservationMultiplicator;
            double observationFixIncrement=this.ObservationIncrement;
            double modelFixMultiplicator=protocolAttributeModel.ModelMultiplicator;
            double modelFixIncrement=protocolAttributeModel.ModelIncrement;

            double divergence=0.0;
            double[] debugArray=new double[Fingerprint.FINGERPRINT_LENGTH];
            for(int i=0; i<Fingerprint.FINGERPRINT_LENGTH; i++) {
                //divergence+=(this.FingerprintData[i]*observationFixMultiplicator+observationFixAddition)*Math.Log((this.FingerprintData[i]*observationFixMultiplicator+observationFixAddition)/(protocolAttributeModel.FingerprintData[i]*modelFixMultiplicator+modelFixAddition), 2.0);
                divergence+=(this.FingerprintProbabilityData[i]*observationFixMultiplicator+observationFixIncrement)*Math.Log((this.FingerprintProbabilityData[i]*observationFixMultiplicator+observationFixIncrement)/(protocolAttributeModel.FingerprintProbabilityData[i]*modelFixMultiplicator+modelFixIncrement), 2.0);
                debugArray[i]=(this.FingerprintProbabilityData[i]*observationFixMultiplicator+observationFixIncrement)*Math.Log((this.FingerprintProbabilityData[i]*observationFixMultiplicator+observationFixIncrement)/(protocolAttributeModel.FingerprintProbabilityData[i]*modelFixMultiplicator+modelFixIncrement), 2.0);
            }
            System.Diagnostics.Debug.WriteLine(protocolAttributeModel.ClassificationAttrubute.AttributeName+" KL-Divergence: "+divergence);

            return divergence;
        }

        public void AddObservation(byte[] packetData, int packetStartIndex, int packetLength, DateTime packetTimestamp, PacketDirection packetDirection, int packetOrderNumberInSession) {
            foreach(int fingerprintIndex in classificationAttribute.GetFingerprintIndices(packetData, packetStartIndex, packetLength, packetTimestamp, packetDirection, packetOrderNumberInSession))
                fingerprint.IncrementFingerprintDataAtIndex(fingerprintIndex);
        }

        public System.Xml.XmlElement GetXml(System.Xml.XmlDocument xmlDoc) {
            //System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            System.Xml.XmlElement xmlElement=xmlDoc.CreateElement("attributeFingerprint");
            xmlElement.SetAttribute("classificationAttribute", this.classificationAttribute.AttributeName);
            xmlElement.SetAttribute("sampleCount", this.fingerprint.SampleCount.ToString());
            //xmlElement.SetAttribute("fingerprintBits", Fingerprint.FINGERPRINT_BITS.ToString());

            for(int i=0; i<this.fingerprint.FingerprintProbabilityData.Length; i++) {
                System.Xml.XmlElement xmlDataElement=xmlDoc.CreateElement("data");
                xmlDataElement.SetAttribute("bin", i.ToString());
                xmlDataElement.InnerText=this.fingerprint.FingerprintProbabilityData[i].ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                //xmlDataElement.Value=
                xmlElement.AppendChild(xmlDataElement);
            }
            return xmlElement;
        }


        public class Fingerprint {

            //public const ushort FINGERPRINT_LENGTH=256;

            /// <summary>
            /// The number of bits used per fingerprint (normally 8)
            /// </summary>
            public const ushort FINGERPRINT_BITS=7;//preferably 8 I guess...

            /// <summary>
            /// The number of bins (counters) used for the fingerprtint (usually 256)
            /// </summary>
            public static ushort FINGERPRINT_LENGTH { get { return (ushort)(1<<FINGERPRINT_BITS); } }

            private double[] fingerprintProbabilityData;
            private long[] fingerprintCounterData;
            private bool fingerprintProbabilityDataIsIpdated;

            internal double[] FingerprintProbabilityData {
                get {
                    if(!fingerprintProbabilityDataIsIpdated)
                        UpdateFingerprintData();
                    return this.fingerprintProbabilityData;
                }
            }
            internal ulong SampleCount {
                get {
                    ulong returnValue=0;
                    foreach(long l in this.fingerprintCounterData)
                        returnValue+=(ulong)l;
                    return returnValue;
                }
            }

            internal Fingerprint(long[] fingerprintCounterData)
                : this() {
                if(fingerprintCounterData.Length!=FINGERPRINT_LENGTH)
                    throw new Exception("Wrong length of fingerprintRawData");
                this.fingerprintCounterData=fingerprintCounterData;
            }
            internal Fingerprint(double[] fingerprintProbabilityData, ulong fingerprintCount)
                : this() {
                if(fingerprintProbabilityData.Length!=FINGERPRINT_LENGTH)
                    throw new Exception("Wrong length of fingerprintProbabilityData");

                this.fingerprintProbabilityDataIsIpdated=false;
                if(fingerprintCount>long.MaxValue/2)
                    fingerprintCount=long.MaxValue/2;
                fingerprintProbabilityData.CopyTo(this.fingerprintProbabilityData, 0);
                for(int i=0; i<FINGERPRINT_LENGTH; i++)
                    fingerprintCounterData[i]=(long)(fingerprintProbabilityData[i]*fingerprintCount);
                this.fingerprintProbabilityDataIsIpdated=true;
            }
            internal Fingerprint() {
                this.fingerprintProbabilityDataIsIpdated=false;
                this.fingerprintProbabilityData=new double[FINGERPRINT_LENGTH];
                this.fingerprintCounterData=new long[FINGERPRINT_LENGTH];
                /*
                //set all values to 1 to avoid division by zero
                for(int i=0; i<FINGERPRINT_LENGTH; i++)
                    fingerprintCounterData[i]=1;
                 * */
            }

            internal Fingerprint MergeWith(Fingerprint orherFingerprint) {
                long[] newCounterData=(long[])this.fingerprintCounterData.Clone();

                ulong otherCount=orherFingerprint.SampleCount;
                for(int i=0; i<FINGERPRINT_LENGTH; i++){
                    newCounterData[i]+=(long)(otherCount*orherFingerprint.FingerprintProbabilityData[i]);
                }

                return new Fingerprint(newCounterData);
            }

            private void UpdateFingerprintData() {
                lock(this.fingerprintProbabilityData.SyncRoot) {
                    lock(this.fingerprintCounterData.SyncRoot) {//Lets hope we don't get any deadlocks here!
                        this.fingerprintProbabilityDataIsIpdated=false;
                        ulong fingerprintCount=0;
                        for(int i=0; i<FINGERPRINT_LENGTH; i++)
                            fingerprintCount+=(ulong)fingerprintCounterData[i];
                        if(fingerprintCount==0) {
                            System.Diagnostics.Debug.WriteLine("Fingerprint has no data");
                            for(int i=0; i<FINGERPRINT_LENGTH; i++)
                                fingerprintProbabilityData[i]=1.0/FINGERPRINT_LENGTH;
                        }
                        else {
                            double factor=((double)1)/fingerprintCount;
                            for(int i=0; i<FINGERPRINT_LENGTH; i++)
                                fingerprintProbabilityData[i]=factor*fingerprintCounterData[i];
                        }
                        this.fingerprintProbabilityDataIsIpdated=true;
                    }
                }
            }
            internal void IncrementFingerprintDataAtIndex(int index) {
                this.fingerprintProbabilityDataIsIpdated=false;
                System.Threading.Interlocked.Increment(ref fingerprintCounterData[index]);

                if((ulong)fingerprintCounterData[index]>=ulong.MaxValue/FINGERPRINT_LENGTH) {
                    //divide all values by 2
                    lock(this.fingerprintCounterData.SyncRoot) {
                        for(int i=0; i<FINGERPRINT_LENGTH; i++)
                            fingerprintCounterData[i]/=2;//Is this OK notation?
                    }
                }
            }

        }
    }
}
