using System;

namespace DungeonRush.Stats.Data
{
    public enum ValueType
    {
        Set,
        Add,
        Mult
    }

    [Serializable]
    public class ParamModification
    {
        public string ParamName;
        public ValueType ValueType;
        public float ParamValue;
        public string ConditionType;
        public string ConditionValue;

        public ParamModification(
            string paramName,
            ValueType valueType,
            float paramValue,
            string conditionType = null,
            string conditionValue = null)
        {
            ParamName = paramName;
            ValueType = valueType;
            ParamValue = paramValue;
            ConditionType = conditionType;
            ConditionValue = conditionValue;
        }
    }
}
