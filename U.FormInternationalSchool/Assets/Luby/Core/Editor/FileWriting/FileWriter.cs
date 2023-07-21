using System.IO;

namespace LubyLib.Core.Editor.FileWriting
{
	public class FileWriter
	{
		public string path;
		public string data;

		public FileWriter(string path, string data)
		{
			this.path = path;
			this.data = data;
		}

		public virtual void Write()
		{
			StreamWriter streamWriter = new StreamWriter(this.path);
			streamWriter.Write(this.data);
			streamWriter.Flush();
			streamWriter.Close();
		}
	}
}