namespace AoMEngineLibrary.Graphics.Grn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnFile
    {

        public GrnFile()
        {

        }

        public void Read(System.IO.Stream stream)
        {
            using (GrnBinaryReader reader = new GrnBinaryReader(stream))
            {
                reader.ReadBytes(64);

                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32(); // should be FileDirectory
                GrnNode mainNode = GrnNode.ReadByNodeType(reader, null, nodeType);

                mainNode.CreateFolder(@"C:\Users\Petar\Desktop\Nieuwe map (3)\TestFileDirs", 0);

                //Int32 numString = reader.ReadInt32();
                //Int32 stringDataLength = reader.ReadInt32();

                //this.Strings = new string[numString];
                //for (int i = 0; i < numString; i++)
                //{
                //    this.Strings[i] = reader.ReadString();
                //}

                //reader.ReadBytes((-stringDataLength) & 3); // padding
            }
        }
    }
}
