namespace AoMMaxPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IModelMaxUI
    {
        MaxPluginForm Plugin { get; set; }
        string FileName { get; set; }
        int FilterIndex { get; }

        void Read(System.IO.FileStream stream);
        void Write(System.IO.FileStream stream);
        void Clear();

        void LoadUI();
        void SaveUI();

        void Import();
        void Export();
    }
}
