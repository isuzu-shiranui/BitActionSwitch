namespace BitActionSwitch.Editor.Models
{
    internal static class ActionSwitchParameters
    {
        public const string PREFIX = "bas_";
        
        public static string GetObjectStatusLayerName(int objectNum) =>  $"{PREFIX}Object{objectNum.ToString()}Layer";

        public static string GetObjectActiveStatusParameterName(int objectNum) => $"{PREFIX}Is{objectNum.ToString()}Active";
    }
}