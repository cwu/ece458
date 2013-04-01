using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SPID {
    public partial class LoadingProcess : Form {

        //delegate void EmptyDelegateCallback();

        private int filePercent, bufferPercent, completedProtocolModelsPercent;
        //private PcapFileHandler.PcapFileReader pcapReader;
        private bool isAborted;

        //private BackgroundWorker fileWorker, bufferWorker;
        private List<BackgroundWorker> backgroundWorkers;

        internal delegate void SetLoadingProcessValue(int percent);
        internal delegate void ShowLoadingProcess();

        public bool IsAborted { get { return this.isAborted; } }
        //public PcapFileHandler.PcapFileReader PcapReader { get { return this.pcapReader; } }
        //public BackgroundWorker FileWorker { /*get { return this.fileWorker; }*/ set { this.fileWorker=value; } }
        //public BackgroundWorker BufferWorker { /*get { return this.bufferWorker; }*/ set { this.bufferWorker=value; } }

        internal int FilePercent {
            get { return this.filePercent; }
            set {
                if(value>=0 && value<=100) {
                    this.filePercent=value;
                    if(this.pcapFileProgressBar.Visible && this.pcapFileProgressBar.IsHandleCreated)
                        this.pcapFileProgressBar.Value=filePercent;
                }
            }
        }
        internal int BufferPercent {
            get { return this.bufferPercent; }
            set {
                if(value>=0 && value<=100) {
                    this.bufferPercent=value;
                    if(this.frameBufferProgressBar.Visible && this.frameBufferProgressBar.IsHandleCreated)
                        this.frameBufferProgressBar.Value=bufferPercent;
                }
            }
        }
        internal int CompletedProtocolModelsPercent {
            get { return this.completedProtocolModelsPercent; }
            set {
                if(value>=0 && value<=100) {
                    this.completedProtocolModelsPercent=value;
                    if(this.completedProtocolModelsPercentProgressBar.Visible && this.completedProtocolModelsPercentProgressBar.IsHandleCreated)
                        this.completedProtocolModelsPercentProgressBar.Value=completedProtocolModelsPercent;
                }
            }
        }

        public LoadingProcess(string filename) {
            InitializeComponent();
            this.Text="Loading "+filename;
            this.filePercent=0;
            this.bufferPercent=0;
            this.isAborted=false;
            this.backgroundWorkers=new List<BackgroundWorker>();
        }
        /*public LoadingProcess(PcapFileHandler.PcapFileReader pcapReader, string text) {
            InitializeComponent();

            this.pcapReader=pcapReader;
            this.Text=text;
            //this.textLabel.Text=text;
            this.pcapFileProgressBar.Value=0;
            this.percent=0;
            //this.percentLabel.Text=""+percent+" %";

            this.isAborted=false;
        }*/

        public void AddBackgroundWorker(BackgroundWorker bw) {
            this.backgroundWorkers.Add(bw);
        }


        private void LoadingProcess_FormClosing(object sender, FormClosingEventArgs e) {
            this.isAborted=true;
            //pcapReader.AbortFileRead();
            foreach(BackgroundWorker bw in this.backgroundWorkers) {
                if(bw!=null && !bw.CancellationPending)
                    bw.CancelAsync();
            }
            /*
             * if(this.fileWorker!=null && !this.fileWorker.CancellationPending)
                this.fileWorker.CancelAsync();
            if(this.bufferWorker!=null && !this.bufferWorker.CancellationPending)
                this.bufferWorker.CancelAsync();
             * */
        }

        public void CloseOrHide() {
            try {
                this.Close();
            }
            catch {
                this.Hide();
            }
        }

    }
}