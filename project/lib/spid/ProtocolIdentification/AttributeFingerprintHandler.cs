//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification {

    public class AttributeFingerprintHandler{
        public enum PacketDirection { Unknown, ClientToServer, ServerToClient }

        private static AttributeFingerprintHandler emptySingletonInstance=null;
        public static AttributeFingerprintHandler EmptySingletonInstance {
            get {
                if(emptySingletonInstance==null)
                    emptySingletonInstance=new AttributeFingerprintHandler((AttributeMeters.IAttributeMeter)null);
                return emptySingletonInstance;
            }
        }

        private AttributeMeters.IAttributeMeter attributeMeter;
        private string attributeMeterName;
        private Fingerprint fingerprint;
        private System.Diagnostics.Stopwatch observationStopWatch;//timer to meassure performance of th attributeMeter
        private TimeSpan observationComputingTime;

        public Fingerprint AttributeFingerprint { get { return this.fingerprint; } }
        public string AttributeMeterName { get { return this.attributeMeterName; } }


        public TimeSpan ObservationComputingTime { get { return this.observationComputingTime.Add(this.observationStopWatch.Elapsed); } }

        public AttributeFingerprintHandler(AttributeMeters.IAttributeMeter attributeMeter)
        : this(attributeMeter.AttributeName){
            this.attributeMeter=attributeMeter;
            //this.attributeMeterName=attributeMeter.AttributeName;
            this.fingerprint=new Fingerprint();
        }
        public AttributeFingerprintHandler(string attributeMeterName, double[] fingerprintProbabilityDistributionVector, ulong measurementCount, TimeSpan observationComputingTime)
            : this(attributeMeterName, fingerprintProbabilityDistributionVector, measurementCount) {
            this.observationComputingTime=this.observationComputingTime.Add(observationComputingTime);
        }
        public AttributeFingerprintHandler(string attributeMeterName, double[] fingerprintProbabilityDistributionVector, ulong measurementCount)
        : this(attributeMeterName){
            //this.attributeMeterName=attributeMeterName;
            this.attributeMeter=null;
            if(measurementCount>ulong.MaxValue/4) {
                measurementCount/=4;
            }
            this.fingerprint=new Fingerprint(fingerprintProbabilityDistributionVector, measurementCount);
        }
        private AttributeFingerprintHandler(string attributeMeterName) {
            this.attributeMeterName=attributeMeterName;
            this.observationStopWatch=new System.Diagnostics.Stopwatch();
            this.observationComputingTime=new TimeSpan();
        }

        public AttributeFingerprintHandler MergeWith(AttributeFingerprintHandler otherFingerprint) {
            if(otherFingerprint.AttributeMeterName==this.AttributeMeterName) {
                return new AttributeFingerprintHandler(this.attributeMeterName, this.fingerprint.MergeWith(otherFingerprint.fingerprint).ProbabilityVector, this.fingerprint.MeasurementCount+otherFingerprint.fingerprint.MeasurementCount, this.observationComputingTime.Add(otherFingerprint.ObservationComputingTime));
            }
            else
                throw new Exception("Fingerprints must be of the same attribute in order to be merged!");
        }

        public double GetKullbackLeiblerDivergenceFrom(AttributeFingerprintHandler protocolModelAttribute) {
            //We now assume that "this" is the observation (P)
            //and the "protocolAttributeModel" is the model/theory (Q)

            //KL-divergence = SUM_over_all_i( P(i)*log( P(i)/Q(i) ) )

            //The method of using KL-divergence is very similar to using "cross entropy" as in "Evaluation of Language Identification Methods" by Simon Kranig
            //The main difference is that KL-divergence reduces the cross entropy with the entropy of the observation it self

            double divergence=0.0;
            for(int i=0; i<Fingerprint.FINGERPRINT_LENGTH; i++) {
                divergence+=this.AttributeFingerprint.ObservationProbabilityVector[i]*(this.AttributeFingerprint.ObservationLogarithmicProbabilityVector[i]-protocolModelAttribute.AttributeFingerprint.ModelLogarithmicProbabilityVector[i]);
            }
            

            return divergence;
        }

        public void AddObservation(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, PacketDirection packetDirection, int packetOrderNumberInSession) {
            this.observationStopWatch.Start();
            foreach(int measurement in attributeMeter.GetMeasurements(frameData, packetStartIndex, packetLength, packetTimestamp, packetDirection, packetOrderNumberInSession))
                fingerprint.IncrementFingerprintCounterAtIndex(measurement);
            //System.Threading.Thread.Sleep(10);
            this.observationStopWatch.Stop();
            return;
        }

        public System.Xml.XmlElement GetXml(System.Xml.XmlDocument xmlDoc) {
            //System.Xml.XmlDocument xmlDoc=new System.Xml.XmlDocument();
            System.Xml.XmlElement xmlElement=xmlDoc.CreateElement("attributeFingerprint");
            xmlElement.SetAttribute("attributeMeterName", this.attributeMeterName);
            xmlElement.SetAttribute("measurementCount", this.fingerprint.MeasurementCount.ToString());

            for(int i=0; i<this.fingerprint.ProbabilityVector.Length; i++) {
                System.Xml.XmlElement xmlBinElement=xmlDoc.CreateElement("bin");
                xmlBinElement.SetAttribute("i", i.ToString());
                xmlBinElement.InnerText=this.fingerprint.ProbabilityVector[i].ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                //xmlDataElement.Value=
                xmlElement.AppendChild(xmlBinElement);
            }
            return xmlElement;
        }


        public class Fingerprint {

            //public const ushort FINGERPRINT_LENGTH=256;

            /// <summary>
            /// The number of bits used per fingerprint (normally 8)
            /// </summary>
            public const ushort FINGERPRINT_BITS=8;//preferably 8 I guess...
            //private ushort fingerprintBits;

            //public ushort FingerprintBits { get { this.fingerprintBits; } }
            //public ushort FingerprintLength { get { return (ushort)(1<<this.fingerprintBits); } }

            /// <summary>
            /// The number of bins (counters) used for the fingerprtint (usually 256)
            /// </summary>
            public static ushort FINGERPRINT_LENGTH { get { return (ushort)(1<<FINGERPRINT_BITS); } }


            private long[] fingerprintCounterVector;

            //here are some helper variables to trade memory for speed
            private double[] fingerprintProbabilityVector;
            private bool fingerprintProbabilityVectorIsIpdated;
            private double[] modelProbabilityVector;
            private bool modelProbabilityVectorIsIpdated;
            private double[] observationProbabilityVector;
            private bool observationProbabilityVectorIsIpdated;
            //logarithms
            private double[] modelLogarithmicProbabilityVector;
            private bool modelLogarithmicProbabilityVectorIsIpdated;
            private double[] observationLogarithmicProbabilityVector;
            private bool observationLogarithmicProbabilityVectorIsIpdated;
            //counter
            private ulong measurementCount;
            private bool measurementCountIsIpdated;


            private double ModelMultiplicator { get { return this.MeasurementCount/(Fingerprint.FINGERPRINT_LENGTH+(double)this.MeasurementCount); } }
            private double ModelIncrement { get { return 1.0/(Fingerprint.FINGERPRINT_LENGTH+(double)this.MeasurementCount); } }
            private double ObservationMultiplicator { get { return this.MeasurementCount/(1.0+(double)this.MeasurementCount); } }
            private double ObservationIncrement { get { return (1.0/Fingerprint.FINGERPRINT_LENGTH)/(1.0+(double)this.MeasurementCount); } }

            internal double[] ProbabilityVector {
                get {
                    if(!this.fingerprintProbabilityVectorIsIpdated)
                        this.UpdateFingerprintProbabilityVector();
                    return this.fingerprintProbabilityVector;
                }
            }
            internal double[] ModelProbabilityVector {
                get {
                    if(!this.modelProbabilityVectorIsIpdated)
                        this.UpdateModelProbabilityVector();
                    return this.modelProbabilityVector;
                }
            }
            internal double[] ObservationProbabilityVector {
                get {
                    if(!this.observationProbabilityVectorIsIpdated)
                        this.UpdateObservationProbabilityVector();
                    return this.observationProbabilityVector;
                }
            }
            
            internal double[] ModelLogarithmicProbabilityVector {
                get {
                    if(!this.modelLogarithmicProbabilityVectorIsIpdated)
                        this.UpdateModelLogarithmicProbabilityVector();
                    return this.modelLogarithmicProbabilityVector;
                }
            }
            internal double[] ObservationLogarithmicProbabilityVector {
                get {
                    if(!this.observationLogarithmicProbabilityVectorIsIpdated)
                        this.UpdateObservationLogarithmicProbabilityVector();
                    return this.observationLogarithmicProbabilityVector;
                }
            }
             
            internal ulong MeasurementCount {
                get {
                    if(!this.measurementCountIsIpdated) {
                        //ulong nMeasurements=0;
                        this.measurementCount=0;
                        foreach(long l in this.fingerprintCounterVector)
                            this.measurementCount+=(ulong)l;
                        this.measurementCountIsIpdated=true;
                    }
                    return this.measurementCount;
                }
            }

            internal Fingerprint(long[] fingerprintCounterVector)
                : this() {
                if(fingerprintCounterVector.Length!=FINGERPRINT_LENGTH)
                    throw new Exception("Wrong length of fingerprintRawData");
                this.fingerprintCounterVector=fingerprintCounterVector;
            }
            internal Fingerprint(double[] fingerprintProbabilityDistributionVector, ulong measurementCount)
                : this() {
                if(fingerprintProbabilityDistributionVector.Length!=FINGERPRINT_LENGTH)
                    throw new Exception("Wrong length of fingerprintProbabilityData");

                //this.fingerprintProbabilityDataIsIpdated=false;
                //this.measurementCountIsIpdated=false;
                if(measurementCount>long.MaxValue/2)
                    measurementCount=long.MaxValue/2;
                fingerprintProbabilityDistributionVector.CopyTo(this.fingerprintProbabilityVector, 0);
                for(int i=0; i<FINGERPRINT_LENGTH; i++)
                    fingerprintCounterVector[i]=(long)(fingerprintProbabilityDistributionVector[i]*measurementCount);
                this.fingerprintProbabilityVectorIsIpdated=true;
            }
            internal Fingerprint() {
                this.FingerprintCounterVectorChanged();
                //this.fingerprintProbabilityDataIsIpdated=false;
                //this.measurementCountIsIpdated=false;
                this.measurementCount=0;
                this.fingerprintProbabilityVector=new double[FINGERPRINT_LENGTH];
                this.fingerprintCounterVector=new long[FINGERPRINT_LENGTH];
                //this.fingerprintCounterVector=null;

                this.modelProbabilityVector=null;//to save some memory
                //this.modelProbabilityVectorIsIpdated=false;
                this.observationProbabilityVector=null;
                //this.observationProbabilityVectorIsIpdated=false;
                this.modelLogarithmicProbabilityVector=null;
                this.observationLogarithmicProbabilityVector=null;
            }

            internal void Clear(){
                this.FingerprintCounterVectorChanged();

                this.fingerprintProbabilityVector=new double[FINGERPRINT_LENGTH];
                //this.fingerprintProbabilityVector=null;
                this.fingerprintCounterVector=new long[FINGERPRINT_LENGTH];
            }

            internal Fingerprint MergeWith(Fingerprint otherFingerprint) {
                if(this.fingerprintCounterVector.Length!=otherFingerprint.fingerprintCounterVector.Length)
                    throw new Exception("Fingerprint lengths do not match!");

                long[] newCounterVector=(long[])this.fingerprintCounterVector.Clone();

                ulong otherCount=otherFingerprint.MeasurementCount;
                for(int i=0; i<this.fingerprintCounterVector.Length; i++){
                    newCounterVector[i]+=(long)(otherCount*otherFingerprint.ProbabilityVector[i]);
                }

                return new Fingerprint(newCounterVector);
            }

            private void UpdateFingerprintProbabilityVector() {
                ulong fingerprintCount=this.MeasurementCount;

                lock(this.fingerprintProbabilityVector.SyncRoot) {
                    lock(this.fingerprintCounterVector.SyncRoot) {//Lets hope we don't get any deadlocks here!
                        this.fingerprintProbabilityVectorIsIpdated=false;
                        
                        if(fingerprintCount==0) {
                            System.Diagnostics.Debug.WriteLine("Fingerprint has no data");
                            for(int i=0; i<FINGERPRINT_LENGTH; i++)
                                fingerprintProbabilityVector[i]=1.0/FINGERPRINT_LENGTH;
                        }
                        else {
                            double factor=((double)1)/fingerprintCount;
                            for(int i=0; i<FINGERPRINT_LENGTH; i++)
                                fingerprintProbabilityVector[i]=factor*fingerprintCounterVector[i];
                        }
                        this.fingerprintProbabilityVectorIsIpdated=true;
                    }
                }
            }
            private void UpdateModelProbabilityVector() {
                this.modelProbabilityVectorIsIpdated=false;

                double mult=this.ModelMultiplicator;
                double inc=this.ModelIncrement;
                if(this.modelProbabilityVector==null)
                    this.modelProbabilityVector=new double[FINGERPRINT_LENGTH];
                for(int i=0; i<FINGERPRINT_LENGTH; i++) {
                    this.modelProbabilityVector[i]=this.ProbabilityVector[i]*mult+inc;
                }
                this.modelProbabilityVectorIsIpdated=true;
            }
            private void UpdateObservationProbabilityVector() {
                this.observationProbabilityVectorIsIpdated=false;

                double mult=this.ObservationMultiplicator;
                double inc=this.ObservationIncrement;
                if(this.observationProbabilityVector==null)
                    this.observationProbabilityVector=new double[FINGERPRINT_LENGTH];
                for(int i=0; i<FINGERPRINT_LENGTH; i++) {
                    this.observationProbabilityVector[i]=this.ProbabilityVector[i]*mult+inc;
                }
                this.observationProbabilityVectorIsIpdated=true;
            }
            private void UpdateModelLogarithmicProbabilityVector() {
                this.modelLogarithmicProbabilityVectorIsIpdated=false;
                if(this.modelLogarithmicProbabilityVector==null)
                    this.modelLogarithmicProbabilityVector=new double[FINGERPRINT_LENGTH];
                for(int i=0; i<FINGERPRINT_LENGTH; i++) {
                    this.modelLogarithmicProbabilityVector[i]=Math.Log(this.ModelProbabilityVector[i], 2.0);
                }
                this.modelLogarithmicProbabilityVectorIsIpdated=true;
            }
            private void UpdateObservationLogarithmicProbabilityVector() {
                this.observationLogarithmicProbabilityVectorIsIpdated=false;
                if(this.observationLogarithmicProbabilityVector==null)
                    this.observationLogarithmicProbabilityVector=new double[FINGERPRINT_LENGTH];
                for(int i=0; i<FINGERPRINT_LENGTH; i++) {
                    this.observationLogarithmicProbabilityVector[i]=Math.Log(this.ObservationProbabilityVector[i], 2.0);
                }
                this.observationLogarithmicProbabilityVectorIsIpdated=true;
            }

            internal void IncrementFingerprintCounterAtIndex(int index) {
                this.FingerprintCounterVectorChanged();

                System.Threading.Interlocked.Increment(ref fingerprintCounterVector[index]);

                if((ulong)fingerprintCounterVector[index]>=ulong.MaxValue/FINGERPRINT_LENGTH) {
                    //divide all values by 2
                    lock(this.fingerprintCounterVector.SyncRoot) {
                        for(int i=0; i<FINGERPRINT_LENGTH; i++)
                            fingerprintCounterVector[i]/=2;//Is this OK notation?
                    }
                }
            }

            private void FingerprintCounterVectorChanged() {
                this.measurementCountIsIpdated=false;
                this.fingerprintProbabilityVectorIsIpdated=false;
                this.modelProbabilityVectorIsIpdated=false;
                this.observationProbabilityVectorIsIpdated=false;
                this.modelLogarithmicProbabilityVectorIsIpdated=false;
                this.observationLogarithmicProbabilityVectorIsIpdated=false;
            }

        }
    }
}
