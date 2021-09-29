using System.Collections.Generic;
using MagicSoftware.Integration.UserComponents;
using MagicSoftware.Integration.UserComponents.Interfaces;

namespace MagicSoftware.MQTT
{
    /// <summary>
    /// This class represents one checker result that is returned by the Magic xpi Checker.
    /// </summary>
    public class CheckerResult : ICheckerResult
    {
        private List<CheckerMessage> checkerResult = new List<CheckerMessage>();

        public CheckerResult()
        {

        }

        public List<CheckerMessage> checkerMessages
        {
            get { return checkerResult; }
        }

        public void AddCheckerMessage(CheckerMessage checkerMessage)
        {
            checkerResult.Add(checkerMessage);
        }
    }
}
