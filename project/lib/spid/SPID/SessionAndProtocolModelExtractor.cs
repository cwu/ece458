using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SPID {
    class SessionAndProtocolModelExtractor {
        
        delegate void EmptyDelegateCallback();

        private SortedList<string, ProtocolIdentification.ProtocolModel> protocolModels;
        private Type packetBaseType;
        private PcapFileHandler.PcapFileReader observationReader;
        private SessionHandler sessionHandler;

        private Queue<PacketParser.Frame> frameQueue;//~1kB per item
        private Queue<KeyValuePair<ISession, ProtocolIdentification.ProtocolModel>> completedProtocolModelsQueue;//30*256*(8+8)=~120kB per item
        private Queue<System.Windows.Forms.ListViewItem> listViewItemQueue;//~?kB per item... 1kB?
        private int frameQueueMaxSize, completedProtocolModelsQueueMaxSize, listViewItemQueueMaxSize;
        

        private LoadingProcess loadingProcess;
        private BackgroundWorker backgroundFileLoader, backgroundFrameToSessionAdder, backgroundListViewItemCreator;
        //private bool backgroundFileLoaderCompleted, backgroundFrameToSessionAdderCompleted;
        private Configuration config;

        public SessionAndProtocolModelExtractor(string pcapFileName, SortedList<string, ProtocolIdentification.ProtocolModel> protocolModels, Configuration config) {
            //this.parentForm=parentForm;
            this.config=config;
            this.protocolModels=protocolModels;

            this.observationReader=new PcapFileHandler.PcapFileReader(pcapFileName,4000, null);
            
            /**
             * 10000 => more than 400MB (RAM is cached)
             * 1000  => OK, 48,1% RAM usage (30% is base load?)
             * */
            this.sessionHandler=new SessionHandler(config.MaxSimultaneousSessions, config);//1000 parallel sessions is a good value
            this.sessionHandler.SessionProtocolModelCompleted+=new SessionHandler.SessionProtocolModelCompletedEventHandler(sessionHandler_SessionProtocolModelCompleted);

            this.frameQueueMaxSize=10000;//10MB max
            this.frameQueue=new Queue<PacketParser.Frame>(frameQueueMaxSize);
            this.completedProtocolModelsQueueMaxSize=100;//12MB max
            this.completedProtocolModelsQueue=new Queue<KeyValuePair<ISession, ProtocolIdentification.ProtocolModel>>();
            this.listViewItemQueueMaxSize=1000;//10MB max
            this.listViewItemQueue=new Queue<System.Windows.Forms.ListViewItem>(listViewItemQueueMaxSize);

            this.packetBaseType=null;
            if(observationReader.FileDataLinkType==PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_ETHERNET)
                this.packetBaseType=typeof(PacketParser.Packets.Ethernet2Packet);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_RAW_IP || observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_RAW_IP_2 || observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_RAW_IP_3)
                this.packetBaseType=typeof(PacketParser.Packets.IPv4Packet);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_IEEE_802_11)
                this.packetBaseType=typeof(PacketParser.Packets.IEEE_802_11Packet);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_IEEE_802_11_WLAN_RADIOTAP)
                this.packetBaseType=typeof(PacketParser.Packets.IEEE_802_11RadiotapPacket);
            else if(observationReader.FileDataLinkType== PcapFileHandler.PcapFileReader.DataLinkType.WTAP_ENCAP_CHDLC)
                this.packetBaseType=typeof(PacketParser.Packets.CiscoHdlcPacket);
            else
                throw new Exception("No packet type found for "+observationReader.FileDataLinkType.ToString());

            loadingProcess=new LoadingProcess(new System.IO.FileInfo(this.observationReader.Filename).Name);
            loadingProcess.Show();
            loadingProcess.Update();

            //backgroundFileLoader
            backgroundFileLoader=new BackgroundWorker();
            loadingProcess.AddBackgroundWorker(backgroundFileLoader);
            backgroundFileLoader.DoWork+=new DoWorkEventHandler(backgroundFileLoader_DoWork);
            backgroundFileLoader.WorkerSupportsCancellation=true;
            //backgroundFileLoader.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(backgroundFileLoader_RunWorkerCompleted);
            backgroundFileLoader.RunWorkerAsync();

            System.Windows.Forms.Application.DoEvents();

            //backgroundFrameToSessionAdder
            backgroundFrameToSessionAdder=new BackgroundWorker();
            loadingProcess.AddBackgroundWorker(backgroundFrameToSessionAdder);
            backgroundFrameToSessionAdder.DoWork+=new DoWorkEventHandler(backgroundFrameToSessionAdder_DoWork);
            backgroundFrameToSessionAdder.WorkerSupportsCancellation=true;
            //backgroundFrameToSessionAdder.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(backgroundFrameToSessionAdder_RunWorkerCompleted);
            backgroundFrameToSessionAdder.RunWorkerAsync();

            System.Windows.Forms.Application.DoEvents();

            //backgroundListViewItemCreator
            this.backgroundListViewItemCreator=new BackgroundWorker();
            loadingProcess.AddBackgroundWorker(this.backgroundListViewItemCreator);
            backgroundListViewItemCreator.DoWork+=new DoWorkEventHandler(backgroundListViewItemCreator_DoWork);
            backgroundListViewItemCreator.WorkerSupportsCancellation=true;
            backgroundListViewItemCreator.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(backgroundListViewItemCreator_RunWorkerCompleted);
            backgroundListViewItemCreator.RunWorkerAsync();


        }

        void backgroundListViewItemCreator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

            if(this.loadingProcess!=null && this.loadingProcess.Visible && !this.backgroundFileLoader.IsBusy && !this.backgroundFrameToSessionAdder.IsBusy) {
                loadingProcess.Invoke(new EmptyDelegateCallback(loadingProcess.CloseOrHide));
                    /*if(this.loadingProcess!=null)
                        loadingProcess.Invoke(new EmptyDelegateCallback(loadingProcess.Hide));
                     * */
                //}
            }
            
        }

        void backgroundListViewItemCreator_DoWork(object sender, DoWorkEventArgs e) {

            int bufferUsage=0;
            foreach(KeyValuePair<ISession, ProtocolIdentification.ProtocolModel> sessionAndProtocolModelPair in GetSessionAndProtocolModelPairs()) {
                if(this.backgroundListViewItemCreator.CancellationPending)
                    break;
                ISession session=sessionAndProtocolModelPair.Key;
                ProtocolIdentification.ProtocolModel observationModel=sessionAndProtocolModelPair.Value;
                if(observationModel!=null && observationModel.ObservationCount>0) {
                    System.Windows.Forms.ListViewItem item=GetSessionListViewItem(session, observationModel, config.DisplayAllProtocolModelDivergences);
                    this.listViewItemQueue.Enqueue(item);
                }

                if(10*(this.completedProtocolModelsQueue.Count*10/completedProtocolModelsQueueMaxSize)!=bufferUsage) {
                    bufferUsage=10*(this.completedProtocolModelsQueue.Count*10/completedProtocolModelsQueueMaxSize);
                    
                    if(!loadingProcess.IsAborted) {
                        try {
                            loadingProcess.Invoke((EmptyDelegateCallback)delegate() { loadingProcess.CompletedProtocolModelsPercent=bufferUsage; });
                            loadingProcess.Invoke(new EmptyDelegateCallback(loadingProcess.Update));
                        }
                        catch { }
                    }
                }

            }

        }


        void backgroundFrameToSessionAdder_DoWork(object sender, DoWorkEventArgs e) {
            int bufferUsage=0;
            int millisecondsToSleep=1;
            ISession session;
            PacketParser.Frame frame;
            while(this.backgroundFileLoader.IsBusy || this.frameQueue.Count>0) {
                if(this.backgroundFrameToSessionAdder.CancellationPending)
                    break;
                if(this.frameQueue.Count>0 && this.completedProtocolModelsQueue.Count<this.completedProtocolModelsQueueMaxSize) {
                    millisecondsToSleep=1;
                    lock(frameQueue) {
                        frame=frameQueue.Dequeue();
                    }
                    if(this.sessionHandler.TryGetSession(frame, out session)) {
                        session.AddFrame(frame);
                    }
                }
                else {
                    System.Threading.Thread.Sleep(millisecondsToSleep);
                    millisecondsToSleep=Math.Min(2*millisecondsToSleep, 5000);
                }

                if(10*(this.frameQueue.Count*10/frameQueueMaxSize)!=bufferUsage) {
                    bufferUsage=10*(this.frameQueue.Count*10/frameQueueMaxSize);
                    
                    if(!loadingProcess.IsAborted) {
                        try {
                            loadingProcess.Invoke((EmptyDelegateCallback)delegate() { loadingProcess.BufferPercent=bufferUsage; });
                            loadingProcess.Invoke(new EmptyDelegateCallback(loadingProcess.Update));
                        }
                        catch { }
                    }
                }
            }
        }


        void backgroundFileLoader_DoWork(object sender, DoWorkEventArgs e) {
            //LoadingProcess lp=(LoadingProcess)e.Argument;
            int nFramesReceived=0;
            int percentRead=0;

            foreach(PcapFileHandler.PcapPacket packet in observationReader.PacketEnumerator()) {
                if(this.backgroundFileLoader.CancellationPending) {
                    //e.Cancel=true;
                    break;//stop loading more data
                }
                int millisecondsToSleep=1;
                while(this.frameQueue.Count>=frameQueueMaxSize) {
                    System.Threading.Thread.Sleep(millisecondsToSleep);
                    millisecondsToSleep=Math.Min(2*millisecondsToSleep, 5000);

                    if(this.backgroundFileLoader.CancellationPending) {
                        break;
                    }
                }
                
                lock(this.frameQueue) {
                    this.frameQueue.Enqueue(new PacketParser.Frame(packet.Timestamp, packet.Data, packetBaseType, ++nFramesReceived, false));
                }

                if(observationReader.PercentRead!=percentRead) {
                    percentRead=observationReader.PercentRead;
                    
                    if(!loadingProcess.IsAborted) {
                        try {
                            loadingProcess.Invoke((EmptyDelegateCallback)delegate() { loadingProcess.FilePercent=percentRead; });
                            loadingProcess.Invoke(new EmptyDelegateCallback(loadingProcess.Update));
                        }
                        catch { }
                    }
                }


            }
            System.Threading.Thread.Sleep(100);

            //this.backgroundFileLoaderCompleted=true;
        }

        public IEnumerable<System.Windows.Forms.ListViewItem[]> GetSessionListViewItemArrays() {

            while(!this.backgroundListViewItemCreator.CancellationPending && (this.listViewItemQueue.Count>0 || this.backgroundFileLoader.IsBusy || this.backgroundFrameToSessionAdder.IsBusy || this.backgroundListViewItemCreator.IsBusy)) {

                if(this.listViewItemQueue.Count>0) {
                    System.Windows.Forms.ListViewItem[] returnArray=new System.Windows.Forms.ListViewItem[this.listViewItemQueue.Count];
                    for(int i=0;i<returnArray.Length;i++)
                        returnArray[i]=this.listViewItemQueue.Dequeue();
                    yield return returnArray;
                }
                else
                    System.Windows.Forms.Application.DoEvents();
            }
        }
        public IEnumerable<System.Windows.Forms.ListViewItem> GetSessionListViewItems() {

            //while(!this.backgroundListViewItemCreator.CancellationPending && (this.listViewItemQueue.Count>0 || this.backgroundFileLoader.IsBusy || this.backgroundFrameToSessionAdder.IsBusy || this.backgroundListViewItemCreator.IsBusy)) {
            while(this.listViewItemQueue.Count>0 || this.backgroundFileLoader.IsBusy || this.backgroundFrameToSessionAdder.IsBusy || this.backgroundListViewItemCreator.IsBusy) {

                if(this.listViewItemQueue.Count>0) {
                    yield return this.listViewItemQueue.Dequeue();
                }
                else
                    System.Windows.Forms.Application.DoEvents();
            }
        }

        public IEnumerable<System.Collections.Generic.KeyValuePair<ISession, ProtocolIdentification.ProtocolModel>> GetSessionAndProtocolModelPairs() {


            int millisecondsToSleep=1;

            //I'm checking millisecondsToSleep in the wile loop in order to give the SessionHandler a chance
            //to generate an event when a new session protocol model is completed and enqueued
            while(/*millisecondsToSleep<100 || */this.backgroundFileLoader.IsBusy || this.backgroundFrameToSessionAdder.IsBusy || this.completedProtocolModelsQueue.Count>0) {
                if(this.completedProtocolModelsQueue.Count>0 && this.listViewItemQueue.Count<this.listViewItemQueueMaxSize) {
                    lock(this.completedProtocolModelsQueue) {
                        yield return this.completedProtocolModelsQueue.Dequeue();
                    }
                    millisecondsToSleep=1;
                }
                else {
                    System.Threading.Thread.Sleep(millisecondsToSleep);
                    millisecondsToSleep=Math.Min(2*millisecondsToSleep, 5000);
                    
                }
            }
            foreach(ISession session in sessionHandler.GetSessionsWithoutCompletedProtocolModels())
                yield return new System.Collections.Generic.KeyValuePair<ISession, ProtocolIdentification.ProtocolModel>(session, session.ApplicationProtocolModel);
        }

        void sessionHandler_SessionProtocolModelCompleted(ISession session, ProtocolIdentification.ProtocolModel protocolModel) {
            //this.completedProtocolModels.Add(session, protocolModel);
            lock(this.completedProtocolModelsQueue) {
                this.completedProtocolModelsQueue.Enqueue(new KeyValuePair<ISession, ProtocolIdentification.ProtocolModel>(session, protocolModel));
            }
        }

        private System.Windows.Forms.ListViewItem GetSessionListViewItem(ISession session, ProtocolIdentification.ProtocolModel observationModel, bool addProtocolModelDivergencesTag) {
            System.Windows.Forms.ListViewItem item=new System.Windows.Forms.ListViewItem(session.ClientIP.ToString());
            item.SubItems.Add(session.TransportProtocol.ToString()+" "+session.ClientPort.ToString());
            item.SubItems.Add(session.ServerIP.ToString());
            item.SubItems.Add(session.TransportProtocol.ToString()+" "+session.ServerPort.ToString());
            //check format for the timstamp string
            item.SubItems.Add(config.FormatDateTime(session.FirstPacketTimestamp));
            if(observationModel==null)
                item.SubItems.Add("0");
            else
                item.SubItems.Add(observationModel.ObservationCount.ToString());
            if(observationModel!=null && observationModel.ObservationCount>0) {
                ProtocolIdentification.ProtocolModel bestProtocolMatch=GetBestProtocolMatch(observationModel, this.protocolModels);
                if(bestProtocolMatch!=null)
                    item.SubItems.Add(bestProtocolMatch.ProtocolName).Name="protocol";
            }

            if(addProtocolModelDivergencesTag) {
                //this can be removed to improve performace (double speed)
                SortedList<string, double> protocolModelDivergences=new SortedList<string, double>(this.protocolModels.Count);
                foreach(ProtocolIdentification.ProtocolModel m in this.protocolModels.Values)
                    protocolModelDivergences.Add(m.ProtocolName, observationModel.GetAverageKullbackLeiblerDivergenceFrom(m));
                item.Tag=protocolModelDivergences;
            }

            return item;
        }

        private ProtocolIdentification.ProtocolModel GetBestProtocolMatch(ProtocolIdentification.ProtocolModel observationModel, SortedList<string, ProtocolIdentification.ProtocolModel> protocolModels) {
            ProtocolIdentification.ProtocolModel bestProtocolMatch=null;
            double bestProtocolMatchDivergence=config.DivergenceThreshold;//the highest allowed distance for a valid protocol model match
            foreach(ProtocolIdentification.ProtocolModel protocolModel in this.protocolModels.Values) {
                double divergence=observationModel.GetAverageKullbackLeiblerDivergenceFrom(protocolModel);
                if(divergence<bestProtocolMatchDivergence) {
                    bestProtocolMatch=protocolModel;
                    bestProtocolMatchDivergence=divergence;
                }
            }

            //just for test
            //ShowProtocolModelDivergences(observationModel, bestProtocolMatch);


            return bestProtocolMatch;
        }
    }
}
