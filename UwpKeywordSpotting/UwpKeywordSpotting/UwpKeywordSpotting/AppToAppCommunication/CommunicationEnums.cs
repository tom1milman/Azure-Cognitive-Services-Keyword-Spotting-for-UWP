using System;
using System.Collections.Generic;
using System.Text;

namespace UwpKeywordSpotting.AppToAppCommunication
{
    public enum CommunicationEnums
    {
        Request,
        Speech,
        TurnKwsOn,
        TurnKwsOff,
        GuiOn,
        NULL
    }

    /// <summary>
    /// Extention methods for the enums
    /// </summary>
    public static class CommunicationEnumsExtention
    {

        /// <summary>
        /// Converts the given string into its enum
        /// </summary>
        /// <param name="text">String</param>
        /// <returns>CommunicationEnums</returns>
        public static CommunicationEnums StringToEnum(String text)
        {
            foreach (CommunicationEnums comEnum in Enum.GetValues(typeof(CommunicationEnums)))
            {
                if (text.Equals(comEnum.ToString()))
                    return comEnum;
            }

            return CommunicationEnums.NULL;
        }

        /// <summary>
        /// Gets the enum out of a given keys of a ValueSet
        /// </summary>
        /// <param name="set">ValueSet.Keys</param>
        /// <returns>CommunicationEnums</returns>
        public static CommunicationEnums GetEnumFromValueSet(ICollection<string> set)
        {
            foreach (CommunicationEnums comEnum in Enum.GetValues(typeof(CommunicationEnums)))
            {
                if (set.Contains(comEnum.ToString()))
                {
                    return comEnum;
                }
            }

            return CommunicationEnums.NULL;
        }
    }
}
