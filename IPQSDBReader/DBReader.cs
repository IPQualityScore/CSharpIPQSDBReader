
namespace IPQSDBReader
{
    public class DBReader
    {
        public FileReader Open(String file)
        {
            FileReader r = new FileReader
            {
                Valid = false,
                Handler = new FileStream(file, FileMode.Open, FileAccess.Read)
            };

            Bitmask bm = Bitmask.Create(r.Handler.ReadByte());
            r.BinaryData = bm.Has(Bitmask.BinaryData);
             if (bm.Has(Bitmask.IPv4Map))
             {
                 r.Valid = true;
                 r.IPv6 = false;
             }

             if (bm.Has(Bitmask.IPv6Map))
             {
                 r.Valid = true;
                 r.IPv6 = true;
             }

            r.BlacklistFile = bm.Has(Bitmask.BlacklistData);

             if (!r.Valid)
             {
                 throw new IOException("Invalid file format, invalid first byte, EID 1.");
             }

            if (Bitmask.Create(r.Handler.ReadByte()).Has(Bitmask.ReaderVersion) == false)
            {
                throw new IOException("Invalid file version, invalid header bytes, EID 1.");
            }

            r.TreeStart = Utility.uVarInt(r.Readbytes(3));

            if (r.TreeStart == 0) 
            {
                throw new IOException("Invalid file format, invalid record bytes, EID 2.");
            }

            r.RecordBytes = Utility.uVarInt(r.Readbytes(2));
            if (r.RecordBytes == 0)
            {
                throw new IOException("Invalid file format, invalid record bytes, EID 3.");
            }
            
            r.TotalBytes = Utility.toUnsignedInt(r.Readbytes(4));
            if (r.TotalBytes == 0)
            {
                throw new IOException("Invalid file format, EID 4.");
            }

            uint collen = r.TreeStart - 11;
            byte[] columns  = r.Readbytes((int)collen);

            uint totalcols = (r.TreeStart - 11) / 24;

            if (totalcols == 0)
            {
                throw new IOException("File does not appear to be valid, no column data found. EID: 5");
            }

            for (int i = 0; i < totalcols; i++)
            {
                byte[] descriptionraw = Utility.copyOfRange(columns, (uint)(i * 24), (uint)(((i + 1) * 24)-2));
                Column c = new Column();
                c.Name = Utility.SBArrayToString(descriptionraw);
                c.Type = Bitmask.Create((int)Utility.toUnsignedInt(columns[((i + 1) * 24) - 1]));

                r.Columns.Add(c);
            }

            uint rb = (uint)r.Read(1)[0];
            if (Bitmask.Create((int)rb).Has(Bitmask.TreeData) == false)
            {
                throw new IOException("File does not appear to be valid, bad binary tree. EID: 6");
            }

            byte[] treelength = r.Readbytes(4);
            r.TreeEnd = r.TreeStart + Utility.toUnsignedInt(treelength);

            if (r.TreeEnd == 0)
            {
                throw new IOException("File does not appear to be valid, tree size is too small. EID: 7");
            }

            return r;
        }
    }
}