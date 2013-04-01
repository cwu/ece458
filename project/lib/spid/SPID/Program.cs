//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SPID {
    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            /**
             * Read the arguments (args) to enable the user to select config XML-file, pcap-fil
             * Även ett argument för att stänga av programmet efter körningen
             */
            string[] args = Environment.GetCommandLineArgs();
            bool autimaticExit = false;
            List<string> pcapFilenames = new List<string>();
            List<string> configFilenames = new List<string>();
            System.IO.FileInfo fi=new System.IO.FileInfo(Application.ExecutablePath);
            //configFileName=fi.DirectoryName+"\\config.xml";
            string defaultConfigFilename = fi.DirectoryName+System.IO.Path.DirectorySeparatorChar+"config.xml";
            string databaseFilename = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)+System.IO.Path.DirectorySeparatorChar+"DefaultProtocolModelDatabase.xml";
            string traningDataDirectory = null;
            bool pcapDirIsOutputDir=false;

            for (int i=1; i<args.Length; i++) {
                if(args[i].Equals("-x", StringComparison.InvariantCultureIgnoreCase)) {
                    autimaticExit = true;
                }
                else if(args[i].Equals("-p", StringComparison.InvariantCultureIgnoreCase)) {
                    pcapFilenames.Add(args[++i]);
                }
                else if(args[i].Equals("-pd", StringComparison.InvariantCultureIgnoreCase)) {
                    foreach(string filename in System.IO.Directory.GetFiles(args[++i]))
                        pcapFilenames.Add(filename);
                }
                else if(args[i].Equals("-c", StringComparison.InvariantCultureIgnoreCase)) {
                    configFilenames.Add(args[++i]);
                }
                    else if(args[i].Equals("-cd", StringComparison.InvariantCultureIgnoreCase)) {
                    foreach(string filename in System.IO.Directory.GetFiles(args[++i]))
                        configFilenames.Add(filename);
                }
                else if(args[i].Equals("-d", StringComparison.InvariantCultureIgnoreCase)) {
                    databaseFilename = args[++i];
                }
                else if(args[i].Equals("-op", StringComparison.InvariantCultureIgnoreCase)) {
                    pcapDirIsOutputDir=true;
                }
                else if(args[i].Equals("-td", StringComparison.InvariantCultureIgnoreCase)) {
                    traningDataDirectory = args[++i];
                }
                else {
                    Console.Error.WriteLine("Usage: SPID [OPTION]...");
                    Console.Error.WriteLine("");
                    Console.Error.WriteLine("Options:");
                    Console.Error.WriteLine("-c <config_filename> : Load configuration XML file");
                    Console.Error.WriteLine("-cd <directory_name> : Load all files in directory as configuration files");
                    Console.Error.WriteLine("-d <database_filename> : Load protocol model database XML file");
                    Console.Error.WriteLine("-p <pcap_filename> : Load packet capture file (pcap)");
                    Console.Error.WriteLine("-pd <directory_name> : Load all files in directory as packet capture files");
                    Console.Error.WriteLine("-op : Place all output txt files in same directory as respective pcap files");
                    Console.Error.WriteLine("-td <directory_name> : Load training data directory to convert into XML database.");
                    Console.Error.WriteLine("-x : Exit application automatically after execution");
                    Console.Error.WriteLine("");
                    Console.Error.WriteLine("Tip: You can specify multiple configuration or pcap files by using their respective switches multiple times.");
                    Console.Error.WriteLine("Example 1: SPID -p packetdump1.pcap -p packetdump2.pcap -x");
                    Console.Error.WriteLine("Example 2: SPID -cd config_dir -pd pcap_dir1 -pd pcap_dir2 -op -x");
                    Console.Error.WriteLine("Example 3: SPID -td training_data_dir -x");
                    
                    return;
                }

            }

            if(configFilenames.Count == 0)
                configFilenames.Add(defaultConfigFilename);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SpidForm2 spidForm = new SpidForm2(configFilenames, databaseFilename, pcapFilenames, autimaticExit, Console.Out, pcapDirIsOutputDir, traningDataDirectory);

            Application.Run(spidForm);
            Console.WriteLine("Closing SPID...");
        }

    }
}