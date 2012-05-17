using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpClient.JSON_Net
{
    public class Command
    {
        public string CommandName { get; private set; }
        public Int32 Region { get; private set; }
        public string VideoFileName { get; private set; }
        public string AudioFileName0 { get; private set; }
        public string AudioFileName1 { get; private set; }
        public string AudioFileName2 { get; private set; }
        public string AudioFileName3 { get; private set; }
        public string VbiFileName { get; private set; }
        public Boolean UseTDIR { get; private set; }
        public UInt32 InitialFrame { get; private set; }
        public double InitialRate { get; private set; }
        public Boolean Loop { get; private set; }

        public Command(
            string command,
            Int32 region,
            string videoFileName,
            string audioFileName0,
            string audioFileName1,
            string audioFileName2,
            string audioFileName3,
            string vbiFileName,
            Boolean useTDIR,
            UInt32 initialFrame,
            double initialRate,
            Boolean loop)
        {
            CommandName = command;
            Region = region;
            VideoFileName = videoFileName;
            AudioFileName0 = audioFileName0;
            AudioFileName1 = audioFileName1;
            AudioFileName2 = audioFileName2;
            AudioFileName3 = audioFileName3;
            VbiFileName = vbiFileName;
            UseTDIR = useTDIR;
            InitialFrame = initialFrame;
            InitialRate = initialRate;
            Loop = loop;
        }
    }
}
