using System;
using Godot;
using Godot.Collections;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class RaftWaterChallenge
    {
        public Func<BoxStack, bool> Condition { get; private set; }
        public string ConditionString { get; private set; }
        public ChallengeTextBoxType[] BoxTypes { get; private set; }

        public RaftWaterChallenge(Func<BoxStack, bool> condition, string conditionString, ChallengeTextBoxType[] boxTypes)
        {
            Condition = condition;
            ConditionString = conditionString;
            BoxTypes = boxTypes;
        }

        public RaftWaterChallenge(Dictionary<string, Variant> dict) : this(
            (boxStack) => boxStack.Count == (int)dict["Condition"],
            $"Count {(string)dict["Condition"]}",
            new ChallengeTextBoxType[4]
        )
        {
            string[] titles = (string[])dict["BoxTypes"];
            for (var i = 0; i < 4; i++)
            {
                var boxType = Enum.Parse<ChallengeTextBoxType>(titles[i]);
                BoxTypes[i] = boxType;
            }

            string[] conditionArr = ((string)dict["Condition"]).Split();
            string operation = conditionArr[0];
            string operandString = conditionArr[1];
            int operand = 0;
            if (operandString.Contains(" "))
            {
                Expression expr = new();
                expr.Parse(operandString);
                operand = (int)expr.Execute();
                GD.Print(operand);
            }
            else
            {
                operand = int.Parse(conditionArr[1]);
            }

            switch (operation)
            {
                case "==":
                    Condition = (boxStack) => boxStack.Count == operand;
                    break;
                case "!=":
                    Condition = (boxStack) => boxStack.Count != operand;
                    break;
                case ">":
                    Condition = (boxStack) => boxStack.Count > operand;
                    break;
                case "<":
                    Condition = (boxStack) => boxStack.Count < operand;
                    break;
                case ">=":
                    Condition = (boxStack) => boxStack.Count >= operand;
                    break;
                case "<=":
                    Condition = (boxStack) => boxStack.Count <= operand;
                    break;
                default:
                    break;
            }
        }
    }
}