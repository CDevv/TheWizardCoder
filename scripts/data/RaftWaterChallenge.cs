using System;
using Godot;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class RaftWaterChallenge
    {
        public Func<bool> Condition { get; private set; }
        public string ConditionString { get; private set; }
        public ChallengeTextBoxType[] BoxTypes { get; private set; }

        public RaftWaterChallenge(Func<bool> condition, string conditionString, ChallengeTextBoxType[] boxTypes)
        {
            Condition = condition;
            ConditionString = conditionString;
            BoxTypes = boxTypes;
        }
    }
}