namespace DaemonMC.Network.RakNet
{
    public class ACKdata
    {
        public bool singleSequence { get; set; }
        public uint sequenceNumber { get; set; }
        public uint firstSequenceNumber { get; set; }
        public uint lastSequenceNumber { get; set; }
    }

}
