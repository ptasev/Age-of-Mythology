using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgDummyCollection : IList<BrgDummy>, IReadOnlyList<BrgDummy>, IList
    {
        private readonly SortedList<BrgDummyType, List<BrgDummy>> _dummyEntries;
        private byte _unused;

        public byte Version { get; private set; }

        /// <inheritdoc/>
        public int Count => _dummyEntries.Values.Sum(x => x.Count);

        /// <inheritdoc/>
        bool ICollection<BrgDummy>.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => this;

        /// <inheritdoc/>
        public BrgDummy this[int index]
        {
            get
            {
                var (Dummies, LocalIndex) = GetLocalIndex(index);
                return Dummies[LocalIndex];
            }
            set
            {
                var (Dummies, LocalIndex) = GetLocalIndex(index);
                Dummies[LocalIndex] = value;
            }
        }

        /// <inheritdoc/>
        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));

                this[index] = (BrgDummy)value;
            }
        }

        public BrgDummyCollection()
        {
            Version = 1;
            _dummyEntries = new SortedList<BrgDummyType, List<BrgDummy>>();
        }
        public BrgDummyCollection(int capacity)
        {
            Version = 1;
            _dummyEntries = new SortedList<BrgDummyType, List<BrgDummy>>(capacity);
        }

        public void Read(BrgBinaryReader reader, ushort meshVersion, ushort oldObjectCount, ushort oldEntryCount)
        {
            Clear();

            // Read header
            var dummyObjects = oldObjectCount;
            var dummyEntries = oldEntryCount;
            if (meshVersion >= 19)
            {
                dummyObjects = reader.ReadUInt16();
                dummyEntries = reader.ReadUInt16();

                Version = reader.ReadByte();
                _unused = reader.ReadByte();

                if (Version is not 1 and not 2)
                {
                    throw new NotSupportedException($"Reading dummy object version {Version} is not supported.");
                }
            }

            var halfVectors = meshVersion >= 22 && Version == 1;

            // Read object data
            var upVecs = new Vector3[dummyObjects];
            for (var i = 0; i < upVecs.Length; ++i)
            {
                upVecs[i] = reader.ReadVector3D(halfVectors);
            }

            var fwdVecs = new Vector3[dummyObjects];
            for (var i = 0; i < upVecs.Length; ++i)
            {
                fwdVecs[i] = reader.ReadVector3D(halfVectors);
            }

            var rightVecs = new Vector3[dummyObjects];
            for (var i = 0; i < upVecs.Length; ++i)
            {
                rightVecs[i] = reader.ReadVector3D(halfVectors);
            }

            var posVecs = new Vector3[dummyObjects];
            for (var i = 0; i < upVecs.Length; ++i)
            {
                posVecs[i] = reader.ReadVector3D(halfVectors);
            }

            var minVecs = new Vector3[dummyObjects];
            for (var i = 0; i < upVecs.Length; ++i)
            {
                minVecs[i] = reader.ReadVector3D(halfVectors);
            }

            var maxVecs = new Vector3[dummyObjects];
            for (var i = 0; i < upVecs.Length; ++i)
            {
                maxVecs[i] = reader.ReadVector3D(halfVectors);
            }

            // Read dummy entries
            var dummyEntryCounts = new byte[dummyEntries];
            for (var i = 0; i < dummyEntryCounts.Length; ++i)
            {
                dummyEntryCounts[i] = reader.ReadByte();
                _ = reader.ReadBytes(3); // padding
                _ = reader.ReadInt32(); // dummy indices array ptr
            }

            // Read dummy object index for each entry and add dummy
            for (var i = 0; i < dummyEntryCounts.Length; ++i)
            {
                var dummyEntryCount = dummyEntryCounts[i];
                for (var j = 0; j < dummyEntryCount; ++j)
                {
                    // Read object index
                    var index = reader.ReadByte();

                    // Create dummy object
                    var dummy = new BrgDummy()
                    {
                        Type = (BrgDummyType)i,
                        Up = upVecs[index],
                        Forward = fwdVecs[index],
                        Right = rightVecs[index],
                        Position = posVecs[index],
                        BoundingBoxMin = minVecs[index],
                        BoundingBoxMax = maxVecs[index]
                    };
                    Add(dummy, false);
                }
            }
        }

        public void Write(BrgBinaryWriter writer, ushort meshVersion)
        {
            // prefer Version 2 for meshVersion 23+
            Version = (byte)(meshVersion >= 23 ? 2 : 1);
            var halfVectors = Version == 1;

            // Write header
            var keys = _dummyEntries.Keys;
            var dummyObjects = (ushort)Count;
            var dummyEntries = (ushort)(keys.LastOrDefault() + 1);
            writer.Write(dummyObjects);
            writer.Write(dummyEntries);
            writer.Write(Version);
            writer.Write(_unused);

            // Write object data
            foreach (var key in keys)
            {
                var dummies = _dummyEntries[key];
                foreach (var dummy in dummies)
                {
                    writer.WriteVector3D(dummy.Up, halfVectors);
                }
            }

            foreach (var key in keys)
            {
                var dummies = _dummyEntries[key];
                foreach (var dummy in dummies)
                {
                    writer.WriteVector3D(dummy.Forward, halfVectors);
                }
            }

            foreach (var key in keys)
            {
                var dummies = _dummyEntries[key];
                foreach (var dummy in dummies)
                {
                    writer.WriteVector3D(dummy.Right, halfVectors);
                }
            }

            foreach (var key in keys)
            {
                var dummies = _dummyEntries[key];
                foreach (var dummy in dummies)
                {
                    writer.WriteVector3D(dummy.Position, halfVectors);
                }
            }

            foreach (var key in keys)
            {
                var dummies = _dummyEntries[key];
                foreach (var dummy in dummies)
                {
                    writer.WriteVector3D(dummy.BoundingBoxMin, halfVectors);
                }
            }

            foreach (var key in keys)
            {
                var dummies = _dummyEntries[key];
                foreach (var dummy in dummies)
                {
                    writer.WriteVector3D(dummy.BoundingBoxMax, halfVectors);
                }
            }

            // Write dummy entries
            for (var i = 0; i < dummyEntries; ++i)
            {
                var key = (BrgDummyType)i;
                if (_dummyEntries.TryGetValue(key, out var dummies))
                {
                    writer.Write((byte)dummies.Count);
                    writer.Write((byte)0); writer.Write((byte)0); writer.Write((byte)0); // Padding
                    writer.Write(0); // just write as null pointer
                }
                else
                {
                    // Write out 0 for count, and 0 for index pointer
                    writer.Write((ulong)0);
                }
            }

            // Write object indices
            for (var i = 0; i < dummyObjects; ++i)
            {
                writer.Write((byte)i);
            }
        }

        /// <inheritdoc/>
        public void Add(BrgDummy item) => Add(item, true);

        public void Add(BrgDummy item, bool validateDummyCount)
        {
            if (!_dummyEntries.TryGetValue(item.Type, out var dummies))
            {
                dummies = new List<BrgDummy>();
                _dummyEntries.Add(item.Type, dummies);
            }

            // Check if we're at max objects for this dummy type
            if (validateDummyCount)
            {
                // turns out this is only a warning in the AoM code, and many files violate this number
                // we don't want to prevent opening these files if this rule is violated

                var typeInfo = item.Type.GetInfo();
                if (dummies.Count >= typeInfo.Max)
                {
                    throw new InvalidOperationException($"There can only be a max of {typeInfo.Max} dummy objects of type {typeInfo.Type}.");
                }
            }

            dummies.Add(item);
        }

        /// <inheritdoc/>
        public void Clear() => _dummyEntries.Clear();

        /// <inheritdoc/>
        public IEnumerator<BrgDummy> GetEnumerator() => _dummyEntries.Values.SelectMany(x => x).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public bool Contains(BrgDummy item)
        {
            if (!_dummyEntries.TryGetValue(item.Type, out var dummies))
            {
                return false;
            }

            return dummies.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(BrgDummy[] array, int arrayIndex)
        {
            foreach (var kvp in _dummyEntries)
            {
                kvp.Value.CopyTo(array, arrayIndex);
                arrayIndex += kvp.Value.Count;
            }
        }

        /// <inheritdoc/>
        public bool Remove(BrgDummy item)
        {
            if (!_dummyEntries.TryGetValue(item.Type, out var dummies))
            {
                return false;
            }

            var ret = dummies.Remove(item);
            if (dummies.Count <= 0)
            {
                _ = _dummyEntries.Remove(item.Type);
            }

            return ret;
        }

        /// <inheritdoc/>
        public int IndexOf(BrgDummy item)
        {
            var index = 0;
            foreach (var kvp in _dummyEntries)
            {
                if (kvp.Key != item.Type)
                {
                    index += kvp.Value.Count;
                    continue;
                }

                var locIndex = kvp.Value.IndexOf(item);
                if (locIndex == -1)
                {
                    return -1;
                }

                index += locIndex;
                return index;
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList<BrgDummy>.Insert(int index, BrgDummy item) => throw new NotSupportedException("The operation is not supported for this collection.");

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            var (Dummies, LocalIndex) = GetLocalIndex(index);
            Dummies.RemoveAt(LocalIndex);
        }

        /// <inheritdoc/>
        int IList.Add(object? value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            Add((BrgDummy)value);

            return Count - 1;
        }

        /// <inheritdoc/>
        bool IList.Contains(object? value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((BrgDummy)value!);
            }
            return false;
        }

        /// <inheritdoc/>
        int IList.IndexOf(object? value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((BrgDummy)value!);
            }
            return -1;
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object? value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            ((IList<BrgDummy>)this).Insert(index, (BrgDummy)value);
        }

        /// <inheritdoc/>
        void IList.Remove(object? value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((BrgDummy)value!);
            }
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));

            CopyTo((BrgDummy[])array, index);
        }

        private (List<BrgDummy> Dummies, int LocalIndex) GetLocalIndex(int globalIndex)
        {
            foreach (var dummies in _dummyEntries.Values)
            {
                if (globalIndex >= dummies.Count)
                {
                    globalIndex -= dummies.Count;
                    continue;
                }

                return (dummies, globalIndex);
            }

            throw new ArgumentOutOfRangeException("Index is invalid for the collection");
        }

        private static bool IsCompatibleObject(object? value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            return (value is BrgDummy) || (value == null && default(BrgDummy) == null);
        }
    }
}
