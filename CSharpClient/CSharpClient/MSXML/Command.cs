using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpClient.MSXML
{
    [Serializable]
    public class Command
    {
        public string CommandName { get; set; }
        public Int32 Region { get; set; }
        public string VideoFileName { get; set; }
        public string AudioFileName0 { get; set; }
        public string AudioFileName1 { get; set; }
        public string AudioFileName2 { get; set; }
        public string AudioFileName3 { get; set; }
        public string VbiFileName { get; set; }
        public Boolean UseTDIR { get; set; }
        public UInt32 InitialFrame { get; set; }
        public double InitialRate { get; set; }
        public Boolean Loop { get; set; }


        public Command() { }

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
