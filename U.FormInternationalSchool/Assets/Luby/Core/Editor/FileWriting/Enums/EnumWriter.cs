using System.Collections.Generic;
using System.IO;

namespace LubyLib.Core.Editor.FileWriting.Enums
{
	public class EnumWriter : FileWriter
	{

		public EnumWriter(string path) : base(path, "")
		{
			enumFields = new List<string>();
		}

		public EnumWriter(string path, string enumName, List<string> enumFields) : base(path, "")
		{
			this.enumName = enumName;
			this.enumFields = enumFields;
		}

		public List<string> enumFields;
		public string enumName;

		public override void Write()
		{
			data = "public enum " + enumName + "{\n\t";
			for (int i = 0; i < enumFields.Count; i++)
			{
				data += enumFields[i];
				if (i + 1 < enumFields.Count)
					data += ", ";
			}

			data += "\n}";

			base.Write();
		}

	}
}