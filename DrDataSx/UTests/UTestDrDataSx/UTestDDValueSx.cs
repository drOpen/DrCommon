using System;

namespace DrOpen.DrCommon.DrData
{
    public static class DDSchema
    {
        #region Serialize xml
        public const string XML_SERIALIZE_NODE_VALUE = "v";
        public const string XML_SERIALIZE_NODE = "n";
        public const string XML_SERIALIZE_NODE_ATTRIBUTE_COLLECTION = "ac";
        public const string XML_SERIALIZE_NODE_ATTRIBUTE = "a";
        public const string XML_SERIALIZE_NODE_ARRAY_VALUE_ITEM = "i";
        public const string XML_SERIALIZE_ATTRIBUTE_NAME = "n";
        public const string XML_SERIALIZE_ATTRIBUTE_TYPE = "t";
        public const string XML_SERIALIZE_ATTRIBUTE_ROOT = "r";
        public const string XML_SERIALIZE_ATTRIBUTE_SIZE = "s";
        public const string XML_SERIALIZE_ATTRIBUTE_CHILDREN_COUNT = "c";
        public const string XML_SERIALIZE_ATTRIBUTE_CHILDREN_ATTRIBUTE_COUNT = "a";

        public const string XML_SERIALIZE_VALUE_TYPE_NULL = "null";

        #endregion Serialize xml
        #region Serialize bin
        public const string SERIALIZE_ATTRIBUTE_NAME = "n";
        public const string SERIALIZE_NODE_ATTRIBUTE_COLLECTION = "ac";
        public const string SERIALIZE_NODE_ARRAY_VALUE_ITEM = "i";
        public const string SERIALIZE_NODE_ATTRIBUTE = "a";
        public const string SERIALIZE_ATTRIBUTE_TYPE = "t";
        public const string SERIALIZE_ATTRIBUTE_CHILDREN_COUNT = "c";

        #endregion Serialize bin

        #region string format
        public const string StringDateTimeFormat = "o"; //ISO 8601 format
        public const string StringRoundTripFormat = "r"; //round-trip format for Single, Double, and BigInteger types.
        #endregion string format
    }
}
