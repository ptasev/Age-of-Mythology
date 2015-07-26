namespace AoMEngineLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AoMEngineLibrary.Graphics.Model;

    public interface IModelMaxUi
    {
        MaxPluginForm Plugin { get; set; }
        string FileName { get; set; }
        int FilterIndex { get; }

        void Read(System.IO.FileStream stream);
        void Write(System.IO.FileStream stream);
        void Clear();

        void LoadUi();
        void SaveUi();

        void Import();
        void Export();
    }
}
