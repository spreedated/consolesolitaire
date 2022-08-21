using ProtoBuf;

namespace ConsoleSolitaire.Models.Protobuf
{
    [ProtoContract]
    public class SaveState
    {
        [ProtoMember(1)]
        public string Pile1 { get; set; }
        [ProtoMember(2)]
        public string Pile2 { get; set; }
        [ProtoMember(3)]
        public string Pile3 { get; set; }
        [ProtoMember(4)]
        public string Pile4 { get; set; }
        [ProtoMember(5)]
        public string Pile5 { get; set; }
        [ProtoMember(6)]
        public string Pile6 { get; set; }
        [ProtoMember(7)]
        public string Pile7 { get; set; }
        [ProtoMember(8)]
        public string BuildingPile1 { get; set; }
        [ProtoMember(9)]
        public string BuildingPile2 { get; set; }
        [ProtoMember(10)]
        public string BuildingPile3 { get; set; }
        [ProtoMember(11)]
        public string BuildingPile4 { get; set; }
        [ProtoMember(12)]
        public string Talon { get; set; }
    }
}
