using System;

namespace DungeonRush.Stats.Data
{
    public enum ValueType
    {
        Flat,
        Percent
    }

    [Serializable]
    public class ParamModification
    {
        public string ParamName;
        public ValueType ValueType;
        public float Value;

        public ParamModification(string paramName, ValueType valueType, float value)
        {
            ParamName = paramName;
            ValueType = valueType;
            Value = value;
        }
    }
}
