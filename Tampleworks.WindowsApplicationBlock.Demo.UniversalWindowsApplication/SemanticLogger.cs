using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.Tracing;
using Windows.Foundation.Diagnostics;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp
{
    public class SemanticLogger : ISemanticLogger
    {
        static LoggingChannel lc = new LoggingChannel("Robonest-App-Tracing-General", null, new Guid("{fefa1290-86b6-40e8-92f7-559b41e2b3e8}"));

        public void WriteUserPressedButton(string buttonName)
        {
            lc.LogMessage("I made a message!");

            var loggingFields = new LoggingFields();
            loggingFields.AddString("ButtonName", buttonName);
            lc.LogEvent(
                "WriteUserPressedButton",
                loggingFields,
                level: LoggingLevel.Verbose,
                options: new LoggingOptions
                {
                    Keywords = 123,
                    Opcode = LoggingOpcode.Send,
                    Tags = 2,
                    Task = 44,
                }
            );
        }
    }
}
