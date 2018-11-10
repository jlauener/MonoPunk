using System;
using System.IO;

public static class Log
{
	private static StreamWriter file;

	public static void UseFile(string name)
	{
		file = new StreamWriter(name);
	}

	public static void Debug(string log)
	{
		Write("DEBUG " + log);
	}

	public static void Warn(string log)
	{
		Write("WARN " + log);
	}

	public static void Warn(string log, Exception ex)
	{
		Warn(log + "\n" + ex);
	}

	public static void Error(string log)
	{
		Write("ERROR " + log);
	}

	public static void Error(string log, Exception ex)
	{
		Error(log + "\n" + ex);
	}

	public static void Fatal(string log)
	{
		Write("FATAL " + log);
	}

	private static void Write(string msg)
	{
		System.Diagnostics.Debug.WriteLine(msg);
		if (file != null)
		{
			file.WriteLine(msg);
			file.Flush();
		}
	}
}
