namespace AoMMaxPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public static class CheckedListBoxExtensions
    {
        public static void SetEnum<T>(this CheckedListBox box, T enumVal)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            uint eVal = (uint)Convert.ChangeType(enumVal, typeof(uint));
            for (int i = 0; i < box.Items.Count; i++)
            {
                uint prop = (uint)Convert.ChangeType(box.Items[i], typeof(uint));
                if ((eVal & prop) == prop)
                {
                    box.SetItemChecked(i, true);
                }
                else
                {
                    box.SetItemChecked(i, false);
                }
            }
        }
        public static T GetEnum<T>(this CheckedListBox box)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            uint enumVal = 0;
            for (int i = 0; i < box.CheckedItems.Count; i++)
            {
                enumVal |= (uint)Convert.ChangeType(box.CheckedItems[i], typeof(uint));
            }

            return MiscUtil.Operator.Convert<uint, T>(enumVal);
        }
    }
}
