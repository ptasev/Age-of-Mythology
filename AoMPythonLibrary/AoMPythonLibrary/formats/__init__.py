if __name__ == "__main__":
    from brg.brgfile import BRGFile
    b_file = BRGFile()
    b_file.read(r'C:\Programming\CSharp\AoMModding\AoMModelViewer\AoMModelViewer\bin\Debug\hi.json')
    print(b_file.meshes[0].vertices[0])
