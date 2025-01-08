using Godot;
using System;

namespace TheWizardCoder.Utils
{
    public static class HashUtils
    {
        public static byte[] CalculateHash(string fileName)
        {
            using FileAccess file = FileAccess.Open($"user://{fileName}.wand", FileAccess.ModeFlags.Read);
			byte[] buffer = file.GetBuffer((long)file.GetLength());
			file.Close();

			HashingContext hashing = new();
			hashing.Start(HashingContext.HashType.Sha256);
			hashing.Update(buffer);
			byte[] data = hashing.Finish();

			return data;
        }

        public static bool CompareHashes(byte[] a, byte[] b)
        {
            if (a == null && b != null)
			{
				return false;
			}
			else if (b == null && a != null)
			{
				return false;
			}

			if (a.Length != b.Length)
			{
				return false;
			}

			for (int i = 0; i < b.Length; i++)
			{
				var b1 = a[i];
				var b2 = b[i];
				if (b1 != b2)
				{
					return false;
				}
			}

			return true;
        }


    }
}